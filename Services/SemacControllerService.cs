using SemacV14.Service;
using SemacV14.Entity;
using SemacV14.Request;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using System.Data;
using Township_API.Hubs;
using Timer = System.Threading.Timer;

namespace Township_API.Services
{
    public class SemacControllerService : BackgroundService
    {
        private readonly ILogger<SemacControllerService> _logger;
        private readonly string _connString;
        private readonly IHubContext<TransactionHub> _hub;

        private DataTable _terminalTable;
        private readonly object _tableLock = new object();
        private Timer _fullSyncTimer;

        public SemacControllerService(ILogger<SemacControllerService> logger, IConfiguration config, IHubContext<TransactionHub> hub)
        {
            _logger = logger;
            _connString = config.GetConnectionString("DefaultConnection")
    ?? throw new Exception("Connection string 'DefaultConnection' not found in appsettings.json");
            _hub = hub;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            LoadTerminalTable();

            CurrentService.OnConnected += OnConnected;
            CurrentService.OnDisconnected += OnDisconnected;
            CurrentService.OnRealtimeTransaction += HandleRealtime;
            CurrentService.OffLineLogTransaction += HandleRealtime;

            // Pass our own error handler so the DLL's internal catch block
            // never calls MessageBox.Show() (which needs System.Windows.Forms
            // and would crash/hang in a non-UI process).
            CurrentService.StartListen("0.0.0.0", 2000, ex =>
            {
                _logger.LogError(ex, "SemacV14 internal listen error");
            });

            //_fullSyncTimer = new Timer(_ => RunFullSync(), null, TimeSpan.FromMinutes(1), TimeSpan.FromHours(12));
            _fullSyncTimer = new Timer(_ => RunFullSync(), null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        private void LoadTerminalTable()
        {
            _terminalTable = new DataTable();
            foreach (var c in new[] { "ChannelID", "Status", "TerminalID", "IPAddress" })
                _terminalTable.Columns.Add(c);

            using var con = new SqlConnection(_connString);
            using var cmd = new SqlCommand("SELECT code, readerip FROM tblReader WHERE isActive=1 AND IsDeleted=0", con);
            con.Open();
            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);

            lock (_tableLock)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var row = _terminalTable.NewRow();
                    row["ChannelID"] = "";
                    row["Status"] = "Disconnected";
                    row["TerminalID"] = dr["code"].ToString();
                    row["IPAddress"] = dr["readerip"].ToString();
                    _terminalTable.Rows.Add(row);
                }
            }
        }

        private string ResolveByIP(string ip)
        {
            lock (_tableLock)
                foreach (DataRow r in _terminalTable.Rows)
                    if (r["IPAddress"].ToString() == ip) return r["TerminalID"].ToString();
            return null;
        }

        private string ResolveByChannel(string channelID)
        {
            lock (_tableLock)
                foreach (DataRow r in _terminalTable.Rows)
                    if (r["ChannelID"].ToString() == channelID) return r["TerminalID"].ToString();
            return null;
        }

        private void OnConnected(TerminalEntity en)
        {
            string resolvedCode = ResolveByIP(en.IPAddress) ?? en.TerminalID.ToString();
            lock (_tableLock)
            {
                foreach (DataRow r in _terminalTable.Rows)
                {
                    if (r["IPAddress"].ToString() == en.IPAddress)
                    {
                        r["ChannelID"] = en.ChannelID;
                        r["Status"] = "On Line";
                        break;
                    }
                }
            }
            CallUpdateReaderStatus(resolvedCode, "CONNECTED", en.ToMyString);
            _hub.Clients.All.SendAsync("ControllerStatus", new { code = resolvedCode, ip = en.IPAddress, status = "On Line" });
        }

        private void OnDisconnected(TerminalEntity en)
        {
            string resolvedCode = ResolveByChannel(en.ChannelID) ?? en.TerminalID.ToString();
            lock (_tableLock)
            {
                foreach (DataRow r in _terminalTable.Rows)
                {
                    if (r["ChannelID"].ToString() == en.ChannelID)
                    {
                        r["Status"] = "Disconnected";
                        r["ChannelID"] = "";
                        break;
                    }
                }
            }
            CallUpdateReaderStatus(resolvedCode, "DISCONNECTED", en.ToMyString);
            _hub.Clients.All.SendAsync("ControllerStatus", new { code = resolvedCode, status = "Disconnected" });
        }

        private void HandleRealtime(ExecuteArgz ea)
        {
            if (ea.EntityList == null) return;
            string resolvedCode = ResolveByChannel(ea.ChannelID);

            foreach (DoorLogEntity en in ea.EntityList)
            {
                string codeToUse = resolvedCode ?? en.TerminalID.ToString();
                // Fixed: was hardcoded "ONLINE" — must be the actual swiped CardNo,
                // same as desktop app's DBDal.UpdateControllerStatus(codeToUse, En.CardNo, ...)
                CallUpdateReaderStatus(codeToUse, en.CardNo, en.ToMyString);
                _hub.Clients.All.SendAsync("LiveTransaction", new
                {
                    code = codeToUse,
                    cardNo = en.CardNo,
                    detail = en.ToMyString,
                    time = DateTime.Now
                });
            }
        }

        private void CallUpdateReaderStatus(string code, string status, string logMessage)
        {
            try
            {
                using var con = new SqlConnection(_connString);
                using var cmd = new SqlCommand("UpdateReaderStatus", con) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@code", code);
                cmd.Parameters.AddWithValue("@CurrentStatus", status);
                cmd.Parameters.AddWithValue("@LogMessage", logMessage ?? "");
                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex) { _logger.LogError(ex, "UpdateReaderStatus call failed"); }
        }

        // ================= AUTOMATIC (1 per min pending sync, full resync every 12h) =================
        // ================= AUTOMATIC (1 per min pending sync, full resync every 12h) =================
        // ================= AUTOMATIC (1 per min pending sync, full resync every 12h) =================
        private void RunFullSync() => _ = RunFullSyncAsync();

        private async Task RunFullSyncAsync()
        {
            await PushCardHoldersAsync(null);
            await PushControllerDeactivationsAsync(null);
        }

        // ================= MANUAL (called from controller) =================

        public bool IsOnline(string readerCode)
        {
            lock (_tableLock)
                foreach (DataRow r in _terminalTable.Rows)
                    if (r["TerminalID"].ToString() == readerCode) return r["Status"].ToString() == "On Line";
            return false;
        }

        public string GetChannelId(string readerCode)
        {
            lock (_tableLock)
                foreach (DataRow r in _terminalTable.Rows)
                    if (r["TerminalID"].ToString() == readerCode) return r["ChannelID"].ToString();
            return null;
        }

        // Manual "Access Log Upload": push all pending card-holders (optionally filtered to one reader)
        // Manual "Access Log Upload": push all pending card-holders (optionally filtered to one reader)
        // Manual/Automatic "Access Log Upload": push only PENDING card-holders
        // (never synced to this reader yet, or previously deactivated and now valid again)
        public async Task<int> PushCardHoldersAsync(string onlyReaderCode)
        {
            int count = 0;
            DataTable dt;
            try
            {
                using var con = new SqlConnection(_connString);
                using var cmd = new SqlCommand(@"
                    SELECT ch.UserID, ch.CardNo, ch.UserName, ch.ReaderCode
                    FROM vwIDS_ADDCardHolderToControler ch
                    LEFT JOIN tblControllerSync cs
                        ON cs.IDNumber = ch.UserID AND cs.ReaderCode = ch.ReaderCode
                    WHERE (cs.id IS NULL OR cs.Status = 'B')
                      AND (@readerCode IS NULL OR ch.ReaderCode = @readerCode)", con);
                cmd.Parameters.AddWithValue("@readerCode", (object)onlyReaderCode ?? DBNull.Value);
                con.Open();
                dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
            }
            catch (Exception ex) { _logger.LogError(ex, "PushCardHolders: pending-upload query failed"); return 0; }

            foreach (DataRow dr in dt.Rows)
            {
                string readerCode = dr["ReaderCode"].ToString();
                if (!IsOnline(readerCode)) continue;

                string channelID = GetChannelId(readerCode);
                if (string.IsNullOrEmpty(channelID)) continue;
                if (!int.TryParse(readerCode, out int terminalId)) continue;

                string userIdStr = dr["UserID"].ToString();

                try
                {
                    var req = new RegisterModifyUserDataRequest(terminalId)
                    {
                        UserID = (uint)Convert.ToInt64(userIdStr),
                        EnabledStatus = true,
                        UserName = dr["UserName"].ToString(),
                        CardNo = dr["CardNo"].ToString(),
                        EmployeeID = "",
                        UserType = (SemacV14.Define.UserType)0,
                        BypassTimeZoneLevel = (SemacV14.Define.BypassTimeZoneLevel)1,
                        OverWrite = true,
                        Group01 = 0b1,
                        Door01TZ = 1
                    };

                    var ea = new ExecuteArgz(channelID, req);
                    var actor = new Actor(ea);
                    ea = await actor.SendAndReceiveAsync();

                    if (ea.IsCompleted)
                    {
                        count++;
                        await MarkControllerSyncedAsync(userIdStr, readerCode, 'A');
                    }
                    else
                    {
                        _logger.LogWarning("[{code}] push failed for UserID {uid}: {err}", readerCode, userIdStr, ea.ErrorMessage);
                    }
                }
                catch (Exception ex) { _logger.LogError(ex, "PushCardHolders user-level failure"); }
            }
            return count;
        }

        // Automatic "Deactivate": find users who were previously pushed as Active on a
        // controller (tblControllerSync.Status='A') but are no longer valid there
        // (blocked / revoked-expired / lost-damage / expired validity) per
        // vwIDS_ADDCardHolderToControler, and turn them off on the controller.
        public async Task<int> PushControllerDeactivationsAsync(string onlyReaderCode)
        {
            int count = 0;
            DataTable dt;
            try
            {
                using var con = new SqlConnection(_connString);
                using var cmd = new SqlCommand(@"
                    SELECT cs.IDNumber AS UserID, cs.ReaderCode
                    FROM tblControllerSync cs
                    WHERE cs.Status = 'A'
                      AND (@readerCode IS NULL OR cs.ReaderCode = @readerCode)
                      AND NOT EXISTS (
                          SELECT 1 FROM vwIDS_ADDCardHolderToControler ch
                          WHERE ch.UserID = cs.IDNumber AND ch.ReaderCode = cs.ReaderCode
                      )", con);
                cmd.Parameters.AddWithValue("@readerCode", (object)onlyReaderCode ?? DBNull.Value);
                con.Open();
                dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
            }
            catch (Exception ex) { _logger.LogError(ex, "PushControllerDeactivations: pending-deactivate query failed"); return 0; }

            foreach (DataRow dr in dt.Rows)
            {
                string readerCode = dr["ReaderCode"].ToString();
                string userIdStr = dr["UserID"].ToString();
                if (!IsOnline(readerCode)) continue;
                if (!uint.TryParse(userIdStr, out uint userId)) continue;

                try
                {
                    var r = await SaveUserAsync(readerCode, userId, "", "", "", activate: false);
                    if (r.Success)
                    {
                        count++;
                        await MarkControllerSyncedAsync(userIdStr, readerCode, 'B');
                    }
                    else
                    {
                        _logger.LogWarning("[{code}] deactivate failed for UserID {uid}: {err}", readerCode, userIdStr, r.ErrorMessage);
                    }
                }
                catch (Exception ex) { _logger.LogError(ex, "PushControllerDeactivations user-level failure"); }
            }
            return count;
        }

        // Upsert into tblControllerSync (own tracking table, independent of desktop's tblIDSync)
        private async Task MarkControllerSyncedAsync(string idNumber, string readerCode, char status)
        {
            try
            {
                using var con = new SqlConnection(_connString);
                using var cmd = new SqlCommand(@"
                    IF EXISTS (SELECT 1 FROM tblControllerSync WHERE IDNumber=@id AND ReaderCode=@rid)
                        UPDATE tblControllerSync SET Status=@status, SyncedOn=GETDATE() WHERE IDNumber=@id AND ReaderCode=@rid
                    ELSE
                        INSERT INTO tblControllerSync (IDNumber, ReaderCode, Status, SyncedOn) VALUES (@id, @rid, @status, GETDATE())", con);
                cmd.Parameters.AddWithValue("@id", idNumber);
                cmd.Parameters.AddWithValue("@rid", readerCode);
                cmd.Parameters.AddWithValue("@status", status.ToString());
                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex) { _logger.LogError(ex, "MarkControllerSyncedAsync failed for {id}/{rid}", idNumber, readerCode); }
        }

        // Manual "Get User": fetch a single user's data straight from the controller
        // Manual "Get User": fetch a single user's data straight from the controller
        public async Task<(bool Success, string ErrorMessage, uint UserID, string UserName, string CardNo, string EmployeeID, bool Status)> GetUserAsync(string readerCode, uint userId)
        {
            string channelID = GetChannelId(readerCode);
            if (string.IsNullOrEmpty(channelID) || !int.TryParse(readerCode, out int terminalId))
                return (false, "Controller offline", 0, "", "", "", false);

            var req = new GetUserDataRequest(terminalId, userId);
            var ea = new ExecuteArgz(channelID, req);
            var actor = new Actor(ea);
            ea = await actor.SendAndReceiveAsync();

            if (!ea.IsCompleted) return (false, ea.ErrorMessage, 0, "", "", "", false);

            var en = (GetUserDataEntity)ea.CallbackEntity;
            return (true, "", en.UserID, en.UserName, en.CardNo, en.EmployeeID, en.Status);
        }

        // Manual "Save/Register User": add/update/activate/deactivate one user on the controller
        // Manual "Save/Register User": add/update/activate/deactivate one user on the controller
        public async Task<(bool Success, string ErrorMessage)> SaveUserAsync(string readerCode, uint userId, string userName, string cardNo, string employeeId, bool activate)
        {
            string channelID = GetChannelId(readerCode);
            if (string.IsNullOrEmpty(channelID) || !int.TryParse(readerCode, out int terminalId))
                return (false, "Controller offline");

            var req = new RegisterModifyUserDataRequest(terminalId) { UserID = userId };
            if (activate)
            {
                req.EnabledStatus = true;
                req.UserName = userName;
                req.CardNo = cardNo;
                req.EmployeeID = employeeId;
            }
            else
            {
                req.EnabledStatus = false;
            }
            req.UserType = (SemacV14.Define.UserType)0;
            req.BypassTimeZoneLevel = (SemacV14.Define.BypassTimeZoneLevel)1;
            req.OverWrite = true;
            req.Group01 = 0b1;
            req.Door01TZ = 1;

            var ea = new ExecuteArgz(channelID, req);
            var actor = new Actor(ea);
            ea = await actor.SendAndReceiveAsync();

            return (ea.IsCompleted, ea.IsCompleted ? "" : ea.ErrorMessage);
        }

        // Manual "Open Door" (emergency)
        // Manual "Open Door" (emergency)
        public async Task<(bool Success, string ErrorMessage)> OpenDoorAsync(string readerCode)
        {
            string channelID = GetChannelId(readerCode);
            if (string.IsNullOrEmpty(channelID) || !int.TryParse(readerCode, out int terminalId))
                return (false, "Controller offline");

            var req = new CommonRequest(terminalId, SemacV14.Define.CommandType.PulseOpenDoor);
            var ea = new ExecuteArgz(channelID, req);
            var actor = new Actor(ea);
            ea = await actor.SendAndReceiveAsync();

            return (ea.IsCompleted, ea.IsCompleted ? "" : ea.ErrorMessage);
        }

        // Manual "Access Log Download": pull attendance/entry-exit log from controller into tblInoutRegister
        public void DownloadAccessLog(string readerCode)
        {
            string channelID = GetChannelId(readerCode);
            if (string.IsNullOrEmpty(channelID) || !int.TryParse(readerCode, out int terminalId)) return;

            var req = new CommonRequest(terminalId, SemacV14.Define.CommandType.RetrievingAllEntryExitLog);
            var ea = new ExecuteArgz(channelID, req);
            var act = new Actor(ea);
            act.OnEntityDataArrival += HandleRealtime;   // same handler; UpdateReaderStatus SP inserts into tblInoutRegister
            act.Send();
        }
    }
}
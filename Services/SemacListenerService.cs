using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using Township_API.Models;

namespace Township_API.Services
{
    public class SemacListenerService
    {
        private readonly ILogger<SemacListenerService> _logger;
        private readonly string _connString;

        public SemacListenerService(ILogger<SemacListenerService> logger, IConfiguration config)
        {
            _logger = logger;
            _connString = config.GetConnectionString("constr")
                ?? throw new Exception("Connection string 'constr' not found in appsettings.json");
        }

        public PagedResult<SemacTransactionDto> GetRecentTransactions(int page, int pageSize)
        {
            var result = new PagedResult<SemacTransactionDto> { Page = page, PageSize = pageSize };
            try
            {
                using var con = new SqlConnection(_connString);
                con.Open();

                using (var countCmd = new SqlCommand("SELECT COUNT(*) FROM dbo.tblInoutRegister", con))
                {
                    result.TotalCount = (int)countCmd.ExecuteScalar();
                }

                using var cmd = new SqlCommand(@"
                    SELECT r.id, r.ReaderID, r.ReaderLocation, r.IDNumber, r.Accessmode, r.AccessTime, r.CSNNumber,
                           rd.readerip AS IPAddress
                    FROM dbo.tblInoutRegister r
                    LEFT JOIN dbo.tblReader rd ON rd.code = r.ReaderID
                    ORDER BY r.id DESC
                    OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY", con);
                cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);

                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    result.Data.Add(new SemacTransactionDto
                    {
                        Id = Convert.ToInt64(rdr["id"]),
                        ReaderID = rdr["ReaderID"].ToString(),
                        ReaderLocation = rdr["ReaderLocation"].ToString(),
                        IPAddress = rdr["IPAddress"] == DBNull.Value ? "" : rdr["IPAddress"].ToString(),
                        IDNumber = rdr["IDNumber"].ToString(),
                        AccessMode = rdr["Accessmode"].ToString(),
                        AccessTime = Convert.ToDateTime(rdr["AccessTime"]),
                        CSNNumber = rdr["CSNNumber"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetRecentTransactions failed");
            }
            return result;
        }
    }
}
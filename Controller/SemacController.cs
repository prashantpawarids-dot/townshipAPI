using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Township_API.Controllers
{
    [ApiController]
    [Route("api/semac")]
    public class SemacController : ControllerBase
    {
        private readonly IConfiguration _config;
        public SemacController(IConfiguration config) => _config = config;

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            var list = new List<object>();
            using var con = new SqlConnection(_config.GetConnectionString("constr"));
            using var cmd = new SqlCommand(@"
                SELECT r.code, r.readerip, r.readerLocation,
                       ISNULL(s.CurrentStatus,'Disconnected') AS CurrentStatus,
                       s.statuson
                FROM tblReader r
                LEFT JOIN tblReaderStatus s ON s.code = r.code
                WHERE r.isActive=1 AND r.IsDeleted=0", con);
            con.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new
                {
                    Code = rdr["code"].ToString(),
                    IP = rdr["readerip"].ToString(),
                    Location = rdr["readerLocation"].ToString(),
                    Status = rdr["CurrentStatus"].ToString(),
                    LastUpdated = rdr["statuson"] == DBNull.Value ? null : (DateTime?)rdr["statuson"]
                });
            }
            return Ok(list);
        }
    }
}
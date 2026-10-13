using System;
using System.Collections.Generic;

namespace Township_API.Models
{
    public class SemacTransactionDto
    {
        public long Id { get; set; }
        public string ReaderID { get; set; }
        public string ReaderLocation { get; set; }   // site name: Falcon / Skylark
        public string IPAddress { get; set; }
        public string IDNumber { get; set; }
        public string AccessMode { get; set; }        // IN / OUT
        public DateTime AccessTime { get; set; }
        public string CSNNumber { get; set; }
    }

    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public List<T> Data { get; set; } = new List<T>();
    }
}
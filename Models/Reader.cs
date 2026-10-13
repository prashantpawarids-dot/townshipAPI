using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Township_API.Models
{
    [Table("tblReader")]
    public class Reader
    {
        public int ID { get; set; }
        [Required]
        [MaxLength(10)]
        public string code { get; set; }
        [Required] 
        public string? readerip { get; set; }
        [Required]
        public string? readerport { get; set; }
        public string? readermode { get; set; } = "mifare";
        public string? readerLocation { get; set; } = "0";
        public string? readertype { get; set; } = "";
        public bool? isActive { get; set; } = true;
        public bool? IsDeleted { get; set; } = false;

        [NotMapped]
        public string? nrdName { get; set; } = null;
    }
}

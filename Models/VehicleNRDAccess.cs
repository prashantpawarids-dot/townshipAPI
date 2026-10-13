using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Township_API.Models
{
    [Table("tblVehicleNRDAccess")]
    public class VehicleNRDAccess
    {
        [Key]
        public int ID { get; set; }
        public int VehicleID { get; set; }
        public int NRDID { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [NotMapped]
        public string? NRDName { get; set; }

        public string? IsUploaded { get; set; } = "N";
    }
}
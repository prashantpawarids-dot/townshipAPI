using System;
using System.ComponentModel.DataAnnotations;

namespace Township_API.Models
{
        public class ANPR
    {
        [Key]
        public int messageid { get; set; }
        public bool auth { get; set; } = false; //register vehicle if is 
        public string frame { get; set; }
        public string image { get; set; }
        public bool is_entry { get; set; }
        public string number_plate { get; set; }
        public DateTime timestamp { get; set; }
        public string type { get; set; }
        public string client_name { get; set; }
        public string site { get; set; }
        public string site_name { get; set; }
        public int camera_id { get; set; }
        public int brand { get; set; }
        public string client { get; set; }
        public int source_id { get; set; }
        public string node_name { get; set; }
        public int node { get; set; }
        public int _class{ get; set;}
        public int color { get; set; }
        public string hashed_number_plate { get; set; }
        public int speed { get; set; }
        public string vehicle { get; set; }
        public int vtype { get; set; }

    }
}

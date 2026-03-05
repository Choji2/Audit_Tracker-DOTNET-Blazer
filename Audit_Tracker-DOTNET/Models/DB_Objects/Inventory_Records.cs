using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DB_objects
{ 
    public class Inventory_Records
    {
        [Key] public string ID { get; set; }
        [Required]public int ZoneID { get; set; }
        [Required] public int Status { get; set; }


        public string INVID{ get; set; }
        public Inventories INV { get; set; } = null!;

    }
}

using System.ComponentModel.DataAnnotations;

namespace Models.DB_objects
{
    public class Inventories
    {
        [Key] public string ID { get; set; }
        [Required]public string desc {get; set;}

        [Required] 
        public ICollection<Inventory_Records> Records = new List<Inventory_Records>();
      
    }

}

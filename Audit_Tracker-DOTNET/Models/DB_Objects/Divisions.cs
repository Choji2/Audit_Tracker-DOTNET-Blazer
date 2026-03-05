using System.ComponentModel.DataAnnotations;

namespace Models.DB_objects
{
    public class Divisions
    {

        [Key]public int ID { get; set; }
        [Required]public string Div_Code { get; set; }
        [Required]public string Desc { get; set; }
        [Required] public ICollection<Div_Zones> Div_Items { get; set; } = new List<Div_Zones>();

    }

}


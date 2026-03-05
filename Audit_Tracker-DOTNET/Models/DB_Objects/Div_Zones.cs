using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DB_objects
{ 
    public class Div_Zones
    {
        [Key] public int ID { get; set; }
        [Required] public string Zone_Code { get; set; }
        [Required] public string Desc { get; set; }

        public int DivID { get; set; }
        public Divisions? Div { get; set; } = null;



    }
}

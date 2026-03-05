using System.ComponentModel.DataAnnotations;

namespace AAP_Authentication
{
    public class Roles
    {
        [Key] public int ID { get; set; }
        [Required] public string Code { get; set; }
        [Required] public string Desc { get; set; }    
      
    }



}

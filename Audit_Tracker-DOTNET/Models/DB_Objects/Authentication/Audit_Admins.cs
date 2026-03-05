using AAP_Inventory_Zone_Tracker.Components.Pages.Division_Components;
using System.ComponentModel.DataAnnotations;

namespace AAP_Authentication
{
    public class Audit_Admins
    {
        [Required][Key] public string Username { get; set; }
        [Required] public string Name { get; set; }
        [Required] public int RoleID { get; set; }
        public Roles Role { get; set; } = null!;


    }
}

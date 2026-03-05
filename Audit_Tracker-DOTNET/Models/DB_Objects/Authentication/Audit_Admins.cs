using Audit_Tracker_Blazor.Components.Pages.Division_Components;
using System.ComponentModel.DataAnnotations;

namespace Authentication
{
    public class Audit_Admins
    {
        [Required][Key] public string Username { get; set; }
        [Required] public string Name { get; set; }
        [Required] public int RoleID { get; set; }
        public Roles Role { get; set; } = null!;


    }
}

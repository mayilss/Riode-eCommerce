using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Riode.WebUI.Models.Entities.Membership
{
    public class RiodeUser : IdentityUser<int>
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
    }
}

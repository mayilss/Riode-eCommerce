using Riode.WebUI.AppCode.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace Riode.WebUI.Models.Entities
{
    public class Brand : BaseEntity
    {
        [Required(ErrorMessage = "Can't be empty")]
        public string Name { get; set; }
    }
}

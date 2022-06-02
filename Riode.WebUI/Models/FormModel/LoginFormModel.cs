using System.ComponentModel.DataAnnotations;

namespace Riode.WebUI.Models.FormModel
{
    public class LoginFormModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

using Riode.WebUI.AppCode.Infrastructure;
using System;

namespace Riode.WebUI.Models.Entities
{
    public class Subscribe : BaseEntity
    {
        public string Email { get; set; }
        public bool EmailSent { get; set; } = false;
        public DateTime? AppliedDate { get; set; }
    }
}

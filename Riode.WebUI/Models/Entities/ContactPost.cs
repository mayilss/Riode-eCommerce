using Riode.WebUI.AppCode.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace Riode.WebUI.Models.Entities
{
    public class ContactPost : BaseEntity
    {
        [Required(ErrorMessage = "Can not be left empty!")]
        public string Message { get; set; }
        [Required(ErrorMessage = "Can not be left empty!")]
        public string FullName { get; set; }
        [EmailAddress(ErrorMessage ="Please enter a proper email address!")]
        [Required(ErrorMessage ="Can not be left empty!")]
        public string Email { get; set; }

        public DateTime? AnswerDate { get; set; }
        public string AnswerMessage { get; set; }
        public int? AnsweredById { get; set; }

    }
}

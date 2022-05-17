using Riode.WebUI.AppCode.Infrastructure;

namespace Riode.WebUI.Models.Entities
{
	public class Faq : BaseEntity
	{
		public string Question { get; set; }
		public string Answer { get; set; }
	}
}

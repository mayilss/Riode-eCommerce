using Riode.WebUI.Models.Entities;
using System.Collections;
using System.Collections.Generic;

namespace Riode.WebUI.Models.ViewModels
{
	public class SinglePostViewModel
	{
		public BlogPost Post { get; set; }
		public IEnumerable<BlogPost> RelatedPosts { get; set; }
	}
}

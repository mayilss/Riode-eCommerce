using Riode.WebUI.AppCode.Infrastructure;
using System.Collections.Generic;

namespace Riode.WebUI.Models.Entities
{
    public class BlogPost : BaseEntity
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string ImagePath { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<BlogPostTag> TagCloud { get; set; }

    }
}

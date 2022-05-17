using Microsoft.AspNetCore.Html;
using Riode.WebUI.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Riode.WebUI.AppCode.Extensions
{
    public static partial class Extension
    {
        public static HtmlString GetCategoriesRaw(this List<Category> categories)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<ul class=\"widget-body filter-items search-ul\">");
            foreach (var category in categories.Where(c => c.ParentId == null))
            {
                AppendCategory(category, sb);
            }
            sb.Append("</ul>");
            return new HtmlString(sb.ToString());
        }
        static void AppendCategory(Category category, StringBuilder sb)
        {
            bool hasChild = category.Children.Any();
            sb.Append($"<li {(hasChild?"class=\"with-ul\"":"")}><a href=\"#\">{category.Name}");
            if (hasChild)
            {
                sb.Append("<i class=\"fas fa-chevron-down\"></i>");
            }

            sb.Append("</a>");
            if (hasChild)
            {
                sb.Append("<ul>");
                foreach(var item in category.Children)
                {
                    AppendCategory(item, sb);
                }
                sb.Append("</ul>");
            }
            sb.Append("</li>");
        }
         public static IEnumerable<Category> GetChildren(this Category category)
        {
            if(category.ParentId != null)
            {
                yield return category;
            }
            foreach (var child in category.Children.SelectMany(c=>c.GetChildren()))
            {
                yield return child;
            }
        }
    }
}

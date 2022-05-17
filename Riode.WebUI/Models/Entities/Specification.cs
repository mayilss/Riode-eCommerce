using Riode.WebUI.AppCode.Infrastructure;
using System.Collections.Generic;

namespace Riode.WebUI.Models.Entities
{
    public class Specification : BaseEntity
    {
        public string Name { get; set; }
        public virtual ICollection<ProductSpecification> Specifications { get; set; }
    }
}

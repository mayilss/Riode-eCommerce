using Riode.WebUI.Models.Entities;
using System.Collections.Generic;

namespace Riode.WebUI.Models.ViewModels
{
    public class ShopIndexViewModel
    {
        public List<Brand> Brands { get; set; }
        public List<ProductSize> ProductSizes { get; set; }
        public List<ProductColor > ProductColors { get; set; }
        public List<Category> Categories { get; set; }

    }
}

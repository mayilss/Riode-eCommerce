using Riode.WebUI.AppCode.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;

namespace Riode.WebUI.Models.Entities
{
    public class ProductPricing : HistoryEntity
    {
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int ColorId { get; set; }
        public virtual ProductColor Color { get; set; }
        public int SizeId { get; set; }
        public virtual ProductSize Size { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public double Price { get; set; }
    }
}

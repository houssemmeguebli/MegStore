using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Core.Entities.ProductFolder
{
    public class Category
    {
        public long categoryId { get; set; }       
        public string categorydName { get; set; }
        public string categoryDescription { get; set; }
        public IList<Product>? Products { get; set; } = new List<Product>();

    }
}

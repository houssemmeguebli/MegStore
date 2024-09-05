using MegStore.Core.Entities.ProductFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Application.DTOs
{
    public class CategoryDto
    {
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public IList<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}

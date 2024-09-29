using MegStore.Core.Entities.ProductFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Core.Interfaces
{
    public interface IProductService : IService<Product>
    {
      
        Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(long categoryId);
        Task<List<Product>> GetProductByAdminId(long adminId);

    }
}

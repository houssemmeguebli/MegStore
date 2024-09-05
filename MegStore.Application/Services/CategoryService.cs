using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Application.Services
{
    public class CategoryService : Service<Category>, ICategoryService
    {

        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository repository) : base(repository)
        {
            _categoryRepository = repository;
        }

    }
}


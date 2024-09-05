using MegStore.Core.Entities.Users;
using MegStore.Core.Interfaces;
using MegStore.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly MegStoreContext _context;

        public UserRepository(MegStoreContext context) : base(context)
        {
            _context = context;
        }
    }
}

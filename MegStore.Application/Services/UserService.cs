using MegStore.Core.Entities.ProductFolder;
using MegStore.Core.Entities.Users;
using MegStore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Application.Services
{
    public class UserService : Service<User>, IUserService
    {

        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository repository) : base(repository)
        {
            _userRepository = repository;
        }

    }
}

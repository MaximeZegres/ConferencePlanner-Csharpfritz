using FrontEnd.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontEnd.Services
{
    public class AdminService
    {
        private readonly Lazy<long> _creationKey = new Lazy<long>(() => BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 7));
        private readonly IdentityDbContext _dbContext;
        private bool _adminExists;

        public AdminService(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public long CreationKey => _creationKey.Value;

        public async Task<bool> AllowAdminUserCreationAsync()
        {
            if (_adminExists)
            {
                return false;
            }
            else
            {
                if (await _dbContext.Users.AnyAsync(user => user.IsAdmin))
                {
                    // There are already admin users so disable admin creation
                    _adminExists = true;
                    return false;
                }

                // There are no admin users so enable admin creation
                return true;
            }
        }
    }
}

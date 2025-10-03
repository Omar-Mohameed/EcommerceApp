using myshop.Business.Models;
using myshop.Business.Repositories;
using myshop.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.DataAccess.Implementation
{
    public class ApplicationUserRepository : GenericRepository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly AppDbContext context;

        public ApplicationUserRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}

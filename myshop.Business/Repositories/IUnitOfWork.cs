using myshop.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.Business.Repositories
{
    public interface IUnitOfWork :IDisposable
    {
        IGenericRepository<ApplicationUser> ApplicationUser { get; }
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        int Complete();
    }
}

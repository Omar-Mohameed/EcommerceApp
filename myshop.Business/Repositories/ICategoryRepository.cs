using myshop.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.Business.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        // Special Functions In Category
        void Update(Category category);
    }
}

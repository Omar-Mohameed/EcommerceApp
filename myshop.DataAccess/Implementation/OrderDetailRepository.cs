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
    public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
    {
        private readonly AppDbContext context;

        public OrderDetailRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public void Update(OrderDetail orderDetail)
        {
            context.OrderDetails.Update(orderDetail);
        }
    }
}

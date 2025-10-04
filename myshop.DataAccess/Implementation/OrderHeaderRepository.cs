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
    public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly AppDbContext context;

        public OrderHeaderRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public void Update(OrderHeader orderHeader)
        {
            context.OrderHeaders.Update(orderHeader);
        }

        public void UpdateOrderStatus(int id, string orderStatus, string? paymentStatus)
        {
            var orderFromDb = context.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                orderFromDb.PaymentDate = DateTime.Now;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }
    }
}

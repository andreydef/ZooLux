using System;
using ZooLux.Models.Data;

namespace ZooLux.Models.ViewModels.Shop
{
    public class OrderVM
    {
        public OrderVM() { }

        public OrderVM(OrderDTO row)
        {
            OrderId = row.OrderId;
            UserId = row.UserId;
            CreatedAt = row.CreatedAt;
        }

        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
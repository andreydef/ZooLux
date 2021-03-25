using System;
using System.Collections.Generic;
namespace ZooLux.Models.ViewModels.Account
{
    public class OrderForUserVM
    {
        public int OrderNumber { get; set; }
        public string UserName { get; set; }
        public decimal Total { get; set; }
        public Dictionary<string, int> ProductsAndQty { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace EnstrümanHub.Models
{
    public class DashboardStats
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
    }
} 
using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class OrdersFullView
    {
        public List<OpenOrderCustomModel> MostRecentOrders { get; set; }
        public List<OpenOrderCustomModel> FavoriteOrders { get; set; }
        public List<OpenOrderCustomModel> SearchedOrders { get; set; }
        public List<OpenOrderCustomModel> AllPhysicianOrders { get; set; }
        public OpenOrder EncounterOrder { get; set; }
        public List<OpenOrderCustomModel> OpenOrdersList { get; set; }
        public List<OpenOrderCustomModel> ClosedOrdersList { get; set; }
        public List<OrderActivityCustomModel> OpenOrderActivityList { get; set; }
        public List<OrderActivityCustomModel> ClosedOrderActivityList { get; set; }
        public OrderActivity CurrentOrderActivity { get; set; }
        public List<FutureOpenOrderCustomModel> FutureOpenOrdersList { get; set; } 
    }
}
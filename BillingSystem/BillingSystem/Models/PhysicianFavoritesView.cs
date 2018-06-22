using System.Collections.Generic;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Models
{
    public class PhysicianFavoritesView
    {
        public List<OpenOrderCustomModel> MostRecentOrders { get; set; }
        public List<OpenOrderCustomModel> FavoriteOrders { get; set; }
        public List<OpenOrderCustomModel> SearchedOrders { get; set; }
        public List<FavoritesCustomModel> PhyFavoriteOrders { get; set; }
    }
}

using BillingSystem.Model;
using System.Collections.Generic;

namespace BillingSystem.Models
{
    public class ScreenView
    {
        public List<Screen> ScreensList { get; set; }
        public Screen CurrentScreen { get; set; }
        public List<Tabs> TabsList { get; set; }
        public List<Screen> AvailableScreens { get; set; }
        public List<RolePermission> SelectedScreens { get; set; }
    }
    public class RolePermissionInfo
    {
        public int? RolePermissionID { get; set; }
        public int? PermissionID { get; set; }
    }
}
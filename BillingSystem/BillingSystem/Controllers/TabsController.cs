using BillingSystem.Models;
using BillingSystem.Common;
using System;
using System.Web.Mvc;
using BillingSystem.Model;
using BillingSystem.Common.Common;
using System.Linq;
using System.Collections.Generic;
using BillingSystem.Model.CustomModel;
using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Controllers
{
    public class TabsController : BaseController
    {
        private readonly ITabsService _service;

        const string partialViewPath = "../Tabs/";

        public TabsController(ITabsService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get the details of the Tabs View in the Model Tabs such as TabsList, list of countries etc.
        /// </summary>
        /// <returns>returns the actionresult in the form of current object of the Model Tabs to be passed to View Tabs</returns>
        [CheckRolesAuthorize("1")]
        public ActionResult Index()
        {

            //Get the Entity list
            var list = _service.GetAllTabList(true, Helpers.GetLoggedInUserId());

            //Intialize the View Model i.e. TabsView which is binded to Main View Index.cshtml under Tabs
            var tabsView = new TabsView
            {
                TabsList = list,
                CurrentTabs = new Tabs { IsActive = true, IsVisible = true }
            };

            //Pass the View Model in ActionResult to View Tabs
            return View(tabsView);
        }

        /// <summary>
        /// Bind all the Tabs list 
        /// </summary>
        /// <returns>action result with the partial view containing the Tabs list object</returns>
        public ActionResult BindTabsList(bool showIsActive)
        {
            //Get the facilities list
            var tabsList = new List<TabsCustomModel>(); //_service.GetAllTabList(showIsActive, Helpers.GetLoggedInUserId());

            //Pass the ActionResult with List of TabsViewModel object to Partial View TabsList
            return PartialView(PartialViews.TabsList, tabsList);
        }

        public JsonResult GetTListJson()
        {
            //Get the Entity list
            var list = _service.GetAllTabList(true, Helpers.GetLoggedInUserId());
            var jsonResult = new
            {
                aaData = list.Select(t => new[] {
                t.CurrentTab.TabName, t.CurrentTab.TabHierarchy, t.CurrentTab.Controller,t.CurrentTab.Action, t.CurrentTab.RouteValues,
                    t.ParentTabName,t.CurrentTab.TabId.ToString()})
            };
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Add New or Update the Tabs based on if we pass the Tabs ID in the TabsViewModel object.
        /// </summary>
        /// <param name="model">pass the details of Tabs in the view model</param>
        /// <returns>returns the newly added or updated ID of Tabs row</returns>
        public ActionResult SaveTabs(Tabs model)
        {
            var pView = string.Empty;
            var rId = Helpers.GetDefaultRoleId();
            var result = _service.SaveTab(model, Convert.ToInt64(rId), Helpers.GetLoggedInUserId(), Helpers.GetInvariantCultureDateTime());

            if (result.ExecutionStatus > 0)
            {
                var vn = $"{partialViewPath}{PartialViews.TabsList}";
                pView = RenderPartialViewToStringBase(vn, result.AllTabs);

                if (Session[SessionNames.SessionClass.ToString()] != null && result.TabsByRole.Any())
                {
                    var s = Session[SessionNames.SessionClass.ToString()] as SessionClass;
                    s.MenuSessionList = result.TabsByRole;
                }
            }

            return Json(new { pView, status = result.ExecutionStatus }, JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        /// Get the details of the current Tabs in the view model by ID 
        /// </summary>
        /// <param name="tabId"></param>
        /// <returns></returns>
        public ActionResult GetTabById(string tabId)
        {
            var currentTabs = _service.GetTabByTabId(Convert.ToInt32(tabId));
            return PartialView(PartialViews.TabsAddEdit, currentTabs);
        }

        /// <summary>
        /// Delete the current Tabs based on the Tabs ID passed in the TabsModel
        /// </summary>
        /// <param name="tabId"></param>
        /// <returns></returns>
        public ActionResult DeleteTabs(string tabId)
        {
            var model = _service.GetTabByTabId(Convert.ToInt32(tabId));

            //Check If Tabs model is not null
            if (model != null)
            {
                model.IsDeleted = true;
                model.DeletedBy = Helpers.GetLoggedInUserId();
                model.DeletedDate = Helpers.GetInvariantCultureDateTime();

                //Update Operation of current Tabs
                var result = _service.AddUpdateTab(model);

                //Rebind the Session of Menu Tabs List
                ReBindTabsMenu();

                //return deleted ID of current Tabs as Json Result to the Ajax Call.
                return Json(result);
            }

            //Return the Json result as Action Result back JSON Call Success
            return Json(null);
        }

        /// <summary>
        /// Reset the Tabs View Model and pass it to TabsAddEdit Partial View. 
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetTabsForm()
        {
            //Intialize the new object of Tabs ViewModel
            var model = new Tabs { IsVisible = true, IsActive = true };

            //Pass the View Model as TabsViewModel to PartialView TabsAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.TabsAddEdit, model);
        }

        /// <summary>
        /// Gets the parent tabs.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetParentTabs()
        {
            var list = _service.GetAllTabs();
            return Json(list);
        }

        /// <summary>
        /// Checks if duplicate record exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="tabId">The tab identifier.</param>
        /// <param name="parentTabId">The parent tab identifier.</param>
        /// <returns></returns>
        public ActionResult CheckIfDuplicateRecordExists(string name, string tabId, string parentTabId)
        {
            var result = _service.CheckIfDuplicateRecordExists(name, string.IsNullOrEmpty(tabId) ? 0 : Convert.ToInt32(tabId), string.IsNullOrEmpty(parentTabId) ? 0 : Convert.ToInt32(parentTabId));
            return Json(result);
        }

        /// <summary>
        /// Gets the tab order by parent tab identifier.
        /// </summary>
        /// <param name="tabId">The tab identifier.</param>
        /// <returns></returns>
        public ActionResult GetTabOrderByParentTabId(int tabId)
        {
            if (tabId > 0)
            {
                var maxTabOrder = _service.GetMaxTabOrderByParentTabId(tabId);
                return Json(maxTabOrder);
            }
            return Json(1);
        }

        /// <summary>
        /// Rebind tabs menu.
        /// </summary>
        private void ReBindTabsMenu()
        {
            if (Session[SessionNames.SessionClass.ToString()] != null)
            {
                var s = Session[SessionNames.SessionClass.ToString()] as SessionClass;
                s.MenuSessionList = _service.GetTabsByRole(s.UserName, s.RoleId);

            }
        }
    }
}

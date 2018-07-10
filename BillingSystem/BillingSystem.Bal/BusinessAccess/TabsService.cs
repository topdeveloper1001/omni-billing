using System.Linq;
using BillingSystem.Model;
using System.Collections.Generic;
using System;
using BillingSystem.Model.CustomModel;
using BillingSystem.Common.Common;

using System.Data.SqlClient;

using BillingSystem.Bal.Interfaces;

namespace BillingSystem.Bal.BusinessAccess
{
    public class TabsService : ITabsService
    {
        private readonly IRepository<Tabs> _repository;
        private readonly IRepository<RoleTabs> _rRepository;
        private readonly IRepository<ModuleAccess> _mRepository;
        private readonly BillingEntities _context;

        public TabsService(IRepository<Tabs> repository, IRepository<RoleTabs> rRepository, IRepository<ModuleAccess> mRepository, BillingEntities context)
        {
            _repository = repository;
            _rRepository = rRepository;
            _mRepository = mRepository;
            _context = context;
        }

        //Function to get all tabs
        /// <summary>
        /// Gets all tabs.
        /// </summary>
        /// <returns></returns>
        public List<Tabs> GetAllTabs()
        {
            var list = _repository.Where(s => !s.IsDeleted && s.IsActive).OrderBy(a => a.TabName).ToList();
            return list;
        }

        //Function to get tab by tab id
        /// <summary>
        /// Gets the tab by tab identifier.
        /// </summary>
        /// <param name="tabId">The tab identifier.</param>
        /// <returns></returns>
        public Tabs GetTabByTabId(int tabId)
        {
            var tabModel = _repository.Where(s => s.TabId == tabId).FirstOrDefault();
            return tabModel;
        }

        //Function to add update tab
        /// <summary>
        /// Adds the update tab.
        /// </summary>
        /// <param name="tab">The tab.</param>
        /// <returns></returns>
        public int AddUpdateTab(Tabs tab)
        {
            if (tab.TabId > 0)
                _repository.UpdateEntity(tab, tab.TabId);
            else
                _repository.Create(tab);
            return tab.TabId;
        }

        /// <summary>
        /// Gets the tab name by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string GetTabNameById(int id)
        {
            var tab = GetTabByTabId(id);
            return tab != null ? tab.TabName : string.Empty;
        }

        /// <summary>
        /// Gets all tab list.
        /// </summary>
        /// <returns></returns>
        public List<TabsCustomModel> GetAllTabList(bool activeStatus, long userId = 0)
        {
            var sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("pUserId", userId);
            sqlParameters[1] = new SqlParameter("pStatus", activeStatus);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetAllTabs.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var list = r.GetResultWithJson<TabsCustomModel>(JsonResultsArray.Tabs.ToString());
                return list;
            }
        }

        /// <summary>
        /// Adds to role tab.
        /// </summary>
        /// <param name="tabId">The tab identifier.</param>
        /// <returns></returns>
        public int? AddToRoleTab(int tabId)
        {
            var newRoleTab = new RoleTabs
            {
                RoleID = Convert.ToInt32(DefaultRoleIDs.SysAdminRole),
                TabID = tabId
            };
            var newId = _rRepository.Create(newRoleTab);
            return newId;
        }

        /// <summary>
        /// Determines whether [is exist in role tabs] [the specified tab identifier].
        /// </summary>
        /// <param name="tabId">The tab identifier.</param>
        /// <returns></returns>
        public bool IsExistInRoleTabs(int tabId)
        {
            var rt = _rRepository.Where(r => r.TabID == tabId).FirstOrDefault();
            return rt != null;

        }

        /// <summary>
        /// Checks if duplicate record exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="tabId">The tab identifier.</param>
        /// <param name="parentTabId">The parent tab identifier.</param>
        /// <returns></returns>
        public bool CheckIfDuplicateRecordExists(string name, int tabId, int parentTabId)
        {
            var tb = tabId > 0 ? _repository.Where(t => t.TabId != tabId && t.ParentTabId == parentTabId && t.TabName.Trim().Equals(name) && t.IsActive).FirstOrDefault() : _repository.Where(t => t.ParentTabId == parentTabId && t.TabName.Trim().Equals(name)).FirstOrDefault();
            return tb != null;
        }

        //Method added by Shashank on Nov-3 2014
        /// <summary>
        /// Gets the corporate facility tab list.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IEnumerable<TabsCustomModel> GetCorporateFacilityTabList(int corporateid, int facilityid, int? roleId)
        {
            var list = new List<TabsCustomModel>();
            var tabs = _repository.Where(t => !t.TabName.ToLower().Equals("setup")).OrderBy(x => x.TabOrder).ToList();
            var lst = _mRepository.Where(rp => rp.CorporateID == corporateid).ToList();
            if (facilityid != 0)
            {
                if (lst.Any(fc => fc.FacilityID == facilityid))
                    lst = lst.Where(fc => fc.FacilityID == facilityid).ToList();
            }
            var moduleAccess = lst;

            if (roleId != null)
            {
                var roleTabs = _rRepository.Where(rp => rp.RoleID == roleId).ToList();
                roleTabs = roleTabs.Where(
                    t => (moduleAccess.Any(z => z.TabID == t.TabID))).ToList();

                var customlist = tabs.Where(
                    t => (roleTabs.Any(z => z.TabID == t.TabId))).ToList();


                if (!customlist.Any())
                {
                    customlist = tabs.Where(
                    t => (moduleAccess.Any(z => z.TabID == t.TabId))).ToList();
                }

                list.AddRange(customlist.Select(item => new TabsCustomModel
                {
                    CurrentTab = item,
                    ParentTabName = GetTabNameById(item.ParentTabId),
                    HasChilds = HasChildren(item.TabId)
                }));
            }
            else
            {
                var newlist = tabs.Where(
                    t => (moduleAccess.Any(z => z.TabID == t.TabId))).ToList();
                list.AddRange(newlist.Select(item => new TabsCustomModel
                {
                    CurrentTab = item,
                    ParentTabName = GetTabNameById(item.ParentTabId),
                    HasChilds = HasChildren(item.TabId)
                }));
            }
            return list;
        }

        //Method added by Shashank on Nov-3 2014
        /// <summary>
        /// Gets the parent corporate facility tab list.
        /// </summary>
        /// <param name="corporateid">The corporateid.</param>
        /// <param name="facilityid">The facilityid.</param>
        /// <returns></returns>
        public List<TabsCustomModel> GetParentCorporateFacilityTabList(int corporateid, int facilityid)
        {
            var list = new List<TabsCustomModel>();
            var tabs = _repository.GetAll().OrderBy(t => t.TabName).ToList();
            var moduleAccess = GetModulesAccessList(corporateid, facilityid).ToList();
            var newlist = corporateid == 0 ? tabs : tabs.Where(t => moduleAccess.Any(z => z.TabID == t.TabId) && !t.TabName.Equals("Setup")).ToList();
            list.AddRange(newlist.Select(item => new TabsCustomModel
            {
                CurrentTab = item,
                ParentTabName = GetTabNameById(item.ParentTabId),
                HasChilds = HasChildren(item.TabId)
            }));
            return list;
        }
        private List<ModuleAccess> GetModulesAccessList(int corporateId, int? facilityid)
        {
            var list = _mRepository.Where(rp => rp.CorporateID == corporateId).ToList();
            if (facilityid != 0)
            {
                if (list.Any(fc => fc.FacilityID == facilityid))
                    list = list.Where(fc => fc.FacilityID == facilityid).ToList();
            }
            return list;
        }

        /// <summary>
        /// Gets the maximum tab order by parent tab identifier.
        /// </summary>
        /// <param name="tabId">The tab identifier.</param>
        /// <returns></returns>
        public int GetMaxTabOrderByParentTabId(int tabId)
        {
            var tab = _repository.Where(a => a.ParentTabId == tabId && a.IsActive && a.IsVisible).Max(b => b.TabOrder);
            return tab != null ? Convert.ToInt32(tab) + 1 : 1;
        }

        /// <summary>
        /// Gets the tabs by corporate and facility identifier.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <returns></returns>
        public List<TabsCustomModel> GetTabsByCorporateAndFacilityId(int facilityId, int corporateId)
        {
            var list = new List<TabsCustomModel>();
            //Get All Tabs except SetUp 
            var tabs = _repository.Where(t => !t.TabName.ToLower().Equals("setup")).OrderBy(m => m.TabName).ToList();
            //Get All tab IDs that have rights given to current Corporate and Facility
            var intTabs = _mRepository.Where(m => m.CorporateID != null && m.FacilityID != null && m.CorporateID == corporateId && m.FacilityID == facilityId && m.TabID != null).Select(m1 => m1.TabID.Value).ToList();

            //Filter out tabs from all list that are accessible to current Corporate and facility
            tabs = tabs.Where(t => intTabs.Contains(t.TabId)).ToList();

            if (tabs.Count > 0)
            {
                list.AddRange(tabs.Select(item => new TabsCustomModel
                {
                    CurrentTab = item,
                    ParentTabName = GetTabNameById(item.ParentTabId),
                    HasChilds = HasChildren(item.TabId)
                }));
            }
            return list;
        }

        /// <summary>
        /// Gets the patient tabs list.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tabs> GetPatientTabsList()
        {
            var tabs = _repository.Where(m => m.Controller.ToLower().Trim().Equals("patientportal") && m.ParentTabId == 0).ToList();
            return tabs;
        }


        public IEnumerable<Tabs> GetPatientTabsListData(int patientId)
        {
            var tabs = _repository.Where(m => m.Controller.ToLower().Trim().Equals("patientportal") && m.ParentTabId == 0).ToList();

            if (tabs.Count <= 0) return tabs;

            var patientPortal = tabs.FirstOrDefault(p => p.Controller.Equals("PatientPortal") && p.Action.Equals("Index"));
            if (patientPortal != null)
                patientPortal.RouteValues = "pId=" + patientId;
            return tabs;
        }


        private bool HasChildren(int tabId)
        {
            var tabModel = _repository.Where(s => s.ParentTabId == tabId).Any();
            return tabModel;
        }

        public IEnumerable<TabsCustomModel> GetTabsOnModuleAccessLoad(long loggedUserId, int roleId = 0, long facilityId = 0, long corporateId = 0, bool isDeleted = false, bool isActive = true)
        {

            var query = string.Empty;
            var sqlParameters = new SqlParameter[6];
            sqlParameters[0] = new SqlParameter("RId", roleId);
            sqlParameters[1] = new SqlParameter("UId", loggedUserId);
            sqlParameters[2] = new SqlParameter("FId", facilityId);
            sqlParameters[3] = new SqlParameter("CId", corporateId);
            sqlParameters[4] = new SqlParameter("IsDeleted", isDeleted);
            sqlParameters[5] = new SqlParameter("IsActive", isActive);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetTabsListOnModuleAccessLoad.ToString(), isCompiled: false, parameters: sqlParameters))
            {
                var tabs = r.ResultSetFor<Tabs>().ToList();
                var list = r.ResultSetFor<TabsCustomModel>().ToList();

                if (list.Any() && tabs.Any())
                {
                    list.ForEach(t =>
                    {
                        t.CurrentTab = tabs.Where(a => a.TabId == Convert.ToInt32(t.TabId)).FirstOrDefault();
                    });
                }
                return list;
            }

        }


        public IEnumerable<TabsCustomModel> GetTabsListInRoleTabsView(long loggedUserId, int roleId = 0, long facilityId = 0, long corporateId = 0, bool isDeleted = false, bool isActive = true)
        {
            var query = string.Empty;
            var sqlParameters = new SqlParameter[6];
            sqlParameters[0] = new SqlParameter("RId", roleId);
            sqlParameters[1] = new SqlParameter("UId", loggedUserId);
            sqlParameters[2] = new SqlParameter("FId", facilityId);
            sqlParameters[3] = new SqlParameter("CId", corporateId);
            sqlParameters[4] = new SqlParameter("IsDeleted", isDeleted);
            sqlParameters[5] = new SqlParameter("IsActive", isActive);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetTabsInRoleTabsView.ToString(), isCompiled: false, parameters: sqlParameters))
            {
                var tabs = r.ResultSetFor<Tabs>().ToList();
                var list = r.ResultSetFor<TabsCustomModel>().ToList();

                if (list.Any() && tabs.Any())
                {
                    list.ForEach(t =>
                    {
                        t.CurrentTab = tabs.Where(a => a.TabId == Convert.ToInt32(t.TabId)).FirstOrDefault();
                    });
                }
                return list;
            }
        }


        /// <summary>
        /// Gets all tab list.
        /// </summary>
        /// <returns></returns>
        public List<Tabs> GetTabsByRole(string userName, long roleId)
        {
            var sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("pUsername", userName);
            sqlParameters[1] = new SqlParameter("pRole", roleId);
            sqlParameters[2] = new SqlParameter("pPortalKey", ExtensionMethods.DefaultPortalKey);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocGetTabsByRole.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                var list = r.GetResultWithJson<Tabs>(JsonResultsArray.Tabs.ToString());
                return list;
            }
        }

        public TabsData SaveTab(Tabs m, long roleId, long userId, DateTime currentDate)
        {
            var result = new TabsData();
            var sqlParameters = new SqlParameter[13];
            sqlParameters[0] = new SqlParameter("pId", m.TabId);
            sqlParameters[1] = new SqlParameter("pTabName", m.TabName);
            sqlParameters[2] = new SqlParameter("pController", !string.IsNullOrEmpty(m.Controller) ? m.Controller : string.Empty);
            sqlParameters[3] = new SqlParameter("pAction", !string.IsNullOrEmpty(m.Action) ? m.Action : string.Empty);
            sqlParameters[4] = new SqlParameter("pRouteValues", !string.IsNullOrEmpty(m.RouteValues) ? m.RouteValues : string.Empty);
            sqlParameters[5] = new SqlParameter("pTabOrder", m.TabOrder.GetValueOrDefault());
            sqlParameters[6] = new SqlParameter("pTabImageUrl", !string.IsNullOrEmpty(m.TabImageUrl) ? m.TabImageUrl : string.Empty);
            sqlParameters[7] = new SqlParameter("pParentTabId", m.ParentTabId);
            sqlParameters[8] = new SqlParameter("pIsActive", m.IsActive);
            sqlParameters[9] = new SqlParameter("pUserId", userId);
            sqlParameters[10] = new SqlParameter("pCurrentDate", currentDate);
            sqlParameters[11] = new SqlParameter("pCurrentRoleId", roleId);
            sqlParameters[12] = new SqlParameter("pPortalKey", ExtensionMethods.DefaultPortalKey);

            using (var r = _context.MultiResultSetSqlQuery(StoredProcedures.SprocSaveTab.ToString(), parameters: sqlParameters, isCompiled: false))
            {
                result.ExecutionStatus = r.SingleResultSetFor<int>();
                if (result.ExecutionStatus > 0)
                {
                    result.AllTabs = r.GetResultWithJson<TabsCustomModel>(JsonResultsArray.Tabs.ToString());
                    result.TabsByRole = r.GetResultWithJson<Tabs>(JsonResultsArray.Tabs.ToString());
                }
                return result;

            }
        }
    }
}

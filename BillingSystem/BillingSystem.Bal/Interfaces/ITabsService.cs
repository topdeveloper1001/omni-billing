using System;
using System.Collections.Generic;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Bal.Interfaces
{
    public interface ITabsService
    {
        int? AddToRoleTab(int tabId);
        int AddUpdateTab(Tabs tab);
        bool CheckIfDuplicateRecordExists(string name, int tabId, int parentTabId);
        List<TabsCustomModel> GetAllTabList(bool activeStatus, long userId = 0);
        List<Tabs> GetAllTabs();
        IEnumerable<TabsCustomModel> GetCorporateFacilityTabList(int corporateid, int facilityid, int? roleId);
        int GetMaxTabOrderByParentTabId(int tabId);
        List<TabsCustomModel> GetParentCorporateFacilityTabList(int corporateid, int facilityid);
        IEnumerable<Tabs> GetPatientTabsList();
        IEnumerable<Tabs> GetPatientTabsListData(int patientId);
        Tabs GetTabByTabId(int tabId);
        string GetTabNameById(int id);
        List<TabsCustomModel> GetTabsByCorporateAndFacilityId(int facilityId, int corporateId);
        List<Tabs> GetTabsByRole(string userName, long roleId);
        IEnumerable<TabsCustomModel> GetTabsListInRoleTabsView(long loggedUserId, int roleId = 0, long facilityId = 0, long corporateId = 0, bool isDeleted = false, bool isActive = true);
        IEnumerable<TabsCustomModel> GetTabsOnModuleAccessLoad(long loggedUserId, int roleId = 0, long facilityId = 0, long corporateId = 0, bool isDeleted = false, bool isActive = true);
        bool IsExistInRoleTabs(int tabId);
        TabsData SaveTab(Tabs m, long roleId, long userId, DateTime currentDate);
    }
}
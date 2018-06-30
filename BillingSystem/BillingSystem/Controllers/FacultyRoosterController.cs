﻿
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Linq;
using BillingSystem.Common.Common;
using BillingSystem.Bal.Interfaces;
using BillingSystem.Bal.BusinessAccess;
using BillingSystem.Common;
using BillingSystem.Models;
using BillingSystem.Model;
using BillingSystem.Model.CustomModel;

namespace BillingSystem.Controllers
{
    /// <summary>
    /// FacultyRooster controller.
    /// </summary>
    public class FacultyRoosterController : BaseController
    {
        private readonly IFacilityStructureService _fsService;
        private readonly IClinicianRosterService _crService;
        private readonly ICorporateService _cService;

        public FacultyRoosterController(IFacilityStructureService fsService, IClinicianRosterService crService, ICorporateService cService)
        {
            _fsService = fsService;
            _crService = crService;
            _cService = cService;
        }

        #region Public Methods and Operators

        /// <summary>
        ///     Bind all the FacultyRooster list
        /// </summary>
        /// <returns>action result with the partial view containing the FacultyRooster list object</returns>
        [HttpPost]
        public ActionResult BindFacultyRoosterList()
        {
            // Initialize the FacultyRooster BAL object
            using (var facultyRoosterBal = new FacultyRoosterService())
            {
                // Get the facilities list
                var facultyRoosterList = facultyRoosterBal.GetFacultyRoosterByFacility(Helpers.GetDefaultFacilityId());

                // Pass the ActionResult with List of FacultyRoosterViewModel object to Partial View FacultyRoosterList
                return PartialView(PartialViews.FacultyRoosterList, facultyRoosterList);
            }
        }

        /// <summary>
        /// Delete the current FacultyRooster based on the FacultyRooster ID passed in the FacultyRoosterModel
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult DeleteFacultyRooster(int id)
        {
            var returnobj = -1;
            using (var bal = new FacultyRoosterService())
            {
                // Get FacultyRooster model object by current FacultyRooster ID
                var currentFacultyRooster = bal.GetFacultyRoosterById(id);
                var userId = Helpers.GetLoggedInUserId();
                returnobj = bal.DeleteFacultyRooster(id);
            }

            // Return the Json result as Action Result back JSON Call Success
            return Json(returnobj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get the details of the current FacultyRooster in the view model by ID
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult GetFacultyRooster(int id)
        {
            using (var bal = new FacultyRoosterService())
            {
                // Call the AddFacultyRooster Method to Add / Update current FacultyRooster
                var currentFacultyRooster = bal.GetFacultyRoosterCById(id);
                return Json(currentFacultyRooster, JsonRequestBehavior.AllowGet);

                // Pass the ActionResult with the current FacultyRoosterViewModel object as model to PartialView FacultyRoosterAddEdit
                // return this.PartialView(PartialViews.FacultyRoosterAddEdit, currentFacultyRooster);
            }
        }

        /// <summary>
        /// Get the details of the FacultyRooster View in the Model FacultyRooster such as FacultyRoosterList, list of
        ///     countries etc.
        /// </summary>
        /// <returns>
        /// returns the actionresult in the form of current object of the Model FacultyRooster to be passed to View
        ///     FacultyRooster
        /// </returns>
        public ActionResult Index()
        {
            // Initialize the FacultyRooster BAL object
            var facultyRoosterBal = new FacultyRoosterService();

            // Get the Entity list
            var facultyRoosterList = facultyRoosterBal.GetFacultyRoosterByFacility(Helpers.GetDefaultFacilityId());

            // Intialize the View Model i.e. FacultyRoosterView which is binded to Main View Index.cshtml under FacultyRooster
            var facultyRoosterView = new FacultyRoosterView
            {
                FacultyRoosterList = facultyRoosterList,
                CurrentFacultyRooster = new FacultyRooster()
            };

            // Pass the View Model in ActionResult to View FacultyRooster
            return View(facultyRoosterView);
        }

        /// <summary>
        /// Reset the FacultyRooster View Model and pass it to FacultyRoosterAddEdit Partial View.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        public ActionResult ResetFacultyRoosterForm()
        {
            // Intialize the new object of FacultyRooster ViewModel
            var facultyRoosterViewModel = new FacultyRooster();

            // Pass the View Model as FacultyRoosterViewModel to PartialView FacultyRoosterAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.FacultyRoosterAddEdit, facultyRoosterViewModel);
        }

        /// <summary>
        /// Add New or Update the FacultyRooster based on if we pass the FacultyRooster ID in the FacultyRoosterViewModel
        ///     object.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// returns the newly added or updated ID of FacultyRooster row
        /// </returns>
        public ActionResult SaveFacultyRooster(FacultyRooster model)
        {
            // Initialize the newId variable 
            int newId = -1;
            int userId = Helpers.GetLoggedInUserId();

            // Check if Model is not null 
            if (model != null)
            {
                using (var bal = new FacultyRoosterService())
                {
                    // Call the AddFacultyRooster Method to Add / Update current FacultyRooster
                    newId = bal.SaveFacultyRooster(model);
                }
            }

            return Json(newId);
        }

        /// <summary>
        /// Gets the facility departments.
        /// </summary>
        /// <param name="coporateId">The coporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public ActionResult GetFacilityDepartments(int coporateId, int facilityId)
        {
            var list = _fsService.GetFacilityDepartments(coporateId, facilityId.ToString());
            return Json(list, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Saves the faculty timming data.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult SaveFacultyTimmingData(List<FacultyRooster> model)
        {
            // Initialize the newId variable 
            int newId = -1;
            int userId = Helpers.GetLoggedInUserId();
            var createdDate = Helpers.GetInvariantCultureDateTime();
            var errorLog = 0;
            var duplicateEntryLog = new List<FacultyRoosterLogCustomModel>();

            // Check if Model is not null 
            if (model != null)
            {
                using (var bal = new FacultyRoosterService())
                {
                    for (var index = 0; index < model.Count; index++)
                    {
                        var facultyRooster = model[index];
                        facultyRooster.CreatedBy = userId;
                        facultyRooster.CreatedDate = createdDate;
                        facultyRooster.WeekNumber = Helpers.GetWeekOfYearISO8601(Convert.ToDateTime(facultyRooster.FromDate));
                        facultyRooster.IsActive = true;
                        facultyRooster.WorkingDay = Helpers.GetDayOfTheWeek(Convert.ToDateTime(facultyRooster.FromDate));
                        var isDuplicateEntry = bal.CheckForDuplicateEntry(facultyRooster);
                        if (isDuplicateEntry != 0)
                        {
                            duplicateEntryLog.Add(bal.DuplicateEntryLog(facultyRooster, isDuplicateEntry));
                            errorLog = 1;
                            model.RemoveAt(index);
                        }
                    }

                    // Call the AddFacultyRooster Method to Add / Update current FacultyRooster
                    if (model.Count > 0)
                        bal.SaveFacultyRoosterList(model);

                    if (errorLog == 1)
                    {
                        var listToreturn = new { errorLog = 1, duplicateEntryLog, };
                        return Json(listToreturn, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var listToreturn = new { errorLog = 0, duplicateEntryLog };
                        return Json(listToreturn, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            return Json(newId);
        }

        /// <summary>
        /// Binds the faculty rooster list by facility.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public ActionResult BindFacultyRoosterListByFacility(int facilityId)
        {
            // Initialize the FacultyRooster BAL object
            using (var facultyRoosterBal = new FacultyRoosterService())
            {
                // Get the facilities list
                var facultyRoosterList = facultyRoosterBal.GetFacultyRoosterByFacility(facilityId);

                // Pass the ActionResult with List of FacultyRoosterViewModel object to Partial View FacultyRoosterList
                return PartialView(PartialViews.FacultyRoosterList, facultyRoosterList);
            }
        }
        #endregion

        public JsonResult BindDataOnPageLoad()
        {
            List<DropdownListData> cList;
            List<DropdownListData> specialties;
            List<DropdownListData> flist;
            var pList = new List<PhysicianCustomModel>();
            var listDepartments = new List<DropdownListData>();
            List<DropdownListData> mWeekDays = null;
            List<DropdownListData> reasons = null;
            var cId = Helpers.GetSysAdminCorporateID();
            var fId = Helpers.GetDefaultFacilityId();


            //Bind Corporates 
            cList = _cService.GetCorporateDropdownData(Helpers.GetDefaultCorporateId());

            //Bind Facilities
            using (var fBal = new FacilityService())
                flist = fBal.GetFacilitiesForDashboards(fId, cId, Helpers.GetLoggedInUserIsAdmin());

            //Bind Specialties
            using (var sBal = new GlobalCodeService())
            {
                var category = Convert.ToString((int)GlobalCodeCategoryValue.PhysicianSpecialties);
                specialties = sBal.GetListByCategoriesRange(new[] { category });
            }

            //Bind Physicians
            using (var pBal = new PhysicianService())
                pList = pBal.GetPhysicians(cId, Helpers.GetLoggedInUserIsAdmin(), Helpers.GetLoggedInUserId(), fId);


            /*-----------Get Departments Data Start here----------------------*/

            var deps = _fsService.GetFacilityDepartments(cId, Convert.ToString(fId));
            if (deps.Count > 0)
            {
                listDepartments.AddRange(
                    deps.Where(x => !string.IsNullOrEmpty(x.ExternalValue1))
                        .Select(item => new DropdownListData
                        {
                            Value = Convert.ToString(item.FacilityStructureId),
                            Text = string.Format(" {0} ", item.FacilityStructureName)
                        }));
            }

            /*-----------Get Departments Data End here----------------------*/

            var categories = new List<string> { "1121", "901", "80441" };
            var gc = new GlobalCodeService().GetListByCategoriesRange(categories);
            mWeekDays = gc.Where(f => f.ExternalValue1.Equals("901")).OrderBy(f => f.Value).ToList();
            reasons = gc.Where(f => f.ExternalValue1.Equals("80441")).OrderBy(f => f.Text).ToList();
            var jsonData = new
            {
                cList,
                flist,
                specialties,
                cId = Convert.ToString(cId),
                fId = Convert.ToString(fId),
                pList,
                listDepartments,
                mWeekDays,
                reasons
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }



        #region Clinician's Off Time Section
        public async Task<ActionResult> index2()
        {
            IEnumerable<ClinicianRosterCustomModel> list = null;

            // Initialize the FacultyRooster BAL object 
            list = await _crService.GetAll(Helpers.GetDefaultFacilityId(), Helpers.GetSysAdminCorporateID(), Helpers.GetLoggedInUserId(), id: 0);
            var viewModel = new ClinicianRosterView { Current = new ClinicianRosterCustomModel { DateFrom = DateTime.Now, DateTo = DateTime.Now.AddDays(1), Id = 0 }, List = list };
            return View(viewModel);
        }

        public async Task<JsonResult> SaveRecordCR(ClinicianRosterCustomModel vm)
        {
            var result = await _crService.Save(vm, Helpers.GetLoggedInUserId());
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<PartialViewResult> DeleteRecordCR(long id, bool aStatus)
        {
            var result = await _crService.Delete(Helpers.GetDefaultFacilityId(), Helpers.GetSysAdminCorporateID(), Helpers.GetLoggedInUserId(), id, aStatus);

            return PartialView(PartialViews.ClinicianRosterList, result);
        }

        public async Task<PartialViewResult> RebindClinicianRosterList(int fId, int cId, bool aStatus)
        {
            IEnumerable<ClinicianRosterCustomModel> result = null;
            result = await _crService.GetAll(fId, cId, Helpers.GetLoggedInUserId(), aStatus: aStatus);

            return PartialView(PartialViews.ClinicianRosterList, result);
        }

        public async Task<JsonResult> GetSingleClinicianR(long id)
        {
            ClinicianRosterCustomModel vm = null;
            vm = await _crService.GetSingle(Helpers.GetDefaultFacilityId(), Helpers.GetSysAdminCorporateID(), Helpers.GetLoggedInUserId(), id);

            var json = new
            {
                vm.Id,
                vm.ClinicianId,
                vm.ReasonId,
                vm.RosterTypeId,
                vm.Comments,
                vm.CorporateId,
                vm.FacilityId,
                vm.ExtValue1,
                vm.ExtValue2,
                vm.IsActive,
                vm.RepeatitiveDaysInWeek,
                DFrom = vm.DateFrom.ToString("MM/dd/yyyy"),
                DTo = vm.DateTo.GetShortDateString3(),
                vm.TimeFrom,
                vm.TimeTo
            };

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetPhysiciansByFacility(int fId)
        {
            //Bind Physicians
            using (var pBal = new PhysicianService())
            {
                var pList = await pBal.GetFacultyList(fId, Helpers.GetLoggedInUserId());
                return Json(pList, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> BindDataOnPageLoadInCR()
        {
            List<DropdownListData> cList;
            List<DropdownListData> specialties;
            List<DropdownListData> flist;
            var pList = new List<PhysicianViewModel>();
            var listDepartments = new List<DropdownListData>();
            List<DropdownListData> mWeekDays = null;
            List<DropdownListData> reasons = null;
            var cId = Helpers.GetSysAdminCorporateID();
            var fId = Helpers.GetDefaultFacilityId();


            //Bind Corporates 
            cList = _cService.GetCorporateDropdownData(Helpers.GetDefaultCorporateId());

            //Bind Facilities
            using (var fBal = new FacilityService())
                flist = fBal.GetFacilitiesForDashboards(fId, cId, Helpers.GetLoggedInUserIsAdmin());

            //Bind Specialties
            using (var sBal = new GlobalCodeService())
            {
                var category = Convert.ToString((int)GlobalCodeCategoryValue.PhysicianSpecialties);
                specialties = sBal.GetListByCategoriesRange(new[] { category });
            }

            //Bind Physicians
            using (var pBal = new PhysicianService())
                pList = await pBal.GetFacultyList(fId, Helpers.GetLoggedInUserId());


            /*-----------Get Departments Data Start here----------------------*/

            var deps = _fsService.GetFacilityDepartments(cId, Convert.ToString(fId));
            if (deps.Count > 0)
            {
                listDepartments.AddRange(
                    deps.Where(x => !string.IsNullOrEmpty(x.ExternalValue1))
                        .Select(item => new DropdownListData
                        {
                            Value = Convert.ToString(item.FacilityStructureId),
                            Text = string.Format(" {0} ", item.FacilityStructureName)
                        }));
            }

            /*-----------Get Departments Data End here----------------------*/

            var categories = new List<string> { "1121", "901", "80441" };
            var gc = new GlobalCodeService().GetListByCategoriesRange(categories);
            mWeekDays = gc.Where(f => f.ExternalValue1.Equals("901")).OrderBy(f => f.Value).ToList();
            reasons = gc.Where(f => f.ExternalValue1.Equals("80441")).OrderBy(f => f.Text).ToList();
            var jsonData = new
            {
                cList,
                flist,
                specialties,
                cId = Convert.ToString(cId),
                fId = Convert.ToString(fId),
                pList,
                listDepartments,
                mWeekDays,
                reasons
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}

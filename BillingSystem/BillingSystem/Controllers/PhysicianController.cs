
using Microsoft.Ajax.Utilities;

namespace BillingSystem.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using BillingSystem.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Models;
    using System.Threading.Tasks;
    using BillingSystem.Bal.Interfaces;

    /// <summary>
    /// The physician controller.
    /// </summary>
    public class PhysicianController : BaseController
    {

        private readonly IFacilityStructureService _fsService;
        private readonly IUsersService _uService;
        private readonly IClinicianAppointmentTypesService _caService;
        private readonly IPhysicianService _service;
        private readonly IFacilityService _fService;
        private readonly IFacilityRoleService _frService;
        private readonly IGlobalCodeService _gService;

        public PhysicianController(IFacilityStructureService fsService, IUsersService uService, IClinicianAppointmentTypesService caService, IPhysicianService service, IFacilityService fService, IFacilityRoleService frService, IGlobalCodeService gService)
        {
            _fsService = fsService;
            _uService = uService;
            _caService = caService;
            _service = service;
            _fService = fService;
            _frService = frService;
            _gService = gService;
        }



        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {

            var facilityId = Helpers.GetDefaultFacilityId();
            var list = await _service.GetFacultyList(facilityId, Helpers.GetLoggedInUserId());

            // Intialize the View Model i.e. Facility which is binded to PhysicianView
            var physicianModel = new PhysicianView
            {
                PhysicianList = list,
                CurrentPhysician = new PhysicianViewModel()
            };

            // Pass the View Model in ActionResult to View Physician
            return View(physicianModel);
        }

        /// <summary>
        /// Binds the physician list.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> BindPhysicianList()
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var physicianList = await _service.GetFacultyList(facilityId, Helpers.GetLoggedInUserId());
            return PartialView(PartialViews.PhysicianList, physicianList);
        }

        /// <summary>
        /// Saves the physician.
        /// </summary>
        /// <param name="vm">The physician.</param>
        /// <returns></returns>
        public ActionResult SavePhysician(PhysicianViewModel vm)
        {
            var newId = -1;

            if (vm != null)
            {
                var isExists = _service.CheckDuplicateEmpNo(Convert.ToInt32(vm.PhysicianEmployeeNumber), vm.Id);
                if (isExists)
                    return Json("-1");

                isExists = _service.CheckDuplicateClinicalId(vm.PhysicianLicenseNumber, vm.Id);
                if (isExists)
                    return Json("-2");


                // Check if User Type and User ID already exists in the Physician Table
                isExists = _service.CheckIfUserTypeAndUserIdAlreadyExists(Convert.ToInt32(vm.UserType),
                    Convert.ToInt32(vm.UserId), vm.Id);

                if (isExists)
                    return Json("-3", JsonRequestBehavior.AllowGet);

                vm.CorporateId = Helpers.GetSysAdminCorporateID();
                vm.FacilityId = Helpers.GetDefaultFacilityId();

                if (vm.Id > 0)
                {
                    vm.ModifiedBy = Helpers.GetLoggedInUserId();
                    vm.ModifiedDate = Helpers.GetInvariantCultureDateTime();
                }
                else
                {
                    vm.CreatedBy = Helpers.GetLoggedInUserId();
                    vm.CreatedDate = Helpers.GetInvariantCultureDateTime();
                }

                newId = _service.AddUpdatePhysician(vm);
            }

            return Json(newId);
        }

        /// <summary>
        /// Deletes the physician.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeletePhysician(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                // Intialize the Deleted ID variable 
                var deletedId = Convert.ToInt32(id);

                var vm = new PhysicianViewModel { Id = deletedId, IsDeleted = true, DeletedBy = Helpers.GetLoggedInUserId(), DeletedDate = Helpers.GetInvariantCultureDateTime() };

                deletedId = _service.AddUpdatePhysician(vm);

                // Return the Json result as Action Result back JSON Call Success
                return Json(deletedId);
            }

            return Json(null);
        }

        /// <summary>
        /// Gets the physician.
        /// </summary>
        /// <param name="physicianId">
        /// The Id.
        /// </param>
        /// <returns>
        /// partial View
        /// </returns>
        public JsonResult GetPhysician(int physicianId)
        {
            var current = _service.GetPhysicianCModelById(physicianId);
            var json = new
            {
                current.Physician.Id,
                current.Physician.UserType,
                current.Physician.UserId,
                current.Physician.FacultySpeciality,
                current.Physician.FacultyDepartment,
                current.Physician.PhysicianEmployeeNumber,
                current.Physician.PhysicianName,
                current.Physician.PhysicianLicenseNumber,
                current.Physician.FacultyLunchTimeFrom,
                current.Physician.FacultyLunchTimeTill,
                current.Physician.PhysicianLicenseType,
                PhysicianLicenseEffectiveStartDate = current.Physician.PhysicianLicenseEffectiveStartDate.GetShortDateString3(),
                PhysicianLicenseEffectiveEndDate = current.Physician.PhysicianLicenseEffectiveEndDate.GetShortDateString3(),
                current.Physician.PhysicianPrimaryFacility,
                current.Physician.PhysicianSecondaryFacility,
                current.Physician.PhysicianThirdFacility,
                current.UserTypeStr,
                current.Physician.IsSchedulingPublic,
                current.Physician.AssociatedFacilities
            };
            return Json(json, JsonRequestBehavior.AllowGet);
            //return PartialView(PartialViews.PhysicianAddEdit, current);
        }

        /// <summary>
        /// Gets the name of the facility.
        /// </summary>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns>Json String</returns>
        public ActionResult GetFacilityName(string facilityId)
        {
            var name = _fService.GetFacilityNameById(Convert.ToInt32(facilityId));
            return Json(name);
        }

        /// <summary>
        /// Reset the Facility View Model and pass it to FacilityAddEdit Partial View.
        /// </summary>
        /// <returns>Partial View</returns>
        public ActionResult ResetPhysicianForm()
        {
            // Pass the View Model as FacilityViewModel to PartialView FacilityAddEdit just to update the AddEdit partial view.
            return PartialView(PartialViews.PhysicianAddEdit, new Physician());
        }

        /// <summary>
        /// Gets the facilities.
        /// </summary>
        /// <returns>Json List</returns>
        public ActionResult GetFacilities()
        {
            var cId = Helpers.GetSysAdminCorporateID();
            var facilityList = _fService.GetFacilitiesByCorpoarteId(cId);
            var list = facilityList.Select(item => new SelectListItem { Text = item.FacilityName, Value = item.FacilityId.ToString() }).ToList();

            return Json(list);
        }

        /// <summary>
        /// Binds the physicianddl list.
        /// </summary>
        /// <returns>Json List</returns>
        public ActionResult BindPhysicianddlList()
        {
            var facilityId = Helpers.GetDefaultFacilityId();
            var physicianList = _service.GetFacilityPhysicians(facilityId);
            return Json(physicianList);
        }

        /// <summary>
        /// Gets the users bytype.
        /// </summary>
        /// <param name="userTypeId">The Role ID identifier.</param>
        /// <added by="Shashank">ON 12/16/2014</added>
        /// <returns>Json result</returns>
        public JsonResult BindUserOnUserRoleSelection(int userTypeId)
        {
            var list = Helpers.GetPhysiciansByUserRole(userTypeId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BindUsersType()
        {
            var list = new List<DropdownListData>();
            var corporateId = Helpers.GetSysAdminCorporateID();

            var facilityId = Helpers.GetDefaultFacilityId();
            var roleList = _frService.GetUserTypeRoleDropDown(corporateId, facilityId, true);
            if (roleList.Count > 0)
            {
                list.AddRange(roleList.Select(item => new DropdownListData
                {
                    Text = string.Format("{0}", item.RoleName),
                    Value = Convert.ToString(item.RoleId)
                }));
            }
            return Json(list, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// Method is used to get the physician list by patient id
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public JsonResult GetPhysicianListByPatientId(int patientId)
        {
            var list = _service.GetPhysicianListByPatientId(patientId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method is used to bind physicians 
        /// </summary>
        /// <param name="facilityId"></param>
        /// <param name="departmentId"></param>
        /// <param name="specialityId"></param>
        /// <returns></returns>
        public JsonResult BindPhysicianBySpeciality(int facilityId, string specialityId)
        {
            var list = _service.BindPhysicianBySpeciality(facilityId, specialityId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public JsonResult BindAllFacultyData()
        {
            var categories = new List<string> { "1121" };
            var fList = new List<DropdownListData>();
            //var ltList = new List<DropdownListData>();
            var sList = new List<DropdownListData>();
            var dList = new List<DropdownListData>();
            var uList = new List<UsersViewModel>();
            var urList = new List<DropdownListData>();

            var fId = Helpers.GetDefaultFacilityId();
            var cId = Helpers.GetSysAdminCorporateID();
            var facilityListView = string.Empty;

            //Get Facilities data
            fList = _fService.GetFacilitiesForDashboards(fId, cId, Helpers.GetLoggedInUserIsAdmin());

            //Bind the physicians list to div #divPhysicianList
            var viewpath = string.Format("../Physician/{0}", PartialViews.FacilityListViewInPhysicianView);
            facilityListView = RenderPartialViewToStringBase(viewpath, fList);
            uList = _uService.GetUsersByRole(fId, cId);
            if (uList.Count > 0)
            {
                urList.AddRange(uList.DistinctBy(k => k.RoleId).Select(item => new DropdownListData
                {
                    Text = item.RoleName,
                    Value = Convert.ToString(item.RoleId)
                }));

            }

            //Get License Types and Specialties Data
            var results = _gService.GetListByCategoriesRange(categories, facilityId: Convert.ToString(Helpers.GetDefaultFacilityId()));
            if (results.Count > 0)
            {
                //ltList = results.Where(g => g.ExternalValue1.Equals("1104")).ToList();
                sList = results.Where(g => g.ExternalValue1.Equals("1121")).ToList();
            }

            //Get Departments Data

            var deps = _fsService.GetFacilityDepartments(Helpers.GetSysAdminCorporateID(), Convert.ToString(fId));
            if (deps.Any())
            {
                dList.AddRange(deps.Select(item => new DropdownListData
                {
                    Text = string.Format(" {0} ", item.FacilityStructureName),
                    Value = Convert.ToString(item.FacilityStructureId)
                }));
            }



            var jsonData = new
            {
                fList,
                uList,
                sList,
                dList,
                urList,
                fId,
                facilityListView
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BindLicenseTypes(string roleName)
        {
            var value = new List<string> { roleName };
            var ltList = new List<DropdownListData>();

            //Get License Types and Specialties Data
            ltList = _gService.GetGCodesListByCategoryValue("1104", value, string.Empty);

            return Json(ltList, JsonRequestBehavior.AllowGet);
        }



        #region Clinician AppointmentTypes View
        public ViewResult ClinicianAppTypes()
        {
            return View(new ClinicianAppointmentTypesView
            {
                List = _caService.GetList(Helpers.GetDefaultFacilityId(), Helpers.GetLoggedInUserId(), 0),
                CurrentClinician = new ClinicianAppTypesCustomModel()
            });
        }

        public JsonResult BindClinicianAppointmentDataOnLoad()
        {
            var vm = _caService.GetDataOnViewLoad(Helpers.GetDefaultFacilityId(), Helpers.GetLoggedInUserId());

            //Bind the physicians list to div #divPhysicianList
            var viewpath = string.Format("../Physician/{0}", PartialViews.FacilityListViewInPhysicianView);
            var facilityListView = RenderPartialViewToStringBase(viewpath, vm.AppointmentTypes);

            var jsonResult = Json(new
            {
                facilityListView,
                Physicians = vm.Physicians
            }, JsonRequestBehavior.AllowGet);

            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public PartialViewResult BindClinicianAppointmentTypesData()
        {
            var list = _caService.GetList(Helpers.GetDefaultFacilityId(), Helpers.GetLoggedInUserId(), 0);
            return PartialView(PartialViews.CAppointmentTypesPartial, list);
        }

        public JsonResult SaveClinicianAppTypeData(ClinicianAppTypesCustomModel vm)
        {
            vm.CreatedBy = Helpers.GetLoggedInUserId();
            var result = _caService.Save(vm);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditClinicianAppointmentTypesData(long clinicianId)
        {
            var current = _caService.GetList(Helpers.GetDefaultFacilityId(), Helpers.GetLoggedInUserId(), clinicianId).FirstOrDefault();
            return Json(current, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the corporate physicians.
        /// </summary>
        /// <param name="corporateId">The corporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public ActionResult GetCorporatePhysicians(string corporateId, string facilityId)
        {
            var cId = string.IsNullOrEmpty(corporateId) ? Helpers.GetSysAdminCorporateID().ToString() : corporateId;
            cId = string.IsNullOrEmpty(facilityId)
                      ? cId
                      : Helpers.GetCorporateIdByFacilityId(Convert.ToInt32(facilityId)).ToString();
            var isAdmin = Helpers.GetLoggedInUserIsAdmin();
            var userid = Helpers.GetLoggedInUserId();
            var corporateUsers = _service.GetCorporatePhysiciansList(Convert.ToInt32(cId), isAdmin, userid, Convert.ToInt32(facilityId));
            var viewpath = $"../Scheduler/{PartialViews.PhysicianCheckBoxList}";
            return PartialView(viewpath, corporateUsers);
        }


        public ActionResult GetPhysicianByFacility(int facilityId)
        {
            var list = new List<SelectListItem>();
            var corporateUsers = new List<PhysicianCustomModel>();
            var cId = Helpers.GetSysAdminCorporateID().ToString();
            cId = facilityId == 0
                ? cId
                : Helpers.GetCorporateIdByFacilityId(Convert.ToInt32(facilityId)).ToString();
            corporateUsers = _service.GetCorporatePhysiciansPreScheduling(Convert.ToInt32(cId), Convert.ToInt32(facilityId));

            var updatedList = new
            {
                phyList = corporateUsers
            };
            return Json(updatedList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the clinical identifier number.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetClinicalIDNumber()
        {
            var physicians = _service.GetPhysiciansListByFacilityId(Helpers.GetDefaultFacilityId());
            if (physicians.Count > 0)
            {
                var list = new List<SelectListItem>();
                list.AddRange(physicians.Select(item => new SelectListItem
                {
                    Text = item.Physician.PhysicianLicenseNumber,
                    Value = item.Physician.PhysicianLicenseNumber
                }));
                list = list.OrderBy(x => x.Text).ToList();
                return Json(list);
            }
            return Json(0);
        }

        /// <summary>
        /// Gets the facility phycisian.
        /// </summary>
        /// <param name="coporateId">The coporate identifier.</param>
        /// <param name="facilityId">The facility identifier.</param>
        /// <returns></returns>
        public JsonResult GetFacilityPhycisian(int coporateId, int facilityId)
        {
            var lst = _service.GetFacilityPhysicians(facilityId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhysicianController.cs" company="Spadez">
//   Omni Health care
// </copyright>
// <summary>
//   Defines the PhysicianController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Ajax.Utilities;

namespace BillingSystem.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using BillingSystem.Bal.BusinessAccess;
    using BillingSystem.Common;
    using BillingSystem.Model;
    using BillingSystem.Model.CustomModel;
    using BillingSystem.Models;
    using System.Threading.Tasks;

    /// <summary>
    /// The physician controller.
    /// </summary>
    public class PhysicianController : BaseController
    {

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            var bal = new PhysicianBal();

            var facilityId = Helpers.GetDefaultFacilityId();
            var list = await bal.GetFacultyList(facilityId, Helpers.GetLoggedInUserId());

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
            var bal = new PhysicianBal();
            var facilityId = Helpers.GetDefaultFacilityId();
            var physicianList = await bal.GetFacultyList(facilityId, Helpers.GetLoggedInUserId());
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
                var bal = new PhysicianBal();
                var isExists = bal.CheckDuplicateEmpNo(Convert.ToInt32(vm.PhysicianEmployeeNumber), vm.Id);
                if (isExists)
                    return Json("-1");

                isExists = bal.CheckDuplicateClinicalId(vm.PhysicianLicenseNumber, vm.Id);
                if (isExists)
                    return Json("-2");


                // Check if User Type and User ID already exists in the Physician Table
                isExists = bal.CheckIfUserTypeAndUserIdAlreadyExists(Convert.ToInt32(vm.UserType),
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

                newId = bal.AddUpdatePhysician(vm);
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
                var bal = new PhysicianBal();

                deletedId = bal.AddUpdatePhysician(vm);

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
            var bal = new PhysicianBal();
            var current = bal.GetPhysicianCModelById(physicianId);
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
            var bal = new FacilityBal();
            var name = bal.GetFacilityNameById(Convert.ToInt32(facilityId));
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
            var bal = new FacilityBal();
            var cId = Helpers.GetSysAdminCorporateID();
            var facilityList = bal.GetFacilitiesByCorpoarteId(cId);
            var list = facilityList.Select(item => new SelectListItem { Text = item.FacilityName, Value = item.FacilityId.ToString() }).ToList();

            return Json(list);
        }

        /// <summary>
        /// Binds the physicianddl list.
        /// </summary>
        /// <returns>Json List</returns>
        public ActionResult BindPhysicianddlList()
        {
            var bal = new PhysicianBal();
            var facilityId = Helpers.GetDefaultFacilityId();
            var physicianList = bal.GetFacilityPhysicians(facilityId);
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
            using (var fRole = new FacilityRoleBal())
            {
                var list = new List<DropdownListData>();
                var corporateId = Helpers.GetSysAdminCorporateID();

                var facilityId = Helpers.GetDefaultFacilityId();
                var roleList = fRole.GetUserTypeRoleDropDown(corporateId, facilityId, true);
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

        }
        /// <summary>
        /// Method is used to get the physician list by patient id
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public JsonResult GetPhysicianListByPatientId(int patientId)
        {
            var list = new PhysicianBal().GetPhysicianListByPatientId(patientId);
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
            var list = new PhysicianBal().BindPhysicianBySpeciality(facilityId, specialityId);
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
            using (var fBal = new FacilityBal())
            {
                fList = fBal.GetFacilitiesForDashboards(fId, cId, Helpers.GetLoggedInUserIsAdmin());

                //Bind the physicians list to div #divPhysicianList
                var viewpath = string.Format("../Physician/{0}", PartialViews.FacilityListViewInPhysicianView);
                facilityListView = RenderPartialViewToStringBase(viewpath, fList);
            }





            //Get Users and UserRole data
            using (var uBal = new UsersBal())
            {
                uList = uBal.GetUsersByRole(fId, cId);
                if (uList.Count > 0)
                {
                    urList.AddRange(uList.DistinctBy(k => k.RoleId).Select(item => new DropdownListData
                    {
                        Text = item.RoleName,
                        Value = Convert.ToString(item.RoleId)
                    }));
                }
            }

            //Get License Types and Specialties Data
            using (var gcBal = new GlobalCodeBal())
            {
                var results = gcBal.GetListByCategoriesRange(categories, facilityId: Convert.ToString(Helpers.GetDefaultFacilityId()));
                if (results.Count > 0)
                {
                    //ltList = results.Where(g => g.ExternalValue1.Equals("1104")).ToList();
                    sList = results.Where(g => g.ExternalValue1.Equals("1121")).ToList();
                }
            }

            //Get Departments Data
            using (var bal = new FacilityStructureBal())
            {
                var deps = bal.GetFacilityDepartments(Helpers.GetSysAdminCorporateID(), Convert.ToString(fId));
                if (deps.Any())
                {
                    dList.AddRange(deps.Select(item => new DropdownListData
                    {
                        Text = string.Format(" {0} ", item.FacilityStructureName),
                        Value = Convert.ToString(item.FacilityStructureId)
                    }));
                }
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
            using (var gcBal = new GlobalCodeBal())
                ltList = gcBal.GetGCodesListByCategoryValue("1104", value, string.Empty);

            return Json(ltList, JsonRequestBehavior.AllowGet);
        }



        #region Clinician AppointmentTypes View
        public ViewResult ClinicianAppTypes()
        {
            using (var bal = new ClinicianAppointmentTypesBal())
            {
                return View(new ClinicianAppointmentTypesView
                {
                    List = bal.GetList(Helpers.GetDefaultFacilityId(), Helpers.GetLoggedInUserId(), 0),
                    CurrentClinician = new ClinicianAppTypesCustomModel()
                });
            }
        }

        public JsonResult BindClinicianAppointmentDataOnLoad()
        {
            using (var bal = new ClinicianAppointmentTypesBal())
            {
                var vm = bal.GetDataOnViewLoad(Helpers.GetDefaultFacilityId(), Helpers.GetLoggedInUserId());

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
        }

        public PartialViewResult BindClinicianAppointmentTypesData()
        {
            using (var bal = new ClinicianAppointmentTypesBal())
            {
                var list = bal.GetList(Helpers.GetDefaultFacilityId(), Helpers.GetLoggedInUserId(), 0);
                return PartialView(PartialViews.CAppointmentTypesPartial, list);
            }
        }

        public JsonResult SaveClinicianAppTypeData(ClinicianAppTypesCustomModel vm)
        {
            using (var bal = new ClinicianAppointmentTypesBal())
            {
                vm.CreatedBy = Helpers.GetLoggedInUserId();
                var result = bal.Save(vm);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult EditClinicianAppointmentTypesData(long clinicianId)
        {
            using (var bal = new ClinicianAppointmentTypesBal())
            {
                var current = bal.GetList(Helpers.GetDefaultFacilityId(), Helpers.GetLoggedInUserId(), clinicianId).FirstOrDefault();
                return Json(current, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

    }
}
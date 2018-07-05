// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Common.cs" company="SPadez">
//   OmniHealthcare
// </copyright>
// <summary>
//   The menu helpers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BillingSystem.Common
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Dynamic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.UI;
    using System.Xml;
    using System.Xml.Linq;
    using Common;
    using Model.CustomModel;
    using System.Web.Configuration;
    using Newtonsoft.Json.Serialization;
    using Newtonsoft.Json;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Unity;
    using BillingSystem.Bal.Interfaces;
    using BillingSystem.Models;


    // Menu Manipulations
    /// <summary>
    /// The menu helpers.
    /// </summary>
    public static class MenuHelpers
    {
        #region Public Methods and Operators

        /// <summary>
        /// The build value dictionary.
        /// </summary>
        /// <param name="routeValue">
        /// The route value.
        /// </param>
        /// <returns>
        /// The <see cref="RouteValueDictionary"/>.
        /// </returns>
        public static RouteValueDictionary BuildValueDictionary(string routeValue)
        {
            if (string.IsNullOrEmpty(routeValue))
            {
                return null;
            }

            var routeValueDictionary = new RouteValueDictionary();
            var routeCommaSepratedVales = routeValue.Split(',');
            if (routeCommaSepratedVales.Any())
            {
                foreach (var routeCommaSepratedVale in routeCommaSepratedVales)
                {
                    var routeParameter = routeCommaSepratedVale.Split('=');
                    if (routeParameter.Any())
                    {
                        var value = Convert.ToString(routeParameter[1]);
                        if (value.Contains("@Session"))
                        {
                            var sessionContent = value.ReplaceEx("@Session[", string.Empty, true)
                                .ReplaceEx("]", string.Empty, true);
                            var sessionValue = Convert.ToString(HttpContext.Current.Session[sessionContent]);
                            if (!string.IsNullOrEmpty(sessionValue))
                            {
                                routeParameter[1] = sessionValue;
                            }
                            else
                            {
                                routeParameter[1] = "0";
                            }
                        }

                        routeValueDictionary.Add(
                            Convert.ToString(routeParameter[0]),
                            Convert.ToString(routeParameter[1]));
                    }
                }

                return routeValueDictionary;
            }

            return null;
        }

        #endregion
    }

    /// <summary>
    /// The helpers.
    /// </summary>
    public static class Helpers
    {

        #region Public Properties

        /// <summary>
        /// Gets the default bill edit rule table number.
        /// </summary>
        public static string DefaultBillEditRuleTableNumber
        {
            get
            {
                if (HttpContext.Current != null
                    && HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
                {
                    var session = HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass;
                    return !string.IsNullOrEmpty(session.BillEditRuleTableNumber)
                               ? session.BillEditRuleTableNumber
                               : "0";
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the default cpt table number.
        /// </summary>
        public static string DefaultCptTableNumber
        {
            get
            {
                if (HttpContext.Current != null
                    && HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
                {
                    var session = HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass;
                    return !string.IsNullOrEmpty(session.CptTableNumber) ? session.CptTableNumber : string.Empty;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the default diagnosis table number.
        /// </summary>
        public static string DefaultDiagnosisTableNumber
        {
            get
            {
                if (HttpContext.Current != null
                    && HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
                {
                    var session = HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass;
                    return !string.IsNullOrEmpty(session.DiagnosisCodeTableNumber)
                               ? session.DiagnosisCodeTableNumber
                               : string.Empty;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the default drg table number.
        /// </summary>
        public static string DefaultDrgTableNumber
        {
            get
            {
                if (HttpContext.Current != null
                    && HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
                {
                    var session = HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass;
                    return !string.IsNullOrEmpty(session.DrgTableNumber) ? session.DrgTableNumber : string.Empty;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the default drug table number.
        /// </summary>
        public static string DefaultDrugTableNumber
        {
            get
            {
                if (HttpContext.Current != null
                    && HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
                {
                    var session = HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass;
                    return !string.IsNullOrEmpty(session.DrugTableNumber) ? session.DrugTableNumber : string.Empty;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the default hc pcs table number.
        /// </summary>
        public static string DefaultHcPcsTableNumber
        {
            get
            {
                if (HttpContext.Current != null
                    && HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
                {
                    var session = HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass;
                    return !string.IsNullOrEmpty(session.HcPcsTableNumber) ? session.HcPcsTableNumber : string.Empty;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the default language.
        /// </summary>
        public static string DefaultLanguage
        {
            get
            {
                if (HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
                {
                    var objSession = HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass;
                    if (objSession != null)
                    {
                        return !string.IsNullOrEmpty(objSession.SelectedCulture) ? objSession.SelectedCulture : "en-US";
                    }
                }

                return string.Empty;
            }

            set
            {
                if (HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
                {
                    var objSession = HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass;
                    if (objSession != null)
                    {
                        objSession.SelectedCulture = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the default record count.
        /// </summary>
        public static int DefaultRecordCount
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["DefaultRecordCount"]);
            }
        }

        /// <summary>
        /// Gets the default service code table number.
        /// </summary>
        public static string DefaultServiceCodeTableNumber
        {
            get
            {
                if (HttpContext.Current != null
                    && HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
                {
                    var session = HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass;
                    return !string.IsNullOrEmpty(session.ServiceCodeTableNumber)
                               ? session.ServiceCodeTableNumber
                               : string.Empty;
                }

                return string.Empty;
            }
        }

        public static int RunDeleteBillActivityInDiagnosis
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["RunDeleteBillActivityInDiagnosis"]);
            }
        }

        public static string BarCodeImagePathForSaving
        {
            get
            {
                return ConfigurationManager.AppSettings["BarCodeImagePathForSaving"];
            }
        }

        public static string CurrentRoleName
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
                    return (HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass).RoleName;
                return string.Empty;
            }
        }

        public static string CurrentRoleKey
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
                    return (HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass).RoleKey;
                return string.Empty;
            }
        }

        public static int DefaultC
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["DefaultC"]);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The compute user response.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <param name="returnResponse">
        /// The return response.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int ComputeUserResponse(string response, string returnResponse)
        {
            var computedResponse = 0;
            if (returnResponse == "1" || returnResponse == "2")
            {
                computedResponse = response != "no" ? 1 : 0;
            }
            else if (returnResponse == "4")
            {
                computedResponse = 2; // here two is for user has is in waiting list for the invitation. 
            }

            return computedResponse;
        }

        /// <summary>
        /// The convert to us date string.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ConvertToUsDateString(DateTime date)
        {
            var cDate = date.Day;
            var cMonth = date.Month;
            var cYear = date.Year;
            var cHour = date.Hour;
            var cMin = date.Minute;
            return cMonth + "/" + cDate + "/" + cYear + " " + cHour + ":" + cMin;
        }

        /// <summary>
        /// The create thumbnail modify.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        public static Image CreateThumbnailModify(Stream filename, int width, int height)
        {
            // http://west-wind.com/weblog/posts/283.aspx
            Bitmap bmpOut;
            try
            {
                // get bitmap from file (see also GetBitmapFromEmbeddedResource)
                using (var bmp = new Bitmap(filename))
                {
                    decimal ratio;
                    int newWidth;
                    int newHeight;

                    // if the height isn't defined, get one with the right aspect ratio 
                    if (height == 0)
                    {
                        ratio = (decimal)width / bmp.Width;
                        height = (int)(bmp.Height * ratio);
                    }

                    // If the image is smaller than a thumbnail just return it
                    if (bmp.Width < width && bmp.Height < height)
                    {
                        return new Bitmap(bmp); // return a copy as we dispose of the original
                    }

                    if (bmp.Width > bmp.Height)
                    {
                        // wide image
                        ratio = (decimal)width / bmp.Width;
                        newWidth = width;
                        newHeight = (int)(bmp.Height * ratio);
                    }
                    else
                    {
                        // tall image
                        ratio = (decimal)height / bmp.Height;
                        newHeight = height;
                        newWidth = (int)(bmp.Width * ratio);
                    }

                    // This code creates cleaner (though bigger) thumbnails and properly
                    // and handles GIF files better by generating a white background for
                    // transparent images (as opposed to black)
                    bmpOut = new Bitmap(newWidth, newHeight);

                    using (var g = Graphics.FromImage(bmpOut))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.FillRectangle(Brushes.White, 0, 0, newWidth, newHeight);
                        g.DrawImage(bmp, 0, 0, newWidth, newHeight);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return bmpOut;
        }

        /// <summary>
        /// The disable if.
        /// </summary>
        /// <param name="htmlString">
        /// The html string.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <returns>
        /// The <see cref="MvcHtmlString"/>.
        /// </returns>
        public static MvcHtmlString DisableIf(this MvcHtmlString htmlString, Func<bool> expression)
        {
            if (expression.Invoke())
            {
                var html = htmlString.ToString();
                const string disabled = "\"disabled\"";
                html = html.Insert(html.IndexOf(">", StringComparison.Ordinal), " disabled= " + disabled);
                return new MvcHtmlString(html);
            }

            return htmlString;
        }

        /// <summary>
        /// The external dashboard title view.
        /// </summary>
        /// <param name="dashboardTypeid">
        /// The dashboard typeid.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ExternalDashboardTitleView(string dashboardTypeid)
        {
            var title = "Dashboard";
            var reportingType = (DashboardType)Enum.Parse(typeof(DashboardType), dashboardTypeid);
            switch (reportingType)
            {
                case DashboardType.ExecutiveDashboard:
                    title = "Executive Dashboard";
                    break;
                case DashboardType.SummaryDashboard:
                    title = "Summary KPI";
                    break;
                case DashboardType.ClinicalQulaityDashboard:
                    //title = "Clinical Quality";
                    title = "Clinical";
                    break;
                case DashboardType.FinancialMGTDashboard:
                    title = "Financial Management";
                    break;
                case DashboardType.RevenueMGTDashboard:
                    title = "Revenue Management";
                    break;
                case DashboardType.CaseMGTDashboard:
                    title = "Case Management";
                    break;
                case DashboardType.HumanResourcesDashboard:
                    title = "Human Resources";
                    break;
                case DashboardType.ProjectsDashboard:
                    title = "Projects";
                    break;
                case DashboardType.ExecutiveKeyPerformanceDashboard:
                    title = "Executive Key Performance Dashboard";
                    break;
                case DashboardType.ClinicalCompliance:
                    title = "Patient Acquisition";
                    break;
                case DashboardType.BillScrubber:
                    title = "Bill Scrubber";
                    break;
                default:
                    break;
            }

            return title;
        }

        /// <summary>
        /// Firsts the date of week.
        /// </summary>
        /// <param name="year">
        /// The year.
        /// </param>
        /// <param name="weekNum">
        /// The week number.
        /// </param>
        /// <param name="rule">
        /// The rule.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime FirstDateOfWeek(int year, int weekNum, CalendarWeekRule rule)
        {
            Debug.Assert(weekNum >= 1);
            var jan1 = new DateTime(year, 1, 1);

            var daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
            var firstMonday = jan1.AddDays(daysOffset);
            Debug.Assert(firstMonday.DayOfWeek == DayOfWeek.Monday);

            var cal = CultureInfo.CurrentCulture.Calendar;
            var firstWeek = cal.GetWeekOfYear(firstMonday, rule, DayOfWeek.Monday);

            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }

            var result = firstMonday.AddDays(weekNum * 7);

            return result;
        }

        /// <summary>
        /// The generate grid data source.
        /// </summary>
        /// <param name="descendantName">
        /// The descendant name.
        /// </param>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<dynamic> GenerateGridDataSource(string descendantName, string filePath)
        {
            // initialize list of dynamic objects
            var dataList = new List<dynamic>();

            // xml-string
            var doc = XDocument.Load(filePath);

            // selecting all the Descendats of Ex: "Result"
            var descendants = doc.Descendants(descendantName);

            // loop through each Descendats
            foreach (var descendant in descendants)
            {
                // creatingthe ExpandoObject
                dynamic expandoObject = new ExpandoObject();

                // casting it to a dictionary object
                var dictionaryExpandoObject = (IDictionary<string, object>)expandoObject;

                // loop through each elements of descendant
                foreach (var element in descendant.Elements())
                {
                    // assiging of element name as propertyName
                    var propertyName = element.Name.LocalName;

                    // adding the property name and value to the dictionary
                    dictionaryExpandoObject.Add(propertyName, element.Value);
                }

                // finally add each ExpandoObject to list
                dataList.Add(dictionaryExpandoObject);
            }

            return dataList;
        }

        /// <summary>
        /// Generate 10 digit random number
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GenerateRandomNumber()
        {
            var rn = new Random(GetInvariantCultureDateTime().Ticks.GetHashCode());
            return "1000000" + rn.Next(1, 100);
        }

        /// <summary>
        /// Gets the corporate identifier by facility identifier.
        /// </summary>
        /// <param name="facilityId">
        /// The facility identifier.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        ///// </returns>
        public static int GetCorporateIdByFacilityId(int facilityId)
        {
            var container = UnityConfig.RegisterComponents();
            var service = container.Resolve<IFacilityService>();

            var facilityObj = service.GetFacilityById(facilityId);
            return facilityObj != null ? Convert.ToInt32(facilityObj.CorporateID) : 0;

        }

        /// <summary>
        /// Get Current week of the year on the basis of current date
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetCurrentWeekOfYearISO8601()
        {
            var day = (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(GetInvariantCultureDateTime());
            return
                CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                    GetInvariantCultureDateTime().AddDays(4 - (day == 0 ? 7 : day)),
                    CalendarWeekRule.FirstFourDayWeek,
                    DayOfWeek.Monday);
        }

        /// <summary>
        /// Gets the default corporate identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetDefaultCorporateId()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
            {
                var session = HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass;
                var cId = session.CorporateId;
                var roleId = session.RoleId;
                if (roleId == Convert.ToInt32(DefaultRoles.SysAdmin))
                {
                    return 0;
                }

                return cId;
            }

            return 0;
        }

        /// <summary>
        /// Gets the default facility identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetDefaultFacilityId()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
            {
                var facilityId = (HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass).FacilityId;
                return facilityId;
            }

            return 0;
        }

        /// <summary>
        /// Gets the default facility number.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetDefaultFacilityNumber()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
            {
                var facilityNumber =
                    (HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass).FacilityNumber;
                return facilityNumber;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the default role identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetDefaultRoleId()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
            {
                var session = HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass;
                if (session != null)
                {
                    var roleId = session.RoleId;
                    return Convert.ToString(roleId);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the type of the encounter patient.
        /// </summary>
        /// <param name="pType">
        /// Type of the p.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetEncounterPatientType(int pType)
        {
            if (pType > 0)
            {
                var encounterType = (EncounterPatientType)Enum.Parse(typeof(EncounterPatientType), pType.ToString());
                switch (encounterType)
                {
                    case EncounterPatientType.ERPatient:
                        return EncounterPatientType.ERPatient.ToString();
                    case EncounterPatientType.InPatient:
                        return EncounterPatientType.ERPatient.ToString();
                    case EncounterPatientType.OutPatient:
                        return EncounterPatientType.ERPatient.ToString();
                    default:
                        break;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// The get enum list.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public static List<string> GetEnumList<T>()
        {
            // validate that T is in fact an enum
            if (!typeof(T).IsEnum)
            {
                throw new InvalidOperationException();
            }

            return Enum.GetNames(typeof(T)).ToList();
        }

        /// <summary>
        /// Gets the facility identifier next default cororate facility.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetFacilityIdNextDefaultCororateFacility()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
            {
                var facilityId =
                    (HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass).FacilityId;

                var container = UnityConfig.RegisterComponents();
                var service = container.Resolve<IFacilityService>();

                var facilityObj = service.GetFacilityById(facilityId);
                var isFacilityDefaultCorporateFacility = facilityObj.LoggedInID != 0;
                if (isFacilityDefaultCorporateFacility)
                {
                    var corporatedId = GetSysAdminCorporateID();
                    var facilities =
                        service.GetFacilitiesByCorpoarteId(corporatedId)
                            .Where(x => x.FacilityId != facilityId)
                            .ToList()
                            .OrderBy(x => x.FacilityId);
                    facilityId = facilities.FirstOrDefault() != null ? facilities.FirstOrDefault().FacilityId : 0;
                }

                return facilityId;
            }

            return 0;
        }

        /// <summary>
        /// Gets the first dayof current month.
        /// </summary>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime GetFirstDayofCurrentMonth()
        {
            var currentDate = GetInvariantCultureDateTime();
            var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1).Date;
            return firstDayOfMonth;
        }

        /// <summary>
        /// Gets the invariant culture date time.
        /// </summary>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime GetInvariantCultureDateTime()
        {
            var facilityid = GetDefaultFacilityId();

            var container = UnityConfig.RegisterComponents();
            var service = container.Resolve<IFacilityService>();

            var facilityObj = service.GetFacilityTimeZoneById(facilityid);
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(facilityObj);
            var utcTime = DateTime.Now.ToUniversalTime();
            var convertedTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

            // d = DateTime.Parse(s, CultureInfo.InvariantCulture);
            // string strDate = localTime.ToString(FMT);
            // DateTime now2 = DateTime.ParseExact(strDate, FMT, CultureInfo.InvariantCulture);
            // DateTime now2 = DateTime.Parse(s, CultureInfo.InvariantCulture);
            return convertedTime;
        }


        //public static DateTime GetInvariantCultureDateTime(int fId)
        //{

        //    var container = UnityConfig.RegisterComponents();
        //    var service = container.Resolve<IFacilityService>();
        //    var facilityObj = service.GetFacilityTimeZoneById(fId);
        //    var tzi = TimeZoneInfo.FindSystemTimeZoneById(facilityObj);
        //    var utcTime = DateTime.Now.ToUniversalTime();
        //    var convertedTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
        //    return convertedTime;
        //}

        /// <summary>
        /// Gets the last day of current month.
        /// </summary>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime GetLastDayOfCurrentMonth()
        {
            var currentDate = GetInvariantCultureDateTime();
            var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1).Date;
            return lastDayOfMonth;
        }

        /// <summary>
        /// Gets the logged in user identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetLoggedInUserId()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
            {
                var session = HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass;
                var userid = session.UserId;
                return userid;
            }

            return 0;
        }

        public static bool CheckAccess(string controller, string action, bool isAjaxRequest = true)
        {
            //var access = false;
            //if (
            //    HttpContext.Current != null &&
            //    HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null
            //    && !string.IsNullOrEmpty(controller) && !string.IsNullOrEmpty(action)
            //    )
            //{
            //    controller = controller.ToLower().Trim();
            //    action = action.ToLower().Trim();

            //    var session = HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass;
            //    var menus = session.MenuSessionList;

            //    access = menus.Any(a => ((!string.IsNullOrEmpty(a.Controller) && a.Controller.ToLower().Trim().Equals(controller)
            //     && !string.IsNullOrEmpty(a.Action) && a.Action.ToLower().Trim().Equals(action)) || isAjaxRequest)
            //     && a.IsActive && !a.IsDeleted);
            //}

            //return access;
            return true;
        }

        public static long GetDefaultCountryCode
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
                {
                    var session = HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass;
                    var defaultCountryId = session.DefaultCountryId;
                    return defaultCountryId;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the logged in user is admin.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetLoggedInUserIsAdmin()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
            {
                var session = HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass;
                var userisAdmin = session.UserIsAdmin;
                return userisAdmin;
            }

            return false;
        }

        /// <summary>
        /// Gets the logged in username.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetLoggedInUsername()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
            {
                var session = HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass;
                return session.UserName;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the type of the login user.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetLoginUserType()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
            {
                var session = HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass;
                return session.LoginUserType;
            }

            return 0;
        }

        /// <summary>
        /// The get physicians by user role.
        /// </summary>
        /// <param name="userTypeId">
        /// The user type id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<DropdownListData> GetPhysiciansByUserRole(int userTypeId)
        {

            var container = UnityConfig.RegisterComponents();
            var service = container.Resolve<IPhysicianService>();

            var list = new List<DropdownListData>();
            var usersList = service.GetDistinctUsersByUserTypeId(
                userTypeId,
                GetSysAdminCorporateID(),
                GetDefaultFacilityId());
            if (usersList.Count > 0)
            {
                list.AddRange(
                    usersList.Select(
                        item =>
                        new DropdownListData
                        {
                            Text = string.Format("{0} {1}", item.FirstName, item.LastName),
                            Value = Convert.ToString(item.UserID)
                        }));
            }

            return list;
        }


        /// <summary>
        /// Gets the reporting type action.
        /// </summary>
        /// <param name="reportingTypeId">
        /// The reporting type identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetReportingTypeAction(string reportingTypeId)
        {
            var action = string.Empty;
            var reportingType = (ReportingTypeActions)Enum.Parse(typeof(ReportingTypeActions), reportingTypeId);

            switch (reportingType)
            {
                case ReportingTypeActions.UserLoginActivityPdf:
                    action = ReportingTypeActions.UserLoginActivityPdf.ToString();
                    break;
                case ReportingTypeActions.PasswordDisablesLogPdf:
                    action = ReportingTypeActions.PasswordDisablesLogPdf.ToString();
                    break;
                case ReportingTypeActions.PasswordChangeLogPdf:
                    action = ReportingTypeActions.PasswordChangeLogPdf.ToString();
                    break;
                case ReportingTypeActions.DailyChargeReportPdf:
                    action = ReportingTypeActions.DailyChargeReportPdf.ToString();
                    break;
                case ReportingTypeActions.ChargesByPayorReportPdf:
                    action = ReportingTypeActions.ChargesByPayorReportPdf.ToString();
                    break;
                case ReportingTypeActions.PayorWiseAgeingReport:
                    action = ReportingTypeActions.PayorWiseAgeingReport.ToString();
                    break;
                case ReportingTypeActions.PatientWiseAgeingReport:
                    action = ReportingTypeActions.PatientWiseAgeingReport.ToString();
                    break;
                case ReportingTypeActions.DepartmentWiseAgeingReport:
                    action = ReportingTypeActions.DepartmentWiseAgeingReport.ToString();
                    break;
                case ReportingTypeActions.PayorWiseReconciliationReport:
                    action = ReportingTypeActions.PayorWiseReconciliationReport.ToString();
                    break;
                case ReportingTypeActions.PatientWiseReconciliationReport:
                    action = ReportingTypeActions.PatientWiseReconciliationReport.ToString();
                    break;
                case ReportingTypeActions.DepartmentWiseReconciliationReport:
                    action = ReportingTypeActions.DepartmentWiseReconciliationReport.ToString();
                    break;
                case ReportingTypeActions.CorrectionLog:
                    action = ReportingTypeActions.CorrectionLog.ToString();
                    break;
                default:
                    action = ReportingTypeActions.UserLoginActivityPdf.ToString();
                    break;
            }

            return action;
        }

        /// <summary>
        /// Get Short Date from DateTime variable in Short Date String in Format 'MM/dd/yyyy'
        /// </summary>
        /// <param name="obj">
        /// Datetime value
        /// </param>
        /// <returns>
        /// short date string in format 'MM/dd/yyyy'
        /// </returns>
        public static string GetShortDateString1(this DateTime? obj)
        {
            return obj.HasValue ? obj.Value.ToString("d") : string.Empty;
        }

        /// <summary>
        /// </summary>
        /// <param name="obj">
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetShortDateString3(this DateTime? obj)
        {
            return obj.HasValue ? obj.Value.ToString("MM/dd/yyyy") : string.Empty;
        }

        public static string GetDateTimeString24HoursFormat(this DateTime? obj)
        {
            return obj.HasValue ? obj.Value.ToString("g") : string.Empty;
        }

        public static string GetDateTimeString24HoursFormat(this DateTime obj)
        {
            return obj.ToString("g");
        }

        /// <summary>
        /// Gets the system admin corporate identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetSysAdminCorporateID()
        {
            var cId = GetDefaultCorporateId();
            var userId = GetLoggedInUserId();
            if (cId == 0 && userId > 0)
            {
                return 6;
            }

            return cId;
        }

        /// <summary>
        /// </summary>
        /// <param name="obj">
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetTimeStringFromDateTime(this DateTime? obj)
        {
            return obj.HasValue ? obj.Value.ToString("HH:mm") : string.Empty;
        }

        /// <summary>
        /// Gets the user_ ip.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetUser_IP()
        {
            var VisitorsIPAddr = string.Empty;
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                {
                    VisitorsIPAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                }
                else if (!string.IsNullOrEmpty(HttpContext.Current.Request.UserHostAddress))
                {
                    VisitorsIPAddr = HttpContext.Current.Request.UserHostAddress;
                }

                if (string.IsNullOrEmpty(VisitorsIPAddr))
                {
                    VisitorsIPAddr = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (string.IsNullOrEmpty(VisitorsIPAddr))
                {
                    VisitorsIPAddr = HttpContext.Current.Request.UserHostAddress;
                }
            }

            return VisitorsIPAddr;
        }

        /// <summary>
        /// Gets the week of year is o8601.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetWeekOfYearISO8601(DateTime date)
        {
            var day = (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(date);
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                date.AddDays(4 - (day == 0 ? 7 : day)),
                CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday);
        }

        /// <summary>
        /// Gets the day of the week.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static int GetDayOfTheWeek(DateTime date)
        {
            var day = (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(date);
            return day == 0 ? 7 : day;
        }

        /// <summary>
        /// The get xml.
        /// </summary>
        /// <param name="xmlFileUrl">
        /// The xml file url.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetXML(string xmlFileUrl)
        {
            try
            {

                using (var xmlreader = new XmlTextReader(xmlFileUrl))
                {
                    var ds = new DataSet();
                    ds.ReadXml(xmlreader);
                    return ds.GetXml();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// The image to byte.
        /// </summary>
        /// <param name="img">
        /// The img.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public static byte[] ImageToByte(Image img)
        {
            var converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        /// <summary>
        /// Orders the by.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="sortExpression">
        /// The sort expression.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        /// <exception cref="System.Exception">
        /// No property ' + property + ' in +  + typeof(T).Name + '
        /// </exception>
        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> list, string sortExpression)
        {
            sortExpression += string.Empty;
            var parts = sortExpression.Split(' ');
            var descending = false;
            var property = string.Empty;

            if (parts.Length > 0 && parts[0] != string.Empty)
            {
                property = parts[0];

                if (parts.Length > 1)
                {
                    descending = parts[1].ToLower().Contains("esc");
                }

                var prop = typeof(T).GetProperty(property);

                if (prop == null)
                {
                    throw new Exception("No property '" + property + "' in + " + typeof(T).Name + "'");
                }

                if (descending)
                {
                    return list.OrderByDescending(x => prop.GetValue(x, null));
                }

                return list.OrderBy(x => prop.GetValue(x, null));
            }

            return list;
        }

        /// <summary>
        /// The parse value to invariant date time.
        /// </summary>
        /// <param name="dtValue">
        /// The dt value.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime ParseValueToInvariantDateTime(string dtValue)
        {
            var datetimeValue = DateTime.ParseExact(dtValue, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return datetimeValue;
        }

        /// <summary>
        /// The render partial to string.
        /// </summary>
        /// <param name="controlName">
        /// The control name.
        /// </param>
        /// <param name="viewData">
        /// The view data.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string RenderPartialToString(string controlName, object viewData)
        {
            var viewPage = new ViewPage { ViewContext = new ViewContext(), ViewData = new ViewDataDictionary(viewData) };
            viewPage.Controls.Add(viewPage.LoadControl(controlName));
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                using (var tw = new HtmlTextWriter(sw))
                {
                    viewPage.RenderControl(tw);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Reportings the title view.
        /// </summary>
        /// <param name="reportingTitleId">
        /// The reporting title identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ReportingTitleView(string reportingTitleId)
        {
            var title = "Reporting View";
            var reportingType = (ReportingType)Enum.Parse(typeof(ReportingType), reportingTitleId);
            switch (reportingType)
            {
                case ReportingType.UserLoginActivity:
                    title = "User Login Activity";
                    break;
                case ReportingType.PasswordDisablesLog:
                    title = "Password Disables Log";
                    break;
                case ReportingType.PasswordChangeLog:
                    title = "Password Change Log";
                    break;
                case ReportingType.DailyChargeReport:
                    title = "Daily Charges Report";
                    break;
                case ReportingType.ChargesByPayorReport:
                    title = "Charges by Payor Report";
                    break;
                case ReportingType.PayorWiseAgeingReport:
                    title = "Payor Wise Aging Report";
                    break;
                case ReportingType.PatientWiseAgeingReport:
                    title = "Patient Wise Aging Report";
                    break;
                case ReportingType.DepartmentWiseAgeingReport:
                    title = "Department Wise Aging Report";
                    break;
                case ReportingType.PayorWiseReconciliationReport:
                    title = "Payor Wise Reconciliation Report";
                    break;
                case ReportingType.PatientWiseReconciliationReport:
                    title = "Patient Wise Reconciliation Report";
                    break;
                case ReportingType.DepartmentWiseReconciliationReport:
                    title = "Department Wise Reconciliation Report";
                    break;
                case ReportingType.JournalEntrySupport:
                    title = "Journal Entry Support Charges/Acct Receivable";
                    break;
                case ReportingType.JournalEntrySupportMCD:
                    title = "Journal Entry Support Managed Care Discounts";
                    break;
                case ReportingType.JournalEntrySupportWO:
                    title = "Journal Entry Support Bad Debt Writeoffs";
                    break;
                case ReportingType.CorrectionLog:
                    title = "Correction Log";
                    break;
                case ReportingType.ChargeReport:
                    title = "Department Summary Report";
                    break;
                case ReportingType.ChargeDetailReport:
                    title = "Daily Charge Report";
                    break;
                case ReportingType.PhysicianUtilization:
                    title = "Physician Utilization Report";
                    break;
                case ReportingType.DepartmentUtilization:
                    title = "Department Utilization Report";
                    break;
                case ReportingType.ErrorDetail:
                    title = "Error Detail Report";
                    break;
                case ReportingType.ErrorSummary:
                    title = "Error Summary Report";
                    break;
                case ReportingType.ScrubbeSummary:
                    title = "Scrubbe Summary Report";
                    break;
                case ReportingType.ClaimTransactionDetailReport:
                    title = "Bill Transmission Report";
                    break;
                case ReportingType.PhysicianActivityReport:
                    title = "Physician Activity Report";
                    break;
                default:
                    break;
            }

            return title;
        }

        /// <summary>
        /// The resolve url.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ResolveUrl(string url)
        {
            var outurl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + CommonConfig.SiteUrl
                            + url;

            return outurl;
        }

        /// <summary>
        /// The resolve url 2.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ResolveUrl2(string url)
        {
            var outurl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + url;
            return outurl;
        }

        /// <summary>
        /// Gets the default facility identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int SetDropDownSelectedFacilityId()
        {
            if (HttpContext.Current.Session[SessionNames.SelectedFacilityId.ToString()] != null)
            {
                return Convert.ToInt32(HttpContext.Current.Session[SessionNames.SelectedFacilityId.ToString()]);
            }

            return 0;
        }

        /// <summary>
        /// The show till date field in reports.
        /// </summary>
        /// <param name="reportingId">
        /// The reporting id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ShowTillDateFieldInReports(string reportingId)
        {
            var isShow = true;
            var reportingType = (ReportingType)Enum.Parse(typeof(ReportingType), reportingId);
            switch (reportingType)
            {
                // case ReportingType.UserLoginActivity:
                // case ReportingType.PasswordDisablesLog:
                // case ReportingType.PasswordChangeLog:
                // case ReportingType.DailyChargeReport:
                // case ReportingType.ChargesByPayorReport:
                // break;
                case ReportingType.PayorWiseAgeingReport:
                case ReportingType.PatientWiseAgeingReport:
                case ReportingType.DepartmentWiseAgeingReport:
                case ReportingType.PayorWiseReconciliationReport:
                case ReportingType.PatientWiseReconciliationReport:
                case ReportingType.DepartmentWiseReconciliationReport:

                    isShow = false;
                    break;
            }

            return isShow;
        }

        /// <summary>
        /// The show view type field in reports.
        /// </summary>
        /// <param name="reportingId">
        /// The reporting id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ShowViewTypeFieldInReports(string reportingId)
        {
            var isShow = false;
            var reportingType = (ReportingType)Enum.Parse(typeof(ReportingType), reportingId);
            switch (reportingType)
            {
                case ReportingType.PayorWiseReconciliationReport:
                case ReportingType.PatientWiseReconciliationReport:
                case ReportingType.DepartmentWiseReconciliationReport:
                    isShow = true;
                    break;
            }

            return isShow;
        }

        /// <summary>
        /// The to client time.
        /// </summary>
        /// <param name="dt">
        /// The dt.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime ToClientTime(this DateTime dt)
        {
            var timeOffSet = HttpContext.Current.Session["UtcDiffWithuniversalTime"]; // read the value from session
            if (timeOffSet != null)
            {
                var offset = int.Parse(timeOffSet.ToString());
                if (Convert.ToString(HttpContext.Current.Session["IsDayLight"]) == "true")
                {
                    offset = offset - 1;
                }

                dt = dt.AddMinutes(-1 * offset);
                return dt;
            }

            // if there is no offset in session return the datetime in server timezone
            return dt.ToLocalTime();
        }

        /// <summary>
        /// Get total hours and day between start and end time
        /// </summary>
        /// <param name="startDateTime">
        /// The start date time.
        /// </param>
        /// <param name="enDateTime">
        /// The en date time.
        /// </param>
        /// <returns>
        /// The <see cref="TimeSpan"/>.
        /// </returns>
        public static TimeSpan TotalTimeOfActivity(DateTime startDateTime, DateTime enDateTime)
        {
            var span = enDateTime - startDateTime;
            return span;
        }

        /// <summary>
        /// Gets the default name of the facility.
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultFacilityName()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session[SessionNames.SessionClass.ToString()] != null)
            {
                var facilityName =
                    (HttpContext.Current.Session[SessionNames.SessionClass.ToString()] as SessionClass).FacilityName;
                return facilityName;
            }

            return string.Empty;
        }

        /// <summary>
        /// Generates the custom random number.
        /// </summary>
        /// <returns></returns>
        public static string GenerateCustomRandomNumber()
        {
            var rn = new Random(GetInvariantCultureDateTime().Ticks.GetHashCode());
            return string.Empty + Math.Abs(rn.Next(int.MinValue, int.MaxValue));
        }

        public static async Task<bool> SendAppointmentNotification(List<SchedulingCustomModel> scheduling, string toEmailAddress, string emailTemplateId, int patientId, int physicianId, int viewType)
        {
            if (scheduling.Count > 0)
            {
                var proceTypesHtml = string.Empty;
                var appTemplate =
                    ResourceKeyValues.GetFileText(
                        Convert.ToString(SchedularNotificationTypes.appointmenttypessubtemplate));
                proceTypesHtml = (from item in scheduling

                                  let html = appTemplate
                                  let timefrom =
                                      item.ScheduleFrom.HasValue ? Convert.ToString(item.ScheduleFrom.Value.TimeOfDay) : string.Empty

                                  //let date =
                                  //    item.ScheduleFrom.HasValue ? Convert.ToString(item.ScheduleFrom.Value.Date.ToShortDateString()) : string.Empty
                                  let date =
                                     item.ScheduleFrom.HasValue ? string.Format("{00:MMM. dd,yyyy}", item.ScheduleFrom) : string.Empty
                                  let timeto =
                                      item.ScheduleTo.HasValue ? Convert.ToString(item.ScheduleTo.Value.TimeOfDay) : string.Empty

                                  select
                                      html.Replace("{timefrom}", timefrom)
                                          .Replace("{longDate}", date)
                                          .Replace("{timeto}", timeto)
                                          .Replace("{patient}", item.PatientName)
                                          .Replace("{appointmenttype}", item.AppointmentType)).Aggregate(proceTypesHtml,
                                (current, html) => current + html);
                var mainTemplate = string.Empty;
                var cancelLink = string.Empty;
                var confirmLink = string.Empty;
                if (viewType == 2 && scheduling[0].SchedulingId == 0)
                {
                    mainTemplate =
                        ResourceKeyValues.GetFileText(
                            Convert.ToString(SchedularNotificationTypes.fromschedulartopatientonnewbooking));
                    confirmLink =
                   ResolveUrl2(string.Format("/Home/ConfirmationView?st=2&vtoken={0}&vType={1}&patientId={2}&physicianId={3}&bit=true",
                       scheduling[0].ExtValue4, viewType, patientId, physicianId));

                    cancelLink =
                       ResolveUrl2(string.Format("/Home/ConfirmationView?st=4&vtoken={0}&vType={1}&patientId={2}&physicianId={3}&bit=false",
                           scheduling[0].ExtValue4, viewType, patientId, physicianId));

                    mainTemplate = mainTemplate.Replace("{imagesUrl}",
                        HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority));
                }
                if (viewType == 2 && scheduling[0].SchedulingId > 0)
                {
                    mainTemplate =
                       ResourceKeyValues.GetFileText(
                           Convert.ToString(SchedularNotificationTypes.physicianapporovelemail));
                    confirmLink =
                   ResolveUrl2(string.Format("/Home/ConfirmationView?st=2&vtoken={0}&vType={1}&patientId={2}&physicianId={3}&bit=false",
                       scheduling[0].ExtValue4, viewType, patientId, physicianId));

                    cancelLink =
                       ResolveUrl2(string.Format("/Home/ConfirmationView?st=4&vtoken={0}&vType={1}&patientId={2}&physicianId={3}&bit=true",
                           scheduling[0].ExtValue4, viewType, patientId, physicianId));

                    mainTemplate = mainTemplate.Replace("{imagesUrl}",
                        HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority));
                }
                if (viewType == 5)
                {
                    mainTemplate =
                       ResourceKeyValues.GetFileText(
                           Convert.ToString(SchedularNotificationTypes.physiciancancelappointment));
                    mainTemplate = mainTemplate.Replace("{imagesUrl}",
                       HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority));
                }
                if (viewType == 4)
                {
                    mainTemplate =
                          ResourceKeyValues.GetFileText(
                           Convert.ToString(SchedularNotificationTypes.appointmentapprovedbyphysician));
                    mainTemplate = mainTemplate.Replace("{imagesUrl}",
                    HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority));
                }
                //confirmLink =
                //   ResolveUrl2(string.Format("/Home/ConfirmationView?st=2&vtoken={0}&vType={1}&patientId={2}&physicianId={3}&bit=true",
                //       scheduling[0].ExtValue4, viewType, patientId, physicianId));

                //cancelLink =
                //   ResolveUrl2(string.Format("/Home/ConfirmationView?st=4&vtoken={0}&vType={1}&patientId={2}&physicianId={3}&bit=false",
                //       scheduling[0].ExtValue4, viewType, patientId, physicianId));

                mainTemplate = mainTemplate.Replace("{procedures}", proceTypesHtml)
                    .Replace("{confirmurl}", confirmLink)
                    .Replace("{physician}", scheduling[0].PhysicianName)
                    .Replace("{patient}", scheduling[0].PatientName)
                    .Replace("{Cancel}", cancelLink);

                var emailInfo = new EmailInfo
                {
                    Email = toEmailAddress,
                    Subject = ResourceKeyValues.GetKeyValue("fromschedulartopatientonnewbookingsub"),
                    MessageBody = mainTemplate
                };

                var re = await MailHelper.SendEmailAsync(emailInfo);//.Equals(true) ? true : false;
                return re;
            }
            return false;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Owner:Shivani
        /// Purpose: to find max substring length
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>
        /// The <see cref="int" />.
        /// </returns>
        private static int GetEmailMaskLength(string username)
        {
            var emailSubstringLength = 8;
            if (!string.IsNullOrEmpty(username))
            {
                if (username.Length <= emailSubstringLength)
                {
                    emailSubstringLength = username.Length - 1;
                }
            }

            return emailSubstringLength;
        }

        #endregion

        public static DateTime GetLinkerTime(this Assembly assembly, TimeZoneInfo target = null)
        {
            var filePath = assembly.Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;

            var buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                stream.Read(buffer, 0, 2048);

            var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            var tz = target ?? TimeZoneInfo.Local;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

            return localTime;
        }

        public static long CurrentAssemblyTicks
        {
            get
            {
                var tt = Assembly.GetExecutingAssembly().GetLinkerTime().Ticks;
                return tt;
            }
        }
        public static DataTable ToDataTable<T>(List<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            var Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (var item in items)
            {
                var values = new object[Props.Length];
                for (var i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }


        /// <summary>
        /// Reportings the title view.
        /// </summary>
        /// <param name="reportingTitleId">
        /// The reporting title identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string XmlReportingTitleView(string reportingTitleId)
        {
            var title = "XML Reporting View";
            var reportingType = (XmlReportingType)Enum.Parse(typeof(XmlReportingType), reportingTitleId);
            switch (reportingType)
            {
                case XmlReportingType.BatchReport:
                    title = "Batch Report";
                    break;
                case XmlReportingType.XmlReportingInitialClaimErrorReport:
                    title = "Initial Claim Error Report";
                    break;
                default:
                    break;
            }

            return title;
        }


        public static void EncryptConnString()
        {
            var config = WebConfigurationManager.OpenWebConfiguration("~");
            var section = config.GetSection("connectionStrings");

            if (!section.SectionInformation.IsProtected)
            {
                section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                config.Save();
            }
        }

        public static void EncryptMailSettings()
        {
            var config = WebConfigurationManager.OpenWebConfiguration("~");
            var section = config.GetSection("system.net/mailSettings/smtp");

            if (!section.SectionInformation.IsProtected)
            {
                section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                config.Save();
            }
        }

        public static int GetAgeByDate(DateTime dateValue)
        {
            var age = DateTime.Now - dateValue;
            return (int)(age.Days / 365.25);
        }


        public static DataTable FillDataTableToImport(DataTable dt, string codeType, out int status)
        {
            status = (int)ExcelImportResultCodes.Success;
            var dateFormat2 = false;

            var dt1 = new DataTable();
            if (dt.Rows.Count > 0)
            {
                dt1.Columns.Add("Id", typeof(long));
                dt1.Columns.Add("TableNumber", typeof(string));
                dt1.Columns.Add("Code", typeof(string));
                dt1.Columns.Add("CodeDescription", typeof(string));
                dt1.Columns.Add("Price", typeof(string));
                dt1.Columns.Add("CodeGroup", typeof(string));
                dt1.Columns.Add("OtherValue1", typeof(string));
                dt1.Columns.Add("OtherValue2", typeof(string));
                dt1.Columns.Add("OtherValue3", typeof(string));
                dt1.Columns.Add("OtherValue4", typeof(string));
                dt1.Columns.Add("OtherValue5", typeof(string));
                dt1.Columns.Add("OtherValue6", typeof(string));
                dt1.Columns.Add("EffectiveFrom", typeof(DateTime));
                dt1.Columns.Add("EffectiveTill", typeof(DateTime));

                try
                {
                    switch (codeType.ToLower())
                    {
                        case "cpt":
                            ValidateBillingFile(dt, "CodeNumbering", "CodeEffectiveDate", "CodeExpiryDate", out status, out dateFormat2);
                            if (status != (int)ExcelImportResultCodes.Success)
                                return null;

                            foreach (DataRow dr in dt.Rows)
                            {
                                DateTime? fDate = null;
                                DateTime? tillDate = null;

                                if (!string.IsNullOrEmpty(Convert.ToString(dr["CodeEffectiveDate"])))
                                    fDate = dateFormat2 ? DateTime.Parse(Convert.ToString(dr["CodeEffectiveDate"]))
                                        : DateTime.FromOADate(Convert.ToDouble(dr["CodeEffectiveDate"]));

                                if (!string.IsNullOrEmpty(Convert.ToString(dr["CodeExpiryDate"])))
                                    tillDate = dateFormat2 ? DateTime.Parse(Convert.ToString(dr["CodeExpiryDate"]))
                                        : DateTime.FromOADate(Convert.ToDouble(dr["CodeExpiryDate"]));

                                dt1.Rows.Add(0,
                                    Convert.ToString(dr["TableNumber"]),
                                    Convert.ToString(dr["CodeNumbering"]),
                                    Convert.ToString(dr["CodeDescription"]),
                                    Convert.ToString(dr["CodePrice"]),
                                    Convert.ToString(dr["CodeGroup"]),
                                    Convert.ToString(dr["TableDescription"]), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                                    fDate, tillDate
                                    );
                            }
                            break;
                        case "hcpcs":
                            ValidateBillingFile(dt, "CodeNumbering", "CodeEffectiveDate", "CodeExpiryDate", out status, out dateFormat2);
                            if (status != (int)ExcelImportResultCodes.Success)
                                return null;

                            foreach (DataRow dr in dt.Rows)
                            {
                                DateTime? fDate = null;
                                DateTime? tillDate = null;

                                if (!string.IsNullOrEmpty(Convert.ToString(dr["CodeEffectiveDate"])))
                                    fDate = dateFormat2 ? DateTime.Parse(Convert.ToString(dr["CodeEffectiveDate"]))
                                        : DateTime.FromOADate(Convert.ToDouble(dr["CodeEffectiveDate"]));

                                if (!string.IsNullOrEmpty(Convert.ToString(dr["CodeExpiryDate"])))
                                    tillDate = dateFormat2 ? DateTime.Parse(Convert.ToString(dr["CodeExpiryDate"])) :
                                        DateTime.FromOADate(Convert.ToDouble(dr["CodeExpiryDate"]));

                                dt1.Rows.Add(0,
                                    Convert.ToString(dr["TableNumber"]),
                                    Convert.ToString(dr["CodeNumbering"]),
                                    Convert.ToString(dr["CodeDescription"]),
                                    Convert.ToString(dr["CodePrice"]),
                                    string.Empty, Convert.ToString(dr["TableDescription"]), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                                    fDate, tillDate
                                    );
                            }
                            break;
                        case "drug":
                            ValidateBillingFile(dt, "DrugCode", "LastChangeOn", "DeletedDate", out status, out dateFormat2);
                            if (status != (int)ExcelImportResultCodes.Success)
                                return null;

                            foreach (DataRow dr in dt.Rows)
                            {
                                DateTime? fDate = null;
                                DateTime? tillDate = null;

                                if (!string.IsNullOrEmpty(Convert.ToString(dr["LastChangeOn"])))
                                    fDate = dateFormat2 ? DateTime.Parse(Convert.ToString(dr["LastChangeOn"])) : DateTime.FromOADate(Convert.ToDouble(dr["LastChangeOn"]));

                                if (!string.IsNullOrEmpty(Convert.ToString(dr["DeletedDate"])))
                                    tillDate = dateFormat2 ? DateTime.Parse(Convert.ToString(dr["DeletedDate"]))
                                        : DateTime.FromOADate(Convert.ToDouble(dr["DeletedDate"]));

                                dt1.Rows.Add(0,
                                    Convert.ToString(dr["TableNumber"]),
                                    Convert.ToString(dr["DrugCode"]),
                                    Convert.ToString(dr["GenericName"]),
                                    string.Empty,   //Price
                                    Convert.ToString(dr["PackageName"]),
                                    Convert.ToString(dr["PricePharmacy"]),
                                    Convert.ToString(dr["PricePublic"]),
                                    Convert.ToString(dr["UnitPricePharmacy"]),
                                    Convert.ToString(dr["UnitPricePublic"]),
                                    Convert.ToString(dr["Dosage"]),
                                    Convert.ToString(dr["Strength"]),
                                    fDate, tillDate
                                    );
                            }
                            break;
                        case "diagnosis":
                            ValidateBillingFile(dt, "DiagnosisCode", "DiagnosisEffectiveStartDate", "DiagnosisEffectiveEndDate", out status, out dateFormat2);
                            if (status != (int)ExcelImportResultCodes.Success)
                                return null;

                            foreach (DataRow dr in dt.Rows)
                            {

                                DateTime? fDate = null;
                                if (!string.IsNullOrEmpty(Convert.ToString(dr["DiagnosisEffectiveStartDate"])))
                                    fDate = dateFormat2 ? DateTime.Parse(Convert.ToString(dr["DiagnosisEffectiveStartDate"]))
                                        : DateTime.FromOADate(Convert.ToDouble(dr["DiagnosisEffectiveStartDate"]));

                                DateTime? tillDate = null;
                                if (!string.IsNullOrEmpty(Convert.ToString(dr["DiagnosisEffectiveEndDate"])))
                                    tillDate = dateFormat2
                                        ? DateTime.Parse(Convert.ToString(dr["DiagnosisEffectiveEndDate"])) :
                                        DateTime.FromOADate(Convert.ToDouble(dr["DiagnosisEffectiveEndDate"]));

                                if (fDate > tillDate)
                                {
                                    status = (int)ExcelImportResultCodes.DatesInCorrect;
                                    break;
                                }

                                //dt1.Rows.Add(0,
                                //   Convert.ToString(dr["TableNumber"]),
                                //   Convert.ToString(dr["DiagnosisCode"]),
                                //   Convert.ToString(dr["FullDescription"]),
                                //   string.Empty,
                                //   Convert.ToString(dr["CodeGroup"]),
                                //   Convert.ToString(dr["OtherValue1"]),
                                //   string.Empty, Convert.ToString(dr["TableDescription"]), Convert.ToString(dr["TableDescription"]), string.Empty, string.Empty,
                                //   fDate, tillDate
                                //   );


                                dt1.Rows.Add(0,
                                    Convert.ToString(dr["TableNumber"]),
                                    Convert.ToString(dr["DiagnosisCode"]),
                                    Convert.ToString(dr["ShortDescription"]),
                                    string.Empty,
                                    string.Empty,
                                    Convert.ToString(dr["DiagnosisMediumDescription"]),
                                    Convert.ToString(dr["FullDescription"]),
                                    string.Empty,
                                    Convert.ToString(dr["TableDescription"]),
                                    Convert.ToString(dr["TableDescription"]),
                                    string.Empty,
                                    fDate, tillDate
                                    );
                            }
                            break;
                        case "drg":
                            ValidateBillingFile(dt, "CodeNumbering", "CodeEffectiveDate", "CodeExpiryDate", out status, out dateFormat2);
                            if (status != (int)ExcelImportResultCodes.Success)
                                return null;

                            foreach (DataRow dr in dt.Rows)
                            {
                                DateTime? fDate = null;
                                if (!string.IsNullOrEmpty(Convert.ToString(dr["CodeEffectiveDate"])))
                                    fDate = dateFormat2 ? DateTime.Parse(Convert.ToString(dr["CodeEffectiveDate"]))
                                        : DateTime.FromOADate(Convert.ToDouble(dr["CodeEffectiveDate"]));

                                DateTime? tillDate = null;
                                if (!string.IsNullOrEmpty(Convert.ToString(dr["CodeExpiryDate"])))
                                    tillDate = dateFormat2 ? DateTime.Parse(Convert.ToString(dr["CodeExpiryDate"]))
                                        : DateTime.FromOADate(Convert.ToDouble(dr["CodeExpiryDate"]));

                                if (fDate > tillDate)
                                {
                                    status = (int)ExcelImportResultCodes.DatesInCorrect;
                                    break;
                                }

                                dt1.Rows.Add(0,
                                    Convert.ToString(dr["TableNumber"]),
                                    Convert.ToString(dr["CodeNumbering"]),
                                    Convert.ToString(dr["CodeDescription"]),
                                    Convert.ToString(dr["CodePrice"]),
                                    string.Empty,
                                    Convert.ToString(dr["CodeDRGWeight"]),
                                    Convert.ToString(dr["AverageLengthOfStayValue"]),
                                    string.Empty, string.Empty, string.Empty, string.Empty,
                                    fDate, tillDate
                                    );
                            }
                            break;
                        case "atc":
                            ValidateBillingFile(dt, "CodeNumbering", "CodeEffectiveDate", "CodeExpiryDate", out status, out dateFormat2);
                            if (status != (int)ExcelImportResultCodes.Success)
                                return null;

                            foreach (DataRow dr in dt.Rows)
                            {
                                DateTime? fDate = null;
                                if (!string.IsNullOrEmpty(Convert.ToString(dr["CodeEffectiveDate"])))
                                    fDate = dateFormat2 ? DateTime.Parse(Convert.ToString(dr["CodeEffectiveDate"]))
                                        : DateTime.FromOADate(Convert.ToDouble(dr["CodeEffectiveDate"]));

                                DateTime? tillDate = null;
                                if (!string.IsNullOrEmpty(Convert.ToString(dr["CodeExpiryDate"])))
                                    tillDate = dateFormat2 ? DateTime.Parse(Convert.ToString(dr["CodeExpiryDate"])) :
                                        DateTime.FromOADate(Convert.ToDouble(dr["CodeExpiryDate"]));

                                if (fDate > tillDate)
                                {
                                    status = (int)ExcelImportResultCodes.DatesInCorrect;
                                    break;
                                }

                                dt1.Rows.Add(0,
                                    Convert.ToString(dr["TableNumber"]),
                                    Convert.ToString(dr["CodeNumbering"]),
                                    Convert.ToString(dr["CodeDescription"]),
                                    Convert.ToString(dr["CodePrice"]),
                                    Convert.ToString(dr["DrugName"]),
                                    Convert.ToString(dr["Purpose"]),
                                    Convert.ToString(dr["DrugDescription"]),
                                    string.Empty, string.Empty, string.Empty, string.Empty,
                                    fDate, tillDate
                                    );
                            }
                            break;
                        case "pos":
                            ValidateBillingFile(dt, "PlaceOfServiceCode", "EffectiveStartDate", "EffectiveEndDate", out status, out dateFormat2);
                            if (status != (int)ExcelImportResultCodes.Success)
                                return null;

                            foreach (DataRow dr in dt.Rows)
                            {
                                DateTime? fDate = null;
                                if (!string.IsNullOrEmpty(Convert.ToString(dr["EffectiveStartDate"])))
                                    fDate = dateFormat2 ? DateTime.Parse(Convert.ToString(dr["EffectiveStartDate"]))
                                        : DateTime.FromOADate(Convert.ToDouble(dr["EffectiveStartDate"]));

                                DateTime? tillDate = null;
                                if (!string.IsNullOrEmpty(Convert.ToString(dr["EffectiveEndDate"])))
                                    tillDate = dateFormat2 ? DateTime.Parse(Convert.ToString(dr["EffectiveEndDate"])) :
                                        DateTime.FromOADate(Convert.ToDouble(dr["EffectiveEndDate"]));

                                if (fDate > tillDate)
                                {
                                    status = (int)ExcelImportResultCodes.DatesInCorrect;
                                    break;
                                }

                                dt1.Rows.Add(0,
                                    string.Empty,
                                    Convert.ToString(dr["PlaceOfServiceCode"]),
                                    Convert.ToString(dr["Description"]),
                                   string.Empty,
                                    string.Empty,
                                    Convert.ToString(dr["PlaceOfServiceName"]),
                                    string.Empty,
                                    string.Empty, string.Empty, string.Empty, string.Empty,
                                    fDate, tillDate
                                    );
                            }
                            break;
                        case "bm":
                            ValidateBillingFile(dt, "ModifierCode", "EffectiveStartDate", "EffectiveEndDate", out status, out dateFormat2);
                            if (status != (int)ExcelImportResultCodes.Success)
                                return null;

                            foreach (DataRow dr in dt.Rows)
                            {
                                DateTime? fDate = null;
                                if (!string.IsNullOrEmpty(Convert.ToString(dr["EffectiveStartDate"])))
                                    fDate = dateFormat2 ? DateTime.Parse(Convert.ToString(dr["EffectiveStartDate"]))
                                        : DateTime.FromOADate(Convert.ToDouble(dr["EffectiveStartDate"]));

                                DateTime? tillDate = null;
                                if (!string.IsNullOrEmpty(Convert.ToString(dr["EffectiveEndDate"])))
                                    tillDate = dateFormat2 ? DateTime.Parse(Convert.ToString(dr["EffectiveEndDate"])) :
                                        DateTime.FromOADate(Convert.ToDouble(dr["EffectiveEndDate"]));

                                if (fDate > tillDate)
                                {
                                    status = (int)ExcelImportResultCodes.DatesInCorrect;
                                    break;
                                }

                                dt1.Rows.Add(0,
                                    string.Empty,
                                    Convert.ToString(dr["ModifierCode"]),
                                    Convert.ToString(dr["Description"]),
                                   string.Empty,
                                    string.Empty,
                                    Convert.ToString(dr["ModifierName"]),
                                    Convert.ToString(dr["Type"]),
                                    string.Empty, string.Empty, string.Empty, string.Empty,
                                    fDate, tillDate
                                    );
                            }
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    status = (int)ExcelImportResultCodes.ExecutionError;
                    return dt1;
                }
            }
            else
                status = (int)ExcelImportResultCodes.FileEmpty;

            return dt1;
        }

        public static DataTable FillDataTableToExport(IEnumerable<ExportCodesData> list, string codeType)
        {
            DataTable dt = null;
            dt = new DataTable();

            if (list == null)
                list = new List<ExportCodesData>();

            switch (codeType.ToLower())
            {
                case "cpt":
                    dt.Columns.Add("TableNumber", typeof(string));
                    dt.Columns.Add("TableDescription", typeof(string));
                    dt.Columns.Add("CodeNumbering", typeof(string));
                    dt.Columns.Add("CodeDescription", typeof(string));
                    dt.Columns.Add("CodePrice", typeof(string));
                    dt.Columns.Add("CodeGroup", typeof(string));
                    dt.Columns.Add("CodeEffectiveDate", typeof(DateTime));
                    dt.Columns.Add("CodeExpiryDate", typeof(DateTime));

                    foreach (var i in list)
                    {
                        dt.Rows.Add(i.TableNumber,
                            i.OtherValue1,
                            i.Code,
                            i.CodeDescription,
                            i.Price,
                            i.CodeGroup,
                            i.EffectiveFrom,
                            i.EffectiveTill
                            );
                    }

                    break;
                case "hcpcs":
                    dt.Columns.Add("TableNumber", typeof(string));
                    dt.Columns.Add("TableDescription", typeof(string));
                    dt.Columns.Add("CodeNumbering", typeof(string));
                    dt.Columns.Add("CodeDescription", typeof(string));
                    dt.Columns.Add("CodePrice", typeof(string));
                    dt.Columns.Add("CodeEffectiveDate", typeof(DateTime));
                    dt.Columns.Add("CodeExpiryDate", typeof(DateTime));

                    foreach (var i in list)
                    {
                        dt.Rows.Add(i.TableNumber,
                            i.OtherValue1,
                            i.Code,
                            i.CodeDescription,
                            i.Price,
                            i.EffectiveFrom,
                            i.EffectiveTill
                            );
                    }
                    break;
                case "drug":
                    dt.Columns.Add("TableNumber", typeof(string));
                    dt.Columns.Add("DrugCode", typeof(string));
                    dt.Columns.Add("GenericName", typeof(string));
                    dt.Columns.Add("PackageName", typeof(string));
                    dt.Columns.Add("PricePharmacy", typeof(string));
                    dt.Columns.Add("PricePublic", typeof(string));
                    dt.Columns.Add("UnitPricePharmacy", typeof(string));
                    dt.Columns.Add("UnitPricePublic", typeof(string));
                    dt.Columns.Add("Dosage", typeof(string));
                    dt.Columns.Add("Strength", typeof(string));
                    dt.Columns.Add("LastChangeOn", typeof(DateTime));
                    dt.Columns.Add("DeletedDate", typeof(DateTime));

                    foreach (var i in list)
                    {
                        dt.Rows.Add(i.TableNumber,
                            i.Code,
                            i.CodeDescription,
                            i.CodeGroup,
                            i.OtherValue1,
                            i.OtherValue2,
                            i.OtherValue3,
                            i.OtherValue4,
                            i.OtherValue5,
                            i.OtherValue6,
                            i.EffectiveFrom,
                            i.EffectiveTill
                            );
                    }
                    break;
                case "diagnosis":
                    dt.Columns.Add("TableNumber", typeof(string));
                    dt.Columns.Add("TableDescription", typeof(string));
                    dt.Columns.Add("DiagnosisCode", typeof(string));
                    dt.Columns.Add("ShortDescription", typeof(string));
                    dt.Columns.Add("DiagnosisMediumDescription", typeof(string));
                    dt.Columns.Add("FullDescription", typeof(string));
                    dt.Columns.Add("DiagnosisEffectiveStartDate", typeof(DateTime));
                    dt.Columns.Add("DiagnosisEffectiveEndDate", typeof(DateTime));

                    foreach (var i in list)
                    {
                        dt.Rows.Add(i.TableNumber,
                            i.OtherValue2,
                            i.Code,
                            i.CodeGroup,
                            i.OtherValue1,
                            i.CodeDescription,
                            i.EffectiveFrom,
                            i.EffectiveTill
                            );
                    }
                    break;
                case "drg":
                    dt.Columns.Add("TableNumber", typeof(string));
                    dt.Columns.Add("CodeNumbering", typeof(string));
                    dt.Columns.Add("CodeDescription", typeof(string));
                    dt.Columns.Add("CodePrice", typeof(string));
                    dt.Columns.Add("CodeDRGWeight", typeof(string));
                    dt.Columns.Add("AverageLengthOfStayValue", typeof(string));
                    dt.Columns.Add("CodeEffectiveDate", typeof(DateTime));
                    dt.Columns.Add("CodeExpiryDate", typeof(DateTime));

                    foreach (var i in list)
                    {
                        dt.Rows.Add(i.TableNumber,
                            i.Code,
                            i.CodeDescription,
                            i.Price,
                            i.OtherValue1,
                            i.OtherValue2,
                            i.EffectiveFrom,
                            i.EffectiveTill
                            );
                    }
                    break;
                case "atc":
                    dt.Columns.Add("TableNumber", typeof(string));
                    dt.Columns.Add("CodeNumbering", typeof(string));
                    dt.Columns.Add("CodeDescription", typeof(string));
                    dt.Columns.Add("CodePrice", typeof(string));
                    dt.Columns.Add("DrugName", typeof(string));
                    dt.Columns.Add("Purpose", typeof(string));
                    dt.Columns.Add("DrugDescription", typeof(string));
                    dt.Columns.Add("CodeEffectiveDate", typeof(DateTime));
                    dt.Columns.Add("CodeExpiryDate", typeof(DateTime));

                    foreach (var i in list)
                    {
                        dt.Rows.Add(i.TableNumber,
                            i.Code,
                            i.CodeDescription,
                            i.Price,
                            i.CodeGroup,
                            i.OtherValue1,
                            i.OtherValue2,
                            i.EffectiveFrom,
                            i.EffectiveTill
                            );
                    }
                    break;
                case "bm":
                    dt.Columns.Add("ModifierCode", typeof(string));
                    dt.Columns.Add("ModifierName", typeof(string));
                    dt.Columns.Add("Description", typeof(string));
                    dt.Columns.Add("Type", typeof(string));
                    dt.Columns.Add("EffectiveStartDate", typeof(DateTime));
                    dt.Columns.Add("EffectiveEndDate", typeof(DateTime));

                    foreach (var i in list)
                    {
                        dt.Rows.Add(i.Code,
                            i.OtherValue1,
                            i.CodeDescription,
                            i.OtherValue2,
                            i.EffectiveFrom,
                            i.EffectiveTill
                            );
                    }
                    break;
                case "pos":
                    dt.Columns.Add("PlaceOfServiceCode", typeof(string));
                    dt.Columns.Add("PlaceOfServiceName", typeof(string));
                    dt.Columns.Add("Description", typeof(string));
                    dt.Columns.Add("EffectiveStartDate", typeof(DateTime));
                    dt.Columns.Add("EffectiveEndDate", typeof(DateTime));

                    foreach (var i in list)
                    {
                        dt.Rows.Add(i.Code,
                            i.OtherValue1,
                            i.CodeDescription,
                            i.EffectiveFrom,
                            i.EffectiveTill
                            );
                    }
                    break;
                default:
                    break;
            }
            return dt;
        }

        public static void ValidateBillingFile(DataTable dt, string codeColumn, string effectiveDateColumn, string expiryDateColumn, out int status, out bool dateFormat2)
        {
            dateFormat2 = false;
            status = (int)ExcelImportResultCodes.Success;

            var queryableRecords = (from rw in dt.AsEnumerable()
                                    where string.IsNullOrEmpty(Convert.ToString(rw[codeColumn]))
                                    select rw).Select(a => Convert.ToString(a["ModifierName"])).ToList();
            var invalidRecord = (from rw in dt.AsEnumerable()
                                 where string.IsNullOrEmpty(Convert.ToString(rw[codeColumn]))
                                 select rw).Any();

            if (!invalidRecord)
            {
                if (dt.Rows.Count > 1)
                    dt.Rows[0][effectiveDateColumn] = dt.Rows[1][effectiveDateColumn];

                if (dt.AsEnumerable().Any(a => Convert.ToString(a[effectiveDateColumn]).Contains("/")
                || Convert.ToString(a[expiryDateColumn]).Contains("/")))
                {
                    invalidRecord = (
                    from rw in dt.AsEnumerable()
                    where (!string.IsNullOrEmpty(Convert.ToString(rw[effectiveDateColumn])) && !string.IsNullOrEmpty(Convert.ToString(rw[expiryDateColumn]))
                    && DateTime.Parse(Convert.ToString(rw[effectiveDateColumn])) > DateTime.Parse(Convert.ToString(rw[expiryDateColumn]))
                    )
                    select rw).Any();

                    dateFormat2 = true;
                }
                else
                {
                    invalidRecord = (
                    from rw in dt.AsEnumerable()
                    where (!string.IsNullOrEmpty(Convert.ToString(rw[effectiveDateColumn])) && !string.IsNullOrEmpty(Convert.ToString(rw[expiryDateColumn]))
                    && DateTime.FromOADate(Convert.ToDouble(rw[effectiveDateColumn])) > DateTime.FromOADate(Convert.ToDouble(rw[expiryDateColumn]))
                    )
                    select rw).Any();
                }

                if (invalidRecord)
                    status = (int)ExcelImportResultCodes.DatesInCorrect;
            }
            else
                status = (int)ExcelImportResultCodes.CodeMissing;

        }
    }

    /// <summary>
    /// The billing system extensions.
    /// </summary>
    public static class BillingSystemExtensions
    {
        /*Converts DataTable To List*/
        #region Public Methods and Operators

        /// <summary>
        /// The to list.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="dataTable">The data table.</param>
        /// <returns>
        /// The <see cref="List" />.
        /// </returns>
        public static List<TSource> ToList<TSource>(this DataTable dataTable) where TSource : new()
        {
            var dataList = new List<TSource>();

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
            var objFieldNames = (from PropertyInfo aProp in typeof(TSource).GetProperties(flags)
                                 select
                                     new
                                     {
                                         aProp.Name,
                                         Type = Nullable.GetUnderlyingType(aProp.PropertyType) ?? aProp.PropertyType
                                     })
                .ToList();
            var dataTblFieldNames =
                (from DataColumn aHeader in dataTable.Columns
                 select new { Name = aHeader.ColumnName, Type = aHeader.DataType }).ToList();
            var commonFields = objFieldNames.Intersect(dataTblFieldNames).ToList();

            foreach (var dataRow in dataTable.AsEnumerable().ToList())
            {
                var aTSource = new TSource();
                foreach (var aField in commonFields)
                {
                    var propertyInfos = aTSource.GetType().GetProperty(aField.Name);
                    var value = (dataRow[aField.Name] == DBNull.Value) ? null : dataRow[aField.Name];
                    propertyInfos.SetValue(aTSource, value, null);
                }

                dataList.Add(aTSource);
            }

            return dataList;
        }

        public static string GetEnumDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
        #endregion
    }

    /// <summary>
    /// The log file.
    /// </summary>
    public class LogFile
    {
        #region Public Methods and Operators

        /// <summary>
        /// The log exception.
        /// </summary>
        /// <param name="Message">
        /// The message.
        /// </param>
        /// <param name="MethodName">
        /// The method name.
        /// </param>
        /// <param name="ErrorPageFilePath">
        /// The error page file path.
        /// </param>
        /// <param name="ERPVAlue">
        /// The erpv alue.
        /// </param>
        /// <param name="UserID">
        /// The user id.
        /// </param>
        public static void LogException(
            string Message,
            string MethodName,
            string ErrorPageFilePath,
            int ERPVAlue,
            long UserID)
        {
        }

        /// <summary>
        /// The write log.
        /// </summary>
        /// <param name="TimeDate">
        /// The time date.
        /// </param>
        /// <param name="Message">
        /// The message.
        /// </param>
        public static void WriteLog(DateTime TimeDate, string Message)
        {
            try
            {
                StreamWriter Temp_StreamWriter = null;

                // Temp_StreamWriter = System.IO.File.AppendText(System.AppDomain.CurrentDomain.BaseDirectory + "\\ErrorLog.txt");
                Temp_StreamWriter = File.AppendText("C:\\ErrorLog.txt");
                Temp_StreamWriter.WriteLine(
                    "[" + TimeDate.ToShortDateString() + " - " + TimeDate.Hour.ToString("00") + ":"
                    + TimeDate.Minute.ToString("00") + "] " + "Message: " + Message);
                Temp_StreamWriter.Close();
            }
            catch (Exception ex)
            {
                WriteLog(DateTime.Now, ex.Message);
            }
        }

        #endregion
    }


    public class DynamicContractResolver : DefaultContractResolver
    {
        private readonly List<string> _propertiesToSerialize;
        public DynamicContractResolver(string propertiesToSerialize = "")
        {
            var list = !string.IsNullOrEmpty(propertiesToSerialize) ? propertiesToSerialize.Split(',').ToList() : new List<string>();
            _propertiesToSerialize = list;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);
            properties = properties.Where(p => _propertiesToSerialize.Contains(p.PropertyName.ToLower())).ToList();
            return properties;
        }
    }
}
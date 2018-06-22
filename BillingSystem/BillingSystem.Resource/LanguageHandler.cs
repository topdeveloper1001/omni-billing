using System.Globalization;
using System.Resources;

namespace BillingSystem.Resource
{
    public class LanguageHandler
    {

        #region Private member variables
        private ResourceManager _resourceMgr;
        private static readonly LanguageHandler instance = new LanguageHandler();
        #endregion

        #region Constructors
        public LanguageHandler()
        {

        }
        #endregion

        #region Public methods
        public static LanguageHandler Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Get the correct languagetext by key, if no key is found then the return value will be "[key]".
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetKeyValue(string key)
        {
            if (_resourceMgr == null)
            {
                _resourceMgr = new ResourceManager(GetType().Namespace + ".Resources.BillingSystem", System.Reflection.Assembly.GetExecutingAssembly());
            }
            key = key.ToLower().Trim();
            var retValue = _resourceMgr.GetString(key.ToLower()) ?? string.Format("[{0}]", key);
            return retValue;
        }

        public string GetKeyValueByCulture(string key, System.Globalization.CultureInfo culture)
        {
            if (_resourceMgr == null)
            {
                _resourceMgr = new ResourceManager(GetType().Namespace + ".Resources.BillingSystem", System.Reflection.Assembly.GetExecutingAssembly());
            }
            var retValue = _resourceMgr.GetString(key.ToLower(), culture) ?? string.Format("[{0}]", key);
            return retValue;
        }

        public string GetFileText(string key, CultureInfo culture)
        {
            var result = string.Empty;
            key = key.ToLower().Trim();
            switch (key)
            {
                case "patientportalemailverification":
                    result = Resources.BillingSystem.PatientPortalEmailVerification_en_US;
                    break;
                case "patientforgotpasswordemail":
                    result = Resources.BillingSystem.PatientForgotPasswordTemplate_en_US;
                    break;
                case "newpasswordemail":
                    result = Resources.BillingSystem.NewPasswordEmailTemplate_en_US;
                    break;
                case "usertokentoaccess":
                    result = Resources.BillingSystem.UserTokenTemplate_en_US;
                    break;
                case "resetpasswordtemplate":
                    result = Resources.BillingSystem.ResetPasswordTemplate_en_US;
                    break;
                case "forgotpwdtemplate":
                    result = Resources.BillingSystem.ForgotPasswordTemplate_en_US;
                    break;
                case "fromschedulartopatientonnewbooking":
                    result = Resources.BillingSystem.fromschedulartopatientonnewbooking;
                    break;
                case "patientreminderbefore3days":
                    result = Resources.BillingSystem.patientreminderbefore3days;
                    break;
                case "patientreminderbefore1day":
                    result = Resources.BillingSystem.patientreminderbefore1day;
                    break;
                case "patientReminderbeforeonappointmentday":
                    result = Resources.BillingSystem.patientReminderbeforeonappointmentday;
                    break;
                case "everymorningnotificationtophysician":
                    result = Resources.BillingSystem.everymorningnotificationtophysician;
                    break;
                case "appointmenttypessubtemplate":
                    result = Resources.BillingSystem.AppointmentTypesSubTemplate;
                    break;
                case "physicianapporovelemail":
                    result = Resources.BillingSystem.PhysicianApporovelEmail;
                    break;
                case "physiciancancelappointment":
                    result = Resources.BillingSystem.AppointmentCancelByPhysician;
                    break;
                case "appointmentapprovedbyphysician":
                    result = Resources.BillingSystem.AppointmentApprovedByPhysician;
                    break;
                case "barcodeview":
                    result = Resources.BillingSystem.BarcodeView;
                    break;
            }
            return result;
        }

        #endregion
    }
}

using System.Globalization;
using BillingSystem.Resource;
namespace BillingSystem.Common
{
    public class ResourceKeyValues
    {
        public static string GetKeyValue(string key)
        {
            //If we call this function it will pick the browser culture and return the string
            return new LanguageHandler().GetKeyValue(key);

        }

        public static string GetFileText(string key)
        {
            //If we call this function it will pick the browser culture and return the string
            return new LanguageHandler().GetFileText(key, CultureInfo.CurrentCulture);
        }
    }
}
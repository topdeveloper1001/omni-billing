using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillingSystem.Common.Common;
using BillingSystem.Model;
using BillingSystem.Model.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class PDFTemplatesRepository : GenericRepository<OtherPatientForm>
    {

       private readonly DbContext _context;

       public PDFTemplatesRepository(BillingEntities context)
            : base(context)
        {
            AutoSave = true;
            _context = context;
        }



       public bool UpdateOutPatientAssessment(OtherPatientForm oOuthOtherPatientForm)
       {
           try
           {
               if (_context != null)
               {
                   var spName = string.Format("EXEC {0} @PatientId, @EncounterId,@GlobalCodeCategory,@GlobalCode,@Value,@ExternalValue1,@ExternalValue3 ", StoredProcedures.SPROC_UpdateOutPatientNurseAssessmentForm);
                   var sqlParameters = new SqlParameter[7];

                   sqlParameters[0] = new SqlParameter("PatientId", oOuthOtherPatientForm.PatientId);
                   sqlParameters[1] = new SqlParameter("EncounterId", oOuthOtherPatientForm.EncounterId);
                   sqlParameters[2] = new SqlParameter("GlobalCodeCategory", oOuthOtherPatientForm.CategoryValue);
                   sqlParameters[3] = new SqlParameter("GlobalCode", oOuthOtherPatientForm.CodeValue);
                   sqlParameters[4] = new SqlParameter("Value", oOuthOtherPatientForm.Value);
                   sqlParameters[5] = new SqlParameter("ExternalValue1", oOuthOtherPatientForm.ExternalValue1);
                   sqlParameters[6] = oOuthOtherPatientForm.ExternalValue3 == null
                       ? new SqlParameter("ExternalValue3", DBNull.Value)
                       : new SqlParameter("ExternalValue3", oOuthOtherPatientForm.ExternalValue3);
                   ExecuteCommand(spName, sqlParameters);
                   return true;
               }
           }
           catch (Exception ex)
           {
               throw ex;
           }
           return false;
       }




    }
}

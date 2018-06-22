using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillingSystem.Model;

namespace BillingSystem.Repository.GenericRepository
{
    public class PatientPhoneRepository : GenericRepository<PatientPhone>
    {
       public PatientPhoneRepository(BillingEntities context)
            : base(context)
       {
           AutoSave = true;
       }
    }
}

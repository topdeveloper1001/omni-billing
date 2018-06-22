using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingSystem.Model.CustomModel
{
    public class DiagnosisTabData
    {
        public List<DiagnosisCustomModel> CurrentDiagnosisList { get; set; }
        public List<DiagnosisCustomModel> PreviousDiagnosisList { get; set; }
        public List<FavoritesCustomModel> FavOrdersList { get; set; }
        public long ExecutionStatus { get; set; }
        public bool PrimaryExists { get; set; }
        public bool MajorCPTExists { get; set; }
        public bool MajorDRGExists { get; set; }
    }
}

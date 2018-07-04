
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Model.CustomModel
{
    [NotMapped]
    public class DashboardBudgetCustomModel
    {
        public int BudgetId { get; set; }
        public int? BudgetType { get; set; }
        public string BudgetDescription { get; set; }
        public decimal? DepartmentNumber { get; set; }
        public string FiscalYear { get; set; }
        public decimal? JanuaryBudget { get; set; }
        public decimal? FebruaryBudget { get; set; }
        public decimal? MarchBudget { get; set; }
        public decimal? AprilBudget { get; set; }
        public decimal? MayBudget { get; set; }
        public decimal? JuneBudget { get; set; }
        public decimal? JulyBudget { get; set; }
        public decimal? AugustBudget { get; set; }
        public decimal? SeptemberBudget { get; set; }
        public decimal? OctoberBudget { get; set; }
        public decimal? NovemberBudget { get; set; }
        public decimal? DecemberBudget { get; set; }
        public int CorporateId { get; set; }
        public int FacilityId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
        public string BudgetFor { get; set; }

        public string BudgetTypeString { get; set; }
        public string BudgetForStr { get; set; }
    }
    [NotMapped]
    public class DashboardBudgetReportCustomModel
    {
        public string BudgetFor { get; set; }
        public string BudgetDescription { get; set; }
        public string FiscalYear { get; set; }
        public int MonthNumber { get; set; }
        public string MonthDescription { get; set; }
        public decimal? Budgets { get; set; }
        public decimal? Actuals { get; set; }
        public decimal? Projection { get; set; }
    }
    [NotMapped]
    public class DashboardChargesCustomModel
    {
        public int BudgetType { get; set; }
        public string BudgetDescription { get; set; }
        public decimal Department { get; set; }
        public string FiscalYear { get; set; }
        public decimal? M1 { get; set; }
        public decimal? M2 { get; set; }
        public decimal? M3 { get; set; }
        public decimal? M4 { get; set; }
        public decimal? M5 { get; set; }
        public decimal? M6 { get; set; }
        public decimal? M7 { get; set; }
        public decimal? M8 { get; set; }
        public decimal? M9 { get; set; }
        public decimal? M10 { get; set; }
        public decimal? M11 { get; set; }
        public decimal? M12 { get; set; }
        public decimal? Budgets { get; set; }
        public decimal? Actuals { get; set; }
        public decimal? Projection { get; set; }
        public string BudgetFor { get; set; }
        public int Sorting { get; set; }
    }
}

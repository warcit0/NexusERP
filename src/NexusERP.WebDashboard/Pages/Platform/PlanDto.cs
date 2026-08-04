using System;

namespace NexusERP.WebDashboard.Pages.Platform
{
    public class PlanDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public decimal MonthlyPrice { get; set; }
        public decimal AnnualPrice { get; set; }
        public int MaxUsers { get; set; }
        public int MaxBranches { get; set; }
        public int MaxInvoicesPerMonth { get; set; }
        public bool IncludesAdvancedAnalytics { get; set; }
    }
}

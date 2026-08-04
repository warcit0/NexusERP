using System;

namespace NexusERP.WebDashboard.Pages.Platform
{
    public class LicenseDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public Guid PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string LicenseKey { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}

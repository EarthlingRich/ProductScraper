using System;
using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Api.Application
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}

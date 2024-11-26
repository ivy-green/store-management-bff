﻿using Hangfire.Dashboard;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Jobs.StartUp
{
    [ExcludeFromCodeCoverage]
    public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}

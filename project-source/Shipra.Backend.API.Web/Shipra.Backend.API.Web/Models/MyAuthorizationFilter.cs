using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire.Dashboard;

namespace Shipra.Backend.API.Application.Services;
public class MyAuthorizationFilter : IDashboardAuthorizationFilter
{
  public bool Authorize(DashboardContext context)
  {
    // Retrieve the current HTTP context
    var httpContext = context.GetHttpContext();

    // Example: Only allow access to the Hangfire Dashboard if the user is authenticated
    // You can also add role-based checks here if needed.
    return httpContext.User.Identity?.IsAuthenticated == true;

    // Example with roles:
    // return httpContext.User.IsInRole("Admin"); // Allow only users in Admin role
  }
}

using Microsoft.AspNetCore.Mvc;

namespace Shipra.Backend.API.Application.Common.Security;
public class AWSAuthorize : TypeFilterAttribute
{
  public AWSAuthorize() : base(typeof(AWSAuthorizationFilter))
  {
  }
}

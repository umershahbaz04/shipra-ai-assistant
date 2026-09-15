using MediatR;
using Shipra.Backend.API.Application.DTOs;

namespace Shipra.Backend.API.Application.Features.OrderFeatures.Query.CheckMobileNoDuplicate;
public class CheckMobileNoDuplicateQuery : IRequest<ServiceResultDTO>
{
  public string MobileNo { get; set; }

  public CheckMobileNoDuplicateQuery(string mobileNo)
  {
    MobileNo = mobileNo;
  }
}

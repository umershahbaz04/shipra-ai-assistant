using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Enum;

namespace Shipra.Backend.API.Application.Features.ClientFeatures.Query.GetWipeOutSectionsLookup;

public class GetWipeOutSectionsLookupQueryHandler : RequestHandlerBase<GetWipeOutSectionsLookupQuery, ServiceResultDTO>
{
  public GetWipeOutSectionsLookupQueryHandler(IServiceProvider serviceProvider, ILogger<GetWipeOutSectionsLookupQueryHandler> logger) 
    : base(serviceProvider, logger)
  {
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetWipeOutSectionsLookupQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      await Task.CompletedTask;
      var sections = Enum.GetValues(typeof(EnumWipeOutSection))
          .Cast<EnumWipeOutSection>()
          .Select(e => new
          {
              Id = (int)e,
              Name = e.ToString(),
              Description = GetEnumDescription(e)
          }).ToList();

      serviceResult = new ServiceResultDTO(sections);
      serviceResult.CreateSuccessResponse();
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }

  private static string GetEnumDescription(Enum value)
  {
    var fieldInfo = value.GetType().GetField(value.ToString());
    if (fieldInfo == null) return value.ToString();
    var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
    return attributes.Length > 0 ? attributes[0].Description : value.ToString();
  }
}

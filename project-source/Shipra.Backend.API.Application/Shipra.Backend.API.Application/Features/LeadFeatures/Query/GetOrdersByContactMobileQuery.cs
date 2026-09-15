using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.LeadFeatures.Query;

public class GetOrdersByContactMobileQuery : IRequest<ServiceResultDTO>
{
    public string MobileNumber { get; set; } = string.Empty;
}

public class GetOrdersByContactMobileQueryHandler : RequestHandlerBase<GetOrdersByContactMobileQuery, ServiceResultDTO>
{
    private readonly ILeadRepository _leadRepository;

    public GetOrdersByContactMobileQueryHandler(ILeadRepository leadRepository, IServiceProvider serviceProvider, ILogger<GetOrdersByContactMobileQueryHandler> logger) : base(serviceProvider, logger)
    {
        _leadRepository = leadRepository;
    }

    protected override async Task<ServiceResultDTO> HandleRequest(GetOrdersByContactMobileQuery request, CancellationToken cancellationToken)
    {
        ServiceResultDTO serviceResult = new ServiceResultDTO();
        string clientId = _currentUser.ClientId!.Value.ToString();
        var result = await _leadRepository.GetOrdersByContactMobile(clientId, request.MobileNumber);
        
        serviceResult = new ServiceResultDTO(new { 
            list = result
        });
        return serviceResult;
    }
}

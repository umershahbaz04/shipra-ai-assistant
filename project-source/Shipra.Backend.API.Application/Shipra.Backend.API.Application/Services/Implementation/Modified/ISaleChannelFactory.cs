using Shipra.Backend.API.Application.DTOs.OrderUseCase;
using Shipra.Backend.API.Application.DTOs.SaleChannelUseCase;

namespace Shipra.Backend.API.Application.Services.Implementation.Modified;

public interface ISaleChannelFactory
{
  ISaleChannelService GetSaleChannelService(int saleChannelLookupId);
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Core.CountryAggregate;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Application.Features.CountryFeatures.Query.GetProvinceByCountryId;
public class GetProvinceByCountryIdQuery : IRequest<ServiceResultDTO>
{
  public int CountryId { get; set; }
}
public class GetProvinceByCountryIdQueryHandler : RequestHandlerBase<GetProvinceByCountryIdQuery, ServiceResultDTO>
{
  private readonly ICountryRepository _countryRepository;

  public GetProvinceByCountryIdQueryHandler(ICountryRepository countryRepository, IServiceProvider serviceProvider, ILogger<GetProvinceByCountryIdQueryHandler> logger) : base(serviceProvider, logger)
  {
    _countryRepository = countryRepository;
  }

  protected override async Task<ServiceResultDTO> HandleRequest(GetProvinceByCountryIdQuery request, CancellationToken cancellationToken)
  {
    ServiceResultDTO serviceResult = new ServiceResultDTO();
    try
    {
      List<Province> list = await _countryRepository.GetProvinceByCountryId(request.CountryId);
      var obj = Province.AddDefault();
      list.Add(obj);

      var data = list.Select(x => new
      {
        x.ProvinceId,
        x.Name
      }).OrderBy(x => x.ProvinceId).ToList();

      serviceResult = new ServiceResultDTO(data);
      return serviceResult;
    }
    catch (Exception ex)
    {
      serviceResult.CreateErrorResponse(ex);
      throw;
    }
  }
}
public class GetProvinceByCountryIdQueryValidator : AbstractValidator<GetProvinceByCountryIdQuery>
{
}

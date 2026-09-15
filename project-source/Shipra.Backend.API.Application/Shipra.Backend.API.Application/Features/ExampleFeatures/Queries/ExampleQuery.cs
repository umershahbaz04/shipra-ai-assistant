using MediatR;
using Microsoft.Extensions.Logging;
using Shipra.Backend.API.Application.Common;
using Shipra.Backend.API.Application.DTOs.Example;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Application.Features.ExampleFeatures.Queries;

public class ExampleQuery : IRequest<ObjectResponse<ExampleResponseDTO>>
{
  public string? Id { get; set; }

  public class ExampleQueryHandler : Common.RequestHandlerBase<ExampleQuery, ObjectResponse<ExampleResponseDTO>>
  {

    public ExampleQueryHandler( IServiceProvider serviceProvider, ILogger<ExampleQueryHandler> logger) : base(serviceProvider, logger)
    {
    }

    protected override async Task<ObjectResponse<ExampleResponseDTO>> HandleRequest(ExampleQuery request, CancellationToken cancellationToken)
    {

      var response = _mapper.Map<ExampleResponseDTO>(new ExampleResponseDTO());

      return await Task.Run(() =>
      {
        return new ObjectResponse<ExampleResponseDTO>(response);
      });
    }
  }

}

//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Shipra.Backend.API.Application.Features.ExampleFeatures.Commands;
//using Shipra.Backend.API.Application.Features.ExampleFeatures.Queries;

//namespace Shipra.Backend.API.Web.Api;
//[Route("api/[controller]")]
//[ApiController]
//public class ExampleController : BaseApiController
//{
//  public ExampleController(IServiceProvider serviceProvider) : base(serviceProvider)
//  {

//  }
//  [HttpPost]
//  [ProducesResponseType(StatusCodes.Status201Created)]
//  [ProducesResponseType(StatusCodes.Status400BadRequest)]
//  public async Task<IActionResult> Post([FromBody] ExampleCommand request, CancellationToken cancellationToken)
//  {
//    var response = await Mediator.Send(request, cancellationToken);

//    return Ok(response);
//  }
//  [HttpGet]
//  [ProducesResponseType(StatusCodes.Status200OK)]
//  [ProducesResponseType(StatusCodes.Status400BadRequest)]
//  public async Task<IActionResult> Get([FromQuery] ExampleQuery request, CancellationToken cancellationToken)
//  {
//    var response = await Mediator.Send(request, cancellationToken);

//    return Ok(response);
//  }
//}

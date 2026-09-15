using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shipra.Backend.API.Application.DTOs;
using Shipra.Backend.API.Application.DTOs.Common.Response;

namespace Shipra.Backend.API.Application.Features.ProductFeatures.Commands.UploadProductFiles;
public class UploadProductFilesCommand : IRequest<ServiceResultDTOWithTypeModel<List<S3ResponseDTO>>>
{
  public List<IFormFile>? Files { get; set; }
}

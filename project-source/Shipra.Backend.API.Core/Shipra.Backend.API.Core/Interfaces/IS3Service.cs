using Microsoft.AspNetCore.Http;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Core.Interfaces;
public interface IS3Service
{
  Task<dynamic> CreateBucketAsync(string bucketName);
  Task<dynamic> UploadFileAsync(string image, string path);
  Task<S3ResponseModel> UploadFileAsync(IFormFile file, string path);
  Task<dynamic> UploadFileAsync(Stream stream, string filename, string path);
  Task<dynamic> UploadFileWithExpandoresultAsync(IFormFile formFile, string s3path);
}

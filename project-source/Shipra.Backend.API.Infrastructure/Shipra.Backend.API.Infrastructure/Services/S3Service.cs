using System.Dynamic;
using System.Net;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NPOI.HPSF;
using Shipra.Backend.API.Application.Common.Constants;
using Shipra.Backend.API.Core.Interfaces;
using Shipra.Backend.API.Core.Models;

namespace Shipra.Backend.API.Infrastructure.Services;
public class S3Service : IS3Service
{
  private readonly IAmazonS3 _client; 

  public string region = "";
  public string bucketName = "";

  string baseURL = ApplicationConstants.S3BucketUrl;
   
  public S3Service(IConfiguration configuration)
  { 
    var awsAccessKey = configuration["AWS:AccessKeyId"];
    var awsSecretKey = configuration["AWS:SecretAccessKey"];
    bucketName = configuration["AWS:BucketName"]!;
    region = configuration["AWS:Region"]!;
    baseURL = configuration["AWS:BaseURL"]!;

    var credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
    _client = new AmazonS3Client(credentials, RegionEndpoint.APSouth1);
  }

  public async Task<dynamic> CreateBucketAsync(string bucketName)
  {

    // Check if bucket exists, then Upload it
    try
    {
      if (await AmazonS3Util.DoesS3BucketExistV2Async(_client, bucketName) == false)
      {
        var putBucketRequest = new PutBucketRequest
        {
          BucketName = bucketName,
          UseClientRegion = true

        };
        var request = await _client.PutBucketAsync(putBucketRequest);

        return new { Message = request.ResponseMetadata.RequestId, Status = request.HttpStatusCode };
      }
      else
      {
        return new { Message = "Something went wrong", Status = HttpStatusCode.BadRequest };
      }
    }
    catch (AmazonS3Exception e)
    {
      return new
      {
        Message = e.Message,
        Status = e.StatusCode,
        Url = ""
      };
    }

    // Catch other errors
    catch (Exception e)
    {
      return new
      {
        Message = e.Message,
        Status = HttpStatusCode.InternalServerError,
        Url = ""
      };
    }
  }

  public Task<dynamic> UploadFileAsync(string image, string path)
  {
    throw new NotImplementedException();
  }

  public async Task<S3ResponseModel> UploadFileAsync(IFormFile file, string path)
  {
    try
    {
      string fileType = Path.GetExtension(file.FileName);
      // Check if bucket exists, then Upload it
      if (await AmazonS3Util.DoesS3BucketExistV2Async(_client, bucketName) == false)
      {
        await CreateBucketAsync(bucketName);
      }
      var fileTransferUtility = new TransferUtility(_client);

      var filename = Guid.NewGuid().ToString() + "_" + file.FileName.Replace(" ", "")!;

      string filePath = Path.Combine(path, filename);
      //filePath = filePath + "." + fileType; ;
      //path = path + filename;

      Stream stream = file.OpenReadStream();
      var fileTransferUtilityRequest = new TransferUtilityUploadRequest
      {
        BucketName = bucketName,
        InputStream = stream,
        StorageClass = S3StorageClass.Standard,
        PartSize = 6291456,
        Key = filePath,
        CannedACL = S3CannedACL.PublicRead
      }; 
      await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
      return new S3ResponseModel
      {
        Message = "Uploaded Successfully",
        Status = (int)HttpStatusCode.OK,
        Url = Path.Combine(baseURL + filePath)
      };
    }

    // Catch specific amazon errors
    catch (AmazonS3Exception e)
    {
      return new S3ResponseModel
      {
        Message = e.Message,
        Status = (int)e.StatusCode,
        Url = ""
      };
    }

    // Catch other errors
    catch (Exception e)
    {
      return new S3ResponseModel
      {
        Message = e.Message,
        Status = (int)HttpStatusCode.InternalServerError,
        Url = ""
      };
    }

  }

  public Task<dynamic> UploadFileAsync(Stream stream, string filename, string path)
  {
    throw new NotImplementedException();
  }

  public async Task<dynamic> UploadFileWithExpandoresultAsync(IFormFile file, string path)
  {
    try
    {
      dynamic expandoObj = new ExpandoObject();
      string fileType = Path.GetExtension(file.FileName);
      // Check if bucket exists, then Upload it
      if (await AmazonS3Util.DoesS3BucketExistV2Async(_client, bucketName) == false)
      {
        await CreateBucketAsync(bucketName);
      }
      var fileTransferUtility = new TransferUtility(_client);

      var filename = Guid.NewGuid().ToString() + "_" + file.FileName.Replace(" ", "")!;

      string filePath = Path.Combine(path, filename);
      //filePath = filePath + "." + fileType; ;
      //path = path + filename;

      Stream stream = file.OpenReadStream();
      var fileTransferUtilityRequest = new TransferUtilityUploadRequest
      {
        BucketName = bucketName,
        InputStream = stream,
        StorageClass = S3StorageClass.Standard,
        PartSize = 6291456,
        Key = filePath,
        CannedACL = S3CannedACL.PublicRead
      };

      await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);

      expandoObj.Message = "Uploaded Successfully";
      expandoObj.Status = HttpStatusCode.OK;
      expandoObj.Url = Path.Combine(baseURL + filePath);

      return expandoObj;


    }

    // Catch specific amazon errors
    catch (AmazonS3Exception e)
    {
      return new
      {
        Message = e.Message,
        Status = e.StatusCode,
        Url = ""
      };
    }

    // Catch other errors
    catch (Exception e)
    {
      return new
      {
        Message = e.Message,
        Status = HttpStatusCode.InternalServerError,
        Url = ""
      };
    }

  }
}

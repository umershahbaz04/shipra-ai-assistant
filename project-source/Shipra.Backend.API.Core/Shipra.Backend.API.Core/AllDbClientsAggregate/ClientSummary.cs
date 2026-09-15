using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipra.Backend.API.Core.AllDbClientsAggregate;
public class ClientSummary
{
  public Guid? ClientSummaryId { get; set; }   // Unique identifier (primary key) 
  public string? ServerName { get; set; }      // Server name
  public int DatabaseId { get; set; }         // Database ID
  public string? StatusName { get; set; }      // Status name
  public int? OperationalStatusId { get; set; } // Operational status ID
  public int? TotalCount { get; set; }         // Total count
  public Guid? ClientId { get; set; }           // Client ID
  public string? ClientName { get; set; }      // Client name
  public string? ClientCode { get; set; }      // Client code
  public string? ClientImage { get; set; }     // Client image
  public string? ClientCompanyName { get; set; } // Client's company name
  public string? Mobile { get; set; }          // Mobile number
  public string? Phone { get; set; }           // Phone number
  public string? Email { get; set; }           // Email address
  public string? LicenseNo { get; set; }       // License number
  public int? ClientIdentifier { get; set; } // Client identifier
  public string? TRNNo { get; set; }           // TRN number
  public DateTime? CreatedOn { get; set; }     // Date created
  public string? CountryName { get; set; }     // Country name
  public string? CityName { get; set; }        // City name
  public string? FullAddress { get; set; }     // Full address from ClientAddress
  public string? StreetAddress { get; set; }   // Street address from ClientAddress

  public static ClientSummary CreateClientSummary(string? serverName, int databaseId, string? statusName, int? operationalStatusId, int? totalCount, Guid? clientId, string? clientName, string? clientCode, string? clientImage, string? clientCompanyName, string? mobile, string? phone, string? email, string? licenseNo, int? clientIdentifier, string? tRNNo, DateTime? createdOn, string? countryName, string? cityName, string? fullAddress, string? streetAddress)
  {
    return new ClientSummary
    {
      ClientSummaryId = Guid.NewGuid(),
      ServerName = serverName,
      DatabaseId = databaseId,
      StatusName = statusName,
      OperationalStatusId = operationalStatusId,
      TotalCount = totalCount,
      ClientId = clientId,
      ClientName = clientName,
      ClientCode = clientCode,
      ClientImage = clientImage,
      ClientCompanyName = clientCompanyName,
      Mobile = mobile,
      Phone = phone,
      Email = email,
      LicenseNo = licenseNo,
      ClientIdentifier = clientIdentifier,
      TRNNo = tRNNo,
      CreatedOn = createdOn,
      CountryName = countryName,
      CityName = cityName,
      FullAddress = fullAddress,
      StreetAddress = streetAddress
    };

  }

  public void UpdateClientSummary(string? serverName, int databaseId, string? statusName, int? operationalStatusId, int? totalCount, Guid? clientId, string? clientName, string? clientCode, string? clientImage, string? clientCompanyName, string? mobile, string? phone, string? email, string? licenseNo, int? clientIdentifier, string? tRNNo, DateTime? createdOn, string? countryName, string? cityName, string? fullAddress, string? streetAddress)
  {
    // Update the properties of the ClientSummary object 
    ServerName = serverName;
    DatabaseId = databaseId;
    StatusName = statusName;
    OperationalStatusId = operationalStatusId;
    TotalCount = totalCount;
    ClientId = clientId;
    ClientName = clientName;
    ClientCode = clientCode;
    ClientImage = clientImage;
    ClientCompanyName = clientCompanyName;
    Mobile = mobile;
    Phone = phone;
    Email = email;
    LicenseNo = licenseNo;
    ClientIdentifier = clientIdentifier;
    TRNNo = tRNNo;
    CreatedOn = createdOn;
    CountryName = countryName;
    CityName = cityName;
    FullAddress = fullAddress;
    StreetAddress = streetAddress;
  }

  public void UpdateOperationStatus(int operationlStatusId, string statusName)
  {
    OperationalStatusId = operationlStatusId;
    StatusName = statusName;
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Result;
using Shipra.Backend.API.Application.DTOs.AccountUserCase;
using Shipra.Backend.API.Core.CommonAggregate;
using Shipra.Backend.API.Core.CountryAggregate;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shipra.Backend.API.Application.DTOs.ClientUseCase.Response;
public class AuthResponseModel<T>
{
  public T? result { get; set; }
  public Dictionary<string, string[]>? errors { get; set; }
  public bool isSuccess { get; set; }
}
public class ConfirmUserModel
{
  public string? message { get; set; }
  public string? ClientId { get; set; }
}
public class AuthResponseModelWithD
{
  public dynamic? result { get; set; }
  public Dictionary<string, string[]>? errors { get; set; }
  public bool isSuccess { get; set; }
}
public class SignupResult
{ 
  public string? user_name { get; set; }
  public string? userId { get; set; } 
  public string? userPoolId { get; set; }
  public Userpoolclient? userPoolClient { get; set; }
}

public class Userpoolclient
{
  public string? userPoolClientId { get; set; }
  public string? userPoolClientSecretId { get; set; }
}
public class LoginResult
{
  public string? access_token { get; set; }
  public string? token_type { get; set; }
  public int expires_in { get; set; }
  public string? user_name { get; set; }
  public string? client_code { get; set; }
  public string? refresh_token { get; set; }
  public string? id_token { get; set; }
  public string? client_id { get; set; }
  public string? email { get; set; }
  public string? companyImage { get; set; }
  public string? clientPrefix { get; set; }
  public bool? isSubscriptionCancel { get; set; }
  public bool? isOnTrail { get; set; }
  public string? customerSessionClientSecret { get; set; }
  public string? pricingTableId { get; set; }
  public string? publishableKey { get; set; }
  public bool? isPlannedSubscribed { get; set; }
  public int? userRoleId { get; set; } 
  public bool? allowPersonalCarrierContract { get; set; }
  public bool? allowShipperInvocie { get; set; }
  public RegionTimeZone? region { get; set; }
  public Country? country { get; set; }
  public List<string>? restrictedCountry { get; set; }
  public bool? isShowMetafield { get; set; }
  public LoginSettingConfigDto? settingConfig { get; set; }
} 

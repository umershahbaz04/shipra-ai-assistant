using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Shipra.Backend.API.Core.Interfaces;

namespace Shipra.Backend.API.Infrastructure.Data.Repository.Implementation;
public class UserManagementRepository : IUserManagement
{
  private readonly AppDbContext _context;

  public UserManagementRepository(AppDbContext context)
  {
    _context = context;
  }

  //public string baseUrl = "https://localhost:7075";
  #region cognito service 

  public async Task<string> SignupAsync(string? email, string? userName, string? phone, string? address, string? password, string? confirmPassword, string baseUrl)
  {
    var requestUri = "/api/Tenant/Signup";
    var json = JsonConvert.SerializeObject(new { Email = email, UserName = userName, Phone = phone, Address = address, Password = password, ConfirmPassword = confirmPassword });
    var requestResponse = await SendRequest(json, baseUrl, requestUri);
    return requestResponse;
  }
  public async Task<string> ConfirmSignUpRequestAsync(string userName, string code, string baseUrl)
  {
    var requestUri = "/api/Tenant/ConfirmUser";
    var json = JsonConvert.SerializeObject(new { UserName = userName, Code = code });
    var requestResponse = await SendRequest(json, baseUrl, requestUri);
    return requestResponse;
  }

  public async Task<string> ResendConfirmationCodeAsync(string userName, string baseUrl)
  {
    var requestUri = "/api/Tenant/ResendConfirmationCode";
    var json = JsonConvert.SerializeObject(new { UserName = userName });
    var requestResponse = await SendRequest(json, baseUrl, requestUri);
    return requestResponse;
  }
  public async Task<string> LoginAsync(string userName, string password, string baseUrl)
  {
    var requestUri = "/api/Tenant/Login";
    var json = JsonConvert.SerializeObject(new { UserName = userName, Password = password });
    var requestResponse = await SendRequest(json, baseUrl, requestUri);

    return requestResponse;
  }
  public async Task<string> LogoutAsync(string AccessToken, string baseUrl)
  {
    var requestUri = "/api/Tenant/Logout";
    var json = JsonConvert.SerializeObject(new { AccessToken = AccessToken });
    var requestResponse = await SendRequest(json, baseUrl, requestUri);
    return requestResponse;
  }
  public async Task<string> GetAccessTokenWithRefreshTokenAsync(string userName, string? refreshToken, string baseUrl)
  {
    var requestUri = "/api/Tenant/GetAccessTokenWithRefreshToken";
    var json = JsonConvert.SerializeObject(new { UserName = userName, RefreshToken = refreshToken });
    var requestResponse = await SendRequest(json, baseUrl, requestUri);
    return requestResponse;
  }
  public async Task<string> CheckUsernameAvailability(string userName, string baseUrl)
  {
    var requestUri = "/api/Tenant/CheckUsernameAvailability";
    var json = JsonConvert.SerializeObject(new { UserName = userName });
    var requestResponse = await SendRequest(json, baseUrl, requestUri);
    return requestResponse;
  }
  #endregion

  public async Task<string> SendRequest(string json, string url, string requestUri)
  {
    using (var httpClientHandler = new HttpClientHandler())
    {
      httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri(url);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await client.PostAsync(requestUri, content);

        string responsebody = await response.Content.ReadAsStringAsync();
        return responsebody;
      }
    }
  }


}

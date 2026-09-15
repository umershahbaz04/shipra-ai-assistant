namespace Shipra.Backend.API.Core.Interfaces;
public interface IUserManagement
{
  Task<string> SignupAsync(string? email, string? userName, string? phone, string? address, string? password, string? confirmPassword, string baseUrl);
  Task<string> ConfirmSignUpRequestAsync(string userName, string code, string baseUrl);
  Task<string> ResendConfirmationCodeAsync(string userName, string baseUrl);
  Task<string> LoginAsync(string userName, string password, string baseUrl);
  Task<string> LogoutAsync(string AccessToken, string baseUrl);
  Task<string> GetAccessTokenWithRefreshTokenAsync(string userName, string? refreshToken, string baseUrl);
  Task<string> CheckUsernameAvailability(string userName, string baseUrl);
  //Task<dynamic> CreateAwsCognitoCredential(AwsCognitoCredential awsCognitoCredential);
}

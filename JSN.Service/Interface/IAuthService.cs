using JSN.Core.Model;
using JSN.Core.ViewModel;
using JSN.Shared.Model;

namespace JSN.Service.Interface;

public interface IAuthService
{
    Task<User> RegisterAsync(UserView request);
    Task<TokenModel> LoginAsync(UserView request);
    Task<TokenModel> RefreshTokenAsync(TokenModel? tokenModel);
    string CheckLogin(UserView request);
    string CheckUserExists(UserView request);
    string CheckRefreshToken(TokenModel? tokenModel);
}

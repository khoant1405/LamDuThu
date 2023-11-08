using JSN.Core.Model;

namespace JSN.Service.Interface;

public interface IUserService
{
    User? GetUserByUserName(string? userName);
}

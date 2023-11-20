using JSN.Core.Entity;
using JSN.Core.Model;
using JSN.Service.Interface;

namespace JSN.Service.Implement;

public class UserService(IRepository<User> userRepository) : IUserService
{
    public User? GetUserByUserName(string? userName)
    {
        var user = userRepository.Where(x => x.UserName == userName).SingleOrDefault();
        return user;
    }
}
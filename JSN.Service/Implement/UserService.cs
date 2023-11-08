using JSN.Core.Entity;
using JSN.Core.Model;
using JSN.Service.Interface;

namespace JSN.Service.Implement;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;

    public UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public User? GetUserByUserName(string? userName)
    {
        var user = _userRepository.Where(x => x.UserName == userName).SingleOrDefault();
        return user;
    }
}

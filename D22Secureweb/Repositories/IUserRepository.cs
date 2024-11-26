using D22Secureweb.Model;

namespace D22Secureweb.Repositories
{
    public interface IUserRepository
    {
        Task<bool> Authenticate(User user);
        Task<User?> Authorize(User user);
    }
}

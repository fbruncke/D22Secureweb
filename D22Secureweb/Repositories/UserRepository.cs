using D22Secureweb.Model;

namespace D22Secureweb.Repositories
{
    public class UserRepository : IUserRepository
    {
        public Task<User?> Authorize(User user)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Authenticate(User user)
        {
            //Lookup the database
            var dbUser = new User { Id = 1, UserName="Frank", Password="Frank", Email="frank@frank.dk", Role="User" };

            if (dbUser.UserName.Equals(user.UserName)
                && dbUser.Password.Equals(user.Password))
                return true;
            return false;
        }
    }
}

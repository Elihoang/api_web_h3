using API_WebH3.Repositories;

namespace API_WebH3.Services
{
    public class AccountService
    {
        private readonly IUserRepository _userRepository;

        public AccountService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

    }
}

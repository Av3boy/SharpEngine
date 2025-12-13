namespace Portal.Backend.Services;

public interface IUserService
{

}

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;

    public UserService(ILogger<UserService> logger)
    {
        _logger = logger;
    }

    public async Task Login()
    {
        // TODO: Create user (if not exists) when auth0 login done
        // We need a copy of the user to link achievements
    }
}

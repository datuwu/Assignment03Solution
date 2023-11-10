// CustomSignInManager.cs
using BusinessObject;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

public class CustomSignInManager : SignInManager<User>
{
    private readonly IConfiguration _configuration;

    public CustomSignInManager(UserManager<User> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<User> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<User>> logger, IAuthenticationSchemeProvider authenticationSchemeProvider, IUserConfirmation<User> userConfirmation, IConfiguration configuration)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, authenticationSchemeProvider, userConfirmation)
    {
        _configuration = configuration;
    }

    public override Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
    {
        var adminUsername = _configuration["adminAccount:username"];
        var adminPassword = _configuration["adminAccount:password"];

        if (userName == adminUsername && password == adminPassword)
        {
            var user = UserManager.FindByNameAsync(adminUsername).Result;

            if (user == null)
            {
                // If the user doesn't exist, create a new one
                user = new User { UserName = adminUsername, Email = adminUsername }; // Replace 'User' with your actual user class
                var result = UserManager.CreateAsync(user, adminPassword).Result;

                if (!result.Succeeded)
                {
                    // Handle user creation failure
                    return Task.FromResult(SignInResult.Failed);
                }
            }

            SignInAsync(user, isPersistent, null).Wait();
            return Task.FromResult(SignInResult.Success);
        }

        return base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
    }

}
using Microsoft.AspNetCore.Authentication;

namespace TransportAnimals.Helpers {
    public class MySchemeAuth : IAuthenticationHandler {
        private HttpContext _context;

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context) {
            _context = context;
            return Task.CompletedTask;
        }

        public Task<AuthenticateResult> AuthenticateAsync()
            => Task.FromResult(AuthenticateResult.NoResult());

        public Task ChallengeAsync(AuthenticationProperties properties) {
            return Task.CompletedTask;
        }

        public Task ForbidAsync(AuthenticationProperties properties) {
            properties = properties ?? new AuthenticationProperties();
            _context.Response.StatusCode = 403;
            return Task.CompletedTask;
        }
    }
}

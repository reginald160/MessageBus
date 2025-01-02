using IzukaBus.Data;
using System.Threading.Tasks;

namespace IzukaBus.Service
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context=context;
        }

        public Task<string> GenerateAccessTokenSync(string clientId, string secret)
        {
            _context.Me
        }
    }
}

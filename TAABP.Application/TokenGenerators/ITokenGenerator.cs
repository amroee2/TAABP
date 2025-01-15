namespace TAABP.Application.TokenGenerators
{
    public interface ITokenGenerator
    {
        string GenerateToken(string id, IEnumerable<string>? roles = null);
        bool ValidateToken(string token);
    }
}

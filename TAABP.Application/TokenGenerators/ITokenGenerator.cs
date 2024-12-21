namespace TAABP.Application.TokenGenerators
{
    public interface ITokenGenerator
    {
        string GenerateToken(string email);
        bool ValidateToken(string token);
    }
}

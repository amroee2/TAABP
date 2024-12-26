namespace TAABP.Application.TokenGenerators
{
    public interface ITokenGenerator
    {
        string GenerateToken(string id);
        bool ValidateToken(string token);
    }
}

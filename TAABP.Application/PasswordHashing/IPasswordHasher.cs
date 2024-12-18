namespace TAABP.Application.PasswordHashing
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
    }
}

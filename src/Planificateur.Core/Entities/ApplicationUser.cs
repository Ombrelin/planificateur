namespace Planificateur.Core.Entities;

public class ApplicationUser
{
    public Guid Id { get; }

    private string username;

    public string Username
    {
        get => username;
        private set => username = string.IsNullOrEmpty(value)
            ? throw new ArgumentException("Poll name can't be empty")
            : value;
    }

    public ApplicationUser(Guid id, string username, string password)
    {
        Id = id;
        Username = username;
        Password = password;
    }

    
    public string Password { get; }
    public ApplicationUser(string creationUsername, string creationPassword)
    {
        Id = Guid.NewGuid();
        Username = creationUsername;
        Password = BCrypt.Net.BCrypt.HashPassword(ValidatePassword(creationPassword));
    }


    private static string ValidatePassword(string passwordToValidate)
    {
        if (string.IsNullOrEmpty(passwordToValidate))
        {
            throw new ArgumentException("Password can't be null or empty");
        }

        if (passwordToValidate.Length < 5)
        {
            throw new ArgumentException("Password should at least have a length of 5");
        }
        if (!passwordToValidate.Any(char.IsDigit))
        {
            throw new ArgumentException("Password must contain a digit");
        }

        if (!passwordToValidate.Any(char.IsAsciiLetterUpper))
        {
            throw new ArgumentException("Password must contain an uppercase letter");
        }
        if (!passwordToValidate.Any(char.IsAsciiLetterLower))
        {
            throw new ArgumentException("Password must contain a lowercase letter");
        }

        return passwordToValidate;
    }

    public bool VerifyPassword(string passwordToVerify) => BCrypt.Net.BCrypt.Verify(passwordToVerify, this.Password);
}
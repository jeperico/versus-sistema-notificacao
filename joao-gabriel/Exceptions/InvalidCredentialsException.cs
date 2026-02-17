namespace MinhaNotificacao.Exceptions;

public class InvalidCredentialsException : BusinessException
{
    public InvalidCredentialsException() : base("Email ou senha inválidos")
    {
    }

    public InvalidCredentialsException(string message) : base(message)
    {
    }
}

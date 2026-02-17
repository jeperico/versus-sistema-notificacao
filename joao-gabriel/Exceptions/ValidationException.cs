namespace MinhaNotificacao.Exceptions;

public class ValidationException : BusinessException
{
    public ValidationException(string message) : base(message)
    {
    }

    public ValidationException(string field, string message) 
        : base($"{field}: {message}")
    {
    }
}

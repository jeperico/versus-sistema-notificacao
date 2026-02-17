namespace MinhaNotificacao.Exceptions;

public class UserAlreadyExistsException : BusinessException
{
    public UserAlreadyExistsException(string identifier) 
        : base($"Já existe um usuário cadastrado com: {identifier}")
    {
    }

    public UserAlreadyExistsException(string field, string value) 
        : base($"Já existe um usuário com {field}: {value}")
    {
    }
}

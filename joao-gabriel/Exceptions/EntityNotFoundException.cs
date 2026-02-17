namespace MinhaNotificacao.Exceptions;

public class EntityNotFoundException : BusinessException
{
    public EntityNotFoundException(string entityName, int id) 
        : base($"{entityName} com ID {id} não encontrado")
    {
    }

    public EntityNotFoundException(string entityName, string identifier) 
        : base($"{entityName} com {identifier} não encontrado")
    {
    }

    public EntityNotFoundException(string message) : base(message)
    {
    }
}

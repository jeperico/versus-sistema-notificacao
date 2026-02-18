namespace MinhaNotificacao.Exceptions;

public class UserInactiveException : BusinessException
{
    public UserInactiveException() 
        : base("Conta desativada. Entre em contato com o administrador")
    {
    }

    public UserInactiveException(string message) : base(message)
    {
    }
}

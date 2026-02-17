using System.ComponentModel.DataAnnotations;

namespace MinhaNotificacao.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "O nome de usuário é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome de usuário deve ter no máximo 100 caracteres")]
    [Display(Name = "Nome de Usuário")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome completo é obrigatório")]
    [StringLength(200, ErrorMessage = "O nome completo deve ter no máximo 200 caracteres")]
    [Display(Name = "Nome Completo")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "A confirmação de senha é obrigatória")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Senha")]
    [Compare("Password", ErrorMessage = "As senhas não coincidem")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

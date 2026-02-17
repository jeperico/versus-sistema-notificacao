using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinhaNotificacao.Entities;
using MinhaNotificacao.Services.Interfaces;

namespace MinhaNotificacao.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserEntity>>> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserEntity>> GetById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound($"Usuário com ID {id} não encontrado");

        return Ok(user);
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<UserEntity>> GetByEmail(string email)
    {
        var user = await _userService.GetUserByEmailAsync(email);
        if (user == null)
            return NotFound($"Usuário com email {email} não encontrado");

        return Ok(user);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<UserEntity>>> GetActive()
    {
        var users = await _userService.GetActiveUsersAsync();
        return Ok(users);
    }

    [HttpGet("role/{role}")]
    public async Task<ActionResult<IEnumerable<UserEntity>>> GetByRole(string role)
    {
        var users = await _userService.GetUsersByRoleAsync(role);
        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<UserEntity>> Create([FromBody] UserEntity user)
    {
        try
        {
            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UserEntity user)
    {
        if (id != user.Id)
            return BadRequest("ID da URL não corresponde ao ID do usuário");

        try
        {
            await _userService.UpdateUserAsync(user);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetCount()
    {
        var count = await _userService.GetTotalUsersCountAsync();
        return Ok(count);
    }
}

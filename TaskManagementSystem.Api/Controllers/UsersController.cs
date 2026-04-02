using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Api.Data;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace TaskManagementSystem.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        var dtos = users.Select(u => new UserDto(u.Id, u.FirstName, u.LastName, u.Email, u.Phone));
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();
        return Ok(new UserDto(user.Id, user.FirstName, user.LastName, user.Email, user.Phone));
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userDto)
    {
        var user = new User
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Email = userDto.Email,
            Phone = userDto.Phone
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, 
            new UserDto(user.Id, user.FirstName, user.LastName, user.Email, user.Phone));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto userDto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.FirstName = userDto.FirstName;
        user.LastName = userDto.LastName;
        user.Email = userDto.Email;
        user.Phone = userDto.Phone;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

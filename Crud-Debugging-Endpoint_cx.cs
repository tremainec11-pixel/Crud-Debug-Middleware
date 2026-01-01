using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // Use Dictionary for O(1) lookups instead of List.FirstOrDefault
    private static readonly Dictionary<int, User> Users = new Dictionary<int, User>();
    private static int _nextId = 1;

    // GET: api/users?page=1&pageSize=10
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetAllUsers(int page = 1, int pageSize = 10)
    {
        try
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(new { Message = "Page and pageSize must be positive integers." });

            var pagedUsers = Users.Values
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserDto { Id = u.Id, Name = u.Name, Email = u.Email })
                .ToList();

            return Ok(pagedUsers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Unexpected error retrieving users.", Details = ex.Message });
        }
    }

    // GET: api/users/{id}
    [HttpGet("{id}")]
    public ActionResult<UserDto> GetUserById(int id)
    {
        try
        {
            if (!Users.ContainsKey(id))
                return NotFound(new { Message = $"User with ID {id} not found." });

            var user = Users[id];
            return Ok(new UserDto { Id = user.Id, Name = user.Name, Email = user.Email });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Unexpected error retrieving user.", Details = ex.Message });
        }
    }

    // POST: api/users
    [HttpPost]
    public ActionResult<UserDto> CreateUser([FromBody] User newUser)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            newUser.Id = _nextId++;
            Users[newUser.Id] = newUser;

            var dto = new UserDto { Id = newUser.Id, Name = newUser.Name, Email = newUser.Email };
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, dto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Unexpected error creating user.", Details = ex.Message });
        }
    }

    // PUT: api/users/{id}
    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!Users.ContainsKey(id))
                return NotFound(new { Message = $"User with ID {id} not found." });

            var user = Users[id];
            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.Password = updatedUser.Password;

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Unexpected error updating user.", Details = ex.Message });
        }
    }

    // DELETE: api/users/{id}
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        try
        {
            if (!Users.ContainsKey(id))
                return NotFound(new { Message = $"User with ID {id} not found." });

            Users.Remove(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Unexpected error deleting user.", Details = ex.Message });
        }
    }
}

// Safe DTO (no password exposed)
public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}



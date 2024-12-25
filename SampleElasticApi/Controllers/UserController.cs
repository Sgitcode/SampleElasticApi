using Microsoft.AspNetCore.Mvc;
using SampleElasticApi.Models;
using SampleElasticApi.Services;

namespace SampleElasticApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IElasticSearch _elasticSearch;

    public UserController(ILogger<UserController> logger, IElasticSearch elasticSearch)
    {
        _logger = logger;
        _elasticSearch = elasticSearch;
    }

    [HttpPost(Name = "create-index")]
    public async Task<IActionResult> Create(string name)
    {
        await _elasticSearch.CreateIndexNotExists(name);
        return Ok($"Index {name} Created or Existed");
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        var result = await _elasticSearch.AddOrUpdateUser(user);
        return Ok(result);
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateUser([FromBody] User user)
    {
        var result = await _elasticSearch.AddOrUpdateUser(user);
        return result ? Ok(result) : StatusCode(500);
    }

    [HttpGet("get/{key}")]
    public async Task<IActionResult> GetUser(string key)
    {
        var result = await _elasticSearch.GetUser(key);
        return result != null ? Ok(result) : NotFound("User Not Fund");
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _elasticSearch.GetUsers();
        return result != null ? Ok(result) : StatusCode(500);
    }

    [HttpDelete("delete/{key}")]
    public async Task<IActionResult> DeleteUser(string key)
    {
        var result = await _elasticSearch.RemoveUser(key);
        return result ? Ok(result) : StatusCode(500);
    }

    [HttpDelete("delete-all")]
    public async Task<IActionResult> DeleteAllUsers()
    {
        var result = await _elasticSearch.RemoveAllUsers();
        return result != null ? Ok(result) : StatusCode(500);
    }
}
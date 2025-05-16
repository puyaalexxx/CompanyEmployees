using CompanyEmployees.Models;
using CompanyEmployees.Service;
using LoggingService;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController : ControllerBase
{
    private readonly IPlayerGenerator _playerGenerator;
    private readonly ILoggerManager _logger;

    public GamesController(/*[FromKeyedServices("player")]*/IPlayerGenerator playerGenerator, ILoggerManager logger)
    {
        _playerGenerator = playerGenerator;
        _logger = logger;
    }
    
    [HttpGet]
    public Player Get()
    {
        _logger.LogDebug("This is a debug message");
        _logger.LogInformation("This is an information message");
        _logger.LogWarning("This is a warn message");
        _logger.LogError("This is an error message");
        
        var newPlayer = _playerGenerator.CreateNewPlayer();
        
        return newPlayer;
    }
}
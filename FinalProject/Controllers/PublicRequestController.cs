using System.Drawing;
using Dapper;
using finalproject.Controllers;
using finalproject.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Serilog;


[ApiController]
public class PublicRequestController : ControllerBase
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IConfiguration _configuration;

    private readonly ILogger<PublicRequestController> _logger;

    public PublicRequestController(IRepositoryWrapper repositoryWrapper,IConfiguration configuration, ILogger<PublicRequestController> logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _configuration = configuration;
            _logger = logger;
        }

    [HttpGet("heartbeat", Name = "heartbeat")]
    public ActionResult<string> HeartBeat()
    {
        var responseString = "Api is working well. Server DateTime " + DateTime.Now.ToString();
        return Ok(responseString);
    }
}
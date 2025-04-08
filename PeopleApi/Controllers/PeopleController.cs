using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PeopleApi.Data;
using PeopleApi.Hubs;
using PeopleApi.Models;

namespace PeopleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeopleController : ControllerBase
{
    private readonly IHubContext<PersonHub> _hubContext;
    private readonly DataContext _context;

    public PeopleController(IHubContext<PersonHub> hubContext, DataContext context)
    {
        _hubContext = hubContext;
        _context = context;
    }


    [HttpGet("check/{personId}")]
    public async Task<IActionResult> CheckPerson(string personId)
    {
        var person = await _context.People.FindAsync(personId);

        if (person == null)
        {
            return Ok(new { message = "Person not found, please subscribe for updates." });
        }

        return Ok(new { message = "Person found!", person });
    }


    [HttpGet]
    public async Task<IActionResult> GetAllPeople()
    {
        var people = await _context.People.ToListAsync();
        return Ok(people);
    }


    [HttpGet("search/{name}")]
    public async Task<IActionResult> SearchPersonByName(string name)
    {
        var person = await _context.People
            .FirstOrDefaultAsync(p => p.FirstName.ToLower() == name.ToLower());

        if (person == null)
        {
            return Ok(new { message = "Person not found, please subscribe for updates." });
        }

        return Ok(new { message = "Person found!", person });
    }


    [HttpPost("add")]
    public async Task<IActionResult> AddPerson([FromBody] Person person)
    {
        _context.People.Add(person);
        await _context.SaveChangesAsync();

        var connectionIds = PersonHub.GetConnectionsForPerson(person.FirstName);
        if (connectionIds.Any())
        {
            await _hubContext.Clients.Group(person.FirstName.ToLower()).SendAsync("PersonAvailable", person);
        }

        return Ok(new { message = "Person added and notification sent!" });
    }
}
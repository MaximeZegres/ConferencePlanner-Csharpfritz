using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackEnd.Data;
using ConferenceDTO;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Sessions
        [HttpGet]
        public async Task<ActionResult<List<SessionResponse>>> Get()
        {
            var sessions = await _context.Sessions.AsNoTracking()
                                             .Include(s => s.Track)
                                             .Include(s => s.SessionSpeakers)
                                                .ThenInclude(ss => ss.Speaker)
                                             .Include(s => s.SessionTags)
                                                .ThenInclude(st => st.Tag)
                                             .Select(m => m.MapSessionResponse())
                                             .ToListAsync();
            return sessions;
        }

        // GET: api/Sessions/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SessionResponse>> Get(int id)
        {
            var session = await _context.Sessions.AsNoTracking()
                                            .Include(s => s.Track)
                                            .Include(s => s.SessionSpeakers)
                                                .ThenInclude(ss => ss.Speaker)
                                            .Include(s => s.SessionTags)
                                                .ThenInclude(st => st.Tag)
                                            .SingleOrDefaultAsync(s => s.ID == id);

            if (session == null)
            {
                return NotFound();
            }

            return session.MapSessionResponse();
        }

        // PUT: api/Sessions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, ConferenceDTO.Session input)
        {
            var session = await _context.Sessions.FindAsync(id);

            if (session == null)
            {
                return NotFound();
            }

            session.ID = input.ID;
            session.Title = input.Title;
            session.Abstract = input.Abstract;
            session.StartTime = input.StartTime;
            session.EndTime = input.EndTime;
            session.TrackId = input.TrackId;
            session.ConferenceID = input.ConferenceID;

            await _context.SaveChangesAsync();

            return NoContent();
        }



        // POST: api/Sessions
        [HttpPost]
        public async Task<ActionResult<SessionResponse>> Post(ConferenceDTO.Session input)
        {
            var session = new Data.Session
            {
                Title = input.Title,
                ConferenceID = input.ConferenceID,
                StartTime = input.StartTime,
                EndTime = input.EndTime,
                Abstract = input.Abstract,
                TrackId = input.TrackId
            };

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            var result = session.MapSessionResponse();

            return CreatedAtAction(nameof(Get), new { id = result.ID }, result);
        }

        // DELETE: api/Sessions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SessionResponse>> Delete(int id)
        {
            var session = await _context.Sessions.FindAsync(id);

            if (session == null)
            {
                return NotFound();
            }

            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();

            return session.MapSessionResponse();
        }

        private bool SessionExists(int id)
        {
            return _context.Sessions.Any(e => e.ID == id);
        }
    }
}

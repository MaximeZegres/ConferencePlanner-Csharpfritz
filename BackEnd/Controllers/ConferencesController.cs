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
    public class ConferencesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ConferencesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Conferences
        [HttpGet]
        public async Task<ActionResult<List<ConferenceResponse>>> GetConferences()
        {
            var conferences = await _context.Conferences.AsNoTracking().Select(s => new ConferenceResponse
            {
                ID = s.ID,
                Name = s.Name
            })
            .ToListAsync();

            return conferences;
        }


        // GET: api/Conferences/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ConferenceResponse>> GetConference(int id)
        {
            var conference = await _context.FindAsync<Data.Conference>(id);

            if (conference == null)
            {
                return NotFound();
            }

            var result = new ConferenceResponse
            {
                ID = conference.ID,
                Name = conference.Name
            };

            return result;
        }


        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadConference([Required, FromForm]string conferenceName, [FromForm]ConferenceFormat format, IFormFile file)
        {
            var loader = GetLoader(format);

            using (var stream = file.OpenReadStream())
            {
                await loader.LoadDataAsync(conferenceName, stream, _db);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        // PUT: api/Conferences/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutConference(int id, ConferenceDTO.Conference input)
        {
            var conference = await _context.FindAsync<Data.Conference>(id);

            if (conference == null)
            {
                return NotFound();
            }

            conference.Name = input.Name;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Conferences
        [HttpPost]
        public async Task<ActionResult<ConferenceResponse>> CreateConference(ConferenceDTO.Conference input)
        {
            var conference = new Data.Conference
            {
                Name = input.Name
            };

            _context.Conferences.Add(conference);
            await _context.SaveChangesAsync();

            var result = new ConferenceDTO.ConferenceResponse
            {
                ID = conference.ID,
                Name = conference.Name
            };

            return CreatedAtAction(nameof(GetConference), new { id = conference.ID }, result);
        }

        // DELETE: api/Conferences/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ConferenceResponse>> DeleteConference(int id)
        {
            var conference = await _context.FindAsync<Data.Conference>(id);

            if (conference == null)
            {
                return NotFound();
            }

            _context.Remove(conference);

            await _context.SaveChangesAsync();

            var result = new ConferenceDTO.ConferenceResponse
            {
                ID = conference.ID,
                Name = conference.Name
            };
            return result;
        }

        private static DataLoader GetLoader(ConferenceFormat format)
        {
            if (format == ConferenceFormat.Sessionize)
            {
                return new SessionizeLoader();
            }
            return new DevIntersectionLoader();
        }

        public enum ConferenceFormat
        {
            Sessionize,
            DevIntersections
        }

        private bool ConferenceExists(int id)
        {
            return _context.Conferences.Any(e => e.ID == id);
        }
    }
}

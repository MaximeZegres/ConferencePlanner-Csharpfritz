using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackEnd.Data
{
    public static class EntityExtensions
    {
        // Act like a viewModel and map between the model and what will be represented to the front.
        public static ConferenceDTO.SpeakerResponse MapSpeakerResponse(this Speaker speaker) =>
            new ConferenceDTO.SpeakerResponse
            {
                ID = speaker.ID,
                Name = speaker.Name,
                Bio = speaker.Bio,
                WebSite = speaker.WebSite,
                Sessions = speaker.SessionSpeakers?
                    .Select(ss =>
                        new ConferenceDTO.Session
                        {
                            ID = ss.SessionId,
                            Title = ss.Session.Title
                        })
                    .ToList()
            };
    }
}

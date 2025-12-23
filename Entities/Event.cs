using System;

namespace CampusEventManager.Entities
{
    public class Event
    {
        public int EventId { get; set; }
        public int ClubId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string PosterUrl { get; set; } = "default_event.jpg"; 
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = "";
        public int Quota { get; set; }
        public bool IsPublished { get; set; } 
        public string ClubName { get; set; }
        public double AverageRating { get; set; }
    }
}
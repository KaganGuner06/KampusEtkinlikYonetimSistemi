using System;

namespace CampusEventManager.Entities
{
    public class Announcement
    {
        public int AnnouncementId { get; set; }
        public int ClubId { get; set; }
        public string ClubName { get; set; } = ""; // Ekranda göstermek için
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
namespace CampusEventManager.Entities
{
    public class Club
    {
        public int ClubId { get; set; }
        public string ClubName { get; set; }
        public string Description { get; set; } // Bunu "Kısa Açıklama" olarak kullanacağız
        public string FullDescription { get; set; } // YENİ: Detaylı Açıklama
        public int CategoryId { get; set; }
        public int ManagerUserId { get; set; }
        public string LogoUrl { get; set; }
        public string CoverUrl { get; set; } // YENİ
        public string InstagramLink { get; set; } // YENİ
        public string LinkedinLink { get; set; } // YENİ
        public bool RequiresApproval { get; set; } // YENİ
        public bool IsActive { get; set; }
    }
}
namespace CampusEventManager.Entities
{
    public class Club
    {
        public int ClubId { get; set; }
        public string ClubName { get; set; }
        public string Description { get; set; } 
        public string FullDescription { get; set; } 
        public int CategoryId { get; set; }
        public int ManagerUserId { get; set; }
        public string LogoUrl { get; set; }
        public string CoverUrl { get; set; } 
        public string InstagramLink { get; set; } 
        public string LinkedinLink { get; set; } 
        public bool RequiresApproval { get; set; } 
        public bool IsActive { get; set; }
    }
}
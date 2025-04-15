using PixelPerspective.Areas.Identity.Data;

namespace PixelPerspective.Models
{
    public class GameLibrary
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public PixelPerspectiveUser User { get; set; }
        public long IGDBGameId {  get; set; }
        public string GameTitle { get; set; }
        public string CoverUrl { get; set; }
        public DateTime Added { get; set; } = DateTime.UtcNow;

    }
}

using System.ComponentModel.DataAnnotations;

namespace PixelPerspective.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public string? Genre { get; set; }
        public string? Description { get; set; }

        public string? ThumbnailImagePath { get; set; }

        public string CoverUrl => !string.IsNullOrEmpty(ThumbnailImagePath)
            ? $"https://images.igdb.com/igdb/image/upload/t_cover_big/{ThumbnailImagePath}.jpg"
            : "/images/default-cover.jpg";
    }
}

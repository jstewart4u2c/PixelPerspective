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
    }
}

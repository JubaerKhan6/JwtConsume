namespace JwtConsume.Models
{
    public class VideoGameEdit
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;

        public int size { get; set; }

        public string Developer { get; set; } = string.Empty;

        public int Released { get; set; }
        public float Rating { get; set; }
        public string Description { get; set; }

        public IFormFile ImageFile { get; set; }
        public string Image { get; set; }
        public string Category { get; set; } = "Game      ";

    }
}

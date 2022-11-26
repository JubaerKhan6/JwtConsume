namespace JwtConsume.Models
{
    public class ApiMovie
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Released { get; set; }
        public string Director { get; set; }


        public IFormFile Image { get; set; }
        public string Description { get; set; }

        public float Rating { get; set; }
        public string Category { get; set; } = "Movie";
    }
}

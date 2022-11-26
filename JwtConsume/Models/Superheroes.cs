namespace JwtConsume.Models
{
    public class Superheroes
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;

        public string Powers { get; set; } = String.Empty;

        public string Image { get; set; } = String.Empty;

        public string Description { get; set; } = String.Empty;
        public string Category { get; set; } = "Supe";


    }
}

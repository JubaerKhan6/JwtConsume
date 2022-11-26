namespace JwtConsume.Models
{
    public class LayoutVm
    {
        public List<Movieget> Movies { get; set; }
        public List<VideoGames> VideoGames { get; set; }

        public List<Cart> Carts { get; set; }

        public List<ApiMovie> ApiMovies { get; set; }
        public List<Index2Vm> Index2 { get; set; }
        public List<MovieEdit> MovieEdits { get; set; }
    }
}

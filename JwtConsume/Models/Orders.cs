namespace JwtConsume.Models
{
    public class Orders
    {
        public int OrderId { get; set; }
        public int Orderedby { get; set; }
        public int TotalPrice { get; set; }
        public int TotalItems { get; set; }
        
        public DateTime Time { get; set; }

      
    }
}

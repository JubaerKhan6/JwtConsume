using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JwtConsume.Models
{
    public class User
    {
        
        public int Id { get; set; }
        
        public string Password { get; set; }
    }
}

﻿namespace JwtConsume.Models
{
    public class Cart
    {
        public int SL { get; set; }
        public int productId { get; set; }
        public string productName { get; set; }
        public string productType { get; set; }
        public int productPrice { get; set; }

        public string productImage { get; set; }
        public int UserId { get; set; }


    }
}

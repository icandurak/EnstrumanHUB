namespace EnstrümanHub.Models
{
    public class BassGitar
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int Price { get; set; } // Fiyat kuruş cinsinden (örn: 3700 TL = 370000)
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Stock { get; set; }
    }
}

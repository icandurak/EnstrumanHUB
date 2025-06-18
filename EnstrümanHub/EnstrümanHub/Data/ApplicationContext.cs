using EnstrümanHub.Models;
namespace EnstrümanHub.Data
{
    public static class ApplicationContext
    {
        public static List<Gitar> Gitarlar { get; set; } = new List<Gitar>()
        {
            new Gitar
            {
                Id = 1,
                Name = "Fender Stratocaster",
                Brand = "Fender",
                Price = 1200,
                Description = "Klasik elektro gitar",
                ImageUrl = "https://example.com/stratocaster.jpg",
                Stock = 5
            },
            new Gitar
            {
                Id = 2,
                Name = "Gibson Les Paul",
                Brand = "Gibson",
                Price = 2500,
                Description = "Premium elektro gitar",
                ImageUrl = "https://example.com/lespaul.jpg",
                Stock = 3
            }
        };

        public static List<BassGitar> BassGitarlar { get; set; } = new List<BassGitar>()
        {
            new BassGitar
            {
                Id = 1,
                Name = "Fender Precision Bass",
                Brand = "Fender",
                Price = 1500,
                Description = "Klasik bas gitar",
                ImageUrl = "https://example.com/precision.jpg",
                Stock = 4
            }
        };

        public static List<Bateri> Bateriler { get; set; } = new List<Bateri>()
        {
            new Bateri
            {
                Id = 1,
                Name = "Pearl Export",
                Brand = "Pearl",
                Price = 2000,
                Description = "Profesyonel davul seti",
                ImageUrl = "https://example.com/pearl.jpg",
                Stock = 2
            }
        };
        

    }
}

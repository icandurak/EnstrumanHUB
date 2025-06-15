using EnstrümanHub.Models;
namespace EnstrümanHub.Data
{
    public static class ApplicationContext
    {
        public static List<Gitar> Gitarlar { get; set; }
        public static List<BassGitar> BassGitarlar { get; set; }
        public static List<Bateri> Bateriler { get; set; }

        static ApplicationContext()
        {
            Gitarlar = new List<Gitar>()
            {
                new Gitar() {Id=1,
                             Title="Fender Stratocaster",
                             Price=1000,
                             perde_sayisi=21,
                             gövde_tipi="Ihlamur Ağacı",
                             klavye_tipi="Gülağacı",
                             kasa_tipi="Stratocaster",
                             marka="Fender",
                             uretim_yeri="Mexico",
                             manyetik_tipi="SSS"

                             },

                new Gitar() {Id=2,
                             Title="Gibson Les Paul",
                             Price=2000,
                             perde_sayisi=22,
                             gövde_tipi="Maun Ağacı",
                             klavye_tipi="Gülağacı",
                             kasa_tipi="Les Paul",
                             marka="Gibson",
                             uretim_yeri="USA",
                             manyetik_tipi="HH"

                             }

            };

            BassGitarlar = new List<BassGitar>()
            {
                new BassGitar() { Id = 1,
                    Title = "Fender Bass",
                    Price = 1000,
                    marka = "Fender"


                }
            };

            Bateriler = new List<Bateri>()
            {
                new Bateri() {Id=1,
                             Title="Donner Bateri Seti",
                             Price=2000,
                             marka="Donner"


                             }
            };

                
                             


            /*
                        BassGitarlar = new List<BassGitar>()
                        {
                            new BassGitar() {Id=3,
                                             Title="Fender Bass",
                                             Price=500,
                                             perde_sayisi=20,
                                             marka="Fender",





                        }
                    }
            */
        }

    }
}

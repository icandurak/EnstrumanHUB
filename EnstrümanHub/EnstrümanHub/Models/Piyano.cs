namespace EnstrümanHub.Models
{
    public class Piyano : Enstrumanlar
    {
        public string marka { get; set; }
        public int pedal_sayisi {  get; set; }
        public int tus_sayisi { get; set; }
        public string digital_mi_analog_mu { get; set; }
    }
}

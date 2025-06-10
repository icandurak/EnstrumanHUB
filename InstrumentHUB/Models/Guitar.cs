namespace InstrumentHUB.Models
{
    public class Guitar : Instrument
    {
        public string Type { get; set; }
        public string Brand { get; set; }
        public int NumberOfStrings { get; set; }
        public int SizeOfFretboard { get; set; } 
        public string Color { get; set; }
        public string MadeIn { get; set; }

    }
}

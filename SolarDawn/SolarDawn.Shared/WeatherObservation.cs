namespace SolarDawn.Shared
{

    public record WeatherObservation
    {
        public DateTime Timestamp { get; set; }
        public double Temperature { get; set; }
        public double RainAccumulated { get; set; }
        public double RainAccumulationDay { get; set; }
    }
}

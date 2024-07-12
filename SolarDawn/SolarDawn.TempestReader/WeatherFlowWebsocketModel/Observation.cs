using System.Text;

namespace SolarDawn.TempestReader.WeatherFlowWebsocketModel
{
    /// <summary>
    /// Message type "obs_st" observation.
    /// </summary>
    /// <remarks>
    /// [1597160776,1.03,1.48,1.92,27,3,993.6,26.3,84,50991,3.63,425,0.0,0,35,1,2.61,1,2.307425,null,null,0]
    /// </remarks>
    public class Observation
    {
        /// <summary>
        /// Initializes a new instance of the Observation class.
        /// </summary>
        public Observation(IReadOnlyList<double?> observation)
        {

            Epoch = (int)(observation[0] ?? 0);
            WindLull = observation[1] ?? 0;
            WindAvg = observation[2] ?? 0;
            WindGust = observation[3] ?? 0;
            WindDirection = (int)(observation[4] ?? 0);
            WindSampleInterval = (int)(observation[5] ?? 0);
            Pressure = observation[6] ?? 0;
            AirTemperature = observation[7] ?? 0;
            RelativeHumidity = (int)(observation[8] ?? 0);
            Illuminance = (int)(observation[9] ?? 0);
            VU = observation[10] ?? 0;
            SolarRadiation = (int)(observation[11] ?? 0);
            RainAccumulation = observation[12] ?? 0;
            PrecipitationType = (Precipitation)(observation[13] ?? -1);
            AverageStrikeDistance = (int)(observation[14] ?? 0);
            StrikeCount = (int)(observation[15] ?? 0);
            Battery = observation[16] ?? 0;
            ReportInterval = (int)(observation[17] ?? 1);
            LocalDayRainAccumulation = observation[18] ?? 0;
            RainAccumulationFinal = observation[19] ?? 0;
            LocalDayRainAccumulationFinal = observation[20] ?? 0;

            PrecipitationAnalysisType = (PrecipitationAnalysis)(observation[21] ?? -1);
        }

        /// <summary>
        /// The Epoch (Seconds UTC).
        /// </summary>
        private int Epoch { get; set; }

        /// <summary>
        /// The time at which the observation occured at.
        /// </summary>
        public DateTime OccuredAt => DateTimeOffset.FromUnixTimeSeconds(Epoch).DateTime;

        /// <summary>
        /// The wind lull (m/s).
        /// </summary>
        public double WindLull { get; set; }

        /// <summary>
        /// The wind avg (m/s).
        /// </summary>
        public double WindAvg { get; set; }

        /// <summary>
        /// The Wind Gust (m/s)
        /// </summary>
        public double WindGust { get; set; }

        /// <summary>
        /// The wind direction (degrees).
        /// </summary>
        public int WindDirection { get; set; }

        /// <summary>
        /// Wind sample interval (seconds).
        /// </summary>
        public int WindSampleInterval { get; set; }

        /// <summary>
        /// The pressure (MB).
        /// </summary>
        public double Pressure { get; set; }

        /// <summary>
        /// The air temperature (C).
        /// </summary>
        public double AirTemperature { get; set; }

        /// <summary>
        /// The relative humidity (%).
        /// </summary>
        public int RelativeHumidity { get; set; }

        /// <summary>
        /// The illuminance (lux).
        /// </summary>
        public int Illuminance { get; set; }

        /// <summary>
        /// The UV (index).
        /// </summary>
        public double VU { get; set; }

        /// <summary>
        /// The solar radiation (W/m^2).
        /// </summary>
        public int SolarRadiation { get; set; }

        /// <summary>
        /// The rain accumulation (mm).
        /// </summary>
        public double RainAccumulation { get; set; }

        /// <summary>
        /// The precipitation type(0 = none, 1 = rain, 2 = hail).
        /// </summary>
        public Precipitation PrecipitationType { get; set; }

        /// <summary>
        /// The average strike distance (km).
        /// </summary>
        public int AverageStrikeDistance { get; set; }

        /// <summary>
        /// The strike count.
        /// </summary>
        public int StrikeCount { get; set; }

        /// <summary>
        /// The battery (volts).
        /// </summary>
        public double Battery { get; set; }

        /// <summary>
        /// The report interval(minutes).
        /// </summary>
        public int ReportInterval { get; set; }

        /// <summary>
        /// The local day rain accumulation (mm).
        /// </summary>
        public double LocalDayRainAccumulation { get; set; }

        /// <summary>
        /// The rain accumulation final (Rain Check) (mm).
        /// </summary>
        public double RainAccumulationFinal { get; set; }

        /// <summary>
        /// The local day rain accumulation final (Rain Check) (mm).
        /// </summary>
        public double LocalDayRainAccumulationFinal { get; set; }

        /// <summary>
        /// The Precipitation Analysis Type(0 = none, 1 = Rain Check with user display on, 2 = Rain Check with user display off).
        /// </summary>
        public PrecipitationAnalysis PrecipitationAnalysisType { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var property in GetType().GetProperties().OrderBy(x => x.Name))
            {
                sb.AppendLine($" {property.Name}: {property.GetValue(this)}");
            }
            return sb.ToString();
        }
    }
}
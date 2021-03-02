using SimpleTrading.SettingsReader;

namespace Service.BalanceHistory.Writer.Settings
{
    [YamlAttributesOnly]
    public class SettingsModel
    {
        [YamlProperty("BalanceHistory.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("BalanceHistory.SpotServiceBusHostPort")]
        public string SpotServiceBusHostPort { get; set; }

        [YamlProperty("BalanceHistory.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }
    }
}
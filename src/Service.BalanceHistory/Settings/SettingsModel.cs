using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.BalanceHistory.Settings
{
    public class SettingsModel
    {
        [YamlProperty("BalanceHistory.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("BalanceHistory.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }

        [YamlProperty("BalanceHistory.ZipkinUrl")]
        public string? ZipkinUrl { get; set; }

        [YamlProperty("LiquidityEngine.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }
    }
}
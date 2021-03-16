﻿using SimpleTrading.SettingsReader;

namespace Service.BalanceHistory.Settings
{
    [YamlAttributesOnly]
    public class SettingsModel
    {
        [YamlProperty("BalanceHistory.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("BalanceHistory.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }
    }
}
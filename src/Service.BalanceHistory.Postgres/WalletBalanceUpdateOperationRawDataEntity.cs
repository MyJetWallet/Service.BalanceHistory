﻿namespace Service.BalanceHistory.Postgres
{
    public class WalletBalanceUpdateOperationRawDataEntity
    {
        public WalletBalanceUpdateOperationRawDataEntity()
        {
        }

        public WalletBalanceUpdateOperationRawDataEntity(string operationId, string rawData)
        {
            OperationId = operationId;
            RawData = rawData;
        }

        public string OperationId { get; set; }

        public string RawData { get; set; }
    }
}
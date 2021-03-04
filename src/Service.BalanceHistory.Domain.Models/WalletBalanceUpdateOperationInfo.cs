﻿using System.Runtime.Serialization;

namespace Service.BalanceHistory.Domain.Models
{
    [DataContract]
    public class WalletBalanceUpdateOperationInfo
    {
        public WalletBalanceUpdateOperationInfo()
        {
        }

        public WalletBalanceUpdateOperationInfo(string operationId, string comment, string changeType, string applicationName, string applicationEnvInfo)
        {
            OperationId = operationId;
            Comment = comment;
            ChangeType = changeType;
            ApplicationName = applicationName;
            ApplicationEnvInfo = applicationEnvInfo;
        }

        [DataMember(Order = 1)] public string OperationId { get; set; }
        [DataMember(Order = 2)] public string Comment { get; set; }
        [DataMember(Order = 3)] public string ChangeType { get; set; }
        [DataMember(Order = 4)] public string ApplicationName { get; set; }
        [DataMember(Order = 5)] public string ApplicationEnvInfo { get; set; }
    }
}
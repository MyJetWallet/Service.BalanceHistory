using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain.Transactions;
using MyJetWallet.Sdk.Service;
using Service.BalanceHistory.Domain.Models;
using Service.BalanceHistory.Postgres.Models;

namespace Service.BalanceHistory.Postgres
{
    public class DatabaseContext : DbContext
    {
        public static DatabaseContext Create(DbContextOptionsBuilder<DatabaseContext> options)
        {
            var activity = MyTelemetry.StartActivity($"Database context {Schema}")?.AddTag("db-schema", Schema);

            var ctx = new DatabaseContext(options.Options) {_activity = activity};

            return ctx;
        }

        public const string Schema = "balancehistory";

        private const string SwapHistoryTableName = "swap_history";
        private const string TradeHistoryTableName = "trade_history";
        private const string BalanceHistoryTableName = "balance_history";
        private const string OperationInfoTableName = "operation_info";
        private const string OperationInfoRawDataTableName = "operation_info_rawdata";
        private const string CashInOutHistoryTableName = "cash_in_out_history";


        public DbSet<Swap> Swaps { get; set; }
        public DbSet<TradeHistoryEntity> Trades { get; set; }
        public DbSet<BalanceHistoryEntity> BalanceHistory { get; set; }
        public DbSet<WalletBalanceUpdateOperationInfoEntity> OperationInfo { get; set; }
        public DbSet<WalletBalanceUpdateOperationRawDataEntity> OperationInfoRawData { get; set; }

        public DbSet<CashInOutHistoryEntity> CashInOutHistory { get; set; }
        
        private Activity _activity;

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }
        public static ILoggerFactory LoggerFactory { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (LoggerFactory != null)
            {
                optionsBuilder.UseLoggerFactory(LoggerFactory).EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            SetSwapHistoryEntity(modelBuilder);
            SetTradeHistoryEntity(modelBuilder);
            SetBalanceHistoryEntity(modelBuilder);
            SetWalletBalanceEntity(modelBuilder);
            SetCashInOutHistoryEntity(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SetSwapHistoryEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Swap>().ToTable(SwapHistoryTableName);
            modelBuilder.Entity<Swap>().HasKey(e => new {e.OperationId, e.WalletId});
            modelBuilder.Entity<Swap>().Property(e => e.OperationId);
            modelBuilder.Entity<Swap>().Property(e => e.AccountId);
            modelBuilder.Entity<Swap>().Property(e => e.WalletId);
            modelBuilder.Entity<Swap>().Property(e => e.FromAsset);
            modelBuilder.Entity<Swap>().Property(e => e.ToAsset);
            modelBuilder.Entity<Swap>().Property(e => e.FromVolume);
            modelBuilder.Entity<Swap>().Property(e => e.ToVolume);
            modelBuilder.Entity<Swap>().Property(e => e.EventDate);
            modelBuilder.Entity<Swap>().Property(e => e.SequenceNumber);
            modelBuilder.Entity<Swap>().HasIndex(e => new { e.OperationId, e.WalletId });
            modelBuilder.Entity<Swap>().Property(e => e.SequenceNumber);
            modelBuilder.Entity<Swap>().HasIndex(e => e.SequenceNumber);
        }

        private void SetWalletBalanceEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WalletBalanceUpdateOperationInfoEntity>().ToTable(OperationInfoTableName);
            modelBuilder.Entity<WalletBalanceUpdateOperationInfoEntity>().HasKey(e => e.OperationId);
            modelBuilder.Entity<WalletBalanceUpdateOperationInfoEntity>().Property(e => e.OperationId).HasMaxLength(128);
            modelBuilder.Entity<WalletBalanceUpdateOperationInfoEntity>().Property(e => e.ChangeType).HasMaxLength(128);
            modelBuilder.Entity<WalletBalanceUpdateOperationInfoEntity>().Property(e => e.ApplicationEnvInfo).HasMaxLength(256);
            modelBuilder.Entity<WalletBalanceUpdateOperationInfoEntity>().Property(e => e.ApplicationName).HasMaxLength(256);
            modelBuilder.Entity<WalletBalanceUpdateOperationInfoEntity>().Property(e => e.Changer).HasMaxLength(512);
            modelBuilder.Entity<WalletBalanceUpdateOperationInfoEntity>().Property(e => e.Changer).IsRequired(false);
            modelBuilder.Entity<WalletBalanceUpdateOperationInfoEntity>().Property(e => e.TxId).HasMaxLength(1024);
            modelBuilder.Entity<WalletBalanceUpdateOperationInfoEntity>().Property(e => e.TxId).IsRequired(false);
            modelBuilder.Entity<WalletBalanceUpdateOperationInfoEntity>().Property(e => e.Status).HasDefaultValue(TransactionStatus.Confirmed);
            modelBuilder.Entity<WalletBalanceUpdateOperationInfoEntity>().Property(e => e.WithdrawalAddress).HasMaxLength(512);
            modelBuilder.Entity<WalletBalanceUpdateOperationInfoEntity>().Ignore(e => e.RawData);
            modelBuilder.Entity<WalletBalanceUpdateOperationRawDataEntity>().ToTable(OperationInfoRawDataTableName);
            modelBuilder.Entity<WalletBalanceUpdateOperationRawDataEntity>().HasKey(e => e.OperationId);
            modelBuilder.Entity<WalletBalanceUpdateOperationRawDataEntity>().Property(e => e.OperationId).HasMaxLength(128);
            modelBuilder.Entity<WalletBalanceUpdateOperationRawDataEntity>().Property(e => e.RawData).HasMaxLength(5*1024);
        }

        private void SetBalanceHistoryEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BalanceHistoryEntity>().ToTable(BalanceHistoryTableName);
            modelBuilder.Entity<BalanceHistoryEntity>().Property(e => e.Id).UseIdentityColumn();
            modelBuilder.Entity<BalanceHistoryEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<BalanceHistoryEntity>().Property(e => e.OperationId).HasMaxLength(128);
            modelBuilder.Entity<BalanceHistoryEntity>().Property(e => e.WalletId).HasMaxLength(128);
            modelBuilder.Entity<BalanceHistoryEntity>().Property(e => e.ClientId).HasMaxLength(128);
            modelBuilder.Entity<BalanceHistoryEntity>().Property(e => e.BrokerId).HasMaxLength(128);
            modelBuilder.Entity<BalanceHistoryEntity>().Property(e => e.Symbol).HasMaxLength(64);
            modelBuilder.Entity<BalanceHistoryEntity>().HasIndex(e => new {e.SequenceId, e.WalletId, e.Symbol}).IsUnique();
            modelBuilder.Entity<BalanceHistoryEntity>().HasIndex(e => e.WalletId);
            modelBuilder.Entity<BalanceHistoryEntity>().HasIndex(e => new { e.WalletId, e.Symbol });
            modelBuilder.Entity<BalanceHistoryEntity>().HasIndex(e => new { e.WalletId, e.SequenceId });
            modelBuilder.Entity<BalanceHistoryEntity>().HasIndex(e => new { e.WalletId, e.Symbol, e.SequenceId });
            modelBuilder.Entity<BalanceHistoryEntity>().HasIndex(e => e.SequenceId);
            modelBuilder.Entity<BalanceHistoryEntity>().HasIndex(e => e.OperationId);
        }

        private void SetTradeHistoryEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TradeHistoryEntity>().ToTable(TradeHistoryTableName);
            modelBuilder.Entity<TradeHistoryEntity>().Property(e => e.TradeId).UseIdentityColumn();
            modelBuilder.Entity<TradeHistoryEntity>().HasKey(e => e.TradeId);
            modelBuilder.Entity<TradeHistoryEntity>().HasIndex(e => e.TradeUId).IsUnique();
            modelBuilder.Entity<TradeHistoryEntity>().HasIndex(e => e.TradeUId);
            modelBuilder.Entity<TradeHistoryEntity>().HasIndex(e => e.WalletId);
            modelBuilder.Entity<TradeHistoryEntity>().HasIndex(e => new {e.WalletId, e.InstrumentSymbol});
            modelBuilder.Entity<TradeHistoryEntity>().HasIndex(e => new { e.WalletId, e.SequenceId });
            modelBuilder.Entity<TradeHistoryEntity>().HasIndex(e => new { e.WalletId, e.InstrumentSymbol, e.SequenceId });
            modelBuilder.Entity<TradeHistoryEntity>().HasIndex(e => e.SequenceId);
            modelBuilder.Entity<TradeHistoryEntity>().Property(e => e.OrderId).HasMaxLength(128);
            modelBuilder.Entity<TradeHistoryEntity>().Property(e => e.WalletId).HasMaxLength(128);
            modelBuilder.Entity<TradeHistoryEntity>().Property(e => e.ClientId).HasMaxLength(128);
            modelBuilder.Entity<TradeHistoryEntity>().Property(e => e.BrokerId).HasMaxLength(128);
            modelBuilder.Entity<TradeHistoryEntity>().Property(e => e.InstrumentSymbol).HasMaxLength(64);
            modelBuilder.Entity<TradeHistoryEntity>().Property(e => e.FeeAsset).HasMaxLength(64);
            modelBuilder.Entity<TradeHistoryEntity>().Property(e => e.TradeUId).HasMaxLength(128);
        }

        private void SetCashInOutHistoryEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CashInOutHistoryEntity>().ToTable(CashInOutHistoryTableName);
            modelBuilder.Entity<CashInOutHistoryEntity>().Property(e => e.Id).UseIdentityColumn();
            modelBuilder.Entity<CashInOutHistoryEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<CashInOutHistoryEntity>().Property(e => e.OperationId).HasMaxLength(128);
            modelBuilder.Entity<CashInOutHistoryEntity>().Property(e => e.WalletId).HasMaxLength(128);
            modelBuilder.Entity<CashInOutHistoryEntity>().Property(e => e.ClientId).HasMaxLength(128);
            modelBuilder.Entity<CashInOutHistoryEntity>().Property(e => e.BrokerId).HasMaxLength(128);
            modelBuilder.Entity<CashInOutHistoryEntity>().Property(e => e.Asset).HasMaxLength(64);
            modelBuilder.Entity<CashInOutHistoryEntity>().Property(e => e.FeeAsset).HasMaxLength(64);
            modelBuilder.Entity<CashInOutHistoryEntity>().HasIndex(e => new {e.SequenceId, e.WalletId, e.Asset}).IsUnique();
            modelBuilder.Entity<CashInOutHistoryEntity>().HasIndex(e => e.WalletId);
            modelBuilder.Entity<CashInOutHistoryEntity>().HasIndex(e => new { e.WalletId, e.Asset });
            modelBuilder.Entity<CashInOutHistoryEntity>().HasIndex(e => new { e.WalletId, e.SequenceId });
            modelBuilder.Entity<CashInOutHistoryEntity>().HasIndex(e => new { e.WalletId, e.Asset, e.SequenceId });
            modelBuilder.Entity<CashInOutHistoryEntity>().HasIndex(e => e.SequenceId);
            modelBuilder.Entity<CashInOutHistoryEntity>().HasIndex(e => e.OperationId);
        }

        public async Task<int> UpsetAsync(IEnumerable<TradeHistoryEntity> entities)
        {
            var result = await Trades.UpsertRange(entities).On(e => e.TradeUId).NoUpdate().RunAsync();
            return result;
        }
        public async Task<int> UpsetAsync(IEnumerable<Swap> entities)
        {
            var result = await Swaps
                .UpsertRange(entities)
                .On(e => new {e.OperationId, e.WalletId})
                .NoUpdate()
                .RunAsync();
            return result;
        }

        public async Task<int> UpsetAsync(IEnumerable<BalanceHistoryEntity> entities)
        {
            var result = await BalanceHistory.UpsertRange(entities).On(e => new { e.SequenceId, e.WalletId, e.Symbol}).NoUpdate().RunAsync();
            return result;
        }

        public async Task<int> UpsetAsync(IEnumerable<WalletBalanceUpdateOperationInfoEntity> entities)
        {
            var result = await OperationInfo
                .UpsertRange(entities)
                .On(e => new {Id = e.OperationId})
                .WhenMatched((oldEntity, newEntity) => new WalletBalanceUpdateOperationInfoEntity()
                {
                    OperationId = newEntity.OperationId,
                    Changer = newEntity.Changer,
                    TxId = newEntity.TxId,
                    Status = newEntity.Status,
                    ApplicationEnvInfo = newEntity.ApplicationEnvInfo,
                    ApplicationName = newEntity.ApplicationName,
                    ChangeType = newEntity.ChangeType,
                    Comment = newEntity.Comment
                })
                .RunAsync();
            return result;
        }

        public async Task<int> UpsetAsync(IEnumerable<WalletBalanceUpdateOperationRawDataEntity> entities)
        {
            var result = await OperationInfoRawData
                .UpsertRange(entities)
                .On(e => new { Id = e.OperationId })
                .WhenMatched((oldEntity, newEntity) => new WalletBalanceUpdateOperationRawDataEntity()
                {
                    OperationId = newEntity.OperationId,
                    RawData = newEntity.RawData
                })
                .RunAsync();
            return result;
        }
        
        public async Task<int> UpsetAsync(IEnumerable<CashInOutHistoryEntity> entities)
        {
            var result = await CashInOutHistory.UpsertRange(entities).On(e => new { e.SequenceId, e.WalletId, e.Asset}).NoUpdate().RunAsync();
            return result;
        }

        public override void Dispose()
        {
            _activity?.Dispose();
            base.Dispose();
        }
    }
}

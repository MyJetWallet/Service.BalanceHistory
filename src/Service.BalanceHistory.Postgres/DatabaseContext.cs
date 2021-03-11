using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Service.BalanceHistory.Postgres
{
    public class DatabaseContext : DbContext
    {
        public const string Schema = "balancehistory";

        public const string TradeHistoryTableName = "balance_history";
        public const string OperationInfoTableName = "operation_info";

        public DbSet<BalanceHistoryEntity> BalanceHistory { get; set; }
        public DbSet<WalletBalanceUpdateOperationInfoEntity> OperationInfo { get; set; }

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

            modelBuilder.Entity<BalanceHistoryEntity>().ToTable(TradeHistoryTableName);
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


            base.OnModelCreating(modelBuilder);
        }

        public async Task<int> UpsetAsync(IEnumerable<BalanceHistoryEntity> entities)
        {
            var result = await BalanceHistory.UpsertRange(entities).On(e => new { e.SequenceId, e.WalletId, e.Symbol}).NoUpdate().RunAsync();
            return result;
        }

        public async Task<int> UpsetAsync(IEnumerable<WalletBalanceUpdateOperationInfoEntity> entities)
        {
            var result = await OperationInfo.UpsertRange(entities).On(e => new {Id = e.OperationId}).NoUpdate().RunAsync();
            return result;
        }


    }
}

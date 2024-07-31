using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Context;

public class CoreContext : DbContext
{
    #region Constructor

    public CoreContext()
    {
    }

    public CoreContext(DbContextOptions<CoreContext> options) : base(options)
    {
    }

    #endregion

    #region Properties

    public DbSet<Model.Account> Accounts { get; set; }
    public DbSet<Model.Configuration> Configurations { get; set; }
    public DbSet<Model.RefreshToken> RefreshTokens { get; set; }
    public DbSet<Model.Log> Logs { get; set; }
    public DbSet<Model.PaymentTransaction> PaymentTransactions { get; set; }
    public DbSet<Model.TransactionTracking> TransactionTrackings { get; set; }

    #endregion

    #region Method

    // Use Fluent API
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Finds and runs all your configuration classes in the same assembly as the DbContext
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    #endregion
}
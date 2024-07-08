using Microsoft.EntityFrameworkCore;
using CSProsLibrary.Models;

namespace CSProsLibrary;

public interface IApplicationDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    public DbSet<Game> Games { get; }
    public DbSet<Player> Players { get; }
    public DbSet<Skin> Skins { get; }
    public DbSet<Country> Countries { get; }
    public DbSet<Map> Maps { get; }
    public DbSet<SkinUsage> SkinUsages { get; }
    public DbSet<Weapon> Weapons { get; }
    public DbSet<WeaponItem> WeaponItems { get; }
    public DbSet<Team> Teams { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
using Microsoft.EntityFrameworkCore;
using DhlShipment.Api.Models;

namespace DhlShipment.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Shipment> Shipments => Set<Shipment>();
}

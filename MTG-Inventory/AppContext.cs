using Microsoft.EntityFrameworkCore;
using MTG_Inventory.Models;

namespace MTG_Inventory;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Card> Card { get; set; }
    public DbSet<Deck> Deck { get; set; }
    public DbSet<CardDeck> CardDeck { get; set; }
}
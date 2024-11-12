using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ItbApi
{
    public class ItbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Brewery> Breweries { get; set; }
        public DbSet<BeverageGroup> BeverageGroups { get; set; }
        public DbSet<BeverageType> BeverageTypes { get; set; }
        public DbSet<Beverage> Beverages { get; set; }
        public DbSet<ReviewType> ReviewTypes { get; set; }
        public DbSet<Room2ReviewType> Room2ReviewTypes { get; set; }
        public DbSet<BeverageReview> BeverageReviews { get; set; }
        public DbSet<ReviewPart> ReviewParts { get; set; }
        public DbSet<Room2Beverage> Room2Beverages { get; set; }

        public ItbContext(DbContextOptions<ItbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Set delete behavior to Restrict for User and Room relationships in BeverageReview
            modelBuilder.Entity<BeverageReview>()
                .HasOne(br => br.User)
                .WithMany(u => u.BeverageReviews)
                .HasForeignKey(br => br.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BeverageReview>()
                .HasOne(br => br.Room)
                .WithMany(r => r.BeverageReviews)
                .HasForeignKey(br => br.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // Call base method if needed
            base.OnModelCreating(modelBuilder);
        }
    }

    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public int RoomId { get; set; }
        [MaxLength(50)]
        public string? Name { get; set; }
        public bool IsOwner { get; set; }
        public DateTime DateCreated { get; set; }

        [ForeignKey("RoomId")]
        public Room Room { get; set; }
        public ICollection<BeverageReview> BeverageReviews { get; set; }
    }

    public class Room
    {
        [Key]
        public int Id { get; set; }
        public string? Code { get; set; }
        public int Pin { get; set; }
        [MaxLength(50)]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; }
        public string? Owner { get; set; }
        public int RoomType { get; set; }
        public string? BlindRevealCode { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Room2ReviewType> Room2ReviewTypes { get; set; }
        public ICollection<BeverageReview> BeverageReviews { get; set; }
        public ICollection<Room2Beverage> Room2Beverages { get; set; }
    }

    public class Brewery
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string? Name { get; set; }
        public string? Logo { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Twitter { get; set; }
        public string? Facebook { get; set; }
        public string? WebPage { get; set; }
        public DateTime DateAdded { get; set; }

        public ICollection<Beverage> Beverages { get; set; }
    }

    public class BeverageGroup
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(20)]
        public string? Name { get; set; }
        public int DisplayOrder { get; set; }

        public ICollection<Beverage> Beverages { get; set; }
    }

    public class BeverageType
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string? Name { get; set; }
        public int DisplayOrder { get; set; }

        public ICollection<Beverage> Beverages { get; set; }
    }

    public class Beverage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public int BreweryId { get; set; }
        [Required]
        public decimal AlcoholPercentage { get; set; }
        [Required]
        public decimal IBU { get; set; }
        public string? Description { get; set; }
        [Required]
        public int MajorGroup { get; set; }
        public string? ImageUrlSm { get; set; }
        public string? ImageUrlMed { get; set; }
        public string? LableSm { get; set; }
        public string? LableMed { get; set; }
        [Required]
        public int Type { get; set; }
        [Required]
        public bool Accepted { get; set; }
        public string? IpAddress { get; set; }
        public decimal UtappedRating { get; set; }
        public DateTime DateCreated { get; set; }

        [ForeignKey("BreweryId")]
        public Brewery Brewery { get; set; }
        [ForeignKey("MajorGroup")]
        public BeverageGroup BeverageGroup { get; set; }
        [ForeignKey("Type")]
        public BeverageType BeverageType { get; set; }

        public ICollection<Room2Beverage> Room2Beverages { get; set; }
    }

    public class ReviewType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public bool Accepted { get; set; }
        [Required, MaxLength(200)]
        public string? IpAddress { get; set; }
        public DateTime DateAdded { get; set; }
        public string? Abbr { get; set; }

        public ICollection<Room2ReviewType> Room2ReviewTypes { get; set; }
        public ICollection<ReviewPart> ReviewParts { get; set; }
    }

    public class Room2ReviewType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int RoomId { get; set; }
        [Required]
        public int ReviewTypeId { get; set; }
        public int DisplayOrder { get; set; }
        public int MaxValue { get; set; }

        [ForeignKey("RoomId")]
        public Room Room { get; set; }
        [ForeignKey("ReviewTypeId")]
        public ReviewType ReviewType { get; set; }
    }

    public class BeverageReview
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public int BeverageId { get; set; }
        [Required]
        public int RoomId { get; set; }
        public decimal TotalScore { get; set; }
        public DateTime DateCreated { get; set; }
        [MaxLength(250)]
        public string? Comment { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        [ForeignKey("RoomId")]
        public Room Room { get; set; }
        public ICollection<ReviewPart> ReviewParts { get; set; }
    }

    public class ReviewPart
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public long BeverageReviewId { get; set; }
        [Required]
        public int ReviewTypeId { get; set; }
        public decimal Score { get; set; }

        [ForeignKey("BeverageReviewId")]
        public BeverageReview BeverageReview { get; set; }
        [ForeignKey("ReviewTypeId")]
        public ReviewType ReviewType { get; set; }
    }

    public class Room2Beverage
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int RoomId { get; set; }
        [Required]
        public int BeverageId { get; set; }

        [ForeignKey("RoomId")]
        public Room Room { get; set; }
        [ForeignKey("BeverageId")]
        public Beverage Beverage { get; set; }

        public int DisplayOrder { get; set; }
        public int FinalScore { get; set; }
        public bool IsLocked { get; set; }
        public bool ReviewFinished { get; set; }
    }
}

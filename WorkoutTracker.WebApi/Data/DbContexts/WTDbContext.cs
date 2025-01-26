using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using WorkoutTracker.WebApi.Data.Entities;

namespace WorkoutTracker.WebApi.Data.DbContexts
{
    public class WTDbContext : DbContext
    {
        public DbSet<WTUserEntity> Users { get; set; }

        public DbSet<WorkoutDayEntity> WorkoutDays { get; set; }

        public DbSet<WorkoutDayExerciseEntity> WorkoutDay_Exercise { get; set; }

        public DbSet<ExerciseEntity> Exercises { get; set; }

        public WTDbContext(DbContextOptions<WTDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Users
            modelBuilder.Entity<WTUserEntity>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Username)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.Password)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.HasMany(e => e.WorkoutDays)
                      .WithOne(e => e.User)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                SeedDataFromFile<WTUserEntity>("Users.Json", modelBuilder);
            });

            // WorkoutDays
            modelBuilder.Entity<WorkoutDayEntity>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.UserId, e.WorkoutDate })
                      .IsUnique()
                      .HasDatabaseName("Index_WorkoutDay_UserId_WorkoutDate");

                entity.Property(e => e.WorkoutTime)
                      .HasDefaultValueSql("GETUTCTIME()");

                entity.Property(e => e.CreatedOn)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedOn)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.HasMany(e => e.WorkoutDayExercises)
                      .WithOne(e => e.WorkoutDay)
                      .HasForeignKey(e => e.WorkoutDayId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                      .WithMany(e => e.WorkoutDays)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.NoAction);

                SeedDataFromFile<WorkoutDayEntity>("WorkoutDays.json", modelBuilder);
            });

            // Exercises
            modelBuilder.Entity<ExerciseEntity>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(280);

                entity.Property(e => e.MuscleGroup)
                      .IsRequired();

                entity.Property(e => e.CreatedOn)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedOn)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.HasMany(e => e.WorkoutDayExercises)
                      .WithOne(e => e.Exercise)
                      .HasForeignKey(e => e.ExerciseId)
                      .OnDelete(DeleteBehavior.Cascade);

                SeedDataFromFile<ExerciseEntity>("Exercises.Json", modelBuilder);
            });

            // WorkoutDay_Exercises
            modelBuilder.Entity<WorkoutDayExerciseEntity>(entity =>
            {
                entity.ToTable("WorkoutDay_Exercise");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.CreatedOn)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedOn)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.WorkoutDay)
                      .WithMany(e => e.WorkoutDayExercises)
                      .HasForeignKey(e => e.WorkoutDayId);

                entity.HasOne(e => e.Exercise)
                      .WithMany(e => e.WorkoutDayExercises)
                      .HasForeignKey(e => e.ExerciseId);

                SeedDataFromFile<WorkoutDayExerciseEntity>("WorkoutDayExercises.json", modelBuilder);
            });
        }

        private void SeedDataFromFile<TEntity>(string fileName, ModelBuilder modelBuilder)
            where TEntity : class
        {
            var jsonData = File.ReadAllText("Data/Seeds/" + fileName);
            var entities = System.Text.Json.JsonSerializer.Deserialize<List<TEntity>>(jsonData);
            modelBuilder.Entity<TEntity>().HasData(entities!);
        }
    }
}

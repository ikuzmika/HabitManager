using HabitManager.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace HabitManager.Infrastructure.Persistence;

public class HabitDbContext : DbContext
{
    public HabitDbContext(DbContextOptions<HabitDbContext> options)
        : base(options)
    {
    }

    public DbSet<HabitEntity> Habits => Set<HabitEntity>();

    public DbSet<HabitScheduleDayEntity> HabitScheduleDays =>
        Set<HabitScheduleDayEntity>();

    public DbSet<HabitCompletionEntity> HabitCompletions =>
        Set<HabitCompletionEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<HabitEntity>(entity =>
        {
            entity.ToTable("Habits");

            entity.HasKey(habit => habit.Id);

            entity.Property(habit => habit.Id)
                .ValueGeneratedNever();

            entity.Property(habit => habit.UserId)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(habit => habit.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(habit => habit.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(habit => habit.ScheduleType)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(habit => habit.CreatedAt)
                .IsRequired();

            entity.Property(habit => habit.UpdatedAt)
                .IsRequired();

            entity.HasIndex(habit => habit.UserId);
        });
        
        modelBuilder.Entity<HabitScheduleDayEntity>(entity =>
        {
            entity.ToTable("HabitScheduleDays");

            entity.HasKey(day => new
            {
                day.HabitId,
                day.Day
            });

            entity.HasOne(day => day.Habit)
                .WithMany(habit => habit.ScheduleDays)
                .HasForeignKey(day => day.HabitId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<HabitCompletionEntity>(entity =>
        {
            entity.ToTable("HabitCompletions");

            entity.HasKey(completion => completion.Id);

            entity.Property(completion => completion.Id)
                .ValueGeneratedNever();

            entity.Property(completion => completion.Date)
                .HasColumnType("date")
                .IsRequired();

            entity.HasIndex(completion => new
            {
                completion.HabitId,
                completion.Date
            })
                .IsUnique();

            entity.HasOne(completion => completion.Habit)
                .WithMany(habit => habit.Completions)
                .HasForeignKey(completion => completion.HabitId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
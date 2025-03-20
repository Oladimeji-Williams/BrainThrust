using Microsoft.EntityFrameworkCore;
using BrainThrust.src.Models.Entities;
using System.Linq.Expressions;
using BrainThrust.src.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<LessonProgress> LessonProgresses { get; set; }
    public DbSet<TopicProgress> TopicProgresses { get; set; }
    public DbSet<SubjectProgress> SubjectProgresses { get; set; }
    public DbSet<UserQuizSubmission> UserQuizSubmissions { get; set; }
    public DbSet<UserQuizAttempt> UserQuizAttempts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Define relationships
        modelBuilder.Entity<UserQuizSubmission>()
            .HasOne(uqs => uqs.User)
            .WithMany(u => u.QuizSubmissions)
            .HasForeignKey(uqs => uqs.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserQuizSubmission>()
            .HasOne(uqs => uqs.Quiz)
            .WithMany()
            .HasForeignKey(uqs => uqs.QuizId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserQuizSubmission>()
            .HasOne(uqs => uqs.UserQuizAttempt)
            .WithMany(uqa => uqa.Submissions)
            .HasForeignKey(uqs => uqs.UserQuizAttemptId)
            .OnDelete(DeleteBehavior.Restrict);
        

        modelBuilder.Entity<Quiz>()
            .HasMany(q => q.Questions)
            .WithOne()
            .HasForeignKey(q => q.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Question>()
            .HasOne(q => q.Quiz)
            .WithMany(qz => qz.Questions)
            .HasForeignKey(q => q.QuizId);

        // Apply soft delete query filter
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType) && entityType.ClrType.GetProperty("IsDeleted") != null)
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, "IsDeleted");
                var condition = Expression.Equal(property, Expression.Constant(false));
                var lambda = Expression.Lambda(condition, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                modelBuilder.Entity(entityType.ClrType).Property("IsDeleted").HasDefaultValue(false);
            }
        }
    }

    public override int SaveChanges()
    {
        HandleSoftDelete();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        HandleSoftDelete();
        return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private void HandleSoftDelete()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.Created = DateTime.UtcNow;
                    entry.Entity.Modified = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.Modified = DateTime.UtcNow;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DateDeleted = DateTime.UtcNow;
                    break;
            }
        }
    }
}

using api.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class DataContext : IdentityDbContext<IdentityUser>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) {}

    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<MeetingAttendee> MeetingAttendees { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<UserService> UserServices { get; set; }
    public DbSet<GoogleAuth> GoogleAuths { get; set; }
    public DbSet<AvailabilityHour> AvailabilityHours { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Meeting>(entity =>
        {
            entity.ToTable("meetings");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("uuid_generate_v4()");

            entity.Property(e => e.Title)
                .HasColumnName("title")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasColumnName("description");
            
            entity.Property(e => e.ExternalId)
                .HasColumnName("external_id")      
                .HasMaxLength(1024);

            entity.Property(e => e.StartTime)
                .HasColumnName("start_time")
                .HasColumnType("timestamp without time zone")
                .IsRequired();

            entity.Property(e => e.EndTime)
                .HasColumnName("end_time")
                .HasColumnType("timestamp without time zone")
                .IsRequired();
            
            entity.Property(e => e.ServiceId)
                .HasColumnName("service_id");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        
        modelBuilder.Entity<MeetingAttendee>(entity =>
        {
            entity.ToTable("meeting_attendees");
            
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("uuid_generate_v4()");

            entity.Property(e => e.MeetingId)
                .HasColumnName("meeting_id");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            entity.Property(e => e.IsCreator)
                .HasColumnName("is_creator")
                .IsRequired();

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasConversion<int>();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.UserName)
                .HasColumnName("user_name")
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.FirstName)
                .HasColumnName("first_name")
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.LastName)
                .HasColumnName("last_name")
                .IsRequired()
                .HasMaxLength(50); 
            
            entity.HasIndex(u => u.EmailAddress).IsUnique();

            entity.Property(u => u.EmailAddress)
                .HasColumnName("email_address")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.PhotoUrl)
                .HasColumnName("photo_url");
            
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Address)
                .HasColumnName("address");
            
            entity.Property(e => e.CountryCode)
                .HasColumnName("country_code")
                .HasMaxLength(2);  
            
            entity.Property(e => e.LangCode)
                .HasColumnName("lang_code")
                .HasMaxLength(5);  
            
            entity.Property(e => e.PreferredTimeZone)
                .HasColumnName("preferred_time_zone")
                .HasMaxLength(50);  
            
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        
        modelBuilder.Entity<Service>(entity =>
        {
            entity.ToTable("services");
            
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.Duration)
                .HasColumnName("duration")
                .IsRequired();
            
            entity.Property(e => e.Price)
                .HasColumnName("price")
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        
        modelBuilder.Entity<UserService>(entity =>
        {
            entity.ToTable("user_services");
            
            entity.HasKey(us => new { us.UserId, us.ServiceId });

            entity.Property(us => us.UserId)
                .HasColumnName("user_id"); 
            
            entity.HasOne(us => us.User)
                .WithMany(u => u.UserServices)
                .HasForeignKey(us => us.UserId);

            entity.Property(us => us.ServiceId)
                .HasColumnName("service_id");

            entity.HasOne(us => us.Service)
                .WithMany(s => s.UserServices)
                .HasForeignKey(us => us.ServiceId);

            entity.Property(us => us.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(us => us.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        
        modelBuilder.Entity<GoogleAuth>(entity =>
        {
            entity.ToTable("google_auths");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("uuid_generate_v4()");
            
            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();
            
            entity.Property(e => e.AccessToken)
                .HasColumnName("access_token")
                .IsRequired();

            entity.Property(e => e.ExpiresIn)
                .HasColumnName("expires_in")
                .IsRequired();
            
            entity.Property(e => e.RefreshToken)
                .HasColumnName("refresh_token")
                .IsRequired();

            entity.Property(e => e.Scope)
                .HasColumnName("scope")
                .IsRequired();

            entity.Property(e => e.TokenType)
                .HasColumnName("token_type")
                .IsRequired()
                .HasMaxLength(32);

            entity.Property(e => e.IdToken)
                .HasColumnName("id_token")
                .IsRequired();
        });
        
        modelBuilder.Entity<AvailabilityHour>(entity =>
        {
            entity.ToTable("availability_hours");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("uuid_generate_v4()");
            
            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();
            
            entity.Property(e => e.DayOfWeek)
                .HasColumnName("day_of_week");

            entity.Property(e => e.StartTime)
                .HasColumnName("start_time")
                .HasColumnType("time without time zone")
                .IsRequired();

            entity.Property(e => e.EndTime)
                .HasColumnName("end_time")
                .HasColumnType("time without time zone")
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
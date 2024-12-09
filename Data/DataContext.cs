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
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<UserService> UserServices { get; set; }
    public DbSet<GoogleAuth> GoogleAuths { get; set; }
    public DbSet<WeeklyHour> WeeklyHours { get; set; }
    public DbSet<DateSpecificHour> DateSpecificHours { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityUser>(entity =>
        {
            entity.ToTable("user_credentials");
            
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserName).HasColumnName("userName");
            entity.Property(e => e.NormalizedUserName).HasColumnName("normalizedUserName");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.NormalizedEmail).HasColumnName("normalizedEmail");
            entity.Property(e => e.EmailConfirmed).HasColumnName("emailConfirmed");
            entity.Property(e => e.PasswordHash).HasColumnName("passwordHash");
            entity.Property(e => e.SecurityStamp).HasColumnName("securityStamp");
            entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrencyStamp");
            entity.Property(e => e.PhoneNumber).HasColumnName("phoneNumber");
            entity.Property(e => e.PhoneNumberConfirmed).HasColumnName("phoneNumberConfirmed");
            entity.Property(e => e.TwoFactorEnabled).HasColumnName("twoFactorEnabled");
            entity.Property(e => e.LockoutEnd).HasColumnName("lockoutEnd");
            entity.Property(e => e.LockoutEnabled).HasColumnName("lockoutEnabled");
            entity.Property(e => e.AccessFailedCount).HasColumnName("accessFailedCount");
        });

        modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("user_claims");
            
            entity.HasKey(e => e.Id).HasName("PK_user_claims");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.ClaimType).HasColumnName("claimType");
            entity.Property(e => e.ClaimValue).HasColumnName("claimValue");

            entity.HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_user_claims_user_credentials_UserId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("user_logins");
            
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey }).HasName("PK_user_logins");

            entity.Property(e => e.LoginProvider).HasColumnName("loginProvider");
            entity.Property(e => e.ProviderKey).HasColumnName("providerKey");
            entity.Property(e => e.ProviderDisplayName).HasColumnName("providerDisplayName");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_user_logins_user_credentials_UserId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("user_tokens");
            
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name }).HasName("PK_user_tokens");

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.LoginProvider).HasColumnName("loginProvider");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Value).HasColumnName("value");

            entity.HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_user_tokens_user_credentials_UserId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<IdentityRole>(entity =>
        {
            entity.ToTable("roles");
            
            entity.HasKey(e => e.Id).HasName("PK_roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.NormalizedName).HasColumnName("normalizedName");
            entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrencyStamp");
        });

        modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("role_claims");
            
            entity.HasKey(e => e.Id).HasName("PK_role_claims");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.ClaimType).HasColumnName("claimType");
            entity.Property(e => e.ClaimValue).HasColumnName("claimValue");

            entity.HasOne<IdentityRole>()
                .WithMany()
                .HasForeignKey(e => e.RoleId)
                .HasConstraintName("FK_role_claims_roles_RoleId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("user_roles");
            
            entity.HasKey(e => new { e.UserId, e.RoleId }).HasName("PK_user_roles");

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.RoleId).HasColumnName("roleId");

            entity.HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_user_roles_user_credentials_UserId")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<IdentityRole>()
                .WithMany()
                .HasForeignKey(e => e.RoleId)
                .HasConstraintName("FK_user_roles_roles_RoleId")
                .OnDelete(DeleteBehavior.Cascade);
        });
        
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
       
        modelBuilder.Entity<Profile>(entity =>
        {
            entity.ToTable("profiles");
            
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.ProfileName)
                .HasColumnName("profile_name")
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(u => u.EmailAddress).IsUnique();

            entity.Property(u => u.EmailAddress)
                .HasColumnName("email_address")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.PhotoUrl)
                .HasColumnName("photo_url");

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
            
            entity.Property(e => e.IsOrg)
                .HasColumnName("is_org")
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
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
            
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
            
            entity.Property(e => e.ProfileId)
                .HasColumnName("profile_id");

            entity.HasOne(u => u.Profile)
                .WithMany(p => p.Users)
                .HasForeignKey(u => u.ProfileId);
            
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
            
            entity.Property(e => e.DefaultDuration)
                .HasColumnName("default_duration")
                .IsRequired();
            
            entity.Property(e => e.DefaultPrice)
                .HasColumnName("default_price")
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
            
            entity.Property(e => e.Duration)
                .HasColumnName("duration")
                .IsRequired();
            
            entity.Property(e => e.Price)
                .HasColumnName("price")
                .IsRequired();

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
        
        modelBuilder.Entity<WeeklyHour>(entity =>
        {
            entity.ToTable("weekly_hours");

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
        
        modelBuilder.Entity<DateSpecificHour>(entity =>
        {
            entity.ToTable("date_specific_hours");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("uuid_generate_v4()");
            
            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();
            
            entity.Property(e => e.SpecificDate)
                .HasColumnName("specific_date");

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
            
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
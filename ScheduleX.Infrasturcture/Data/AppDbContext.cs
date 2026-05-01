using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;





namespace ScheduleX.Infrastructure.Data;

public class AppDbContext
    : IdentityDbContext<User, IdentityRole<int>, int> 
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSets (tables)
    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>(); // ✅ ADDED

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<TTCoordinatorCourse> TTCoordinatorCourses { get; set; }
    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<Division> Divisions => Set<Division>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<DivisionRoomAllocation> DivisionRoomAllocations => Set<DivisionRoomAllocation>();
    public DbSet<Faculty> Faculties => Set<Faculty>();
    public DbSet<FacultyAvailability> FacultyAvailabilities => Set<FacultyAvailability>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<SubjectSemester> SubjectSemesters => Set<SubjectSemester>();
    public DbSet<SubjectFaculty> SubjectFaculties => Set<SubjectFaculty>();
    public DbSet<SubjectLectureConfig> SubjectLectureConfigs => Set<SubjectLectureConfig>();
    public DbSet<SubjectRoomConfig> SubjectRoomConfigs => Set<SubjectRoomConfig>();
    public DbSet<ScheduleConfig> ScheduleConfigs => Set<ScheduleConfig>();
    public DbSet<BreakRule> BreakRules => Set<BreakRule>();
    public DbSet<TimeTableTemplate> TimeTableTemplates => Set<TimeTableTemplate>();
    public DbSet<TimeTableBatch> TimeTableBatches => Set<TimeTableBatch>();
    public DbSet<TimeTableBatchSemester> TimeTableBatchSemesters => Set<TimeTableBatchSemester>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<TimeTableEntry> TimeTableEntries => Set<TimeTableEntry>();

    // ❌ REMOVED:
    // public DbSet<BatchTemplateSnapshot> BatchTemplateSnapshots
    // public DbSet<TimeTableEntryHistory> TimeTableEntryHistories
    // public DbSet<ExportHistory> ExportHistories

    public DbSet<SemesterStudentStrength> SemesterStudentStrengths { get; set; }
    public DbSet<ExternalFacultyPermission> ExternalFacultyPermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ✅ Table names
        modelBuilder.Entity<AcademicYear>().ToTable("TblAcademicYear"); // ✅ ADDED

        modelBuilder.Entity<Department>().ToTable("TblDepartment");
        modelBuilder.Entity<User>().ToTable("TblUser");
        modelBuilder.Entity<Course>().ToTable("TblCourse");
        modelBuilder.Entity<TTCoordinatorCourse>().ToTable("TblTTCoordinatorCourse");
        modelBuilder.Entity<Semester>().ToTable("TblSemester");
        modelBuilder.Entity<Division>().ToTable("TblDivision");
        modelBuilder.Entity<Room>().ToTable("TblRoom");
        modelBuilder.Entity<DivisionRoomAllocation>().ToTable("TblDivisionRoomAllocation");
        modelBuilder.Entity<Faculty>().ToTable("TblFaculty");
        modelBuilder.Entity<FacultyAvailability>().ToTable("TblFacultyAvailability");
        modelBuilder.Entity<Subject>().ToTable("TblSubject");
        modelBuilder.Entity<SubjectSemester>().ToTable("TblSubjectSemester");
        modelBuilder.Entity<SubjectFaculty>().ToTable("TblSubjectFaculty");
        modelBuilder.Entity<SubjectLectureConfig>().ToTable("TblSubjectLectureConfig");
        modelBuilder.Entity<SubjectRoomConfig>().ToTable("TblSubjectRoomConfig");
        modelBuilder.Entity<ScheduleConfig>().ToTable("TblScheduleConfig");
        modelBuilder.Entity<BreakRule>().ToTable("TblBreakRule");
        modelBuilder.Entity<TimeTableTemplate>().ToTable("TblTimeTableTemplate");
        modelBuilder.Entity<TimeTableBatch>().ToTable("TblTimeTableBatch");
        modelBuilder.Entity<TimeTableBatchSemester>().ToTable("TblTimeTableBatchSemester");
        modelBuilder.Entity<TimeSlot>().ToTable("TblTimeSlot");
        modelBuilder.Entity<TimeTableEntry>().ToTable("TblTimeTableEntry");

        // ❌ REMOVED mappings:
        // modelBuilder.Entity<BatchTemplateSnapshot>()
        // modelBuilder.Entity<TimeTableEntryHistory>()
        // modelBuilder.Entity<ExportHistory>()

        // =========================
        // UNIQUE CONSTRAINTS
        // =========================

        modelBuilder.Entity<Department>()
            .HasIndex(x => x.DepartmentName).IsUnique();

        modelBuilder.Entity<Department>()
            .HasIndex(x => x.DepartmentCode).IsUnique()
            .HasFilter("[DepartmentCode] IS NOT NULL");

        modelBuilder.Entity<User>()
    .HasIndex(x => x.UserName).IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<User>()
     .HasIndex(u => u.PhoneNumber).IsUnique();

        modelBuilder.Entity<Course>()
            .HasIndex(x => x.CourseCode).IsUnique()
            .HasFilter("[CourseCode] IS NOT NULL");

        modelBuilder.Entity<Semester>()
            .HasIndex(x => new { x.CourseId, x.SemesterNo }).IsUnique();

        modelBuilder.Entity<Division>()
            .HasIndex(x => new { x.SemesterId, x.DivisionName }).IsUnique();

        modelBuilder.Entity<Room>()
            .HasIndex(x => x.RoomName).IsUnique();

        modelBuilder.Entity<DivisionRoomAllocation>()
            .HasIndex(x => x.DivisionId).IsUnique();

        modelBuilder.Entity<Faculty>()
            .HasIndex(x => x.FacultyCode).IsUnique()
            .HasFilter("[FacultyCode] IS NOT NULL");

        modelBuilder.Entity<Subject>()
            .HasIndex(x => x.SubjectCode).IsUnique()
            .HasFilter("[SubjectCode] IS NOT NULL");

        modelBuilder.Entity<SubjectSemester>()
            .HasIndex(x => new { x.SubjectId, x.SemesterId }).IsUnique();

        modelBuilder.Entity<SubjectFaculty>()
            .HasIndex(x => new { x.SubjectSemesterId, x.DivisionId, x.TeachingType }).IsUnique();

        modelBuilder.Entity<SubjectLectureConfig>()
            .HasIndex(x => x.SubjectSemesterId).IsUnique();

        modelBuilder.Entity<SubjectRoomConfig>()
            .HasIndex(x => x.SubjectSemesterId).IsUnique();

        modelBuilder.Entity<ExternalFacultyPermission>()
            .HasIndex(x => new { x.FacultyId, x.DepartmentId }).IsUnique();

        modelBuilder.Entity<BreakRule>()
            .HasIndex(x => new { x.ConfigId, x.BreakNo }).IsUnique();

        modelBuilder.Entity<BreakRule>()
            .HasIndex(x => new { x.ConfigId, x.AfterLectureNo }).IsUnique();

        modelBuilder.Entity<TimeTableTemplate>()
            .HasIndex(x => x.TemplateName).IsUnique();

        modelBuilder.Entity<TimeTableBatchSemester>()
            .HasIndex(x => new { x.BatchId, x.SemesterId }).IsUnique();

        modelBuilder.Entity<TimeSlot>()
            .HasIndex(x => new { x.ConfigId, x.SlotNo }).IsUnique();

        modelBuilder.Entity<TTCoordinatorCourse>()
            .HasIndex(x => new { x.UserId, x.CourseId }).IsUnique();

        modelBuilder.Entity<TimeTableEntry>()
            .HasIndex(x => new { x.BatchId, x.DivisionId, x.DayOfWeek, x.TimeSlotId }).IsUnique();

        modelBuilder.Entity<TimeTableEntry>()
            .HasIndex(x => new { x.BatchId, x.RoomId, x.DayOfWeek, x.TimeSlotId })
            .IsUnique()
            .HasFilter("[RoomId] IS NOT NULL");

        // =========================
        // RELATIONSHIPS
        // =========================

        foreach (var fk in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys()))
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }

        modelBuilder.Entity<FacultyAvailability>()
            .HasOne(x => x.Faculty)
            .WithMany(x => x.FacultyAvailabilities)
            .HasForeignKey(x => x.FacultyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BreakRule>()
            .HasOne(x => x.ScheduleConfig)
            .WithMany(x => x.BreakRules)
            .HasForeignKey(x => x.ConfigId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExternalFacultyPermission>()
            .ToTable("TblExternalFacultyPermission");
    }
}
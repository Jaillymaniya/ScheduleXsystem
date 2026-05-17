using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Infrastructure.Repositories.TTCoordinator
{
    public class TimetableRepository : ITimetableRepository
    {
        private readonly AppDbContext _context;

        public TimetableRepository(AppDbContext context)
        {
            _context = context;
        }
        private async Task GenerateTimeSlotsAsync(ScheduleConfig config)
        {
            var breakRules = await _context.BreakRules
                .Where(x => x.ConfigId == config.ConfigId)
                .OrderBy(x => x.AfterLectureNo)
                .ToListAsync();

            var slots = new List<TimeSlot>();

            var currentTime = config.StartTime;
            var slotNo = 1;
            var lectureCounter = 0;

            while (currentTime < config.EndTime)
            {
                lectureCounter++;

                var lectureEnd = currentTime.AddMinutes(config.LectureDurationMin);

                if (lectureEnd > config.EndTime)
                    break;

                slots.Add(new TimeSlot
                {
                    ConfigId = config.ConfigId,
                    SlotNo = slotNo++,
                    StartTime = currentTime,
                    EndTime = lectureEnd,
                    SlotType = SlotTypeEnum.Lecture
                });

                currentTime = lectureEnd;

                var breakRule = breakRules
                    .FirstOrDefault(x => x.AfterLectureNo == lectureCounter);

                if (breakRule != null)
                {
                    var breakEnd = currentTime.AddMinutes(breakRule.BreakDurationMin);

                    if (breakEnd > config.EndTime)
                        break;

                    slots.Add(new TimeSlot
                    {
                        ConfigId = config.ConfigId,
                        SlotNo = slotNo++,
                        StartTime = currentTime,
                        EndTime = breakEnd,
                        SlotType = SlotTypeEnum.Break,
                        BreakRuleId = breakRule.BreakRuleId
                    });

                    currentTime = breakEnd;
                }
            }

            await _context.TimeSlots.AddRangeAsync(slots);
            await _context.SaveChangesAsync();
        }
        //keep
        public async Task<(bool Success, string Message, int BatchId, List<TimeTableEntry> Entries)>
            GenerateAsync(
                int userId,
                int academicYearId,
                int academicTermId,
                int courseId,
                int templateId)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                // =========================
                // VALIDATE COORDINATOR ACCESS
                // =========================
                var allowed = await _context.TTCoordinatorCourses
                    .AnyAsync(x =>
                        x.UserId == userId &&
                        x.CourseId == courseId &&
                        x.IsActive);

                if (!allowed)
                {
                    return (false,
                        "You are not assigned to this course.",
                        0,
                        new List<TimeTableEntry>());
                }

                // =========================
                // LOAD TERM
                // =========================
                var term = await _context.AcademicTerms
                    .FirstOrDefaultAsync(x =>
                        x.AcademicTermId == academicTermId &&
                        x.AcademicYearId == academicYearId &&
                        x.CourseId == courseId);

                if (term == null)
                {
                    return (false,
                        "Academic term not found.",
                        0,
                        new List<TimeTableEntry>());
                }

                // =========================
                // LOAD CONFIG
                // =========================
                var config = await _context.ScheduleConfigs
                    .Include(x => x.TimeSlots)
                    .Include(x => x.BreakRules)
                    .FirstOrDefaultAsync(x =>
                        x.AcademicYearId == academicYearId &&
                        x.CourseId == courseId &&
                        x.IsActive);

                if (config == null)
                {
                    return (false,
                        "No active schedule configuration found.",
                        0,
                        new List<TimeTableEntry>());
                }

                if (!config.TimeSlots.Any())
                {
                    await GenerateTimeSlotsAsync(config);

                    config = await _context.ScheduleConfigs
                        .Include(x => x.TimeSlots)
                        .Include(x => x.BreakRules)
                        .FirstAsync(x => x.ConfigId == config.ConfigId);
                }

                // =========================
                // LOAD TEMPLATE
                // =========================
                var template = await _context.TimeTableTemplates
                    .FirstOrDefaultAsync(x =>
                        x.TemplateId == templateId &&
                        x.IsActive);

                if (template == null)
                {
                    return (false,
                        "Timetable template not found.",
                        0,
                        new List<TimeTableEntry>());
                }

                // =========================
                // GET ELIGIBLE SEMESTERS
                // =========================
                var semesters = await GetEligibleSemestersAsync(term);

                if (!semesters.Any())
                {
                    return (false,
                        "No semesters found for selected term.",
                        0,
                        new List<TimeTableEntry>());
                }

                // =========================
                // GET DIVISIONS
                // =========================
                var semesterIds = semesters
                    .Select(x => x.SemesterId)
                    .ToList();

                var divisions = await _context.Divisions
                    .Include(x => x.DivisionRoomAllocations)
                    .Include(x => x.Course)
                     .ThenInclude(x => x.Department)
                    .Where(x =>
                        x.AcademicYearId == academicYearId &&
                        x.CourseId == courseId &&
                        x.IsActive &&
                        semesterIds.Contains(x.SemesterId))
                    .ToListAsync();

                if (!divisions.Any())
                {
                    return (false,
                        "No divisions found.",
                        0,
                        new List<TimeTableEntry>());
                }

                // =========================
                // LOAD SUBJECT DATA
                // =========================
                var subjectSemesters = await _context.SubjectSemesters
                    .Include(x => x.Subject)
                    .Include(x => x.Semester)
                    .Where(x =>
                        x.AcademicYearId == academicYearId &&
                        x.IsActive &&
                        semesterIds.Contains(x.SemesterId))
                    .ToListAsync();

                if (!subjectSemesters.Any())
                {
                    return (false,
                        "No subjects configured.",
                        0,
                        new List<TimeTableEntry>());
                }

                var subjectSemesterIds = subjectSemesters
                    .Select(x => x.SubjectSemesterId)
                    .ToList();

                var lectureConfigs = await _context.SubjectLectureConfigs
                    .Where(x =>
                        x.AcademicYearId == academicYearId &&
                        x.IsActive &&
                        subjectSemesterIds.Contains(x.SubjectSemesterId))
                    .ToListAsync();

                if (!lectureConfigs.Any())
                {
                    return (false,
                        "No lecture configuration found.",
                        0,
                        new List<TimeTableEntry>());
                }

                var subjectFaculties = await _context.SubjectFaculties
                    .Include(x => x.Faculty)
                    .Where(x =>
                        x.AcademicYearId == academicYearId &&
                        x.IsActive)
                    .ToListAsync();

                if (!subjectFaculties.Any())
                {
                    return (false,
                        "No faculty mapping found.",
                        0,
                        new List<TimeTableEntry>());
                }

                var roomConfigs = await _context.SubjectRoomConfigs
                    .Include(x => x.Room)
                    .Where(x =>
                        x.IsActive &&
                        subjectSemesterIds.Contains(x.SubjectSemesterId))
                    .ToListAsync();

                var facultyAvailability = await _context.FacultyAvailabilities
                    .ToListAsync();

                var externalPermissions = await _context.ExternalFacultyPermissions
                    .Where(x => x.IsActive)
                    .ToListAsync();

                var roomAllocations = await _context.DivisionRoomAllocations
                    .Include(x => x.Room)
                    .Where(x => x.AcademicTermId == academicTermId)
                    .ToListAsync();

                var allRooms = await _context.Rooms
                    .Where(x => x.IsActive)
                    .ToListAsync();

                // =========================
                // CREATE BATCH
                // =========================
                var batch = new TimeTableBatch
                {
                    AcademicYearId = academicYearId,
                    AcademicTermId = academicTermId,
                    DepartmentId = config.DepartmentId,
                    CourseId = courseId,
                    ConfigId = config.ConfigId,
                    TemplateId = templateId,
                    VersionNo = 1,
                    Status = BatchStatusEnum.Draft,
                    IsActiveVersion = true,
                    CreatedByUserId = userId,
                    CreatedAt = DateTime.Now
                };

                _context.TimeTableBatches.Add(batch);
                await _context.SaveChangesAsync();

                foreach (var sem in semesters)
                {
                    _context.TimeTableBatchSemesters.Add(
                        new TimeTableBatchSemester
                        {
                            BatchId = batch.BatchId,
                            SemesterId = sem.SemesterId
                        });
                }

                await _context.SaveChangesAsync();

                // =========================
                // GENERATE
                // =========================
                var generatedEntries = await GenerateScheduleInternalAsync(
                    batch,
                    term,
                    config,
                    semesters,
                    divisions,
                    subjectSemesters,
                    lectureConfigs,
                    subjectFaculties,
                    roomConfigs,
                    facultyAvailability,
                    externalPermissions,
                    roomAllocations,
                    allRooms
                );

                if (generatedEntries == null || !generatedEntries.Any())
                {
                    await tx.RollbackAsync();

                    return (false,
                        "Timetable generation failed.",
                        0,
                        new List<TimeTableEntry>());
                }

                _context.TimeTableEntries.AddRange(generatedEntries);

                batch.Status = BatchStatusEnum.Generated;
                batch.GeneratedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return (true,
                    "Timetable generated successfully.",
                    batch.BatchId,
                    generatedEntries);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();

                return (false,
                    ex.Message,
                    0,
                    new List<TimeTableEntry>());
            }
        }
        //keep
        public async Task<List<Course>> GetCoursesForCoordinatorAsync(int userId)
        {
            return await _context.TTCoordinatorCourses
                .Include(x => x.Course)
                .Where(x => x.UserId == userId && x.IsActive)
                .Select(x => x.Course)
                .Where(x => x.IsActive)
                .ToListAsync();
        }
        //keep
        public async Task<List<AcademicTerm>> GetTermsByCourseAsync(
            int academicYearId,
            int courseId)
        {
            return await _context.AcademicTerms
                .Where(x =>
                    x.AcademicYearId == academicYearId &&
                    x.CourseId == courseId)
                .OrderBy(x => x.TermType)
                .ToListAsync();
        }
        //keep
        public async Task<List<TimeTableTemplate>> GetTemplatesAsync()
        {
            return await _context.TimeTableTemplates
                .Where(x => x.IsActive)
                .OrderBy(x => x.TemplateName)
                .ToListAsync();
        }

     

       

       
       
        //keep
        public async Task<List<AcademicYear>> GetAcademicYearsAsync()
        {
            return await _context.AcademicYears
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
        private async Task<List<Semester>> GetEligibleSemestersAsync(
            AcademicTerm term)
        {
            IQueryable<Semester> query = _context.Semesters
                .Where(x =>
                    x.CourseId == term.CourseId &&
                    x.IsActive);

            switch (term.TermType)
            {
                case TermTypeEnum.Winter:
                    query = query.Where(x => x.SemesterNo % 2 == 1);
                    break;

                case TermTypeEnum.Summer:
                    query = query.Where(x => x.SemesterNo % 2 == 0);
                    break;

                case TermTypeEnum.Annual:
                    break;
            }

            return await query
                .OrderBy(x => x.SemesterNo)
                .ToListAsync();
        }

        private List<byte> GetWorkingDays(int mask)
        {
            var days = new List<byte>();

            if ((mask & 1) == 1)
                days.Add(1); // Monday

            if ((mask & 2) == 2)
                days.Add(2); // Tuesday

            if ((mask & 4) == 4)
                days.Add(3); // Wednesday

            if ((mask & 8) == 8)
                days.Add(4); // Thursday

            if ((mask & 16) == 16)
                days.Add(5); // Friday

            if ((mask & 32) == 32)
                days.Add(6); // Saturday

            if ((mask & 64) == 64)
                days.Add(7); // Sunday

            // SAFETY FIX
            // if config is bad (only 1 day or empty),
            // fallback to Mon-Sat
            if (days.Count <= 1)
            {
                days = new List<byte>
        {
            1, 2, 3, 4, 5, 6
        };
            }

            return days;
        }

        private bool IsProjectSubject(
            SubjectSemester subjectSemester)
        {
            if (subjectSemester.Subject == null)
                return false;

            var name = subjectSemester.Subject.SubjectName;

            if (string.IsNullOrWhiteSpace(name))
                return false;

            return
                name.Contains("project",
                    StringComparison.OrdinalIgnoreCase)
                ||
                name.Contains("(pw)",
                    StringComparison.OrdinalIgnoreCase);
        }

        private bool IsBreakSlot(TimeSlot slot)
        {
            return slot.SlotType == SlotTypeEnum.Break;
        }

        private bool IsLectureSlot(TimeSlot slot)
        {
            return slot.SlotType == SlotTypeEnum.Lecture;
        }

        private string GetEntryDisplayName(
            SubjectSemester? subjectSemester)
        {
            if (subjectSemester == null)
                return "SELF STUDY";

            if (IsProjectSubject(subjectSemester))
                return "PROJECT";

            return subjectSemester.Subject.SubjectName;
        }
        private async Task<List<TimeTableEntry>> GenerateScheduleInternalAsync(
     TimeTableBatch batch,
     AcademicTerm term,
     ScheduleConfig config,
     List<Semester> semesters,
     List<Division> divisions,
     List<SubjectSemester> subjectSemesters,
     List<SubjectLectureConfig> lectureConfigs,
     List<SubjectFaculty> subjectFaculties,
     List<SubjectRoomConfig> roomConfigs,
     List<FacultyAvailability> facultyAvailability,
     List<ExternalFacultyPermission> externalPermissions,
     List<DivisionRoomAllocation> roomAllocations,
     List<Room> allRooms)
        {
            var result = new List<TimeTableEntry>();

            var practicalLoadPerDay = new Dictionary<byte, int>();
            

            var workingDays = GetWorkingDays(config.WorkingDaysMask);

            foreach (var day in workingDays)
            {
                practicalLoadPerDay[day] = 0;
            }

            //var lectureSlots = config.TimeSlots
            //    .Where(IsLectureSlot)
            //    .OrderBy(x => x.SlotNo)
            //    .ToList();
            var allSlots = config.TimeSlots
    .OrderBy(x => x.SlotNo)
    .ToList();

            var lectureSlots = allSlots
    .Where(x => x.SlotType == SlotTypeEnum.Lecture)
    .ToList();

            var breakSlots = allSlots
                .Where(x => x.SlotType == SlotTypeEnum.Break)
                .ToList();

            if (!lectureSlots.Any())
                throw new Exception("No lecture slots configured.");

            foreach (var division in divisions)
            {
                var semesterSubjects = subjectSemesters
                    .Where(x => x.SemesterId == division.SemesterId)
                    .ToList();

                if (!semesterSubjects.Any())
                    continue;

                var semesterLectureConfigs = lectureConfigs
                    .Where(x =>
                        semesterSubjects
                            .Select(s => s.SubjectSemesterId)
                            .Contains(x.SubjectSemesterId))
                    .ToList();

                var divisionFacultyMappings = subjectFaculties
                    .Where(x => x.DivisionId == division.DivisionId)
                    .ToList();

                // RANDOMIZE DAY ORDER
                var randomizedDays = workingDays
                    .OrderBy(x => Guid.NewGuid())
                    .ToList();

                // ================= PRACTICAL =================
                foreach (var subject in semesterSubjects)
                {
                    var lectureConfig = semesterLectureConfigs
                        .FirstOrDefault(x =>
                            x.SubjectSemesterId == subject.SubjectSemesterId);

                    if (lectureConfig == null)
                        continue;

                    if (lectureConfig.PracticalLecturesPerWeek <= 0)
                        continue;

                    var practicalFaculty = divisionFacultyMappings
                        .FirstOrDefault(x =>
                            x.SubjectSemesterId == subject.SubjectSemesterId &&
                            x.TeachingType == SubjectCategoryEnum.Practical);

                    if (practicalFaculty == null)
                        continue;

                    byte configuredBlockSize =
                        lectureConfig.PracticalBlockSize ?? 2;

                    byte remainingPractical =
                        lectureConfig.PracticalLecturesPerWeek;

                    while (remainingPractical > 0)
                    {
                        bool allocated = false;

                        byte currentBlockSize =
                            (byte)Math.Min(
                                configuredBlockSize,
                                remainingPractical);

                        foreach (var day in randomizedDays
                            .OrderBy(d => practicalLoadPerDay[d]))
                        {
                            if (HasSubjectOnDay(
                                result,
                                batch.BatchId,
                                division.DivisionId,
                                day,
                                subject.SubjectSemesterId))
                            {
                                continue;
                            }

                            for (int size = currentBlockSize; size >= 1; size--)
                            {
                                for (int i = 0; i <= lectureSlots.Count - size; i++)
                                {
                                    var candidateSlots = lectureSlots
                                        .Skip(i)
                                        .Take(size)
                                        .ToList();

                                    if (!AreConsecutiveSlots(candidateSlots))
                                        continue;

                                    if (!CanAssignPracticalBlock(
                                        result,
                                        batch.BatchId,
                                        division,
                                        day,
                                        candidateSlots,
                                        subject,
                                        practicalFaculty,
                                        roomConfigs,
                                        facultyAvailability,
                                        externalPermissions,
                                        roomAllocations,
                                        allRooms,
                                        out Room? selectedRoom))
                                    {
                                        continue;
                                    }

                                    var blockId = Guid.NewGuid();
                                    byte part = 1;



                                    foreach (var slot in candidateSlots)
                                    {

                                        result.Add(new TimeTableEntry
                                        {
                                            BatchId = batch.BatchId,
                                            SemesterId = division.SemesterId,
                                            DivisionId = division.DivisionId,
                                            DayOfWeek = day,
                                            TimeSlotId = slot.TimeSlotId,
                                            EntryType = EntryTypeEnum.Lecture,
                                            SubjectSemesterId = subject.SubjectSemesterId,
                                            FacultyId = practicalFaculty.FacultyId,
                                            RoomId = selectedRoom?.RoomId,
                                            BlockId = blockId,
                                            BlockPart = part++
                                        });
                                    }

                                    // ADD THIS
                                    practicalLoadPerDay[day] += size;

                                    // ❗ BREAK AFTER BLOCK (not inside loop)
                                    //var nextSlot = lectureSlots
                                    //    .FirstOrDefault(x => x.SlotNo == candidateSlots.Last().SlotNo + 1);


                                    remainingPractical =
                                        (byte)(remainingPractical - size);

                                    allocated = true;
                                    break;
                                }

                                if (allocated)
                                    break;
                            }

                            if (allocated)
                                break;
                        }

                        if (!allocated)
                            break;
                    }
                }

                // ================= THEORY =================
                foreach (var subject in semesterSubjects)
                {
                    var lectureConfig = semesterLectureConfigs
                        .FirstOrDefault(x => x.SubjectSemesterId == subject.SubjectSemesterId);

                    if (lectureConfig == null || lectureConfig.TheoryLecturesPerWeek <= 0)
                        continue;

                    var theoryFaculty = divisionFacultyMappings
                        .FirstOrDefault(x => x.SubjectSemesterId == subject.SubjectSemesterId &&
                                            x.TeachingType == SubjectCategoryEnum.Theory);

                    if (theoryFaculty == null)
                        continue;

                    byte remainingTheory = lectureConfig.TheoryLecturesPerWeek;

                    while (remainingTheory > 0)
                    {
                        bool allocated = false;

                        foreach (var day in randomizedDays)
                        {
                            // ✅ FIX: Allow theory on the same day as practicals by specifying category validation
                            if (HasSubjectOnDay(result, batch.BatchId, division.DivisionId, day, subject.SubjectSemesterId, (int)SubjectCategoryEnum.Theory))
                            {
                                continue;
                            }

                            foreach (var slot in lectureSlots)
                            {
                                if (slot.SlotType == SlotTypeEnum.Break)
                                    continue;

                                if (!CanAssignTheorySlot(
                                    result,
                                    batch.BatchId,
                                    division,
                                    day,
                                    slot,
                                    subject,
                                    theoryFaculty,
                                    roomConfigs,
                                    facultyAvailability,
                                    externalPermissions,
                                    roomAllocations,
                                    allRooms,
                                    out Room? selectedRoom))
                                {
                                    continue;
                                }

                                result.Add(new TimeTableEntry
                                {
                                    BatchId = batch.BatchId,
                                    SemesterId = division.SemesterId,
                                    DivisionId = division.DivisionId,
                                    DayOfWeek = day,
                                    TimeSlotId = slot.TimeSlotId,
                                    EntryType = EntryTypeEnum.Lecture,
                                    SubjectSemesterId = subject.SubjectSemesterId,
                                    FacultyId = theoryFaculty.FacultyId,
                                    RoomId = selectedRoom?.RoomId
                                });

                                remainingTheory--;
                                allocated = true;
                                break;
                            }

                            if (allocated)
                                break;
                        }

                        if (!allocated)
                            break; // Break loop if slot configuration can't find an open room
                    }
                }
            }

            FillProjectAndSelfStudy(
                result,
                batch,
                divisions,
                subjectSemesters,
                config);

            // BREAK SLOT AUTO-FILL (USE EXISTING allSlots VARIABLE)
            //var breakSlots = allSlots
            //    .Where(x => x.SlotType == SlotTypeEnum.Break)
            //    .ToList();

            foreach (var division in divisions)
            {
                foreach (var day in workingDays)
                {
                    foreach (var slot in breakSlots)
                    {
                        bool exists = result.Any(x =>
                            x.BatchId == batch.BatchId &&
                            x.DivisionId == division.DivisionId &&
                            x.DayOfWeek == day &&
                            x.TimeSlotId == slot.TimeSlotId);

                        if (!exists)
                        {
                            result.Add(new TimeTableEntry
                            {
                                BatchId = batch.BatchId,
                                SemesterId = division.SemesterId,
                                DivisionId = division.DivisionId,
                                DayOfWeek = day,
                                TimeSlotId = slot.TimeSlotId,
                                EntryType = EntryTypeEnum.Break
                            });
                        }
                    }
                }
            }

            return result;
        }
        private bool HasSubjectOnDay(
            List<TimeTableEntry> entries,
            int batchId,
            int divisionId,
            byte day,
            int subjectSemesterId,
            int? teachingType = null) // Added teaching type filter
        {
            return entries.Any(x =>
                x.BatchId == batchId &&
                x.DivisionId == divisionId &&
                x.DayOfWeek == day &&
                x.SubjectSemesterId == subjectSemesterId &&
                (teachingType == null || _context.SubjectFaculties.Any(sf => sf.FacultyId == x.FacultyId && (int)sf.TeachingType == teachingType)));
        }
        private bool AreConsecutiveSlots(List<TimeSlot> slots)
        {
            if (!slots.Any())
                return false;

            var ordered = slots
                .OrderBy(x => x.StartTime)
                .ToList();

            for (int i = 1; i < ordered.Count; i++)
            {
                if (ordered[i].StartTime != ordered[i - 1].EndTime)
                    return false;
            }

            return true;
        }


        private bool CanAssignPracticalBlock(
         List<TimeTableEntry> entries,
         int batchId,
         Division division,
         byte day,
         List<TimeSlot> slots,
         SubjectSemester subject,
         SubjectFaculty faculty,
         List<SubjectRoomConfig> roomConfigs,
         List<FacultyAvailability> facultyAvailability,
         List<ExternalFacultyPermission> externalPermissions,
         List<DivisionRoomAllocation> roomAllocations,
         List<Room> allRooms,
         out Room? selectedRoom)
        {
            selectedRoom = null;

            foreach (var slot in slots)
            {
                if (entries.Any(x =>
                    x.BatchId == batchId &&
                    x.DivisionId == division.DivisionId &&
                    x.DayOfWeek == day &&
                    x.TimeSlotId == slot.TimeSlotId))
                {
                    return false;
                }

                if (IsFacultyBusy(
                    entries,
                    batchId,
                    faculty.FacultyId,
                    day,
                    slot.TimeSlotId))
                {
                    return false;
                }
            }

            selectedRoom = FindBestRoom(
                entries,
                batchId,
                subject,
                division,
                day,
                slots.Select(x => x.TimeSlotId).ToList(),
                roomConfigs,
                roomAllocations,
                allRooms,
                true);

            return selectedRoom != null;
        }

        private bool CanAssignTheorySlot(
            List<TimeTableEntry> entries,
            int batchId,
            Division division,
            byte day,
            TimeSlot slot,
            SubjectSemester subject,
            SubjectFaculty faculty,
            List<SubjectRoomConfig> roomConfigs,
            List<FacultyAvailability> facultyAvailability,
            List<ExternalFacultyPermission> externalPermissions,
            List<DivisionRoomAllocation> roomAllocations,
            List<Room> allRooms,
            out Room? selectedRoom)
        {
            selectedRoom = null;

            if (entries.Any(x =>
                x.BatchId == batchId &&
                x.DivisionId == division.DivisionId &&
                x.DayOfWeek == day &&
                x.TimeSlotId == slot.TimeSlotId))
            {
                return false;
            }

            if (IsFacultyBusy(
                entries,
                batchId,
                faculty.FacultyId,
                day,
                slot.TimeSlotId))
            {
                return false;
            }

            if (!IsFacultyAvailable(
                faculty.Faculty,
                facultyAvailability,
                externalPermissions,
                division,
                slot,
                day))
            {
                return false;
            }

            selectedRoom = FindBestRoom(
                entries,
                batchId,
                subject,
                division,
                day,
                new List<int> { slot.TimeSlotId },
                roomConfigs,
                roomAllocations,
                allRooms,
                false);

            return selectedRoom != null;
        }

        private bool IsFacultyBusy(
            List<TimeTableEntry> entries,
            int batchId,
            int facultyId,
            byte day,
            int slotId)
        {
            return entries.Any(x =>
                x.BatchId == batchId &&
                x.FacultyId == facultyId &&
                x.DayOfWeek == day &&
                x.TimeSlotId == slotId);
        }

        private bool IsFacultyAvailable(
            Faculty faculty,
            List<FacultyAvailability> availability,
            List<ExternalFacultyPermission> externalPermissions,
            Division division,
            TimeSlot slot,
            byte day)
        {
            if (faculty.IsExternal)
            {
                var allowed = externalPermissions.Any(x =>
                    x.FacultyId == faculty.FacultyId &&
                    x.DepartmentId == division.Course.Department.DepartmentId &&
                    x.IsActive);

                if (!allowed)
                    return false;
            }

            var records = availability
                .Where(x =>
                    x.FacultyId == faculty.FacultyId &&
                    x.DayOfWeek == day)
                .ToList();

            if (!records.Any())
                return true;

            foreach (var item in records)
            {
                var inside =
                    slot.StartTime >= item.StartTime &&
                    slot.EndTime <= item.EndTime;

                if (inside)
                    return item.IsAvailable;
            }

            return true;
        }

        private Room? FindBestRoom(
            List<TimeTableEntry> entries,
            int batchId,
            SubjectSemester subject,
            Division division,
            byte day,
            List<int> slotIds,
            List<SubjectRoomConfig> roomConfigs,
            List<DivisionRoomAllocation> roomAllocations,
            List<Room> allRooms,
            bool practical)
        {
            var roomConfig = roomConfigs
                .FirstOrDefault(x =>
                    x.SubjectSemesterId == subject.SubjectSemesterId);

            if (!practical)
            {
                var fixedRoom = roomAllocations
                    .FirstOrDefault(x =>
                        x.DivisionId == division.DivisionId);

                if (fixedRoom != null &&
                    IsRoomFree(entries, batchId, fixedRoom.RoomId, day, slotIds))
                {
                    return fixedRoom.Room;
                }
            }

            if (roomConfig?.RoomId != null)
            {
                var directRoom = allRooms
                    .FirstOrDefault(x => x.RoomId == roomConfig.RoomId);

                if (directRoom != null &&
                    IsRoomFree(entries, batchId, directRoom.RoomId, day, slotIds))
                {
                    return directRoom;
                }
            }

            if (roomConfig?.PreferredRoomType != null)
            {
                var preferred = allRooms
                .Where(x =>
                    x.RoomType == roomConfig.PreferredRoomType &&
                    x.Capacity >= division.StudentStrength)
                .ToList();

                foreach (var room in preferred)
                {
                    if (IsRoomFree(entries, batchId, room.RoomId, day, slotIds))
                        return room;
                }
            }

            var fallback = practical
     ? allRooms
         .Where(x =>
             x.RoomType == RoomTypeEnum.Lab &&
             x.Capacity >= division.StudentStrength)
         .ToList()
     : allRooms
         .Where(x =>
             x.RoomType == RoomTypeEnum.Classroom &&
             x.Capacity >= division.StudentStrength)
         .ToList();
            foreach (var room in fallback)
            {
                if (IsRoomFree(entries, batchId, room.RoomId, day, slotIds))
                    return room;
            }

            return null;
        }

        private bool IsRoomFree(
            List<TimeTableEntry> entries,
            int batchId,
            int roomId,
            byte day,
            List<int> slotIds)
        {
            return !entries.Any(x =>
                x.BatchId == batchId &&
                x.RoomId == roomId &&
                x.DayOfWeek == day &&
                slotIds.Contains(x.TimeSlotId));
        }

        private void FillProjectAndSelfStudy(
            List<TimeTableEntry> entries,
            TimeTableBatch batch,
            List<Division> divisions,
            List<SubjectSemester> subjectSemesters,
            ScheduleConfig config)
        {
            var workingDays = GetWorkingDays(config.WorkingDaysMask);

            var lectureSlots = config.TimeSlots
                .Where(x => x.SlotType == SlotTypeEnum.Lecture)
                .OrderBy(x => x.SlotNo)
                .ToList();

            foreach (var division in divisions)
            {
                var projectSubject = subjectSemesters
                    .Where(x => x.SemesterId == division.SemesterId)
                    .FirstOrDefault(IsProjectSubject);

                foreach (var day in workingDays)
                {
                    foreach (var slot in lectureSlots)
                    {
                        var exists = entries.Any(x =>
                            x.BatchId == batch.BatchId &&
                            x.DivisionId == division.DivisionId &&
                            x.DayOfWeek == day &&
                            x.TimeSlotId == slot.TimeSlotId);

                        if (exists)
                            continue;

                        entries.Add(new TimeTableEntry
                        {
                            BatchId = batch.BatchId,
                            SemesterId = division.SemesterId,
                            DivisionId = division.DivisionId,
                            DayOfWeek = day,
                            TimeSlotId = slot.TimeSlotId,
                            EntryType = EntryTypeEnum.Free,
                            SubjectSemesterId = projectSubject?.SubjectSemesterId
                        });
                    }
                }
            }
        }//keep
        public async Task<(bool Success, string Message)> SwapEntriesAsync(
            int entryId1,
            int entryId2,
            int userId)
        {
            var entry1 = await _context.TimeTableEntries
            .Include(x => x.TimeTableBatch)
                .ThenInclude(x => x.Course)
            .Include(x => x.TimeSlot)
                .FirstOrDefaultAsync(x => x.EntryId == entryId1);

            var entry2 = await _context.TimeTableEntries
                .Include(x => x.TimeTableBatch)
                    .ThenInclude(x => x.Course)
                .Include(x => x.TimeSlot)
                .FirstOrDefaultAsync(x => x.EntryId == entryId2);

            if (entry1 == null || entry2 == null)
            {
                return (false, "Entry not found.");
            }

            if (entry1.BatchId != entry2.BatchId)
            {
                return (false, "Entries must belong to same batch.");
            }

            var allowed = await _context.TTCoordinatorCourses
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.CourseId == entry1.TimeTableBatch.CourseId &&
                    x.IsActive);

            if (!allowed)
            {
                return (false, "Unauthorized action.");
            }

            if (entry1.EntryType == EntryTypeEnum.Free ||
                entry2.EntryType == EntryTypeEnum.Free)
            {
                return (false, "Free entries cannot be swapped.");
            }

            var tempDay = entry1.DayOfWeek;
            var tempSlot = entry1.TimeSlotId;
            var tempRoom = entry1.RoomId;

            entry1.DayOfWeek = entry2.DayOfWeek;
            entry1.TimeSlotId = entry2.TimeSlotId;
            entry1.RoomId = entry2.RoomId;

            entry2.DayOfWeek = tempDay;
            entry2.TimeSlotId = tempSlot;
            entry2.RoomId = tempRoom;

            try
            {

                await _context.SaveChangesAsync();
                return (true, "Entries swapped successfully.");
            }
            catch
            {
                return (false, "Swap failed due to conflict.");
            }
        }
    }
}
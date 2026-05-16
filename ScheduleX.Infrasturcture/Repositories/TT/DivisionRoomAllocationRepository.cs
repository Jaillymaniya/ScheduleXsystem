

//using Microsoft.EntityFrameworkCore;
//using ScheduleX.Core.Entities;
//using ScheduleX.Core.Interfaces.TTCoordinator;
//using ScheduleX.Infrastructure.Data;

//namespace ScheduleX.Infrastructure.Repositories.TT
//{
//    public class DivisionRoomAllocationRepository
//        : IDivisionRoomAllocationRepository
//    {
//        private readonly AppDbContext _context;

//        public DivisionRoomAllocationRepository(
//            AppDbContext context)
//        {
//            _context = context;
//        }

//        //public async Task<List<DivisionRoomAllocation>>
//        //GetAllAsync(int ttCoordinatorId)
//        //{
//        //    return await _context
//        //        .DivisionRoomAllocations

//        //        .Include(x => x.Semester)

//        //        .Include(x => x.Division)

//        //        .Include(x => x.Room)
//        //            .ThenInclude(x => x.Department)

//        //        .Where(x =>
//        //            x.Division.TTCoordinatorId
//        //            == ttCoordinatorId)

//        //        .OrderByDescending(x =>
//        //            x.AllocationId)

//        //        .ToListAsync();
//        //}

//        public async Task<List<DivisionRoomAllocation>>
//GetAllAsync(
//    int ttCoordinatorId,
//    int academicYearId,
//    int courseId,
//    int academicTermId)
//        {
//            var term = await _context.AcademicTerms
//                .FirstOrDefaultAsync(x =>
//                    x.AcademicTermId == academicTermId);

//            if (term == null)
//                return new();

//            return await _context
//                .DivisionRoomAllocations

//                .Include(x => x.Semester)

//                .Include(x => x.Division)

//                .Include(x => x.Room)
//                    .ThenInclude(x => x.Department)

//                .Where(x =>

//                    x.Division.TTCoordinatorId
//                    == ttCoordinatorId

//                    &&

//                    x.Division.AcademicYearId
//                    == academicYearId

//                    &&

//                    x.Division.CourseId
//                    == courseId

//                    &&

//                    x.Semester.SemesterPattern
//                    == term.SemesterPattern)

//                .OrderByDescending(x =>
//                    x.AllocationId)

//                .ToListAsync();
//        }



//        public async Task<List<Semester>>
//        GetSemestersAsync()
//        {
//            return await _context.Semesters

//                .OrderBy(x => x.SemesterNo)

//                .ToListAsync();
//        }



//        public async Task<List<Division>>
//        GetDivisionsAsync(int ttCoordinatorId)
//        {
//            return await _context.Divisions

//                .Include(x => x.Semester)

//                .Where(x =>
//                    x.TTCoordinatorId
//                    == ttCoordinatorId)

//                .OrderBy(x => x.DivisionName)

//                .ToListAsync();
//        }



//        public async Task<List<Room>>
//        GetRoomsAsync(int ttCoordinatorId)
//        {
//            return await _context.Rooms

//                .Include(x => x.Department)

//                .Where(x =>
//                    x.TTCoordinatorId
//                    == ttCoordinatorId)

//                .OrderBy(x => x.RoomName)

//                .ToListAsync();
//        }



//        public async Task<DivisionRoomAllocation?>
//        GetByIdAsync(int id)
//        {
//            return await _context
//                .DivisionRoomAllocations

//                .Include(x => x.Semester)

//                .Include(x => x.Division)

//                .Include(x => x.Room)
//                    .ThenInclude(x => x.Department)

//                .FirstOrDefaultAsync(x =>
//                    x.AllocationId == id);
//        }



//        public async Task<bool>
//        ExistsAsync(
//            int divisionId,
//            int roomId,
//            int? excludeId = null)
//        {
//            return await _context
//                .DivisionRoomAllocations

//                .AnyAsync(x =>

//                    x.DivisionId == divisionId

//                    &&

//                    x.RoomId == roomId

//                    &&

//                    (!excludeId.HasValue
//                    || x.AllocationId
//                    != excludeId.Value));
//        }



//        //public async Task<bool>
//        //RoomAlreadyAllocatedAsync(
//        //    int roomId,
//        //    int allocationId = 0)
//        //{
//        //    return await _context
//        //        .DivisionRoomAllocations

//        //        .AnyAsync(x =>

//        //            x.RoomId == roomId

//        //            &&

//        //            x.AllocationId != allocationId);
//        //}


//        //        public async Task<bool>
//        //RoomAlreadyAllocatedAsync(
//        //    int semesterId,
//        //    int roomId,
//        //    int allocationId = 0)
//        //        {
//        //            return await _context
//        //                .DivisionRoomAllocations

//        //                .AnyAsync(x =>

//        //                    x.SemesterId == semesterId

//        //                    &&

//        //                    x.RoomId == roomId

//        //                    &&

//        //                    x.AllocationId != allocationId);
//        //        }


//        public async Task<bool>
//RoomAlreadyAllocatedAsync(
//    int semesterId,
//    int roomId,
//    int allocationId = 0)
//        {
//            return await _context
//                .DivisionRoomAllocations

//                .AnyAsync(x =>

//                    x.SemesterId == semesterId

//                    &&

//                    x.RoomId == roomId

//                    &&

//                    x.AllocationId != allocationId);
//        }

//        public async Task AddAsync(
//            DivisionRoomAllocation allocation)
//        {
//            await _context
//                .DivisionRoomAllocations
//                .AddAsync(allocation);
//        }



//        public async Task UpdateAsync(
//            DivisionRoomAllocation allocation)
//        {
//            _context
//                .DivisionRoomAllocations
//                .Update(allocation);

//            await Task.CompletedTask;
//        }



//        public async Task DeleteAsync(int id)
//        {
//            var existing =
//                await _context
//                .DivisionRoomAllocations
//                .FindAsync(id);

//            if (existing != null)
//            {
//                _context
//                    .DivisionRoomAllocations
//                    .Remove(existing);
//            }
//        }

//        public async Task<bool>
//DivisionAlreadyAllocatedAsync(
//    int divisionId,
//    int allocationId = 0)
//        {
//            return await _context
//                .DivisionRoomAllocations

//                .AnyAsync(x =>

//                    x.DivisionId == divisionId

//                    &&

//                    x.AllocationId != allocationId);
//        }


//        public async Task SaveAsync()
//        {
//            await _context.SaveChangesAsync();
//        }

//        //public async Task<List<Semester>> GetSemestersByCourseAsync(int courseId)
//        //{
//        //    return await _context.Semesters
//        //        .Where(x => x.CourseId == courseId)
//        //        .OrderBy(x => x.SemesterNo)
//        //        .ToListAsync();
//        //}



//        public async Task<List<Semester>>
//GetSemestersByCourseAsync(
//    int courseId,
//    int academicTermId)
//        {
//            return await _context.Semesters
//                .Where(x => x.CourseId == courseId)
//                .OrderBy(x => x.SemesterNo)

//                .ToListAsync();
//        }


//        public async Task<DivisionRoomAllocation?>
//GetRoomAllocationAsync(int roomId)
//        {
//            return await _context
//                .DivisionRoomAllocations

//                .Include(x => x.Semester)

//                .FirstOrDefaultAsync(x =>
//                    x.RoomId == roomId);
//        }
//    }
//}





//using Microsoft.EntityFrameworkCore;
//using ScheduleX.Core.Entities;
//using ScheduleX.Core.Interfaces.TTCoordinator;
//using ScheduleX.Infrastructure.Data;

//namespace ScheduleX.Infrastructure.Repositories.TT
//{
//    public class DivisionRoomAllocationRepository
//        : IDivisionRoomAllocationRepository
//    {
//        private readonly AppDbContext _context;

//        public DivisionRoomAllocationRepository(
//            AppDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<List<DivisionRoomAllocation>>
//        GetAllAsync(int ttCoordinatorId)
//        {
//            return await _context
//                .DivisionRoomAllocations

//                .Include(x => x.Semester)

//                .Include(x => x.Division)

//                .Include(x => x.Room)

//                .Where(x =>
//                    x.Division.TTCoordinatorId
//                    == ttCoordinatorId)

//                .OrderByDescending(x =>
//                    x.AllocationId)

//                .ToListAsync();
//        }

//        public async Task<List<Semester>>
//        GetSemestersAsync()
//        {
//            return await _context.Semesters
//                .OrderBy(x => x.SemesterNo)
//                .ToListAsync();
//        }

//        public async Task<List<Division>>
//        GetDivisionsAsync(int ttCoordinatorId)
//        {
//            return await _context.Divisions

//                .Where(x =>
//                    x.TTCoordinatorId
//                    == ttCoordinatorId)

//                .OrderBy(x => x.DivisionName)

//                .ToListAsync();
//        }

//        public async Task<List<Room>>
//        GetRoomsAsync(int ttCoordinatorId)
//        {
//            return await _context.Rooms

//                .Where(x =>
//                    x.TTCoordinatorId
//                    == ttCoordinatorId)

//                .OrderBy(x => x.RoomName)

//                .ToListAsync();
//        }

//        public async Task<DivisionRoomAllocation?>
//        GetByIdAsync(int id)
//        {
//            return await _context
//                .DivisionRoomAllocations
//                .FirstOrDefaultAsync(x =>
//                    x.AllocationId == id);
//        }

//        public async Task<bool>
//        ExistsAsync(
//            int divisionId,
//            int roomId,
//            int? excludeId = null)
//        {
//            return await _context
//                .DivisionRoomAllocations
//                .AnyAsync(x =>

//                    x.DivisionId == divisionId

//                    &&

//                    x.RoomId == roomId

//                    &&

//                    (!excludeId.HasValue
//                    || x.AllocationId
//                    != excludeId.Value));
//        }

//        public async Task AddAsync(
//            DivisionRoomAllocation allocation)
//        {
//            await _context
//                .DivisionRoomAllocations
//                .AddAsync(allocation);
//        }

//        public async Task UpdateAsync(
//            DivisionRoomAllocation allocation)
//        {
//            _context
//                .DivisionRoomAllocations
//                .Update(allocation);

//            await Task.CompletedTask;
//        }

//        public async Task DeleteAsync(int id)
//        {
//            var existing =
//                await _context
//                .DivisionRoomAllocations
//                .FindAsync(id);

//            if (existing != null)
//            {
//                _context
//                    .DivisionRoomAllocations
//                    .Remove(existing);
//            }
//        }

//        public async Task SaveAsync()
//        {
//            await _context.SaveChangesAsync();
//        }
//        public async Task<bool>
//RoomAlreadyAllocatedAsync(
//    int roomId,
//    int allocationId = 0)
//        {
//            return await _context.DivisionRoomAllocations
//                .AnyAsync(x =>
//                    x.RoomId == roomId &&
//                    x.AllocationId != allocationId);
//        }
//    }
//}



using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Infrastructure.Repositories.TT
{
    public class DivisionRoomAllocationRepository
        : IDivisionRoomAllocationRepository
    {
        private readonly AppDbContext _context;

        public DivisionRoomAllocationRepository(
            AppDbContext context)
        {
            _context = context;
        }

        //public async Task<List<DivisionRoomAllocation>>
        //GetAllAsync(int ttCoordinatorId)
        //{
        //    return await _context
        //        .DivisionRoomAllocations

        //        .Include(x => x.Semester)

        //        .Include(x => x.Division)

        //        .Include(x => x.Room)
        //            .ThenInclude(x => x.Department)

        //        .Where(x =>
        //            x.Division.TTCoordinatorId
        //            == ttCoordinatorId)

        //        .OrderByDescending(x =>
        //            x.AllocationId)

        //        .ToListAsync();
        //}

        public async Task<List<DivisionRoomAllocation>>
GetAllAsync(
    int ttCoordinatorId,
    int academicYearId,
    int courseId,
    int academicTermId)
        {
            var term = await _context.AcademicTerms
                .FirstOrDefaultAsync(x =>
                    x.AcademicTermId == academicTermId);

            if (term == null)
                return new();

            return await _context
                .DivisionRoomAllocations

                .Include(x => x.Semester)

                .Include(x => x.Division)

                .Include(x => x.Room)
                    .ThenInclude(x => x.Department)

                .Where(x =>

                    x.Division.TTCoordinatorId
                    == ttCoordinatorId

                    &&

                    x.Division.AcademicYearId
                    == academicYearId

                    &&

                    x.Division.CourseId
                    == courseId

                    &&

                    x.Semester.SemesterPattern
                    == term.SemesterPattern)

                .OrderByDescending(x =>
                    x.AllocationId)

                .ToListAsync();
        }



        public async Task<List<Semester>>
        GetSemestersAsync()
        {
            return await _context.Semesters

                .OrderBy(x => x.SemesterNo)

                .ToListAsync();
        }



        public async Task<List<Division>>
        GetDivisionsAsync(int ttCoordinatorId)
        {
            return await _context.Divisions

                .Include(x => x.Semester)

                .Where(x =>
                    x.TTCoordinatorId
                    == ttCoordinatorId)

                .OrderBy(x => x.DivisionName)

                .ToListAsync();
        }



        public async Task<List<Room>>
        GetRoomsAsync(int ttCoordinatorId)
        {
            return await _context.Rooms

                .Include(x => x.Department)

                .Where(x =>
                    x.TTCoordinatorId
                    == ttCoordinatorId)

                .OrderBy(x => x.RoomName)

                .ToListAsync();
        }



        public async Task<DivisionRoomAllocation?>
        GetByIdAsync(int id)
        {
            return await _context
                .DivisionRoomAllocations

                .Include(x => x.Semester)

                .Include(x => x.Division)

                .Include(x => x.Room)
                    .ThenInclude(x => x.Department)

                .FirstOrDefaultAsync(x =>
                    x.AllocationId == id);
        }



        public async Task<bool>
        ExistsAsync(
            int divisionId,
            int roomId,
            int? excludeId = null)
        {
            return await _context
                .DivisionRoomAllocations

                .AnyAsync(x =>

                    x.DivisionId == divisionId

                    &&

                    x.RoomId == roomId

                    &&

                    (!excludeId.HasValue
                    || x.AllocationId
                    != excludeId.Value));
        }



        //public async Task<bool>
        //RoomAlreadyAllocatedAsync(
        //    int roomId,
        //    int allocationId = 0)
        //{
        //    return await _context
        //        .DivisionRoomAllocations

        //        .AnyAsync(x =>

        //            x.RoomId == roomId

        //            &&

        //            x.AllocationId != allocationId);
        //}


        //        public async Task<bool>
        //RoomAlreadyAllocatedAsync(
        //    int semesterId,
        //    int roomId,
        //    int allocationId = 0)
        //        {
        //            return await _context
        //                .DivisionRoomAllocations

        //                .AnyAsync(x =>

        //                    x.SemesterId == semesterId

        //                    &&

        //                    x.RoomId == roomId

        //                    &&

        //                    x.AllocationId != allocationId);
        //        }


        public async Task<bool>
RoomAlreadyAllocatedAsync(
    int semesterId,
    int roomId,
    int allocationId = 0)
        {
            return await _context
                .DivisionRoomAllocations

                .AnyAsync(x =>

                    x.SemesterId == semesterId

                    &&

                    x.RoomId == roomId

                    &&

                    x.AllocationId != allocationId);
        }

        public async Task AddAsync(
            DivisionRoomAllocation allocation)
        {
            await _context
                .DivisionRoomAllocations
                .AddAsync(allocation);
        }



        public async Task UpdateAsync(
            DivisionRoomAllocation allocation)
        {
            _context
                .DivisionRoomAllocations
                .Update(allocation);

            await Task.CompletedTask;
        }



        public async Task DeleteAsync(int id)
        {
            var existing =
                await _context
                .DivisionRoomAllocations
                .FindAsync(id);

            if (existing != null)
            {
                _context
                    .DivisionRoomAllocations
                    .Remove(existing);
            }
        }

        public async Task<bool>
DivisionAlreadyAllocatedAsync(
    int divisionId,
    int allocationId = 0)
        {
            return await _context
                .DivisionRoomAllocations

                .AnyAsync(x =>

                    x.DivisionId == divisionId

                    &&

                    x.AllocationId != allocationId);
        }


        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        //public async Task<List<Semester>> GetSemestersByCourseAsync(int courseId)
        //{
        //    return await _context.Semesters
        //        .Where(x => x.CourseId == courseId)
        //        .OrderBy(x => x.SemesterNo)
        //        .ToListAsync();
        //}



        public async Task<List<Semester>>
GetSemestersByCourseAsync(
    int courseId,
    int academicTermId)
        {
            var term = await _context.AcademicTerms
                .FirstOrDefaultAsync(x =>
                    x.AcademicTermId == academicTermId);

            if (term == null)
                return new();

            return await _context.Semesters

                .Where(x =>

                    x.CourseId == courseId

                    &&

                    x.SemesterPattern
                    == term.SemesterPattern)

                .OrderBy(x => x.SemesterNo)

                .ToListAsync();
        }


        public async Task<DivisionRoomAllocation?>
GetRoomAllocationAsync(int roomId)
        {
            return await _context
                .DivisionRoomAllocations

                .Include(x => x.Semester)

                .FirstOrDefaultAsync(x =>
                    x.RoomId == roomId);
        }
    }
}

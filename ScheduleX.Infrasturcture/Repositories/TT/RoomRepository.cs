using Microsoft.EntityFrameworkCore;
using ScheduleX.Core.Entities;
using ScheduleX.Core.Interfaces.TTCoordinator;
using ScheduleX.Infrastructure.Data;

namespace ScheduleX.Infrastructure.Repositories.TT
{
    public class RoomRepository : IRoomRepository
    {
        private readonly AppDbContext _context;

        public RoomRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Room>> GetAllAsync()
        {
            return await _context.Rooms

                .Include(x => x.Department)

                .OrderBy(x => x.RoomName)

                .ToListAsync();
        }

        public async Task<List<Department>> GetDepartmentsAsync()
        {
            return await _context.Departments

                .Where(x => x.IsActive)

                .OrderBy(x => x.DepartmentName)

                .ToListAsync();
        }

        public async Task<Room?> GetByIdAsync(int id)
        {
            return await _context.Rooms
                .FirstOrDefaultAsync(x => x.RoomId == id);
        }

        public async Task<bool> ExistsAsync(
            string roomName,
            int departmentId,
            int? excludeId = null)
        {
            return await _context.Rooms.AnyAsync(x =>

                x.RoomName.ToLower() ==
                roomName.ToLower()

                &&

                x.DepartmentId == departmentId

                &&

                (!excludeId.HasValue ||
                 x.RoomId != excludeId.Value)
            );
        }

        public async Task AddAsync(Room room)
        {
            await _context.Rooms.AddAsync(room);
        }

        public async Task UpdateAsync(Room room)
        {
            var existingRoom = await _context.Rooms
                .FirstOrDefaultAsync(x => x.RoomId == room.RoomId);

            if (existingRoom != null)
            {
                existingRoom.DepartmentId =
                    room.DepartmentId;

                existingRoom.RoomName =
                    room.RoomName;

                existingRoom.RoomType =
                    room.RoomType;

                existingRoom.Capacity =
                    room.Capacity;
            }
        }

        public async Task DeleteAsync(int id)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(x => x.RoomId == id);

            if (room != null)
            {
                _context.Rooms.Remove(room);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
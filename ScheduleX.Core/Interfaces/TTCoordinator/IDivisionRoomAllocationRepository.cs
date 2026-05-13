using ScheduleX.Core.Entities;

namespace ScheduleX.Core.Interfaces.TTCoordinator
{
    public interface IDivisionRoomAllocationRepository
    {
        Task<List<DivisionRoomAllocation>>
        GetAllAsync(int ttCoordinatorId);

        Task<List<Semester>>
        GetSemestersAsync();

        Task<List<Division>>
        GetDivisionsAsync(int ttCoordinatorId);

        Task<List<Room>>
        GetRoomsAsync(int ttCoordinatorId);

        Task<DivisionRoomAllocation?>
        GetByIdAsync(int id);

        Task<bool>
        ExistsAsync(
            int divisionId,
            int roomId,
            int? excludeId = null);

        Task AddAsync(
            DivisionRoomAllocation allocation);

        Task UpdateAsync(
            DivisionRoomAllocation allocation);

        Task DeleteAsync(int id);

        Task SaveAsync();

        //    Task<bool> RoomAlreadyAllocatedAsync(
        //int roomId,
        //int allocationId = 0);


        Task<bool> RoomAlreadyAllocatedAsync(
    int semesterId,
    int roomId,
    int allocationId = 0);
        Task<bool> DivisionAlreadyAllocatedAsync(
    int divisionId,
    int allocationId = 0);
    }
}
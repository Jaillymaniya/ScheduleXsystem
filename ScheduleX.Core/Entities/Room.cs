//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ScheduleX.Core.Entities
//{

//    public enum RoomTypeEnum : byte
//    {
//        Classroom = 1,
//        Lab = 2
//    }

//    public class Room
//    {
//        [Key]
//        public int RoomId { get; set; }

//        [Required]
//        public int DepartmentId { get; set; }

//        [ForeignKey(nameof(DepartmentId))]
//        public Department Department { get; set; } = null!;

//        [Required, MaxLength(50)]
//        public string RoomName { get; set; } = null!; // UNIQUE

//        [Required]
//        public RoomTypeEnum RoomType { get; set; }

//        [Required]
//        public int Capacity { get; set; }
//         // 🔥 NEW
//        public int? TTCoordinatorId { get; set; }

//        [ForeignKey(nameof(TTCoordinatorId))]
//        public User? TTCoordinator { get; set; }

//        public bool IsActive { get; set; } = true;

//        public DateTime CreatedAt { get; set; } = DateTime.Now;

//        // Nav
//        public ICollection<DivisionRoomAllocation> DivisionRoomAllocations { get; set; } = new List<DivisionRoomAllocation>();
//        public ICollection<TimeTableEntry> TimeTableEntries { get; set; } = new List<TimeTableEntry>();
//    }
//}




//using Microsoft.EntityFrameworkCore;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace ScheduleX.Core.Entities
//{
//    public enum RoomTypeEnum : byte
//    {
//        Classroom = 1,
//        Lab = 2
//    }


//    [Index(
//        nameof(DepartmentId),
//        nameof(RoomName),
//        IsUnique = true)]

//    public class Room
//    {
//        [Key]
//        public int RoomId { get; set; }

//        [Required]
//        public int DepartmentId { get; set; }

//        [ForeignKey(nameof(DepartmentId))]
//        public Department Department { get; set; } = null!;

//        [Required]
//        [MaxLength(50)]
//        public string RoomName { get; set; } = null!;

//        [Required]
//        public RoomTypeEnum RoomType { get; set; }

//        [Required]
//        public int Capacity { get; set; }

//        // TT OWNER

//        public int? TTCoordinatorId { get; set; }

//        [ForeignKey(nameof(TTCoordinatorId))]
//        public User? TTCoordinator { get; set; }

//        public bool IsActive { get; set; } = true;

//        public DateTime CreatedAt { get; set; }
//            = DateTime.Now;

//        // NAVIGATION

//        public ICollection<DivisionRoomAllocation>
//            DivisionRoomAllocations
//        { get; set; }
//            = new List<DivisionRoomAllocation>();

//        public ICollection<TimeTableEntry>
//            TimeTableEntries
//        { get; set; }
//            = new List<TimeTableEntry>();
//    }
//}



using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleX.Core.Entities
{
    public enum RoomTypeEnum : byte
    {
        Classroom = 1,
        Lab = 2
    }

    // SAME DEPARTMENT MA SAME ROOM
    // NAME ALLOW NAHI HOGA

    [Index(
        nameof(DepartmentId),
        nameof(RoomName),
        IsUnique = true)]

    public class Room
    {
        [Key]
        public int RoomId { get; set; }



        // DEPARTMENT

        [Required]
        public int DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; } = null!;



        // ROOM NAME

        [Required]
        [MaxLength(50)]
        public string RoomName { get; set; } = null!;



        // ROOM TYPE

        [Required]
        public RoomTypeEnum RoomType { get; set; }



        // CAPACITY

        [Required]
        public int Capacity { get; set; }



        // TT COORDINATOR OWNER

        public int? TTCoordinatorId { get; set; }

        [ForeignKey(nameof(TTCoordinatorId))]
        public User? TTCoordinator { get; set; }



        // COMMON

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
            = DateTime.Now;



        // NAVIGATION

        public ICollection<DivisionRoomAllocation>
            DivisionRoomAllocations
        { get; set; }
            = new List<DivisionRoomAllocation>();


        public ICollection<TimeTableEntry>
            TimeTableEntries
        { get; set; }
            = new List<TimeTableEntry>();
    }
}
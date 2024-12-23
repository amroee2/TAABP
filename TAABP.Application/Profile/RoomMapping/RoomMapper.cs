using Riok.Mapperly.Abstractions;
using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile.RoomMapping
{
    [Mapper]
    public partial class RoomMapper : IRoomMapper
    {
        public partial Room RoomDtoToRoom(RoomDto roomDto);
        public partial RoomDto RoomToRoomDto(Room room);
        public partial RoomImage RoomImageDtoToRoomImage(RoomImageDto roomImageDto);
        public partial RoomImageDto RoomImageToRoomImageDto(RoomImage roomImage);
    }
}

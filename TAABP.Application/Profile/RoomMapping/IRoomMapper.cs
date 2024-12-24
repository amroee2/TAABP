using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile.RoomMapping
{
    public partial interface IRoomMapper
    {
        Room RoomDtoToRoom(RoomDto roomDto);
        RoomDto RoomToRoomDto(Room room);
        RoomImage RoomImageDtoToRoomImage(RoomImageDto roomImageDto);
        RoomImageDto RoomImageToRoomImageDto(RoomImage roomImage);
    }
}

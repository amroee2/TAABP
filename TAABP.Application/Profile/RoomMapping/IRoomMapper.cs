using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile.RoomMapping
{
    public partial interface IRoomMapper
    {
        void RoomDtoToRoom(RoomDto roomDto, Room room);
        RoomDto RoomToRoomDto(Room room);
        RoomImage RoomImageDtoToRoomImage(RoomImageDto roomImageDto);
        RoomImageDto RoomImageToRoomImageDto(RoomImage roomImage);
    }
}

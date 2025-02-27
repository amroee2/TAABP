﻿using TAABP.Application.DTOs;

namespace TAABP.Application.ServiceInterfaces
{
    public interface IRoomService
    {
        Task<int> CreateRoomAsync(RoomDto roomDto);
        Task<RoomDto> GetRoomByIdAsync(int hotelId, int id);
        Task<List<RoomDto>> GetRoomsAsync(int hotelId);
        Task DeleteRoomAsync(int hotelId, int id);
        Task UpdateRoomAsync(RoomDto roomDto);
        Task<int> CreateRoomImageAsync(RoomImageDto roomImageDto);
        Task<RoomImageDto> GetRoomImageByIdAsync(int roomId, int id);
        Task<List<RoomImageDto>> GetRoomImagesAsync(int roomId);
        Task DeleteRoomImageAsync(int roomId, int id);
    }
}

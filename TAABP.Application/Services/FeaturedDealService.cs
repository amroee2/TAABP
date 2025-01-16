using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.FeaturedDealMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;

namespace TAABP.Application.Services
{
    public class FeaturedDealService : IFeaturedDealService
    {
        private readonly IFeaturedDealRepository _featuredDealRepository;
        private readonly IFeaturedDealMapper _featuredDealMapper;
        private readonly IRoomRepository _roomRepository;
        public FeaturedDealService(IRoomRepository roomRepository, IFeaturedDealMapper featuredDealMapper, IFeaturedDealRepository featuredDealRepository)
        {
            _featuredDealRepository = featuredDealRepository;
            _featuredDealMapper = featuredDealMapper;
            _roomRepository = roomRepository;
        }

        public async Task<FeatueredDealDto> GetFeaturedDealByIdAsync(int id)
        {
            var featuredDeal = await _featuredDealRepository.GetFeaturedDealAsync(id);
            if (featuredDeal == null)
            {
                throw new EntityNotFoundException("Featured deal not found");
            }
            return _featuredDealMapper.FeaturedDealToFeaturedDealDto(featuredDeal);
        }

        public async Task<List<FeatueredDealDto>> GetFeaturedDealsAsync()
        {
            var featuredDeals = await _featuredDealRepository.GetFeaturedDealsAsync();
            return featuredDeals.Select(x => _featuredDealMapper.FeaturedDealToFeaturedDealDto(x)).ToList();
        }

        public async Task<int> CreateFeaturedDealAsync(FeatueredDealDto featuredDealDto)
        {
            var room = await _roomRepository.GetRoomByIdAsync(featuredDealDto.RoomId);
            if (room == null)
            {
                throw new EntityNotFoundException("Room not found");
            }
            var featuredDeal = _featuredDealMapper.FeaturedDealDtoToFeaturedDeal(featuredDealDto);
            await _featuredDealRepository.CreateFeaturedDealAsync(featuredDeal);
            return featuredDeal.FeaturedDealId;
        }

        public async Task UpdateFeaturedDealAsync(FeatueredDealDto featuredDealDto)
        {
            var targetFeaturedDeal = await _featuredDealRepository.GetFeaturedDealAsync(featuredDealDto.FeaturedDealId);
            if (targetFeaturedDeal == null)
            {
                throw new EntityNotFoundException("Featured deal not found");
            }
            var room = await _roomRepository.GetRoomByIdAsync(targetFeaturedDeal.RoomId);
            if (room == null)
            {
                throw new EntityNotFoundException("Room not found");
            }

            var featuredDeal = _featuredDealMapper.FeaturedDealDtoToFeaturedDeal(featuredDealDto);
            featuredDeal.RoomId = targetFeaturedDeal.RoomId;
            await _featuredDealRepository.UpdateFeaturedDealAsync(featuredDeal);
        }

        public async Task DeleteFeaturedDealAsync(int id)
        {
            var featuredDeal = await _featuredDealRepository.GetFeaturedDealAsync(id);
            if (featuredDeal == null)
            {
                throw new EntityNotFoundException("Featured deal not found");
            }
            await _featuredDealRepository.DeleteFeaturedDealAsync(featuredDeal);
        }
    }
}

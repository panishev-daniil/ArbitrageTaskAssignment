using AutoMapper;
using StorageService.Abstraction.Models;
using StorageService.Abstraction.Models.Dtos;

namespace StorageService.API
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ArbitrageDifferenceDto, ArbitrageDifference>()
                .ReverseMap();
        }
    }
}

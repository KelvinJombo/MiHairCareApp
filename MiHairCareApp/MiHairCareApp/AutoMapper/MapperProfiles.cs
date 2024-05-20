using AutoMapper;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Domain.Entities;

namespace MiHairCareApp.AutoMapper
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            CreateMap<CreateWalletDto, Wallet>().ReverseMap();
            CreateMap<Wallet, WalletResponseDto>().ReverseMap();
        }
    }
}

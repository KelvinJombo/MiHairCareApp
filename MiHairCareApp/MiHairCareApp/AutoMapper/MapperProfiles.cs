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
            CreateMap<AppUser, RegisterResponseDto>().ReverseMap();
            CreateMap<AppUser, GetUserResponseDto>().ReverseMap();
            CreateMap<CreateProductDto, HaircareProduct>();
            CreateMap<HaircareProduct, ViewProductDto>();
            CreateMap< UpdateProductsDto, HaircareProduct >();
            CreateMap<UserTransaction, StripeResultDto>();
            CreateMap<Review, ViewReviewDto>();
            CreateMap<Review, ReviewSentDto>();
            CreateMap<CreateUserReviewDto, Review>();
            CreateMap<UpdateReviewDto, Review>();
            CreateMap<CreateBookingDto, Booking>();
            CreateMap<Booking, BookingResponseDto>();
            CreateMap<UpdateBookingDto, Booking > ();
            CreateMap<HairStyle, HairStyleResponseDto>();
            CreateMap<CreateHairStyleDto, HairStyle>();
            CreateMap<CreateReferralDto, Referral>();
            CreateMap<Referral, ReferralResponseDto>();
            CreateMap<HaircareProduct, ViewProductDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
            .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity));
        }
    }
}

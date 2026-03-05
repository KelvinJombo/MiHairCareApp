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
            CreateMap< UpdateProductsDto, HaircareProduct >();
            CreateMap<UserTransaction, StripeResultDto>();
            CreateMap<Review, ViewReviewDto>();
            CreateMap<Review, ReviewSentDto>();
            CreateMap<CreateStylistReviewDto, Review>();
            CreateMap<CreateProductReviewDto, Review>();
            CreateMap<UpdateReviewDto, Review>();
            CreateMap<CreateBookingDto, Booking>();
            CreateMap<Booking, BookingResponseDto>();
            CreateMap<UpdateBookingDto, Booking > ();
            CreateMap<HairStyle, HairStyleResponseDto>()
            .ForMember(dest => dest.HairStyleId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos));
            CreateMap<CreateHairStyleDto, HairStyle>();
            CreateMap<CreateReferralDto, Referral>();
            CreateMap<Referral, ReferralResponseDto>();             
            CreateMap<CreateProductDto, HaircareProduct>();
            CreateMap<HairStyle, AllHairStylesResponseDto>()
            .ForMember(dest => dest.HairStyleId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos));
            CreateMap<HaircareProduct, ViewProductDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src =>
                src.Photos.OrderByDescending(p => p.IsMain).Select(p => p.Url)
                    .FirstOrDefault() ?? string.Empty    
            ));

            CreateMap<Cart, CartDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<Photo, PhotoDto>()
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
            .ForMember(dest => dest.IsMain, opt => opt.MapFrom(src => src.IsMain))
            .ForMember(dest => dest.PublicId, opt => opt.MapFrom(src => src.PublicId));

            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice));
        }
    }
}

using MiHairCareApp.Application.DTO;
using MiHairCareApp.Domain;
using static MiHairCareApp.Application.DTO.AvailabilityDto;

namespace MiHairCareApp.Application.Interfaces.Services
{
    public interface IBookingService
    {
        Task<ApiResponse<BookingResponseDto>> CreateBookingAsync(CreateBookingDto bookingDto);
        Task<AvailabilityResponseDto> GetStylistAvailabilityAsync(string stylistId, DateTime date);
        Task<ApiResponse<IEnumerable<BookingResponseDto>>> GetAllBookingsAsync();  
        Task<ApiResponse<BookingResponseDto>> GetBookingByIdAsync(string bookingId);
        Task<ApiResponse<BookingResponseDto>> UpdateBookingAsync(UpdateBookingDto bookingDto);
        Task<ApiResponse<bool>> DeleteABookingAsync(string bookingId);
    }
}

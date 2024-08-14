using MiHairCareApp.Application.DTO;
using MiHairCareApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.Interfaces.Services
{
    public interface IBookingService
    {
        Task<ApiResponse<BookingResponseDto>> CreateBookingAsync(CreateBookingDto bookingDto);
        Task<ApiResponse<IEnumerable<BookingResponseDto>>> GetAllBookingsAsync();  
        Task<ApiResponse<BookingResponseDto>> GetBookingByIdAsync(string bookingId);
        Task<ApiResponse<BookingResponseDto>> UpdateBookingAsync(UpdateBookingDto bookingDto);
        Task<ApiResponse<bool>> DeleteABookingAsync(string bookingId);
    }
}

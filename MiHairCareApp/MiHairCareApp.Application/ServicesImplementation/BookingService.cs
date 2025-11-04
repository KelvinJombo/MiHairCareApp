using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain;
using MiHairCareApp.Domain.Entities;

namespace MiHairCareApp.Application.ServicesImplementation
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingService> _logger;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BookingService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }





        public async Task<ApiResponse<BookingResponseDto>> CreateBookingAsync(CreateBookingDto bookingDto)
        {
            if (bookingDto == null)
            {
                return ApiResponse<BookingResponseDto>.Failed(
                    "Booking DTO cannot be null",
                    StatusCodes.Status400BadRequest,
                    new List<string> { "Invalid input" });
            }

            try
            {
                // Map DTO to domain model
                var bookingModel = _mapper.Map<Booking>(bookingDto);

                // Add to repository and save changes
                await _unitOfWork.BookingRepository.AddAsync(bookingModel);
                await _unitOfWork.SaveChangesAsync();

                // Map to response DTO
                var bookingSetDto = _mapper.Map<BookingResponseDto>(bookingModel);

                return ApiResponse<BookingResponseDto>.Success(
                    bookingSetDto,
                    "Booking created successfully",
                    StatusCodes.Status201Created);
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, "Mapping failure in CreateBookingAsync");
                return ApiResponse<BookingResponseDto>.Failed(
                    "An error occurred while mapping the Booking DTO",
                    StatusCodes.Status500InternalServerError,
                    new List<string> { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the booking");
                return ApiResponse<BookingResponseDto>.Failed(
                    "An error occurred while processing your request",
                    StatusCodes.Status500InternalServerError,
                    new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<bool>> DeleteABookingAsync(string bookingId)
        {
            if (string.IsNullOrEmpty(bookingId))
            {
                return ApiResponse<bool>.Failed("Booking ID cannot be null or empty", StatusCodes.Status400BadRequest, new List<string> { "Invalid input" });
            }

            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
            {
                return ApiResponse<bool>.Failed("Booking details not found", StatusCodes.Status404NotFound, new List<string>());
            }

            try
            {
                _unitOfWork.BookingRepository.DeleteAsync(booking);
                await _unitOfWork.SaveChangesAsync();
                return ApiResponse<bool>.Success(true, "Booking entry deleted successfully", StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the booking");
                return ApiResponse<bool>.Failed("An error occurred while deleting the booking records", StatusCodes.Status500InternalServerError, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<IEnumerable<BookingResponseDto>>> GetAllBookingsAsync()
        {
            try
            {
                var booking = await _unitOfWork.BookingRepository.GetAllAsync();

                if (booking == null || !booking.Any())
                {
                    return ApiResponse<IEnumerable<BookingResponseDto>>.Failed("No bookings were found", 404, new List<string> { "Nothing found" });
                }

                var viewBookings = _mapper.Map<List<BookingResponseDto>>(booking);

                return ApiResponse<IEnumerable<BookingResponseDto>>.Success(viewBookings, "Bookings retrieved successfully", 200);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while retrieving the bookings");
                return ApiResponse<IEnumerable<BookingResponseDto>>.Failed("An error occurred while retrieving the bookings", 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<BookingResponseDto>> GetBookingByIdAsync(string bookingId)
        {
            try
            {
                var booking = await _unitOfWork.BookingRepository.FindSingleAsync(b => b.Id == bookingId);

                if (booking == null)
                {
                    return ApiResponse<BookingResponseDto>.Failed("Can't find a booking with the specified ID", 404, new List<string> { "No Product Found" });
                }

                var viewBooking = _mapper.Map<BookingResponseDto>(booking);

                return ApiResponse<BookingResponseDto>.Success(viewBooking, "Booking retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the booking");
                return ApiResponse<BookingResponseDto>.Failed("An error occurred while retrieving the booking", 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<BookingResponseDto>> UpdateBookingAsync(UpdateBookingDto bookingDto)
        {
            try
            {
                var booking = await _unitOfWork.BookingRepository.FindSingleAsync(b => b.Id == bookingDto.BookingID);

                if (booking == null)
                {
                    return ApiResponse<BookingResponseDto>.Failed("Booking not found", 404, new List<string> { "No Booking Info Found" });
                }

                _mapper.Map(bookingDto, booking);

                _unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();

                var updatedbooking = _mapper.Map<BookingResponseDto> (booking);
                return ApiResponse<BookingResponseDto>.Success(updatedbooking, "Booking Record updated successfully", 200);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while updating the booking record");
                return ApiResponse<BookingResponseDto >.Failed("An error occurred while updating the booking", 500, new List<string> { ex.Message });
            }
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain;
using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Domain.Exceptions;
using static MiHairCareApp.Application.DTO.AvailabilityDto;

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
                throw new ValidationException("Booking DTO cannot be null");

            try
            {
                var bookingModel = _mapper.Map<Booking>(bookingDto) ?? throw new ServiceException("Mapping Booking DTO failed");
                await _unitOfWork.BookingRepository.AddAsync(bookingModel);
                await _unitOfWork.SaveChangesAsync();

                var bookingSetDto = _mapper.Map<BookingResponseDto>(bookingModel);
                return ApiResponse<BookingResponseDto>.Success(bookingSetDto, "Booking created successfully", StatusCodes.Status201Created);
            }
            catch (AutoMapperMappingException ex)
            {
                _logger.LogError(ex, "Mapping failure in CreateBookingAsync");
                throw new ServiceException("An error occurred while mapping the Booking DTO", ex);
            }
        }

        public async Task<AvailabilityResponseDto> GetStylistAvailabilityAsync(string userId, DateTime date)
        {
            var stylist = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (stylist == null) return null;

            var workingHours = GenerateTimeSlots(stylist.StartHour, stylist.EndHour);

            var bookedSlots = await _unitOfWork.BookingRepository.Query(b => b.AppUserId == userId && b.AppointmentDate.Date == date.Date)
                .Select(b => b.TimeSlot)
                .ToListAsync();

            var availableSlots = workingHours.Except(bookedSlots).ToList();

            return new AvailabilityResponseDto(userId, date, workingHours, bookedSlots, availableSlots);
        }

        public async Task<ApiResponse<bool>> DeleteABookingAsync(string bookingId)
        {
            if (string.IsNullOrEmpty(bookingId))
                throw new ValidationException("Booking ID cannot be null or empty");

            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
                throw new NotFoundException("Booking details not found");

                  _unitOfWork.BookingRepository.DeleteAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "Booking entry deleted successfully", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<IEnumerable<BookingResponseDto>>> GetAllBookingsAsync()
        {
            var bookings = await _unitOfWork.BookingRepository.GetAllAsync();
            if (bookings == null || !bookings.Any())
                return ApiResponse<IEnumerable<BookingResponseDto>>.Failed("No bookings were found", 404, new List<string> { "Nothing found" });

            var viewBookings = _mapper.Map<List<BookingResponseDto>>(bookings);
            return ApiResponse<IEnumerable<BookingResponseDto>>.Success(viewBookings, "Bookings retrieved successfully", 200);
        }

        public async Task<ApiResponse<BookingResponseDto>> GetBookingByIdAsync(string bookingId)
        {
            if (string.IsNullOrEmpty(bookingId))
                throw new ValidationException("Booking id is required");

            var booking = await _unitOfWork.BookingRepository.FindSingleAsync(b => b.Id == bookingId);
            if (booking == null)
                throw new NotFoundException("Can't find a booking with the specified ID");

            var viewBooking = _mapper.Map<BookingResponseDto>(booking);
            return ApiResponse<BookingResponseDto>.Success(viewBooking, "Booking retrieved successfully", 200);
        }

        public async Task<ApiResponse<BookingResponseDto>> UpdateBookingAsync(UpdateBookingDto bookingDto)
        {
            if (bookingDto == null) throw new ValidationException("Booking DTO cannot be null");

            var booking = await _unitOfWork.BookingRepository.FindSingleAsync(b => b.Id == bookingDto.BookingID);
            if (booking == null) throw new NotFoundException("Booking not found");

            _mapper.Map(bookingDto, booking);
            _unitOfWork.BookingRepository.Update(booking);
            await _unitOfWork.SaveChangesAsync();

            var updatedbooking = _mapper.Map<BookingResponseDto>(booking);
            return ApiResponse<BookingResponseDto>.Success(updatedbooking, "Booking Record updated successfully", 200);
        }

        private List<string> GenerateTimeSlots(TimeSpan start, TimeSpan end)
        {
            List<string> slots = new();
            var current = start;
            while (current < end)
            {
                slots.Add(current.ToString(@"hh\:mm"));
                current = current.Add(TimeSpan.FromMinutes(30));
            }
            return slots;
        }
    }
}

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
    public class ReferralService : IReferralService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ReferralService> _logger;

        public ReferralService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ReferralService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }




        public async Task<ApiResponse<ReferralResponseDto>> CreateReferralAsync(CreateReferralDto referralDto)
        {
            if (referralDto == null)
            {
                return ApiResponse<ReferralResponseDto>.Failed("Referral DTO cannot be null", 400, new List<string> { "Invalid input" });
            }

            var referralModel = _mapper.Map<Referral>(referralDto);

            if (referralModel == null)
            {
                return ApiResponse<ReferralResponseDto>.Failed("Mapping Referral DTO to Referral Model failed", 500, new List<string> { "Mapping failure" });
            }

            try
            {
                await _unitOfWork.ReferralsRepository.AddAsync(referralModel);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the referral");
                return ApiResponse<ReferralResponseDto>.Failed("An error occurred while adding the referral", 500, new List<string> { ex.Message });
            }

            var referralSetDto = _mapper.Map<ReferralResponseDto>(referralModel);

            if (referralSetDto == null)
            {
                return ApiResponse<ReferralResponseDto>.Failed("Mapping Referral model to ReferralSet DTO failed", 500, new List<string> { "Mapping failure" });
            }

            return ApiResponse<ReferralResponseDto>.Success(referralSetDto, "Referral added successfully", 201);
        }

        public async Task<ApiResponse<bool>> DeleteReferralAsync(string referralId)
        {
            if (string.IsNullOrEmpty(referralId))
            {
                return ApiResponse<bool>.Failed("Referral ID cannot be null or empty", StatusCodes.Status400BadRequest, new List<string> { "Invalid input" });
            }

            var referral = await _unitOfWork.ReferralsRepository.GetByIdAsync(referralId);
            if (referral == null)
            {
                return ApiResponse<bool>.Failed("Booking details not found", StatusCodes.Status404NotFound, new List<string>());
            }

            try
            {
                _unitOfWork.ReferralsRepository.DeleteAsync(referral);
                await _unitOfWork.SaveChangesAsync();
                return ApiResponse<bool>.Success(true, "Booking entry deleted successfully", StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the referral");
                return ApiResponse<bool>.Failed("An error occurred while deleting the referral records", StatusCodes.Status500InternalServerError, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<IEnumerable<ReferralResponseDto>>> GetAllReferralsAsync()
        {
            try
            {
                var referral = await _unitOfWork.ReferralsRepository.GetAllAsync();

                if (referral == null || !referral.Any())
                {
                    return ApiResponse<IEnumerable<ReferralResponseDto>>.Failed("No referrals were found", 404, new List<string> { "Nothing found" });
                }

                var viewReferrals = _mapper.Map<List<ReferralResponseDto>>(referral);

                return ApiResponse<IEnumerable<ReferralResponseDto>>.Success(viewReferrals, "Referrals retrieved successfully", 200);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while retrieving the referrals");
                return ApiResponse<IEnumerable<ReferralResponseDto>>.Failed("An error occurred while retrieving the referrals", 500, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<ReferralResponseDto>> GetReferralByIdAsync(string referralId)
        {
            try
            {
                var referal = await _unitOfWork.ReferralsRepository.FindSingleAsync(b => b.Id == referralId);

                if (referal == null)
                {
                    return ApiResponse<ReferralResponseDto>.Failed("Can't find a referral with the specified ID", 404, new List<string> { "No Product Found" });
                }

                var viewReferral = _mapper.Map<ReferralResponseDto>(referal);

                return ApiResponse<ReferralResponseDto>.Success(viewReferral, "Referral retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the referral");
                return ApiResponse<ReferralResponseDto>.Failed("An error occurred while retrieving the referral", 500, new List<string> { ex.Message });
            }
        }
    }
}

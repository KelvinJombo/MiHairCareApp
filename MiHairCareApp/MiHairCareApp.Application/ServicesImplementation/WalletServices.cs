//using AutoMapper;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using MiHairCareApp.Application.DTO;
//using MiHairCareApp.Application.Interfaces.Repository;
//using MiHairCareApp.Application.Interfaces.Services;
//using MiHairCareApp.Commons.Utilities;
//using MiHairCareApp.Domain;
//using MiHairCareApp.Domain.Entities;
//using MiHairCareApp.Domain.Enums;

//namespace MiHairCareApp.Application.ServicesImplementation
//{
//    public class WalletServices : IWalletServices
//    {
//        private readonly ILogger<WalletServices> _logger;
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//        private readonly IConfiguration _config;
//        private readonly HttpClient _httpClient;

//        public WalletServices(ILogger<WalletServices> logger, IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
//        {
//            _logger = logger;
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//            _config = config;
//            _httpClient = new HttpClient();
//           // _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {secretKey}");
//        }


//        public async Task<ApiResponse<bool>> CreateWallet(CreateWalletDto createWalletDto)
//        {
//            try
//            {
//                var wallet = _mapper.Map<Wallet>(createWalletDto);
//                wallet.WalletNumber = WalletGenerator.SetWalletId(createWalletDto.PhoneNumber);
//                wallet.Currency = Currency.Naira;

//                // Generate a random transaction pin or prompt the user to set their own
//                wallet.TransactionPin = GenerateRandomTransactionPin();

//                await _unitOfWork.WalletRepository.AddAsync(wallet);
//                await _unitOfWork.SaveChangesAsync();

//                var responseDto = _mapper.Map<WalletResponseDto>(wallet);
//                return new ApiResponse<bool>(true, "Wallet Created Successfully");
//            }
//            catch (Exception ex)
//            {
//                // Log the exception details for debugging
//                _logger.LogError(ex, "Failed to create wallet");

//                // Provide a generic error message to users
//                return new ApiResponse<bool>(false, "Failed to create wallet. Please try again later.");
//            }
//        } 

//        private string GenerateRandomTransactionPin()
//        {
//            // Generate a random 4-digit transaction pin
//            Random random = new Random();
//            int pin = random.Next(1000, 9999);
//            return pin.ToString();
//        }



//        public async Task<ApiResponse<WalletResponseDto>> GetWalletByUserId(string userId)
//        {
//            try
//            {
//                // Fetch the wallet associated with the given user ID
//                var wallet = await _unitOfWork.WalletRepository.FindSingleAsync(w => w.UserId == userId);

//                if (wallet == null)
//                {
//                    // Return a failure response if no wallet is found
//                    return ApiResponse<WalletResponseDto>.Failed("Wallet not found", StatusCodes.Status404NotFound, new List<string>());
//                }

//                // Map the Wallet entity to a WalletResponseDto
//                var responseDto = _mapper.Map<WalletResponseDto>(wallet);

//                // Return a success response with the wallet data
//                return ApiResponse<WalletResponseDto>.Success(responseDto, "Wallet retrieved successfully", StatusCodes.Status200OK);
//            }
//            catch (Exception ex)
//            {
//                // Log the exception details for debugging
//                _logger.LogError(ex, "Failed to retrieve wallet");

//                // Provide a generic error message to users
//                return ApiResponse<WalletResponseDto>.Failed("Failed to retrieve wallet. Please try again later.", StatusCodes.Status500InternalServerError, new List<string> { ex.Message });
//            }
//        }





//    }
//}

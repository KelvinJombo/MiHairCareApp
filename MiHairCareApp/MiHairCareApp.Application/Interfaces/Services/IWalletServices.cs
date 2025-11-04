using MiHairCareApp.Application.DTO;
using MiHairCareApp.Domain;

namespace MiHairCareApp.Application.Interfaces.Services
{
    public interface IWalletServices
    {
        Task<ApiResponse<bool>> CreateWallet(CreateWalletDto createWalletDto);
        //Task<ApiResponse<List<WalletResponseDto>>> GetAllWallets();
        //Task<ApiResponse<Wallet>> GetWalletByNumber(string phone);
        //Task<ApiResponse<CreditResponseDto>> FundWallet(FundWalletDto fundWalletDto);
        //Task<ApiResponse<DebitResponseDto>> DebitWallet(DebitWalletDto debitWalletDto);
        //Task<string> VerifyTransaction(string referenceCode, string userId);
        Task<ApiResponse<WalletResponseDto>> GetWalletByUserId(string userId);
        //Task<ApiResponse<decimal>> GetTotalWalletBalance(string userId);
    }
}

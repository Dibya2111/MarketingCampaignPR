using System.Threading.Tasks;
using MarketingCampaignServer.Models.Dtos;

namespace MarketingCampaignServer.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    }
}

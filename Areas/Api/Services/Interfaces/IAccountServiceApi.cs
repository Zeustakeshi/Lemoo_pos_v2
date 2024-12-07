using Lemoo_pos.Areas.Api.Dto;


namespace Lemoo_pos.Areas.Api.Services.Interfaces
{
    public interface IAccountServiceApi
    {
        AccountInfoResponseDto GetAccountById(long accountId);
    }
}

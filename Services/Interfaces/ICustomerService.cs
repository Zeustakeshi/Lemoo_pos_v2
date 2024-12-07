using Lemoo_pos.Areas.Api.Dto;

namespace Lemoo_pos.Services.Interfaces
{
    public interface ICustomerService
    {
        CustomerResponseDto CreateCustomer(CreateCustomerDto dto, long storeId, long accountId);

        long GetCustomerCount(long storeId);
    }
}

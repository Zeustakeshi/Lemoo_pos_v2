using Lemoo_pos.Models.Dto;

namespace Lemoo_pos.Services.Interfaces
{
    public interface ICustomerService
    {
        CustomerResponseDto CreateCustomer(CreateCustomerDto dto, long storeId, long accountId);
    }
}

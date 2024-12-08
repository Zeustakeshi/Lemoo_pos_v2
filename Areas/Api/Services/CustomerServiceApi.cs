using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Areas.Api.Services.Interfaces;
using Lemoo_pos.Common.Enums;
using Lemoo_pos.Data;
using Lemoo_pos.Helper;
using Lemoo_pos.Messages;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Services.Interfaces;
using MassTransit;

namespace Lemoo_pos.Areas.Api.Services
{
    public class CustomerServiceApi : ICustomerServiceApi
    {
        private readonly AppDbContext _db;
        private readonly IElasticsearchService _elasticsearchService;
        private readonly IPublishEndpoint _publishEndpoint;

        public CustomerServiceApi(AppDbContext db, IElasticsearchService elasticsearchService, IPublishEndpoint publishEndpoint)
        {
            _db = db;
            _elasticsearchService = elasticsearchService;
            _publishEndpoint = publishEndpoint;
        }

        public CustomerResponseDto CreateCustomer(CreateCustomerDto dto, long storeId, long accountId)
        {
            Store store = _db.Stores.FirstOrDefault(s => s.Id == storeId) ?? throw new Exception($"Store: {storeId} not found");

            Staff staff = _db.Staffs.FirstOrDefault(staff => staff.AccountId == accountId) ?? throw new Exception($"Staff with accountId: {accountId} not found");

            bool hasMatchingPhone = _db.Customers.Any(customer =>
    customer.StoreId == storeId && customer.Phone == dto.Phone);

            bool hasMatchingEmail = !String.IsNullOrEmpty(dto.Email) &&
                _db.Customers.Any(customer =>
                    customer.StoreId == storeId && customer.Email == dto.Email);

            bool isExistedCustomer = hasMatchingPhone || hasMatchingEmail;

            if (isExistedCustomer)
            {
                throw new ArgumentException("Customer already existed in this store");
            }

            Customer customer = new()
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Staff = staff,
                Store = store,
                StoreId = store.Id,
                StaffId = staff.Id,
                Address = dto.Address,
                Email = dto.Email,
                Province = dto.Province,
                District = dto.District,
                Ward = dto.Ward,
                ProvinceCode = dto.ProvinceCode,
                DistrictCode = dto.DistrictCode,
                WardCode = dto.WardCode,
            };

            if (dto.Gender != null && Enum.TryParse(dto.Gender, out Gender gender))
            {
                customer.Gender = gender;
            }

            Customer newCustomer = _db.Customers.Update(customer).Entity;

            _db.SaveChanges();

            // save customer to elasticsearch 

            _publishEndpoint.Publish(new SaveSearchCustomerMessage()
            {
                Id = newCustomer.Id,
                Name = newCustomer.Name,
                Phone = newCustomer.Phone,
                StoreId = storeId,
                Email = newCustomer.Email,
                Keyword = LanguageHelper.RemoveVietnameseTones(newCustomer.Name)
            });

            return new()
            {
                Id = newCustomer.Id,
                Name = newCustomer.Name,
                Phone = newCustomer.Phone,
                Address = newCustomer.Address,
                Province = newCustomer.Province,
                ProvinceCode = newCustomer.ProvinceCode,
                District = newCustomer.District,
                DistrictCode = newCustomer.DistrictCode,
                Ward = newCustomer.Ward,
                WardCode = newCustomer.WardCode,
                Email = newCustomer.Email,
                Gender = nameof(newCustomer.Gender),
            };

        }

        public long GetCustomerCount(long storeId)
        {
            return _db.Customers.Where(c => c.StoreId == storeId).Count();
        }
    }
}
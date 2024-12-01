using Lemoo_pos.Data;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services.Interfaces;

namespace Lemoo_pos.Services
{
    public class BrandService : IBrandService
    {
        private readonly AppDbContext _db;
        private readonly ISessionService _sessionService;


        public BrandService(
            AppDbContext db,
            ISessionService sessionService
        ) { 
            _db = db;
            _sessionService = sessionService;
        }

        public Brand CreateBrand(CreateBrandViewModel model)
        {
           
            Store store = _sessionService.GetStoreSession();

            bool brandExists = _db.Brands.Any(brand => brand.StoreId == store.Id && brand.Name.Equals(model.Name));

            if (brandExists)
            {
                throw new Exception("Tên nhãn hiệu đã tồn tại trên cửa hàng");
            }

            Brand brand = new()
            {
                Name = model.Name,
                Store = store,
                StoreId = store.Id,
            };

            Brand newBrand = _db.Brands.Add(brand).Entity;

            _db.SaveChanges();

            return newBrand;
        }

        public List<Brand> GetAllBrand()
        {
            long storeId = _sessionService.GetStoreIdSession();
            return _db.Brands.Where(brand => brand.StoreId == storeId).ToList();
        }
    }
}

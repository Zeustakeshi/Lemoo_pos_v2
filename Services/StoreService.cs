using Lemoo_pos.Data;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Services.Interfaces;

namespace Lemoo_pos.Services
{
    public class StoreService : IStoreService
    {

        private readonly AppDbContext _db; 

        public StoreService(AppDbContext db)
        {
            _db = db;
        }

        public Store CreateNewStore(string name)
        {
            return  _db.Stores.Add(new() { Name = name }).Entity;
        }

        public Branch CreateDefaultBranch (Store store)
        {
            Branch defaultBranch = new() { Name = "Chi nhánh mặc định", Store = store };
            return  _db.Branches.Add(defaultBranch).Entity;
        }

    }
}

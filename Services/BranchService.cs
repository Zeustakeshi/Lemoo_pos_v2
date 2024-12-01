using Lemoo_pos.Data;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Services.Interfaces;

namespace Lemoo_pos.Services
{
    public class BranchService : IBranchService
    {
        private readonly AppDbContext _db;
        private readonly ISessionService _sessionService;

        public BranchService(AppDbContext db, ISessionService sessionService) 
        {
            _db = db;
            _sessionService = sessionService;
        }

        public List<Branch> GetAllBranch()
        {
            long storeId = _sessionService.GetStoreIdSession();
            return _db.Branches.Where(branch => branch.StoreId == storeId).ToList();
        }
    }
}

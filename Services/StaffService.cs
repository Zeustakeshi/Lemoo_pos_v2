using Lemoo_pos.Common.Enums;
using Lemoo_pos.Data;
using Lemoo_pos.Helper;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace Lemoo_pos.Services
{
    public class StaffService : IStaffService
    {
        private readonly string BASE_URL = Environment.GetEnvironmentVariable("BASE_URL") ?? "https://localhost:7278";

        private readonly AppDbContext _db;
        private readonly ISessionService _sessionService;
        private readonly IAccountService _accountService;
        private readonly PasswordHelper _passworHelper;
        private readonly IMailService _mailService;
        private readonly IAuthService _authService;

        public StaffService(
            AppDbContext db,
            ISessionService sessionService,
            IAccountService accountService,
            PasswordHelper passwordHelper,
            IMailService mailService,
            IAuthService authService
         )
        {
            _db = db;
            _sessionService = sessionService;
            _accountService = accountService;
            _passworHelper = passwordHelper;
            _mailService = mailService;
            _authService = authService;
        }

        public List<Staff> GetAllStaff(long storeId)
        {
            List<Staff> staffs = [.. _db.Staffs
                .Where(s => s.Branch.Store.Id == storeId)
                .Include(s => s.Branch)
                    .ThenInclude(branch => branch.Store)
                .Include(s => s.Account)
            ];

            foreach (var staff in staffs)
            {
                List<AccountAuthority> authorities = [.. _db.AccountAuthorities.Where(a => a.Account.Id == staff.Account.Id)];
                staff.Account.Authorities = authorities;
            }
            return staffs;
        }

        public List<string> GetAllStaffStatus()
        {
            return [.. Enum.GetNames(typeof(StaffStatus))];
        }

        public void CreateStaff(CreateStaffViewModel model)
        {
            Store store = _sessionService.GetStoreSession();

            bool existedStaff = _db.Staffs
                .Include(s => s.Account)
                .Any(s => s.Account.Email == model.Email ||
                s.Account.Phone == model.Phone
            );

            if (existedStaff)
            {
                throw new Exception("Nhân viên đã tồn tại trong cửa hàng");
            }


            List<Authority> authorities = [.. _db.Authorities
                .Where(a => model.Roles.Contains(a.Id) && a.StoreId == store.Id)];

            Account account = _accountService.CreateAccount(
                email: model.Email,
                phone: model.Phone,
                name: model.Name,
                password: _passworHelper.HashPassword(Guid.NewGuid().ToString()),
                store: store,
                isActive: false,
                authorities: authorities
            );

            Branch branch = _db.Branches.Single(branch => branch.Id == model.Branch) ?? throw new Exception("Không tìm thấy chi nhánh này");

            if (Enum.TryParse(model.Gender, out Gender gender))
            {

                Staff staff = new()
                {
                    Account = account,
                    AccountId = account.Id,
                    Branch = branch,
                    Status = StaffStatus.PENDING_INVITATION,
                    Gender = gender,
                };

                _db.Staffs.Add(staff);

                _db.SaveChanges();


            }
            else
            {
                string errorMessage = $"Không thể ánh xạ giá trị '{gender}' thành enum.";
                Console.WriteLine(errorMessage);
                throw new Exception(errorMessage);
            }


            string resetPasswordToken = _authService.GenerateResetPasswordToken(account.Id, model.Email);

            _mailService.SendAccountActivationEmail(
                email: model.Email,
                activationLink: BASE_URL + "/auth/reset-password?token=" + resetPasswordToken,
                storeName: store.Name,
                staffName: model.Name,
                staffEmail: model.Email,
                supportStoreEmail: "lemoo@support.lemoo.pos.com",
                supportStorePhone: "0123456789",
                storeOwnerName: store.Name
             );
        }
    }
}

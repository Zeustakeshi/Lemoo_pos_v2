using System.Security.Claims;
using Elasticsearch.Net;
using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Areas.Api.Dto.Response;
using Lemoo_pos.Areas.Api.Services.Interfaces;
using Lemoo_pos.Data;
using Lemoo_pos.Helper;
using Lemoo_pos.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Nest;


namespace Lemoo_pos.Areas.Api.Services
{
    public class ShiftServiceApi : IShiftServiceApi
    {
        private readonly AppDbContext _db;
        public ShiftServiceApi(AppDbContext db)
        {
            _db = db;
        }

        public List<ShiftResponseDto> GetAllShift(long storeId, long? staffId, DateTime? startTime, DateTime? endTime)
        {
            DateTime now = DateTime.UtcNow;
            endTime ??= now;
            startTime ??= now.AddDays(-1);

            return [.. _db.Shifts
        .Where(s => s.StoreId == storeId
                && s.StartTime <= endTime
                && s.StartTime >= startTime
                && (staffId == null || s.StaffId == staffId)
        )
        .Select(shift => new ShiftResponseDto()
        {
            Id = shift.Id,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            TotalFunds = shift.Orders.Sum(o => o.Total),
            Status = shift.Status,
            Staff = new () {
                Id = shift.Staff.Id,
                Name = shift.Staff.Account.Name,
                Avatar = shift.Staff.Account.Avatar,
            }
        }).OrderByDescending(s=> s.StartTime)];

        }

        public ShiftDetailResponseDto GetShiftById(long storeId, long shiftId)
        {
            ShiftDetailResponseDto shift = _db.Shifts
                .Where(shift => shift.Id == shiftId && shift.StoreId == storeId)
                .Select(shift => new ShiftDetailResponseDto()
                {
                    Id = shift.Id,
                    StartTime = shift.StartTime,
                    Status = shift.Status,
                    EndTime = shift.EndTime,
                    OpenNote = shift.OpenNote,
                    CloseNote = shift.CloseNote,
                    Staff = new()
                    {
                        Id = shift.Staff.Id,
                        Name = shift.Staff.Account.Name,
                        Avatar = shift.Staff.Account.Avatar
                    }
                }).FirstOrDefault() ?? throw new KeyNotFoundException($"Shift {shiftId} not found");

            // add shift orders
            shift.Orders = GetAllShiftOrderByShiftId(storeId, shift.Id);
            return shift;
        }

        public long CreateShift(long storeId, long accountId, ShiftRequestDto dto)
        {
            Staff staff = _db.Staffs.FirstOrDefault(s => s.AccountId == accountId && s.Branch.StoreId == storeId) ??
             throw new UnauthorizedAccessException($"Staff with account id ${accountId} not found");

            Store store = _db.Stores.FirstOrDefault(s => s.Id == storeId) ??
                          throw new UnauthorizedAccessException($"Store with id {storeId} not found");

            bool existedShift = _db.Shifts.Any(s => s.StaffId == staff.Id && s.StoreId == storeId && s.Status.Equals(Common.Enums.ShiftStatus.OPENING));

            if (existedShift)
            {
                throw new ArgumentException("Cannot create a new shift, there is an open shift already.");
            }

            Shift shift = new()
            {
                Staff = staff,
                StaffId = staff.Id,
                Store = store,
                StoreId = store.Id,
                StartTime = DateTime.UtcNow,
                OpenNote = dto.Note
            };

            Shift newShift = _db.Shifts.Add(shift).Entity;
            _db.SaveChanges();

            return newShift.Id;
        }

        public ShiftDetailResponseDto CloseShift(long storeId, long accountId, ShiftRequestDto dto)
        {
            Staff staff = _db.Staffs
            .Include(staff => staff.Account)
            .FirstOrDefault(s => s.AccountId == accountId && s.Branch.StoreId == storeId) ??
                throw new UnauthorizedAccessException($"Staff with account id ${accountId} not found");
            Store store = _db.Stores.FirstOrDefault(s => s.Id == storeId) ??
                throw new UnauthorizedAccessException($"Store with id {storeId} not found");
            Shift shift = _db.Shifts
            .FirstOrDefault(s => s.StaffId == staff.Id && s.StoreId == storeId && s.Status.Equals(Common.Enums.ShiftStatus.OPENING)) ??
                throw new ArgumentException("There is no open shift for this staff.");

            shift.EndTime = DateTime.UtcNow;
            shift.Status = Common.Enums.ShiftStatus.CLOSED;
            shift.CloseNote = dto.Note;

            _db.Shifts.Update(shift);
            _db.SaveChanges();
            return new ShiftDetailResponseDto()
            {
                Id = shift.Id,
                Staff = new()
                {
                    Id = staff.Id,
                    Name = staff.Account.Name,
                    Avatar = staff.Account.Avatar
                },
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                Status = shift.Status,
                CloseNote = shift.CloseNote,
                OpenNote = shift.OpenNote,
                Orders = GetAllShiftOrderByShiftId(storeId, shift.Id)
            };
        }

        public List<ShiftOrderResponseDto> GetAllShiftOrderByShiftId(long storeId, long shiftId)
        {
            bool existedShift = _db.Shifts.Any(s => s.StoreId == storeId);
            if (!existedShift) throw new KeyNotFoundException($"Shift {shiftId} not found");

            return [.._db.Orders.Where(o => o.StoreId == storeId && o.ShiftId == shiftId)
            .Select(order => new ShiftOrderResponseDto()
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                PaymentMethod = order.PaymentMethod.GetStringValue(),
                Total = order.Total
            })];
        }


    }
}
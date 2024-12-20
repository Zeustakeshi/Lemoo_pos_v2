﻿using Lemoo_pos.Data;
using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Lemoo_pos.Services
{
    public class InventoryService : IInventoryService
    {

        private readonly AppDbContext _db;
        private readonly ISessionService _sessionService;
        private readonly IElasticsearchService _elasticsearchService;

        public InventoryService(AppDbContext db, ISessionService sessionService, IElasticsearchService elasticsearchService)
        {

            _db = db;
            _sessionService = sessionService;
            _elasticsearchService = elasticsearchService;
        }

        public async Task<Inventory> CreateInventory(long productVariantId, long branchId, long quantity, long available, long staffId)
        {

            Staff staff = _db.Staffs.FirstOrDefault(staff => staff.Id == staffId) ?? throw new Exception($"Staff {staffId} not found");
            Branch branch = _db.Branches.FirstOrDefault(branch => branch.Id == branchId) ?? throw new Exception($"Branch {branchId} not found");
            ProductVariant productVariant = _db.ProductVariants.FirstOrDefault(variant => variant.Id == productVariantId) ?? throw new Exception($"Variant {productVariantId} not found");

            Inventory inventory = new()
            {
                ProductVariant = productVariant,
                ProductVariantId = productVariant.Id,
                Available = available,
                Branch = branch,
                Quantity = quantity,
            };

            var newInventory = _db.Inventories.Add(inventory).Entity;

            InventoryLog log = new()
            {
                Inventory = newInventory,
                InventoryId = newInventory.Id,
                PreviousAvailableQuantity = newInventory.Available,
                PreviousQuantity = newInventory.Quantity,
                NewAvailableQuantity = newInventory.Available,
                NewQuantity = newInventory.Quantity,
                Action = "Khời tạo kho hàng",
                Staff = staff,
                StaffId = staff.Id
            };

            _db.InventoryLogs.Add(log);

            await _db.SaveChangesAsync();

            return inventory;
        }



        public List<InventoryHistoryViewModel> GetInventoryHistories(long productVariantId)
        {

            List<Inventory> inventories = [.. _db.Inventories
                .Include(inventory => inventory.Branch)
                .Where(inventory => inventory.ProductVariantId == productVariantId)];

            List<InventoryHistoryViewModel> inventoryHistories = [];


            foreach (var inventory in inventories)
            {
                List<InventoryLog> logs = [.. _db.InventoryLogs
                    .Include(log => log.Staff)
                    .ThenInclude(staff=> staff.Account)
                    .Where(log => log.InventoryId == inventory.Id)];

                foreach (var log in logs)
                {
                    inventoryHistories.Add(new()
                    {
                        Action = log.Action,
                        BranchName = inventory.Branch.Name,
                        CreatedAt = log.CreatedAt,
                        UpdatedAt = log.UpdatedAt,
                        UpdateBy = log.Staff,
                        NewAvailableQuantity = log.NewAvailableQuantity ?? 0,
                        NewQuantity = log.NewQuantity ?? 0,
                        PreviousAvailableQuantity = log.PreviousAvailableQuantity ?? 0,
                        PreviousQuantity = log.PreviousQuantity ?? 0,
                    });
                }
            }

            return inventoryHistories;
        }

        public void UpdateInventory(long id, UpdateVariantInventoryViewModel model)
        {
            Staff staff = _sessionService.GetStaffSession();

            Inventory inventory = _db.Inventories
                .Include(inventory => inventory.ProductVariant)
                .Single(inventory => inventory.Id == id) ?? throw new Exception("Không tìm thấy thông tin kho");
            if (inventory.Quantity == model.Quantity && inventory.Available == model.Available) return;

            InventoryLog log = new()
            {
                Inventory = inventory,
                InventoryId = id,
                PreviousAvailableQuantity = inventory.Available,
                PreviousQuantity = inventory.Quantity,
                NewAvailableQuantity = model.Available,
                NewQuantity = model.Quantity,
                Action = GetInventoryLogAction(model.Reason),
                Staff = staff,
                StaffId = staff.Id
            };

            inventory.Quantity = model.Quantity;
            inventory.Available = model.Available;


            _db.Inventories.Update(inventory);

            _db.InventoryLogs.Add(log);

            _db.SaveChanges();

            _elasticsearchService.SaveDocumentById(new
            {
                Quantity = model.Available,
            }, inventory.ProductVariant.ProductId.ToString(), "products");

        }

        private string GetInventoryLogAction(string reason)
        {
            string prefix = "cập nhật tồn kho do ";
            return reason switch
            {
                "ACTUAL" => prefix + "thực tế",
                "DAMAGED" => prefix + "hư hỏng",
                "RETURN" => prefix + "trả hàng",
                "TRANSFER" => prefix + "chuyển hàng",
                "PRODUCTION" => prefix + "sản xuất thêm",
                "LOST" => "thất lạc",
                _ => "nguyên nhân khác",
            };
        }
    }
}

using Lemoo_pos.Data;
using Lemoo_pos.Models.Entities;
using Lemoo_pos.Models.ViewModels;
using Lemoo_pos.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Lemoo_pos.Services
{
    public class ProductCategoryService : IProductCategoryService
    {

        private readonly ISessionService _sessionService;
        private readonly AppDbContext _db;
        private readonly HttpContext _httpContext;
        public ProductCategoryService(
            AppDbContext db,
            IHttpContextAccessor httpContextAccessor,
            ISessionService sessionService
         )
        {
            _db = db;
            _httpContext = httpContextAccessor.HttpContext;
            _sessionService = sessionService;
        }



        public List<ProductCategory> GetAllCategories()
        {
            long storeId = Convert.ToInt64(_httpContext.Session.GetString("StoreId"));

            return [.. _db.ProductCategories
                .Where(category => category.Store.Id == storeId)
                .Include(category => category.Products)
                .OrderBy(category => category.UpdatedAt)];
        }


        public void CreateCategory(CreateCategoryViewModel model)
        {
            Store store = _sessionService.GetStoreSession();

            ProductCategory category = new()
            {
                Name = model.Name,
                Store = store,
                Description = model.Description
            };

            ProductCategory newCategory = _db.ProductCategories.Add(category).Entity;

            _db.SaveChanges();

            if (!model.AddProductManual) AddProductToCategory(newCategory.Id, store.Id, model.Conditions, model.MatchAllCondition);

        }

        private async Task AddProductToCategory(long categoryId, long storeId, List<CreateCategoryCondition> conditions, bool isMatchAll)
        {
            IQueryable<Product> query = _db.Products.Where(product => product.Store.Id == storeId);

            foreach (var condition in conditions)
            {
                query = ApplyFilter(query, condition);
            }
            if (isMatchAll) query = query.Distinct();

            try
            {
                await query.ExecuteUpdateAsync(product => product
                     .SetProperty(p => p.CategoryId, categoryId)
                     .SetProperty(p => p.UpdatedAt, DateTime.UtcNow));
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);

            }

        }


        private IQueryable<Product> ApplyFilter(IQueryable<Product> query, CreateCategoryCondition condition)
        {
            switch (condition.ProductProperty)
            {
                case "PRODUCT_NAME":
                    if (condition.Condition == "EQUAL")
                        query = query.Where(product => product.Name == condition.Value);
                    else if (condition.Condition == "CONTAINS")
                        query = query.Where(product => product.Name.Contains(condition.Value));
                    else if (condition.Condition == "NOT_CONTAINS")
                        query = query.Where(product => !product.Name.Contains(condition.Value));
                    else if (condition.Condition == "START_WITH")
                        query = query.Where(product => product.Name.StartsWith(condition.Value));
                    break;

            }
            return query;
        }

    }
}

﻿using Lemoo_pos.Areas.Api.Dto;
using Lemoo_pos.Models.ViewModels;

namespace Lemoo_pos.Services.Interfaces
{
    public interface IProductService
    {
        Task CreateProduct(CreateProductViewModel model, IFormFile image);

        List<ProductResponseViewModel> GetAllProduct();

        List<ProductVariantResponseViewModel> GetAllVariants(long productId);

        ProductVariantDetailResponseViewModel GetProductVariantByIdAndProductID(long variantId, long productId);

        Task UpdateProductVariant(long productId, long variantId, UpdateProductVariantViewModel model, IFormFile image);

        void DeleteProduct(long productId);

        List<TopProductViewModel> GetTopProducts(int limit);

    }
}

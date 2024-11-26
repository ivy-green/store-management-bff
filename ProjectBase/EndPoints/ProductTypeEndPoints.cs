using ProjectBase.Domain.Constants;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.Interfaces.IServices;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.EndPoints
{
    [ExcludeFromCodeCoverage]
    public static class ProductTypeEndPoints
    {
        public static void MapProductTypePoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/productType");

            group.MapGet("", GetList);

            group.MapPost("", AddProductType);

            group.MapPut("", UpdateProductType);

            group.MapDelete("{id}", RemoveProductType);
        }

        public static async Task<IResult> GetList(int pageIndex, int pageSize, IProductTypeService _ProductTypeService)
        {
            var res = await _ProductTypeService.GetPagedList(pageSize, pageIndex);
            return res.IsSuccess
                ? Results.Ok(res.Value)
                : Results.BadRequest(res.Error);
        }

        public static async Task<IResult> AddProductType(ProductTypeCreateDTO data, IProductTypeService _ProductTypeService)
        {
            var res = await _ProductTypeService.AddProductType(data);
            return res.IsSuccess
                ? Results.Ok(ResponseMessages.SUCCESS)
                : Results.BadRequest(res.Error);
        }

        public static async Task<IResult> UpdateProductType(ProductTypeUpdateDTO data, IProductTypeService _ProductTypeService)
        {
            var res = await _ProductTypeService.UpdateProductType(data);
            return res.IsSuccess
                ? Results.Ok(ResponseMessages.SUCCESS)
                : Results.BadRequest(res.Error);
        }

        public static async Task<IResult> RemoveProductType(int id, IProductTypeService _ProductTypeService)
        {
            var res = await _ProductTypeService.RemoveProductType(id);
            return res.IsSuccess
                ? Results.Ok(ResponseMessages.SUCCESS)
                : Results.BadRequest(res.Error);
        }
    }
}

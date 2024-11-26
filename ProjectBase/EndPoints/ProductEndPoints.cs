using Polly.Registry;
using ProjectBase.Domain.Constants;
using ProjectBase.Domain.DTOs.Requests;
using ProjectBase.Domain.Interfaces.IServices;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.EndPoints
{
    [ExcludeFromCodeCoverage]
    public static class ProductEndPoints
    {
        public static void MapProductPoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/product").RequireAuthorization("ProductManagement");

            group.MapGet("", GetList);

            group.MapGet("{code}", GetList);

            group.MapGet("market", GetMarketList);

            group.MapPost("", AddProduct);

            group.MapPut("", UpdateProduct);

            group.MapPut("sale", UpdateProductOnSale);

            group.MapDelete("{code}", RemoveProduct);
        }

        public static async Task<IResult> GetList(
            int pageIndex, int pageSize,
            IProductService _productService,
            HttpContext _httpContext,
            ResiliencePipelineProvider<string> pipelineProvider,
            string searchString = "",
            int categoryCode = 0)
        {
            var policy = pipelineProvider.GetPipeline("gh-null-retry");
            var response = await policy.ExecuteAsync(async token =>
            {
                var currUsername = BaseController.GetCurrentUsername(_httpContext);

                return await _productService.SearchV2(
                    pageIndex,
                    pageSize,
                    currUsername,
                    searchString,
                    categoryCode);
            }, CancellationToken.None);

            return response.IsSuccess
                ? Results.Ok(response.Value)
                : Results.BadRequest(response.Error);
        }

        public static async Task<IResult> GetMarketList(
            int pageIndex, int pageSize,
            IProductService _productService,
            ResiliencePipelineProvider<string> pipelineProvider)
        {
            var policy = pipelineProvider.GetPipeline("gh-null-retry");
            var response = await policy.ExecuteAsync(async token =>
            {
                return await _productService.Search(pageIndex, pageSize);
            }, CancellationToken.None);

            return response.IsSuccess
                ? Results.Ok(response.Value)
                : Results.BadRequest(response.Error);
        }

        public static async Task<IResult> AddProduct(
            ProductCreateDTO data,
            HttpContext _httpContext,
            IProductService _productService)
        {
            var currUsername = BaseController.GetCurrentUsername(_httpContext);
            if (currUsername is null)
            {
                return Results.Unauthorized();
            }

            var res = await _productService.AddProduct(data, currUsername);
            return res.IsSuccess
                ? Results.Ok(ResponseMessages.SUCCESS)
                : Results.BadRequest(res.Error);
        }

        public static async Task<IResult> UpdateProduct(
            ProductCreateDTO data,
            HttpContext _httpContext,
            IProductService _productService)
        {
            var currUsername = BaseController.GetCurrentUsername(_httpContext);
            if (currUsername is null)
            {
                return Results.Unauthorized();
            }

            var res = await _productService.UpdateProduct(data, currUsername);
            return res.IsSuccess
                ? Results.Ok(ResponseMessages.SUCCESS)
                : Results.BadRequest(res.Error);
        }

        public static async Task<IResult> UpdateProductOnSale(
            int productCode,
            HttpContext _httpContext,
            IProductService _productService)
        {
            var currUsername = BaseController.GetCurrentUsername(_httpContext);
            if (currUsername is null)
            {
                return Results.Unauthorized();
            }

            var res = await _productService.UpdateOnSaleState(productCode, currUsername);
            return res.IsSuccess
                ? Results.Ok(ResponseMessages.SUCCESS)
                : Results.BadRequest(res.Error);
        }

        public static async Task<IResult> RemoveProduct(
            int code,
            HttpContext _httpContext,
            IProductService _productService)
        {
            var currUsername = BaseController.GetCurrentUsername(_httpContext);
            if (currUsername is null)
            {
                return Results.Unauthorized();
            }

            var res = await _productService.RemoveProduct(code, currUsername);
            return res.IsSuccess
                ? Results.Ok(ResponseMessages.SUCCESS)
                : Results.BadRequest(res.Error);
        }
    }
}

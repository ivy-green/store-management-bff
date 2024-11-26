
using ProjectBase.Domain.Abstractions;
using System.Net;

namespace ProjectBase.Domain.Errors
{
    public sealed record ProductError(
        string Code,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        string? Description = "")
    {
        public static readonly Error ProductNotFound = new(
            "PRODUCT_NOT_FOUND",
            Description: "Product not found");

        public static readonly Error ProductExists = new(
            "PRODUCT_EXISTS",
            Description: "Product exists!");

        public static readonly Error ProductTypeNotFound = new(
            "PRODUCT_TYPE_NOT_FOUND",
            Description: "Product Type not found!");

        public static readonly Error NegativeData = new(
            "NEGATIVE_DATA",
            Description: "Quantity and Price cannot be negative!");

        public static readonly Error PriceLessThanThousand = new(
            "PRICE_INVALID",
            Description: "Price must be at least 1.000VND!");

        public static readonly Error NameNotFound = new(
            "NAME_NOT_FOUND",
            Description: "Name is missing!");

        public static readonly Error BranchCodeNotFound = new(
            "BRANCH_CODE_NOT_FOUND",
            Description: "Branch's code is missing!");

        public static readonly Error BranchNotFound = new(
            "BRANCH_NOT_FOUND",
            Description: "Branch is not found!");
    }
}

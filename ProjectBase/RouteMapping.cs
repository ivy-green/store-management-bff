using ProjectBase.EndPoints;

namespace ProjectBase
{
    public static class RouteMapping
    {
        public static void AddRouteMapping(this IEndpointRouteBuilder app)
        {
            app.MapProductTypePoints();
            app.MapProductPoints();
            app.MapUserPoints();
            app.MapStatisticPoints();
            app.MapBranchPoints();
            app.MapBillPoints();
            app.MapAmazonPoints();
            //app.MapAzurePoints();
            //app.MapAuthPoints();
        }
    }
}

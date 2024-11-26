namespace ProjectBase.Domain.Entities
{
    public class ProductCoupon
    {
        public virtual string ProductId { get; set; } = string.Empty;
        public virtual string CouponId { get; set; } = string.Empty;
        public virtual Product? Product { get; set; }
        public virtual Coupon? Coupon { get; set; }
    }
}

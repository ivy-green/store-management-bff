using ProjectBase.Domain.Abstractions;

namespace ProjectBase.Domain.Enums
{
    public abstract class CustomerType : Enumeration<CustomerType>
    {
        public static readonly CustomerType Copper = new CopperCustomer();
        public static readonly CustomerType Silver = new SilverCustomer();
        public static readonly CustomerType Gold = new GoldCustomer();
        protected CustomerType(int value, string name)
            : base(value, name) { }
        public abstract double Discount { get; }
        public abstract CustomerType NextType { get; }

        private sealed class CopperCustomer : CustomerType
        {
            public CopperCustomer() : base(1, "Copper") { }
            public override double Discount => 0.01;

            public override CustomerType NextType => new SilverCustomer();
        }

        private sealed class SilverCustomer : CustomerType
        {
            public SilverCustomer() : base(2, "Silver") { }
            public override double Discount => 0.04;

            public override CustomerType NextType => new GoldCustomer();
        }

        private sealed class GoldCustomer : CustomerType
        {
            public GoldCustomer() : base(3, "Gold") { }
            public override double Discount => 0.15;

            public override CustomerType NextType => throw new NotImplementedException();
        }
    }
}

namespace ProjectBase.Jobs.Core.Entities
{
    public class StatisticBill
    {
        public double Revenue { get; set; } // bill's total
        public DateOnly Date { get; set; }
        public int BillQuantity { get; set; }
        public int ProductQuantity { get; set; }
    }
}

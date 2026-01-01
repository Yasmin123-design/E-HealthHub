namespace E_PharmaHub.Dtos
{
    public class OrdersSummaryDto
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ConfirmedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int DelieveredOrders { get; set; }
        public decimal TotalRevenue { get; set; } 


    }
}

namespace StoreApi.Models
{
    public class OrderDetailsModel
    {
        public string BasketId { get; set;}
        public string Address { get; set;}
        public List<BasketModel> Goods { get; set;}
        public string CardNumber { get; set;}
        public List<OrderStatusHistory> StatusHistory { get; set;}

    }
}

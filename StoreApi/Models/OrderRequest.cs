namespace StoreApi.Models
{
    public class OrderRequest
    {
        public string OrderId { get; set; }
        public string BasketId { get; set; }
        public string Address { get; set; }
        public string CardNumber { get; set; }
    }
}

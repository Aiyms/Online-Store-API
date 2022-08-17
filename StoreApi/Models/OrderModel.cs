namespace StoreApi.Models
{
    public class OrderModel
    {
        public string OrderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string StatusName { get; set; }
    }
}

namespace StoreApi.Models
{
    public class BasketModel
    {
        public string GoodId  { get; set; }
        public string Title { get; set;}
        public int Quantity { get; set;}
        public Decimal Amount { get; set;}

    }
}

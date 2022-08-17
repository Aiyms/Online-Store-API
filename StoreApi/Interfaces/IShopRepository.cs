using StoreApi.Models;

namespace StoreApi.Interfaces
{
    public interface IShopRepository
    {
         public ApiResponse<List<GoodsModel>> GetAllGoods();
         public ApiResponse<string> AddProductToBasket(string GoodId);
         public ApiResponse<string> ChangeProductQuantityInBasket(string GoodId, int Quantity, string BasketId);
         public ApiResponse<List<BasketModel>> GetBasket(string BasketId);
         public ApiResponse<string> CreateOrder(string BasketId/*OrderRequest request*/);
         public ApiResponse<string> PostOrderPayment(OrderRequest order);
         public ApiResponse<List<OrderModel>> GetOrders();
         public ApiResponse<OrderDetailsModel> GetOrderDetails(string OrderId);
         public ApiResponse<string> ChangeOrderStatus(string OrderId, int StatusId);
    }
}

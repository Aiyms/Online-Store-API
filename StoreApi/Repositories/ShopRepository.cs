using Newtonsoft.Json;
using StoreApi.Interfaces;
using StoreApi.Models;
using System.Data.SqlClient;

namespace StoreApi.Repositories
{
    public class ShopRepository: IShopRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ShopRepository(IConfiguration configuration) 
        {
            this._configuration = configuration;
            this._connectionString = configuration.GetConnectionString("Connection");
        }

        public ApiResponse<List<GoodsModel>> GetAllGoods() 
        {
            ApiResponse<List<GoodsModel>> result = new ApiResponse<List<GoodsModel>>();
            List<GoodsModel> Goods = new List<GoodsModel>();
            try
            {
                 using (SqlConnection conn = new SqlConnection(_connectionString))
                 {
                     conn.Open();
                     using (SqlCommand cmd = new SqlCommand("dbo.p_GetAllGoods", conn))
                     {
                         cmd.CommandType = System.Data.CommandType.StoredProcedure;
                         using (SqlDataReader reader = cmd.ExecuteReader()) 
                         {
                             if (reader.HasRows)
                             {
                                 while (reader.Read()) 
                                 { 
                                     GoodsModel product = new GoodsModel();
                                     product.Id = reader["Id"].ToString();
                                     product.Title = reader["Title"].ToString();
                                     product.Price = reader["Price"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Price"]);
                                     Goods.Add(product);
                                 }
                             }
                             else
                             {
                                 result.Code = -1;
                                 result.Data = null;
                                 result.Message = "Товаров нет в в базе данных!";
                             }
                         }
                     }
                 }
                 result.Data = Goods;
            }
            catch (Exception ex) 
            {
                result.Code = -1;
                result.Data = null;
                result.Message = ex.Message;

            }
            return result;
        }
        public ApiResponse<string> AddProductToBasket(string GoodId) 
        {
            string BasketId;
            ApiResponse<string> result = new ApiResponse<string>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.p_AddProductToBasket", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@GoodId", GoodId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read() && reader.HasRows)
                            {
                                BasketId =  reader["NewBasketId"].ToString();
                                result.Data = BasketId;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = -1;
                result.Data = "";
                result.Message = ex.Message;

            }
            return result;
        }

        public ApiResponse<string> ChangeProductQuantityInBasket(string GoodId, int Quantity, string BasketId) 
        {
            ApiResponse<string> result = new ApiResponse<string>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.p_ChangeProductQuantityInBasket", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@GoodId", GoodId);
                        cmd.Parameters.AddWithValue("@Quantity", Quantity);
                        cmd.Parameters.AddWithValue("@BasketId", BasketId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = -1;
                result.Message = ex.Message;

            }
            return result;
        }

        public ApiResponse<List<BasketModel>> GetBasket(string BasketId)
        {
            ApiResponse<List<BasketModel>> result = new ApiResponse<List<BasketModel>>();
            List<BasketModel> basket = new List<BasketModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.p_GetBasket", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@BasketId", BasketId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    BasketModel model = new BasketModel();
                                    model.GoodId = reader["GoodId"].ToString();
                                    model.Title = reader["Title"].ToString();
                                    model.Quantity = Convert.ToInt32(reader["Quantity"]);
                                    model.Amount = Convert.ToDecimal(reader["Amount"]);
                                    basket.Add(model);
                                }
                            }
                        }
                    }
                }
                result.Data = basket;
            }
            catch (Exception ex)
            {
                result.Code = -1;
                result.Data = null;
                result.Message = ex.Message;
            }
            return result;
        }

        public ApiResponse<string> CreateOrder(string BasketId/*OrderRequest request*/) 
        {
            ApiResponse<string> result = new ApiResponse<string>();
            string OrderId;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.p_CreateOrder", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@BasketId", BasketId /*request.BasketId*/);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read() && reader.HasRows)
                            {
                                OrderId = reader["OrderId"].ToString();
                                result.Data = OrderId;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = -1;
                result.Message = ex.Message;

            }
            return result;
        }

        public ApiResponse<List<OrderModel>> GetOrders() 
        {
            ApiResponse<List<OrderModel>> result = new ApiResponse<List<OrderModel>>();
            List<OrderModel> orders = new List<OrderModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.p_GetOrders", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read() && reader.HasRows)
                            {
                                OrderModel model = new OrderModel();
                                model.OrderId = reader["OrderId"].ToString();
                                model.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);
                                model.StatusName = reader["StatusName"].ToString();
                                orders.Add(model);
                            }
                        }
                    }
                }
                result.Data = orders;
            }
            catch (Exception ex)
            {
                result.Code = -1;
                result.Message = ex.Message;
            }
            return result;
        }

        public ApiResponse<OrderDetailsModel> GetOrderDetails(string OrderId) 
        {
            ApiResponse<OrderDetailsModel> result = new ApiResponse<OrderDetailsModel>();
            OrderDetailsModel order = new OrderDetailsModel();
            List<OrderStatusHistory> history = new List<OrderStatusHistory>();
            ApiResponse<List<BasketModel>> goods = new ApiResponse<List<BasketModel>>();
            List<BasketModel> goodsModel = new List<BasketModel>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.p_GetOrderDetails", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OrderId", OrderId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read() && reader.HasRows)
                            {
                                order.BasketId = reader["BasketId"].ToString();
                                order.Address = reader["Address"].ToString();
                                order.CardNumber = reader["CardNumber"].ToString();
                            }
                        }
                    }
                    using (SqlCommand cmd = new SqlCommand("dbo.p_GetOrderDetailsHistory", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OrderId", OrderId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read()) 
                            {
                                if (reader.HasRows)
                                {
                                    OrderStatusHistory item = new OrderStatusHistory();
                                    item.StatusName = reader["StatusName"].ToString();
                                    item.Date = Convert.ToDateTime(reader["Date"]);
                                    history.Add(item);
                                }
                            }
                        }
                    }
                }
                order.Goods = GetBasket(order.BasketId).Data;
                order.StatusHistory = history;
                result.Data = order;
            }
            catch (Exception ex)
            {
                result.Code = -1;
                result.Message = ex.Message;

            }
            return result;
        }

        public ApiResponse<string> ChangeOrderStatus(string OrderId, int StatusCode) 
        {
            ApiResponse<string> result = new ApiResponse<string>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.p_UpdateOrderStatus", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OrderId", OrderId);
                        cmd.Parameters.AddWithValue("@StatusCode", StatusCode);
                        cmd.ExecuteNonQuery();
                        result.Data = "Статус изменен";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = -1;
                result.Message = ex.Message;
            }
            return result;
        }

        public ApiResponse<string> PostOrderPayment(OrderRequest order)
        {
            ApiResponse<string> result = new ApiResponse<string>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.p_PostOrderPayment", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@OrderId", order.OrderId);
                        cmd.Parameters.AddWithValue("@BasketId", order.BasketId);
                        cmd.Parameters.AddWithValue("@Address", order.Address);
                        cmd.Parameters.AddWithValue("@CardNumber", order.CardNumber);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = -1;
                result.Message = ex.Message;
            }
            return result;
        }
    }
}

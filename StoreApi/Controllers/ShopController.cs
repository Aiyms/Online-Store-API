using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StoreApi.Interfaces;
using StoreApi.Models;

namespace StoreApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class ShopController : ControllerBase
    {
        private readonly IShopRepository _shopRepository;

        public ShopController(IShopRepository repository)
        {
            _shopRepository = repository;
        }

        [HttpGet("[action]")]
        public IActionResult GetAllGoods()
        {
            ApiResponse<List<GoodsModel>> result = _shopRepository.GetAllGoods();
            return Ok(result);
        }

        [HttpPost("[action]")]
        public IActionResult AddProductToBasket(string GoodId)
        {
            ApiResponse<string> result = _shopRepository.AddProductToBasket(GoodId);
            return Ok(result);
        }

        [HttpPut("[action]")]
        public IActionResult ChangeProductQuantityInBasket(string GoodId, int Quantity, string BasketId)
        {
            ApiResponse<string> result = _shopRepository.ChangeProductQuantityInBasket(GoodId, Quantity, BasketId);
            return Ok(result);
        }

        [HttpGet("[action]")]
        public IActionResult GetBasket(string BasketId)
        {
            ApiResponse<List<BasketModel>> result = _shopRepository.GetBasket(BasketId);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public IActionResult CreateOrder(string BasketId/*[FromBody] OrderRequest request*/)
        {
            ApiResponse<string> result = _shopRepository.CreateOrder(BasketId/*request*/);
            return Ok(result);
        }

        [HttpGet("[action]")]
        public IActionResult GetOrders() 
        {
            ApiResponse<List<OrderModel>> result = _shopRepository.GetOrders();
            return Ok(result);
        }

        [HttpGet("[action]")]
        public IActionResult GetOrderDetails(string OrderId) 
        {
            ApiResponse<OrderDetailsModel> result = _shopRepository.GetOrderDetails(OrderId);
            return Ok(result);
        }

        [HttpPut("[action]")]
        public IActionResult ChangeOrderStatus(string OrderId, int StatusCode)
        {
            ApiResponse<string> result = _shopRepository.ChangeOrderStatus(OrderId, StatusCode);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public IActionResult PostOrderPayment([FromBody]OrderRequest request)
        {
            ApiResponse<string>  result = _shopRepository.PostOrderPayment(request);
            return Ok(result);
        }

    }
}
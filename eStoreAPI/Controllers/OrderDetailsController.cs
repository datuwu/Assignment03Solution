using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using DataAccess.UnitOfWork;

namespace EBookstoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ODataController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();
        public OrderDetailsController()
        {

        }

        // GET: api/OrderDetails
        [EnableQuery]
        [HttpGet]
        public IActionResult GetAll()
        {
            var list = _unitOfWork.OrderDetailRepository.Get();
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        // GET: api/OrderDetails/ById?Id=5
        [EnableQuery]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int productId, int orderId)
        {
            var item = await _unitOfWork.OrderDetailRepository.GetByIDAsync(new object[productId, orderId]);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }


        // PUT: api/OrderDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [EnableQuery]
        [HttpPut("/product/{productId}/order/{orderId}")]
        public async Task<IActionResult> Put(int productId, int orderId, OrderDetail orderDetail)
        {
            if (productId != orderDetail.ProductId || orderId != orderDetail.OrderId)
            {
                return BadRequest();
            }

            try
            {
                _unitOfWork.OrderDetailRepository.Update(orderDetail);
                await _unitOfWork.SaveAsync();

                if (!IsExists(productId, orderId))
                {
                    return NotFound();
                }
            }
            catch
            {
                return BadRequest();
            }

            return NoContent();
        }

        // POST: api/OrderDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> Post(OrderDetail orderDetail)
        {

            try
            {
                await _unitOfWork.OrderDetailRepository.Insert(orderDetail);
                await _unitOfWork.SaveAsync();
            }
            catch (DbUpdateException)
            {
                if (IsExists(orderDetail.ProductId, orderDetail.OrderId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Post", new { bookId = orderDetail.ProductId, authorId = orderDetail.OrderId }, orderDetail);
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int productId, int orderId)
        {
            var orderDetail = await _unitOfWork.OrderDetailRepository.GetByIDAsync(new object[productId, orderId]);

            if (orderDetail == null)
            {
                return NotFound();
            }

            _unitOfWork.OrderDetailRepository.Delete(orderDetail);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        private bool IsExists(int productId, int orderId)
        {
            var list = _unitOfWork.OrderDetailRepository.Get();
            return (list.ToList().Any(e => e.ProductId == productId && e.OrderId == orderId));
        }
    }
}
using BusinessObject;
using DataAccess.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace eStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ODataController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public OrdersController()
        {
        }

        // GET: api/Orders
        [EnableQuery]
        [HttpGet]
        public IActionResult Get()
        {
            var list = _unitOfWork.OrderRepository.Get();
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        // GET: api/Orders/5
        [EnableQuery]
        [HttpGet("{key}")]
        public async Task<IActionResult> GetById(int key)
        {
            var item = await _unitOfWork.OrderRepository.GetByIDAsync(key);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }


        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest();
            }

            try
            {
                _unitOfWork.OrderRepository.Update(order);
                await _unitOfWork.SaveAsync();

                if (!IsExists(id))
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

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> Post(Order order)
        {

            try
            {
                await _unitOfWork.OrderRepository.Insert(order);
                await _unitOfWork.SaveAsync();
            }
            catch (DbUpdateException)
            {
                if (IsExists(order.OrderId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Post", new { id = order.OrderId }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _unitOfWork.OrderRepository.GetByIDAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _unitOfWork.OrderRepository.Delete(order);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        private bool IsExists(int id)
        {
            var list = _unitOfWork.OrderRepository.Get();
            return (list.ToList().Any(e => e.OrderId == id));
        }
    }
}


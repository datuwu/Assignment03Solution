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
    public class ProductsController : ODataController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();

        public ProductsController()
        {
        }

        // GET: api/Products
        [EnableQuery]
        [HttpGet]
        public IActionResult Get()
        {
            var list = _unitOfWork.ProductRepository.Get();
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        // GET: api/Products/5
        [EnableQuery]
        [HttpGet("{key}")]
        public async Task<IActionResult> GetById(int key)
        {
            var item = await _unitOfWork.ProductRepository.GetByIDAsync(key);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }


        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            try
            {
                _unitOfWork.ProductRepository.Update(product);
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> Post(Product product)
        {

            try
            {
                await _unitOfWork.ProductRepository.Insert(product);
                await _unitOfWork.SaveAsync();
            }
            catch (DbUpdateException)
            {
                if (IsExists(product.ProductId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Post", new { id = product.ProductId }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIDAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _unitOfWork.ProductRepository.Delete(product);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        private bool IsExists(int id)
        {
            var list = _unitOfWork.ProductRepository.Get();
            return (list.ToList().Any(e => e.ProductId == id));
        }
    }
}


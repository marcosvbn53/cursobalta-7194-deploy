using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductyController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get([FromServices] DataContext context)
        {
            var products = await context
            .Products
            .Include(px => px.Category)
            .AsNoTracking()
            .ToListAsync();

            if (products == null)
                return BadRequest(products);

            return Ok(products);
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById(
            int id,
            [FromServices] DataContext context
        )
        {
            var produto = await context
            .Products
            .Include(px => px.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(px => px.Id == id);

            if (produto == null)
                return BadRequest(produto);

            return Ok(produto);
        }

        [HttpGet]
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategory(
            int id,
            [FromServices] DataContext context)
        {
            var products = await context
            .Products
            .Include(px => px.Category)
            .AsNoTracking()
            .Where(pw => pw.CategoryId == id)
            .ToListAsync();

            if (products == null)
                return BadRequest(new { message = "Nenhum produto encontrado!" });


            return Ok(products);
        }


        [HttpPost]
        [Route("")]
        [Authorize(Roles = "clientes")]
        public async Task<ActionResult<Product>> Post(
            [FromBody]Product model,
            [FromServices]DataContext context)
        {

            if(ModelState.IsValid)
            {
                context.Products.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }

            return BadRequest(ModelState);
        }




    }
}

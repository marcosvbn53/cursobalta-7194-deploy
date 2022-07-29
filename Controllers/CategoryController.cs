using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;


namespace Shop.Controllers
{
    //https://localhost:5001
    //http://localhost:5000
    //https://localhost:5001/categories
    [ApiController]
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        //Para fazer um cache por método 
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 20)]
        //Para definir que dentre todos os métodos um método não será cacheado, utilize a linha abaixo
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ActionResult<List<Category>>> Get([FromServices] DataContext context)
        {
            var categoria = await context.Categories.AsNoTracking().ToListAsync();
            return Ok(categoria);
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(
            int id,
            [FromServices] DataContext context)
        {
            var categoria = await context.Categories.FirstOrDefaultAsync(px => px.Id == id);
            if (categoria != null)
            {
                return Ok(categoria);
            }
            else
            {
                return BadRequest(new { message = "Categoria não encontrada!" });
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "clientes")]
        public async Task<ActionResult<List<Category>>> Post(
            [FromBody] Category model,
            [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                context.Categories.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível criar a categoria!" });
            }

        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "Clientes")]
        public async Task<ActionResult<Category>> Put(
            int id,
            [FromBody] Category model,
            [FromServices] DataContext context)
        {
            if (model.Id != id)
            {
                return NotFound(new { message = "Categoria não encontrada!" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Este registro já foi atualizado!" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível atualizar a categoria" });
            }
        }

        [HttpDelete]
        [Route("")]
        [Authorize(Roles = "clientes")]
        public async Task<ActionResult<Category>> Delete(
            int id,
            [FromServices] DataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(px => px.Id == id);
            if (category == null)
                return NotFound(new { message = "Categoria não encontrado" });

            try
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(new { message = "Categoria removida com sucesso!" });

            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível remover a categoria" });
            }
        }
    }
}
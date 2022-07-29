using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Shop.Services;

namespace Shop.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : Controller
    {

        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get([FromServices] DataContext context)
        {
            var users = await context
                              .Users
                              .AsNoTracking()
                              .ToListAsync();

            return users;
        }


        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        //[Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Post(
            [FromServices] DataContext context,
            [FromBody] User model)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            bool itemExist = context.Users.Any(px => px.Username == model.Username);
            if(itemExist)
                return BadRequest(new { message = "Usuário já existe!" });

            try
            {
                model.Role = "clientes";
                context.Users.Add(model);
                await context.SaveChangesAsync();
                model.Password = "";
                return model;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });
            }
        }


        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate(
            [FromServices] DataContext context,
            [FromBody] User model
        )
        {
            var user = await context.Users
            .AsNoTracking()
            .Where(px => px.Username == model.Username && px.Password == model.Password)
            .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { message = "Usuário ou senha invalidos" });

            var token = TokenService.GenerateToken(user);
            user.Password = "";

            return new{
                user = user,
                token = token
            };
        }

        // [HttpGet]
        // [Route("anonimo")]
        // [AllowAnonymous]
        // public string Anonimo() => "Anonimo";

        // [HttpGet]
        // [Route("autenticado")]
        // [Authorize]
        // public string Autenticado() => "Autenticado";

        // [HttpGet]
        // [Route("funcionario")]
        // [Authorize(Roles = "clientes")]
        // public string Funcionario() => "Funcionario";

        // [HttpGet]
        // [Route("gerente")]
        // [Authorize(Roles = "manager")]
        // public string Gerente() => "Gerente";





    }
}
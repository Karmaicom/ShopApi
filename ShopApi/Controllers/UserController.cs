using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Models;
using ShopApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApi.Controllers
{
    [Route("v1/user")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get([FromServices] DataContext context)
        {
            try
            {
                var users = await context.Users.AsNoTracking().ToListAsync();

                if (users == null)
                    return NotFound(new { message = "Nenhum usuário encontrado!" });

                return Ok(users);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "Não foi possível listar os usuários!", error = $"Erro: {e.Message}" });
            }
        
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        //[Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Post(
            [FromServices] DataContext context,
            [FromBody] User model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Força o usuário a ser sempre "funcionário"
                model.Role = "employee";

                context.Users.Add(model);
                await context.SaveChangesAsync();

                // Esconder a senha
                model.Password = "";
                return model;
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "Não foi possível criar o usuário!", error = $"Erro {e.Message}" });
            }

        }

        [HttpPut]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put(
            int id,
            [FromBody] User model,
            [FromServices] DataContext context)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (id != model.Id)
                    return NotFound(new { message = "Usuário não encontrado!" });

                context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();

                // Esconder a senha
                model.Password = "";
                return model;
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "Não foi possível atualizar o usuário!", error = $"Erro {e.Message}" });
            }

        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate(
            [FromServices] DataContext context,
            [FromBody] User model)
        {

            try
            {
                var user = await context.Users.AsNoTracking()
                    .Where(x => x.Username == model.Username && x.Password == model.Password)
                    .FirstOrDefaultAsync();

                if (user == null)
                    return NotFound(new { message = "Usuário ou senha inválidos!" });

                var token = TokenService.GenerateToken(user);

                // Esconder a senha
                model.Password = "";

                return new
                {
                    user = user,
                    token = token
                };
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "Não possível efetuar o login!", error = $"Erro: {e.Message}" });
            }

        }

        // Exemplos utilizando Authorization
        [HttpGet]
        [Route("anonimo")]
        [AllowAnonymous]
        public string Anonimo() => "Anonimo";

        [HttpGet]
        [Route("autenticado")]
        [Authorize]
        public string Autenticado() => "Autenticado";

        [HttpGet]
        [Route("funcionario")]
        [Authorize(Roles = "employee")]
        public string Funcionario() => "Funcionario";

        [HttpGet]
        [Route("gerente")]
        [Authorize(Roles = "manager")]
        public string Gerente() => "Gerente";

    }
}

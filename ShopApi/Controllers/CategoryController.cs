using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopApi.Controllers
{
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.Any, NoStore = true)]
        public async Task<ActionResult<List<Category>>> Get(
            [FromServices] DataContext context)
        {
            try
            {
                var categories = await context.Categories.AsNoTracking().ToListAsync();

                if (categories == null)
                    return NotFound(new { message = "Nenhuma categoria foi encontrada!" });

                return Ok(categories);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = $"Não foi possível listar todas as categorias.", error = $"Erro: { e.Message}" });
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(
            int id,
            [FromServices] DataContext context)
        {
            try
            {
                var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

                if (category == null)
                    return NotFound(new { message = "Categoria não encontrada!" });

                return Ok(category);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = $"Não foi possível listar todas as categorias.", error = $"Erro: { e.Message}" });
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Post(
            [FromBody] Category category,
            [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Categories.Add(category);
                await context.SaveChangesAsync();
                return Ok(category);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = $"Não foi possível criar a categoria.", error = $"Erro: { e.Message}" });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Put(
            int id, 
            [FromBody] Category category,
            [FromServices] DataContext context)
        {
            if (category.Id != id)
                return NotFound(new { message = "Categoria não encontrada!"});

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Category>(category).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return category;
            }
            catch (DbUpdateConcurrencyException e)
            {
                return BadRequest(new { message = $"Esta já foi atualizada.", error = $"Erro: { e.Message}" });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = $"Não foi possível atualizar categoria.", error = $"Erro: { e.Message}" });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Delete(
            int id,
            [FromServices] DataContext context)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                if (category == null)
                {
                    return NotFound(new { message = "Categoria ão encontrada!" });
                }

                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(new { message = "Categoria removida!" });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = $"Não foi possível remover a categoria.", error = $"Erro: { e.Message}" });
            }
        }
    }
}

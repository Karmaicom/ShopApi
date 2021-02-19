
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApi.Controllers
{
    [Route("products")]
    public class ProductController : ControllerBase
    {

        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get([FromServices] DataContext context)
        {
            try
            {
                var products = await context.Products.Include(x => x.Category).AsNoTracking().ToListAsync();

                if (products == null)
                {
                    return NotFound(new { message = $"Nenhum produto foi encontrado!" });
                }

                return Ok(products);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "Não foi possível listar todos os produtos.", error = $"Erro: {e.Message}" });
            }
        }

        [HttpGet]
        [Route("id:int")]
        [AllowAnonymous]
        public ActionResult<Product> GetById(int id, [FromServices] DataContext context)
        {
            try
            {
                var product = context.Products.Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

                if (product == null)
                {
                    return NotFound(new { message = $"Nenhum produto foi encontrado!" });
                }

                return Ok(product);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "Produto não encontrado", error = $"Erro: {e.Message}" });
            }
        }

        [HttpGet]
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategoryId(int id, [FromServices] DataContext context)
        {
            try
            {
                var products = await context.Products.Include(x => x.Category).AsNoTracking().Where(x => x.CategoryId == id).ToListAsync();

                if (products == null)
                    return NotFound(new { message = $"Nenhum produto foi encontrado para esta categoria!" });

                return Ok(products);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "Não foi possível listar as produtos dessa categoria.", error = $"Erro: {e.Message}" });
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Post([FromBody] Product product, [FromServices] DataContext context)
        {

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                context.Products.Add(product);
                await context.SaveChangesAsync();

                return Ok(product);

            }
            catch (Exception e)
            {
                return BadRequest(new { message = "Não foi possível criar o produto!", error = $"Erro: {e.Message}" });
            }

        }

    }
}

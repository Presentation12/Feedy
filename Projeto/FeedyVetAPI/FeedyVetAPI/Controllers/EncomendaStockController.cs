using FeedyVetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace FeedyVetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncomendaStockController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public EncomendaStockController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Get

        /// <summary>
        /// Lista a informação de todos os produtos das encomendas
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize(Roles = "Admin")]
        public IEnumerable<EncomendaStock> Get()
        {
            using (var context = new FeedyVetContext())
            {
                return context.EncomendaStock.ToList();
            }
        }
        #endregion

        #region Post

        /// <summary>
        /// Envia dados da relação Encomenda <-> StockEstabelecimento para a BD
        /// </summary>
        /// <param name="enc"> EncomendaStock </param>
        /// <returns> Mensagem do sucedido </returns>
        [HttpPost, Authorize(Roles = "Admin, Cliente")]
        public IActionResult Post(EncomendaStock enc)
        {
            using (var context = new FeedyVetContext())
            {
                EncomendaStock encomendaStock = new EncomendaStock();

                try
                {
                    encomendaStock.IdEncomendaNavigation = context.Encomenda.Where(e => e.IdEncomenda == enc.IdEncomenda).FirstOrDefault();
                    encomendaStock.IdStockNavigation = context.StockEstabelecimento.Where(e => e.IdStock == enc.IdStock).FirstOrDefault();
                    encomendaStock.Qtd = enc.Qtd;

                    context.EncomendaStock.Add(encomendaStock);

                    context.SaveChanges();
                    return Ok();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// Atualiza a informacao de uma certa EncomendaStock na base de dados
        /// </summary>
        /// <param name="encomendaStock"></param>
        /// <returns>  </returns>
        [HttpPatch, Authorize(Roles = "Admin")]
        public IActionResult Patch(EncomendaStock encomendaStock)
        {
            using(var context = new FeedyVetContext())
            {
                EncomendaStock es = context.EncomendaStock
                    .Where(e => e.IdStock == encomendaStock.IdStock && e.IdEncomenda == encomendaStock.IdEncomenda).FirstOrDefault();

                try
                {
                    if(encomendaStock.IdEncomenda != 0) es.IdEncomendaNavigation = context.Encomenda.Where(e => e.IdEncomenda == encomendaStock.IdEncomenda).FirstOrDefault();
                    if(encomendaStock.IdStock != 0) es.IdStockNavigation = context.StockEstabelecimento.Where(e => e.IdStock == encomendaStock.IdStock).FirstOrDefault();
                    es.Qtd = encomendaStock.Qtd;


                    context.SaveChanges();
                    return Ok();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }
            }
        }
        #endregion

        #region Delete

        /// <summary>
        /// Apaga um produto de uma encomenda
        /// </summary>
        /// <param name="enc"></param>
        /// <returns></returns>
        [HttpDelete, Authorize(Roles = "Admin")]
        public IActionResult Delete(EncomendaStock enc)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    context.EncomendaStock.Remove(context.EncomendaStock
                        .Where(e => e.IdStock == enc.IdStock && e.IdEncomenda == enc.IdEncomenda).FirstOrDefault());

                    context.SaveChanges();
                }

                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }
        #endregion
    }
}

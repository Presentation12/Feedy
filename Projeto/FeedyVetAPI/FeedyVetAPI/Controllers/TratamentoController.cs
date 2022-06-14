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
using System.Security.Claims;

namespace FeedyVetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TratamentoController : ControllerBase
    {
        private readonly IConfiguration _configuration;  //Injeção privada para utilizar certos metodos

        public TratamentoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Get

        /// <summary>
        /// Metodo que retorna a informação de todos os tratamentos da base de dados
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize(Roles = "Admin")]
        public IEnumerable<Tratamento> Get()
        {
            using (var context = new FeedyVetContext())
            {
                return context.Tratamento.ToList();
            }
        }
        #endregion


        // não está terminado, ver se este funcionario ou gerente pode aceder a esta prescrição! (LATER)
        [Route("gettratamentobyprescricao/{idPrescricao}")]
        [HttpGet, Authorize(Roles = "Funcionario,Gerente")]
        public IActionResult GetTratamentoByPrescricao(int idPrescricao)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    string funcEmail = User.FindFirstValue(ClaimTypes.Email);
                    Funcionario f = context.Funcionario.Where(f => f.Email == funcEmail).FirstOrDefault();
                    if (f == null) return Forbid();
                    Tratamento t = context.Tratamento.Where(t => t.IdPrescricao == idPrescricao).FirstOrDefault();
                    return new JsonResult(t);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        #region Post

        /// <summary>
        /// Metodo que permite adicionar um novo elemento no Tratamento
        /// </summary>
        /// <param name="tratamento"></param>
        /// <returns> Novo tratamento </returns>
        [HttpPost, Authorize(Roles = "Admin")]
        public IActionResult Post(Tratamento tratamento)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    Tratamento t = new Tratamento();
                    
                    t.IdPrescricaoNavigation = context.Prescricao.Where(p => p.IdPrescricao == tratamento.IdPrescricao).FirstOrDefault();
                    t.IdStockNavigation = context.StockEstabelecimento.Where(s => s.IdStock == s.IdStock).FirstOrDefault();
                    t.Quantidade = tratamento.Quantidade;

                    context.Tratamento.Add(t);

                    context.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }
        #endregion

        #region Update

        /// <summary>
        /// Metodo de atualizar informação de um elemento de um tratamento
        /// </summary>
        /// <param name="tratamento"></param>
        /// <returns></returns>
        [HttpPatch, Authorize(Roles = "Admin")]
        public IActionResult Patch(Tratamento tratamento)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    Tratamento t = context.Tratamento.Where(t => t.IdStock == tratamento.IdStock && t.IdPrescricao == tratamento.IdPrescricao).FirstOrDefault();

                    if(tratamento.IdPrescricao != 0) t.IdPrescricaoNavigation = context.Prescricao.Where(p => p.IdPrescricao == tratamento.IdPrescricao).FirstOrDefault();
                    if (tratamento.IdStock != 0) t.IdStockNavigation = context.StockEstabelecimento.Where(s => s.IdStock == s.IdStock).FirstOrDefault();
                    t.Quantidade = tratamento.Quantidade;

                    context.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }
        #endregion

        #region Delete

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id1"></param>
        /// <param name="id2"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult Delete(Tratamento tratamento)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    context.Tratamento.Remove(context.Tratamento
                        .Where(t => t.IdStock == tratamento.IdStock && t.IdPrescricao == tratamento.IdPrescricao).FirstOrDefault());

                    context.SaveChanges();
                }

                return Ok();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }
        #endregion
    }
}

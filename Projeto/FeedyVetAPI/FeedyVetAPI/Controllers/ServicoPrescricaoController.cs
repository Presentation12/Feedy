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
    public class ServicoPrescricaoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ServicoPrescricaoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Get

        /// <summary>
        /// Listar informação sobre todos os elementos do ServicoPrescricao da base de dados
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize(Roles = "Admin")]
        public IEnumerable<ServicoPrescricao> Get()
        {
            using (var context = new FeedyVetContext())
                return context.ServicoPrescricao.ToList();
        }

        #endregion

        #region Post

        /// <summary>
        /// Metodo que permite adicionar um novo elemento de ServicoPrescricao na base de dados
        /// </summary>
        /// <param name="servicoPrescricao"></param>
        /// <returns></returns>
        [HttpPost, Authorize(Roles = "Admin")]
        public IActionResult Post(ServicoPrescricao servicoPrescricao)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    ServicoPrescricao sp = new ServicoPrescricao();

                    sp.IdPrescricaoNavigation = context.Prescricao.Where(p => p.IdPrescricao == servicoPrescricao.IdPrescricao).FirstOrDefault();
                    sp.IdServicoNavigation = context.Servico.Where(s => s.IdServico == servicoPrescricao.IdServico).FirstOrDefault();

                    context.ServicoPrescricao.Add(sp);

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
        /// Metodo que permite atualizar um elemento ServicoPrescricao na base de dados
        /// </summary>
        /// <param name="servicoPrescricao"></param>
        /// <returns></returns>
        [HttpPatch, Authorize(Roles = "Admin")]
        public IActionResult Patch(ServicoPrescricao servicoPrescricao)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    ServicoPrescricao sp = context.ServicoPrescricao
                        .Where(s => s.IdPrescricao == servicoPrescricao.IdPrescricao && s.IdServico == servicoPrescricao.IdServico).FirstOrDefault();

                    if (servicoPrescricao.IdPrescricao != 0) sp.IdPrescricaoNavigation = context.Prescricao.Where(p => p.IdPrescricao == servicoPrescricao.IdPrescricao).FirstOrDefault();
                    if (servicoPrescricao.IdServico != 0)  sp.IdServicoNavigation = context.Servico.Where(s => s.IdServico == servicoPrescricao.IdServico).FirstOrDefault();

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
        [HttpDelete, Authorize(Roles = "Admin")]
        public IActionResult Delete(ServicoPrescricao servicoPrescricao)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    context.ServicoPrescricao.Remove(context.ServicoPrescricao
                        .Where(s => s.IdPrescricao == servicoPrescricao.IdPrescricao
                        && s.IdServico == servicoPrescricao.IdServico).FirstOrDefault());

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
    }
}


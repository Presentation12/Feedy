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
using System.Security.Claims;
using System.Threading.Tasks;

namespace FeedyVetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificacaoController : ControllerBase
    {
        private readonly IConfiguration _configuration;  //Injeção privada para utilizar certos metodos
        public NotificacaoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Get

        /// <summary>
        /// Busca e mostra as notificações do user
        /// </summary>
        /// <param name="id_cliente"> id do cliente </param>
        /// <returns>  </returns>
        [Route("getnotifications/{id_cliente}")]
        [HttpGet, Authorize(Roles = "Admin,Cliente")]
        public IActionResult GetNotification(int id_cliente)
        {
            using (var context = new FeedyVetContext())
            {
                bool authorized = false;

                if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                {
                    string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                    Cliente c = context.Cliente.Where(c => c.Email == emailCliente).FirstOrDefault();

                    if (c != null)
                    {
                        authorized = true;
                    }

                }

                if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                if (authorized == false) return Forbid();

                try
                {
                    Cliente cliente = context.Cliente.Where(c => c.IdCliente == id_cliente).FirstOrDefault();

                    return new JsonResult(context.Notificacao.Where(e => e.Estado != "Inativo" && e.IdCliente == cliente.IdCliente).OrderByDescending(n => n.DataNotificacao).ToList());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }
            }
        }

        #endregion

        #region Post

        /// <summary>
        /// Adiciona uma notificação
        /// </summary>
        /// <param name="not"> notificação </param>
        /// <returns> mensagem do sucedido </returns>
        [HttpPost, Authorize(Roles = "Admin")]
        public IActionResult AddNotification(Notificacao not)
        {
            using (var context = new FeedyVetContext())
            {

                bool authorized = false;

                if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                if (authorized == false) return Forbid();

                Notificacao notificacao = new Notificacao();
                notificacao.IdCliente = not.IdCliente;
                notificacao.Estado = "Por Visualizar";
                notificacao.Descricao = not.Descricao;
                notificacao.DataNotificacao = not.DataNotificacao;


                context.Notificacao.Add(notificacao);

                try
                {
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
        /// Editar uma notificação
        /// </summary>
        /// <param name="not"> notificação </param>
        /// <returns> mensagem do sucedido </returns>
        [HttpPatch, Authorize(Roles = "Admin")]
        public IActionResult EditNotification(Notificacao not)
        {
            using (var context = new FeedyVetContext())
            {
                bool authorized = false;

                if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                if (authorized == false) return Forbid();

                try
                {
                    Notificacao notificacao = context.Notificacao.Where(e => e.IdNotificacao == not.IdNotificacao).FirstOrDefault();

                    notificacao.Estado = not.Estado is null ? notificacao.Estado : not.Estado;
                    notificacao.Descricao = not.Descricao is null ? notificacao.Descricao : not.Descricao;
                    notificacao.DataNotificacao = not.DataNotificacao == null ? notificacao.DataNotificacao : not.DataNotificacao;

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
        /// Remove uma notificação
        /// Autorizado ao cliente e ao admin
        /// </summary>
        /// <param name="not"> Notificação </param>
        /// <returns> Mensagem do sucedido </returns>
        [HttpPatch("delete"),Authorize(Roles = "Admin,Cliente")]
        public IActionResult DeleteNotification(Notificacao not)
        {
            using (var context = new FeedyVetContext())
            {
                bool authorized = false;

                if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                {
                    string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                    Cliente c = context.Cliente.Where(c => c.Email == emailCliente).FirstOrDefault();

                    if (c != null && c.IdCliente == not.IdCliente)
                    {
                        authorized = true;
                    }
                }

                if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                if (authorized == false) return Forbid();

                try
                {
                    Notificacao notificacao = context.Notificacao.Where(n => n.IdNotificacao == not.IdNotificacao).FirstOrDefault();
                    context.Notificacao.Remove(notificacao);
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
    }
}

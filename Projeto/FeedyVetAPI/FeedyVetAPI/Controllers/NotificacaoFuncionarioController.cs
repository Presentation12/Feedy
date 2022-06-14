using FeedyVetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FeedyVetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificacaoFuncionarioController : ControllerBase
    {
        /// <summary>
        /// Busca e mostra as notificações do veterinario
        /// </summary>
        /// <param name="id_cliente"> id do cliente </param>
        /// <returns>  </returns>
        [Route("getnotifications/{idFuncionario}")]
        [HttpGet, Authorize(Roles = "Admin,Gerente,Funcionario")]
        public IActionResult Get(int idFuncionario)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    bool authorized = false;

                    if (User.HasClaim(ClaimTypes.Role, "Gerente") || (User.HasClaim(ClaimTypes.Role, "Funcionario") && !User.HasClaim(ClaimTypes.Role, "Admin")))
                    {
                        string emailFuncionario = User.FindFirstValue(ClaimTypes.Email);
                        Funcionario f = context.Funcionario.Where(f => f.Email == emailFuncionario).FirstOrDefault();

                        if (f != null)
                        {
                            authorized = true;
                        }

                    }

                    if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                    if (authorized == false) return Forbid();

                    Funcionario func = context.Funcionario.Where(c => c.IdFuncionario == idFuncionario).FirstOrDefault();

                    return new JsonResult(context.NotificacaoFuncionario.Where(e => (e.Estado != "Lida" && e.Estado != "Inativo") && e.IdFuncionario == func.IdFuncionario).ToList());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }
            }
        }

        // DELETE api/<NotificacaoFuncionarioController>/5
        [HttpPatch("delete"), Authorize(Roles = "Admin,Gerente, Funcionario")]
        public IActionResult DeleteNotification(NotificacaoFuncionario not)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    bool authorized = false;

                    if (User.HasClaim(ClaimTypes.Role, "Gerente") || (User.HasClaim(ClaimTypes.Role, "Funcionario") && !User.HasClaim(ClaimTypes.Role, "Admin")))
                    {
                        string emailFuncionario = User.FindFirstValue(ClaimTypes.Email);
                        Funcionario f = context.Funcionario.Where(f => f.Email == emailFuncionario).FirstOrDefault();

                        if (f != null)
                        {
                            authorized = true;
                        }

                    }

                    if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                    if (authorized == false) return Forbid();


                    NotificacaoFuncionario notificacao = context.NotificacaoFuncionario.Where(n => n.IdNotificacao == not.IdNotificacao).FirstOrDefault();
                    context.NotificacaoFuncionario.Remove(notificacao);

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
    }
}

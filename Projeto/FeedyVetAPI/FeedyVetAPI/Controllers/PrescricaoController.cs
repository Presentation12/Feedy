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
    public class PrescricaoController : ControllerBase
    {
        private readonly IConfiguration _configuration;  //Injeção privada para utilizar certos metodos
        public PrescricaoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Get

        /// <summary>
        /// Busca e mostra as receitas/prescrições de um certo serviço prestado a um animal de um determinado cliente
        /// </summary>
        /// <param name="idServico"> Id do servico </param>
        /// <returns> Dados da prescricao </returns>
        [Route("{idServico}")]
        [HttpGet, Authorize(Roles = "Admin,Cliente,Funcionario,Gerente")]
        public IActionResult Get(int idServico)
        {
            using (var context = new FeedyVetContext())
            {
                bool authorized = false;
                Servico s = context.Servico.Where(s => s.IdServico == idServico).FirstOrDefault();

                if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                {
                    string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                    Cliente c = context.Cliente.Where(c => c.Email == emailCliente).FirstOrDefault();

                    if (c != null && c.IdCliente == s.IdCliente)
                    {
                        authorized = true;
                    }

                }

                if (User.HasClaim(ClaimTypes.Role, "Funcionario"))
                {
                    string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                    Funcionario f = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();
                    if (f != null && s.IdFuncionario == f.IdFuncionario)
                    {
                        authorized = true;
                    }
                }

                if (User.HasClaim(ClaimTypes.Role, $"Gerente_{s.IdEstabelecimento}"))
                {
                    authorized = true;
                }

                if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                if (authorized == false) return Forbid();

                try
                {
                    ServicoPrescricao servicoPrescricao = context.ServicoPrescricao
                        .Where(sp => sp.IdServico == idServico).FirstOrDefault();

                    return new JsonResult(context.Prescricao.Select(p => new {
                        p.IdPrescricao,
                        p.Estado,
                        p.Descricao,
                        p.DataExpiracao
                    }).Where(p => p.IdPrescricao == servicoPrescricao.IdPrescricao).ToList());
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }
            }
        }


        // adaptar para cliente tambem se for preciso
        [Route("getprescricoesbytoken")]
        [HttpGet, Authorize(Roles = "Funcionario,Gerente")]
        public IActionResult GetPrescricoesByToken()
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    string funcEmail = User.FindFirstValue(ClaimTypes.Email);
                    Funcionario f = context.Funcionario.Where(f => f.Email == funcEmail).FirstOrDefault();
                    if (f == null) return Forbid();

                    List<FuncionarioEstabelecimento> feList = context.FuncionarioEstabelecimento.Where(fe => fe.IdFuncionario == f.IdFuncionario).ToList();
                    List<Servico> servicos = new List<Servico>();
                    foreach (FuncionarioEstabelecimento fe in feList)
                    {
                        servicos.AddRange(context.Servico.Where(se => se.IdEstabelecimento == fe.IdEstabelecimento).ToList());
                    }
                    List<ServicoPrescricao> servicosPrescricoes = new List<ServicoPrescricao>();
                    foreach (Servico se in servicos)
                    {
                        servicosPrescricoes.AddRange(context.ServicoPrescricao.Where(sp => sp.IdServico == se.IdServico).ToList());
                    }
                    List<Prescricao> prescricoes = new List<Prescricao>();
                    foreach (ServicoPrescricao sp_aux in servicosPrescricoes)
                    {
                        prescricoes.Add(context.Prescricao.Where(p => p.IdPrescricao == sp_aux.IdPrescricao).FirstOrDefault());
                    }
                    return new JsonResult(prescricoes);
                }
            } catch(Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
            
        }



        #endregion



        #region Post

        /// <summary>
        /// Cria uma prescrição passada por um funcionario
        /// </summary>
        /// <param name="idServico"> Id do serviço </param>
        /// <param name="pres"> Prescrição </param>
        /// <returns> Mensagem do sucedido </returns>
        [Route("{idServico}")]
        [HttpPost, Authorize(Roles = "Admin,Funcionario")]
        public IActionResult AddPrescricao(int idServico, Prescricao pres)
        {
            using (var context = new FeedyVetContext())
            {
                bool authorized = false;
                Servico s = context.Servico.Where(s => s.IdServico == idServico).FirstOrDefault();

                if (User.HasClaim(ClaimTypes.Role, "Funcionario"))
                {
                    string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                    Funcionario f = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();
                    if (f != null && s.IdFuncionario == f.IdFuncionario)
                    {
                        authorized = true;
                    }
                }

                if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                if (authorized == false) return Forbid();

                Servico servico = context.Servico.Where(s => s.IdServico == idServico).FirstOrDefault();
                ServicoPrescricao servicoPrescricao = new ServicoPrescricao();
                Prescricao prescricao = new Prescricao();

                prescricao.IdPrescricao = pres.IdPrescricao;
                prescricao.Estado = pres.Estado;
                prescricao.Descricao = pres.Descricao;
                prescricao.DataExpiracao = pres.DataExpiracao;

                servicoPrescricao.IdServicoNavigation = servico;
                servicoPrescricao.IdPrescricaoNavigation = prescricao;

                context.Prescricao.Add(prescricao);
                context.ServicoPrescricao.Add(servicoPrescricao);

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
        /// Editar prescrição
        /// </summary>
        /// <param name="prescricao"> prescrição </param>
        /// <param name="idServico"> id de serviço </param>
        /// <returns> Mensagem do sucedido </returns>
        [Route("editprescricao/{idServico}")]
        [HttpPatch, Authorize(Roles = "Admin,Funcionario")]
        public IActionResult EditPrescricao(Prescricao prescricao, int idServico)
        {
            using (var context = new FeedyVetContext())
            {
                bool authorized = false;
                Servico s = context.Servico.Where(s => s.IdServico == idServico).FirstOrDefault();

                if (User.HasClaim(ClaimTypes.Role, "Funcionario"))
                {
                    string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                    Funcionario f = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();
                    if (f != null && s.IdFuncionario == f.IdFuncionario)
                    {
                        authorized = true;
                    }
                }

                if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                if (authorized == false) return Forbid();

                try
                {
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
        /// Remover uma prescrição
        /// </summary>
        /// <param name="pres"> prescrição </param>
        /// <param name="idServico"> id do serviço </param>
        /// <returns> mensagem do sucedido </returns>
        [Route("deleteprescricao/{idServico}")]
        [HttpDelete, Authorize(Roles = "Admin,Funcionario")]
        public IActionResult DeletePrescricao(Prescricao pres, int idServico)
        {
            using (var context = new FeedyVetContext())
            {
                bool authorized = false;
                Servico s = context.Servico.Where(s => s.IdServico == idServico).FirstOrDefault();

                if (User.HasClaim(ClaimTypes.Role, "Funcionario"))
                {
                    string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                    Funcionario f = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();
                    if (f != null && s.IdFuncionario == f.IdFuncionario)
                    {
                        authorized = true;
                    }
                }

                if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                if (authorized == false) return Forbid();

                try
                {
                    Prescricao prescricao = context.Prescricao.Where(p => p.IdPrescricao == pres.IdPrescricao).FirstOrDefault();
                    context.Prescricao.Remove(prescricao);
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

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
    public class FuncionarioEstabelecimentoController : ControllerBase
    {
        private readonly IConfiguration _configuration;  //Injeção privada para utilizar certos metodos
        public FuncionarioEstabelecimentoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        
        #region Get
        [HttpGet, Authorize(Roles = "Admin")] 
        public IEnumerable<FuncionarioEstabelecimento> Get()
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    return context.FuncionarioEstabelecimento.ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        #endregion
        
        #region Post
        [HttpPost, Authorize(Roles = "Admin,Gerente")] 
        public IActionResult Post(FuncionarioEstabelecimento funcionarioEstabelecimento)
        {
            if (CheckGerente(funcionarioEstabelecimento.IdEstabelecimento))
            {
                using (var context = new FeedyVetContext())
                {
                    try
                    {
                        FuncionarioEstabelecimento fe = new FuncionarioEstabelecimento();
                        fe.IdFuncionario = funcionarioEstabelecimento.IdFuncionario;
                        fe.IdEstabelecimento = funcionarioEstabelecimento.IdEstabelecimento;
                        fe.DataInicio = funcionarioEstabelecimento.DataInicio;
                        fe.DataFim = funcionarioEstabelecimento.DataFim;

                        context.FuncionarioEstabelecimento.Add(fe);

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
            else return Forbid();
            
        }
        #endregion
        
        #region Update
        [HttpPatch, Authorize(Roles ="Admin,Gerente")] //Metodo para dar update dos valores na table
        public IActionResult Patch(FuncionarioEstabelecimento funcionarioEstabelecimento)
        {
            if (CheckGerente(funcionarioEstabelecimento.IdEstabelecimento))
            {
                using (var context = new FeedyVetContext())
                {
                    try
                    {
                        FuncionarioEstabelecimento fe = new FuncionarioEstabelecimento();
                        fe.IdFuncionario = funcionarioEstabelecimento.IdFuncionario == 0 ? fe.IdFuncionario : funcionarioEstabelecimento.IdFuncionario;
                        fe.IdEstabelecimento = funcionarioEstabelecimento.IdEstabelecimento == 0 ? fe.IdEstabelecimento : funcionarioEstabelecimento.IdEstabelecimento;
                        fe.DataInicio = funcionarioEstabelecimento.DataInicio is null ? fe.DataInicio : funcionarioEstabelecimento.DataInicio;
                        fe.DataFim = funcionarioEstabelecimento.DataFim is null ? fe.DataFim : funcionarioEstabelecimento.DataFim;

                        context.SaveChanges();

                        return Ok();
                    } catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return BadRequest();
                    }
                }
            } else return Forbid();
            
        }
        #endregion
        
        #region Delete
        [HttpDelete("{id_func}/{id_estab}"), Authorize(Roles = "Admin,Gerente")] 
        public IActionResult Delete(int id_func, int id_estab)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    FuncionarioEstabelecimento fe = context.FuncionarioEstabelecimento.Where(fe => fe.IdEstabelecimento == id_estab).FirstOrDefault();
                    if (CheckGerente(fe.IdEstabelecimento))
                    {
                        context.FuncionarioEstabelecimento.Remove(fe);
                        context.SaveChanges();
                        return Ok();
                    }
                    else return Forbid();
                }catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }

        }
        }
        #endregion

        public bool CheckGerente(int idEstab)
        {
            if (User.HasClaim(ClaimTypes.Role, $"Gerente_{idEstab}"))
            {
                return true;
            }
            else if (User.HasClaim(ClaimTypes.Role, "Admin"))
            {
                return true;
            }
            else return false;
        }
    }
}

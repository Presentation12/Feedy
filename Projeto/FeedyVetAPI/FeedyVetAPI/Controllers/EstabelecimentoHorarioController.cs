using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using FeedyVetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FeedyVetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstabelecimentoHorarioController : ControllerBase
    {
        private readonly IConfiguration _configuration;  //Injeção privada para utilizar certos metodos

        public EstabelecimentoHorarioController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Get
        [HttpGet, Authorize(Roles = "Admin")] //Metodo para ir buscar os dados na tabela
        public IEnumerable<EstabelecimentoHorario> Get() // TESTAR
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    return context.EstabelecimentoHorario.ToList();
                }
            } catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

        }
        #endregion

        
        #region Post
        [HttpPost, Authorize(Roles = "Admin,Gerente")]
        public IActionResult Post(EstabelecimentoHorario estabHorario)
        {
            if (estabHorario.IdEstabelecimento == 0) return new JsonResult("Erro");
            if (CheckGerente(estabHorario.IdEstabelecimento))
            {
                using (var context = new FeedyVetContext())
                {
                    try
                    {
                        EstabelecimentoHorario eh = new EstabelecimentoHorario();
                        eh.IdEstabelecimentoHorario = estabHorario.IdEstabelecimentoHorario;
                        eh.IdEstabelecimento = estabHorario.IdEstabelecimento;
                        eh.HorarioAbertura = estabHorario.HorarioAbertura;
                        eh.HorarioEncerramento = estabHorario.HorarioEncerramento;
                        eh.DiaSemana = estabHorario.DiaSemana;

                        context.EstabelecimentoHorario.Add(eh);

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
        [HttpPatch, Authorize(Roles = "Gerente")] //Metodo para dar update dos valores na table
        public IActionResult Patch(EstabelecimentoHorario estabHorario)
        {
            if (CheckGerente(estabHorario.IdEstabelecimento))
            {
                using (var context = new FeedyVetContext())
                {
                    try
                    {
                        EstabelecimentoHorario eh = context.EstabelecimentoHorario.Where(eh => eh.IdEstabelecimentoHorario == estabHorario.IdEstabelecimentoHorario).FirstOrDefault();

                        eh.IdEstabelecimento = estabHorario.IdEstabelecimento == 0 ? eh.IdEstabelecimento : estabHorario.IdEstabelecimento;
                        eh.HorarioAbertura = estabHorario.HorarioAbertura is null ? eh.HorarioAbertura : estabHorario.HorarioAbertura;
                        eh.HorarioEncerramento = estabHorario.HorarioEncerramento is null ? eh.HorarioEncerramento : estabHorario.HorarioEncerramento;
                        eh.DiaSemana = estabHorario.DiaSemana is null ? eh.DiaSemana : estabHorario.DiaSemana;

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

        #region Delete
        [Route("{idEstabHorario}")]
        [HttpDelete, Authorize(Roles = "Gerente")] //Metodo para dar delete dos valores na table //Passar por referencia por ser com o ID
        public IActionResult Delete(int idEstabHorario)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    EstabelecimentoHorario eh = context.EstabelecimentoHorario.Where(eh => eh.IdEstabelecimentoHorario == idEstabHorario).FirstOrDefault();
                    if (CheckGerente(eh.IdEstabelecimento))
                    {
                        context.EstabelecimentoHorario.Remove(eh);
                        context.SaveChanges();
                        return Ok();
                    }
                    else return Forbid();
                }
                catch (Exception e)
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


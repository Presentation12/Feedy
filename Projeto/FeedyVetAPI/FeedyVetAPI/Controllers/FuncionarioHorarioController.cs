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
    public class FuncionarioHorarioController : ControllerBase
    {
        private readonly IConfiguration _configuration;  //Injeção privada para utilizar certos metodos
        public FuncionarioHorarioController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Get

        /// <summary>
        /// Metodo que lista a informacao dos horarios de todos os funcionarios
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize(Roles = "Admin")]
        public IEnumerable<FuncionarioHorario> Get()
        {
            using (var context = new FeedyVetContext())
            {
                try
                { 
                    return context.FuncionarioHorario.ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
        }
        }
        #endregion

        #region Post

        /// <summary>
        /// Adicionar um novo horario a um funcionario
        /// </summary>
        /// <param name="func_horario"></param>
        /// <returns></returns>
        [HttpPost, Authorize(Roles = "Admin, Gerente, Funcionario")]
        public IActionResult Post(FuncionarioHorario func_horario)
        {
            using (var context = new FeedyVetContext())
            {
                if (User.HasClaim(ClaimTypes.Role, "Funcionario"))
                {
                    string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                    Funcionario f = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();
                    if (f == null) Forbid();
                    if (func_horario.IdFuncionario != f.IdFuncionario) Forbid();
                }

                if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                {
                    bool authorized = false;
                    string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                    Funcionario f = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();

                    List<FuncionarioEstabelecimento> list = context.FuncionarioEstabelecimento
                        .Where(func => func.IdFuncionario == f.IdFuncionario).ToList();

                    foreach(FuncionarioEstabelecimento fe in list)
                    {
                        if (User.HasClaim(ClaimTypes.Role, $"Gerente_{fe.IdEstabelecimento}")) 
                        { 
                            authorized = true;
                            continue;
                        }
                    }

                    if (authorized != true) Forbid();
                }

                try
                {
                    FuncionarioHorario funcionarioHorario = new FuncionarioHorario();

                    funcionarioHorario.IdFuncionarioNavigation = context.Funcionario
                        .Where(f => f.IdFuncionario == func_horario.IdFuncionario).FirstOrDefault();
                    funcionarioHorario.HoraEntrada = func_horario.HoraEntrada;
                    funcionarioHorario.HoraSaida = func_horario.HoraSaida;
                    funcionarioHorario.DiaSemana = func_horario.DiaSemana;

                    context.FuncionarioHorario.Add(funcionarioHorario);

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
        /// Metodo que atualiza a informação de um horario de um funcionario
        /// </summary>
        /// <param name="vet_horario"></param>
        /// <returns></returns>
        [HttpPatch, Authorize(Roles = "Admin, Gerente, Funcionario")]
        public IActionResult Patch(FuncionarioHorario func_horario)
        {
            using (var context = new FeedyVetContext())
            {
                if (User.HasClaim(ClaimTypes.Role, "Funcionario"))
                {
                    string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                    Funcionario f = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();
                    if (f == null) Forbid();
                    if (func_horario.IdFuncionario != f.IdFuncionario) Forbid();
                }

                if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                {
                    bool authorized = false;
                    string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                    Funcionario f = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();

                    List<FuncionarioEstabelecimento> list = context.FuncionarioEstabelecimento
                        .Where(func => func.IdFuncionario == f.IdFuncionario).ToList();

                    foreach (FuncionarioEstabelecimento fe in list)
                    {
                        if (User.HasClaim(ClaimTypes.Role, $"Gerente_{fe.IdEstabelecimento}"))
                        {
                            authorized = true;
                            continue;
                        }
                    }

                    if (authorized != true) Forbid();
                }

                try
                {
                    FuncionarioHorario funcionarioHorario = context.FuncionarioHorario
                        .Where(fh => fh.IdFuncionarioHorario == func_horario.IdFuncionarioHorario).FirstOrDefault();

                    if(func_horario.IdFuncionario != 0) funcionarioHorario.IdFuncionarioNavigation = context.Funcionario
                        .Where(f => f.IdFuncionario == func_horario.IdFuncionario).FirstOrDefault();
                    funcionarioHorario.HoraEntrada = func_horario.HoraEntrada;
                    funcionarioHorario.HoraSaida = func_horario.HoraSaida;
                    funcionarioHorario.DiaSemana = func_horario.DiaSemana;

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
        /// Permite apagar um horario de um funcionario da base de dados
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}"), Authorize(Roles = "Admin, Gerente")]
        public IActionResult Delete(int id)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    context.FuncionarioHorario.Remove(context.FuncionarioHorario
                        .Where(fh => fh.IdFuncionario == id).FirstOrDefault());

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

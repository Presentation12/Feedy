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
    public class EstabelecimentoGerenteController : ControllerBase
    {
        private readonly IConfiguration _configuration;  //Injeção privada para utilizar certos metodos

        public EstabelecimentoGerenteController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Get

        /// <summary>
        /// Busca e mostra os dados da relação Estabelecimento <-> Funcionario (Mostra os gerentes desse estabeleciemento)
        /// </summary>
        /// <returns> Gerentes e o Estabeleciemento </returns>
        [HttpGet, Authorize(Roles = "Admin")]
        public IEnumerable<EstabelecimentoGerente> Get()
        {
            using (var context = new FeedyVetContext())
            {
                return context.EstabelecimentoGerente.ToList();
            }
        }

        #endregion

        #region Post

        /// <summary>
        /// Metodo que adiciona um novo gerente a uma clinica
        /// </summary>
        /// <param name="estabelecimentoGerente"></param>
        /// <returns></returns>
        [HttpPost, Authorize(Roles = "Admin, Gerente")]
        public IActionResult Post(EstabelecimentoGerente estabelecimentoGerente)
        {
            if (User.HasClaim(ClaimTypes.Role, "Gerente"))
            {
                if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{estabelecimentoGerente.IdEstabelecimento}")) return Forbid();
            }

            using (var context = new FeedyVetContext())
            {
                try
                {
                    EstabelecimentoGerente eg = new EstabelecimentoGerente();
                    
                    eg.IdEstabelecimentoNavigation = context.Estabelecimento
                        .Where(e => e.IdEstabelecimento == estabelecimentoGerente.IdEstabelecimento).FirstOrDefault();
                    eg.IdFuncionarioNavigation = context.Funcionario
                        .Where(e => e.IdFuncionario == estabelecimentoGerente.IdFuncionario).FirstOrDefault(); ;

                    context.EstabelecimentoGerente.Add(eg);

                    context.SaveChanges();
                    return Ok();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// Metodo que atualiza informacao sobre um gerente de um estabelecimento
        /// </summary>
        /// <param name="estabelecimentoGerente"></param>
        /// <returns></returns>
        [HttpPatch, Authorize(Roles = "Admin, Gerente")]
        public IActionResult Patch(EstabelecimentoGerente estabelecimentoGerente)
        {
            if (User.HasClaim(ClaimTypes.Role, "Gerente"))
            {
                if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{estabelecimentoGerente.IdEstabelecimento}")) return Forbid();
            }

            using (var context = new FeedyVetContext())
            {
                try
                {
                    EstabelecimentoGerente eg = new EstabelecimentoGerente();

                    if(estabelecimentoGerente.IdEstabelecimento != 0) eg.IdEstabelecimentoNavigation = context.Estabelecimento
                        .Where(e => e.IdEstabelecimento == estabelecimentoGerente.IdEstabelecimento).FirstOrDefault();
                    if (estabelecimentoGerente.IdFuncionario != 0) eg.IdFuncionarioNavigation = context.Funcionario
                        .Where(e => e.IdFuncionario == estabelecimentoGerente.IdFuncionario).FirstOrDefault(); ;

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
        [Route("/delete/{id_estabelecimento}/{id_funcionario}")]
        [HttpDelete, Authorize(Roles = "Admin")]
        public IActionResult Delete(int id_estabelecimento, int id_funcionario)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    EstabelecimentoGerente clinGer = context.EstabelecimentoGerente.Where(clinG => (clinG.IdEstabelecimento == id_estabelecimento && clinG.IdFuncionario == id_funcionario)).FirstOrDefault();
                    context.EstabelecimentoGerente.Remove(clinGer);

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

        #region Comentarios
        /*[HttpGet] //Metodo para ir buscar os dados na tabela
        public JsonResult Get()
        {
            string query = @"
                select *
                from dbo.Clinica_Gerente
                ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("FeedyAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon)) //Executar a query e filtar os dados numa tabela
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader); ;

                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpPost] //Metodo para escrever na table
        public JsonResult Post(ClinicaGerente clinicagerente)
        {
            String query = @"
                    insert into dbo.Clinica_Gerente
                    (id_clinica, id_veterinario)
                    values
                    ( 
                    '" + clinicagerente.IdClinica + @"'
                    '" + clinicagerente.IdVeterinario + @"'
                    )
                    ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("FeedyAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon)) //Executar a query e filtar os dados numa tabela
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader); ;

                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Added Successfully");
        }

        [HttpPut] //Metodo para dar update dos valores na table
        public JsonResult Put(ClinicaGerente clinicagerente)
        {
            string query = @"
                   update dbo.Clinica_Gerente set
                   
                   Where id_clinica = " + clinicagerente.IdClinica + @" and id_Veterinario = " + clinicagerente.IdVeterinario + @"                   
                    ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("FeedyAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon)) //Executar a query e filtar os dados numa tabela
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader); ;

                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Updated Successfully");
        }

        [HttpDelete("{id}")] //Metodo para dar delete dos valores na table //Passar por referencia por ser com o ID
        public JsonResult Delete(int id_cli, int id_vet)
        {
            string query = @"
                Delete from dbo.Clinica_Gerente
                where id_clinica = " + id_cli + @" and id_veterinario = " + id_vet + @"
                ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("FeedyAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon)) //Executar a query e filtar os dados numa tabela
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader); ;

                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Deleted Successfully");
        }*/
        #endregion
    }
}

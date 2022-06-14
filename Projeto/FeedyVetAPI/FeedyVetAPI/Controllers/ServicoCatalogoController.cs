using FeedyVetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FeedyVetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicoCatalogoController : ControllerBase
    {
        private readonly IConfiguration _configuration;  //Injeção privada para utilizar certos metodos
        private readonly IWebHostEnvironment _env;
        public ServicoCatalogoController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        #region Get

        /// <summary>
        /// Busca e mostra os dados de todos os serviços dos catalogos
        /// </summary>
        /// <returns> Serviços dos catalogos </returns>
        [HttpGet, Authorize(Roles = "Admin")]
        public IEnumerable<ServicoCatalogo> Get()
        {
            using (var context = new FeedyVetContext())
            {
                return context.ServicoCatalogo.ToList();
            }
        }

        /// <summary>
        /// Busca e mostra os tipos de serviços do catalogo de um estabelecimento pelo id
        /// </summary>
        /// <param name="idEstabelecimento"> Id do estabelecimento </param>
        /// <returns> Serviços do catalogo do estabelecimento </returns>
        [Route("{idEstabelecimento}")]
        [HttpGet, Authorize(Roles = "Admin, Cliente, Gerente, Funcionario")]
        public IActionResult GetServicoCatalogoByid(int idEstabelecimento)
        {
            using (var context = new FeedyVetContext())
            {
                if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                {
                    if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{idEstabelecimento}")) Forbid();
                }

                if (User.HasClaim(ClaimTypes.Role, "Funcionario"))
                {
                    string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                    Funcionario func = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();

                    FuncionarioEstabelecimento funcest = context.FuncionarioEstabelecimento
                        .Where(f => f.IdEstabelecimento == idEstabelecimento && f.IdFuncionario == func.IdFuncionario && !f.DataFim.HasValue).FirstOrDefault();

                    if (funcest is null) Forbid();
                }

                try
                {
                    return new JsonResult(context.ServicoCatalogo.Where(s => s.IdEstabelecimento == idEstabelecimento).ToList());
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
        /// Inserir dados de um serviço catalogo
        /// </summary>
        /// <param name="sc"> Serviço Catalogo </param>
        /// <returns> Mensagem do sucedido </returns>
        [HttpPost, Authorize(Roles = "Admin, Gerente")]
        public IActionResult Post(ServicoCatalogo sc)
        {
            using (var context = new FeedyVetContext())
            {
                if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                {
                    if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{sc.IdEstabelecimento}")) Forbid();
                }

                try
                {
                    ServicoCatalogo servicocatalogo = new ServicoCatalogo();

                    servicocatalogo.IdEstabelecimentoNavigation = context.Estabelecimento.Where(e => e.IdEstabelecimento == sc.IdEstabelecimento).FirstOrDefault();

                    servicocatalogo.Estado = sc.Estado;
                    servicocatalogo.Tipo = sc.Tipo;
                    servicocatalogo.Preco = sc.Preco;
                    servicocatalogo.Descricao = sc.Descricao;
                    context.ServicoCatalogo.Add(servicocatalogo);

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
        /// Atualizar os dados de um serviço no catálogo
        /// </summary>
        /// <param name="id_estabelecimento"></param>
        /// <param name="id_funcionario"></param>
        /// <param name="sc"></param>
        /// <returns></returns>
        [Route("{id_estabelecimento}/change")]
        [HttpPatch, Authorize(Roles = "Admin, Gerente")]
        public IActionResult Patch(int id_estabelecimento, ServicoCatalogo sc)
        {
            using (var context = new FeedyVetContext())
            {
                if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                {
                    if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{sc.IdEstabelecimento}")) Forbid();
                }

                try
                {
                    ServicoCatalogo servicocatalogo = context.ServicoCatalogo.Where(s => s.IdServicoCatalogo == sc.IdServicoCatalogo).FirstOrDefault();

                    if (id_estabelecimento != servicocatalogo.IdEstabelecimento) return BadRequest();

                    if(sc.IdEstabelecimento != 0) context.Estabelecimento.Where(e => e.IdEstabelecimento == sc.IdEstabelecimento).FirstOrDefault();
                    servicocatalogo.Estado = sc.Estado is null ? servicocatalogo.Estado : sc.Estado;
                    servicocatalogo.Tipo = sc.Tipo is null ? servicocatalogo.Tipo : sc.Tipo;
                    servicocatalogo.Preco = sc.Preco == servicocatalogo.Preco ? servicocatalogo.Preco : sc.Preco;
                    servicocatalogo.Descricao = sc.Descricao is null ? servicocatalogo.Descricao : sc.Descricao;
                    servicocatalogo.CatalogoFoto = sc.CatalogoFoto is null ? servicocatalogo.CatalogoFoto : sc.CatalogoFoto;

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
        /// Arquivar um serviço do catálogo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="id_estabelecimento"></param>
        /// <param name="id_funcionario"></param>
        /// <returns></returns>
        [Route("{id_estabelecimento}/deleteservicocat/{id}")]
        [HttpPatch, Authorize(Roles = "Admin, Gerente")]
        public IActionResult DelServicoCatalogo(int id, int id_estabelecimento)
        {
            using (var context = new FeedyVetContext())
            {
                if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                {
                    if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{id_estabelecimento}")) Forbid();
                }

                ServicoCatalogo servicoCatalogo = context.ServicoCatalogo.Where(sc => sc.IdServicoCatalogo == id).FirstOrDefault();

                servicoCatalogo.Estado = "Inativo";
                return Patch(id_estabelecimento, servicoCatalogo);
            }
        }

        /// <summary>
        /// Método que elimina um elemento de ServiçoCatalogo da base de dados
        /// </summary>
        /// <param name="id"> ID do ServiçoCatálogo </param>
        /// <returns> Estado do Método </returns>
        [Route("{id}")]
        [HttpDelete, Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            try
            {

                using (var context = new FeedyVetContext())
                {
                    context.ServicoCatalogo.Remove
                        (context.ServicoCatalogo.Where(sc => sc.IdServicoCatalogo == id).FirstOrDefault());

                    context.SaveChanges();
                    return Ok();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        #endregion

        #region Comentarios

        /*
        //AVISO: ENTITY FRAMEWORK NAO APLICADO
        #region Get

        /// <summary>
        /// Busca e mostra os dados do catalogo de cada estabelecimento
        /// </summary>
        /// <returns> Dados do catalogo de todos os estabelecimentos </returns>
        [HttpGet]
        public JsonResult Get()
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    return new JsonResult(context.ServicoCatalogo.Select(s => new {
                        s.IdServicoCatalogo,
                        s.IdEstabelecimento,
                        s.Estado,
                        s.Tipo,
                        s.Preco,
                        s.Descricao,
                        s.CatalogoFoto,
                        s.Duracao
                    }).ToList());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return new JsonResult("Error");
                }
            }
        }

        /// <summary>
        /// Busca e mostra os dados do catalogo de um estabelecimento
        /// </summary>
        /// <param name="id_estabelecimento"></param>
        /// <returns> Dados do catalogo de um dado estabelecimento </returns>
        [Route("{id_estabelecimento}/showcatalogo")]
        [HttpGet]
        public JsonResult CatalogoEstabelecimento(int id_estabelecimento)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    return new JsonResult(context.ServicoCatalogo.Where(sc => sc.IdEstabelecimento == id_estabelecimento).Select(s => new {
                        s.IdEstabelecimento,
                        s.Estado,
                        s.Tipo,
                        s.Preco,
                        s.Descricao,
                        s.CatalogoFoto,
                        s.Duracao
                    }).ToList()); ;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return new JsonResult("Error");
                }
            }
        }

        /// <summary>
        /// Busca e mostra os dados de um serviço no catalogo
        /// </summary>
        /// <param name="id_servicoCatalogo"></param>
        /// <returns> Dados de um serviço no catalogo </returns>
        [Route("{id_servicoCatalogo}")]
        [HttpGet]
        public JsonResult GetServicoCatalogoByid(int id_servicoCatalogo)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    return new JsonResult(context.ServicoCatalogo.Where(s => s.IdServicoCatalogo == id_servicoCatalogo).Select(sc => new
                    {
                        sc.IdEstabelecimento,
                        sc.Estado,
                        sc.Tipo,
                        sc.Preco,
                        sc.Descricao,
                        sc.CatalogoFoto,
                        sc.Duracao
                    }).FirstOrDefault()); ;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return new JsonResult("Error");
                }
            }
        }

        #endregion

        //AVISO: ENTITY FRAMEWORK NAO APLICADO
        #region Update
        [HttpPut] //Metodo para dar update dos valores na table
        public JsonResult Put(ServicoCatalogo servicoCatalogo)
        {
            string query = @"
                    update dbo.Servico_Catalogo set
                    id_clinica = '" + servicoCatalogo.IdEstabelecimento + @"'
                    ,estado = '" + servicoCatalogo.Estado + @"'
                    ,tipo = '" + servicoCatalogo.Tipo + @"'
                    ,preco = '" + servicoCatalogo.Preco + @"'
                    ,descricao = '" + servicoCatalogo.Descricao + @"'
                    ,catalogo_foto = '" + servicoCatalogo.CatalogoFoto + @"'
                    Where id_servico_catalogo = " + servicoCatalogo.IdServicoCatalogo + @"
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
        #endregion

        //AVISO: ENTITY FRAMEWORK NAO APLICADO
        #region Delete
        [HttpDelete("{id}")] //Metodo para dar delete dos valores na table //Passar por referencia por ser com o ID
        public JsonResult Delete(int id)
        {
            string query = @"
                Delete from dbo.Servico_Catalogo
                where id_servico_catalogo = " + id + @"
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
        }
        #endregion
        */

        /*
       [HttpPost] //Metodo para escrever na table
       public JsonResult Post(ServicoCatalogo servicoCatalogo)
       {
           String query = @"
                   insert into dbo.Servico_Catalogo
                   (id_clinica,estado,tipo,preco,descricao,catalogo_foto)
                   values
                   ( 
                   '" + servicoCatalogo.IdClinica + @"'
                   ,'" + servicoCatalogo.Estado + @"'
                   ,'" + servicoCatalogo.Tipo + @"'
                   ,'" + servicoCatalogo.Preco + @"'
                   ,'" + servicoCatalogo.Descricao + @"'
                   ,'" + servicoCatalogo.CatalogoFoto + @"'
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
       */
        #endregion
    }
}

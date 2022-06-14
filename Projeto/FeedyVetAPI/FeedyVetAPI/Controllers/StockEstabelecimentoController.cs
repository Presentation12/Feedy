using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using FeedyVetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FeedyVetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockEstabelecimentoController : ControllerBase
    {
        private readonly IConfiguration _configuration;  //Injeção privada para utilizar certos metodos

        public StockEstabelecimentoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Get
        
        /// <summary>
        /// Busca e mostra produtos de varios estabelecimentos
        /// </summary>
        /// <returns> Produtos </returns>
        [HttpGet, Authorize(Roles = "Admin, Cliente")] 
        public JsonResult Get()
        {
            using (var context = new FeedyVetContext())
            {
                List<StockEstabelecimento> stockEstabelecimentos = context.StockEstabelecimento.ToList();
                List<Estabelecimento> estabelecimentos = context.Estabelecimento.Where(e => e.Estado != "Inativo").ToList();

                return new JsonResult(stockEstabelecimentos.Join(estabelecimentos, se => se.IdEstabelecimento, e => e.IdEstabelecimento,
                    (se,e)=>new
                    { 
                        se.IdStock,
                        se.Nome,
                        se.Preco,
                        se.Stock,
                        se.TipoStock,
                        se.Volume,
                        se.Descricao,
                        NomeEstabelecimento = e.Nome
                    }).ToList());
            }
        }

        /// <summary>
        /// Busca e mostra produtos de um estabelecimento em especifico
        /// </summary>
        /// <returns> Produtos </returns>
        [Route("{idEstabelecimento}")]
        [HttpGet, Authorize(Roles = "Admin, Cliente, Gerente")]
        public IActionResult GetStockByEstabelecimento(int idEstabelecimento)
        {
            using (var context = new FeedyVetContext())
            {
                if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                {
                    string clienteEmail = User.FindFirstValue(ClaimTypes.Email);

                    Cliente c = context.Cliente.Where(c => c.Email == clienteEmail).FirstOrDefault();
                    if (c == null) Forbid();
                }
                else if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                    if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{idEstabelecimento}")) Forbid();

                return new JsonResult(context.StockEstabelecimento.Where(s => s.IdEstabelecimento == idEstabelecimento).ToList());
            }
        }

        [Route("getstockbyid/{idStock}")]
        [HttpGet, Authorize(Roles = "Admin,Gerente,Funcionario")]
        public IActionResult GetStockFromTratamento(int idStock)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    string funcEmail = User.FindFirstValue(ClaimTypes.Email);
                    Funcionario f = context.Funcionario.Where(f => f.Email == funcEmail).FirstOrDefault();
                    if (f == null) return Forbid();

                    StockEstabelecimento stock = context.StockEstabelecimento.Where(st => st.IdStock == idStock).FirstOrDefault();
                    return new JsonResult(stock);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        #endregion

        #region Post
        
        [HttpPost, Authorize(Roles = "Admin, Gerente")] //Metodo para escrever na table
        public IActionResult Post(StockEstabelecimento stock)
        {
            using (var context = new FeedyVetContext())
            {
                if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                    if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{stock.IdEstabelecimento}")) Forbid();

                try
                {
                    StockEstabelecimento stockEst = new StockEstabelecimento();

                    stockEst.IdEstabelecimentoNavigation = context.Estabelecimento
                            .Where(e => e.IdEstabelecimento == stock.IdEstabelecimento).FirstOrDefault();
                    stockEst.Nome = stock.Nome;
                    stockEst.Descricao = stock.Descricao;
                    stockEst.Volume = stock.Volume;
                    stockEst.Stock = stock.Stock;
                    stockEst.Preco = stock.Preco;
                    stockEst.TipoStock = stock.TipoStock;

                    context.StockEstabelecimento.Add(stockEst);

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
        /// Metodo que permite atualizar informção de um produto do stock de um estabelecimento
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        [HttpPatch, Authorize(Roles = "Admin, Gerente")]
        public IActionResult Patch(StockEstabelecimento stock)
        {
            using (var context = new FeedyVetContext())
            {
                if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                    if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{stock.IdEstabelecimento}")) Forbid();

                try
                {
                    StockEstabelecimento stockEst = context.StockEstabelecimento.Where(c => c.IdStock == stock.IdStock).FirstOrDefault();

                    if (stock.IdEstabelecimento != 0) stockEst.IdEstabelecimentoNavigation = context.Estabelecimento
                             .Where(e => e.IdEstabelecimento == stock.IdEstabelecimento).FirstOrDefault();
                    stockEst.Nome = stock.Nome is null ? stockEst.Nome : stock.Nome;
                    stockEst.Descricao = stock.Descricao is null ? stockEst.Descricao : stock.Descricao;
                    stockEst.Volume = stock.Volume is null ? stockEst.Volume : stock.Volume;
                    stockEst.Stock = stock.Stock == 0 ? stockEst.Stock : stock.Stock;
                    stockEst.Preco = stock.Preco == 0 ? stockEst.Preco : stock.Preco;
                    stockEst.TipoStock = stock.TipoStock is null ? stockEst.TipoStock : stock.TipoStock;

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
        /// Metodo que permite apagar stock de um estabelecimento
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    context.StockEstabelecimento.Remove(context.StockEstabelecimento.Where(s => s.IdStock == id).FirstOrDefault());

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

        #region Comentarios
        /*
        [HttpGet] //Metodo para ir buscar os dados na tabela
        public JsonResult Get()
        {
            string query = @"
                select *
                from dbo.Stock_Clinica
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
        */
        #endregion
    }
}

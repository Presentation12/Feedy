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
    public class EncomendaController : ControllerBase
    {
        private readonly IConfiguration _configuration;  //Injeção privada para utilizar certos metodos
        public EncomendaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Get

        /// <summary>
        /// Busca e mostra os dados de todas as encomendas da BD
        /// </summary>
        /// <returns> Dados das encomendas </returns>
        [HttpGet, Authorize(Roles = "Admin")]
        public IEnumerable<Encomenda> Get()
        {
            using (var context = new FeedyVetContext())
            {
                try 
                { 
                    return context.Encomenda.ToList(); 
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }
        }

        /// <summary>
        /// Busca e mostra o historico de encomendas de um utilizador
        /// Mostra os dados da relação da encomenda com os produtos e os dados de cada produto
        /// </summary>
        /// <param name="idCliente"> Id do cliente </param>
        /// <returns> Dados das encomendas </returns>
        [Route("{idCliente}/hclient")]
        [HttpGet, Authorize(Roles = "Cliente, Admin")]
        public IActionResult GetHistoricClient(int idCliente)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    if(User.HasClaim(ClaimTypes.Role, "Cliente"))
                    {
                        string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                        Cliente cliente = context.Cliente.Where(c => c.Email == emailCliente && c.Estado == "Ativo").FirstOrDefault();

                        if (cliente is null) return Forbid();
                    }

                    List<Encomenda> encomendas = context.Encomenda.Where(e => e.IdCliente == idCliente).ToList();
                    List<EncomendaStock> encomendaStocks = context.EncomendaStock.ToList();
                    List<StockEstabelecimento> stockEstabelecimentos = context.StockEstabelecimento.ToList();

                    //Retirar Selects mas tratar do PreçoTotal no Front-End!!
                    return new JsonResult(encomendas
                               .Join(
                               encomendaStocks,
                               e => e.IdEncomenda,
                               es => es.IdEncomenda,
                               (e, es) => new
                               {
                                   e.Data,
                                   e.IdEncomenda,
                                   e.MetodoPagamento,
                                   es.Qtd,
                                   es.IdStock,
                                   e.Estado

                               }).Join(
                        stockEstabelecimentos,
                        x => x.IdStock,
                        se => se.IdStock,
                        (x, se) => new
                        {
                            x,
                            IdProduto = se.IdStock,
                            se.Nome,
                            se.TipoStock,
                            se.Volume,
                            se.Preco, //preco de cada unidade!!!

                        }).Select(y => new {

                        y.x.IdEncomenda,
                        y.x.Data,
                        y.IdProduto,
                        y.Nome,
                        y.TipoStock,
                        y.Volume,
                        y.x.Qtd,
                        y.Preco, //preco de cada unidade!!!
                        precoTotal = y.Preco * y.x.Qtd,
                        y.x.MetodoPagamento,
                        y.x.Estado

                    }).ToList()) ;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }

            }

        }

        /// <summary>
        /// Historico de encomendas de um estabelecimento pelo id do estabelecimento
        /// </summary>
        /// <param name="idEstabelecimento"> Id do estabelecimento </param>
        /// <returns> Encomendas do estabelecimento </returns>
        [Route("{idEstabelecimento}/hestabelecimento")]
        [HttpGet, Authorize(Roles = "Admin, Gerente")]
        public IActionResult GetHistoricoEstabelecimento(int idEstabelecimento)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                    {
                        if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{idEstabelecimento}")) Forbid();
                    }

                    List<StockEstabelecimento> stocks = context.StockEstabelecimento.Where(s => s.IdEstabelecimento == idEstabelecimento).ToList();
                    List<EncomendaStock> encomendaStocks = context.EncomendaStock.ToList();
                    List<Encomenda> encomendas = context.Encomenda.ToList();
                    List<Cliente> clientes = context.Cliente.ToList();

                    var x = stocks
                               .Join(
                               encomendaStocks,
                               s => s.IdStock,
                               es => es.IdStock,
                               (s, es) => new
                               {
                                   es.IdEncomenda,
                                   s.TipoStock,
                                   NomeStock = s.Nome,
                                   s.Preco,
                                   s.Volume,
                                   s.Descricao,
                                   es.Qtd,
                                   PrecoTotal = es.Qtd * s.Preco //preço total da encomenda

                               }).ToList();

                    var x2 = x
                            .Join(
                            encomendas,
                            x => x.IdEncomenda,
                            e => e.IdEncomenda,
                            (x, e) => new
                            {
                                x,
                                e.Data,
                                e.IdCliente,
                                e.MetodoPagamento,
                                e.Estado

                            }).ToList();

                    var x3 = x2
                            .Join(
                            clientes,
                            x2 => x2.IdCliente,
                            c => c.IdCliente,
                            (x2, c) => new
                            {
                                x2,
                                c.Nome,
                                c.Apelido,
                                c.Email,
                                c.Telemovel

                            }).ToList();


                    return new JsonResult(x3.Select(e => new {
                        e.x2.x.IdEncomenda,
                        e.x2.Data,
                        e.x2.x.NomeStock,
                        e.x2.x.TipoStock,
                        e.x2.x.Preco,
                        e.x2.x.Qtd,
                        e.x2.x.Volume,
                        e.x2.Estado,
                        e.x2.MetodoPagamento,
                        e.x2.x.PrecoTotal,
                        e.Nome,
                        e.Apelido,
                        e.Email,
                        e.Telemovel
                    }).ToList());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }
            }
        }

        /// <summary>
        /// Busca e mostra o estado de uma encomenda
        /// </summary>
        /// <param name="idEncomenda"> Id da encomenda </param>
        /// <returns> Estado da encomenda </returns>
        [Route("{idEncomenda}/status")]
        [HttpGet, Authorize(Roles = "Admin, Cliente")]
        public IActionResult GetStatus(int idEncomenda)
        {
            using (var context = new FeedyVetContext())
            {
                try {
                    if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                    {
                        string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                        Cliente cliente = context.Cliente.Where(c => c.Email == emailCliente && c.Estado == "Ativo").FirstOrDefault();

                        if (cliente is null) return Forbid();

                        Encomenda encomenda = context.Encomenda.Where(e => e.IdCliente == cliente.IdCliente && e.IdEncomenda == idEncomenda).FirstOrDefault();

                        if (encomenda is null) return BadRequest();
                    }
                    else
                    {
                        Encomenda encomenda = context.Encomenda.Where(e => e.IdEncomenda == idEncomenda).FirstOrDefault();

                        if (encomenda is null) BadRequest();
                    }

                    return new JsonResult(context.Encomenda.Select(e => new { e.IdEncomenda, e.Estado })
                    .ToList().Where(e => e.IdEncomenda == idEncomenda)); 
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
        /// Envia dados da encomenda para a BD
        /// </summary>
        /// <param name="enc"> Encomenda </param>
        /// <returns> Mensagem do sucedido </returns>
        [HttpPost, Authorize(Roles = "Admin")]
        public IActionResult Post(Encomenda enc)
        {
            using (var context = new FeedyVetContext())
            {
                Encomenda encomenda = new Encomenda();

                try
                {
                    encomenda.IdEncomenda = enc.IdEncomenda;
                    encomenda.IdClienteNavigation = context.Cliente.Where(c => c.IdCliente == enc.IdCliente).FirstOrDefault();
                    encomenda.IdMoradaNavigation = context.Morada.Where(m => m.IdMorada == enc.IdMorada).FirstOrDefault(); 
                    encomenda.Estado = "Pago";
                    encomenda.MetodoPagamento = enc.MetodoPagamento;

                    encomenda.Data = DateTime.Now;

                    context.Encomenda.Add(encomenda);

                    context.SaveChanges();
                    return new JsonResult(encomenda);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }
            }
        }

        /// <summary>
        /// Permite o utilizador realizar uma encomenda 
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        [Route("{idCliente}")]
        [HttpPost, Authorize(Roles = "Admin, Cliente")]
        public IActionResult Encomenda_Cliente(int idCliente, Encomenda enc)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    if(User.HasClaim(ClaimTypes.Role, "Cliente"))
                    {
                        string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                        Cliente cliente = context.Cliente.Where(c => c.Email == emailCliente && c.Estado == "Ativo").FirstOrDefault();

                        if (cliente is null) return Forbid();

                        Encomenda encomenda = new Encomenda();

                        if (idCliente == cliente.IdCliente)
                        {
                            return Post(enc);
                        } 
                        else
                        {
                            return BadRequest();
                        }
                    }
                    else
                    {
                        Encomenda encomenda = new Encomenda();
                        Cliente cliente = context.Cliente.Where(c => c.IdCliente == idCliente).FirstOrDefault();

                        if (idCliente == enc.IdCliente)
                        {
                            return Post(enc);
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }
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
        /// Método que altera a informação de uma encomenda na Base de Dados
        /// </summary>
        /// <returns></returns>
        [HttpPatch, Authorize(Roles = "Admin")]
        public IActionResult Patch(Encomenda enc)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    Encomenda encomenda = context.Encomenda.Where(e => e.IdEncomenda == enc.IdEncomenda).FirstOrDefault();

                    if (enc.IdCliente != 0) encomenda.IdClienteNavigation = context.Cliente.Where(c => c.IdCliente == enc.IdCliente).FirstOrDefault();
                    if (enc.IdMorada != 0) encomenda.IdMoradaNavigation = context.Morada.Where(c => c.IdMorada == enc.IdMorada).FirstOrDefault();
                    encomenda.MetodoPagamento = enc.MetodoPagamento is null ? encomenda.MetodoPagamento : enc.MetodoPagamento;
                    encomenda.Estado = enc.Estado is null ? encomenda.Estado : enc.Estado;

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

        /// <summary>
        /// Metodo auxiliar que verifica a possibilidade de cancelar uma encomenda e faze-lo
        /// </summary>
        /// <param name="encomenda"></param>
        /// <returns></returns>
        public static Encomenda VerifyCancelaEncomenda(Encomenda encomenda)
        {
            if (encomenda.Estado == "Cancelado" || encomenda.Estado == "Prestes a receber")
            {
                throw new ArgumentException("Impossível cancelar esta encomenda", "encomenda");
            }

            encomenda.Estado = "Cancelado";
            return encomenda;
        }

        /// <summary>
        /// Cancelar uma encomenda (Muda o estado dela para cancelado)
        /// </summary>
        /// <param name="idCliente"> Id do cliente </param>
        /// <param name="enc"> Encomenda </param>
        /// <returns> Mensagem do sucedido </returns>
        [Route("canceldelivery/{idEnc}")]
        [HttpPatch, Authorize(Roles = "Admin, Cliente")]
        public IActionResult CancelDelivery(int idEnc)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                    {
                        string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                        Cliente cliente = context.Cliente.Where(c => c.Email == emailCliente && c.Estado == "Ativo").FirstOrDefault();

                        if (cliente is null) return Forbid();

                        Encomenda encomenda = context.Encomenda.Where(e => e.IdEncomenda == idEnc && e.IdCliente == cliente.IdCliente).FirstOrDefault();

                        if (encomenda is null) return null;
                        encomenda = VerifyCancelaEncomenda(encomenda);

                    }
                    else
                    {
                        Encomenda encomenda = context.Encomenda.Where(e => e.IdEncomenda == idEnc).FirstOrDefault();

                        if (encomenda is null) return null;

                        encomenda = VerifyCancelaEncomenda(encomenda);
                    }

                    context.SaveChanges();

                    return Ok();
                }
                catch (ArgumentException ae)
                {
                    Console.WriteLine(ae);
                    return BadRequest();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }
            }
        }

        [Route("{idEncomenda}/payment")]
        [HttpPatch, Authorize(Roles = "Admin, Cliente")]
        public IActionResult PagamentoServico(int idEncomenda)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    if(User.HasClaim(ClaimTypes.Role, "Cliente"))
                    {
                        string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                        Cliente cliente = context.Cliente.Where(c => c.Email == emailCliente && c.Estado == "Ativo").FirstOrDefault();

                        if (cliente is null) return Forbid();
                    }

                    Encomenda encomenda = context.Encomenda.Where(s => s.IdEncomenda == idEncomenda).FirstOrDefault();

                    encomenda.Estado = "Pago";

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
        /// Metodo apaga uma Encomenda da base de dados 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}"), Authorize(Roles = "Admin")] //Metodo para dar delete dos valores na table //Passar por referencia por ser com o ID
        public IActionResult Delete(int id)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    context.Encomenda.Remove(context.Encomenda.Where(e => e.IdEncomenda == id).FirstOrDefault());

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
            string query = @"select * from dbo.Encomenda";
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

        /*
        [HttpPost] //Metodo para escrever na table
            public JsonResult Post(Encomenda encomenda)
            {
                String query = @"
                    insert into dbo.Encomenda
                    (id_cliente,id_morada,estado,metodo_pagamento,desconto)
                    values
                    ( 
                    '" + encomenda.IdCliente + @"'
                    ,'" + encomenda.IdMorada + @"'
                    ,'" + encomenda.Estado + @"'
                    ,'" + encomenda.MetodoPagamento + @"'
                    ,'" + encomenda.Desconto + @"'
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

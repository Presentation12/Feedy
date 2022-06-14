using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using FeedyVetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FeedyVetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvaliacaoEstabelecimentoUtilizadorController : ControllerBase
    {
        private readonly IConfiguration _configuration;  //Injeção privada para utilizar certos metodos
        public AvaliacaoEstabelecimentoUtilizadorController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Get

        /// <summary>
        /// Busca e mostra os atributos de todas as avaliações da BD  
        /// </summary>
        /// <returns> Dados de todas as avaliações </returns>
        [HttpGet, Authorize(Roles = "Admin")]
        public IEnumerable<AvaliacaoEstabelecimentoUtilizador> Get()
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    return context.AvaliacaoEstabelecimentoUtilizador.ToList();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Busca e mostra os atributos de todas as avaliações da BD em que o seu estado é ativo
        /// </summary>
        /// <returns> Dados de todas as avaliações ativas </returns>
        [Route("ativas"), Authorize(Roles = "Admin,Cliente")]
        [HttpGet]
        public JsonResult GetAtivas()
        {
            using (var context = new FeedyVetContext())
            {
                List<AvaliacaoEstabelecimentoUtilizador> avaliacoes = context.AvaliacaoEstabelecimentoUtilizador.ToList();
                List<AvaliacaoEstabelecimentoUtilizador> avaliacoesAtivas = new List<AvaliacaoEstabelecimentoUtilizador>();
                foreach (AvaliacaoEstabelecimentoUtilizador avaliacao in avaliacoes)
                {
                    int idEstabelecimento = avaliacao.IdEstabelecimento;
                    Estabelecimento estabelecimento = context.Estabelecimento.Where(e => e.IdEstabelecimento == idEstabelecimento).FirstOrDefault();
                    if (estabelecimento.Estado == "Ativo")
                    {
                        avaliacoesAtivas.Add(avaliacao);
                    }
                }
                return new JsonResult(avaliacoesAtivas.ToList());
            }
        }

        /// <summary>
        /// Visualizar as avaliações de uma clinica em que o funcionario tenha permissões
        /// </summary>
        /// <param name="idFuncionario"> Id do funcionario </param>
        /// <returns> Dados de todas as avaliações da clinica </returns>
        [Route("{idEstabelecimento}/checkavaliacoes")]
        [HttpGet, Authorize(Roles = "Admin,Gerente")]
        public IActionResult CheckAvaliacoesGerente(int idEstabelecimento)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    List<Cliente> clientes = context.Cliente.ToList();

                    if (User.HasClaim(ClaimTypes.Role, $"Gerente_{idEstabelecimento}"))
                    {
                        List<AvaliacaoEstabelecimentoUtilizador> ava = context.AvaliacaoEstabelecimentoUtilizador.Where(a => a.IdEstabelecimento == idEstabelecimento).ToList();
                        return new JsonResult(ava.Join(clientes, a => a.IdCliente, c => c.IdCliente, (a, c) => new {
                            Nome = c.Nome,
                            ClienteFoto = c.ClienteFoto,
                            Texto = a.Texto,
                            Avaliacao = a.Avaliacao,
                            IdEstabelecimento = a.IdEstabelecimento,
                            DataAvaliacao = a.DataAvaliacao
                        }).ToList());
                    }
                    else if (User.HasClaim(ClaimTypes.Role, "Admin"))
                    {
                        List<AvaliacaoEstabelecimentoUtilizador> ava = context.AvaliacaoEstabelecimentoUtilizador.Where(a => a.IdEstabelecimento == idEstabelecimento).ToList();
                        return new JsonResult(ava.Join(clientes, a=>a.IdCliente, c=>c.IdCliente,(a,c) => new { 
                            Nome = c.Nome,
                            ClienteFoto = c.ClienteFoto,
                            Texto = a.Texto,
                            Avaliacao = a.Avaliacao,
                            IdEstabelecimento = a.IdEstabelecimento,
                            DataAvaliacao = a.DataAvaliacao
                        }).ToList());
                    }

                    return Forbid();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        /// <summary>
        /// Visualizar as avaliações de uma clinica
        /// </summary>
        /// <param name="idFuncionario"> Id do Estabelecimento </param>
        /// <returns> Dados de todas as avaliações da clinica </returns>
        [Route("{idEstabelecimento}/checkavaliacoesestabelecimento")]
        [HttpGet, Authorize(Roles = "Admin,Cliente")]
        public IActionResult CheckAvaliacoesCliente(int idEstabelecimento)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    List<AvaliacaoEstabelecimentoUtilizador> avaliacoes = context.AvaliacaoEstabelecimentoUtilizador.Where(ae => ae.IdEstabelecimento == idEstabelecimento).ToList();

                    return new JsonResult(avaliacoes);
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

        /// <summary>
        /// Envia os dados de uma avaliação para o servidor
        /// </summary>
        /// <param name="idUser"> Id do utilizador </param>
        /// <param name="avaliacao"> Avaliacao </param>
        /// <returns> Mensagem do sucedido </returns>
        [HttpPost, Authorize(Roles = "Admin")]
        public IActionResult Post(int idUser, AvaliacaoEstabelecimentoUtilizador avaliacao)
        {
            try
            {
                double counter = 0;
                int counter2 = 0;

                using (var context = new FeedyVetContext())
                {
                    AvaliacaoEstabelecimentoUtilizador ava = new AvaliacaoEstabelecimentoUtilizador();
                    List<AvaliacaoEstabelecimentoUtilizador> list = context.AvaliacaoEstabelecimentoUtilizador
                        .Where(a => a.IdEstabelecimento == avaliacao.IdEstabelecimento).ToList();
                    Estabelecimento estabelecimento = context.Estabelecimento.Where(c => c.IdEstabelecimento == avaliacao.IdEstabelecimento).FirstOrDefault();
                    Cliente cliente = context.Cliente.Where(c => c.IdCliente == idUser).FirstOrDefault();

                    Console.WriteLine("Boas");

                    ava.IdEstabelecimentoNavigation = estabelecimento;
                    ava.IdClienteNavigation = cliente;
                    ava.Avaliacao = avaliacao.Avaliacao;
                    ava.Texto = avaliacao.Texto;
                    ava.DataAvaliacao = DateTime.Now;

                    Console.WriteLine(ava.IdEstabelecimento);
                    Console.WriteLine(ava.IdCliente);
                    Console.WriteLine(ava.Avaliacao);
                    Console.WriteLine(ava.Texto);
                    Console.WriteLine(ava.DataAvaliacao);

                    foreach (AvaliacaoEstabelecimentoUtilizador a in list)
                    {
                        counter += a.Avaliacao;
                        counter2++;
                    }

                    if (estabelecimento.AvaliacaoMedia == 0) estabelecimento.AvaliacaoMedia = (float)ava.Avaliacao;
                    else estabelecimento.AvaliacaoMedia = (float)(counter / counter2);
                   
                    context.AvaliacaoEstabelecimentoUtilizador.Add(ava);
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

        [Route("rateestabelecimento")]
        [HttpPost, Authorize(Roles = "Cliente, Admin")]
        public IActionResult AvaliarClinica(AvaliacaoEstabelecimentoUtilizador avaliacao)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                    Cliente cliente = context.Cliente.Where(c => c.Email == emailCliente && c.Estado == "Ativo").FirstOrDefault();
                    
                    if (cliente is null) return Forbid();
                    
                    return Post(cliente.IdCliente, avaliacao);
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
        /// Altera os dados de uma avaliação a um estabelecimento
        /// </summary>
        /// <param name="avaliacao"></param>
        /// <returns></returns>
        [HttpPatch, Authorize(Roles = "Cliente, Admin")]
        public IActionResult Patch(AvaliacaoEstabelecimentoUtilizador avaliacao)
        {
            try
            {
                double counter = 0;
                int counter2 = 0;

                using (var context = new FeedyVetContext())
                {
                    AvaliacaoEstabelecimentoUtilizador ava = context.AvaliacaoEstabelecimentoUtilizador
                        .Where(a => a.IdAvaliacaoEstabelecimentoUtilizador == avaliacao.IdAvaliacaoEstabelecimentoUtilizador).FirstOrDefault();

                    List<AvaliacaoEstabelecimentoUtilizador> list = context.AvaliacaoEstabelecimentoUtilizador
                        .Where(a => a.IdEstabelecimento == avaliacao.IdEstabelecimento).ToList();

                    Estabelecimento estabelecimento = context.Estabelecimento.Where(c => c.IdEstabelecimento == avaliacao.IdEstabelecimento).FirstOrDefault();

                    if (avaliacao.IdEstabelecimento != 0) ava.IdEstabelecimentoNavigation = estabelecimento;
                    if (avaliacao.IdEstabelecimento != 0) ava.IdClienteNavigation = context.Cliente.Where(c => c.IdCliente == avaliacao.IdCliente).FirstOrDefault();
                    ava.Avaliacao = avaliacao.Avaliacao == 0 ? ava.Avaliacao : avaliacao.Avaliacao;
                    ava.Texto = avaliacao.Texto is null ? ava.Texto : avaliacao.Texto;

                    foreach (AvaliacaoEstabelecimentoUtilizador a in list)
                    {
                        counter += a.Avaliacao;
                        counter2++;
                    }

                    estabelecimento.AvaliacaoMedia = (float)(counter / counter2);
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

        #region Delete

        /// <summary>
        /// Método de apagar uma avaliação da base de dados
        /// </summary>
        /// <param name="id"> ID da avaliação </param>
        /// <returns> Resultado da operação DELETE </returns>
        [HttpDelete("{id}"), Authorize(Roles = "Admin")] 
        public IActionResult Delete(int id)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    AvaliacaoEstabelecimentoUtilizador ava = context.AvaliacaoEstabelecimentoUtilizador
                        .Where(a => a.IdAvaliacaoEstabelecimentoUtilizador == id).FirstOrDefault();

                    context.AvaliacaoEstabelecimentoUtilizador.Remove(ava);
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

        /* [HttpPost] 
         public JsonResult Post(AvaliacaoEstabelecimentoUtilizador avaliacao)
         {
             String query = @"
                     insert into dbo.Avaliacao_Clinica_Utilizador
                     (id_clinica,id_cliente,avaliacao,texto,data_avaliacao)
                     values
                     ( 
                     '" + avaliacao.IdEstabelecimento + @"'
                     ,'" + avaliacao.IdCliente + @"'
                     ,'" + avaliacao.Avaliacao + @"'
                     ,'" + avaliacao.Texto + @"'
                     ,'" + avaliacao.DataAvaliacao + @"'
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
         }*/

        #endregion
    }
}

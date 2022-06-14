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
    public class EstabelecimentoController : ControllerBase
    {
        private readonly IConfiguration _configuration;  //Injeção privada para utilizar certos metodos
        private readonly IWebHostEnvironment _env;
        struct Pagamento
        {
            public string tipo { get; set; }
            public decimal preco { get; set; }
            public DateTime data { get; set; }
        }

        public EstabelecimentoController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        #region Get

        /// <summary>
        /// Busca e mostra os dados dos estabelecimentos ativos
        /// </summary>
        /// <returns> Dados dos estabelecimentos inativos </returns>
        [HttpGet, Authorize(Roles = "Admin, Cliente")]
        public JsonResult Get()
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    List<Estabelecimento> estabelecimentos = context.Estabelecimento.ToList();
                    List<Morada> moradas = context.Morada.ToList();

                    var x = estabelecimentos
                        .Join(
                        moradas,
                        e => e.IdMorada,
                        m => m.IdMorada,
                        (e, m) => new
                        {
                            e.AvaliacaoMedia,
                            e.Contacto,
                            e.Estado,
                            e.Nome,
                            e.IdEstabelecimento,
                            e.TipoEstabelecimento,
                            e.EstabelecimentoFoto,
                            m.Pais,
                            m.Distrito,
                            m.Concelho,
                            m.Freguesia,
                            m.Rua,
                            m.Porta,
                            m.Andar,
                            m.CodigoPostal

                        }).ToList();

                    return new JsonResult(x.Where(x => x.Estado != "Inativo").ToList());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }
        }

        [Route("getestabelecimentobyidservico/{idServico}")]
        [HttpGet, Authorize(Roles = "Admin,Funcionario,Gerente")]
        public IActionResult GetEstabelecimentoByIdServico(int idServico)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    Servico s = context.Servico.Where(s => s.IdServico == idServico).FirstOrDefault();
                    Estabelecimento e = context.Estabelecimento.Where(e => e.IdEstabelecimento == s.IdEstabelecimento).FirstOrDefault();
                    return new JsonResult(e);
                }
            } catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        /// <summary>
        /// Busca e mostra os distritos existentes de estabelecimentos ativos
        /// </summary>
        /// <returns> Distritos de estabelecimentos ativos </returns>
        [Route("distritos")]
        [HttpGet, Authorize(Roles = "Admin, Cliente")]
        public JsonResult GetDistritosExistentes()
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    List<Estabelecimento> estabelecimentos = context.Estabelecimento.ToList();
                    List<Morada> moradas = context.Morada.ToList();

                    var distritos = estabelecimentos
                        .Join(
                        moradas,
                        e => e.IdMorada,
                        m => m.IdMorada,
                        (e, m) => new
                        {
                            e.Estado,
                            m.Distrito

                        }).ToList();

                    List<string> dis = new List<string>();
                    foreach (var d in distritos)
                    {
                        if (!dis.Contains(d.Distrito) && d.Estado == "Ativo")
                        {
                            dis.Add(d.Distrito);
                        }
                    }

                    return new JsonResult(dis);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }
        }

        /// <summary>
        /// Busca e mostra os dados dos estabelecimentos
        /// </summary>
        /// <returns> Dados dos estabelecimentos </returns>
        [Route("getallestabelecimentos")]
        [HttpGet, Authorize(Roles = "Admin, Cliente")]
        public IEnumerable<Estabelecimento> GetAll()
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    return context.Estabelecimento.ToList();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }
        }

        /// <summary>
        /// Método que procura Estabelecimentos que são escolas de cães
        /// </summary>
        /// <returns></returns>
        [Route("GetEscolasCaes"), Authorize(Roles = "Admin, Cliente")]
        public IEnumerable<Estabelecimento> GetEscolasCaes()
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    return context.Estabelecimento.Where(e => e.TipoEstabelecimento == "Escola_Caes" && e.Estado != "Inativo").ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }
        }

        /// <summary>
        /// Busca e mostra os dados do estabelecimento pelo id caso este seja ativo
        /// </summary>
        /// <param name="idEstabelecimento"> Id do estabelecimento </param>
        /// <returns> Dados do estabelecimento </returns>
        [HttpGet("{idEstabelecimento}"), Authorize(Roles = "Admin, Gerente, Cliente")]
        public IActionResult GetEstabelecimentoByID(int idEstabelecimento)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    if (User.HasClaim(ClaimTypes.Role, $"Gerente_{idEstabelecimento}"))
                        return new JsonResult(context.Estabelecimento.Where(e => e.IdEstabelecimento == idEstabelecimento && e.Estado != "Inativo").FirstOrDefault());
                    else if (User.HasClaim(ClaimTypes.Role, "Admin"))
                        return new JsonResult(context.Estabelecimento.Where(e => e.IdEstabelecimento == idEstabelecimento && e.Estado != "Inativo").FirstOrDefault());
                    else if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                        return new JsonResult(context.Estabelecimento.Where(e => e.IdEstabelecimento == idEstabelecimento && e.Estado != "Inativo").FirstOrDefault());

                    return Forbid();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }
        }

        /// <summary>
        /// Busca e mostra o nome dos estabelecimentos ativos
        /// </summary>
        /// <returns> Nome dos estabelecimentos ativos </returns>
        [Route("GetAllEstabelecimentosNames")]
        public JsonResult GetAllEstabelecimentosNames()
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    return new JsonResult(context.Estabelecimento.Where(e => e.Estado != "Inativo").Select(e => new { e.Nome, e.EstabelecimentoFoto }).ToList());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return new JsonResult("Error");
                }
            }
        }

        /// <summary>
        /// Busca lista dos pagamentos de encomendas e serviços
        /// </summary>
        /// <param name="idEstabelecimento">id do cliente</param>
        /// <returns>Lista de pagamentos</returns>
        [HttpGet("{idEstabelecimento}/getmoney"), Authorize(Roles = "Admin, Gerente")]
        public IActionResult GetPagamentosEstabelecimento(int idEstabelecimento)
        {
            using (var context = new FeedyVetContext())
            {
                if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                {
                    if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{idEstabelecimento}")) return Forbid();
                }
                else return Forbid();

                List<Pagamento> pagamentos = new List<Pagamento>();
                Estabelecimento estabelecimento = context.Estabelecimento.Where(e => e.IdEstabelecimento == idEstabelecimento).FirstOrDefault();
                List<Servico> servicos = context.Servico.Where(s => s.IdEstabelecimento == idEstabelecimento && (s.Estado == "Concluido" || s.Estado == "Pago")).ToList();
                List<ServicoCatalogo> servicoCatalogos = context.ServicoCatalogo.Where(sc => sc.IdEstabelecimento == idEstabelecimento).ToList();
                List<Encomenda> encomendas = context.Encomenda.Where(e => e.Estado == "Concluido" || e.Estado == "Pago").ToList();
                List<EncomendaStock> encomendaStocks = context.EncomendaStock.ToList();
                List<StockEstabelecimento> stocks = context.StockEstabelecimento.Where(se => se.IdEstabelecimento == idEstabelecimento).ToList();
                bool check;

                Pagamento p = new Pagamento();

                foreach (Servico s in servicos)
                {
                    p.data = s.DataServico;
                    p.tipo = "servico";

                    foreach (ServicoCatalogo sc in servicoCatalogos)
                    {
                        if (s.IdServicoCatalogo == sc.IdServicoCatalogo)
                        {
                            p.preco = (decimal)sc.Preco;
                        }
                    }
                    pagamentos.Add(p);
                }

                foreach (Encomenda e in encomendas)
                {

                    check = false;

                    foreach (EncomendaStock es in encomendaStocks)
                    {
                        if (e.IdEncomenda == es.IdEncomenda)
                        {

                            foreach (StockEstabelecimento s in stocks)
                            {
                                if (es.IdStock == s.IdStock && s.IdEstabelecimento == idEstabelecimento)
                                {

                                    if (check == false)
                                    {
                                        p.data = e.Data;
                                        p.tipo = "encomenda";
                                        p.preco = 0;
                                        check = true;
                                    }

                                    if (check == true)
                                    {
                                        p.preco += (es.Qtd * s.Preco);
                                    }

                                }
                            }
                        }
                    }
                    if (check == true && (p.preco != (decimal)0.0))
                    {
                        pagamentos.Add(p);
                    }

                }

                return new JsonResult(pagamentos.ToList());
            }
        }

        /// <summary>
        /// Retorna um estabelecimento pelo ID do Gerente
        /// </summary>
        /// <param name="idGerente"></param>
        /// <returns></returns>
        [HttpGet("estabelecimentoger/{idGerente}"), Authorize(Roles = "Admin, Gerente")]
        public IActionResult GetEstabelecimentoByGerente(int idGerente)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    EstabelecimentoGerente eg = context.EstabelecimentoGerente.Where(e => e.IdFuncionario == idGerente).FirstOrDefault();

                    if (eg is null) return BadRequest();

                    Estabelecimento estabelecimento = context.Estabelecimento.Where(e => e.IdEstabelecimento == eg.IdEstabelecimento).FirstOrDefault();

                    if (estabelecimento is null) BadRequest();

                    return new JsonResult(estabelecimento);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        /// <summary>
        /// Busca valor adquirido numa certa clinica e num diariamente
        /// </summary>
        /// <param name="idEstabelecimento"></param>
        /// <returns>data, valor total</returns>
        [HttpGet("{idEstabelecimento}/getdaily"), Authorize(Roles = "Admin, Gerente")]
        public IActionResult GetValorDiario(int idEstabelecimento)
        {
            if (User.HasClaim(ClaimTypes.Role, "Gerente"))
            {
                if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{idEstabelecimento}")) return Forbid();
            }

            using (var context = new FeedyVetContext())
            {   
                Estabelecimento estabelecimento = context.Estabelecimento.Where(e => e.IdEstabelecimento == idEstabelecimento).FirstOrDefault();
                List<Servico> servicos = context.Servico.Where(s => s.IdEstabelecimento == idEstabelecimento && (s.Estado == "Concluido" || s.Estado == "Pago")).ToList();
                List<ServicoCatalogo> servicoCatalogos = context.ServicoCatalogo.Where(sc => sc.IdEstabelecimento == idEstabelecimento).ToList();
                List<Encomenda> encomendas = context.Encomenda.Where(e => e.Estado == "Concluido" || e.Estado == "Pago").ToList();
                List<EncomendaStock> encomendaStocks = context.EncomendaStock.ToList();
                List<StockEstabelecimento> stocks = context.StockEstabelecimento.Where(se => se.IdEstabelecimento == idEstabelecimento).ToList();
                decimal valorTotal = 0;

                foreach (Servico s in servicos)
                {

                    foreach (ServicoCatalogo sc in servicoCatalogos)
                    {
                        if (s.IdServicoCatalogo == sc.IdServicoCatalogo && DateTime.Compare(s.DataServico.Date, DateTime.Today.Date) == 0)
                        {
                            valorTotal += (decimal)sc.Preco;
                        }
                    }
                }

                foreach (Encomenda e in encomendas)
                {
                    foreach (EncomendaStock es in encomendaStocks)
                    {  
                        if (e.IdEncomenda == es.IdEncomenda)
                        {
                            foreach (StockEstabelecimento s in stocks)
                            {
                                if (es.IdStock == s.IdStock && s.IdEstabelecimento == idEstabelecimento && DateTime.Compare(e.Data, DateTime.Today.Date) ==0) 
                                {
                                    valorTotal += es.Qtd * s.Preco;
                                }
                            }
                        }
                    }
                }

                return new JsonResult(valorTotal);
            }
        }

        /// <summary>
        /// Busca valor adquirido numa certa clinica e num certo dia
        /// </summary>
        /// <param name="idEstabelecimento"></param>
        /// <returns>data, valor total</returns>
        [HttpGet("{idEstabelecimento}/{dia}_{mes}_{ano}/getdatevalues"), Authorize(Roles = "Admin, Gerente")]
        public IActionResult GetValorDate(int idEstabelecimento, int dia, int mes, int ano)
        {
            if (User.HasClaim(ClaimTypes.Role, "Gerente"))
            {
                if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{idEstabelecimento}")) return Forbid();
            }
            else return Forbid();

            using (var context = new FeedyVetContext())
            {
                DateTime data = new DateTime(ano, mes, dia);
                Estabelecimento estabelecimento = context.Estabelecimento.Where(e => e.IdEstabelecimento == idEstabelecimento).FirstOrDefault();
                List<Servico> servicos = context.Servico.Where(s => s.IdEstabelecimento == idEstabelecimento && (s.Estado == "Concluido" || s.Estado == "Pago")).ToList();
                List<ServicoCatalogo> servicoCatalogos = context.ServicoCatalogo.Where(sc => sc.IdEstabelecimento == idEstabelecimento).ToList();
                List<Encomenda> encomendas = context.Encomenda.Where(e => e.Estado == "Concluido" || e.Estado == "Pago").ToList();
                List<EncomendaStock> encomendaStocks = context.EncomendaStock.ToList();
                List<StockEstabelecimento> stocks = context.StockEstabelecimento.Where(se => se.IdEstabelecimento == idEstabelecimento).ToList();
                decimal valorTotal = 0;

                foreach (Servico s in servicos)
                {
                    foreach (ServicoCatalogo sc in servicoCatalogos)
                    {
                        if (s.IdServicoCatalogo == sc.IdServicoCatalogo && DateTime.Compare(s.DataServico.Date, data.Date) == 0)
                        {
                            valorTotal += (decimal)sc.Preco;
                        }
                    }
                }

                foreach (Encomenda e in encomendas)
                {
                    foreach (EncomendaStock es in encomendaStocks)
                    {
                        if (e.IdEncomenda == es.IdEncomenda)
                        {
                            foreach (StockEstabelecimento s in stocks)
                            {
                                if (es.IdStock == s.IdStock && s.IdEstabelecimento == idEstabelecimento && DateTime.Compare(e.Data, data.Date) == 0)
                                {
                                    valorTotal += es.Qtd * s.Preco;
                                }
                            }
                        }
                    }
                }
                return new JsonResult("O valor total adquirido no dia "+ data.Date.ToShortDateString() + " foi de " + valorTotal);
            }
        }

        #endregion

        #region Post

        /// <summary>
        /// Envia os dados de um estabelecimento para a BD
        /// </summary>
        /// <param name="est"> Estabelecimento </param>
        /// <returns> Mensagem do sucedido </returns>
        [HttpPost, Authorize(Roles = "Admin")]
        public IActionResult Post(Estabelecimento est)
        {
            using (var context = new FeedyVetContext())
            {
                Estabelecimento estabelecimento = new Estabelecimento();

                try
                {
                    estabelecimento.IdEstabelecimento = est.IdEstabelecimento;
                    estabelecimento.Nome = est.Nome;
                    estabelecimento.Estado = est.Estado;
                    estabelecimento.IdMoradaNavigation = context.Morada.Where(m => m.IdMorada == estabelecimento.IdMorada).FirstOrDefault();
                    estabelecimento.Contacto = est.Contacto;
                    estabelecimento.TipoEstabelecimento = est.TipoEstabelecimento;
                    estabelecimento.EstabelecimentoFoto = "https://localhost:5001/images/house.png";

                    context.Estabelecimento.Add(estabelecimento);

                    context.SaveChanges();
                    return Ok();
                } catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }
            } 
        }  

        /// <summary>
        /// Guardar foto
        /// </summary>
        /// <returns> Nome do ficheiro </returns>
        [Route("SaveFile")]
        [HttpPost, Authorize(Roles = "Admin, Cliente")]
        public IActionResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + filename;

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        #endregion

        #region Update

        /// <summary>
        /// Alterar dados parcialmente do estabelecimento
        /// </summary>
        /// <param name="est"> Estabelecimento </param>
        /// <returns> Dados do estabelecimento </returns>
        [HttpPatch, Authorize(Roles = "Admin, Gerente")]
        public IActionResult Patch(Estabelecimento est)
        {
            using (var context = new FeedyVetContext())
            {
                if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                {
                    if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{est.IdEstabelecimento}")) return Forbid();
                }
                else return Forbid();

                try
                {
                    Estabelecimento estabelecimento = context.Estabelecimento.Where(c => c.IdEstabelecimento == est.IdEstabelecimento).FirstOrDefault();

                    estabelecimento.Nome = est.Nome is null ? estabelecimento.Nome : est.Nome;
                    estabelecimento.Estado = est.Estado is null ? estabelecimento.Estado : est.Estado;
                    if(est.IdMorada != 0) estabelecimento.IdMoradaNavigation = context.Morada
                            .Where(m => m.IdMorada == est.IdMorada).FirstOrDefault();
                    estabelecimento.Contacto = est.Contacto is 0 ? estabelecimento.Contacto : est.Contacto;
                    estabelecimento.EstabelecimentoFoto = est.EstabelecimentoFoto is null ? estabelecimento.EstabelecimentoFoto : est.EstabelecimentoFoto;
                    
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

        /// <summary>
        /// Alterar estado de um estabelecimento
        /// </summary>
        /// <param name="est"> Estabelecimento </param>
        /// <param name="idFuncionario"> Id do Funcionario </param>
        /// <returns></returns>
        [Route("{id}/changestatusclinica")]
        [HttpPatch, Authorize(Roles = "Admin, Gerente")]
        public IActionResult ChangeStatusEstabelecimento(Estabelecimento est, int idFuncionario)
        {
            using (var context = new FeedyVetContext())
            {
                if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                {
                    if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{est.IdEstabelecimento}")) return Forbid();
                }
                else return Forbid();

                try
                {
                    Estabelecimento aux = context.Estabelecimento.Where(aux2 => aux2.IdEstabelecimento == est.IdEstabelecimento).FirstOrDefault();

                    aux.Estado = est.Estado;
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

        public static bool VerificaPodeDesativarEstabelecimento(int id_estabelecimento)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    List<Servico> servicos = context.Servico.Where(
                            s => s.IdEstabelecimento == id_estabelecimento
                            && (s.Estado != "Inativo"
                            && s.DataServico > DateTime.Now)).ToList();

                    if (servicos.Count == 0) return true;

                    throw new ArgumentException("Esta clinica já se encontra inativa ou com serviços por realizar", "id_estabelecimento");
                }
                catch (ArgumentException ae)
                {
                    Console.WriteLine(ae.Message);
                    return false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
        }

        /// <summary>
        /// Cancelar parceria (mudar o estado do estabelecimento para inativo)
        /// </summary>
        /// <param name="id_estabelecimento"> Id do estabelecimento </param>
        /// <returns> Mensagem do sucedido </returns>
        [HttpDelete("{id_estabelecimento}"), Authorize(Roles = "Admin, Gerente")]
        public IActionResult CancelarParceria(int id_estabelecimento)
        {
            using (var context = new FeedyVetContext())
            {
                if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                {
                    if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{id_estabelecimento}")) return Forbid();
                }
                else return Forbid();

                try
                {
                    Estabelecimento estabelecimento = context.Estabelecimento.Where(e => e.IdEstabelecimento == id_estabelecimento).FirstOrDefault();


                    if(VerificaPodeDesativarEstabelecimento(estabelecimento.IdEstabelecimento))
                    {
                        estabelecimento.Estado = "Inativo";
                        context.SaveChanges();
                        return Ok();
                    }

                    return new JsonResult("Neste momento é impossível cancelar a sua parceria connosco, " +
                        "verifique se ainda tem serviços agendados neste estabelecimento");
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
            string query = @" select * from dbo.Clinica";
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
        }*/

        /*[HttpPost] //Metodo para escrever na table
        public JsonResult Post(Clinica clinica)
        {
            String query = @"
                insert into dbo.Clinica
                (nome,estado,id_morada,contacto,clinica_foto)
                values
                ( 
                '" + clinica.Nome + @"'
                ,'" + clinica.Estado + @"'
                ,'" + clinica.IdMorada + @"'
                ,'" + clinica.Contacto + @"'
                ,'" + clinica.ClinicaFoto + @"'
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
        /*[HttpPut] //Metodo para dar update dos valores na table
            public JsonResult Put(Clinica clinica)
            {
                string query = @"
                    update dbo.Clinica set
                    nome = '" + clinica.Nome + @"'
                    ,estado = '" + clinica.Estado + @"'
                    ,id_morada = '" + clinica.IdMorada + @"'
                    ,contacto = '" + clinica.Contacto + @"'
                    ,clinica_foto = '" + clinica.ClinicaFoto + @"'
                    Where id_clinica = " + clinica.IdClinica + @"
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
            }*/
        #endregion
    }
}

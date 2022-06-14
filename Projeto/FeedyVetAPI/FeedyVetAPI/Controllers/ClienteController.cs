using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Data;
using FeedyVetAPI.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace FeedyVetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public ClienteController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        #region Get

        /// <summary>
        /// Busca e mostra os dados de todos os clientes com as suas passwords escondidas
        /// </summary>
        /// <returns> Dados dos utilizadores </returns>
        [HttpGet, Authorize(Roles = "Admin")]
        public IEnumerable<Cliente> Get()
        {
            using (var context = new FeedyVetContext())
            {
                List<Cliente> ClientesObtidos = context.Cliente.ToList();
                foreach (Cliente c in ClientesObtidos)
                {
                    c.Pass = "Hidden";
                    c.PassSalt = "Hidden";
                }
                return ClientesObtidos;
            }
        }

        /// <summary>
        /// Obter Cliente pelo token
        /// </summary>
        /// <returns></returns>
        [HttpGet("getclientebytoken"), Authorize(Roles = "Cliente")]
        public IActionResult GetClienteByToken()
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                    Cliente c = context.Cliente.Where(c => c.Email == emailCliente).FirstOrDefault();
                    if (c == null) return BadRequest();
                    return new JsonResult(c);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }

        }

        /// <summary>
        /// Busca e mostra os dados de um cliente com as suas passwords escondidas
        /// </summary>
        /// <param name="idCliente"> Id do cliente</param>
        /// <returns> Dados do cliente </returns>
        [HttpGet("{idCliente}"), Authorize]
        // Admin,Gerente,Funcionario,Cliente
        // Se for Gerente / Funcionario, o cliente tem de ter pelo menos um serviço lá feito
        // Se for cliente, tem de ser este cliente!
        public IActionResult GetClienteByID(int idCliente)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    bool authorized = false;
                    if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                    {
                        string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                        Cliente c = context.Cliente.Where(c => c.Email == emailCliente).FirstOrDefault();
                        if (c != null && c.IdCliente == idCliente)
                        {
                            authorized = true;
                        }
                    }

                    if (User.HasClaim(ClaimTypes.Role, "Gerente") || User.HasClaim(ClaimTypes.Role, "Funcionario"))
                    {
                        string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                        Funcionario f = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();
                        if (f == null) return BadRequest();

                        List<FuncionarioEstabelecimento> fe = context.FuncionarioEstabelecimento.Where(fe => fe.IdFuncionario == f.IdFuncionario).ToList();

                        foreach (FuncionarioEstabelecimento fe_aux in fe)
                        {
                            if (context.Servico.Where(s => s.IdEstabelecimento == fe_aux.IdEstabelecimento && s.IdCliente == idCliente).ToList().Count != 0)
                            {
                                authorized = true;
                            }
                        }

                    }

                    if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                    if (authorized == true)
                    {
                        return new JsonResult(context.Cliente.Where(cliente => cliente.IdCliente == idCliente).Select(c => new {
                            c.IdCliente,
                            c.Nome,
                            c.Apelido,
                            c.IdMorada,
                            c.Email,
                            c.Telemovel,
                            c.ClienteFoto,
                            c.Estado,
                            c.IdMoradaNavigation
                        }).FirstOrDefault());
                    }
                    else return Forbid();


                } catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }
            }
        }

        [HttpGet("getclientebyservico/{idServico}"), Authorize(Roles = "Admin,Gerente,Funcionario")]
        public IActionResult GetClienteByIDServico(int idServico)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    try
                    {
                        Servico s = context.Servico.Where(s => s.IdServico == idServico).FirstOrDefault();
                        Cliente c = context.Cliente.Where(c => c.IdCliente == s.IdCliente).FirstOrDefault();
                        return new JsonResult(c);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
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

        [HttpGet("getclientebyanimal/{idAnimal}"), Authorize]
        public IActionResult GetClienteByIDAnimal(int idAnimal)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    bool authorized = false;
                    if (User.HasClaim(ClaimTypes.Role, "Gerente") || (User.HasClaim(ClaimTypes.Role, "Funcionario") && !User.HasClaim(ClaimTypes.Role, "Admin")))
                    {
                        string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                        Funcionario f = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();
                        if (f == null) return BadRequest();

                        List<FuncionarioEstabelecimento> fe = context.FuncionarioEstabelecimento.Where(fe => fe.IdFuncionario == f.IdFuncionario).ToList();

                        foreach (FuncionarioEstabelecimento fe_aux in fe)
                        {
                            if (context.Servico.Where(s => s.IdEstabelecimento == fe_aux.IdEstabelecimento && s.IdAnimal == idAnimal).ToList().Count != 0)
                            {
                                authorized = true;
                            }
                        }

                    }

                    if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                    if (authorized == true)
                    {
                        try
                        {

                            ClienteAnimal ca = context.ClienteAnimal.Where(ca => ca.IdAnimal == idAnimal).FirstOrDefault();
                            if (ca != null)
                            {
                                return new JsonResult(context.Cliente.Where(c => c.IdCliente == ca.IdCliente).FirstOrDefault());
                            } else
                            {
                                return new JsonResult("Sem cliente");
                            }
                            
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex);
                            return BadRequest(); // MUDAR PARA ERRO SUPOSTO.
                        }
                        
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

        // get clientes by estabelecimento

        struct Pagamento
        {
            public string tipo { get; set; }
            public decimal preco { get; set; }
            public DateTime data { get; set; }
            public int IdEstabelecimento { get; set; }

        }

        /// <summary>
        /// Busca lista dos pagamentos de encomendas e serviços
        /// </summary>
        /// <param name="idCliente">id do cliente</param>
        /// <returns>Lista de pagamentos</returns>
        [HttpGet("{idCliente}/getpaymentlist"), Authorize(Roles = "Admin,Cliente")]
        public IActionResult GetPagamentosEfetuados(int idCliente)
        {
            if (CheckCliente(idCliente) == true)
            {
                using (var context = new FeedyVetContext())
                {
                    List<Pagamento> pagamentos = new List<Pagamento>();
                    Cliente cliente = context.Cliente.Where(c => c.IdCliente == idCliente).FirstOrDefault();
                    List<Servico> servicos = context.Servico.Where(s => s.IdCliente == cliente.IdCliente && (s.Estado == "Concluido" || s.Estado == "Pago")).ToList();
                    List<ServicoCatalogo> servicoCatalogos = context.ServicoCatalogo.ToList();
                    List<Encomenda> encomendas = context.Encomenda.Where(e => e.IdCliente == cliente.IdCliente && (e.Estado == "Concluido" || e.Estado == "Pago")).ToList();
                    List<EncomendaStock> encomendaStocks = context.EncomendaStock.ToList();
                    List<StockEstabelecimento> stocks = context.StockEstabelecimento.ToList();
                    List<Estabelecimento> estabelecimentos = context.Estabelecimento.ToList();

                    Pagamento p = new Pagamento();

                    foreach (Servico s in servicos)
                    {
                        p.data = s.DataServico;
                        p.tipo = "Serviço";
                        p.IdEstabelecimento = s.IdEstabelecimento;

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
                        p.data = e.Data;
                        p.tipo = "Encomenda";
                        p.preco = 0;
                        bool check = false;

                        foreach (EncomendaStock es in encomendaStocks)
                        {
                            if (e.IdEncomenda == es.IdEncomenda)
                            {
                                foreach (StockEstabelecimento s in stocks)
                                {
                                    if(check == false)
                                    {
                                        check = true;
                                        p.IdEstabelecimento = s.IdEstabelecimento;
                                    }
                                    if (es.IdStock == s.IdStock)
                                    {
                                        p.preco += (es.Qtd * s.Preco);
                                    }
                                }
                            }
                        }

                        if (p.preco != (decimal)0.0) pagamentos.Add(p);
                    }

                    return new JsonResult(pagamentos.Join(estabelecimentos, p=>p.IdEstabelecimento, e=>e.IdEstabelecimento,
                        (p,e) => new { 
                            e.Nome,
                            p.preco,
                            p.tipo,
                            p.data
                        }).ToList());
                }
            } else
            {
                return Forbid();
            }

        }
        
        #endregion

        #region Login

        public static bool VerifyAccount(Cliente account)
        {
            using (var context = new FeedyVetContext())
            {
                Cliente cliente = context.Cliente.FirstOrDefault(aux => aux.Email == account.Email && aux.Estado != "Inativo");

                if (cliente == null)
                {
                    throw new ArgumentException("Cliente não existe!", "account");
                }
                else
                {
                    if (!FeedyVetAPI.HashSaltPW.VerifyPasswordHash(account.Pass, Convert.FromBase64String(cliente.Pass), Convert.FromBase64String(cliente.PassSalt)))
                    {
                        throw new ArgumentException("Password Errada.", "account");
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Login do cliente
        /// </summary>
        /// <param name="account"> Cliente </param>
        /// <returns> Mensagem do sucedido </returns>
        [Route("Login")]
        [HttpPost]
        public IActionResult Login(Cliente account)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    Cliente cliente = context.Cliente.FirstOrDefault(aux => aux.Email == account.Email);

                    VerifyAccount(account);

                    string token = CreateToken(cliente);
                    return new JsonResult(token);
                }
                catch (ArgumentException ae)
                {
                    Console.WriteLine(ae);
                    return new JsonResult(ae.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }
            }
        }

        /// <summary>
        /// Cria tokens de autenticação
        /// </summary>
        /// <param name="cliente"> Cliente </param>
        /// <returns> JSON web token </returns>
        private string CreateToken(Cliente cliente)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, cliente.Email),
                new Claim(ClaimTypes.Role, "Cliente")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
        #endregion

        #region Post

        /// <summary>
        ///  Envia os dados do cliente para o servidor API / Registo do cliente
        /// </summary>
        /// <param name="cli"> Cliente </param>
        /// <returns> Mensagem do sucedido </returns>
        [HttpPost]
        public IActionResult Post(Cliente cli)
        {
            using (var context = new FeedyVetContext())
            {

                Cliente cliente = new Cliente();

                cliente.IdCliente = cli.IdCliente;
                cliente.Nome = cli.Nome;
                cliente.Apelido = cli.Apelido;
                cliente.IdMoradaNavigation = context.Morada.Where(m => m.IdMorada == cli.IdMorada).FirstOrDefault();
                cliente.Email = cli.Email;
                cliente.Telemovel = cli.Telemovel;

                FeedyVetAPI.HashSaltPW.CreatePasswordHash(cli.Pass, out byte[] passwordHash, out byte[] passwordSalt);
                cliente.Pass = Convert.ToBase64String(passwordHash);
                cliente.PassSalt = Convert.ToBase64String(passwordSalt);

                cliente.TipoConta = cli.TipoConta is null ? "Livre" : cli.TipoConta;

                if (cliente.TipoConta != "Livre") { cliente.Pontos = 0; }

                if (cliente.TipoConta == "Livre") cliente.ValorConta = 0.0;
                else if (cliente.TipoConta == "Mensal") cliente.ValorConta = 1.99;
                else if (cliente.TipoConta == "Anual") cliente.ValorConta = 17.99;
                else return new JsonResult("Tipo de conta inexistente, selecione entre 'Livre', 'Mensal' ou 'Anual'");

                cliente.ClienteFoto = "https://localhost:5001/images/avatar.png";
                cliente.Estado = "Ativo";
                cliente.Pontos = 0;

                context.Cliente.Add(cliente);

                try
                {
                    context.SaveChanges();
                    return Ok();
                }
                // Handle exceptions! (Diferentes exceptions...)
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }
            }
        }

        /// <summary>
        /// Guarda a foto do cliente
        /// </summary>
        /// <returns> Nome do ficheiro </returns>
        [Route("SaveFile")]
        [HttpPost, Authorize(Roles = "Admin,Cliente")]
        public JsonResult SaveFile()
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

                return new JsonResult(filename);
            }
            catch (Exception)
            {
                return new JsonResult("nonePhoto.png");
            }
        }


        #endregion

        #region Update

        /// <summary>
        /// Recuperar a password de um cliente atraves do seu email inserido
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [Route("recoverpass")]
        [HttpPatch, Authorize(Roles = "Admin,Cliente")]
        public IActionResult RecoverPassword(Cliente cli)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                    {
                        string emailCliente = User.FindFirstValue(ClaimTypes.Email);

                        Cliente cliente = context.Cliente.Where(c => c.Email == cli.Email && c.Estado == "Ativo").FirstOrDefault();

                        if (cliente == null) return BadRequest();

                        if (emailCliente != cliente.Email)
                        {
                            return Forbid();
                        }

                        HashSaltPW.CreatePasswordHash(cli.Pass, out byte[] passwordHash, out byte[] passwordSalt);
                        cliente.Pass = Convert.ToBase64String(passwordHash);
                        cliente.PassSalt = Convert.ToBase64String(passwordSalt);
                    }
                    else
                    {
                        Cliente cliente = context.Cliente.Where(c => c.Email == cli.Email).FirstOrDefault();

                        if (cliente == null) return BadRequest();

                        HashSaltPW.CreatePasswordHash(cli.Pass, out byte[] passwordHash, out byte[] passwordSalt);
                        cliente.Pass = Convert.ToBase64String(passwordHash);
                        cliente.PassSalt = Convert.ToBase64String(passwordSalt);
                    }

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
        /// Altera o estado do cliente
        /// </summary>
        /// <param name="cli"> Cliente </param>
        /// <returns> Mensagem do sucedido </returns>
        [HttpPut, Authorize(Roles = "Admin,Cliente")]
        public IActionResult ChangeStatus(Cliente cli)
        {
            using (var context = new FeedyVetContext())
            {
                if (CheckCliente(cli.IdCliente))
                {
                    try
                    {
                        Cliente cliente = context.Cliente.Where(c => c.IdCliente == cli.IdCliente).FirstOrDefault();

                        cliente.Estado = cli.Estado;

                        context.SaveChanges();
                        return Ok();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return BadRequest();
                    }
                } else
                {
                    return Forbid();
                }
            }
        }

        /// <summary>
        /// Arquivar perfil do cliente (altera o seu estado para inativo)
        /// -> Remoção da relação entre o cliente e todos os seus animais
        /// -> Arquiva animais (altera o seu estado para inativo)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/del")]
        [HttpPatch, Authorize(Roles = "Admin,Cliente")]
        public IActionResult DelCliente(int id)
        {
            if (CheckCliente(id))
            {
                using (var context = new FeedyVetContext())
                {
                    try
                    {
                        Cliente cliente = context.Cliente.Where(c => c.IdCliente == id).FirstOrDefault();
                        List<ClienteAnimal> list = context.ClienteAnimal.ToList();

                        foreach (ClienteAnimal ca in list)
                        {
                            if (ca.IdCliente == id)
                            {
                                Animal animal = context.Animal.Where(a => a.IdAnimal == ca.IdAnimal).FirstOrDefault();

                                animal.Estado = "Inativo";
                                context.ClienteAnimal.Remove(ca);
                            }
                        }

                        cliente.Estado = "Inativo";

                        context.SaveChanges();
                        return Ok();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return BadRequest();
                    }
                }
            } else
            {
                return Forbid();
            }

        }

        /// <summary>
        /// Alterar parcialmente atributos do cliente
        /// </summary>
        /// <param name="cli"> Cliente </param>
        /// <returns> Mensagem do sucedido </returns>
        [HttpPatch, Authorize(Roles = "Admin, Cliente")]
        public IActionResult Patch(Cliente cli)
        {
            using (var context = new FeedyVetContext())
            {
                if (CheckCliente(cli.IdCliente))
                {
                    try
                    {
                        Cliente cliente = context.Cliente.Where(c => c.IdCliente == cli.IdCliente).FirstOrDefault();

                        cliente.Nome = cli.Nome is null ? cliente.Nome : cli.Nome;
                        cliente.Apelido = cli.Apelido is null ? cliente.Apelido : cli.Apelido;
                        if (cli.IdMorada != 0) cliente.IdMoradaNavigation = context.Morada.Where(m => m.IdMorada == cli.IdMorada).FirstOrDefault();
                        cliente.Email = cli.Email is null ? cliente.Email : cli.Email;
                        cliente.Telemovel = cli.Telemovel is 0 ? cliente.Telemovel : cli.Telemovel;
                        cliente.Pass = cli.Pass is null ? cliente.Pass : cli.Pass;
                        cliente.TipoConta = cli.TipoConta is null ? cliente.TipoConta : cli.TipoConta;
                        cliente.ValorConta = cli.ValorConta == cliente.ValorConta ? cliente.ValorConta : cli.ValorConta;
                        cliente.ClienteFoto = cli.ClienteFoto is null ? cliente.ClienteFoto : cli.ClienteFoto;
                        cliente.Estado = cli.Estado is null ? cliente.Estado : cli.Estado;

                        context.SaveChanges();
                        return Ok();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return BadRequest();
                    }
                } else
                {
                    return Forbid();
                }

            }
        }

        /// <summary>
        /// Altera o tipo de plano da conta do cliente
        /// </summary>
        /// <param name="cli"> Cliente </param>
        /// <returns> Mensagem do sucedido </returns>
        [Route("ChangePlan")]
        [HttpPatch, Authorize(Roles = "Admin, Cliente")]
        public IActionResult ChangePlan(Cliente cli) //Basta passar cliente usando assim o ID + Tipo_Conta
        {
            if (CheckCliente(cli.IdCliente))
            {
                using (var context = new FeedyVetContext())
                {

                    try
                    {
                        Cliente cliente = context.Cliente.Where(c => c.IdCliente == cli.IdCliente).FirstOrDefault();
                        cliente.TipoConta = cli.TipoConta is null ? cliente.TipoConta : cli.TipoConta;
                        cliente.ValorConta = cli.ValorConta == cliente.ValorConta ? cliente.ValorConta : cli.ValorConta;
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

        /// <summary>
        /// Cancela o plano atual (muda o tipo de conta para gratuita)
        /// </summary>
        /// <param name="cli"> Cliente </param>
        /// <returns> Mensagem do sucedido </returns>
        [Route("CancelPlan")]
        [HttpPatch, Authorize(Roles = "Admin, Cliente")]
        public IActionResult CancelPlan(Cliente cli) //Basta passar cliente usando assim o ID + Tipo_Conta
        {
            if (CheckCliente(cli.IdCliente))
            {
                using (var context = new FeedyVetContext())
                {
                    try
                    {
                        Cliente cliente = context.Cliente.Where(c => c.IdCliente == cli.IdCliente).FirstOrDefault();

                        cliente.TipoConta = "Gratuita";
                        cliente.ValorConta = 0.0;
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

        /// <summary>
        /// Método que elimina um cliente da base de dados
        /// </summary>
        /// <param name="id"> ID do Cliente </param>
        /// <returns> Estado do método </returns>
        [Route("{id}")]
        [HttpDelete, Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    context.Cliente.Remove(context.Cliente.Where(c => c.IdCliente == id).FirstOrDefault());

                    context.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        #endregion

        #region Comentarios
        /**
        [HttpPut] //Metodo para dar update dos valores na table
        public JsonResult Put(Cliente cliente)
        {
            string query = @"
                    update dbo.Avaliacao_Clinica_Utilizador set
                    nome = '" + cliente.Nome + @"'
                    ,apelido = '" + cliente.Apelido + @"'
                    ,id_morada = '" + cliente.IdMorada + @"'
                    ,email = '" + cliente.Email + @"'
                    ,telemovel = '" + cliente.Telemovel + @"'
                    ,pass = '" + cliente.Pass + @"'
                    ,tipo_conta = '" + cliente.TipoConta + @"'
                    ,valor_conta = '" + cliente.ValorConta + @"'
                    ,cliente_foto = '" + cliente.ClienteFoto + @"'
                    ,estado = '" + cliente.Estado + @"'
                    Where id_cliente = " + cliente.IdCliente + @"
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
        **/

        /*[HttpGet]
        public JsonResult Get()
        {
            string query = @"select * from dbo.Cliente";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("FeedyAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon=new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }*/

        /*[HttpPost] //Metodo para escrever na table
        public JsonResult Post(Cliente cliente)
        {
            String query = @"
                    insert into dbo.Cliente
                    (nome,apelido,id_morada,email,telemovel,pass,tipo_conta,valor_conta,cliente_foto,estado)
                    values
                    ( 
                    '" + cliente.Nome + @"'
                    ,'" + cliente.Apelido + @"'
                    ,'" + cliente.IdMorada + @"'
                    ,'" + cliente.Email + @"'
                    ,'" + cliente.Telemovel + @"'
                    ,'" + cliente.Pass + @"'
                    ,'" + cliente.TipoConta + @"'
                    ,'" + cliente.ValorConta + @"'
                    ,'" + cliente.ClienteFoto + @"'
                    ,'" + cliente.Estado + @"'
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

        /**
        [HttpDelete("{id}")] //Metodo para dar delete dos valores na table //Passar por referencia por ser com o ID
        public JsonResult Delete(int id)
        {
            string query = @"
                Delete from dbo.Cliente
                where id_cliente = " + id + @"
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
        **/

        /*
        [HttpGet]
        public IEnumerable<Cliente> Get()
        {
            using (var context = new FeedyVetContext())
            {
                return context.Cliente.ToList();
            }
        }
        */

        /*
        [Route("Login")]
        [HttpGet]
        public JsonResult Login(Cliente cliente)
        {
            string query = @"select email, pass from dbo.Cliente where email = '"
            + cliente.Email + @"' and pass = '" + cliente.Pass + @"'";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("FeedyAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }
            }

            if (table.Rows.Count != 0)
            {
                return new JsonResult("True");
            } else
            {
                return new JsonResult("False");
            }
        
        }  
        */

        /* QTD TEM DE ESTAR NO ENCOMENDA E PRECO UNITARIO PARA QUE???
        //encomendas
        [Route("{id}/adddelivery")]
        [HttpPost]
        public JsonResult AddDelivery(int id, Encomenda enc)
        {
            using (var context = new FeedyVetContext())
            {
                Encomenda encomenda = new Encomenda();
                EncomendaStock encomendaStock = new EncomendaStock();
                StockClinica stockClinica = new StockClinica();
                Cliente cliente = context.Cliente.Where(c => c.IdCliente == id).FirstOrDefault();
                Morada morada = context.Morada.Where(m => m.IdMorada == cliente.IdMorada).FirstOrDefault();

                try
                {
                    encomenda.IdClienteNavigation = cliente;
                    encomenda.IdMoradaNavigation = morada;
                    encomenda.Estado = enc.Estado;
                    encomenda.MetodoPagamento = enc.MetodoPagamento;

                    encomendaStock.IdEncomendaNavigation = encomenda;
                    encomendaStock.IdStockNavigation = stockClinica;
                    encomendaStock.Qtd = ;
                    encomendaStock.PrecoUnitario = ;

                    context.Encomenda.Add(encomenda);
                    context.EncomendaStock.Add(encomendaStock);

                    context.SaveChanges();
                    return new JsonResult("ID: " + encomenda.IdEncomenda + " foi encomendado com sucesso!");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return new JsonResult("Error");
                }
            }
        }
        */
        #endregion

        public bool CheckCliente(int idCliente)
        {
            if (User.HasClaim(ClaimTypes.Role, "Cliente"))
            {
                using (var context = new FeedyVetContext())
                {
                    string clienteEmail = User.FindFirstValue(ClaimTypes.Email);
                    Cliente c = context.Cliente.Where(c => c.Email == clienteEmail).FirstOrDefault();
                    if (c == null) return false;
                    else return true;
                }
            }
            else if (User.HasClaim(ClaimTypes.Role, "Admin"))
            {
                return true;
            }
            else return false;
        }

    }
}

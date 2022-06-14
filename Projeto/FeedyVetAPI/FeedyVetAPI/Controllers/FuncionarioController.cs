using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using FeedyVetAPI.Models;
using System.IO;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FeedyVetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuncionarioController : ControllerBase
    {
        private readonly IConfiguration _configuration;  //Injeção privada para utilizar certos metodos
        private readonly IWebHostEnvironment _env;
        public FuncionarioController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        #region Get

        /// <summary>
        /// Busca e mostra os dados de todos os funcionarios com as suas passwords escondidas
        /// </summary>
        /// <returns> Dados dos funcionarios </returns>
        [HttpGet, Authorize(Roles = "Admin")]
        public IEnumerable<Funcionario> Get()
        {

            using (var context = new FeedyVetContext())
            {
                List<Funcionario> VetsObtidos = context.Funcionario.ToList();
                foreach (Funcionario v in VetsObtidos)
                {
                    v.Pass = "Hidden";
                    v.PassSalt = "Hidden";
                }
                return VetsObtidos;
            }
        }

        /// <summary>
        /// Busca e mostra os dados de um funcionario com as suas passwords escondidas
        /// </summary>
        /// <param name="idFuncionario"> Id do Funcionario </param>
        /// <returns> Dados do funcionario </returns>
        [HttpGet("{idFuncionario}"), Authorize(Roles = "Admin, Gerente, Funcionario")]
        public IActionResult GetFuncionarioByID(int idFuncionario)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    bool authorized = false;
                    if (User.HasClaim(ClaimTypes.Role, "Gerente") || (User.HasClaim(ClaimTypes.Role, "Funcionario") && !User.HasClaim(ClaimTypes.Role, "Admin")))
                    {
                        string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                        Funcionario f = context.Funcionario.Where(f => f.Email == emailFunc && f.IdFuncionario == idFuncionario).FirstOrDefault();

                        if (f == null) return BadRequest();

                        authorized = true;
                    }

                    if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                    if (authorized == true)
                    {
                        Funcionario func = context.Funcionario.Where(funcionario => funcionario.IdFuncionario == idFuncionario).FirstOrDefault();

                        if (func is null) return BadRequest();

                        func.Pass = "Hidden";
                        func.PassSalt = "Hidden";

                        return new JsonResult(func);
                    }
                    else return Forbid();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        [HttpGet("getfuncionariobytoken"), Authorize(Roles = "Gerente, Funcionario")]
        public IActionResult GetFuncionarioByToken()
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                    Funcionario f = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();
                    if (f == null) return BadRequest();
                    return new JsonResult(f);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }

        }

        /// <summary>
        /// Busca e mostra todas as clinicas de um funcionario
        /// </summary>
        /// <param name="idFuncionario"> Id do Funcionario </param>
        /// <returns> Dados do funcionario </returns>
        [HttpGet("{idFuncionario}/clinicas"), Authorize(Roles = "Admin, Gerente, Funcionario")]
        public IActionResult GetClinicasFuncionarioByID(int idFuncionario)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    bool authorized = false;
                    if (User.HasClaim(ClaimTypes.Role, "Gerente") || (User.HasClaim(ClaimTypes.Role, "Funcionario") && !User.HasClaim(ClaimTypes.Role, "Admin")))
                    {
                        string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                        Funcionario f = context.Funcionario.Where(f => f.Email == emailFunc && f.IdFuncionario == idFuncionario).FirstOrDefault();

                        if (f == null) return BadRequest();

                        authorized = true;
                    }

                    if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                    if (authorized == true)
                    {
                        Funcionario func = context.Funcionario.Where(funcionario => funcionario.IdFuncionario == idFuncionario).FirstOrDefault();
                        if (func is null) return BadRequest();
                        List<FuncionarioEstabelecimento> funcest = context.FuncionarioEstabelecimento.Where(fe => fe.IdFuncionario == func.IdFuncionario).ToList();
                        List<Estabelecimento> est = context.Estabelecimento.ToList();

                        return new JsonResult(funcest.Join(est, fe => fe.IdEstabelecimento, e => e.IdEstabelecimento, (fe, e) => new
                        {
                            e.Nome
                        }).ToList());
                    }
                    else return Forbid();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        /// <summary>
        /// Permite um funcionário ver os serviços que este tem ainda por fazer hoje
        /// </summary>
        /// <param name="idFuncionario"> ID do funcionário </param>
        /// <returns> Dados dos serviços restantes para aquele dia </returns>
        [Route("{idFuncionario}/personaldailyservices")]
        [HttpGet, Authorize(Roles = "Admin,Funcionario")]
        public IActionResult GetServicosRestantesFunc(int idFuncionario)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    if (User.HasClaim(ClaimTypes.Role, "Funcionario"))
                    {
                        string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                        Funcionario f = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();
                        if (f == null) return new JsonResult("Erro");
                        if (idFuncionario != f.IdFuncionario) Forbid();
                    }

                    DateTime hoje = DateTime.Today;
                    TimeSpan inicioDia = new TimeSpan(00, 00, 01);
                    TimeSpan fimDia = new TimeSpan(23, 59, 59);

                    Funcionario checker = context.Funcionario.Where(f => f.IdFuncionario == idFuncionario && f.Estado != "Inativo").FirstOrDefault();

                    if (checker == null) return new JsonResult("Funcionario inexistente");


                    List<Servico> servicos = context.Servico.Where(s => s.IdFuncionario == idFuncionario
                    && DateTime.Compare(s.DataServico, hoje + inicioDia) >= 0
                    && DateTime.Compare(s.DataServico, hoje + fimDia) <= 0
                    && s.Estado != "Concluido").ToList();

                    return new JsonResult(servicos.Select(s => new
                    {
                        IdServico = s.IdServico,
                        IdCliente = s.IdCliente,
                        IdFuncionario = s.IdFuncionario,
                        IdAnimal = s.IdAnimal,
                        IdEstabelecimento = s.IdEstabelecimento,
                        IdServicoCatalogo = s.IdServicoCatalogo,
                        DataServico = s.DataServico,
                        Descricao = s.Descricao,
                        Estado = s.Estado
                    }));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return new JsonResult("Error");
                }
            }
        }

        [Route("{idFuncionario}/servicesdone")]
        [HttpGet, Authorize(Roles = "Admin, Funcionario")]
        public IActionResult GetServicosFeitosFunc(int idFuncionario)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    if (User.HasClaim(ClaimTypes.Role, "Funcionario"))
                    {
                        string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                        Funcionario f = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();
                        if (f == null) return new JsonResult("Erro");
                        if (idFuncionario != f.IdFuncionario) Forbid();
                    }

                    DateTime hoje = DateTime.Today;
                    TimeSpan inicioDia = new TimeSpan(00, 00, 01);
                    TimeSpan fimDia = new TimeSpan(23, 59, 59);

                    Funcionario checker = context.Funcionario.Where(f => f.IdFuncionario == idFuncionario && f.Estado != "Inativo").FirstOrDefault();

                    if (checker == null) return new JsonResult("Funcionario inexistente");

                    List<Servico> servicos = context.Servico.Where(s => s.IdFuncionario == idFuncionario
                    && DateTime.Compare(s.DataServico, hoje + inicioDia) >= 0
                    && DateTime.Compare(s.DataServico, hoje + fimDia) <= 0
                    && s.Estado == "Concluido").ToList();

                    return new JsonResult(servicos.Select(s => new
                    {
                        IdServico = s.IdServico,
                        IdCliente = s.IdCliente,
                        IdAnimal = s.IdAnimal,
                        IdEstabelecimento = s.IdEstabelecimento,
                        IdServicoCatalogo = s.IdServicoCatalogo,
                        DataServico = s.DataServico,
                        Descricao = s.Descricao,
                        Estado = s.Estado
                    }));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return new JsonResult("Error");
                }
            }
        }


        /// <summary>
        /// Permite um gerente ver os serviços que o estabelecimento tem ainda por fazer hoje
        /// 
        /// </summary>
        /// <param name="idFuncionario"> ID do funcionario </param>
        /// <param name="idEstabelecimento"> ID do estabelecimento </param>
        /// <returns> Dados dos serviços restantes para aquele dia </returns>
        [Route("{idFuncionario}/{idEstabelecimento}/dailyservices")]                // ALTERAR STAUTS
        [HttpGet, Authorize(Roles = "Admin, Gerente")]
        public IActionResult GetServicosRestantesGer(int idFuncionario, int idEstabelecimento)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                    {
                        if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{idEstabelecimento}")) Forbid();
                    }

                    DateTime hoje = DateTime.Today;

                    Funcionario checkerFuncionario = context.Funcionario.Where(f => f.IdFuncionario == idFuncionario
                    && f.Estado != "Inativo").FirstOrDefault();
                    Estabelecimento checkerEstabelecimento = context.Estabelecimento.Where(f => f.IdEstabelecimento == idEstabelecimento
                    && f.Estado != "Inativo").FirstOrDefault();
                    EstabelecimentoGerente checkerGerente = context.EstabelecimentoGerente
                        .Where(eg => eg.IdEstabelecimento == idEstabelecimento && eg.IdFuncionario == idFuncionario).FirstOrDefault();

                    if (checkerFuncionario == null) return new JsonResult("Funcionario inexistente"); //mudar status
                    if (checkerEstabelecimento == null) return new JsonResult("Estabelecimento inexistente"); //mudar status
                    if (checkerGerente == null) return Forbid();

                    List<Servico> servicos = context.Servico.Where(s => s.IdEstabelecimento == idEstabelecimento
                    && DateTime.Compare(s.DataServico.Date, hoje.Date) == 0
                    && s.Estado != "Concluido").ToList();


                    List<Animal> animais = context.Animal.ToList();
                    List<Funcionario> funcionarios = context.Funcionario.ToList();
                    List<Cliente> clientes = context.Cliente.ToList();

                    return new JsonResult(servicos.Join(funcionarios, s => s.IdFuncionario, f => f.IdFuncionario,
                        (s, f) => new
                        {
                            f.Nome,
                            f.Apelido,
                            f.FuncionarioFoto,
                            s.DataServico,
                            s.IdServico,
                            s.Estado,
                            s.Descricao,
                            s.IdCliente,
                            s.IdAnimal,
                            f.IdFuncionario
                        }).Join(clientes, se => se.IdCliente, c => c.IdCliente,
                        (se, c) => new 
                        {
                            se.Nome,
                            se.Apelido,
                            se.FuncionarioFoto,
                            se.DataServico,
                            se.IdServico,
                            se.Estado,
                            se.Descricao,
                            se.IdAnimal,
                            se.IdCliente,
                            se.IdFuncionario,
                            NomeCliente = c.Nome,
                            c.Email,
                            c.Telemovel
                        }).Join(animais, se2 => se2.IdAnimal, a => a.IdAnimal,
                        (se2, a) => new
                        {
                            se2.Nome,
                            se2.Apelido,
                            se2.FuncionarioFoto,
                            se2.DataServico,
                            se2.IdServico,
                            se2.Estado,
                            se2.Descricao,
                            se2.IdAnimal,
                            se2.IdCliente,
                            se2.IdFuncionario,
                            se2.NomeCliente,
                            se2.Email,
                            se2.Telemovel,
                            NomeAnimal = a.Nome
                        }));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }
            }
        }

        /// <summary>
        /// Metodo que retorna os servicos efetuados diariamente
        /// </summary>
        /// <param name="idFuncionario"></param>
        /// <param name="idEstabelecimento"></param>
        /// <returns></returns>
        [Route("{idFuncionario}/{idEstabelecimento}/servicesdone")]                // ALTERAR STAUTS
        [HttpGet, Authorize(Roles = "Admin, Gerente")]
        public IActionResult GetServicosEfetuadosGer(int idFuncionario, int idEstabelecimento)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                    {
                        if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{idEstabelecimento}")) Forbid();
                    }

                    DateTime hoje = DateTime.Today;

                    Funcionario checkerFuncionario = context.Funcionario.Where(f => f.IdFuncionario == idFuncionario
                    && f.Estado != "Inativo").FirstOrDefault();
                    Estabelecimento checkerEstabelecimento = context.Estabelecimento.Where(f => f.IdEstabelecimento == idEstabelecimento
                    && f.Estado != "Inativo").FirstOrDefault();
                    EstabelecimentoGerente checkerGerente = context.EstabelecimentoGerente
                        .Where(eg => eg.IdEstabelecimento == idEstabelecimento && eg.IdFuncionario == idFuncionario).FirstOrDefault();

                    if (checkerFuncionario == null) return new JsonResult("Funcionario inexistente"); //mudar status
                    if (checkerEstabelecimento == null) return new JsonResult("Estabelecimento inexistente"); //mudar status
                    if (checkerGerente == null) return Forbid();

                    List<Servico> servicos = context.Servico.Where(s => s.IdEstabelecimento == idEstabelecimento
                    && DateTime.Compare(s.DataServico.Date, hoje.Date) == 0
                    && s.Estado == "Concluido").ToList();


                    List<Animal> animais = context.Animal.ToList();
                    List<Funcionario> funcionarios = context.Funcionario.ToList();

                    return new JsonResult(servicos.Join(funcionarios, s => s.IdFuncionario, f => f.IdFuncionario,
                        (s, f) => new
                        {
                            f.Nome,
                            f.Apelido,
                            f.FuncionarioFoto,
                            s.DataServico,
                            s.IdServico,
                            f.IdFuncionario
                        }));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }
            }
        }

        /// <summary>
        /// Busca e mostra os dados de todos os funcionários de um estabelecimento
        /// </summary>
        /// <param name="idFuncionario"> ID do Funcionário que pretende obter esta informação</param>
        /// <param name="idEstabelecimento"> ID do Estabelecimento </param>
        /// <returns> Dados dos funcionários do estabelecimento </returns>
        [Route("{idFuncionario}/{idEstabelecimento}")]
        [HttpGet, Authorize]
        public IActionResult GetFuncionariosEstabelecimento(int idFuncionario, int idEstabelecimento)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                    {
                        if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{idEstabelecimento}")) Forbid();
                    }

                    EstabelecimentoGerente eg = context.EstabelecimentoGerente.Where(e => e.IdFuncionario == idFuncionario
                    && e.IdEstabelecimento == idEstabelecimento).FirstOrDefault();

                    if (eg == null) return Forbid();

                    List<FuncionarioEstabelecimento> fe = context.FuncionarioEstabelecimento.Where(f => f.IdEstabelecimento == idEstabelecimento).ToList();
                    List<Funcionario> funcionarios_list = new List<Funcionario>();

                    foreach (FuncionarioEstabelecimento f in fe)
                    {
                        Funcionario func = context.Funcionario.Where(funcionario => funcionario.IdFuncionario == f.IdFuncionario && funcionario.Estado != "Inativo").FirstOrDefault();

                        if (func != null)
                        {
                            funcionarios_list.Add(func);
                        }
                    }

                    return new JsonResult(funcionarios_list);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }
            }
        }

        /// <summary>
        /// Busca e mostra os dados de todos os funcionários de um estabelecimento
        /// </summary>
        /// <param name="idFuncionario"> ID do Funcionário que pretende obter esta informação</param>
        /// <param name="idEstabelecimento"> ID do Estabelecimento </param>
        /// <returns> Dados dos funcionários do estabelecimento </returns>
        [Route("showfuncionarios/{idEstabelecimento}")]
        [HttpGet, Authorize]
        public IActionResult GetFuncionariosEstabelecimentoParaCliente(int idEstabelecimento)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    List<FuncionarioEstabelecimento> fe = context.FuncionarioEstabelecimento.Where(f => f.IdEstabelecimento == idEstabelecimento).ToList();
                    List<Funcionario> funcionarios_list = new List<Funcionario>();

                    foreach (FuncionarioEstabelecimento f in fe)
                    {
                        Funcionario func = context.Funcionario.Where(funcionario => funcionario.IdFuncionario == f.IdFuncionario && funcionario.Estado != "Inativo").FirstOrDefault();
                        Console.WriteLine(func.Nome);
                        funcionarios_list.Add(func);
                    }

                    return new JsonResult(funcionarios_list);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }
            }
        }


        #endregion

        #region Login

        struct LoginFuncionario
        {
            public string token { get; set; }
            public string claimReturn { get; set; }
        }

        /// <summary>
        /// Login do funcionario
        /// </summary>
        /// <param name="account"> Funcionario </param>
        /// <returns> Mensagem do sucedido </returns>
        [Route("Login")]
        [HttpPost]
        public IActionResult Login(Funcionario account)  // VER OS RETURNS
        {

            using (var context = new FeedyVetContext())
            {
                Funcionario vet = context.Funcionario.FirstOrDefault(aux => aux.Codigo == account.Codigo && aux.Estado != "Inativo");

                if (vet == null)
                {
                    return new JsonResult("Funcionário não existe!");
                }
                else
                {
                    if (!FeedyVetAPI.HashSaltPW.VerifyPasswordHash(account.Pass, Convert.FromBase64String(vet.Pass), Convert.FromBase64String(vet.PassSalt)))
                    {
                        return new JsonResult("Password Errada.");
                    }
                    else
                    {
                        List<string> claims = new List<string>();
                        string token;
                        LoginFuncionario login = new LoginFuncionario();
                        EstabelecimentoGerente eg = context.EstabelecimentoGerente.FirstOrDefault(aux => aux.IdFuncionario == vet.IdFuncionario);
                        if (eg == null)
                        {

                            claims.Add("Funcionario");
                            login.claimReturn = "Funcionario";
                            if (vet.Estado == "Admin") claims.Add("Admin"); // admin temporario! depois tirar isto quando for adicionada a tabela dos admins
                            List<FuncionarioEstabelecimento> fg_list = context.FuncionarioEstabelecimento.Where(aux => aux.IdFuncionario == vet.IdFuncionario).ToList();
                            foreach (FuncionarioEstabelecimento fg_aux in fg_list)
                            {
                                claims.Add($"Funcionario_{fg_aux.IdEstabelecimento}");
                            }

                        } else
                        {
                            claims.Add("Gerente");
                            login.claimReturn = "Gerente";
                            List<EstabelecimentoGerente> eg_list = context.EstabelecimentoGerente.Where(aux => aux.IdFuncionario == vet.IdFuncionario).ToList();
                            foreach (EstabelecimentoGerente eg_aux in eg_list)
                            {
                                claims.Add($"Gerente_{eg_aux.IdEstabelecimento}");
                            }

                        }
                        token = CreateToken(vet, claims);

                        login.token = token;

                        return new JsonResult(login);
                    }
                }
            }
        }

        /// <summary>
        /// Cria tokens de autenticação
        /// </summary>
        /// <param name="func"> Funcionario </param>
        /// <returns></returns>
        private string CreateToken(Funcionario func, List<string> claims_received)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, func.Email)
            };

            foreach (string claim in claims_received)
            {
                claims.Add(new Claim(ClaimTypes.Role, claim));
            }

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

        public static bool Gerente_Estabelecimento_Confirm(int idEstabelecimento, List<Claim> roles)
        {
            foreach (Claim r in roles)
            {
                if (r.Value == "Admin" || r.Value == $"Gerente_{idEstabelecimento}") return true;
            }
            return false;
        }

        public static bool Funcionario_Estabelecimento_Confirm(int idEstabelecimento, List<Claim> roles)
        {
            foreach (Claim r in roles)
            {
                if (r.Value == "Admin" || r.Value == $"Funcionario_{idEstabelecimento}") return true;
            }
            return false;
        }

        #region Post

        /// <summary>
        /// Envia os dados do funcionario para o servidor API / Registo do funcionario
        /// </summary>
        /// <param name="func"> Funcionario </param>
        /// <returns> Mensagem do sucedido </returns>
        [HttpPost]
        public IActionResult Post(Funcionario func)
        {
            using (var context = new FeedyVetContext())
            {
                Funcionario funcionario = new Funcionario();
                funcionario.IdFuncionario = func.IdFuncionario;
                funcionario.Nome = func.Nome;
                funcionario.Apelido = func.Apelido;
                funcionario.Estado = func.Estado;
                funcionario.Especialidade = func.Especialidade;
                funcionario.Email = func.Email;
                funcionario.Telemovel = func.Telemovel;

                FeedyVetAPI.HashSaltPW.CreatePasswordHash(func.Pass, out byte[] passwordHash, out byte[] passwordSalt);
                funcionario.Pass = Convert.ToBase64String(passwordHash);
                funcionario.PassSalt = Convert.ToBase64String(passwordSalt);

                funcionario.Codigo = func.Codigo;
                funcionario.FuncionarioFoto = "https://localhost:5001/images/avatar.jpg";

                context.Funcionario.Add(funcionario);

                try
                {
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
        /// Cria um funcionario e adiciona-o a um estabelecimento
        /// </summary>
        /// <param name="idEstabelecimento"> Id do estabelecimento </param>
        /// <param name="func"> Funcionario </param>
        /// <returns> Mensagem do sucedido </returns>
        [Route("{idEstabelecimento}/addFunc")]
        [HttpPost, Authorize(Roles = "Admin, Gerente")]
        public IActionResult AddFuncionario(int idEstabelecimento, Funcionario func)
        {
            int check = 0;

            using (var context = new FeedyVetContext())
            {
                try
                {
                    if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                    {
                        if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{idEstabelecimento}")) Forbid();
                    }

                    Estabelecimento estabelecimento = context.Estabelecimento.Where(c => c.IdEstabelecimento == idEstabelecimento).FirstOrDefault();
                    List<FuncionarioEstabelecimento> ListVets = context.FuncionarioEstabelecimento.ToList();
                    FuncionarioEstabelecimento vet_cli_novo = new FuncionarioEstabelecimento();
                    Funcionario funcionario = new Funcionario();


                    funcionario.Nome = func.Nome;
                    funcionario.Apelido = func.Apelido;
                    funcionario.Estado = "Ativo";
                    funcionario.Especialidade = func.Especialidade;
                    funcionario.Email = func.Email;
                    funcionario.Telemovel = func.Telemovel;

                    FeedyVetAPI.HashSaltPW.CreatePasswordHash(func.Pass, out byte[] passwordHash, out byte[] passwordSalt);
                    funcionario.Pass = Convert.ToBase64String(passwordHash);
                    funcionario.PassSalt = Convert.ToBase64String(passwordSalt);

                    funcionario.Codigo = func.Codigo;
                    funcionario.FuncionarioFoto = func.FuncionarioFoto;

                    vet_cli_novo.IdEstabelecimentoNavigation = estabelecimento;
                    vet_cli_novo.IdFuncionarioNavigation = funcionario;

                    context.FuncionarioEstabelecimento.Add(vet_cli_novo);
                    context.Funcionario.Add(funcionario);

                    foreach (FuncionarioEstabelecimento vc in ListVets)
                    {
                        if (estabelecimento.IdEstabelecimento == vc.IdEstabelecimento)
                        {
                            check = 1;
                            continue;
                        }
                    }

                    if (check == 0)
                    {
                        EstabelecimentoGerente estabelecimentoGerente = new EstabelecimentoGerente();

                        estabelecimentoGerente.IdEstabelecimentoNavigation = estabelecimento;
                        estabelecimentoGerente.IdFuncionarioNavigation = funcionario;

                        context.EstabelecimentoGerente.Add(estabelecimentoGerente);

                        context.SaveChanges();
                        return Ok();  // funcionario vira gerente
                    }

                    context.SaveChanges();

                    FuncionarioHorario segunda = new FuncionarioHorario();
                    FuncionarioHorario terca = new FuncionarioHorario();
                    FuncionarioHorario quarta = new FuncionarioHorario();
                    FuncionarioHorario quinta = new FuncionarioHorario();
                    FuncionarioHorario sexta = new FuncionarioHorario();
                    FuncionarioHorario sabado = new FuncionarioHorario();
                    FuncionarioHorario domingo = new FuncionarioHorario();

                    segunda.IdFuncionario = funcionario.IdFuncionario;
                    segunda.HoraEntrada = new TimeSpan(9, 0, 0);
                    segunda.HoraSaida = new TimeSpan(21, 0, 0);
                    segunda.DiaSemana = "Mon";

                    terca.IdFuncionario = funcionario.IdFuncionario;
                    terca.HoraEntrada = new TimeSpan(9, 0, 0);
                    terca.HoraSaida = new TimeSpan(21, 0, 0);
                    terca.DiaSemana = "Tue";

                    quarta.IdFuncionario = funcionario.IdFuncionario;
                    quarta.HoraEntrada = new TimeSpan(9, 0, 0);
                    quarta.HoraSaida = new TimeSpan(21, 0, 0);
                    quarta.DiaSemana = "Wed";

                    quinta.IdFuncionario = funcionario.IdFuncionario;
                    quinta.HoraEntrada = new TimeSpan(9, 0, 0);
                    quinta.HoraSaida = new TimeSpan(21, 0, 0);
                    quinta.DiaSemana = "Thu";

                    sexta.IdFuncionario = funcionario.IdFuncionario;
                    sexta.HoraEntrada = new TimeSpan(9, 0, 0);
                    sexta.HoraSaida = new TimeSpan(21, 0, 0);
                    sexta.DiaSemana = "Fri";

                    sabado.IdFuncionario = funcionario.IdFuncionario;
                    sabado.HoraEntrada = new TimeSpan(9, 0, 0);
                    sabado.HoraSaida = new TimeSpan(21, 0, 0);
                    sabado.DiaSemana = "Sat";

                    domingo.IdFuncionario = funcionario.IdFuncionario;
                    domingo.HoraEntrada = new TimeSpan(9, 0, 0);
                    domingo.HoraSaida = new TimeSpan(21, 0, 0);
                    domingo.DiaSemana = "Sun";

                    context.FuncionarioHorario.Add(segunda);
                    context.FuncionarioHorario.Add(terca);
                    context.FuncionarioHorario.Add(quarta);
                    context.FuncionarioHorario.Add(quinta);
                    context.FuncionarioHorario.Add(sexta);
                    context.FuncionarioHorario.Add(sabado);
                    context.FuncionarioHorario.Add(domingo);

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

        public struct PedidoFalta
        {
            public string motivo { get; set; }
            public DateTime dataInicial { get; set; } 
            public DateTime dataFinal { get; set; } 
        };

        [HttpPost("pedidofaltar/{IdFuncionario}"), Authorize(Roles = "Admin, Gerente, Funcionario")]
        public IActionResult PedidoFaltar(int IdFuncionario, [FromBody] PedidoFalta pedido)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    if (User.HasClaim(ClaimTypes.Role, "Funcionario") && !User.HasClaim(ClaimTypes.Role, "Admin"))
                    {
                        NotificacaoFuncionario nf = new NotificacaoFuncionario();
                        Funcionario funcionario = context.Funcionario.Where(func => func.IdFuncionario == IdFuncionario).FirstOrDefault();
                        
                        string emailFunc = User.FindFirstValue(ClaimTypes.Email);

                        if (funcionario is null) return BadRequest();
                        if (funcionario.Email != emailFunc) return Forbid();

                        List<Servico> servicos = context.Servico.Where(ser => DateTime.Compare(ser.DataServico, pedido.dataInicial) >= 0 
                        && DateTime.Compare(ser.DataServico, pedido.dataFinal) <= 0 && ser.IdFuncionario == IdFuncionario).ToList();

                        List<Funcionario> gerentes = new List<Funcionario>();
                        EstabelecimentoGerente gerenteAux = new EstabelecimentoGerente();
                        Funcionario gerenteAux2 = new Funcionario();
                        Estabelecimento auxEstabelecimento = new Estabelecimento();

                        foreach (Servico servico in servicos)
                        {
                            auxEstabelecimento = context.Estabelecimento.Where(est => servico.IdEstabelecimento == est.IdEstabelecimento).FirstOrDefault();

                            if(auxEstabelecimento != null)
                            {
                                gerenteAux = context.EstabelecimentoGerente.Where(eg => eg.IdEstabelecimento == servico.IdEstabelecimento).FirstOrDefault();

                                if (gerenteAux != null)
                                {
                                    gerenteAux2 = context.Funcionario.Where(g => g.IdFuncionario == gerenteAux.IdFuncionario).FirstOrDefault();

                                    if (!gerentes.Contains(gerenteAux2))
                                    {
                                        gerentes.Add(gerenteAux2);
                                    }
                                }
                            }                            
                        }
                        foreach (Funcionario gerente in gerentes)
                        {
                            context.NotificacaoFuncionario.Add(new NotificacaoFuncionario
                            {
                                IdFuncionario = gerente.IdFuncionario,
                                Estado = "Pedido",
                                Descricao = "Pedido faltar: " + funcionario.Nome + " " + funcionario.Apelido + $" ({funcionario.IdFuncionario})" +
                                "\nMotivo: " + pedido.motivo + "\nDatas: " + pedido.dataInicial + " - " + pedido.dataFinal,
                                DataNotificacao = DateTime.Now.Date
                            });
                        }

                    }

                    if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                    {
                        Funcionario f = context.Funcionario.Where(func => func.IdFuncionario == IdFuncionario).FirstOrDefault();

                        string emailGer = User.FindFirstValue(ClaimTypes.Email);

                        if (f is null) return BadRequest();
                        if (f.Email != emailGer) return Forbid();

                        for (DateTime date = pedido.dataInicial; date <= pedido.dataFinal; date = date.AddDays(1))
                        {
                            if (!ServicoController.DesmarcarTodasAsConsultasDia(date, f.IdFuncionario))
                                return BadRequest();
                        }
                    }
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
        /// Guarda a foto do funcionario
        /// </summary>
        /// <returns> Nome do ficheiro </returns>
        [Route("SaveFile")]
        [HttpPost, Authorize(Roles = "Admin, Gerente, Funcionario")] //POR AUTORIZAÇOES
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filemane = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/images/" + filemane;

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult(filemane);
            }
            catch (Exception)
            {
                return new JsonResult("avatar.png");
            }
        }
        #endregion

        #region Update

        

        [Route("aceitarpedidofalta")]
        [HttpPatch, Authorize(Roles = "Admin, Gerente")]
        public IActionResult AceitarPedido(NotificacaoFuncionario notificacao)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    int idFromAux = notificacao.Descricao.IndexOf("(") + "(".Length;
                    int idToAux = notificacao.Descricao.IndexOf(")");
                    int idFinal = Int32.Parse(notificacao.Descricao.Substring(idFromAux, idToAux - idFromAux));
                    Console.WriteLine(idFinal);


                    int dataInicialFromAux = notificacao.Descricao.IndexOf("Datas: ") + "Datas: ".Length;
                    int dataInicialToAux = notificacao.Descricao.LastIndexOf("-") - 1;
                    DateTime dataInicial = DateTime.Parse(notificacao.Descricao.Substring(dataInicialFromAux, dataInicialToAux - dataInicialFromAux));


                    int dataFinalFromAux = dataInicialToAux + 3;
                    int dataFinalToAux = notificacao.Descricao.Length;
                    DateTime dataFinal = DateTime.Parse(notificacao.Descricao.Substring(dataFinalFromAux, dataFinalToAux - dataFinalFromAux));
                    Console.WriteLine(dataInicial);
                    Console.WriteLine(dataFinal);

                    Funcionario gerente = new Funcionario();

                    if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                    {
                        string emailFunc = User.FindFirstValue(ClaimTypes.Email).ToString();
                        gerente = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();
                        if (gerente == null) return Forbid();
                    }


                    List<EstabelecimentoGerente> eg_list = context.EstabelecimentoGerente.Where(eg => eg.IdFuncionario == gerente.IdFuncionario).ToList();

                    List<Servico> servicosParaCancelar = new List<Servico>();

                    foreach (EstabelecimentoGerente eg in eg_list)
                    {
                        servicosParaCancelar.AddRange(context.Servico.Where(s_aux => s_aux.IdFuncionario == idFinal && DateTime.Compare(s_aux.DataServico.Date, dataInicial.Date) >= 0
                        && DateTime.Compare(s_aux.DataServico.Date, dataFinal.Date) <= 0 && s_aux.IdEstabelecimento == eg.IdEstabelecimento).ToList());
                    }

                    foreach (Servico s in servicosParaCancelar)
                    {
                        s.Estado = "Cancelado";

                        context.Notificacao.Add(new Notificacao
                        {
                            IdCliente = s.IdCliente,
                            Estado = "Por Visualizar",
                            Descricao = $"Serviço com ID {s.IdServico} cancelado.",
                            DataNotificacao = DateTime.Now
                        });

                    }

                    context.NotificacaoFuncionario.Add(new NotificacaoFuncionario
                    {
                        IdFuncionario = idFinal,
                        Estado = "Por Visualizar",
                        Descricao = $"Pedido para faltar de dia {dataInicial} a {dataFinal} foi aceite pelo Gerente {gerente.Nome} {gerente.Apelido}.",
                        DataNotificacao = DateTime.Now
                    });

                    NotificacaoFuncionario not = context.NotificacaoFuncionario.Where(nt => nt.IdNotificacao == notificacao.IdNotificacao).FirstOrDefault();
                    not.Estado = "Inativo";

                    context.SaveChanges();
                }
               
               
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }




        [Route("rejeitarpedidofalta")]
        [HttpPatch, Authorize(Roles = "Admin, Gerente")]
        public IActionResult RejeitarPedido(NotificacaoFuncionario notificacao)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    int idFromAux = notificacao.Descricao.IndexOf("(") + "(".Length;
                    int idToAux = notificacao.Descricao.IndexOf(")");
                    int idFinal = Int32.Parse(notificacao.Descricao.Substring(idFromAux, idToAux - idFromAux));


                    int dataInicialFromAux = notificacao.Descricao.IndexOf("Datas: ") + "Datas: ".Length;
                    int dataInicialToAux = notificacao.Descricao.LastIndexOf("-") - 1;
                    DateTime dataInicial = DateTime.Parse(notificacao.Descricao.Substring(dataInicialFromAux, dataInicialToAux - dataInicialFromAux));


                    int dataFinalFromAux = dataInicialToAux + 3;
                    int dataFinalToAux = notificacao.Descricao.Length;
                    DateTime dataFinal = DateTime.Parse(notificacao.Descricao.Substring(dataFinalFromAux, dataFinalToAux - dataFinalFromAux));

                    Funcionario gerente = new Funcionario();

                    if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                    {
                        string emailFunc = User.FindFirstValue(ClaimTypes.Email).ToString();
                        gerente = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();
                        if (gerente == null) return Forbid();
                    }

                    context.NotificacaoFuncionario.Add(new NotificacaoFuncionario
                    {
                        IdFuncionario = idFinal,
                        Estado = "Por Visualizar",
                        Descricao = $"Pedido para faltar de dia {dataInicial} a {dataFinal} foi recusado pelo Gerente {gerente.Nome} {gerente.Apelido}.",
                        DataNotificacao = DateTime.Now
                    });

                    NotificacaoFuncionario not = context.NotificacaoFuncionario.Where(nt => nt.IdNotificacao == notificacao.IdNotificacao).FirstOrDefault();
                    not.Estado = "Inativo";

                    context.SaveChanges();
                }


                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }


        /// <summary>
        /// Altera o estado do funcionario
        /// </summary>
        /// <param name="func"> Funcionario </param>
        /// <returns> Mensagem do sucedido </returns>
        [Route("changestatus")]
        [HttpPatch, Authorize(Roles = "Admin, Gerente")]
        public IActionResult ChangeStatus(Funcionario func)
        {
            using (var context = new FeedyVetContext())
            {
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

                Funcionario funcionario = context.Funcionario.Where(v => v.IdFuncionario == func.IdFuncionario).FirstOrDefault();

                try
                {
                    funcionario.Estado = func.Estado is null ? funcionario.Estado : func.Estado;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }


                context.SaveChanges();
                return Ok();

            }
        }

        /// <summary>
        /// Alterar parcialmente atributos do cliente
        /// </summary>
        /// <param name="func"> Funcionario </param>
        /// <returns></returns>
        [HttpPatch, Authorize(Roles = "Admin, Gerente")]
        public IActionResult Patch(Funcionario func)
        {
            using (var context = new FeedyVetContext())
            {
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

                Funcionario funcionario = context.Funcionario.Where(c => c.IdFuncionario == func.IdFuncionario).FirstOrDefault();

                try
                {
                    funcionario.Nome = func.Nome is null ? funcionario.Nome : func.Nome;
                    funcionario.Apelido = func.Apelido is null ? funcionario.Apelido : func.Apelido;
                    funcionario.Email = func.Email is null ? funcionario.Email : func.Email;

                    funcionario.Estado = func.Estado is null ? funcionario.Estado : func.Estado;
                    funcionario.Especialidade = func.Especialidade is null ? funcionario.Especialidade : func.Especialidade;
                    funcionario.Telemovel = func.Telemovel is null ? funcionario.Telemovel : func.Telemovel;
                    funcionario.Codigo = func.Codigo is 0 ? funcionario.Codigo : func.Codigo;
                    funcionario.FuncionarioFoto = func.FuncionarioFoto is null ? funcionario.FuncionarioFoto : func.FuncionarioFoto;
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
        /// Funcionario edita a sua conta
        /// </summary>
        /// <param name="func"> Funcionario </param>
        /// <returns> Mensagem do sucedido </returns>
        [Route("editaccount")]
        [HttpPatch, Authorize(Roles = "Admin, Funcionario, Gerente")]
        public IActionResult EditAccount(Funcionario func)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    bool authorized = false;
                    string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                    if ((User.HasClaim(ClaimTypes.Role, "Funcionario") && !User.HasClaim(ClaimTypes.Role, "Admin")) || User.HasClaim(ClaimTypes.Role, "Gerente"))
                    {
                        Funcionario f = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();

                        if (f == null) return BadRequest();

                        authorized = true;
                    }

                    Funcionario funcionario = context.Funcionario.Where(c => c.Email == emailFunc).FirstOrDefault();
                    funcionario.Nome = func.Nome is null ? funcionario.Nome : func.Nome;
                    funcionario.Apelido = func.Apelido is null ? funcionario.Apelido : func.Apelido;
                    funcionario.Email = func.Email is null ? funcionario.Email : func.Email;
                    funcionario.Estado = func.Estado is null ? funcionario.Estado : func.Estado;
                    funcionario.Especialidade = func.Especialidade is null ? funcionario.Especialidade : func.Especialidade;
                    funcionario.Telemovel = func.Telemovel is null ? funcionario.Telemovel : func.Telemovel;
                    funcionario.Codigo = func.Codigo is 0 ? funcionario.Codigo : func.Codigo;
                    funcionario.FuncionarioFoto = func.FuncionarioFoto is null ? funcionario.FuncionarioFoto : func.FuncionarioFoto;

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

        /// <summary>
        /// Alterar password do funcionario
        /// </summary>
        /// <param name="func"> Funcionario </param>
        /// <returns> Mensagem do sucedido </returns>
        [Route("changepassword")]
        [HttpPatch, Authorize(Roles = "Admin, Gerente, Funcionario")]
        public IActionResult ChangePass(Funcionario func)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    if ((User.HasClaim(ClaimTypes.Role, "Funcionario") && !User.HasClaim(ClaimTypes.Role, "Admin")) || User.HasClaim(ClaimTypes.Role, "Gerente"))
                    {
                        string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                        Funcionario f = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();

                        if (f == null) return BadRequest();

                        HashSaltPW.CreatePasswordHash(func.Pass, out byte[] passwordHash, out byte[] passwordSalt);
                        f.Pass = Convert.ToBase64String(passwordHash);
                        f.PassSalt = Convert.ToBase64String(passwordSalt);
                    }
                    else if (User.HasClaim(ClaimTypes.Role, "Admin"))
                    {
                        Funcionario f = context.Funcionario.Where(f => f.IdFuncionario == func.IdFuncionario).FirstOrDefault();

                        if (f == null) return BadRequest();

                        HashSaltPW.CreatePasswordHash(func.Pass, out byte[] passwordHash, out byte[] passwordSalt);
                        f.Pass = Convert.ToBase64String(passwordHash);
                        f.PassSalt = Convert.ToBase64String(passwordSalt);
                    }
                    else return Forbid();

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
        /// Metodo que apaga um funcionario da base de dados
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

                    context.Funcionario.Remove(context.Funcionario.Where(f => f.IdFuncionario == id).FirstOrDefault());

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
        /// Metodo que permite arquivar um funcionario de um estabelecimento
        /// </summary>
        /// <returns></returns>
        public JsonResult RemoveFuncionario()       // TO DO
        {
            return new JsonResult("TO DO");
        }

        #endregion

        #region Comentarios
        /*
        [HttpPut]
        public JsonResult Put(Funcionario vet)
        {
            string query = @"
                    update dbo.Funcionario set
                    nome = '" + vet.Nome + @"'
                    ,apelido = '" + vet.Apelido + @"'
                    ,estado = '" + vet.Estado + @"'
                    ,especialidade = '" + vet.Especialidade + @"'
                    ,email = '" + vet.Email + @"'
                    ,telemovel = '" + vet.Telemovel + @"'
                    ,pass = '" + vet.Pass + @"'
                    ,codigo = '" + vet.Codigo + @"'
                    ,funcionario_foto = '" + vet.FuncionarioFoto + @"'
                    Where id_funcionario = " + vet.IdFuncionario + @"
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
        */

        /*[HttpGet]
        public JsonResult Get()
        {
            string query = @"
                select *
                from dbo.Funcionario
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
        }*/

        // GET api/<FuncionarioController>/5
        /*[HttpGet("{id}")]
        public JsonResult Get(int id)
        {
            string query = @"
                select *
                from dbo.Funcionario where id_funcionario = " + id + @"
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
        }*/
        #endregion
    }
}

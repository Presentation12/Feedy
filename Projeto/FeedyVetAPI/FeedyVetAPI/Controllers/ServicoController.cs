using FeedyVetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FeedyVetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicoController : ControllerBase
    {
        private readonly IConfiguration _configuration;  //Injeção privada para utilizar certos metodos
        public ServicoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Get
        /// <summary>
        /// Busca e mostra os dados de todos os serviços da BD
        /// <returns> Dados dos se
        /// </summary>rviços </returns>
        [HttpGet, Authorize(Roles = "Admin")]
        public IEnumerable<Servico> Get()
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    return context.Servico.ToList();
                }
                catch (Exception e)
                {   
                    Console.WriteLine(e);
                    return null;
                }
            }
        }

        /// <summary>
        /// Método que retorna o próximo serviço de um animal associado ao cliente
        /// </summary>
        /// <param name="id_cliente"> ID do Cliente </param>
        /// <returns> O serviço em causa </returns>
        [Route("last/{id_cliente}")]
        [HttpGet]
        public IActionResult GetLastServico(int id_cliente)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    List<Servico> ser = context.Servico.Where(s => s.IdCliente == id_cliente).ToList();
                    List<Servico> sortedList = ser.Where(s => s.Estado == "Marcado" && DateTime.Compare(DateTime.Now, s.DataServico) < 0).OrderBy(s => s.DataServico).ToList();
                    List<Animal> animais = context.Animal.ToList();

                    return new JsonResult(sortedList.Join(animais, s => s.IdAnimal,a => a.IdAnimal,
                    (s,a) => new
                    {
                        s.IdServico,
                        s.IdCliente,
                        s.DataServico,
                        s.Descricao,
                        s.Estado,
                        IdAnimal = a.IdAnimal,
                        a.Nome,
                        a.AnimalFoto
                    }
                        ).FirstOrDefault());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        /// <summary>
        /// Lista detalhes de um servico realizado
        /// </summary>
        /// <param name="ser"> Serviço </param>
        /// <returns> Serviço </returns>
        [Route("{idServico}/details")]
        [HttpGet, Authorize(Roles = "Admin, Gerente, Funcionario")]
        public JsonResult GetServico(int idServico)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    Servico servico = context.Servico.Where(s => s.IdServico == idServico).FirstOrDefault();

                    if (User.HasClaim(ClaimTypes.Role, "Funcionario"))
                    {
                        string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                        Funcionario f = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();
                        if (f == null) Forbid();
                        if (servico.IdFuncionario != f.IdFuncionario) Forbid();
                    }
                    if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                    {
                        if (User.HasClaim(ClaimTypes.Role, $"Gerente_{servico.IdEstabelecimento}")) Forbid();
                    }

                    List<Servico> servicos = context.Servico.Where(s => s.IdServico == servico.IdServico).ToList();
                    List<Cliente> clientes = context.Cliente.Where(c => c.IdCliente == servico.IdCliente).ToList();
                    List<Animal> animais = context.Animal.Where(a => a.IdAnimal == servico.IdAnimal).ToList();
                    List<Funcionario> funcionarios = context.Funcionario.Where(f => f.IdFuncionario == servico.IdFuncionario).ToList();
                    List<Estabelecimento> estabelecimentos = context.Estabelecimento.Where(e => e.IdEstabelecimento == servico.IdEstabelecimento).ToList();
                    List<ServicoCatalogo> catalogos = context.ServicoCatalogo.Where(c => c.IdServicoCatalogo == servico.IdServicoCatalogo).ToList();

                    var x = servicos
                        .Join(
                        clientes,
                        s => s.IdCliente,
                        c => c.IdCliente,
                        (s, c) => new
                        {
                            s.IdServico,
                            s.IdCliente,
                            s.IdAnimal,
                            s.IdFuncionario,
                            s.IdEstabelecimento,
                            s.IdServicoCatalogo,
                            DataServico = s.DataServico,
                            DescricaoServico = s.Descricao,
                            EstadoServico = s.Estado,
                            NomeCliente = c.Nome,
                            ApelidoCliente = c.Apelido,
                            EmailCliente = c.Email,
                            TelemovelCliente = c.Telemovel
                        }).Join(
                        animais,
                        x => x.IdAnimal,
                        a => a.IdAnimal,
                        (x, a) => new
                        {
                            x,
                            NomeAnimal = a.Nome,
                            ClasseAnimal = a.Classe
                        }).Join(
                        funcionarios,
                        x2 => x2.x.IdFuncionario,
                        f => f.IdFuncionario,
                        (x2, f) => new
                        {
                            x2,
                            NomeFunc = f.Nome,
                            ApelidoFunc = f.Apelido,
                            CodigoFunc = f.Codigo,
                            EmailFunc = f.Email,
                            TelemovelFunc = f.Telemovel
                        }).Join(
                        estabelecimentos,
                        x3 => x3.x2.x.IdEstabelecimento,
                        e => e.IdEstabelecimento,
                        (x3, e) => new
                        {
                            x3,
                            NomeEst = e.Nome,
                            TipoEst = e.TipoEstabelecimento,
                            ContactoEst = e.Contacto
                        }).Join(
                        catalogos,
                        x4 => x4.x3.x2.x.IdServicoCatalogo,
                        c => c.IdServicoCatalogo,
                        (x4, c) => new
                        {
                            x4,
                            PrecoServico = c.Preco,
                            TipoServico = c.Tipo,
                            DuracaoServico = c.Duracao
                        }).ToList();

                   
                    return new JsonResult(x.Select(s => new {

                        //servico
                        s.x4.x3.x2.x.IdServico,
                        s.x4.x3.x2.x.EstadoServico,
                        s.x4.x3.x2.x.DataServico,
                        s.x4.x3.x2.x.DescricaoServico,
                        // catalogo
                        s.TipoServico,
                        s.PrecoServico,
                        s.DuracaoServico,
                        // estabelecimento
                        s.x4.TipoEst,
                        s.x4.NomeEst,
                        s.x4.ContactoEst,
                        // funcionario
                        s.x4.x3.NomeFunc,
                        s.x4.x3.ApelidoFunc,
                        s.x4.x3.CodigoFunc,
                        s.x4.x3.EmailFunc,
                        s.x4.x3.TelemovelFunc,
                        // cliente
                        s.x4.x3.x2.x.NomeCliente,
                        s.x4.x3.x2.x.ApelidoCliente,
                        s.x4.x3.x2.x.EmailCliente,
                        s.x4.x3.x2.x.TelemovelCliente,
                        // animal
                        s.x4.x3.x2.NomeAnimal,
                        s.x4.x3.x2.ClasseAnimal,
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
        /// Busca e mostra os dados de todos os serviços de um cliente pelo id
        /// </summary>
        /// <param name="idCliente"> Id do cliente </param>
        /// <returns></returns>
        [Route("cliente/{idCliente}")]
        [HttpGet, Authorize(Roles = "Admin, Cliente")]
        public IEnumerable<Servico> GetHistoricoCliente(int idCliente)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                    {
                        string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                        Cliente c = context.Cliente.Where(c => c.Email == emailCliente).FirstOrDefault();

                        if (c is null) Forbid();
                    }

                    return context.Servico.Where(s => s.IdCliente == idCliente).ToList();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }
        }

        /// <summary>
        /// Busca e mostra os dados de todos os serviços de um funcionario pelo id
        /// </summary>
        /// <param name="idFuncionario"></param>
        /// <returns></returns>
        [Route("funcionario/{idFuncionario}"), Authorize]
        [HttpGet, Authorize(Roles = "Admin,Gerente,Funcionario")]
        public IActionResult GetHistoricoFuncionario(int idFuncionario)
        {
            int idFunc;
            List<int> estabelecimentosGeridos = new List<int>();
            using (var context = new FeedyVetContext())
            {
                bool hasAuth = false;
                if (User.HasClaim(ClaimTypes.Role, "Funcionario"))
                {
                    string email = User.FindFirstValue(ClaimTypes.Email);
                    idFunc = context.Funcionario.Where(f_aux => f_aux.Email == email).FirstOrDefault().IdFuncionario;
                    if (idFunc == idFuncionario)
                    {
                        hasAuth = true;
                    }
                }
                else if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                {
                    try
                    {
                        string email = User.FindFirstValue(ClaimTypes.Email);
                        idFunc = context.Funcionario.Where(f_aux => f_aux.Email == email).FirstOrDefault().IdFuncionario;
                        List<FuncionarioEstabelecimento> fe_list = context.FuncionarioEstabelecimento.Where(fe_aux => fe_aux.IdFuncionario == idFunc).ToList();

                        foreach (FuncionarioEstabelecimento fe in fe_list)
                        {
                            if (User.HasClaim(ClaimTypes.Role, $"Gerente_{fe.IdEstabelecimento}"))
                            {
                                estabelecimentosGeridos.Add(fe.IdEstabelecimento);
                                hasAuth = true;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return BadRequest();
                    } 
                }
                else if (User.HasClaim(ClaimTypes.Role, "Admin")) hasAuth = true;
                
                if (hasAuth)
                {
                    try
                    {
                        if (estabelecimentosGeridos.Count == 0)
                        {
                            return new JsonResult(context.Servico.Where(s => s.IdFuncionario == idFuncionario).ToList());
                        }


                        List<Servico> servicos = new List<Servico>();

                        foreach(int idEstab in estabelecimentosGeridos)
                        {
                            servicos.AddRange(context.Servico.Where(s => s.IdEstabelecimento == idEstab).ToList());
                        }

                        return new JsonResult(servicos);
                        
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

        [Route("funcionario"), Authorize]
        [HttpGet, Authorize(Roles = "Admin, Gerente, Funcionario")]
        public IActionResult GetHistoricoFuncionarioByToken()
        {
            int idFunc;
            List<int> estabelecimentosGeridos = new List<int>();
            using (var context = new FeedyVetContext())
            {
                bool hasAuth = false;
                string email = User.FindFirstValue(ClaimTypes.Email);
                idFunc = context.Funcionario.Where(f_aux => f_aux.Email == email).FirstOrDefault().IdFuncionario;
                if (User.HasClaim(ClaimTypes.Role, "Funcionario"))
                {
                    // scuffed
                    hasAuth = true;
                }
                else if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                {
                    try
                    {

                        List<FuncionarioEstabelecimento> fe_list = context.FuncionarioEstabelecimento.Where(fe_aux => fe_aux.IdFuncionario == idFunc).ToList();

                        foreach (FuncionarioEstabelecimento fe in fe_list)
                        {
                            if (User.HasClaim(ClaimTypes.Role, $"Gerente_{fe.IdEstabelecimento}"))
                            {
                                estabelecimentosGeridos.Add(fe.IdEstabelecimento);
                                hasAuth = true;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return BadRequest();
                    }
                }
                else if (User.HasClaim(ClaimTypes.Role, "Admin")) hasAuth = true;

                if (hasAuth)
                {
                    try
                    {
                        if (estabelecimentosGeridos.Count == 0)
                        {
                            return new JsonResult(context.Servico.Where(s => s.IdFuncionario == idFunc).ToList());
                        }


                        List<Servico> servicos = new List<Servico>();

                        foreach (int idEstab in estabelecimentosGeridos)
                        {
                            servicos.AddRange(context.Servico.Where(s => s.IdEstabelecimento == idEstab).ToList());
                        }

                        return new JsonResult(servicos);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return BadRequest();
                    }
                }
                else
                {
                    return Forbid();
                }


            }
        }


        // FINALIZAR! funciona mas talvez precise de auth e afins
        [Route("getservicobyidprescricao/{idPrescricao}"), Authorize]
        [HttpGet, Authorize(Roles = "Admin,Gerente,Funcionario")]
        public IActionResult GetServicoByIdPrescricao(int idPrescricao)
        {
            using (var context = new FeedyVetContext())
            {
                ServicoPrescricao sp = context.ServicoPrescricao.Where(sp => sp.IdPrescricao == idPrescricao).FirstOrDefault();
                Servico s = context.Servico.Where(s => s.IdServico == sp.IdServico).FirstOrDefault();
                return new JsonResult(s);
            }
        }

        /// <summary>
        /// Busca e mostra o historico de todos os serviços de uma estabelecimento pelo id
        /// </summary>
        /// <param name="idEstabelecimento"> Id do cliente </param>
        /// <returns></returns>
        [Route("estabelecimento/{idEstabelecimento}"), Authorize]
        [HttpGet, Authorize(Roles = "Admin, Gerente")]
        public IActionResult Get_Historico_Estabelecimento(int idEstabelecimento)
        {
            using (var context = new FeedyVetContext())
            {
                List<Claim> roles = User.FindAll(ClaimTypes.Role).ToList();
                bool gerente_confirm = FuncionarioController.Gerente_Estabelecimento_Confirm(idEstabelecimento, roles);

                if (gerente_confirm == false)
                {
                    return Forbid();
                }
            
                try
                {
                    List<Servico> servicos = context.Servico.Where(s => s.IdEstabelecimento == idEstabelecimento).ToList();
                    List<Animal> animais = context.Animal.ToList();
                    List<ServicoCatalogo> servicocatalogo = context.ServicoCatalogo.ToList();
                    List<Funcionario> funcionario = context.Funcionario.ToList();

                    return new JsonResult(servicos.Join(animais, s => s.IdAnimal, a => a.IdAnimal, (s, a) => new {
                        IdAnimal = a.IdAnimal,
                        s.IdCliente,
                        s.IdFuncionario,
                        s.IdEstabelecimento,
                        s.IdServicoCatalogo,
                        s.IdServico,
                        s.DataServico,
                        s.Descricao,
                        s.Estado,
                        s.MetodoPagamento,
                        a.Nome,
                        a.Peso,
                        a.Altura,
                        a.Classe,
                        a.Especie,
                        a.Genero,
                        a.AnimalFoto,
                        a.DataNascimento
                    }).Join(servicocatalogo, a => a.IdServicoCatalogo, sc => sc.IdServicoCatalogo,
                    (b, sc) => new
                    {
                        b.IdAnimal,
                        b.IdCliente,
                        b.IdFuncionario,
                        b.IdEstabelecimento,
                        b.IdServicoCatalogo,
                        b.IdServico,
                        b.DataServico,
                        b.Descricao,
                        b.Estado,
                        b.MetodoPagamento,
                        b.Nome,
                        b.Peso,
                        b.Altura,
                        b.Classe,
                        b.Especie,
                        b.Genero,
                        b.AnimalFoto,
                        b.DataNascimento,
                        sc.Preco
                    }).Join(funcionario, c => c.IdFuncionario, f => f.IdFuncionario,
                    (c, f) => new{
                        c.IdAnimal,
                        c.IdCliente,
                        c.IdFuncionario,
                        c.IdEstabelecimento,
                        c.IdServicoCatalogo,
                        c.IdServico,
                        c.DataServico,
                        c.Descricao,
                        c.Estado,
                        c.MetodoPagamento,
                        c.Nome,
                        c.Peso,
                        c.Altura,
                        c.Classe,
                        c.Especie,
                        c.Genero,
                        c.AnimalFoto,
                        c.DataNascimento,
                        c.Preco,
                        NomeFunc = f.Nome,
                        f.Apelido
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
        /// Busca e mostra o historico de todos os serviços de um animal pelo id
        /// </summary>
        /// <param name="idAnimal"> Id do animal </param>
        /// <returns></returns>
        [Route("animal/{idAnimal}"), Authorize]
        [HttpGet, Authorize(Roles = "Admin, Cliente")]
        public IActionResult Get_Historico_Animal(int idAnimal)
        {
            using (var context = new FeedyVetContext())
            {
                //Fazer confirmacao de user
                try
                {
                    List<Servico> servicos = context.Servico.Where(s => s.IdAnimal == idAnimal).ToList();
                    List<Animal> animais = context.Animal.ToList();
                    List<ServicoCatalogo> servicocatalogo = context.ServicoCatalogo.ToList();
                    List<Funcionario> funcionario = context.Funcionario.ToList();

                    return new JsonResult(servicos.Join(animais, s => s.IdAnimal, a => a.IdAnimal, (s, a) => new {
                        IdAnimal = a.IdAnimal,
                        s.IdCliente,
                        s.IdFuncionario,
                        s.IdEstabelecimento,
                        s.IdServicoCatalogo,
                        s.IdServico,
                        s.DataServico,
                        s.Descricao,
                        s.Estado,
                        s.MetodoPagamento,
                        a.Nome,
                        a.Peso,
                        a.Altura,
                        a.Classe,
                        a.Especie,
                        a.Genero,
                        a.AnimalFoto,
                        a.DataNascimento
                    }).Join(servicocatalogo, a => a.IdServicoCatalogo, sc => sc.IdServicoCatalogo,
                    (b, sc) => new
                    {
                        b.IdAnimal,
                        b.IdCliente,
                        b.IdFuncionario,
                        b.IdEstabelecimento,
                        b.IdServicoCatalogo,
                        b.IdServico,
                        b.DataServico,
                        b.Descricao,
                        b.Estado,
                        b.MetodoPagamento,
                        b.Nome,
                        b.Peso,
                        b.Altura,
                        b.Classe,
                        b.Especie,
                        b.Genero,
                        b.AnimalFoto,
                        b.DataNascimento,
                        sc.Preco,
                        sc.Tipo
                    }).Join(funcionario, c => c.IdFuncionario, f => f.IdFuncionario,
                    (c, f) => new {
                        c.IdAnimal,
                        c.IdCliente,
                        c.IdFuncionario,
                        c.IdEstabelecimento,
                        c.IdServicoCatalogo,
                        c.IdServico,
                        c.DataServico,
                        c.Descricao,
                        c.Estado,
                        c.MetodoPagamento,
                        c.Nome,
                        c.Peso,
                        c.Altura,
                        c.Classe,
                        c.Especie,
                        c.Genero,
                        c.AnimalFoto,
                        c.DataNascimento,
                        c.Preco,
                        c.Tipo,
                        NomeFunc = f.Nome,
                        f.Apelido
                    }));
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
        /// Cria um serviço 
        /// </summary>
        /// <param name="ser"> Serviço </param>
        /// <returns> Lista de serviços </returns>
        [HttpPost, Authorize(Roles = "Admin")]
        public IActionResult Post(Servico ser)
        {
            using (var context = new FeedyVetContext())
            {

                Servico servico = new Servico();
                servico.IdClienteNavigation = context.Cliente.Where(c => c.IdCliente == ser.IdCliente).FirstOrDefault();
                servico.IdAnimalNavigation = context.Animal.Where(a => a.IdAnimal == ser.IdAnimal).FirstOrDefault();
                servico.IdFuncionarioNavigation = context.Funcionario.Where(f => f.IdFuncionario == ser.IdFuncionario).FirstOrDefault();
                servico.IdEstabelecimentoNavigation = context.Estabelecimento.Where(e => e.IdEstabelecimento == ser.IdEstabelecimento).FirstOrDefault();
                servico.IdServicoCatalogoNavigation = context.ServicoCatalogo.Where(sc => sc.IdServicoCatalogo == ser.IdServicoCatalogo).FirstOrDefault();
                servico.DataServico = ser.DataServico;
                servico.Descricao = ser.Descricao;
                servico.Estado = ser.Estado;

                context.Servico.Add(servico);

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

        public static bool VerificaDisponibilidade(DateTime dataServico, Funcionario func, int idservcatalogo)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    string dia_semana_ing = dataServico.ToString("ddd", new CultureInfo("en-US")); // Mon; Tue; Wen; Thu; Fri; Sat; Sun;
                    List<FuncionarioHorario> funcHorario = context.FuncionarioHorario.Where(fh => fh.IdFuncionario == func.IdFuncionario).ToList();

                    ServicoCatalogo servcatalogo = context.ServicoCatalogo.Where(sc => sc.IdServicoCatalogo == idservcatalogo).FirstOrDefault();
                    bool dentroHorario = false;

                    foreach (FuncionarioHorario fh in funcHorario)
                    {
                        if (fh.DiaSemana == dia_semana_ing)
                        {
                            if (dataServico.TimeOfDay > fh.HoraEntrada && dataServico.TimeOfDay + servcatalogo.Duracao < fh.HoraSaida)
                            {
                                dentroHorario = true;
                                break;
                            }
                        }
                    }

                    if (dentroHorario == true)
                    {
                        List<Servico> servicos = context.Servico.Where(s => s.IdFuncionario == func.IdFuncionario).ToList();
                        foreach (Servico s in servicos)
                        {
                            ServicoCatalogo servcatalogo_s = context.ServicoCatalogo.Where(servicCatalog => servicCatalog.IdServicoCatalogo == s.IdServicoCatalogo).FirstOrDefault(); // Select do servico catalogo
                            if ((s.DataServico < dataServico && s.DataServico + servcatalogo_s.Duracao > dataServico)
                                || (s.DataServico > dataServico && dataServico + servcatalogo.Duracao > s.DataServico) || (s.DataServico == dataServico))
                            {
                                return true;
                            }
                        }
                    }
                    else return true;

                    return false;
                }
                catch (ArgumentException ae)
                {
                    Console.WriteLine(ae);
                    return true;
                }
            }
        }

        /// <summary>
        /// Marcação de um serviço
        /// </summary>
        /// <param name="idCliente"> Id do cliente </param>
        /// <param name="serv"> Serviço </param>
        /// <returns> Mensagem do sucedido </returns>
        [Route("{idCliente}/MarcarServico")]
        [HttpPost, Authorize(Roles = "Admin, Cliente")]
        public Servico MarcarServico(int idCliente, Servico serv) //Verificar vagas 
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                    {
                        string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                        Cliente c = context.Cliente.Where(c => c.Email == emailCliente && serv.IdCliente == c.IdCliente).FirstOrDefault();

                        if (c is null) Forbid();
                    }

                    Servico servico = new Servico();

                    FuncionarioEstabelecimento funcestabelecimento = context.FuncionarioEstabelecimento.Where(funcest => funcest.IdFuncionario == serv.IdFuncionario).FirstOrDefault(); // Select da clinica onde está o funcionario
                    Funcionario func = context.Funcionario.Where(funci => funci.IdFuncionario == serv.IdFuncionario).FirstOrDefault(); // Select do funcionario 
                    ServicoCatalogo servcatalogo = context.ServicoCatalogo.Where(servicCatalog => servicCatalog.IdServicoCatalogo == serv.IdServicoCatalogo).FirstOrDefault(); // Select do servico catalogo
                    Estabelecimento estabelecimento = context.Estabelecimento.Where(clc => clc.IdEstabelecimento == serv.IdEstabelecimento).FirstOrDefault(); // Select da clinica
                    Animal animal = context.Animal.Where(animale => animale.IdAnimal == serv.IdAnimal).FirstOrDefault(); // Select do animal
                    Cliente client = context.Cliente.Where(cliente => cliente.IdCliente == idCliente).FirstOrDefault(); // Select do cliente
                    ClienteAnimal clienteAnimal = context.ClienteAnimal.Where(ca => ca.IdAnimal == animal.IdAnimal && ca.IdCliente == client.IdCliente).FirstOrDefault(); // Select da relação cliente-animal

                    if (funcestabelecimento != null && func != null && servcatalogo != null && estabelecimento != null && animal != null && client != null && clienteAnimal != null)
                    {

                        bool colide = VerificaDisponibilidade(serv.DataServico, func, servcatalogo.IdServicoCatalogo);
                        
                        if (colide == false)
                        {
                            servico.IdServicoCatalogoNavigation = servcatalogo;
                            servico.IdClienteNavigation = client;
                            servico.IdAnimalNavigation = animal;
                            servico.IdFuncionarioNavigation = func;
                            servico.IdEstabelecimentoNavigation = estabelecimento;
                            servico.DataServico = serv.DataServico;
                            servico.Descricao = serv.Descricao;
                            servico.Estado = "Marcado";
                            servico.MetodoPagamento = serv.MetodoPagamento;

                            context.Servico.Add(servico);

                            Notificacao not = new Notificacao() { 
                                IdClienteNavigation = client,
                                Descricao = serv.Descricao +"\n "+ serv.DataServico,
                                Estado = "Por Visualizar",
                                DataNotificacao = serv.DataServico.AddDays(-1) 
                            };

                            context.Notificacao.Add(not);

                            context.SaveChanges();
                            return context.Servico.Where(s => s.IdServico == servico.IdServico).FirstOrDefault();
                        }
                    }  
                }

                return null;  
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// Altera parcialmente o serviço
        /// </summary>
        /// <param name="servico"> Serviço </param>
        /// <returns> Mensagem do sucedido </returns>
        [HttpPatch, Authorize(Roles = "Admin, Gerente, Funcionario")]
        public IActionResult Patch(Servico servico)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    Servico service = context.Servico.Where(c => c.IdServico == servico.IdServico).FirstOrDefault();

                    if(servico.IdCliente != 0) service.IdClienteNavigation = context.Cliente.Where(c => c.IdCliente == service.IdCliente).FirstOrDefault();
                    if (servico.IdAnimal != 0) service.IdAnimalNavigation = context.Animal.Where(c => c.IdAnimal == service.IdAnimal).FirstOrDefault();
                    if (servico.IdFuncionario != 0) service.IdFuncionarioNavigation = context.Funcionario.Where(c => c.IdFuncionario == service.IdFuncionario).FirstOrDefault();
                    if (servico.IdEstabelecimento != 0) service.IdEstabelecimentoNavigation = context.Estabelecimento.Where(c => c.IdEstabelecimento == service.IdEstabelecimento).FirstOrDefault();
                    if (servico.IdServicoCatalogo != 0) service.IdServicoCatalogoNavigation = context.ServicoCatalogo.Where(c => c.IdServicoCatalogo == service.IdServicoCatalogo).FirstOrDefault();
                    service.DataServico = servico.DataServico;
                    service.Descricao = servico.Descricao is null ? service.Descricao : servico.Descricao;
                    service.Estado = servico.Estado is null ? service.Estado : servico.Estado;

                    context.SaveChanges();
                    return Ok();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return Forbid();
                }
            }
        }

        [HttpPatch("remarcarservicosdia/{date}/{IdFuncionario}"), Authorize(Roles = "Admin")]
        public static bool DesmarcarTodasAsConsultasDia(DateTime date, int IdFuncionario)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    List<Servico> services = context.Servico.Where(s => s.IdFuncionario == IdFuncionario && s.DataServico.Date == date.Date).ToList();

                    if (services.Count == 0) return true;

                    foreach(Servico servico in services)
                    {
                        servico.Estado = "Cancelado";
                    }

                    context.SaveChanges();
                    return true;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        /// Metodo que chama função para alterar serviço
        /// </summary>
        /// <param name="servico"> Serviço </param>
        /// <returns> Serviço </returns>
        [Route("changestatusservico")]
        [HttpPatch, Authorize(Roles = "Admin,Gerente,Funcionario")]
        public IActionResult ChangeStatusServico(Servico servico)
        {
            if (User.HasClaim(ClaimTypes.Role, "Gerente"))
            {
                if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{servico.IdEstabelecimento}")) Forbid();
            }
            else if (User.HasClaim(ClaimTypes.Role, "Funcionario"))
            {
                try
                {
                    using (var context = new FeedyVetContext())
                    {
                        string funcEmail = User.FindFirstValue(ClaimTypes.Email);
                        Funcionario f = context.Funcionario.Where(f => f.Email == funcEmail).FirstOrDefault();
                        if (f.IdFuncionario != servico.IdFuncionario) return Forbid();
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }
            }

            return Patch(servico);
        }

        /// <summary>
        /// Metodo que chama função para alterar serviço
        /// </summary>
        /// <param name="servico"> Serviço </param>
        /// <returns> Serviço </returns>
        [Route("cancelservicegerente")]
        [HttpPatch, Authorize(Roles = "Admin, Gerente")]
        public IActionResult CancelServiceGerente(Servico servico)
        {
            if (User.HasClaim(ClaimTypes.Role, "Gerente"))
            {
                if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{servico.IdEstabelecimento}")) Forbid();
            }

            servico.Estado = "Inativo";

            return Patch(servico);
        }

        // VET

        // CLIENT



        /// <summary>
        /// Reagendar um serviço
        /// </summary>
        /// <param name="idCliente"> Id do cliente</param>
        /// <param name="ser"> Serviço </param>
        /// <returns> Mensagem do sucedido </returns>
        [Route("reschedule")]
        [HttpPatch, Authorize(Roles = "Admin,Gerente,Funcionario,Cliente")]
        public IActionResult RescheduleService(Servico ser)
        {
            using (var context = new FeedyVetContext())
            {
                if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                {
                    if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{ser.IdEstabelecimento}")) return Forbid();
                }

                if (User.HasClaim(ClaimTypes.Role, "Funcionario"))
                {
                    try
                    {
                        string funcEmail = User.FindFirstValue(ClaimTypes.Email);
                        Funcionario f = context.Funcionario.Where(f => f.Email == funcEmail).FirstOrDefault();
                        if (f.IdFuncionario != ser.IdFuncionario) return Forbid();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return BadRequest();
                    }
                }

                if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                {
                    string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                    Cliente c = context.Cliente.Where(c => c.Email == emailCliente).FirstOrDefault();
                    Servico s = context.Servico.Where(s => s.IdServico == ser.IdServico && s.IdCliente == c.IdCliente).FirstOrDefault();

                    if (c is null || s is null) Forbid();
                }

                try
                {
                    if (ser.Estado == "Inativo" || ser.Estado == "Concluido" || ser.Estado == "Cancelado") return BadRequest();

                    Servico serv = context.Servico.Where(s => s.IdServico == ser.IdServico).FirstOrDefault();
                    FuncionarioEstabelecimento funcestabelecimento = context.FuncionarioEstabelecimento.Where(funcest => funcest.IdFuncionario == ser.IdFuncionario).FirstOrDefault(); // Select da clinica onde está o funcionario
                    Funcionario func = context.Funcionario.Where(funci => funci.IdFuncionario == ser.IdFuncionario).FirstOrDefault(); // Select do funcionario 
                    ServicoCatalogo servcatalogo = context.ServicoCatalogo.Where(servicCatalog => servicCatalog.IdServicoCatalogo == ser.IdServicoCatalogo).FirstOrDefault(); // Select do servico catalogo
                    Estabelecimento estabelecimento = context.Estabelecimento.Where(clc => clc.IdEstabelecimento == ser.IdEstabelecimento).FirstOrDefault(); // Select da clinica
                    Animal animal = context.Animal.Where(animale => animale.IdAnimal == ser.IdAnimal).FirstOrDefault(); // Select do animal

                    bool colide = VerificaDisponibilidade(ser.DataServico, func, servcatalogo.IdServicoCatalogo);
                    
                    if (colide == false)
                    {
                        serv.DataServico = ser.DataServico;

                        context.NotificacaoFuncionario.Add(new NotificacaoFuncionario
                        {
                            IdFuncionario = ser.IdFuncionario,
                            Estado = "Por visualizar",
                            Descricao = $"O serviço {ser.IdServico} foi alterado para o dia {ser.DataServico}",
                            DataNotificacao = DateTime.Now
                        });

                        serv.Estado = "Marcado";
                        context.Notificacao.Add(new Notificacao
                        {
                            IdCliente = ser.IdCliente,
                            Estado = "Por visualizar",
                            Descricao = $"O seu serviço foi alterado para o dia {ser.DataServico}",
                            DataNotificacao = DateTime.Now
                        });
                    }
                    else return BadRequest();


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



        [Route("cancelservicecliente")]
        [HttpPatch, Authorize(Roles = "Admin, Gerente, Cliente")]
        public IActionResult CancelServiceCliente(Servico ser)
        {
            using (var context = new FeedyVetContext())
            {
                if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                {
                    if (!User.HasClaim(ClaimTypes.Role, $"Gerente_{ser.IdEstabelecimento}")) Forbid();
                }

                if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                {
                    string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                    Cliente c = context.Cliente.Where(c => c.Email == emailCliente).FirstOrDefault();
                    Servico s = context.Servico.Where(s => s.IdServico == ser.IdServico && s.IdCliente == c.IdCliente).FirstOrDefault();

                    if (c is null || s is null) Forbid();
                }

                try
                {

                    bool possivelPedirCancelamento = false;
                    Servico servico = context.Servico.Where(s => s.IdServico == ser.IdServico).FirstOrDefault();

                    if (servico.Estado != "Concluido" && servico.Estado != "Realizado" && servico.Estado != "Aguarda Pagamento")
                    {
                        if (DateTime.Compare(servico.DataServico, DateTime.Now) < 0) return BadRequest();

                        if ((servico.DataServico - DateTime.Now).TotalHours > 24)
                        {
                            possivelPedirCancelamento = true;
                        }
                    }

                    if (possivelPedirCancelamento == true)
                    {

                        context.NotificacaoFuncionario.Add(new NotificacaoFuncionario
                        {
                            IdFuncionario = servico.IdFuncionario,
                            Estado = "Por visualizar",
                            Descricao = $"O serviço foi cancelado",
                            DataNotificacao = DateTime.Now
                        });

                        servico.Estado = "Cancelado";

                        context.Notificacao.Add(new Notificacao
                        {
                            IdCliente = servico.IdCliente,
                            Estado = "Por visualizar",
                            Descricao = $"O seu serviço foi cancelado",
                            DataNotificacao = DateTime.Now
                        });
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

        [Route("{idServico}/payment")]
        [HttpPatch, Authorize(Roles = "Admin, Cliente")]
        public IActionResult PagamentoServico(int idServico)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                    {
                        string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                        Cliente c = context.Cliente.Where(c => c.Email == emailCliente).FirstOrDefault();

                        if (c is null) Forbid();
                    }

                    Servico servico = context.Servico.Where(s => s.IdServico == idServico).FirstOrDefault();
                    Cliente cliente = context.Cliente.Where(c => c.IdCliente == servico.IdCliente).FirstOrDefault();
                    ServicoCatalogo sc = context.ServicoCatalogo.Where(ser => ser.IdServicoCatalogo == servico.IdServicoCatalogo).FirstOrDefault();
                    
                    servico.Estado = "Pago";
                    
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
        /// Metodo que permite apagar um servico da base de dados
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
                    context.Servico.Remove(context.Servico.Where(s => s.IdServico == id).FirstOrDefault());

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
        [HttpPost] //Metodo para escrever na table
        public JsonResult Post(Servico servico)
        {
            String query = @"
                    insert into dbo.Servico
                    (id_cliente,id_animal,id_veterinario,id_clinica,id_servico_catalogo,data_servico,descricao,estado)
                    values
                    ( 
                    '" + servico.IdCliente + @"'
                    ,'" + servico.IdAnimal + @"'
                    ,'" + servico.IdVeterinario + @"'
                    ,'" + servico.IdClinica + @"'
                    ,'" + servico.IdServicoCatalogo + @"'
                    ,'" + servico.DataServico + @"'
                    ,'" + servico.Descricao + @"'
                    ,'" + servico.Estado + @"'
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

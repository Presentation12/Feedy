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
using System.Threading.Tasks;
using System.Security.Claims;

namespace FeedyVetAPI.Controllers
{
    /// <summary>
    /// Controller do animal que contém os seus métodos de ação e respetivas requests
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController : ControllerBase
    {
        private readonly IConfiguration _configuration;  //Injeção privada para utilizar certos metodos
        private readonly IWebHostEnvironment _env;

        public AnimalController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        #region Get

        /// <summary>
        /// Busca e mostra os atributos de todos os animais da BD  
        /// </summary>
        /// <returns> Dados de todos os animais </returns>
        [HttpGet, Authorize(Roles = "Admin")]
        public IActionResult Get()
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    return new JsonResult(context.Animal.ToList());
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Busca e mostra os atributos do animal pelo id do animal
        /// </summary>
        /// <param name="id_animal"> id do animal </param>
        /// <returns> Dados do animal </returns>
        [Route("{id_animal}")]
        [HttpGet, Authorize(Roles = "Admin,Gerente,Funcionario,Cliente")] 
        // se for gerente, tem de ser do estabelecimento ao qual o animal pertence (ja fez la pelo menos uma consulta)
        // se for funcionario, tem de ser do estabelecimento ao qual o animal pertence (ja fez la pelo menos uma consulta)
        // se for cliente, tem de ser dono deste animal
        public IActionResult GetAnimalByID(int id_animal)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    bool authorized = false;

                    if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                    {
                        string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                        Cliente c = context.Cliente.Where(c => c.Email == emailCliente).FirstOrDefault();
                        ClienteAnimal ca = context.ClienteAnimal.Where(ca => ca.IdCliente == c.IdCliente && ca.IdAnimal == id_animal).FirstOrDefault();

                        if (c != null && ca != null)
                        {
                            authorized = true;
                        }
                    }

                    if (User.HasClaim(ClaimTypes.Role, "Gerente") || User.HasClaim(ClaimTypes.Role, "Funcionario"))
                    {
                        string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                        Funcionario f = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();
                        if (f == null) return new JsonResult("Error");

                        List<FuncionarioEstabelecimento> fe = context.FuncionarioEstabelecimento.Where(fe => fe.IdFuncionario == f.IdFuncionario).ToList();

                        foreach (FuncionarioEstabelecimento fe_aux in fe)
                        {
                            if (context.Servico.Where(s => s.IdEstabelecimento == fe_aux.IdEstabelecimento && s.IdAnimal == id_animal).ToList().Count != 0)
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
                            Animal animalTarget = context.Animal.Where(a => a.IdAnimal == id_animal).FirstOrDefault();
                            
                            if(animalTarget.Estado != "Inativo") return new JsonResult(animalTarget);
                            return BadRequest();
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
            catch(Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Busca e mostra os dados de um animal que recebeu um serviço em específico
        /// </summary>
        /// <param name="idFuncionario"> ID do Funcionário </param>
        /// <param name="idServico"> ID do Serviço </param>
        /// <returns> Dados do Animal </returns>
        [Route("getanimalbyservice/{idServico}")]
        [HttpGet, Authorize(Roles = "Admin,Funcionario,Gerente")]
        // este método vê se o serviço é deste funcionario ou se este funcionário é gerente, depois, devolve o animal do serviço
        public IActionResult GetAnimalByService(int idServico)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    bool authorized = false;

                    Servico ser = context.Servico.Where(s => s.IdServico == idServico).FirstOrDefault();
                    if (ser == null) BadRequest();

                    if (User.HasClaim(ClaimTypes.Role, "Funcionario"))
                    {
                        string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                        Funcionario f = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();
                        if (f == null) Forbid();
                        if (ser.IdFuncionario == f.IdFuncionario) authorized = true;
                    }

                    if (User.HasClaim(ClaimTypes.Role, "Gerente"))
                    {
                        int idEstabelecimento = ser.IdEstabelecimento;
                        if (User.HasClaim(ClaimTypes.Role, $"Gerente_{idEstabelecimento}")) authorized = true;
                    }

                    if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                    if (authorized == true)
                    {
                        Animal animal = context.Animal.Where(a => a.IdAnimal == ser.IdAnimal).FirstOrDefault();

                        return new JsonResult(animal);
                    } else
                    {
                        return Forbid();
                    }

                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }
        }

        /// <summary>
        /// Retorna os animais de um cliente
        /// </summary>
        /// <param name="idCliente">ID do cliente</param>
        /// <returns>Lista de animais</returns>
        [Route("getanimalsbyclient/{idCliente}")]
        [HttpGet, Authorize(Roles = "Admin,Funcionario,Gerente,Cliente")]
        // se for funcionário / gerente , só deve conseguir ver os animais do cliente que tenham feito serviços nos estabelecimentos do funcionario
        // se for cliente, confirma se é o mesmo do token
        public IActionResult GetAnimalsByClient(int idCliente)
        {
            using (var context = new FeedyVetContext())
            {
                if ((User.HasClaim(ClaimTypes.Role, "Funcionario") && !User.HasClaim(ClaimTypes.Role, "Admin")) || User.HasClaim(ClaimTypes.Role, "Gerente"))
                {
                    string emailFunc = User.FindFirstValue(ClaimTypes.Email);
                    Funcionario f = context.Funcionario.Where(f => f.Email == emailFunc).FirstOrDefault();
                    if (f == null) BadRequest();

                    List<FuncionarioEstabelecimento> fe = context.FuncionarioEstabelecimento.Where(fe => fe.IdFuncionario == f.IdFuncionario).ToList();

                    List<Servico> servicos = new List<Servico>();

                    foreach (FuncionarioEstabelecimento fe_aux in fe)
                    {
                        List<Servico> servicos_aux = context.Servico.Where(s => s.IdEstabelecimento == fe_aux.IdEstabelecimento && s.IdCliente == idCliente).ToList();
                        servicos.AddRange(servicos_aux);
                    }

                    List<int> idAnimais = new List<int>();

                    foreach (Servico servico_aux in servicos)
                    {
                        if (!idAnimais.Contains(servico_aux.IdAnimal))
                        {
                            idAnimais.Add(servico_aux.IdAnimal);
                        }
                    }

                    List<Animal> animais = new List<Animal>();

                    foreach (int id in idAnimais)
                    {
                        Animal aux = context.Animal.Where(a => a.IdAnimal == id).FirstOrDefault();

                        if (aux.Estado != "Inativo") animais.Add(aux);
                    }

                    return new JsonResult(animais);
                }

                if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                {
                    string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                    Cliente c = context.Cliente.Where(c => c.Email == emailCliente).FirstOrDefault();
                    if (c == null) return Forbid();
                    else
                    {
                        try
                        {
                            List<Animal> animais_cliente = new List<Animal>();
                            List<ClienteAnimal> ca_list = context.ClienteAnimal.Where(ca => ca.IdCliente == c.IdCliente).ToList();

                            foreach (ClienteAnimal ca_aux in ca_list)
                            {
                                Animal aux = context.Animal.Where(a => a.IdAnimal == ca_aux.IdAnimal).FirstOrDefault();

                                if (aux.Estado != "Inativo") animais_cliente.Add(aux);
                            }

                            return new JsonResult(animais_cliente);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            return new JsonResult("Error");
                        }
                    }
                }

                if (User.HasClaim(ClaimTypes.Role, "Admin"))
                {
                    try
                    {
                        List<Animal> animais_cliente = new List<Animal>();
                        List<ClienteAnimal> ca_list = context.ClienteAnimal.Where(ca => ca.IdCliente == idCliente).ToList();

                        foreach (ClienteAnimal ca_aux in ca_list)
                        {
                            Animal aux = context.Animal.Where(a => a.IdAnimal == ca_aux.IdAnimal).FirstOrDefault();

                            if(aux.Estado != "Inativo") animais_cliente.Add(aux);
                        }

                        return new JsonResult(animais_cliente);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return BadRequest();
                    }
                }
                else return Forbid();
            }
        }

        /// <summary>
        /// Mostra os animais "de um" estabelecimento
        /// Os animais são considerados "de um estabelecimento" caso já lá tenham feito um serviço
        /// </summary>
        /// <param name="idEstabelecimento">id do estabelecimento</param>
        /// <returns>Lista de animais</returns>
        [Route("getanimalsbyestabelecimento/{idEstabelecimento}")]
        [HttpGet, Authorize(Roles = "Admin,Gerente,Funcionario")]
        // se for gerente, só deve conseguir ver os animais do estabelecimento caso ele seja gerente deste estabelecimento específico
        public IActionResult GetAnimalsByEstabelecimento(int idEstabelecimento)
        {
            using (var context = new FeedyVetContext())
            {
                if (User.HasClaim(ClaimTypes.Role, $"Gerente_{idEstabelecimento}") || User.HasClaim(ClaimTypes.Role, $"Funcionario_{idEstabelecimento}") ||  User.HasClaim(ClaimTypes.Role, "Admin"))
                {
                    List<int> idAnimais = new List<int>();
                    List<Servico> servicos_estabelecimento = context.Servico.Where(s => s.IdEstabelecimento == idEstabelecimento).ToList();
                    foreach (Servico servico_aux in servicos_estabelecimento)
                    {
                        if (!idAnimais.Contains(servico_aux.IdAnimal))
                        {
                            idAnimais.Add(servico_aux.IdAnimal);
                        }
                    }

                    List<Animal> animaisEstabelecimento = new List<Animal>();

                    foreach (int idAnimal in idAnimais)
                    {
                        animaisEstabelecimento.Add(context.Animal.Where(a => a.IdAnimal == idAnimal).FirstOrDefault());
                    }

                    return new JsonResult(animaisEstabelecimento);

                } else
                {
                    return Forbid();
                }

            }



        }

        [Route("getanimalsbyfuncionario")]
        [HttpGet, Authorize(Roles = "Gerente,Funcionario")]
        // se for gerente, só deve conseguir ver os animais do estabelecimento caso ele seja gerente deste estabelecimento específico
        public IActionResult GetAnimalsByFuncionario()
        {
            using (var context = new FeedyVetContext())
            {
                string emailFuncionario = User.FindFirstValue(ClaimTypes.Email);
                Funcionario f = context.Funcionario.Where(f => f.Email == emailFuncionario).FirstOrDefault();
                if (f == null)
                {
                    return Forbid();
                } else
                {
                    
                    List<FuncionarioEstabelecimento> fe = context.FuncionarioEstabelecimento.Where(fe => fe.IdFuncionario == f.IdFuncionario).ToList();
                    List<int> idAnimais = new List<int>();
                    foreach(FuncionarioEstabelecimento fe_aux in fe)
                    {
                        List<Servico> servicos_estabelecimento = context.Servico.Where(s => s.IdEstabelecimento == fe_aux.IdEstabelecimento).ToList();
                        foreach (Servico servico_aux in servicos_estabelecimento)
                        {
                            if (!idAnimais.Contains(servico_aux.IdAnimal))
                            {
                                idAnimais.Add(servico_aux.IdAnimal);
                            }
                        }
                    }
                    List<Animal> animaisFuncionario = new List<Animal>();

                    foreach (int idAnimal in idAnimais)
                    {
                        animaisFuncionario.Add(context.Animal.Where(a => a.IdAnimal == idAnimal).FirstOrDefault());
                    }

                    return new JsonResult(animaisFuncionario);
                }
            }
        }




            #endregion

        #region Post

        /// <summary>
        /// Envia os dados do animal para o servidor API / Registo do animal
        /// </summary>
        /// <param name="ani"> Animal </param>
        /// <returns> Mensagem do sucedido </returns>
        [HttpPost, Authorize(Roles = "Admin")]
        public IActionResult Post(Animal ani)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    Animal animal = new Animal();
                    animal.IdAnimal = ani.IdAnimal;
                    animal.Nome = ani.Nome;
                    animal.Peso = ani.Peso;
                    animal.Altura = ani.Altura;
                    animal.Classe = ani.Classe;
                    animal.Especie = ani.Especie;
                    animal.Genero = ani.Genero;
                    animal.Estado = "Ativo";
                    animal.AnimalFoto = ani.AnimalFoto;
                    animal.DataNascimento = ani.DataNascimento;

                    context.Animal.Add(animal);

                    return Ok();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }

            }
        }

        public static bool VerificaAddAnimalPlano(Cliente cliente)
        {
            using (var context = new FeedyVetContext())
            {
                if (context.Cliente.Where(c => c.IdCliente == cliente.IdCliente).FirstOrDefault() is null)
                    throw new ArgumentException("Cliente Inexistente", "cliente");

                if (cliente.TipoConta == "Livre" && context.ClienteAnimal.Where(ca => ca.IdCliente == cliente.IdCliente).Count() > 0)
                {
                    throw new ArgumentException("Opção Limitada pelo plano do cliente", "cliente");
                }
            }

            return true;
        }

        /// <summary>
        /// Envia os dados do animal para o servidor API / Registo do animal
        /// Esse animal é associado a um utilizador através do id utilizador na relação Cliente <-> Animal
        /// </summary>
        /// <param name="idCliente"> Id do cliente </param>
        /// <param name="ani"> Animal </param>
        /// <returns> Mensagem do sucedido </returns>
        [Route("{idCliente}/addanimal")]
        [HttpPost, Authorize(Roles = "Admin,Cliente")]
        public IActionResult AddAnimal(int idCliente, Animal ani)
        {
            bool authorized = CheckCliente();
            if (authorized == true)
            {
                using (var context = new FeedyVetContext())
                {
                    try
                    {

                        Animal animal = new Animal();
                        ClienteAnimal ani_cli = new ClienteAnimal();
                        Cliente cliente = context.Cliente.Where(c => c.IdCliente == idCliente).FirstOrDefault();

                        VerificaAddAnimalPlano(cliente);

                        animal.Nome = ani.Nome;
                        animal.Peso = ani.Peso;
                        animal.Altura = ani.Altura;
                        animal.Classe = ani.Classe;
                        animal.Especie = ani.Especie;
                        animal.Genero = ani.Genero;
                        animal.Estado = "Ativo";
                        animal.AnimalFoto = ani.AnimalFoto;
                        animal.DataNascimento = ani.DataNascimento;

                        ani_cli.IdAnimalNavigation = animal;
                        ani_cli.IdClienteNavigation = cliente;

                        Post(animal);
                        context.ClienteAnimal.Add(ani_cli);

                        context.SaveChanges();
                        return Ok();
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
            } else
            {
                return Forbid();
            }
            
        }

        // Pode precisar de refactoring!
        /// <summary>
        /// Guarda a foto do animal
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
                var physicalPath = _env.ContentRootPath + "/images/" + filename;

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult(filename);
            }
            catch (Exception)
            {
                return new JsonResult("avatar.png");
            }
        }
        #endregion

        #region Update
        
        /// <summary>
        /// Altera os dados do animal parcialmente
        /// </summary>
        /// <param name="ani"> Animal </param>
        /// <returns> Lista dos animais (confirmação de edição) </returns>
        

        // Faltam confirmações de role dentro da função (se este gerente realmente pode dar update a este animal)
        [HttpPatch, Authorize(Roles = "Admin,Gerente")]
        public IActionResult Patch(Animal ani)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {  
                    Animal animal = context.Animal.Where(a => a.IdAnimal == ani.IdAnimal).FirstOrDefault();

                    animal.Nome = ani.Nome is null ? animal.Nome : ani.Nome;
                    animal.Peso = ani.Peso == animal.Peso ? animal.Peso : ani.Peso;
                    animal.Altura = ani.Altura == animal.Altura ? animal.Altura : ani.Altura;
                    animal.Classe = ani.Classe is null ? animal.Classe : ani.Classe;
                    animal.Especie = ani.Especie is null ? animal.Especie : ani.Especie;
                    animal.Genero = ani.Genero is null ? animal.Genero : ani.Genero;
                    animal.Estado = ani.Estado is null ? animal.Estado : ani.Estado;
                    animal.AnimalFoto = ani.AnimalFoto is null ? animal.AnimalFoto : ani.AnimalFoto;
                    animal.DataNascimento = ani.DataNascimento is null ? animal.DataNascimento : ani.DataNascimento;

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
        /// Edita do animal
        /// Verifica se o animal pertence ao utilizador e altera utilizando o metodo Patch
        /// </summary>
        /// <param name="ani"> Animal </param>
        /// <param name="idCliente"> Id do cliente </param>
        /// <returns> Mensagem do sucedido </returns>
        [Route("{idCliente}/editanimal")]
        [HttpPatch, Authorize(Roles = "Admin,Cliente")]
        public IActionResult EditAnimal(Animal ani, int idCliente)
        {
            bool authorized = CheckCliente();

            if (authorized == true)
            {
                using (var context = new FeedyVetContext())
                {
                    try
                    {
                        if (ani.Estado == "Inativo")
                        {
                            return BadRequest();
                        }
                        ClienteAnimal ca = context.ClienteAnimal.Where(ca2 => ca2.IdCliente == idCliente && ca2.IdAnimal == ani.IdAnimal).FirstOrDefault();
                        if (ca != null)
                        {
                            return Patch(ani);
                        }
                        return Forbid();
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

        #endregion

        #region Delete

        /// <summary>
        /// Método que permite apagar um animal da base de dados
        /// </summary>
        /// <param name="idAnimal"> ID do Animal </param>
        /// <returns> Estado do método </returns>
        [Route("{idAnimal}")]
        [HttpDelete, Authorize(Roles = "Admin")]
        public IActionResult DeleteAnimal(int idAnimal)
        {
            using (var context = new FeedyVetContext())
            {
                Animal animal = context.Animal.Where(a => a.IdAnimal == idAnimal).FirstOrDefault();
                context.Animal.Remove(animal);
                context.SaveChanges();
                return Ok();
            }
        }

        /// <summary>
        /// Arquiva o animal
        /// Altera o estado do animal para inativo
        /// </summary>
        /// <param name="idAnimal"> Id do animal </param>
        /// <returns></returns>
        [Route("{idAnimal}/del")]
        [HttpPatch, Authorize(Roles = "Admin,Cliente")]
        public IActionResult DelAnimal(int idAnimal)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    if(User.HasClaim(ClaimTypes.Role, "Cliente"))
                    {
                        string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                        Cliente c = context.Cliente.Where(c => c.Email == emailCliente).FirstOrDefault();

                        Animal animal = context.Animal.Where(a => a.IdAnimal == idAnimal).FirstOrDefault();

                        if (animal.Estado == "Inativo") BadRequest();
                        
                        ClienteAnimal ca = context.ClienteAnimal.Where(ca => ca.IdAnimal == animal.IdAnimal && ca.IdCliente == c.IdCliente).FirstOrDefault();

                        if (c != null && animal != null && ca != null)
                        {
                            animal.Estado = "Inativo";
                            return Patch(animal);
                        }

                        return Forbid();
                    }
                    else if (User.HasClaim(ClaimTypes.Role, "Admin"))
                    {
                        Animal animal = context.Animal.Where(a => a.IdAnimal == idAnimal).FirstOrDefault();
                        
                        if (animal != null)
                        {
                            animal.Estado = "Inativo";
                            return Patch(animal);
                        }

                        return BadRequest();
                    }
                    else
                    {
                        return Forbid();
                    }
                } catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }
                
            }
        }

        #endregion

        #region Verify

        /// <summary>
        /// Altera o estado do animal para verificado
        /// </summary>
        /// <param name="idAnimal"> Id do animal </param>
        /// <returns> Mensagem do sucedido </returns>
        [Route("{idAnimal}/verify")]
        [HttpPatch, Authorize(Roles = "Admin,Gerente,Funcionario")]
        public IActionResult VerifyAnimal(int idAnimal)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    bool authorized = false;

                    if (User.HasClaim(ClaimTypes.Role, "Gerente") || User.HasClaim(ClaimTypes.Role, "Funcionario"))
                    {
                        string emailFuncionario = User.FindFirstValue(ClaimTypes.Email);
                        Funcionario f = context.Funcionario.Where(f => f.Email == emailFuncionario).FirstOrDefault();
                        if (f == null) return Forbid();

                        List<FuncionarioEstabelecimento> fe = context.FuncionarioEstabelecimento.Where(fe => fe.IdFuncionario == f.IdFuncionario).ToList();

                        foreach (FuncionarioEstabelecimento fe_aux in fe)
                        {
                            if (context.Servico.Where(s => s.IdEstabelecimento == fe_aux.IdEstabelecimento && s.IdAnimal == idAnimal).FirstOrDefault() != null)
                            {
                                authorized = true;
                            }
                        }
                    }
                    else if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                    if (authorized == true)
                    {
                        Animal animal = context.Animal.Where(a => a.IdAnimal == idAnimal).FirstOrDefault();
                        animal.Estado = "Verificado";
                        return Patch(animal);
                    } else
                    {
                        return Forbid();
                    }
                    
                } catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }
                
            }
        }

        #endregion

        #region Comentario
        /*[HttpGet] //Metodo para ir buscar os dados na tabela
        public JsonResult Get()
        {
            string query = @"
                select *
                from dbo.Animal
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

        /*[HttpPost] //Metodo para escrever na table
        public JsonResult Post(Animal animal)
        {
            String query = @"
                    insert into dbo.Animal
                    (nome,peso,altura,classe,especie,genero,estado,animal_foto,data_nascimento)
                    values
                    ( 
                    '" + animal.Nome + @"'
                    ,'" + animal.Peso + @"'
                    ,'" + animal.Altura + @"'
                    ,'" + animal.Classe + @"'
                    ,'" + animal.Especie + @"'
                    ,'" + animal.Genero + @"'
                    ,'" + animal.Estado + @"'
                    ,'" + animal.AnimalFoto + @"'
                    '" + animal.DataNascimento + @"'
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
        public JsonResult Put(Animal animal)
        {
            string query = @"
                    update dbo.Animal set
                    nome = '" + animal.Nome + @"'
                    ,peso = '" + animal.Peso + @"'
                    ,altura = '" + animal.Altura + @"'
                    ,classe = '" + animal.Classe + @"'
                    ,especie = '" + animal.Especie + @"'
                    ,genero = '" + animal.Genero + @"'
                    ,estado = '" + animal.Estado + @"'
                    ,animal_foto = '" + animal.AnimalFoto + @"'
                    ,data_nascimento = '" + animal.DataNascimento + @"'
                    Where id_animal = " + animal.IdAnimal + @"
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

        /*[HttpDelete("{id}")] //Metodo para dar delete dos valores na table //Passar por referencia por ser com o ID
        public JsonResult Delete(int id)
        {
            string query = @"
                Delete from dbo.Animal 
                where id_animal = " + id + @"
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

        public bool CheckCliente()
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
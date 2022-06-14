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
    public class LembreteController : ControllerBase
    {
        private readonly IConfiguration _configuration;  //Injeção privada para utilizar certos metodos
        public LembreteController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Get

        /// <summary>
        /// Mostra um lembrete
        /// </summary>
        /// <param name="idLembrete"> id do lembrete </param>
        /// <returns> lembrete </returns>
        [Route("{idLembrete}")]
        [HttpGet, Authorize(Roles = "Admin,Cliente")]
        public IActionResult GetLembrete(int idLembrete)
        {
            using (var context = new FeedyVetContext())
            {
                Lembrete lembrete = context.Lembrete.Where(l => l.IdLembrete == idLembrete).FirstOrDefault();
                ClienteAnimal clienteAnimal = context.ClienteAnimal.Where(ca => ca.IdAnimal == lembrete.IdAnimal).FirstOrDefault();
                Cliente cliente = context.Cliente.Where(c => c.IdCliente == clienteAnimal.IdCliente).FirstOrDefault();
                bool authorized = false;
                    
                if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                {
                    string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                    Cliente c = context.Cliente.Where(c => c.Email == emailCliente).FirstOrDefault();

                    if (c != null && c.IdCliente == cliente.IdCliente)
                    {
                        authorized = true;
                    }

                }

                if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                if (authorized == false) return Forbid();

                try
                {
                    return new JsonResult(context.Lembrete.Where(l => l.IdLembrete == lembrete.IdLembrete).Select(l => new
                    {
                        l.DataLembrete,
                        l.Frequencia,
                        l.HoraLembrete,
                        l.LembreteDescricao
                    }).FirstOrDefault());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest();
                }
            }
        }

        /// <summary>
        /// Método que mostra todos os lembretes associados aos animais de um cliente
        /// </summary>
        /// <param name="idCliente"> ID do Cliente </param>
        /// <returns> Os lembretes em causa </returns>
        [Route("showall/{idCliente}")]
        [HttpGet, Authorize(Roles = "Admin, Cliente")]
        public IActionResult GetLembretes(int idCliente)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    List<Animal> animais = context.Animal.ToList();
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
                    else if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                    if (authorized == false) return Forbid();

                    List<Lembrete> lembretes = context.Lembrete.ToList();
                    List<Lembrete> lst = new List<Lembrete>();

                    foreach (Lembrete lembrete in lembretes)
                    {
                        if (context.ClienteAnimal.Where(ca => ca.IdAnimal == lembrete.IdAnimal
                         && ca.IdCliente == idCliente).FirstOrDefault() != null) lst.Add(lembrete);
                    }

                    return new JsonResult(lst.Join(animais, l => l.IdAnimal, a => a.IdAnimal,
                        (l, a) => new
                        {
                            l.IdLembrete,
                            IdAnimal = a.IdAnimal,
                            l.LembreteDescricao,
                            l.DataLembrete,
                            l.HoraLembrete,
                            l.Frequencia,
                            a.Nome
                        }));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        /// <summary>
        /// Método que mostra todos os lembretes associados a um animal
        /// </summary>
        /// <param name="idCliente"> ID do Cliente </param>
        /// <returns> Os lembretes em causa </returns>
        [Route("showallanimal/{idAnimal}")]
        [HttpGet, Authorize(Roles = "Admin, Cliente")]
        public IActionResult GetLembretesAnimal(int idAnimal)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    List<Animal> animais = context.Animal.ToList();
                    bool authorized = false;

                    if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                    {
                        string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                        Cliente c = context.Cliente.Where(c => c.Email == emailCliente).FirstOrDefault();

                        if (c != null)
                        {
                            authorized = true;
                        }
                    }
                    else if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                    if (authorized == false) return Forbid();

                    List<Lembrete> lembretes = context.Lembrete.Where(lem => lem.IdAnimal == idAnimal).ToList();

                    return new JsonResult(lembretes.Join(animais, l => l.IdAnimal, a => a.IdAnimal,
                        (l, a) => new
                        {
                            l.IdLembrete,
                            IdAnimal = a.IdAnimal,
                            l.LembreteDescricao,
                            l.DataLembrete,
                            l.HoraLembrete,
                            l.Frequencia,
                            a.Nome
                        }));
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
        /// Criar lembrete
        /// </summary>
        /// <param name="lemb"> Lembrete </param>
        /// <returns> Mensagem do sucedido </returns>
        [HttpPost, Authorize(Roles = "Admin")]
        public IActionResult Post(Lembrete lemb)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {   
                    Lembrete lembrete = new Lembrete();
                    Animal animal = context.Animal.Where(a => a.IdAnimal == lemb.IdAnimal).FirstOrDefault();
                    
                    lembrete.IdAnimalNavigation = animal;
                    lembrete.LembreteDescricao = lemb.LembreteDescricao;
                    lembrete.DataLembrete = lemb.DataLembrete;
                    lembrete.HoraLembrete = lemb.HoraLembrete;
                    lembrete.Frequencia = lemb.Frequencia;
                    

                    context.Lembrete.Add(lembrete);

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
        /// Adiciona lembrete a BD chamando o Post
        /// </summary>
        /// <param name="lembrete"> Lembrete </param>
        /// <returns> Lembrete </returns>
        [Route("addlembrete")]
        [HttpPost, Authorize(Roles = "Admin,Cliente")]
        public IActionResult AddLembrete(Lembrete lemb)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    ClienteAnimal clienteAnimal = context.ClienteAnimal.Where(ca => ca.IdAnimal == lemb.IdAnimal).FirstOrDefault();
                    Cliente cliente = context.Cliente.Where(c => c.IdCliente == clienteAnimal.IdCliente && c.TipoConta != "Livre").FirstOrDefault();

                    if (cliente is null) return BadRequest();

                    bool authorized = false;

                    if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                    {
                        string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                        Cliente c = context.Cliente.Where(c => c.Email == emailCliente).FirstOrDefault();

                        if (c != null && c.IdCliente == cliente.IdCliente)
                        {
                            authorized = true;
                        }

                    }

                    if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                    if (authorized == false) return Forbid();

                    return Post(lemb);
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
        /// Editar lembrete
        /// </summary>
        /// <param name="idCliente"> Id do cliente </param>
        /// <param name="lemb"> Lembrete </param>
        /// <returns></returns>
        [Route("{idCliente}/editreminder")]
        [HttpPatch,Authorize(Roles = "Admin,Cliente")] // Nesta função, confirmar se quem faz isto é o próprio lembrete
        public IActionResult Patch(int idCliente, Lembrete lemb)
        {
            using (var context = new FeedyVetContext())
            {
                Lembrete lembrete = context.Lembrete.Where(l => l.IdLembrete == lemb.IdLembrete).FirstOrDefault();
                ClienteAnimal clienteAnimal = context.ClienteAnimal.Where(ca => ca.IdAnimal == lembrete.IdAnimal).FirstOrDefault();
                Cliente cliente = context.Cliente.Where(c => c.IdCliente == clienteAnimal.IdCliente).FirstOrDefault();
                bool authorized = false;

                if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                {
                    string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                    Cliente c = context.Cliente.Where(c => c.Email == emailCliente).FirstOrDefault();

                    if (c != null && c.IdCliente == cliente.IdCliente)
                    {
                        authorized = true;
                    }

                }

                if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                if (authorized == false) return Forbid();

                try
                {
                    lembrete.IdAnimal = lemb.IdAnimal is 0 ? lembrete.IdAnimal : lemb.IdAnimal;
                    lembrete.LembreteDescricao = lemb.LembreteDescricao is null ? lembrete.LembreteDescricao : lemb.LembreteDescricao;
                    lembrete.DataLembrete = lemb.DataLembrete;
                    lembrete.HoraLembrete = lemb.HoraLembrete;
                    lembrete.Frequencia = lemb.Frequencia;

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
        /// Remove um lembrete
        /// </summary>
        /// <param name="idCliente"> Id do cliente </param>
        /// <param name="lem"> Lembrete </param>
        /// <returns> Mensagem do sucedido </returns>
        [Route("{idLembrete}/remreminder")]
        [HttpDelete, Authorize(Roles = "Admin, Cliente")]
        public IActionResult RemoveReminder(int idLembrete)
        {
            using (var context = new FeedyVetContext())
            {
                try
                {
                    Lembrete lembrete = context.Lembrete.Where(l => l.IdLembrete == idLembrete).FirstOrDefault();
                    ClienteAnimal clienteAnimal = context.ClienteAnimal.Where(ca => ca.IdAnimal == lembrete.IdAnimal).FirstOrDefault();
                    Cliente cliente = context.Cliente.Where(c => c.IdCliente == clienteAnimal.IdCliente).FirstOrDefault();
                    bool authorized = false;

                    if (clienteAnimal == null) return new JsonResult("Lembrete já foi eliminado, dê refresh para atualizar a página");

                    if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                    {
                        string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                        Cliente c = context.Cliente.Where(c => c.Email == emailCliente).FirstOrDefault();

                        if (c != null && c.IdCliente == cliente.IdCliente)
                        {
                            authorized = true;
                        }

                    }

                    if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                    if (authorized == false) return Forbid();



                    context.Lembrete.Remove(lembrete);
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

    }
}

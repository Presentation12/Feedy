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
    public class MoradaController : ControllerBase
    {
        private readonly IConfiguration _configuration;  //Injeção privada para utilizar certos metodos
        public MoradaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Get

        /// <summary>
        /// Mostra todas as moradas dos users e clinicas
        /// Autorizado ao admin apenas
        /// </summary>
        /// <returns> Todas as moradas </returns>
        [HttpGet, Authorize(Roles = "Admin")]
        public IActionResult GetAllAddresses()
        {
            using (var context = new FeedyVetContext())
            {
                bool authorized = false;
                if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;
                
                if (authorized == false) return Forbid();

                return new JsonResult(context.Morada.Select(m => new
                {
                    m.Pais,
                    m.Distrito,
                    m.Concelho,
                    m.Freguesia,
                    m.Rua,
                    m.Porta,
                    m.Andar,
                    m.CodigoPostal

                }).ToList());
            }
        }

        /// <summary>
        /// Busca e mostra o endereço do cliente
        /// Autoprizado ao cliente que possui a morada e ao admin
        /// </summary>
        /// <param name="id_cliente"> Id do cliente </param>
        /// <returns> Morada do cliente </returns>
        [Route("getclientaddress/{id_cliente}")]
        [HttpGet, Authorize(Roles = "Admin,Cliente")]
        public IActionResult GetClientAdress(int id_cliente)
        {
            using (var context = new FeedyVetContext())
            {

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

                if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                if (authorized == false) return Forbid();

                try
                {
                    Cliente cli = context.Cliente.Where(c => c.IdCliente == id_cliente).FirstOrDefault();

                    return new JsonResult(context.Morada.Where(m => m.IdMorada == cli.IdMorada).Select(m => new
                    {
                        m.Pais,
                        m.Distrito,
                        m.Concelho,
                        m.Freguesia,
                        m.Rua,
                        m.Porta,
                        m.Andar,
                        m.CodigoPostal

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
        /// Busca e mostra a morada de uma clinica
        /// Autorizada a todas as roles
        /// </summary>
        /// <param name="id_establishment"> Id do estabelecimento </param>
        /// <returns></returns>
        [Route("getestablishmentaddress/{id_establishment}")]
        [HttpGet, Authorize(Roles = "Admin,Gerente,Funcionario,Cliente")]
        public IActionResult GetEstablishmentAdress(int id_establishment)
        {
            using (var context = new FeedyVetContext())
            {

                bool authorized = false;

                if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;
                if (User.HasClaim(ClaimTypes.Role, "Gerente")) authorized = true;
                if (User.HasClaim(ClaimTypes.Role, "Funcionario")) authorized = true;
                if (User.HasClaim(ClaimTypes.Role, "Cliente")) authorized = true;

                if (authorized == false) return Forbid();

                Estabelecimento e = context.Estabelecimento.Where(e => e.IdEstabelecimento == id_establishment).FirstOrDefault();

                try
                {
                    return new JsonResult(context.Morada.Where(m => m.IdMorada == e.IdMorada).Select(m => new
                    {
                        m.Pais,
                        m.Distrito,
                        m.Concelho,
                        m.Freguesia,
                        m.Rua,
                        m.Porta,
                        m.Andar,
                        m.CodigoPostal

                    }).FirstOrDefault());

                }
                catch (Exception ee)
                {
                    Console.WriteLine(ee);
                    return BadRequest();
                }
            }
        }

        #endregion

        #region Post

        /// <summary>
        /// Adiciona uma morada ao cliente
        /// Cliente e admin autorizados
        /// </summary>
        /// <param name="morada"> Morada </param>
        /// <returns> Mensagem do sucedido </returns>
        [Route("addaddressclient/{id_cliente}")]
        [HttpPost, Authorize(Roles = "Admin,Cliente")]
        public IActionResult AddAdressClient(int id_cliente, Morada m)
        {
            using (var context = new FeedyVetContext())
            {
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

                if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                if (authorized == false) return Forbid();

                try
                {
                    Morada morada = new Morada();
                    Cliente cliente = context.Cliente.Where(c => c.IdCliente == id_cliente).FirstOrDefault();

                    morada.Pais = m.Pais;
                    morada.Distrito = m.Distrito;
                    morada.Concelho = m.Concelho;
                    morada.Freguesia = m.Freguesia;
                    morada.Rua = m.Rua;
                    morada.Porta = m.Porta;
                    morada.Andar = m.Andar;
                    morada.CodigoPostal = m.CodigoPostal;
                    morada.IdMorada = m.IdMorada;

                    cliente.IdMoradaNavigation = morada;

                    context.Morada.Add(morada);

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
        /// Adiciona uma morada ao estabelecimento
        /// </summary>
        /// <param name="id_establishment"> id do estabelecimento </param>
        /// <param name="m"> morada </param>
        /// <returns> Menagem do sucedido </returns>
        [Route("addaddressclient/{id_establishment}")]
        [HttpPost, Authorize(Roles = "Admin,Gerente")]
        public IActionResult AddAdressEstablishment(int id_establishment, Morada m)
        {
            using (var context = new FeedyVetContext())
            {
                bool authorized = false;

                if (User.HasClaim(ClaimTypes.Role, $"Gerente_{id_establishment}") || User.HasClaim(ClaimTypes.Role, "Admin"))
                {
                    authorized = true;
                }

                if (authorized == false) return Forbid();

                try
                {
                    Morada morada = new Morada();
                    Estabelecimento estabelecimento = context.Estabelecimento.Where(e => e.IdEstabelecimento == id_establishment).FirstOrDefault();

                    morada.Pais = m.Pais;
                    morada.Distrito = m.Distrito;
                    morada.Concelho = m.Concelho;
                    morada.Freguesia = m.Freguesia;
                    morada.Rua = m.Rua;
                    morada.Porta = m.Porta;
                    morada.Andar = m.Andar;
                    morada.CodigoPostal = m.CodigoPostal;
                    morada.IdMorada = m.IdMorada;

                    estabelecimento.IdMoradaNavigation = morada;

                    context.Morada.Add(morada);

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
        /// Editar morada de um cliente
        /// Autorizado ao cliente e ao admin
        /// </summary>
        /// <param name="mor"> morada </param>
        /// <returns> mensagem do sucedido </returns>
        [Route("editaddressclient")]
        [HttpPatch, Authorize(Roles = "Admin,Cliente")]
        public IActionResult EditAddressClient(Morada mor)
        {
            using (var context = new FeedyVetContext())
            {
                bool authorized = false;

                if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                {
                    string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                    Cliente c = context.Cliente.Where(c => c.Email == emailCliente).FirstOrDefault();

                    if (c != null && c.IdMorada == mor.IdMorada)
                    {
                        authorized = true;
                    }
                }

                if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                if (authorized == false) return Forbid();

                try
                {
                    Morada morada = context.Morada.Where(m => m.IdMorada == mor.IdMorada).FirstOrDefault();

                    morada.Pais = mor.Pais is null ? morada.Pais : mor.Pais;
                    morada.Distrito = mor.Distrito is null ? morada.Distrito : mor.Distrito;
                    morada.Concelho = mor.Concelho is null ? morada.Concelho : mor.Concelho;
                    morada.Freguesia = mor.Freguesia is null ? morada.Freguesia : mor.Freguesia;
                    morada.Rua = mor.Rua is null ? morada.Rua : mor.Rua;
                    morada.Porta = mor.Porta == 0 ? morada.Porta : mor.Porta;
                    morada.Andar = mor.Andar == 0 ? morada.Andar : mor.Andar;
                    morada.CodigoPostal = mor.CodigoPostal is null ? morada.CodigoPostal : mor.CodigoPostal;

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
        /// Editar morada de um estabelecimento
        /// </summary>
        /// <param name="mor"> morada </param>
        /// <param name="id_establishment"> id do estabelecimento </param>
        /// <returns> mensagem do sucedido </returns>
        [Route("editaddressestablishment/{id_establishment}")]
        [HttpPatch, Authorize(Roles = "Admin,Gerente")]
        public IActionResult EditAddressEstablishment(Morada mor, int id_establishment)
        {
            using (var context = new FeedyVetContext())
            {
                bool authorized = false;

                if (User.HasClaim(ClaimTypes.Role, $"Gerente_{id_establishment}") || User.HasClaim(ClaimTypes.Role, "Admin"))
                {
                    authorized = true;
                }

                if (authorized == false) return Forbid();

                try
                {
                    Morada morada = context.Morada.Where(m => m.IdMorada == mor.IdMorada).FirstOrDefault();

                    morada.Pais = mor.Pais is null ? morada.Pais : mor.Pais;
                    morada.Distrito = mor.Distrito is null ? morada.Distrito : mor.Distrito;
                    morada.Concelho = mor.Concelho is null ? morada.Concelho : mor.Concelho;
                    morada.Freguesia = mor.Freguesia is null ? morada.Freguesia : mor.Freguesia;
                    morada.Rua = mor.Rua is null ? morada.Rua : mor.Rua;
                    morada.Porta = mor.Porta == 0 ? morada.Porta : mor.Porta;
                    morada.Andar = mor.Andar == 0 ? morada.Andar : mor.Andar;
                    morada.CodigoPostal = mor.CodigoPostal is null ? morada.CodigoPostal : mor.CodigoPostal;

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
        /// Remove o endereço de um cliente
        /// Autorizado ao admin e ao cliente
        /// </summary>
        /// <param name="mor"> morada </param>
        /// <returns> mensagem do sucedido </returns>
        [Route("removeaddressclient")]
        [HttpDelete, Authorize(Roles = "Admin,Cliente")]
        public IActionResult DeleteAddressClient(Morada mor)
        {
            using (var context = new FeedyVetContext())
            {
                bool authorized = false;

                if (User.HasClaim(ClaimTypes.Role, "Cliente"))
                {
                    string emailCliente = User.FindFirstValue(ClaimTypes.Email);
                    Cliente c = context.Cliente.Where(c => c.Email == emailCliente).FirstOrDefault();

                    if (c != null && c.IdMorada == mor.IdMorada)
                    {
                        authorized = true;
                    }
                }

                if (User.HasClaim(ClaimTypes.Role, "Admin")) authorized = true;

                if (authorized == false) return Forbid();

                try
                {
                    Morada morada = context.Morada.Where(m => m.IdMorada == mor.IdMorada).FirstOrDefault();
                    context.Morada.Remove(morada);
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
        /// Remove o endereço de um estabelecimento
        /// </summary>
        /// <param name="mor"> morada </param>
        /// <param name="id_establishment"> id do estabelecimento </param>
        /// <returns> mensagem do sucedido </returns>
        [Route("removeaddressestablishment/{id_establishment}")]
        [HttpDelete, Authorize(Roles = "Admin,Gerente")]
        public IActionResult DeleteAddressClient(Morada mor, int id_establishment)
        {
            using (var context = new FeedyVetContext())
            {
                bool authorized = false;

                if (User.HasClaim(ClaimTypes.Role, $"Gerente_{id_establishment}") || User.HasClaim(ClaimTypes.Role, "Admin"))
                {
                    authorized = true;
                }

                if (authorized == false) return Forbid();

                try
                {
                    Morada morada = context.Morada.Where(m => m.IdMorada == mor.IdMorada).FirstOrDefault();

                    context.SaveChanges();
                    context.Morada.Remove(morada);
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

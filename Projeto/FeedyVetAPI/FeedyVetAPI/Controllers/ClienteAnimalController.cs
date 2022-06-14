using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using FeedyVetAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace FeedyVetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteAnimalController : ControllerBase
    {
        private readonly IConfiguration _configuration;  //Injeção privada para utilizar certos metodos

        public ClienteAnimalController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Get

        /// <summary>
        /// Retorna a informação de todas as ligações entre animais e clientes
        /// </summary>
        /// <returns> Lista de ligações entre animais e clientes </returns>
        [HttpGet, Authorize(Roles = "Admin")] 
        public IEnumerable<ClienteAnimal> Get()
        {
            using (var context = new FeedyVetContext())
            {
                return context.ClienteAnimal.ToList();
            }
        }

        #endregion

        #region Post

        /// <summary>
        /// Método que permite adicionar uma nova ligação entre animal e cliente na base de dados
        /// </summary>
        /// <param name="clienteanimal"></param>
        /// <returns> Nova ligação entre cliente e animal </returns>
        [HttpPost, Authorize(Roles = "Admin")]
        public IActionResult Post(ClienteAnimal clienteanimal)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    ClienteAnimal ca = new ClienteAnimal();

                    ca.IdAnimalNavigation = context.Animal.Where(a => a.IdAnimal == clienteanimal.IdAnimal).FirstOrDefault();
                    ca.IdClienteNavigation = context.Cliente.Where(a => a.IdCliente == clienteanimal.IdCliente).FirstOrDefault();

                    context.ClienteAnimal.Add(ca);
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

        #region Update

        /// <summary>
        /// Método que permite atualizar a informação de uma ligação entre animal e cliente na base de dados
        /// </summary>
        /// <param name="clienteanimal"></param>
        /// <returns> Ligação entre cliente e animal atualizada </returns>
        [HttpPatch, Authorize(Roles = "Admin")]
        public IActionResult Patch(ClienteAnimal clienteanimal)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    ClienteAnimal ca = context.ClienteAnimal.Where(c => c.IdAnimal == clienteanimal.IdAnimal
                    && c.IdCliente == clienteanimal.IdCliente).FirstOrDefault();

                    if (clienteanimal.IdAnimal != 0) ca.IdAnimalNavigation = context.Animal.Where(a => a.IdAnimal == clienteanimal.IdAnimal).FirstOrDefault();
                    if (clienteanimal.IdCliente != 0) ca.IdClienteNavigation = context.Cliente.Where(a => a.IdCliente == clienteanimal.IdCliente).FirstOrDefault();

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
        /// Método que permite remover uma ligação da base de dados
        /// </summary>
        /// <param name="id_ani"></param>
        /// <param name="id_cli"></param>
        /// <returns></returns>
        [HttpDelete, Authorize(Roles = "Admin")]
        public IActionResult Delete(ClienteAnimal clienteAnimal)
        {
            try
            {
                using (var context = new FeedyVetContext())
                {
                    context.ClienteAnimal.Remove(context.ClienteAnimal
                        .Where(ca => ca.IdAnimal == clienteAnimal.IdAnimal
                        && ca.IdCliente == clienteAnimal.IdCliente).FirstOrDefault());
                }

                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        #endregion
    }
}

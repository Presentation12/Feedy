using FeedyVetAPI.Models;
using FeedyVetAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FeedyVet.Test
{
    public class AnimaisTest
    {
        [Theory]
        [InlineData(1, "Mensal")]
        [InlineData(1, "Anual")]
        public static void ShouldAddAnimal(int idcliente, string tipoconta)
        {
            Cliente c = new Cliente { IdCliente = idcliente, TipoConta = tipoconta };

            Assert.True(AnimalController.VerificaAddAnimalPlano(c));
        }

        [Theory]
        [InlineData(1, "Livre")]
        [InlineData(-1, "Mensal")]
        public static void ShouldNotAddAnimal(int idcliente, string tipoconta)
        {
            Cliente c = new Cliente { IdCliente = idcliente, TipoConta = tipoconta };

            Assert.Throws<ArgumentException>("cliente", () => AnimalController.VerificaAddAnimalPlano(c));
        }    
    }
}

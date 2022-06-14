using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeedyVetAPI.Controllers;
using FeedyVetAPI.Models;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;

namespace FeedyVet.Test
{
    public class EncomendaTest
    {
        public EncomendaTest()
        {  }

        [Fact]
        public void ShouldCancelDelivery()
        {
            Encomenda e = new Encomenda { Estado = "Solicitar Stock" };

            Assert.NotNull(EncomendaController.VerifyCancelaEncomenda(e));
            Assert.Equal("Cancelado", e.Estado);
        }

        [Theory]
        [InlineData("Cancelado")]
        [InlineData("Prestes a receber")]
        public void ShouldNotCancelDelivery(string estado)
        {
            Encomenda e = new Encomenda { Estado = estado };

            Assert.Throws<ArgumentException>("encomenda", () => EncomendaController.VerifyCancelaEncomenda(e));
        }
    }
}

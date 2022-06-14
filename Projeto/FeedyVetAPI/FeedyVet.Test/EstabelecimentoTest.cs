using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeedyVetAPI.Controllers;
using Xunit;

namespace FeedyVet.Test
{
    public class EstabelecimentoTest
    {
        [Fact]
        public void ShouldInativarClinica()
        {
            Assert.True(EstabelecimentoController.VerificaPodeDesativarEstabelecimento(2));
        }

        [Theory(Skip="Bug Throw de Exception")]
        [InlineData(1)]
        public void ShouldNotInativarClinica(int id_estabelecimento)
        {
            Assert.False(EstabelecimentoController.VerificaPodeDesativarEstabelecimento(id_estabelecimento));
            Assert.Throws<ArgumentException>("id_estabelecimento", () => EstabelecimentoController.VerificaPodeDesativarEstabelecimento(id_estabelecimento));
        }
    }
}

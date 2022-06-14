using FeedyVetAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedyVetAPI.Viewmodels
{
    public class AvaliacaoEstabelecimentotilizadorViewmodel
    {
        public AvaliacaoEstabelecimentoUtilizador AvaliacaoEstabelecimentoUtilizador { get; set; }
        public List<Cliente> Cliente { get; set; }
        public List<Estabelecimento> Estabelecimento { get; set; }
    }
}

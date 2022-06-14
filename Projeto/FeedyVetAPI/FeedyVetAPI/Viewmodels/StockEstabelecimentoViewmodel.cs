using FeedyVetAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedyVetAPI.Viewmodels
{
    public class StockEstabelecimentoViewmodel
    {
        public StockEstabelecimento StockEstabelecimento { get; set; }
        public List<Estabelecimento> Estabelecimento { get; set; }
    }
}

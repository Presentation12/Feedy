using FeedyVetAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedyVetAPI.Viewmodels
{
    public class EncomendaStockViewmodel
    {
        public EncomendaStock EncomendaStock { get; set; }
        public List<Encomenda> Encomenda { get; set; }
        public List<StockEstabelecimentoViewmodel> StockEstabelecimento { get; set; }
    }
}

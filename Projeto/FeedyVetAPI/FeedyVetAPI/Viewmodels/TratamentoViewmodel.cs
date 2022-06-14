using FeedyVetAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedyVetAPI.Viewmodels
{
    public class TratamentoViewmodel
    {
        public Tratamento Tratamento { get; set; }
        public List<Prescricao> Prescricao { get; set; }
        public List<StockEstabelecimentoViewmodel> StockEstabelecimento { get; set; }
    }
}

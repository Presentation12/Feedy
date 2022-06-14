using FeedyVetAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedyVetAPI.Viewmodels
{
    public class EstabelecimentoGerenteViewmodel
    {
        public List<Estabelecimento> Estabelecimento { get; set; }
        public List<Funcionario> Funcionario { get; set; }
    }
}

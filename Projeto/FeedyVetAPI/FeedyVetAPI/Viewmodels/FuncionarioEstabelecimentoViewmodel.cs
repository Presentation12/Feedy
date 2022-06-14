using FeedyVetAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedyVetAPI.Viewmodels
{
    public class FuncionarioEstabelecimentoViewmodel
    {
        public FuncionarioEstabelecimento FuncionarioEstabelecimento { get; set; }
        public List<Estabelecimento> Estabelecimento { get; set; }
        public List<Funcionario> Funcionario { get; set; }
    }
}

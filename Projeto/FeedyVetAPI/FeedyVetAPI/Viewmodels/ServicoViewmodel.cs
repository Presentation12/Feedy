using FeedyVetAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedyVetAPI.Viewmodels
{
    public class ServicoViewmodel
    {
        public Servico Servico { get; set; }
        public List<Animal> Animal { get; set; }
        public List<ServicoPrescricao> ServicoPrescricao { get; set; }
        public List<Funcionario> Funcionario { get; set; }
        public List<ServicoCatalogo> ServicoCatalogo { get; set; }
    }
}

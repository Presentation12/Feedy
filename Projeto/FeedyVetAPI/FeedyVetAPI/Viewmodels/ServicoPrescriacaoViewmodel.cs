using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeedyVetAPI.Models;

namespace FeedyVetAPI.Viewmodels
{
    public class ServicoPrescriacaoViewmodel
    {
        public ServicoPrescricao ServicoPrescricao { get; set; }
        public List<Servico> Servico { get; set; }
        public List<Prescricao> Prescricao { get; set; }
    }
}

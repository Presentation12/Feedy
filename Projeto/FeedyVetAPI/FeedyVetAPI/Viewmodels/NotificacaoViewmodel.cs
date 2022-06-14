using FeedyVetAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedyVetAPI.Viewmodels
{
    public class NotificacaoViewmodel
    {
        public Notificacao Notificacao { get; set; }
        public List<Cliente> Cliente { get;set; }
    }
}

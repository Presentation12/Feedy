using System;
using System.Collections.Generic;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa a notificação e contém todos os seus atributos
    /// </summary>
    public partial class Notificacao
    {
        public int IdNotificacao { get; set; }
        public int IdCliente { get; set; }
        public string Estado { get; set; }
        public string Descricao { get; set; }
        public DateTime DataNotificacao { get; set; }

        public virtual Cliente IdClienteNavigation { get; set; }
    }
}

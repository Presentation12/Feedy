using System;
using System.Collections.Generic;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa a encomenda e contém todos os seus atributos
    /// </summary>
    public partial class Encomenda
    {
        public Encomenda()
        {
            EncomendaStock = new HashSet<EncomendaStock>();
        }

        public int IdEncomenda { get; set; }
        public int IdCliente { get; set; }
        public int IdMorada { get; set; }
        public string Estado { get; set; }
        public DateTime Data { get; set; }
        public string MetodoPagamento { get; set; }

        public virtual Cliente IdClienteNavigation { get; set; }
        public virtual Morada IdMoradaNavigation { get; set; }
        public virtual ICollection<EncomendaStock> EncomendaStock { get; set; }
    }
}

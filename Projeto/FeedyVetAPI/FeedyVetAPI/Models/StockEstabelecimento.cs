using System;
using System.Collections.Generic;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa o stock/produto de um estabelecimento e contém todos os seus atributos
    /// </summary>
    public partial class StockEstabelecimento
    {
        public StockEstabelecimento()
        {
            EncomendaStock = new HashSet<EncomendaStock>();
            Tratamento = new HashSet<Tratamento>();
        }

        public int IdStock { get; set; }
        public int IdEstabelecimento { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int? Volume { get; set; }
        public int Stock { get; set; }
        public decimal Preco { get; set; }
        public string TipoStock { get; set; }

        public virtual Estabelecimento IdEstabelecimentoNavigation { get; set; }
        public virtual ICollection<EncomendaStock> EncomendaStock { get; set; }
        public virtual ICollection<Tratamento> Tratamento { get; set; }
    }
}

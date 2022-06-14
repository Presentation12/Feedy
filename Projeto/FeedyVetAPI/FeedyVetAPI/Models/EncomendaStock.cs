using System;
using System.Collections.Generic;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa a relação entidade encomenda <-> stockEstabelecimento e contém todos os seus atributos
    /// </summary>
    public partial class EncomendaStock
    {
        public int IdEncomenda { get; set; }
        public int IdStock { get; set; }
        public int Qtd { get; set; }

        public virtual Encomenda IdEncomendaNavigation { get; set; }
        public virtual StockEstabelecimento IdStockNavigation { get; set; }
    }
}

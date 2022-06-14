using System;
using System.Collections.Generic;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa a relacao entidade Prescricao <-> Stock e contém todos os seus atributos
    /// </summary>
    public partial class Tratamento
    {
        public int IdPrescricao { get; set; }
        public int IdStock { get; set; }
        public int Quantidade { get; set; }

        public virtual Prescricao IdPrescricaoNavigation { get; set; }
        public virtual StockEstabelecimento IdStockNavigation { get; set; }
    }
}

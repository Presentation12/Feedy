using System;
using System.Collections.Generic;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa a avaliação de um utilizador referente a um estabelecimento e contém todos os seus atributos
    /// </summary>
    public partial class AvaliacaoEstabelecimentoUtilizador
    {
        public int IdAvaliacaoEstabelecimentoUtilizador { get; set; }
        public int IdEstabelecimento { get; set; }
        public int IdCliente { get; set; }
        public double Avaliacao { get; set; }
        public string Texto { get; set; }
        public DateTime DataAvaliacao { get; set; }

        public virtual Cliente IdClienteNavigation { get; set; }
        public virtual Estabelecimento IdEstabelecimentoNavigation { get; set; }
    }
}

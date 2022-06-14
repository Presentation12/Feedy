using System;
using System.Collections.Generic;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa a relação entidade Servico <-> Estabelecimento e contém todos os seus atributos
    /// </summary>
    public partial class ServicoCatalogo
    {
        public ServicoCatalogo()
        {
            Servico = new HashSet<Servico>();
        }

        public int IdServicoCatalogo { get; set; }
        public int IdEstabelecimento { get; set; }
        public string Estado { get; set; }
        public string Tipo { get; set; }
        public double Preco { get; set; }
        public string Descricao { get; set; }
        public string CatalogoFoto { get; set; }

        public TimeSpan Duracao { get; set; }

        public virtual Estabelecimento IdEstabelecimentoNavigation { get; set; }
        public virtual ICollection<Servico> Servico { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa o estabelecimento e contém todos os seus atributos
    /// </summary>
    public partial class Estabelecimento
    {
        public Estabelecimento()
        {
            AvaliacaoEstabelecimentoUtilizador = new HashSet<AvaliacaoEstabelecimentoUtilizador>();
            EstabelecimentoHorario = new HashSet<EstabelecimentoHorario>();
            Servico = new HashSet<Servico>();
            ServicoCatalogo = new HashSet<ServicoCatalogo>();
            StockEstabelecimento = new HashSet<StockEstabelecimento>();
            FuncionarioEstabelecimento = new HashSet<FuncionarioEstabelecimento>();
        }

        public int IdEstabelecimento { get; set; }
        public string Nome { get; set; }

        public string TipoEstabelecimento { get; set; }
        public string Estado { get; set; }
        public int IdMorada { get; set; }
        public int Contacto { get; set; }

        public float AvaliacaoMedia { get; set; }
        public string EstabelecimentoFoto { get; set; }

        public virtual Morada IdMoradaNavigation { get; set; }
        public virtual ICollection<AvaliacaoEstabelecimentoUtilizador> AvaliacaoEstabelecimentoUtilizador { get; set; }
        public virtual ICollection<EstabelecimentoHorario> EstabelecimentoHorario { get; set; }
        public virtual ICollection<Servico> Servico { get; set; }
        public virtual ICollection<ServicoCatalogo> ServicoCatalogo { get; set; }
        public virtual ICollection<StockEstabelecimento> StockEstabelecimento { get; set; }
        public virtual ICollection<FuncionarioEstabelecimento> FuncionarioEstabelecimento { get; set; }
        public virtual ICollection<EstabelecimentoGerente> ClinicaGerente { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa o cliente e contém todos os seus atributos
    /// </summary>
    public partial class Cliente
    {
        public Cliente()
        {
            AvaliacaoEstabelecimentoUtilizador = new HashSet<AvaliacaoEstabelecimentoUtilizador>();
            ClienteAnimal = new HashSet<ClienteAnimal>();
            Encomenda = new HashSet<Encomenda>();
            Notificacao = new HashSet<Notificacao>();
            Servico = new HashSet<Servico>();
        }

        public int IdCliente { get; set; }
        public string Nome { get; set; }
        public string Apelido { get; set; }
        public int? IdMorada { get; set; }
        public string Email { get; set; }
        public int? Telemovel { get; set; }

        public string Pass { get; set; }
        public string PassSalt { get; set; }
        public string TipoConta { get; set; }
        public double? ValorConta { get; set; }
        public string ClienteFoto { get; set; }
        public string Estado { get; set; }
        public int? Pontos { get; set; }

        public virtual Morada IdMoradaNavigation { get; set; }
        public virtual ICollection<AvaliacaoEstabelecimentoUtilizador> AvaliacaoEstabelecimentoUtilizador { get; set; }
        public virtual ICollection<ClienteAnimal> ClienteAnimal { get; set; }
        public virtual ICollection<Encomenda> Encomenda { get; set; }
        public virtual ICollection<Notificacao> Notificacao { get; set; }
        public virtual ICollection<Servico> Servico { get; set; }

    }
}

using System;
using System.Collections.Generic;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa a morada de um cliente/clinica e contém todos os seus atributos
    /// </summary>
    public partial class Morada
    {
        public Morada()
        {
            Cliente = new HashSet<Cliente>();
            Estabelecimento = new HashSet<Estabelecimento>();
            Encomenda = new HashSet<Encomenda>();
        }

        public int IdMorada { get; set; }
        public string Rua { get; set; }
        public int Porta { get; set; }
        public int? Andar { get; set; }
        public string CodigoPostal { get; set; }
        public string Freguesia { get; set; }
        public string Distrito { get; set; }
        public string Concelho { get; set; }
        public string Pais { get; set; }

        public virtual ICollection<Cliente> Cliente { get; set; }
        public virtual ICollection<Estabelecimento> Estabelecimento { get; set; }
        public virtual ICollection<Encomenda> Encomenda { get; set; }
    }
}

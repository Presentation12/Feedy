using System;
using System.Collections.Generic;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa o serviço (consulta, vacina, ...) e contém todos os seus atributos
    /// </summary>
    public partial class Servico
    {
        public Servico()
        {
            ServicoPrescricao = new HashSet<ServicoPrescricao>();
        }

        public int IdServico { get; set; }
        public int IdCliente { get; set; }
        public int IdAnimal { get; set; }
        public int IdFuncionario { get; set; }
        public int IdEstabelecimento { get; set; }
        public int IdServicoCatalogo { get; set; }
        public DateTime DataServico { get; set; }
        public string Descricao { get; set; }
        public string Estado { get; set; }
        public string MetodoPagamento { get; set; }

        public virtual Animal IdAnimalNavigation { get; set; }
        public virtual Cliente IdClienteNavigation { get; set; }
        public virtual Estabelecimento IdEstabelecimentoNavigation { get; set; }
        public virtual ServicoCatalogo IdServicoCatalogoNavigation { get; set; }
        public virtual Funcionario IdFuncionarioNavigation { get; set; }

        public virtual ICollection<ServicoPrescricao> ServicoPrescricao { get; set; }
    }
}

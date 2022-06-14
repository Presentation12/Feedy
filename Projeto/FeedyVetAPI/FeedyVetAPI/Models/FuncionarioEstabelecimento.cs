using System;
using System.Collections.Generic;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa a relação entidade Funcionario <-> Estabelecimento e contém todos os seus atributos
    /// </summary>
    public partial class FuncionarioEstabelecimento
    {
        public int IdFuncionario { get; set; }
        public int IdEstabelecimento { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }

        public virtual Estabelecimento IdEstabelecimentoNavigation { get; set; }
        public virtual Funcionario IdFuncionarioNavigation { get; set; }
    }
}

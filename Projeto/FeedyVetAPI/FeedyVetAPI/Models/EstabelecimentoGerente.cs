using System;
using System.Collections.Generic;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa a relação entidade Estabelcimento <-> Funcionario e contém todos os seus atributos
    /// </summary>
    public partial class EstabelecimentoGerente
    {
        public int IdEstabelecimento { get; set; }
        public int IdFuncionario { get; set; }

        public virtual Estabelecimento IdEstabelecimentoNavigation { get; set; }
        public virtual Funcionario IdFuncionarioNavigation { get; set; }
    }
}

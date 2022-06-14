using System;
using System.Collections.Generic;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa a relação entidade Servico <-> Prescricao e contém todos os seus atributos
    /// </summary>
    public partial class ServicoPrescricao
    {
        public int IdServico { get; set; }
        public int IdPrescricao { get; set; }

        public virtual Prescricao IdPrescricaoNavigation { get; set; }
        public virtual Servico IdServicoNavigation { get; set; }
    }
}

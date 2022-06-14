using System;
using System.Collections.Generic;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa o horario de um estabelecimento e contém todos os seus atributos
    /// </summary>
    public partial class EstabelecimentoHorario
    {
        public int IdEstabelecimentoHorario { get; set; }
        public int IdEstabelecimento { get; set; }
        public string HorarioAbertura { get; set; }
        public string HorarioEncerramento { get; set; }
        public string DiaSemana { get; set; }

        public virtual Estabelecimento IdEstabelecimentoNavigation { get; set; }
    }
}

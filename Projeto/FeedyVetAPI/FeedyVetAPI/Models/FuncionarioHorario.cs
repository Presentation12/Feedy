using System;
using System.Collections.Generic;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa o horario de um funcionario e contém todos os seus atributos
    /// </summary>
    public partial class FuncionarioHorario
    {
        public int IdFuncionarioHorario { get; set; }
        public int IdFuncionario { get; set; }
        public TimeSpan HoraEntrada { get; set; }
        public TimeSpan HoraSaida { get; set; }
        public string DiaSemana { get; set; }

        public virtual Funcionario IdFuncionarioNavigation { get; set; }
    }
}

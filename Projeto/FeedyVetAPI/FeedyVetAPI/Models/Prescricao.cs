using System;
using System.Collections.Generic;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa a prescrição passada numa consulta e contém todos os seus atributos
    /// </summary>
    public partial class Prescricao
    {
        public Prescricao()
        {
            ServicoPrescricao = new HashSet<ServicoPrescricao>();
            Tratamento = new HashSet<Tratamento>();
        }

        public int IdPrescricao { get; set; }
        public string Estado { get; set; }
        public string Descricao { get; set; }
        public string DataExpiracao { get; set; }

        public virtual ICollection<ServicoPrescricao> ServicoPrescricao { get; set; }
        public virtual ICollection<Tratamento> Tratamento { get; set; }
    }
}

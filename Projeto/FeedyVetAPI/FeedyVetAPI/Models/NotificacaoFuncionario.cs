using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa a notificação do funcionário e contém todos os seus atributos
    /// </summary>
    public partial class NotificacaoFuncionario
    {
        [Key]
        public int IdNotificacao { get; set; }

        [ForeignKey("Funcionario")]
        public int IdFuncionario { get; set; }
        public string Estado { get; set; }
        public string Descricao { get; set; }
        public DateTime DataNotificacao { get; set; }
    }
}

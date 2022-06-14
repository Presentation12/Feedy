using System;
using System.Collections.Generic;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa o funcionario e contém todos os seus atributos
    /// </summary>
    public partial class Funcionario
    {
        public Funcionario()
        {
            Servico = new HashSet<Servico>();
            FuncionarioEstabelecimento = new HashSet<FuncionarioEstabelecimento>();
            FuncionarioHorario = new HashSet<FuncionarioHorario>();
        }

        public int IdFuncionario { get; set; }
        public string Nome { get; set; }
        public string Apelido { get; set; }
        public string Estado { get; set; }
        public string Especialidade { get; set; }
        public string Email { get; set; }
        public int? Telemovel { get; set; }
        public string Pass { get; set; }

        public string PassSalt { get; set; }
        public int Codigo { get; set; }
        public string FuncionarioFoto { get; set; }

        public virtual ICollection<Servico> Servico { get; set; }
        public virtual ICollection<FuncionarioEstabelecimento> FuncionarioEstabelecimento { get; set; }
        public virtual ICollection<FuncionarioHorario> FuncionarioHorario { get; set; }
        public virtual ICollection<EstabelecimentoGerente> ClinicaGerente { get; set; }
    }
}

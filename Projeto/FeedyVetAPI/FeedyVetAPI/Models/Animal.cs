using System;
using System.Collections.Generic;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa o animal e contém todos os seus atributos
    /// </summary>
    public partial class Animal
    {
        public Animal()
        {
          Lembrete = new HashSet<Lembrete>();
          Servico = new HashSet<Servico>();
        }

        public int IdAnimal { get; set; }
        public string Nome { get; set; }
        public double? Peso { get; set; }
        public double? Altura { get; set; }
        public string Classe { get; set; }
        public string Especie { get; set; }
        public string Genero { get; set; }
        public string Estado { get; set; }
        public string AnimalFoto { get; set; }
        public DateTime? DataNascimento { get; set; }

        public virtual ClienteAnimal ClienteAnimal { get; set; }
        public virtual ICollection<Lembrete> Lembrete { get; set; }
        public virtual ICollection<Servico> Servico { get; set; }
    }
}

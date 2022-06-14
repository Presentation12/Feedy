using System;
using System.Collections.Generic;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa o lembrete e contém todos os seus atributos
    /// </summary>
    public partial class Lembrete
    {
        public int IdLembrete { get; set; }
        public int IdAnimal { get; set; }
        public string LembreteDescricao { get; set; }
        public DateTime DataLembrete { get; set; }
        public TimeSpan HoraLembrete { get; set; }
        public string Frequencia { get; set; }

        public virtual Animal IdAnimalNavigation { get; set; }
    }
}

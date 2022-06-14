using System;
using System.Collections.Generic;

namespace FeedyVetAPI.Models
{
    /// <summary>
    /// Model que representa a relação entidade Animal <-> Cliente e contém todos os seus atributos
    /// </summary>
    public partial class ClienteAnimal
    {
        public int IdCliente { get; set; }
        public int IdAnimal { get; set; }

        public virtual Animal IdAnimalNavigation { get; set; }
        public virtual Cliente IdClienteNavigation { get; set; }
    }
}

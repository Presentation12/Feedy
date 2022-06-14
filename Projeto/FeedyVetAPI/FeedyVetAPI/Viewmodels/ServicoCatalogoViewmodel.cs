using FeedyVetAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedyVetAPI.Viewmodels
{
    public class ServicoCatalogoViewmodel
    {
        public ServicoCatalogo ServicoCatalogo { get; set; }
        public List<Estabelecimento> Estabelecimento { get; set; }
    }
}

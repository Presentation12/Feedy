using FeedyVetAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedyVetAPI.Viewmodels
{
    public class LembreteViewmodel
    {
        public Lembrete Lembrete { get; set; }
        public List<Animal> Animal { get; set; }
    }
}

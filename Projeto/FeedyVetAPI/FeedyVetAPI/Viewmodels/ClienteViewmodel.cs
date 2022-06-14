using FeedyVetAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeedyVetAPI.Viewmodels
{
    public class ClienteViewmodel
    {
        public Cliente Cliente { get; set; }
        public List<Morada> Morada { get; set; }
    }
}

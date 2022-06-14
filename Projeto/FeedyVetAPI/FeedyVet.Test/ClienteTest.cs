using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FeedyVetAPI.Controllers;
using FeedyVetAPI.Models;

namespace FeedyVet.Test
{
    public class ClienteTest
    {
        [Fact]
        public static void ShouldMakeLogin()
        {
            Cliente cliente = new Cliente() { Email = "duarte@gmail.com", Pass = "dm" };

            Assert.True(ClienteController.VerifyAccount(cliente));
        }

        [Theory]
        [InlineData("naoexiste@gmail.com", "dm")]
        [InlineData("duarte@gmail.com", "passincorreta")]
        public static void ShouldNotMakeLogin(string email, string pass)
        {
            Cliente cliente = new Cliente() { Email = email, Pass = pass };

            Assert.Throws<ArgumentException>("account", () => ClienteController.VerifyAccount(cliente));
        }
    }
}

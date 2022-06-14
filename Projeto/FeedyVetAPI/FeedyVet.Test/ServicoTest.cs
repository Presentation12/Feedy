using System;
using Xunit;
using FeedyVetAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using FeedyVetAPI.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace FeedyVet.Test
{
    public class ServicoTest
    {
        public FeedyVetContext context { get; set; }
        private readonly IConfiguration _configuration;
        public ServicoController servico;

        public ServicoTest()
        {
            var builder = new DbContextOptionsBuilder<FeedyVetContext>();
            builder.UseInMemoryDatabase(databaseName: "LibraryDbInMemory");

            var dbContextOptions = builder.Options;
            servico = new ServicoController(_configuration);
            context = new FeedyVetContext(dbContextOptions);

            // Delete existing db before creating a new one
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        [Theory(Skip = "Teste integrado (faz alterações na base de dados)")]
        [InlineData(1, 2, 3, 2, 1, 2028, 4, 4, 15, 0, 0, "Unit Test", "Unit Test")]
        public void ShouldMarcarServico(int idcliente, int idanimal, int idfuncionario, int idestabelecimento,
            int idservicocatalogo, int ano, int mes, int dia, int hora, int min, int sec, string descricao, string metodopagamento)
        {
            Servico serv = new Servico
            {
                IdAnimal = idanimal,
                IdFuncionario = idfuncionario,
                IdEstabelecimento = idestabelecimento,
                IdServicoCatalogo = idservicocatalogo,
                DataServico = new DateTime(ano, mes, dia, hora, min, sec),
                Descricao = descricao,
                MetodoPagamento = metodopagamento
            };
            string expected = "Marcado";

            Servico result = servico.MarcarServico(idcliente, serv);

            Assert.Equal(result.Estado, expected);
            //Arranjar forma de remover serviço marcado
        }

        [Theory]
        [InlineData(1, 1, 2022, 02, 02, 15, 30, 00)]
        [InlineData(3, 1, 2022, 03, 02, 15, 30, 00)]
        public void ShouldBeDisponibilizadoServico(int idfuncionario, int idservicocatalogo, 
            int ano, int mes, int dia, int hora, int min, int seg)
        {
            Funcionario funcionario = new Funcionario() { IdFuncionario = idfuncionario };
            DateTime data = new DateTime(ano, mes, dia, hora, min, seg);

            Assert.False(ServicoController.VerificaDisponibilidade(data, funcionario, idservicocatalogo));
        }

        [Theory]
        [InlineData(1, 1, 2023, 03, 04, 15, 30, 00)]
        public void ShouldBeIndisponvelServico(int idfuncionario, int idservicocatalogo,
            int ano, int mes, int dia, int hora, int min, int seg)
        {
            Funcionario funcionario = new Funcionario() { IdFuncionario = idfuncionario };
            DateTime data = new DateTime(ano, mes, dia, hora, min, seg);

            Assert.True(ServicoController.VerificaDisponibilidade(data, funcionario, idservicocatalogo));
        }

        [Theory]
        [InlineData(-1, 1, 2022, 03, 02, 15, 30, 00)]
        public void ShouldBeFuncionarioInexistente(int idfuncionario, int idservicocatalogo,
            int ano, int mes, int dia, int hora, int min, int seg)
        {
            Funcionario funcionario = new Funcionario() { IdFuncionario = idfuncionario };
            DateTime data = new DateTime(ano, mes, dia, hora, min, seg);

            Assert.False(ServicoController.VerificaDisponibilidade(data, funcionario, idservicocatalogo));
        }

        [Theory]
        [InlineData(-2, 1, 1, 1, 2030, 3, 4, 15, 0, 0, "Unit Test", "Unit Test")]
        [InlineData(2, -1, 1, 1, 2030, 3, 4, 15, 0, 0, "Unit Test", "Unit Test")]
        [InlineData(2, 1, -1, 1, 2030, 3, 4, 15, 0, 0, "Unit Test", "Unit Test")]
        [InlineData(2, 1, 1, -1, 2030, 3, 4, 15, 0, 0, "Unit Test", "Unit Test")]
        public void ShouldntMarcarServico(int idanimal, int idfuncionario, int idestabelecimento,
            int idservicocatalogo, int ano, int mes, int dia, int hora, int min, int sec, string descricao, string metodopagamento)
        {
            Servico serv = new Servico
            {
                IdAnimal = idanimal,
                IdFuncionario = idfuncionario,
                IdEstabelecimento = idestabelecimento,
                IdServicoCatalogo = idservicocatalogo,
                DataServico = new DateTime(ano, mes, dia, hora, min, sec),
                Descricao = descricao,
                MetodoPagamento = metodopagamento
            };

            Assert.Null(servico.MarcarServico(1, serv));
        }
    }
}

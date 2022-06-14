using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FeedyVetAPI.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Animal",
                columns: table => new
                {
                    id_animal = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    peso = table.Column<double>(nullable: true),
                    altura = table.Column<double>(nullable: true),
                    classe = table.Column<string>(unicode: false, maxLength: 30, nullable: true),
                    especie = table.Column<string>(unicode: false, maxLength: 30, nullable: true),
                    genero = table.Column<string>(unicode: false, maxLength: 30, nullable: true),
                    estado = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    animal_foto = table.Column<string>(unicode: false, maxLength: 500, nullable: true),
                    data_nascimento = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_id_animal", x => x.id_animal);
                });

            migrationBuilder.CreateTable(
                name: "Funcionario",
                columns: table => new
                {
                    id_funcionario = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    apelido = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    estado = table.Column<string>(unicode: false, maxLength: 15, nullable: false),
                    especialidade = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    email = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    telemovel = table.Column<int>(nullable: true),
                    pass = table.Column<string>(unicode: false, maxLength: 5000, nullable: false),
                    PassSalt = table.Column<string>(nullable: true),
                    codigo = table.Column<int>(nullable: false),
                    funcionario_foto = table.Column<string>(unicode: false, maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_id_funcionario", x => x.id_funcionario);
                });

            migrationBuilder.CreateTable(
                name: "Morada",
                columns: table => new
                {
                    id_morada = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rua = table.Column<string>(unicode: false, maxLength: 60, nullable: false),
                    porta = table.Column<int>(nullable: false),
                    andar = table.Column<int>(nullable: true),
                    codigo_postal = table.Column<string>(unicode: false, maxLength: 10, nullable: false),
                    freguesia = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    distrito = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    concelho = table.Column<string>(unicode: false, maxLength: 40, nullable: false),
                    pais = table.Column<string>(unicode: false, maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_id_morada", x => x.id_morada);
                });

            migrationBuilder.CreateTable(
                name: "Prescricao",
                columns: table => new
                {
                    id_prescricao = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    estado = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    descricao = table.Column<string>(unicode: false, maxLength: 200, nullable: false),
                    data_expiracao = table.Column<string>(unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_id_prescricao", x => x.id_prescricao);
                });

            migrationBuilder.CreateTable(
                name: "Lembrete",
                columns: table => new
                {
                    id_lembrete = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_animal = table.Column<int>(nullable: false),
                    lembrete_descricao = table.Column<string>(unicode: false, maxLength: 500, nullable: true),
                    data_lembrete = table.Column<DateTime>(type: "datetime", nullable: false),
                    hora_lembrete = table.Column<TimeSpan>(nullable: false),
                    frequencia = table.Column<string>(unicode: false, maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_id_lembrete", x => x.id_lembrete);
                    table.ForeignKey(
                        name: "fk_id_animal_lembrete",
                        column: x => x.id_animal,
                        principalTable: "Animal",
                        principalColumn: "id_animal",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Funcionario_Horario",
                columns: table => new
                {
                    id_funcionario_horario = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_funcionario = table.Column<int>(nullable: false),
                    hora_entrada = table.Column<TimeSpan>(nullable: false),
                    hora_saida = table.Column<TimeSpan>(nullable: false),
                    dia_semana = table.Column<string>(unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_funcionario_horario", x => x.id_funcionario_horario);
                    table.ForeignKey(
                        name: "fk_funcionario_horario_id_funcionario",
                        column: x => x.id_funcionario,
                        principalTable: "Funcionario",
                        principalColumn: "id_funcionario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    id_cliente = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    apelido = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    id_morada = table.Column<int>(nullable: true),
                    email = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    telemovel = table.Column<int>(nullable: true),
                    pass = table.Column<string>(unicode: false, maxLength: 5000, nullable: false),
                    PassSalt = table.Column<string>(nullable: true),
                    tipo_conta = table.Column<string>(unicode: false, maxLength: 10, nullable: false),
                    valor_conta = table.Column<double>(nullable: true),
                    cliente_foto = table.Column<string>(unicode: false, maxLength: 500, nullable: true),
                    estado = table.Column<string>(unicode: false, maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_id_cliente", x => x.id_cliente);
                    table.ForeignKey(
                        name: "fk_id_morada",
                        column: x => x.id_morada,
                        principalTable: "Morada",
                        principalColumn: "id_morada",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Estabelecimento",
                columns: table => new
                {
                    id_estabelecimento = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(unicode: false, maxLength: 60, nullable: false),
                    TipoEstabelecimento = table.Column<string>(nullable: true),
                    estado = table.Column<string>(unicode: false, maxLength: 15, nullable: false),
                    id_morada = table.Column<int>(nullable: false),
                    contacto = table.Column<int>(nullable: false),
                    AvaliacaoMedia = table.Column<float>(nullable: false),
                    estabelecimento_foto = table.Column<string>(unicode: false, maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_id_estabelecimento", x => x.id_estabelecimento);
                    table.ForeignKey(
                        name: "fk_id_morada_cliente",
                        column: x => x.id_morada,
                        principalTable: "Morada",
                        principalColumn: "id_morada",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cliente_Animal",
                columns: table => new
                {
                    id_animal = table.Column<int>(nullable: false),
                    id_cliente = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cliente_animal", x => x.id_animal);
                    table.ForeignKey(
                        name: "fk_cliente_animal_id_animal",
                        column: x => x.id_animal,
                        principalTable: "Animal",
                        principalColumn: "id_animal",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cliente_animal_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "Cliente",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Encomenda",
                columns: table => new
                {
                    id_encomenda = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_cliente = table.Column<int>(nullable: false),
                    id_morada = table.Column<int>(nullable: false),
                    estado = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    Data = table.Column<DateTime>(nullable: false),
                    metodo_pagamento = table.Column<string>(unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_encomenda", x => x.id_encomenda);
                    table.ForeignKey(
                        name: "fk_encomenda_cliente",
                        column: x => x.id_cliente,
                        principalTable: "Cliente",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_encomenda_morada",
                        column: x => x.id_morada,
                        principalTable: "Morada",
                        principalColumn: "id_morada",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notificacao",
                columns: table => new
                {
                    id_notificacao = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_cliente = table.Column<int>(nullable: false),
                    estado = table.Column<string>(unicode: false, maxLength: 15, nullable: false),
                    descricao = table.Column<string>(unicode: false, maxLength: 150, nullable: false),
                    data_notificacao = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_id_notificacao", x => x.id_notificacao);
                    table.ForeignKey(
                        name: "fk_id_cliente_notificacao",
                        column: x => x.id_cliente,
                        principalTable: "Cliente",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Avaliacao_Estabelecimento_Utilizador",
                columns: table => new
                {
                    id_avaliacao_estabelecimento_utilizador = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_estabelecimento = table.Column<int>(nullable: false),
                    id_cliente = table.Column<int>(nullable: false),
                    avaliacao = table.Column<double>(nullable: false),
                    texto = table.Column<string>(unicode: false, maxLength: 250, nullable: false),
                    data_avaliacao = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_id_avaliacao_estabelecimento_utilizador", x => x.id_avaliacao_estabelecimento_utilizador);
                    table.ForeignKey(
                        name: "fk_id_cliente_avaliacao",
                        column: x => x.id_cliente,
                        principalTable: "Cliente",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_id_estabelecimento_avaliacao",
                        column: x => x.id_estabelecimento,
                        principalTable: "Estabelecimento",
                        principalColumn: "id_estabelecimento",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Estabelecimento_Gerente",
                columns: table => new
                {
                    id_estabelecimento = table.Column<int>(nullable: false),
                    id_funcionario = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_estabelecimento_gerente", x => new { x.id_estabelecimento, x.id_funcionario });
                    table.ForeignKey(
                        name: "FK_Estabelecimento_Gerente_Estabelecimento_id_estabelecimento",
                        column: x => x.id_estabelecimento,
                        principalTable: "Estabelecimento",
                        principalColumn: "id_estabelecimento",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Estabelecimento_Gerente_Funcionario_id_funcionario",
                        column: x => x.id_funcionario,
                        principalTable: "Funcionario",
                        principalColumn: "id_funcionario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Estabelecimento_Horario",
                columns: table => new
                {
                    id_estabelecimento_horario = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_estabelecimento = table.Column<int>(nullable: false),
                    horario_abertura = table.Column<string>(unicode: false, maxLength: 10, nullable: false),
                    horario_encerramento = table.Column<string>(unicode: false, maxLength: 10, nullable: false),
                    dia_semana = table.Column<string>(unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_id_estabelecimento_horario", x => x.id_estabelecimento_horario);
                    table.ForeignKey(
                        name: "fk_id_clincia_horario",
                        column: x => x.id_estabelecimento,
                        principalTable: "Estabelecimento",
                        principalColumn: "id_estabelecimento",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Funcionario_Estabelecimento",
                columns: table => new
                {
                    id_funcionario = table.Column<int>(nullable: false),
                    id_estabelecimento = table.Column<int>(nullable: false),
                    data_inicio = table.Column<DateTime>(type: "date", nullable: true),
                    data_fim = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_funcionario_estabelecimento", x => new { x.id_funcionario, x.id_estabelecimento });
                    table.ForeignKey(
                        name: "fk_funcionario_estabelecimento_id_estabelecimento",
                        column: x => x.id_estabelecimento,
                        principalTable: "Estabelecimento",
                        principalColumn: "id_estabelecimento",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_funcionario_estabelecimento_id_funcionario",
                        column: x => x.id_funcionario,
                        principalTable: "Funcionario",
                        principalColumn: "id_funcionario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Servico_Catalogo",
                columns: table => new
                {
                    id_servico_catalogo = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_estabelecimento = table.Column<int>(nullable: false),
                    estado = table.Column<string>(unicode: false, maxLength: 15, nullable: false),
                    tipo = table.Column<string>(unicode: false, maxLength: 25, nullable: false),
                    preco = table.Column<double>(nullable: false),
                    descricao = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    catalogo_foto = table.Column<string>(unicode: false, maxLength: 500, nullable: true),
                    Duracao = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_id_servico_catalogo", x => x.id_servico_catalogo);
                    table.ForeignKey(
                        name: "fk_id_estabelecimento_catalogo",
                        column: x => x.id_estabelecimento,
                        principalTable: "Estabelecimento",
                        principalColumn: "id_estabelecimento",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Stock_Estabelecimento",
                columns: table => new
                {
                    id_stock = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_estabelecimento = table.Column<int>(nullable: false),
                    nome = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    descricao = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    volume = table.Column<int>(nullable: true),
                    stock = table.Column<int>(nullable: false),
                    preco = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    tipo_stock = table.Column<string>(unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stock_estabelecimento", x => x.id_stock);
                    table.ForeignKey(
                        name: "fk_stock_estabelecimento_id_estabelecimento",
                        column: x => x.id_estabelecimento,
                        principalTable: "Estabelecimento",
                        principalColumn: "id_estabelecimento",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Servico",
                columns: table => new
                {
                    id_servico = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_cliente = table.Column<int>(nullable: false),
                    id_animal = table.Column<int>(nullable: false),
                    id_funcionario = table.Column<int>(nullable: false),
                    id_estabelecimento = table.Column<int>(nullable: false),
                    id_servico_catalogo = table.Column<int>(nullable: false),
                    data_servico = table.Column<DateTime>(type: "datetime", nullable: false),
                    descricao = table.Column<string>(unicode: false, maxLength: 250, nullable: false),
                    estado = table.Column<string>(unicode: false, maxLength: 35, nullable: false),
                    MetodoPagamento = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_id_servico", x => x.id_servico);
                    table.ForeignKey(
                        name: "fk_id_animal_servico",
                        column: x => x.id_animal,
                        principalTable: "Animal",
                        principalColumn: "id_animal",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_id_cliente_servico",
                        column: x => x.id_cliente,
                        principalTable: "Cliente",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_id_estabelecimento_servico",
                        column: x => x.id_estabelecimento,
                        principalTable: "Estabelecimento",
                        principalColumn: "id_estabelecimento",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_id_funcionario_servico",
                        column: x => x.id_funcionario,
                        principalTable: "Funcionario",
                        principalColumn: "id_funcionario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_id_catalogo_servico",
                        column: x => x.id_servico_catalogo,
                        principalTable: "Servico_Catalogo",
                        principalColumn: "id_servico_catalogo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Encomenda_Stock",
                columns: table => new
                {
                    id_encomenda = table.Column<int>(nullable: false),
                    id_stock = table.Column<int>(nullable: false),
                    qtd = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_encomenda_stock", x => new { x.id_encomenda, x.id_stock });
                    table.ForeignKey(
                        name: "fk_encomenda_stock_id_encomenda",
                        column: x => x.id_encomenda,
                        principalTable: "Encomenda",
                        principalColumn: "id_encomenda",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_encomenda_stock_id_stock",
                        column: x => x.id_stock,
                        principalTable: "Stock_Estabelecimento",
                        principalColumn: "id_stock",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tratamento",
                columns: table => new
                {
                    id_prescricao = table.Column<int>(nullable: false),
                    id_stock = table.Column<int>(nullable: false),
                    quantidade = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tratamento", x => new { x.id_prescricao, x.id_stock });
                    table.ForeignKey(
                        name: "fk_tratamento_id_prescricao",
                        column: x => x.id_prescricao,
                        principalTable: "Prescricao",
                        principalColumn: "id_prescricao",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tratamento_id_stock",
                        column: x => x.id_stock,
                        principalTable: "Stock_Estabelecimento",
                        principalColumn: "id_stock",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Servico_Prescricao",
                columns: table => new
                {
                    id_servico = table.Column<int>(nullable: false),
                    id_prescricao = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_servico_prescricao", x => new { x.id_servico, x.id_prescricao });
                    table.ForeignKey(
                        name: "fk_servico_prescricao_id_prescricao",
                        column: x => x.id_prescricao,
                        principalTable: "Prescricao",
                        principalColumn: "id_prescricao",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_servico_prescricao_id_servico",
                        column: x => x.id_servico,
                        principalTable: "Servico",
                        principalColumn: "id_servico",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacao_Estabelecimento_Utilizador_id_cliente",
                table: "Avaliacao_Estabelecimento_Utilizador",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacao_Estabelecimento_Utilizador_id_estabelecimento",
                table: "Avaliacao_Estabelecimento_Utilizador",
                column: "id_estabelecimento");

            migrationBuilder.CreateIndex(
                name: "email_uk",
                table: "Cliente",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_id_morada",
                table: "Cliente",
                column: "id_morada");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Animal_id_cliente",
                table: "Cliente_Animal",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Encomenda_id_cliente",
                table: "Encomenda",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Encomenda_id_morada",
                table: "Encomenda",
                column: "id_morada");

            migrationBuilder.CreateIndex(
                name: "IX_Encomenda_Stock_id_stock",
                table: "Encomenda_Stock",
                column: "id_stock");

            migrationBuilder.CreateIndex(
                name: "contacto_uk",
                table: "Estabelecimento",
                column: "contacto",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estabelecimento_id_morada",
                table: "Estabelecimento",
                column: "id_morada");

            migrationBuilder.CreateIndex(
                name: "estabelecimento_idx",
                table: "Estabelecimento",
                columns: new[] { "nome", "id_morada" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estabelecimento_Gerente_id_funcionario",
                table: "Estabelecimento_Gerente",
                column: "id_funcionario");

            migrationBuilder.CreateIndex(
                name: "estabelecimento_horario_idx",
                table: "Estabelecimento_Horario",
                columns: new[] { "id_estabelecimento", "horario_abertura", "horario_encerramento", "dia_semana" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "codigo_uk",
                table: "Funcionario",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "email_vet_uk",
                table: "Funcionario",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "telemovel_uk",
                table: "Funcionario",
                column: "telemovel",
                unique: true,
                filter: "[telemovel] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Funcionario_Estabelecimento_id_estabelecimento",
                table: "Funcionario_Estabelecimento",
                column: "id_estabelecimento");

            migrationBuilder.CreateIndex(
                name: "IX_Funcionario_Horario_id_funcionario",
                table: "Funcionario_Horario",
                column: "id_funcionario");

            migrationBuilder.CreateIndex(
                name: "IX_Lembrete_id_animal",
                table: "Lembrete",
                column: "id_animal");

            migrationBuilder.CreateIndex(
                name: "morada_idx",
                table: "Morada",
                columns: new[] { "rua", "porta", "andar", "codigo_postal", "freguesia", "distrito", "concelho", "pais" },
                unique: true,
                filter: "[andar] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacao_id_cliente",
                table: "Notificacao",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Servico_id_animal",
                table: "Servico",
                column: "id_animal");

            migrationBuilder.CreateIndex(
                name: "IX_Servico_id_cliente",
                table: "Servico",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Servico_id_estabelecimento",
                table: "Servico",
                column: "id_estabelecimento");

            migrationBuilder.CreateIndex(
                name: "IX_Servico_id_funcionario",
                table: "Servico",
                column: "id_funcionario");

            migrationBuilder.CreateIndex(
                name: "IX_Servico_id_servico_catalogo",
                table: "Servico",
                column: "id_servico_catalogo");

            migrationBuilder.CreateIndex(
                name: "servico_catalogo_idx",
                table: "Servico_Catalogo",
                columns: new[] { "id_estabelecimento", "tipo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Servico_Prescricao_id_prescricao",
                table: "Servico_Prescricao",
                column: "id_prescricao");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_Estabelecimento_id_estabelecimento",
                table: "Stock_Estabelecimento",
                column: "id_estabelecimento");

            migrationBuilder.CreateIndex(
                name: "IX_Tratamento_id_stock",
                table: "Tratamento",
                column: "id_stock");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Avaliacao_Estabelecimento_Utilizador");

            migrationBuilder.DropTable(
                name: "Cliente_Animal");

            migrationBuilder.DropTable(
                name: "Encomenda_Stock");

            migrationBuilder.DropTable(
                name: "Estabelecimento_Gerente");

            migrationBuilder.DropTable(
                name: "Estabelecimento_Horario");

            migrationBuilder.DropTable(
                name: "Funcionario_Estabelecimento");

            migrationBuilder.DropTable(
                name: "Funcionario_Horario");

            migrationBuilder.DropTable(
                name: "Lembrete");

            migrationBuilder.DropTable(
                name: "Notificacao");

            migrationBuilder.DropTable(
                name: "Servico_Prescricao");

            migrationBuilder.DropTable(
                name: "Tratamento");

            migrationBuilder.DropTable(
                name: "Encomenda");

            migrationBuilder.DropTable(
                name: "Servico");

            migrationBuilder.DropTable(
                name: "Prescricao");

            migrationBuilder.DropTable(
                name: "Stock_Estabelecimento");

            migrationBuilder.DropTable(
                name: "Animal");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "Funcionario");

            migrationBuilder.DropTable(
                name: "Servico_Catalogo");

            migrationBuilder.DropTable(
                name: "Estabelecimento");

            migrationBuilder.DropTable(
                name: "Morada");
        }
    }
}

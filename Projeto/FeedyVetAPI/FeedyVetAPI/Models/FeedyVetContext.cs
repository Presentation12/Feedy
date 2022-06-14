using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FeedyVetAPI.Models
{
    public partial class FeedyVetContext : DbContext
    {
        public FeedyVetContext()
        {
        }

        public FeedyVetContext(DbContextOptions<FeedyVetContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Animal> Animal { get; set; }
        public virtual DbSet<AvaliacaoEstabelecimentoUtilizador> AvaliacaoEstabelecimentoUtilizador { get; set; }
        public virtual DbSet<Cliente> Cliente { get; set; }
        public virtual DbSet<ClienteAnimal> ClienteAnimal { get; set; }
        public virtual DbSet<Estabelecimento> Estabelecimento { get; set; }
        public virtual DbSet<EstabelecimentoGerente> EstabelecimentoGerente { get; set; }
        public virtual DbSet<EstabelecimentoHorario> EstabelecimentoHorario { get; set; }
        public virtual DbSet<Encomenda> Encomenda { get; set; }
        public virtual DbSet<EncomendaStock> EncomendaStock { get; set; }
        public virtual DbSet<Lembrete> Lembrete { get; set; }
        public virtual DbSet<Morada> Morada { get; set; }
        public virtual DbSet<Notificacao> Notificacao { get; set; }
        public virtual DbSet<Prescricao> Prescricao { get; set; }
        public virtual DbSet<Servico> Servico { get; set; }
        public virtual DbSet<ServicoCatalogo> ServicoCatalogo { get; set; }
        public virtual DbSet<ServicoPrescricao> ServicoPrescricao { get; set; }
        public virtual DbSet<StockEstabelecimento> StockEstabelecimento { get; set; }
        public virtual DbSet<Tratamento> Tratamento { get; set; }
        public virtual DbSet<Funcionario> Funcionario { get; set; }
        public virtual DbSet<FuncionarioEstabelecimento> FuncionarioEstabelecimento { get; set; }
        public virtual DbSet<FuncionarioHorario> FuncionarioHorario { get; set; }
        public virtual DbSet<NotificacaoFuncionario> NotificacaoFuncionario { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=feedyvet.database.windows.net;Initial Catalog=FeedyVet;Persist Security Info=True;User ID=feedyvet;Password=Quimgordo69;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Animal>(entity =>
            {
                entity.HasKey(e => e.IdAnimal)
                    .HasName("pk_id_animal");

                entity.Property(e => e.IdAnimal).HasColumnName("id_animal");

                entity.Property(e => e.Altura).HasColumnName("altura");

                entity.Property(e => e.AnimalFoto)
                    .HasColumnName("animal_foto")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Classe)
                    .HasColumnName("classe")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DataNascimento)
                    .HasColumnName("data_nascimento")
                    .HasColumnType("datetime");

                entity.Property(e => e.Especie)
                    .HasColumnName("especie")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Estado)
                    .HasColumnName("estado")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Genero)
                    .HasColumnName("genero")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasColumnName("nome")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Peso).HasColumnName("peso");
            });

            modelBuilder.Entity<AvaliacaoEstabelecimentoUtilizador>(entity =>
            {
                entity.HasKey(e => e.IdAvaliacaoEstabelecimentoUtilizador)
                    .HasName("pk_id_avaliacao_estabelecimento_utilizador");

                entity.ToTable("Avaliacao_Estabelecimento_Utilizador");

                entity.Property(e => e.IdAvaliacaoEstabelecimentoUtilizador).HasColumnName("id_avaliacao_estabelecimento_utilizador");

                entity.Property(e => e.Avaliacao).HasColumnName("avaliacao");

                entity.Property(e => e.DataAvaliacao)
                    .HasColumnName("data_avaliacao")
                    .HasColumnType("datetime");

                entity.Property(e => e.IdCliente).HasColumnName("id_cliente");

                entity.Property(e => e.IdEstabelecimento).HasColumnName("id_estabelecimento");

                entity.Property(e => e.Texto)
                    .IsRequired()
                    .HasColumnName("texto")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.AvaliacaoEstabelecimentoUtilizador)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_id_cliente_avaliacao");

                entity.HasOne(d => d.IdEstabelecimentoNavigation)
                    .WithMany(p => p.AvaliacaoEstabelecimentoUtilizador)
                    .HasForeignKey(d => d.IdEstabelecimento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_id_estabelecimento_avaliacao");
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.IdCliente)
                    .HasName("pk_id_cliente");

                entity.HasIndex(e => e.Email)
                    .HasName("email_uk")
                    .IsUnique();

                entity.Property(e => e.IdCliente).HasColumnName("id_cliente");

                entity.Property(e => e.Apelido)
                    .IsRequired()
                    .HasColumnName("apelido")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ClienteFoto)
                    .HasColumnName("cliente_foto")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Estado)
                    .HasColumnName("estado")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.IdMorada).HasColumnName("id_morada");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasColumnName("nome")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pass)
                    .IsRequired()
                    .HasColumnName("pass")
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.Telemovel).HasColumnName("telemovel");

                entity.Property(e => e.TipoConta)
                    .IsRequired()
                    .HasColumnName("tipo_conta")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ValorConta).HasColumnName("valor_conta");

                entity.HasOne(d => d.IdMoradaNavigation)
                    .WithMany(p => p.Cliente)
                    .HasForeignKey(d => d.IdMorada)
                    .HasConstraintName("fk_id_morada");
            });

            modelBuilder.Entity<ClienteAnimal>(entity =>
            {
                entity.HasKey(e => e.IdAnimal)
                    .HasName("pk_cliente_animal");

                entity.ToTable("Cliente_Animal");

                entity.Property(e => e.IdAnimal)
                    .HasColumnName("id_animal")
                    .ValueGeneratedNever();

                entity.Property(e => e.IdCliente).HasColumnName("id_cliente");

                entity.HasOne(d => d.IdAnimalNavigation)
                    .WithOne(p => p.ClienteAnimal)
                    .HasForeignKey<ClienteAnimal>(d => d.IdAnimal)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_cliente_animal_id_animal");

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.ClienteAnimal)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_cliente_animal_id_cliente");
            });

            modelBuilder.Entity<Estabelecimento>(entity =>
            {
                entity.HasKey(e => e.IdEstabelecimento)
                    .HasName("pk_id_estabelecimento");

                entity.HasIndex(e => e.Contacto)
                    .HasName("contacto_uk")
                    .IsUnique();

                entity.HasIndex(e => new { e.Nome, e.IdMorada })
                    .HasName("estabelecimento_idx")
                    .IsUnique();

                entity.Property(e => e.IdEstabelecimento).HasColumnName("id_estabelecimento");

                entity.Property(e => e.EstabelecimentoFoto)
                    .HasColumnName("estabelecimento_foto")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Contacto).HasColumnName("contacto");

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.IdMorada).HasColumnName("id_morada");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasColumnName("nome")
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdMoradaNavigation)
                    .WithMany(p => p.Estabelecimento)
                    .HasForeignKey(d => d.IdMorada)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_id_morada_cliente");
            });

            modelBuilder.Entity<EstabelecimentoGerente>(entity =>
            {
                entity.HasKey(e => new { e.IdEstabelecimento, e.IdFuncionario })
                    .HasName("pk_estabelecimento_gerente");

                entity.ToTable("Estabelecimento_Gerente");

                entity.Property(e => e.IdEstabelecimento).HasColumnName("id_estabelecimento");

                entity.Property(e => e.IdFuncionario).HasColumnName("id_funcionario");
            });

            modelBuilder.Entity<EstabelecimentoHorario>(entity =>
            {
                entity.HasKey(e => e.IdEstabelecimentoHorario)
                    .HasName("pk_id_estabelecimento_horario");

                entity.ToTable("Estabelecimento_Horario");

                entity.HasIndex(e => new { e.IdEstabelecimento, e.HorarioAbertura, e.HorarioEncerramento, e.DiaSemana })
                    .HasName("estabelecimento_horario_idx")
                    .IsUnique();

                entity.Property(e => e.IdEstabelecimentoHorario).HasColumnName("id_estabelecimento_horario");

                entity.Property(e => e.DiaSemana)
                    .IsRequired()
                    .HasColumnName("dia_semana")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.HorarioAbertura)
                    .IsRequired()
                    .HasColumnName("horario_abertura")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.HorarioEncerramento)
                    .IsRequired()
                    .HasColumnName("horario_encerramento")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.IdEstabelecimento).HasColumnName("id_estabelecimento");

                entity.HasOne(d => d.IdEstabelecimentoNavigation)
                    .WithMany(p => p.EstabelecimentoHorario)
                    .HasForeignKey(d => d.IdEstabelecimento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_id_clincia_horario");
            });

            modelBuilder.Entity<Encomenda>(entity =>
            {
                entity.HasKey(e => e.IdEncomenda)
                    .HasName("pk_encomenda");

                entity.Property(e => e.IdEncomenda).HasColumnName("id_encomenda");

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.IdCliente).HasColumnName("id_cliente");

                entity.Property(e => e.IdMorada).HasColumnName("id_morada");

                entity.Property(e => e.MetodoPagamento)
                    .IsRequired()
                    .HasColumnName("metodo_pagamento")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.Encomenda)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_encomenda_cliente");

                entity.HasOne(d => d.IdMoradaNavigation)
                    .WithMany(p => p.Encomenda)
                    .HasForeignKey(d => d.IdMorada)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_encomenda_morada");
            });

            modelBuilder.Entity<EncomendaStock>(entity =>
            {
                entity.HasKey(e => new { e.IdEncomenda, e.IdStock })
                    .HasName("pk_encomenda_stock");

                entity.ToTable("Encomenda_Stock");

                entity.Property(e => e.IdEncomenda).HasColumnName("id_encomenda");

                entity.Property(e => e.IdStock).HasColumnName("id_stock");

                

                entity.Property(e => e.Qtd).HasColumnName("qtd");

                entity.HasOne(d => d.IdEncomendaNavigation)
                    .WithMany(p => p.EncomendaStock)
                    .HasForeignKey(d => d.IdEncomenda)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_encomenda_stock_id_encomenda");

                entity.HasOne(d => d.IdStockNavigation)
                    .WithMany(p => p.EncomendaStock)
                    .HasForeignKey(d => d.IdStock)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_encomenda_stock_id_stock");
            });

            modelBuilder.Entity<Lembrete>(entity =>
            {
                entity.HasKey(e => e.IdLembrete)
                    .HasName("pk_id_lembrete");

                entity.Property(e => e.IdLembrete).HasColumnName("id_lembrete");

                entity.Property(e => e.DataLembrete)
                    .HasColumnName("data_lembrete")
                    .HasColumnType("datetime");

                entity.Property(e => e.Frequencia)
                    .IsRequired()
                    .HasColumnName("frequencia")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.HoraLembrete).HasColumnName("hora_lembrete");

                entity.Property(e => e.IdAnimal).HasColumnName("id_animal");

                entity.Property(e => e.LembreteDescricao)
                    .HasColumnName("lembrete_descricao")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdAnimalNavigation)
                    .WithMany(p => p.Lembrete)
                    .HasForeignKey(d => d.IdAnimal)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_id_animal_lembrete");
            });

            modelBuilder.Entity<Morada>(entity =>
            {
                entity.HasKey(e => e.IdMorada)
                    .HasName("pk_id_morada");

                entity.HasIndex(e => new { e.Rua, e.Porta, e.Andar, e.CodigoPostal, e.Freguesia, e.Distrito, e.Concelho, e.Pais })
                    .HasName("morada_idx")
                    .IsUnique();

                entity.Property(e => e.IdMorada).HasColumnName("id_morada");

                entity.Property(e => e.Andar).HasColumnName("andar");

                entity.Property(e => e.CodigoPostal)
                    .IsRequired()
                    .HasColumnName("codigo_postal")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Concelho)
                    .IsRequired()
                    .HasColumnName("concelho")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Distrito)
                    .IsRequired()
                    .HasColumnName("distrito")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Freguesia)
                    .IsRequired()
                    .HasColumnName("freguesia")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Pais)
                    .IsRequired()
                    .HasColumnName("pais")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Porta).HasColumnName("porta");

                entity.Property(e => e.Rua)
                    .IsRequired()
                    .HasColumnName("rua")
                    .HasMaxLength(60)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Notificacao>(entity =>
            {
                entity.HasKey(e => e.IdNotificacao)
                    .HasName("pk_id_notificacao");

                entity.Property(e => e.IdNotificacao).HasColumnName("id_notificacao");

                entity.Property(e => e.DataNotificacao)
                    .HasColumnName("data_notificacao")
                    .HasColumnType("datetime");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasColumnName("descricao")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.IdCliente).HasColumnName("id_cliente");

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.Notificacao)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_id_cliente_notificacao");
            });

            modelBuilder.Entity<Prescricao>(entity =>
            {
                entity.HasKey(e => e.IdPrescricao)
                    .HasName("pk_id_prescricao");

                entity.Property(e => e.IdPrescricao).HasColumnName("id_prescricao");

                entity.Property(e => e.DataExpiracao)
                    .IsRequired()
                    .HasColumnName("data_expiracao")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasColumnName("descricao")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Servico>(entity =>
            {
                entity.HasKey(e => e.IdServico)
                    .HasName("pk_id_servico");

                entity.Property(e => e.IdServico).HasColumnName("id_servico");

                entity.Property(e => e.DataServico)
                    .HasColumnName("data_servico")
                    .HasColumnType("datetime");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasColumnName("descricao")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasMaxLength(35)
                    .IsUnicode(false);

                entity.Property(e => e.IdAnimal).HasColumnName("id_animal");

                entity.Property(e => e.IdCliente).HasColumnName("id_cliente");

                entity.Property(e => e.IdEstabelecimento).HasColumnName("id_estabelecimento");

                entity.Property(e => e.IdServicoCatalogo).HasColumnName("id_servico_catalogo");

                entity.Property(e => e.IdFuncionario).HasColumnName("id_funcionario");

                entity.HasOne(d => d.IdAnimalNavigation)
                    .WithMany(p => p.Servico)
                    .HasForeignKey(d => d.IdAnimal)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_id_animal_servico");

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.Servico)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_id_cliente_servico");

                entity.HasOne(d => d.IdEstabelecimentoNavigation)
                    .WithMany(p => p.Servico)
                    .HasForeignKey(d => d.IdEstabelecimento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_id_estabelecimento_servico");

                entity.HasOne(d => d.IdServicoCatalogoNavigation)
                    .WithMany(p => p.Servico)
                    .HasForeignKey(d => d.IdServicoCatalogo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_id_catalogo_servico");

                entity.HasOne(d => d.IdFuncionarioNavigation)
                    .WithMany(p => p.Servico)
                    .HasForeignKey(d => d.IdFuncionario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_id_funcionario_servico");
            });

            modelBuilder.Entity<ServicoCatalogo>(entity =>
            {
                entity.HasKey(e => e.IdServicoCatalogo)
                    .HasName("pk_id_servico_catalogo");

                entity.ToTable("Servico_Catalogo");

                entity.HasIndex(e => new { e.IdEstabelecimento, e.Tipo })
                    .HasName("servico_catalogo_idx")
                    .IsUnique();

                entity.Property(e => e.IdServicoCatalogo).HasColumnName("id_servico_catalogo");

                entity.Property(e => e.CatalogoFoto)
                    .HasColumnName("catalogo_foto")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Descricao)
                    .HasColumnName("descricao")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.IdEstabelecimento).HasColumnName("id_estabelecimento");

                entity.Property(e => e.Preco).HasColumnName("preco");

                entity.Property(e => e.Tipo)
                    .IsRequired()
                    .HasColumnName("tipo")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdEstabelecimentoNavigation)
                    .WithMany(p => p.ServicoCatalogo)
                    .HasForeignKey(d => d.IdEstabelecimento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_id_estabelecimento_catalogo");
            });

            modelBuilder.Entity<ServicoPrescricao>(entity =>
            {
                entity.HasKey(e => new { e.IdServico, e.IdPrescricao })
                    .HasName("pk_servico_prescricao");

                entity.ToTable("Servico_Prescricao");

                entity.Property(e => e.IdServico).HasColumnName("id_servico");

                entity.Property(e => e.IdPrescricao).HasColumnName("id_prescricao");

                entity.HasOne(d => d.IdPrescricaoNavigation)
                    .WithMany(p => p.ServicoPrescricao)
                    .HasForeignKey(d => d.IdPrescricao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_servico_prescricao_id_prescricao");

                entity.HasOne(d => d.IdServicoNavigation)
                    .WithMany(p => p.ServicoPrescricao)
                    .HasForeignKey(d => d.IdServico)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_servico_prescricao_id_servico");
            });

            modelBuilder.Entity<StockEstabelecimento>(entity =>
            {
                entity.HasKey(e => e.IdStock)
                    .HasName("pk_stock_estabelecimento");

                entity.ToTable("Stock_Estabelecimento");

                entity.Property(e => e.IdStock).HasColumnName("id_stock");

                entity.Property(e => e.Descricao)
                    .HasColumnName("descricao")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.IdEstabelecimento).HasColumnName("id_estabelecimento");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasColumnName("nome")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Preco)
                    .HasColumnName("preco")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Stock).HasColumnName("stock");

                entity.Property(e => e.TipoStock)
                    .IsRequired()
                    .HasColumnName("tipo_stock")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Volume).HasColumnName("volume");

                entity.HasOne(d => d.IdEstabelecimentoNavigation)
                    .WithMany(p => p.StockEstabelecimento)
                    .HasForeignKey(d => d.IdEstabelecimento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_stock_estabelecimento_id_estabelecimento");
            });

            modelBuilder.Entity<Tratamento>(entity =>
            {
                entity.HasKey(e => new { e.IdPrescricao, e.IdStock })
                    .HasName("pk_tratamento");

                entity.Property(e => e.IdPrescricao).HasColumnName("id_prescricao");

                entity.Property(e => e.IdStock).HasColumnName("id_stock");

                entity.Property(e => e.Quantidade).HasColumnName("quantidade");

                entity.HasOne(d => d.IdPrescricaoNavigation)
                    .WithMany(p => p.Tratamento)
                    .HasForeignKey(d => d.IdPrescricao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tratamento_id_prescricao");

                entity.HasOne(d => d.IdStockNavigation)
                    .WithMany(p => p.Tratamento)
                    .HasForeignKey(d => d.IdStock)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tratamento_id_stock");
            });

            modelBuilder.Entity<Funcionario>(entity =>
            {
                entity.HasKey(e => e.IdFuncionario)
                    .HasName("pk_id_funcionario");

                entity.HasIndex(e => e.Codigo)
                    .HasName("codigo_uk")
                    .IsUnique();

                entity.HasIndex(e => e.Email)
                    .HasName("email_vet_uk")
                    .IsUnique();

                entity.HasIndex(e => e.Telemovel)
                    .HasName("telemovel_uk")
                    .IsUnique();

                entity.Property(e => e.IdFuncionario).HasColumnName("id_funcionario");

                entity.Property(e => e.Apelido)
                    .HasColumnName("apelido")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Codigo).HasColumnName("codigo");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Especialidade)
                    .IsRequired()
                    .HasColumnName("especialidade")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("estado")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasColumnName("nome")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Pass)
                    .IsRequired()
                    .HasColumnName("pass")
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.Telemovel).HasColumnName("telemovel");

                entity.Property(e => e.FuncionarioFoto)
                    .HasColumnName("funcionario_foto")
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FuncionarioEstabelecimento>(entity =>
            {
                entity.HasKey(e => new { e.IdFuncionario, e.IdEstabelecimento })
                    .HasName("pk_funcionario_estabelecimento");

                entity.ToTable("Funcionario_Estabelecimento");

                entity.Property(e => e.IdFuncionario).HasColumnName("id_funcionario");

                entity.Property(e => e.IdEstabelecimento).HasColumnName("id_estabelecimento");

                entity.Property(e => e.DataFim)
                    .HasColumnName("data_fim")
                    .HasColumnType("date");

                entity.Property(e => e.DataInicio)
                    .HasColumnName("data_inicio")
                    .HasColumnType("date");

                entity.HasOne(d => d.IdEstabelecimentoNavigation)
                    .WithMany(p => p.FuncionarioEstabelecimento)
                    .HasForeignKey(d => d.IdEstabelecimento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_funcionario_estabelecimento_id_estabelecimento");

                entity.HasOne(d => d.IdFuncionarioNavigation)
                    .WithMany(p => p.FuncionarioEstabelecimento)
                    .HasForeignKey(d => d.IdFuncionario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_funcionario_estabelecimento_id_funcionario");
            });

            modelBuilder.Entity<FuncionarioHorario>(entity =>
            {
                entity.HasKey(e => e.IdFuncionarioHorario)
                    .HasName("pk_funcionario_horario");

                entity.ToTable("Funcionario_Horario");

                entity.Property(e => e.IdFuncionarioHorario).HasColumnName("id_funcionario_horario");

                entity.Property(e => e.DiaSemana)
                    .IsRequired()
                    .HasColumnName("dia_semana")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.HoraEntrada).HasColumnName("hora_entrada");

                entity.Property(e => e.HoraSaida).HasColumnName("hora_saida");

                entity.Property(e => e.IdFuncionario).HasColumnName("id_funcionario");

                entity.HasOne(d => d.IdFuncionarioNavigation)
                    .WithMany(p => p.FuncionarioHorario)
                    .HasForeignKey(d => d.IdFuncionario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_funcionario_horario_id_funcionario");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

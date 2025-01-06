using DBConnector.Models;
using DBConnector.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Dynamic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DBConnectionProject;

internal class ApplicationDbContext
{
    public class ApplicationDbContextDinamico : DbContext
    {
        public ApplicationDbContextDinamico(DbContextOptions<ApplicationDbContextDinamico> options) : base(options) { }

        public DbSet<Tabela> Tabelas { get; set; }
        public DbSet<Atendente> Atendentes { get; set; }
        public DbSet<Consumidor> Consumidores { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Atendente>(entity =>
            {
                entity.ToTable("tb_atendente");
                entity.HasKey(a => a.id_atendente);
                entity.Property(e => e.id_atendente)
                      .ValueGeneratedOnAdd();

            });


            modelBuilder.Entity<Consumidor>(entity =>
            {
                entity.ToTable("tb_consumidor");
                entity.HasKey(c => c.id_consumidor);
                entity.Property(c => c.id_consumidor)
                      .ValueGeneratedOnAdd();

            });

            modelBuilder.Entity<Tabela>(entity =>
            {
                entity.HasNoKey();

            });

            base.OnModelCreating(modelBuilder);
        }
    }

    // Exceções personalizadas
    public class PostgresConnectionException : Exception
    {
        public PostgresConnectionException(string message, Exception innerException)
            : base($"Falha ao conectar ao banco de dados Postgres: {message}", innerException)
        { }
    }

    public class MySQLConnectionException : Exception
    {
        public MySQLConnectionException(string message, Exception innerException)
            : base($"Falha ao conectar ao banco de dados MySQL: {message}", innerException)
        { }
    }

    public class SQLiteConnectionException : Exception
    {
        public SQLiteConnectionException(string message, Exception innerException)
            : base($"Falha ao conectar ao banco de dados SQLite: {message}", innerException)
        { }
    }

    public class AccessConnectionException : Exception
    {
        public AccessConnectionException(string message, Exception innerException)
            : base($"Falha ao conectar ao banco de dados Access ou ao acessar a tabela: {message}", innerException)
        { }
    }

    // Método para configurar a conexão com Postgres
    public static ServiceProvider ConexaoComBancoPostgres()
    {
        try
        {
            var connectionString = AppConfiguration.GetConnectionString("Postgres");

            var serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationDbContextDinamico>(options =>
                    options.UseNpgsql(connectionString))
                .BuildServiceProvider();

            return serviceProvider;
        }
        catch (Exception ex)
        {
            throw new PostgresConnectionException("Verifique as configurações de conexão ou o status do banco de dados.", ex);
        }
    }

    // Método para configurar a conexão com MySQL
    public static ServiceProvider ConexaoComBancoMySQL()
    {
        try
        {
            var connectionString = AppConfiguration.GetConnectionString("MySQL");

            var serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationDbContextDinamico>(options =>
                    options.UseMySql(connectionString,
                                     new MySqlServerVersion(new Version(8, 0, 21))))
                .BuildServiceProvider();

            return serviceProvider;
        }
        catch (Exception ex)
        {
            throw new MySQLConnectionException("Verifique as configurações de conexão ou o status do banco de dados.", ex);
        }
    }

    // Método para configurar a conexão com SQLite
    public static ServiceProvider ConexaoComBancoSQLite()
    {
        try
        {
            var connectionString = AppConfiguration.GetConnectionString("SQLite");

            var serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationDbContextDinamico>(options =>
                    options.UseSqlite(connectionString))
                .BuildServiceProvider();

            return serviceProvider;
        }
        catch (Exception ex)
        {
            throw new SQLiteConnectionException("Verifique as configurações de conexão ou o caminho do arquivo SQLite.", ex);
        }
    }

    // Método para configurar a conexão com Microsoft Access
    public static async Task<List<dynamic>> ConexaoComBancoAccess(string tabela)
    {
        try
        {
            var registros = new List<dynamic>();
            var connectionString = AppConfiguration.GetConnectionString("Access");

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                await connection.OpenAsync();

                // SQL específico para a tabela tb_cli_atendente e tb_cli_consumidor
                string sql = tabela.ToLower() == "tb_cli_atendente"
                    ? @"SELECT ID_ATENDENTE, NR_DOCUMENTO, NM_ATENDENTE, 
                            DS_EMAIL, DS_TELEFONE_CELULAR AS NR_TELEFONE,
                            NM_LOGIN AS DS_LOGIN, DS_SENHA, FL_SUPORTE, DT_BLOQUEIO 
                     FROM TB_CLI_ATENDENTE"
                    : tabela.ToLower() == "tb_cli_consumidor" 
                    ? @"SELECT ID_CONSUMIDOR, NM_CONSUMIDOR, NR_DOCUMENTO, ID_TIPO_DOCUMENTO,
                            DS_EMAIL, DS_TELEFONE_CELULAR as NR_CELULAR, FL_SEXO, 0 as FL_CRM, 0 as FL_SMS, 0 as FL_EMAIL,
                            NR_CEP, DS_ENDERECO, NM_BAIRRO as DS_BAIRRO, NM_CIDADE,
                            NM_ESTADO as SG_UF, NR_DIA_ANIVERSARIO, NR_MES_ANIVERSARIO
                     FROM TB_CLI_CONSUMIDOR"
                    : $"SELECT * FROM {tabela}";

                OleDbCommand command = new OleDbCommand(sql, connection);

                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var registro = new ExpandoObject() as IDictionary<string, object>;
                        string idTeste = string.Empty;
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string columnName = reader.GetName(i);
                            object columnValue = reader.IsDBNull(i) ? null : reader.GetValue(i);


                            if (columnName.ToLower() == "id_consumidor")
                            {
                                idTeste = columnValue.ToString();
                            }

                            if (reader.GetDataTypeName(i).ToLower().Contains("varchar") && columnName.ToLower() != "ds_senha")
                            {
                                columnValue = columnValue != null
                                    ? columnValue.ToString()!
                                    : null;
                            }

                            if (columnName.ToLower() == "nr_documento" && (columnValue == null || string.IsNullOrEmpty(columnValue.ToString())))
                            {
                                columnValue = $"Doc_Value_Null_{idTeste}";
                            }


                            registro[columnName] = columnValue;
                        }

                        registros.Add(registro);
                    }
                }
            }

            return registros;
        }
        catch (Exception ex)
        {
            throw new AccessConnectionException($"Não foi possível acessar a tabela {tabela}. Verifique se a tabela existe e se os dados de conexão estão corretos.", ex);
        }
    }
}

using DBConnector.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DBConnectionProject.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;

namespace DBConnector.Functions
{
    internal class ColunasHelper
    {
        public static async Task<List<string>> ObterColunasTabela(ServiceProvider? serviceProvider, string banco, string tabela)
        {
            var colunas = new List<string>();

            try
            {
                if (banco.ToLower() == "access")
                {
                    var connectionString = AppConfiguration.GetConnectionString("Access");
                    using (var connection = new OleDbConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        var schemaTable = connection.GetSchema("Columns", new[] { null, null, tabela });

                        foreach (DataRow row in schemaTable.Rows)
                        {
                            colunas.Add(row["COLUMN_NAME"].ToString()!);
                        }
                    }
                }
                else
                {
                    using var scope = serviceProvider!.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContextDinamico>();

                    var query = $"SELECT column_name FROM information_schema.columns WHERE table_name = '{tabela}' AND table_schema = 'public'";
                    colunas = await context.Tabelas
                        .FromSqlRaw(query)
                        .Select(t => t.column_name)
                        .ToListAsync();
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Erro de configuração ou modelo ao acessar as colunas da tabela '{tabela}' no banco '{banco}': {ex.Message}");
            }
            catch (OleDbException oleEx)
            {
                Console.WriteLine($"Erro de conexão com o banco Access ao acessar as colunas da tabela '{tabela}': {oleEx.Message}");
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Erro de banco de dados ao acessar as colunas da tabela '{tabela}' no banco '{banco}': {dbEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado ao acessar as colunas da tabela '{tabela}' no banco '{banco}': {ex.Message}");
            }

            return colunas;
        }
    }
}

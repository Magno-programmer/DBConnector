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
    internal class TabelasHelper
    {
        public static async Task<List<string>> ListarTabelas(ServiceProvider? serviceProvider, string banco)
        {
            var tabelas = new List<string>();

            try
            {
                if (banco.ToLower() == "access")
                {
                    var connectionString = AppConfiguration.GetConnectionString("Access");
                    using (var connection = new OleDbConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        DataTable schemaTable = connection.GetSchema("Tables");

                        tabelas.AddRange(from DataRow row in schemaTable.Rows
                                         where row["TABLE_TYPE"].ToString() == "TABLE"
                                         select row["TABLE_NAME"].ToString()!);
                    }
                }
                else if (banco.ToLower() == "sqlite")
                {
                    using var scope = serviceProvider!.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContextDinamico>();

                    var query = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'";
                    tabelas = await context.Tabelas
                        .FromSqlRaw(query)
                        .Select(t => t.name)
                        .ToListAsync();
                }
                else if (banco.ToLower() == "mysql")
                {
                    using var scope = serviceProvider!.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContextDinamico>();

                    var query = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'inforlube_6'";
                    tabelas = await context.Tabelas
                        .FromSqlRaw(query)
                        .Select(t => t.table_name)
                        .ToListAsync();
                }
                else
                {
                    using var scope = serviceProvider!.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContextDinamico>();

                    var query = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'";
                    tabelas = await context.Tabelas
                        .FromSqlRaw(query)
                        .Select(t => t.table_name)
                        .ToListAsync();
                }
            }
            catch (OleDbException ex)
            {
                Console.WriteLine($"Erro ao acessar o banco Access: {ex.Message}");
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Erro ao acessar o banco relacional ({banco}): {dbEx.Message}");
            }
            catch (InvalidOperationException invEx)
            {
                Console.WriteLine($"Erro de operação ao acessar tabelas do banco {banco}: {invEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado ao listar tabelas do banco {banco}: {ex.Message}");
            }

            return tabelas;
        }
    }
}

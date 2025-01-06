using DBConnectionProject;
using DBConnector.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DBConnectionProject.ApplicationDbContext;

namespace DBConnector.Functions
{
    internal class RegistrosHelper
    {
        public static async Task<List<dynamic>> ObterRegistros(ServiceProvider? serviceProvider, string banco, string tabela)
        {
            var registros = new List<dynamic>();

            try
            {
                if (banco.ToLower() == "access")
                {
                    try
                    {
                        registros = await ApplicationDbContext.ConexaoComBancoAccess(tabela);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao acessar a tabela '{tabela}' no banco Access: {ex.Message}");
                    }
                }
                else
                {
                    try
                    {
                        using var scope = serviceProvider!.CreateScope();
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContextDinamico>();

                        if(tabela == "tb_consumidor")
                        {
                            List<Consumidor> consumidores = await context.Consumidores.ToListAsync();
                            registros.AddRange(consumidores.Cast<dynamic>());

                        }
                        else if(tabela.Equals("tb_atendente"))
                        {

                            List<Atendente> atendentes = await context.Atendentes.ToListAsync();
                            registros.AddRange(atendentes.Cast<dynamic>());
                        }
                        else
                        {
                            Console.WriteLine($"Nenhum registro encontrado na tabela '{tabela}' no banco {banco}.");
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine($"Erro de configuração ou consulta na tabela '{tabela}' no banco {banco}: {ex.Message}");
                    }
                    catch (DbUpdateException dbEx)
                    {
                        Console.WriteLine($"Erro de banco de dados ao acessar a tabela '{tabela}' no banco {banco}: {dbEx.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro inesperado ao acessar a tabela '{tabela}' no banco {banco}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro geral ao obter registros da tabela '{tabela}': {ex.Message}");
            }

            return registros;
        }

        public static async Task<dynamic?> ObterRegistroPorId(ServiceProvider serviceProvider, string banco, string tabela, int id)
        {
            try
            {
                // Constrói o nome do campo ID dinamicamente baseado no nome da tabela

                if (banco.ToLower() == "access")
                {
                    string idCampo;
                    if (tabela.Contains("cli"))
                    {
                        idCampo = $"ID_{tabela.Replace("tb_cli_", "", StringComparison.OrdinalIgnoreCase).ToUpper()}";

                    }
                    else
                    {
                        idCampo = $"ID_{tabela.Replace("tb_", "", StringComparison.OrdinalIgnoreCase).ToUpper()}";
                    }
                    var registros = await ApplicationDbContext.ConexaoComBancoAccess(tabela);

                    return registros.FirstOrDefault(r => (r as IDictionary<string, object>)[idCampo]?.ToString() == id.ToString());
                    
                }
                else
                {
                    using var scope = serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContextDinamico>();

                    var query = $"SELECT * FROM {tabela} WHERE id_{tabela.Replace("tb_", "")} = {id}";
                    var resultado = await context.Database.ExecuteSqlRawAsync(query);
                    return resultado; // Retorna o registro encontrado
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter registro por ID na tabela '{tabela}': {ex.Message}");
                return null;
            }
        }


    }

}

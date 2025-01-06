using DBConnector.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DBConnectionProject.ApplicationDbContext;

namespace DBConnector.Functions;

internal class DeleteHelper
{
    // Deletar registros
    public static void DeletarRegistros(ServiceProvider serviceProvider, List<dynamic> registros, string tabela, string banco)
    {
        try
        {
            Console.WriteLine($"\nTentando deletar {registros.Count} registros da tabela '{tabela}' no banco {banco}.");

            if (registros.Count == 0)
            {
                Console.WriteLine("Nenhum registro encontrado para exclusão.");
                return;
            }

            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContextDinamico>();

            foreach (var registro in registros)
            {
                try
                {
                    if (registro is Atendente atendente)
                    {
                        context.Atendentes.Remove(atendente);
                        Console.WriteLine($"Atendente com ID {atendente.id_atendente} deletado com sucesso.");
                    }
                    else if (registro is Consumidor consumidor)
                    {
                        context.Consumidores.Remove(consumidor);
                        Console.WriteLine($"Consumidor com ID {consumidor.id_consumidor} deletado com sucesso.");
                    }
                    else
                    {
                        Console.WriteLine($"Registro inválido ou não suportado para exclusão: {registro}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao tentar deletar um registro da tabela '{tabela}': {ex.Message}");
                }
            }

            // Salvar alterações para commits em massa
            context.SaveChanges();
            Console.WriteLine($"Todos os registros válidos foram deletados da tabela '{tabela}' no banco {banco}.");

            // Rezetando a sequencia de IDs caso exclua toda tabela
            if (banco == "postgres")
            {
                context.Database.ExecuteSqlRawAsync($"ALTER SEQUENCE {tabela}_id_{tabela.Replace("tb_", "")}_seq RESTART WITH 1").GetAwaiter().GetResult();
            }
            else if (banco == "mysql")
            {
                context.Database.ExecuteSqlRawAsync($"ALTER TABLE {tabela} AUTO_INCREMENT = 1").GetAwaiter().GetResult();
            }
            else if (banco == "sqlite")
            {
                context.Database.ExecuteSqlRawAsync($"DELETE FROM sqlite_sequence WHERE name = '{tabela}'").GetAwaiter().GetResult();
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao deletar registros da tabela '{tabela}' no banco {banco}: {ex.Message}");
        }
    }

    public static void DeletarRegistroPorId(ServiceProvider serviceProvider, string tabela, string banco, int id)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContextDinamico>();

            Console.WriteLine($"\nTentando deletar o registro com ID {id} na tabela '{tabela}' no banco {banco}.");

            // Monta a query de exclusão com base no banco de dados
            var query = banco.ToLower() switch
            {
                "postgres" => $"DELETE FROM {tabela} WHERE id_{tabela.Replace("tb_", "")} = {id}",
                "mysql" => $"DELETE FROM {tabela} WHERE id_{tabela.Replace("tb_", "")} = {id}",
                "sqlite" => $"DELETE FROM {tabela} WHERE id_{tabela.Replace("tb_", "")} = {id}",
                _ => throw new NotSupportedException($"Banco de dados '{banco}' não suportado para exclusão por ID."),
            };

            // Executa a exclusão
            var resultado = context.Database.ExecuteSqlRaw(query);

            if (resultado > 0)
            {
                Console.WriteLine($"Registro com ID {id} deletado com sucesso da tabela '{tabela}' no banco {banco}.");
            }
            else
            {
                Console.WriteLine($"Nenhum registro com ID {id} encontrado na tabela '{tabela}' no banco {banco}.");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao deletar o registro com ID {id} da tabela '{tabela}' no banco {banco}: {ex.Message}");
        }
    }

}

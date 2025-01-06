using DBConnector.Functions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnector.MenuOptions;

internal class ListarTabelas
{
    public static async Task Executar(ServiceProvider serviceProviderOrigem)
    {
        Console.WriteLine("\nOpções de bancos:");
        Console.WriteLine("Postgres");
        Console.WriteLine("MySQL");
        Console.WriteLine("SQLite");
        Console.WriteLine("Access");
        Console.Write("\nDigite o nome do banco: ");
        string banco = Console.ReadLine()!.ToLower();

        serviceProviderOrigem = ConfiguracaoBanco.ConfigurarConexao(banco);

        if (serviceProviderOrigem == null && banco != "access")
        {
            Console.WriteLine("\nBanco não cadastrado");
            return;
        }

        var tabelas = await TabelasHelper.ListarTabelas(serviceProviderOrigem, banco);
        if (tabelas.Count == 0)
            Console.WriteLine($"\nNenhuma tabela encontrada no banco {banco}.");
        else
            tabelas.ForEach(t => Console.WriteLine($"- {t}"));
    }
}

using DBConnector.Functions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnector.MenuOptions;
public static class ObterColunas
{
    public static async Task Executar(ServiceProvider? serviceProviderOrigem)
    {
        Console.WriteLine("\nOpções de bancos:");
        Console.WriteLine("Postgres");
        Console.WriteLine("MySQL");
        Console.WriteLine("SQLite");
        Console.WriteLine("Access");
        Console.Write("\nDigite o nome do banco: ");
        string banco = Console.ReadLine()!.ToLower();

        serviceProviderOrigem = ConfiguracaoBanco.ConfigurarConexao(banco)!;

        if (serviceProviderOrigem == null && banco != "access")
        {
            Console.WriteLine("\nBanco não cadastrado");
            return;
        }

        var tabelas = await TabelasHelper.ListarTabelas(serviceProviderOrigem, banco);
        if (tabelas.Count == 0)
        {
            Console.WriteLine($"\nNenhuma tabela encontrada no banco {banco}.");
        }
        else
        {
            Console.WriteLine("\nTabelas disponíveis:");
            foreach (var tabela in tabelas)
            {
                Console.WriteLine($"- {tabela}");
            }
        }

        Console.Write("\nDigite o nome da tabela que deseja listar as colunas: ");
        string tabelaSelecionada = Console.ReadLine()!;


        if (tabelas.Contains(tabelaSelecionada.ToUpper()) || tabelas.Contains(tabelaSelecionada.ToLower()))
        {
            List<string> colunas = await ColunasHelper.ObterColunasTabela(serviceProviderOrigem, banco, tabelaSelecionada);
            Console.Clear();
            ExibicaoHelper.ExibirColunas(colunas, tabelaSelecionada, banco);
        }
        else
        {
            Console.WriteLine("Tabela não existente");
            return;

        }
    }
}
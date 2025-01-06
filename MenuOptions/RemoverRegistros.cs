using DBConnector.Functions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnector.MenuOptions;
public static class RemoverRegistros
{
    public static async Task Executar(ServiceProvider? serviceProviderOrigem)
    {
        Console.WriteLine("\nOpções de bancos:");
        Console.WriteLine("Postgres");
        Console.WriteLine("MySQL");
        Console.WriteLine("SQLite");
        Console.Write("\nDigite o nome do banco: ");
        string banco = Console.ReadLine()!.ToLower();

        if (banco == "access" || string.IsNullOrEmpty(banco))
        {
            Console.WriteLine("Banco não pode ser usado para exclusão de dados ou alteração");
            return;
        }
        serviceProviderOrigem = ConfiguracaoBanco.ConfigurarConexao(banco)!;

        var tabelas = await TabelasHelper.ListarTabelas(serviceProviderOrigem, banco);

        if (tabelas.Count == 0)
        {
            Console.WriteLine($"\nNenhuma tabela encontrada no banco {banco}.");
            return;
        }

        Console.WriteLine("\nTabelas disponíveis:");
        foreach (var tabela in tabelas)
        {
            Console.WriteLine($"- {tabela}");
        }

        Console.Write("\nDigite o nome da tabela que deseja excluir os registros: ");
        string tabelaSelecionada = Console.ReadLine()!;

        if (!tabelas.Contains(tabelaSelecionada))
        {
            Console.WriteLine("Tabela não existente");
            return;
        }

        Console.Write("Deseja excluir um registro específico? (S/N): ");
        string escolhaUsuario = Console.ReadLine()!.ToLower();

        if (escolhaUsuario == "s")
        {
            int id;
            Console.Write("Digite o valore de um ID existente: ");
            string escolhaDoId = Console.ReadLine();
            if (escolhaDoId.All(char.IsDigit) && !string.IsNullOrEmpty(escolhaDoId))
            {
                id = int.Parse(escolhaDoId);
            }
            else
            {
                Console.WriteLine("Digite uma opção válida");
                return;
            }

            DeleteHelper.DeletarRegistroPorId(serviceProviderOrigem, tabelaSelecionada, banco, id);

        }
        else if (escolhaUsuario == "n")
        {
            var registros = await RegistrosHelper.ObterRegistros(serviceProviderOrigem, banco, tabelaSelecionada);
            DeleteHelper.DeletarRegistros(serviceProviderOrigem, registros, tabelaSelecionada, banco);

        }
        else
        {
            Console.WriteLine("Opção Inválida");
        }
    }
}
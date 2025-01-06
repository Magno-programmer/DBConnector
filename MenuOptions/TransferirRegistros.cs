using DBConnector.Functions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnector.MenuOptions;

internal static class TransferirRegistros
{
    public static async Task Executar(ServiceProvider? serviceProviderOrigem, ServiceProvider? serviceProviderDestino)
    {
        Console.WriteLine("\nOpções de bancos para origem:");
        Console.WriteLine("Postgres");
        Console.WriteLine("MySQL");
        Console.WriteLine("SQLite");
        Console.WriteLine("Access");
        Console.Write("\nDigite o nome do banco de origem: ");
        string bancoOrigem = Console.ReadLine()!.ToLower();

        serviceProviderOrigem = ConfiguracaoBanco.ConfigurarConexao(bancoOrigem)!;
        if (serviceProviderOrigem == null && bancoOrigem != "access")
        {
            Console.WriteLine("\nBanco não cadastrado");
            return;
        }

        Console.Write("\nDigite o nome da tabela no banco de origem: ");
        string tabelaOrigem = Console.ReadLine()!;

        var colunasOrigem = await ColunasHelper.ObterColunasTabela(serviceProviderOrigem, bancoOrigem, tabelaOrigem);

        var registros = await RegistrosHelper.ObterRegistros(serviceProviderOrigem, bancoOrigem, tabelaOrigem);

        Console.WriteLine("\nOpções de bancos para destino:");
        Console.WriteLine("Postgres");
        Console.WriteLine("MySQL");
        Console.WriteLine("SQLite");
        Console.Write("\nDigite o nome do banco de destino: ");
        string bancoDestino = Console.ReadLine()!.ToLower();

        serviceProviderDestino = ConfiguracaoBanco.ConfigurarConexao(bancoDestino)!;
        if (serviceProviderDestino == null)
        {
            Console.WriteLine("\nBanco não cadastrado");
            return;
        }

        Console.Write("\nDigite o nome da tabela no banco de destino: ");
        string tabelaDestino = Console.ReadLine()!;


        Console.Write("Deseja transferir um registro específico? (S/N): ");
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


            // Obtém o registro único pelo ID
            var registro = await RegistrosHelper.ObterRegistroPorId(serviceProviderOrigem, bancoOrigem, tabelaOrigem, id);

            if (registro == null)
            {
                Console.WriteLine($"Registro com ID {id} não encontrado na tabela '{tabelaOrigem}'.");
                return;
            }

            var colunasDestino = await ColunasHelper.ObterColunasTabela(serviceProviderDestino, bancoDestino, tabelaDestino);
            // Transfere o registro
            Console.Clear();
            await TransferenciaHelper.TransferirRegistroUnico(serviceProviderDestino, registro, tabelaOrigem, tabelaDestino, colunasOrigem, colunasDestino, bancoOrigem);

        }
        else if (escolhaUsuario == "n")
        {
            var colunasDestino = await ColunasHelper.ObterColunasTabela(serviceProviderDestino, bancoDestino, tabelaDestino);

            Console.Clear();
            await TransferenciaHelper.TransferirRegistros(serviceProviderDestino, registros, tabelaOrigem, tabelaDestino, colunasOrigem, colunasDestino, bancoOrigem);

        }
        else
        {
            Console.WriteLine("Opção Inválida");
            return;
        }

    }
}

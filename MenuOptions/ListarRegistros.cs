using DBConnector.Functions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnector.MenuOptions;

internal class ListarRegistros
{
    public static async Task Executar(ServiceProvider? serviceProviderOrigem)
    {
        Console.WriteLine("\nDigite o nome do banco: ");
        string banco = Console.ReadLine()!.ToLower();

        serviceProviderOrigem = ConfiguracaoBanco.ConfigurarConexao(banco);
        if (serviceProviderOrigem == null && banco != "access")
        {
            Console.WriteLine("\nBanco não cadastrado");
            return;
        }

        var tabelas = await TabelasHelper.ListarTabelas(serviceProviderOrigem, banco);
        if (tabelas.Count == 0)
        {
            Console.WriteLine($"\nNenhuma tabela encontrada no banco {banco}.");
            return;
        }

        Console.Write("\nDigite o nome da tabela que deseja listar: ");
        string tabela = Console.ReadLine()!;
        var registros = await RegistrosHelper.ObterRegistros(serviceProviderOrigem, banco, tabela);
        ExibicaoHelper.ExibirRegistros(registros, tabela, banco);
    }
}

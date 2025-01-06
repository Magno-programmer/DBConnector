using DBConnector.Utils;
using DBConnector.Functions;
using Microsoft.Extensions.DependencyInjection;
using DBConnector.MenuOptions;

ServiceProvider serviceProviderOrigem = null;
ServiceProvider serviceProviderDestino = null;

while (true)
{
    try
    {
        Console.Clear();
        Console.WriteLine("Escolha uma opção:");
        Console.WriteLine("0 - Alterar a conexão/caminho do banco");
        Console.WriteLine("1 - Listar tabelas disponíveis no banco");
        Console.WriteLine("2 - Listar registros de uma tabela");
        Console.WriteLine("3 - Transferir registros entre bancos");
        Console.WriteLine("4 - Obter colunas de uma tabela");
        Console.WriteLine("5 - Remover registros de uma tabela");
        Console.WriteLine("6 - Sair");

        Console.Write("\nDigite aqui: ");
        string escolha = Console.ReadLine()!;
        switch (escolha)
        {
            case "0":
                AlterarConexao.Executar();
                break;
            case "1":
                await ListarTabelas.Executar(serviceProviderOrigem);
                break;
            case "2":
                await ListarRegistros.Executar(serviceProviderOrigem);
                break;
            case "3":
                await TransferirRegistros.Executar(serviceProviderOrigem, serviceProviderDestino);
                break;
            case "4":
                await ObterColunas.Executar(serviceProviderOrigem);
                break;
            case "5":
                await RemoverRegistros.Executar(serviceProviderOrigem);
                break;
            case "6":
                return;
            default:
                Console.WriteLine("Opção inválida.");
                break;
        }
        Voltar();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro: {ex.Message}");
    }
}
void Voltar()
{
    Console.WriteLine("\nPressione qualquer tecla para voltar ao menu");
    Console.ReadLine();
    Console.Clear();
}

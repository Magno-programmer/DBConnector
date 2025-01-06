using DBConnector.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnector.MenuOptions;

internal class AlterarConexao
{
    public static void Executar()
    {
        var currentSettings = AppConfiguration.LoadSettings();
        Console.Write("\nInforme o tipo de banco (Postgres/MySQL/SQLite/Access): ");
        string banco = Console.ReadLine()!.ToLower();

        if (banco == "postgres")
        {
            Console.Write("Informe o host (e.g., localhost): ");
            currentSettings.ConnectionsSetting.Postgres.Host = Console.ReadLine()!;
            Console.Write("Informe a porta (e.g., 5432): ");
            currentSettings.ConnectionsSetting.Postgres.Port = int.Parse(Console.ReadLine()!);
            Console.Write("Informe o nome do banco de dados: ");
            currentSettings.ConnectionsSetting.Postgres.Database = Console.ReadLine()!;
            Console.Write("Informe o usuário: ");
            currentSettings.ConnectionsSetting.Postgres.Username = Console.ReadLine()!;
            Console.Write("Informe a senha: ");
            currentSettings.ConnectionsSetting.Postgres.Password = Console.ReadLine()!;
        }
        else if (banco == "mysql")
        {
            Console.Write("Informe o server (e.g., localhost): ");
            currentSettings.ConnectionsSetting.MySQL.Server = Console.ReadLine()!;
            Console.Write("Informe o nome do banco de dados: ");
            currentSettings.ConnectionsSetting.MySQL.Database = Console.ReadLine()!;
            Console.Write("Informe o usuário: ");
            currentSettings.ConnectionsSetting.MySQL.User = Console.ReadLine()!;
            Console.Write("Informe a senha: ");
            currentSettings.ConnectionsSetting.MySQL.Password = Console.ReadLine()!;
        }
        else if (banco == "sqlite")
        {
            Console.Write("Informe o caminho do arquivo .db: ");
            currentSettings.ConnectionsSetting.SQLite.DataSource = Console.ReadLine()!;
        }
        else if (banco == "access")
        {
            Console.Write("Informe o caminho do arquivo .mdb: ");
            currentSettings.ConnectionsSetting.Access.DataSource = Console.ReadLine()!;
            Console.Write("Informe a senha do arquivo: ");
            currentSettings.ConnectionsSetting.Access.Password = Console.ReadLine()!;
        }
        else
        {
            Console.WriteLine("Banco não cadastrado");
            return;
        }

        AppConfiguration.SaveSettings(currentSettings);
        Console.WriteLine("\nConfigurações salvas com sucesso.");
    }
}

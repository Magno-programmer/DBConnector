using DBConnector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBConnector.Functions
{
    internal class ExibicaoHelper
    {
        // Exibir registros
        public static void ExibirRegistros(List<dynamic> registros, string tabela, string banco)
        {
            try
            {
                Console.WriteLine($"\nTabela '{tabela}' no banco {banco} contém {registros.Count} registros.");

                if (registros.Count == 0)
                {
                    Console.WriteLine("Nenhum registro encontrado.");
                    return;
                }

                foreach (var registro in registros)
                {
                    try
                    {
                        if (registro is IDictionary<string, object> propDictionary)
                        {
                            foreach (var prop in propDictionary)
                            {
                                Console.WriteLine($"{prop.Key}: {prop.Value}");
                            }
                        }
                        else if (registro is Atendente || registro is Consumidor)
                        {
                            StringBuilder escrita = new StringBuilder();
                            var tipo = registro.GetType();
                            escrita.AppendLine($"\nDados do {tipo.Name}:");

                            foreach (var propriedade in tipo.GetProperties())
                            {
                                var valor = propriedade.GetValue(registro);
                                escrita.AppendLine($"{propriedade.Name}: {valor}");
                            }

                            Console.WriteLine(escrita.ToString());
                        }
                        else
                        {
                            Console.WriteLine($"Registro inválido, não suportado ou não disponível para listagem: {registro}");
                        }
                        Console.WriteLine();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao exibir um registro: {ex.Message}");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao exibir registros da tabela '{tabela}' no banco {banco}: {ex.Message}");
            }
        }


        // Exibir coluna
        public static void ExibirColunas(List<string> colunas, string tabela, string banco)
        {
            try
            {
                Console.WriteLine($"\nTabela '{tabela}' no banco {banco} contém {colunas.Count} colunas.");

                if (colunas.Count == 0)
                {
                    Console.WriteLine("Nenhuma coluna encontrada.");
                    return;
                }

                foreach (var coluna in colunas)
                {
                    try
                    {
                        Console.WriteLine($"- {coluna}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao exibir a coluna '{coluna}': {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao exibir colunas da tabela '{tabela}' no banco {banco}: {ex.Message}");
            }
        }
    }
}

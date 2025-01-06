using DBConnector.Models;
using DBConnector.Utils;
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
    internal class TransferenciaHelper
    {
        public static async Task TransferirRegistros(
            ServiceProvider serviceProviderDestino,
            List<dynamic> registros,
            string tabelaOrigem,
            string tabelaDestino,
            List<string> colunasOrigem,
            List<string> colunasDestino,
            string bancoOrigem)
        {
            try
            {
                using (var scopeDestino = serviceProviderDestino.CreateScope())
                {

                    var contextDestino = scopeDestino.ServiceProvider.GetRequiredService<ApplicationDbContextDinamico>();

                    foreach (var registro in registros)
                    {
                        try
                        {
                            if (tabelaDestino.ToLower() == "tb_atendente" && registro is IDictionary<string, object> registroExpandoA)
                            {
                                // Converte dynamic para Atendente
                                Atendente atendente = MapearParaAtendente(registroExpandoA);

                                StringBuilder escritaAtendente = new StringBuilder();

                                escritaAtendente.AppendLine($"Documento: {atendente.nr_documento}");
                                escritaAtendente.AppendLine($"Nome: {atendente.nm_atendente}");
                                escritaAtendente.AppendLine($"Email: {atendente.ds_email}");
                                escritaAtendente.AppendLine($"Telefone: {atendente.nr_telefone}");
                                escritaAtendente.AppendLine($"Login: {atendente.ds_login}");
                                escritaAtendente.AppendLine($"senha: {atendente.ds_senha}");
                                escritaAtendente.AppendLine($"Suporte: {atendente.fl_suporte}");
                                escritaAtendente.AppendLine($"Bloqueio: {atendente.dt_bloqueio}");
                                Console.WriteLine(escritaAtendente.ToString());

                                contextDestino.Atendentes.Add(new Atendente
                                {
                                    nr_documento = atendente.nr_documento,
                                    nm_atendente = atendente.nm_atendente,
                                    ds_email = atendente.ds_email,
                                    nr_telefone = atendente.nr_telefone,
                                    ds_login = atendente.ds_login,
                                    ds_senha = atendente.ds_senha,
                                    fl_suporte = atendente.fl_suporte,
                                    dt_bloqueio = atendente.dt_bloqueio
                                });

                                // Salva todos os registros adicionados ao contexto
                                contextDestino.SaveChanges();
                                Console.WriteLine($"Registros transferidos para a tabela '{tabelaDestino}' com sucesso.");
                            }
                            else if (tabelaDestino.ToLower() == "tb_consumidor" && registro is IDictionary<string, object> registroExpandoC)
                            {
                                // Converte dynamic para Consumidor
                                Consumidor consumidor = MapearParaConsumidor(registroExpandoC);

                                bool teste = contextDestino.Consumidores.Any(c => c.nr_documento == consumidor.nr_documento || c.id_consumidor == consumidor.id_consumidor);

                                if (teste)
                                {
                                    Console.WriteLine($"Registro: id:{consumidor.id_consumidor}; documento: {consumidor.nr_documento}. Já existem!");
                                    continue;

                                }

                                StringBuilder escritaConsumidor = new StringBuilder();

                                escritaConsumidor.AppendLine($"Nome: {consumidor.nm_consumidor}");
                                escritaConsumidor.AppendLine($"Documento: {consumidor.nr_documento}");
                                escritaConsumidor.AppendLine($"Tipo de Documento: {consumidor.id_tipo_documento}");
                                escritaConsumidor.AppendLine($"Email: {consumidor.ds_email}");
                                escritaConsumidor.AppendLine($"Celular: {consumidor.nr_celular}");
                                escritaConsumidor.AppendLine($"CRM: {consumidor.fl_crm}");
                                escritaConsumidor.AppendLine($"SMS: {consumidor.fl_sms}");
                                escritaConsumidor.AppendLine($"Email Marketing: {consumidor.fl_email}");
                                escritaConsumidor.AppendLine($"CEP: {consumidor.nr_cep}");
                                escritaConsumidor.AppendLine($"Endereço: {consumidor.ds_endereco}");
                                escritaConsumidor.AppendLine($"Bairro: {consumidor.ds_bairro}");
                                escritaConsumidor.AppendLine($"Cidade: {consumidor.nm_cidade}");
                                escritaConsumidor.AppendLine($"UF: {consumidor.sg_uf}");
                                escritaConsumidor.AppendLine($"Dia de Aniversário: {consumidor.nr_dia_aniversario}");
                                escritaConsumidor.AppendLine($"Mês de Aniversário: {consumidor.nr_mes_aniversario}");

                                Console.WriteLine(escritaConsumidor.ToString());

                                contextDestino.Consumidores.Add(new Consumidor
                                {
                                    nm_consumidor = consumidor.nm_consumidor,
                                    nr_documento = consumidor.nr_documento,
                                    id_tipo_documento = consumidor.id_tipo_documento,
                                    ds_email = consumidor.ds_email,
                                    nr_celular = consumidor.nr_celular,
                                    fl_crm = consumidor.fl_crm,
                                    fl_sms = consumidor.fl_sms,
                                    fl_email = consumidor.fl_email,
                                    nr_cep = consumidor.nr_cep,
                                    ds_endereco = consumidor.ds_endereco,
                                    ds_bairro = consumidor.ds_bairro,
                                    nm_cidade = consumidor.nm_cidade,
                                    sg_uf = consumidor.sg_uf,
                                    nr_dia_aniversario = consumidor.nr_dia_aniversario,
                                    nr_mes_aniversario = consumidor.nr_mes_aniversario
                                });

                                // Salva todos os registros adicionados ao contexto
                                contextDestino.SaveChanges();
                                Console.WriteLine($"Registros transferidos para a tabela '{tabelaDestino}' com sucesso.");
                            }
                            else
                            {
                                Console.WriteLine($"Tabela de destino '{tabelaDestino}' não possui mapeamento dinâmico implementado.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erro ao processar registro da tabela '{tabelaOrigem}': {ex.Message}");
                            if (ex.InnerException != null)
                                Console.WriteLine($"Detalhes do erro: {ex.InnerException.Message}");
                        }
                    }

                }
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Erro de banco de dados durante a transferência para a tabela '{tabelaDestino}': {dbEx.Message}");
                if (dbEx.InnerException != null)
                {
                    Console.WriteLine($"Detalhes do erro: {dbEx.InnerException.Message}");
                }
            }
            catch (InvalidOperationException invEx)
            {
                Console.WriteLine($"Erro de configuração ou conexão com a tabela '{tabelaDestino}': {invEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado ao transferir registros para a tabela '{tabelaDestino}': {ex.Message}");
            }
        }

        public static async Task TransferirRegistroUnico(
            ServiceProvider serviceProviderDestino,
            dynamic registro,
            string tabelaOrigem,
            string tabelaDestino,
            List<string> colunasOrigem,
            List<string> colunasDestino,
            string bancoOrigem)
        {
            try
            {
                using var scopeDestino = serviceProviderDestino.CreateScope();
                var contextDestino = scopeDestino.ServiceProvider.GetRequiredService<ApplicationDbContextDinamico>();

                if (tabelaDestino.ToLower() == "tb_atendente" && registro is IDictionary<string, object> registroExpandoA)
                {
                    Atendente atendente = MapearParaAtendente(registroExpandoA);
                    contextDestino.Atendentes.Add(atendente);
                }
                else if (tabelaDestino.ToLower() == "tb_consumidor" && registro is IDictionary<string, object> registroExpandoC)
                {
                    Consumidor consumidor = MapearParaConsumidor(registroExpandoC);
                    contextDestino.Consumidores.Add(consumidor);
                }
                else
                {
                    Console.WriteLine($"Tabela de destino '{tabelaDestino}' não possui mapeamento dinâmico implementado.");
                    return;
                }

                await contextDestino.SaveChangesAsync();
                Console.WriteLine($"Registro transferido para a tabela '{tabelaDestino}' com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao transferir registro único para a tabela '{tabelaDestino}': {ex.Message}");
            }
        }


        // Método auxiliar para mapear ExpandoObject para Atendente
        internal static Atendente MapearParaAtendente(IDictionary<string, object> registro)
        {
            var atendente = new Atendente();
            try
            {

                if (registro.ContainsKey("NR_DOCUMENTO") && registro["NR_DOCUMENTO"] != null)
                    atendente.nr_documento = registro["NR_DOCUMENTO"].ToString();

                if (registro.ContainsKey("NM_ATENDENTE") && registro["NM_ATENDENTE"] != null)
                    atendente.nm_atendente = registro["NM_ATENDENTE"].ToString();

                if (registro.ContainsKey("DS_EMAIL") && registro["DS_EMAIL"] != null)
                    atendente.ds_email = registro["DS_EMAIL"].ToString();

                if (registro.ContainsKey("NR_TELEFONE") && registro["NR_TELEFONE"] != null)
                    atendente.nr_telefone = registro["NR_TELEFONE"].ToString();

                if (registro.ContainsKey("DS_LOGIN") && registro["DS_LOGIN"] != null)
                    atendente.ds_login = registro["DS_LOGIN"].ToString();

                if (registro.ContainsKey("DS_SENHA") && registro["DS_SENHA"] != null)
                    atendente.ds_senha = registro["DS_SENHA"].ToString();

                if (registro.ContainsKey("FL_SUPORTE") && registro["FL_SUPORTE"] != null)
                    atendente.fl_suporte = registro["FL_SUPORTE"].ToString();

                if (registro.ContainsKey("DT_BLOQUEIO") && registro["DT_BLOQUEIO"] != null)
                    atendente.dt_bloqueio = DateTime.TryParse(registro["DT_BLOQUEIO"].ToString(), out var dt) ? dt : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao mapear ExpandoObject para Atendente: {ex.Message}");
            }

            return atendente;
        }

        // Método auxiliar para mapear ExpandoObject para Consumidor
        internal static Consumidor MapearParaConsumidor(IDictionary<string, object> registro)
        {
            var consumidor = new Consumidor();
            try
            {

                if (registro.ContainsKey("NM_CONSUMIDOR") && registro["NM_CONSUMIDOR"] != null)
                    consumidor.nm_consumidor = registro["NM_CONSUMIDOR"].ToString();

                if (registro.ContainsKey("NR_DOCUMENTO") && registro["NR_DOCUMENTO"] != null)
                    consumidor.nr_documento = registro["NR_DOCUMENTO"].ToString();

                if (registro.ContainsKey("ID_TIPO_DOCUMENTO") && registro["ID_TIPO_DOCUMENTO"] != null)
                    consumidor.id_tipo_documento = Convert.ToInt32(registro["ID_TIPO_DOCUMENTO"]);

                if (registro.ContainsKey("DS_EMAIL") && registro["DS_EMAIL"] != null)
                    consumidor.ds_email = registro["DS_EMAIL"].ToString();

                if (registro.ContainsKey("NR_CELULAR") && registro["NR_CELULAR"] != null)
                    consumidor.nr_celular = registro["NR_CELULAR"].ToString();

                if (registro.ContainsKey("FL_CRM") && registro["FL_CRM"] != null)
                    consumidor.fl_crm = registro["FL_CRM"].ToString()?.FirstOrDefault();

                if (registro.ContainsKey("FL_SMS") && registro["FL_SMS"] != null)
                    consumidor.fl_sms = registro["FL_SMS"].ToString()?.FirstOrDefault();

                if (registro.ContainsKey("FL_EMAIL") && registro["FL_EMAIL"] != null)
                    consumidor.fl_email = registro["FL_EMAIL"].ToString()?.FirstOrDefault();

                if (registro.ContainsKey("NR_CEP") && registro["NR_CEP"] != null)
                    consumidor.nr_cep = registro["NR_CEP"].ToString();

                if (registro.ContainsKey("DS_ENDERECO") && registro["DS_ENDERECO"] != null)
                    consumidor.ds_endereco = registro["DS_ENDERECO"].ToString();

                if (registro.ContainsKey("DS_BAIRRO") && registro["DS_BAIRRO"] != null)
                    consumidor.ds_bairro = registro["DS_BAIRRO"].ToString();

                if (registro.ContainsKey("NM_CIDADE") && registro["NM_CIDADE"] != null)
                    consumidor.nm_cidade = registro["NM_CIDADE"].ToString();

                if (registro.ContainsKey("SG_UF") && registro["SG_UF"] != null)
                    consumidor.sg_uf = registro["SG_UF"].ToString();

                if (registro.ContainsKey("NR_DIA_ANIVERSARIO") && registro["NR_DIA_ANIVERSARIO"] != null)
                    consumidor.nr_dia_aniversario = Convert.ToInt32(registro["NR_DIA_ANIVERSARIO"]);

                if (registro.ContainsKey("NR_MES_ANIVERSARIO") && registro["NR_MES_ANIVERSARIO"] != null)
                    consumidor.nr_mes_aniversario = Convert.ToInt32(registro["NR_MES_ANIVERSARIO"]);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao mapear ExpandoObject para Consumidor: {ex.Message}");
            }

            return consumidor;
        }
    }
}

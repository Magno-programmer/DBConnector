using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnector.Models;

internal class Consumidor
{
    public int id_consumidor { get; set; } // Chave primária, se aplicável
    public string? nm_consumidor { get; set; } // Nome do consumidor
    public string nr_documento { get; set; } // Número do documento
    public int? id_tipo_documento { get; set; } // Tipo de documento, se aplicável
    public string? ds_email { get; set; } // Email do consumidor
    public string? nr_celular { get; set; } // Número do celular
    public char? fl_crm { get; set; } // Flag CRM (se aplicável)
    public char? fl_sms { get; set; } // Flag SMS
    public char? fl_email { get; set; } // Flag Email
    public string? nr_cep { get; set; } // CEP
    public string? ds_endereco { get; set; } // Endereço
    public string? ds_bairro { get; set; } // Bairro
    public string? nm_cidade { get; set; } // Cidade
    public string? sg_uf { get; set; } // UF
    public int? fl_sexo { get; set; } // sexualidade
    public int? nr_dia_aniversario { get; set; } // Dia de aniversário
    public int? nr_mes_aniversario { get; set; } // Mês de aniversário
}

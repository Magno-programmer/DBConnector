using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnector.Models;
public class Atendente
{
    public int id_atendente { get; set; }
    public string nr_documento { get; set; }
    public string? nm_atendente { get; set; }
    public string? ds_email { get; set; }
    public string? nr_telefone { get; set; }
    public string? ds_login { get; set; }
    public string ds_senha { get; set; }
    public string? fl_suporte { get; set; }
    public DateTime? dt_bloqueio { get; set; }
}

# Documentação

> Obs.: Estes documentos foram criados somente para fins de comparação entre os tipos de bancos SQL e NoSQL e eficiência entre os bancos Postgres, MySQL e SQLite, sem dados sensíveis da empresa para a qual foi destinado.

[Documentacao_tecnica_sql_nosql.pdf](https://github.com/user-attachments/files/18319190/Documentacao_tecnica_sql_nosql.pdf)

[Documentacao_tecnica_entre_bancos.pdf](https://github.com/user-attachments/files/18319194/Documentacao_tecnica_entre_bancos.pdf)

---

## Introdução

O projeto **DBConnector** é uma solução genérica desenvolvida em C# para facilitar a conexão e manipulação de múltiplos bancos de dados relacionais, como MySQL, PostgreSQL, SQLite e Microsoft Access. Este projeto tem como objetivo fornecer uma interface simples e flexível para interação com diferentes sistemas de gerenciamento de banco de dados (SGBDs), promovendo abstração e reaproveitamento de código.

---

## Objetivos

- **Abstração de Conexão:** Fornecer um único ponto de entrada para gerenciar conexões com diferentes SGBDs.
- **Flexibilidade:** Permitir manipulações de dados sem depender diretamente de um banco de dados específico.
- **Reutilização de Código:** Implementar soluções reutilizáveis para leitura, escrita e execução de comandos SQL.
- **Facilidade de Uso:** Simplificar a configuração e o uso por meio de interfaces claras e documentadas.

---

## Configuração do Ambiente

### Requisitos Pré-Instalação
- **SDK .NET**: Versão 5.0 ou superior.
- **Editor de Texto/IDE**: Visual Studio ou Visual Studio Code.
- **Bancos de Dados**: MySQL, PostgreSQL, SQLite e Microsoft Access devem estar instalados e configurados.

### Passos para Configuração
1. **Clonar o Repositório**
   ```bash
   git clone https://github.com/Magno-programmer/DBConnector.git
   ```
2. **Navegar para o Diretório do Projeto**
   ```bash
   cd DBConnector
   ```
3. **Restaurar Dependências**
   ```bash
   dotnet restore
   ```
4. **Configurar Strings de Conexão**
   - Edite o arquivo de configuração `appsettings.json` para incluir as strings de conexão dos bancos desejados.

5. **Compilar o Projeto**
   ```bash
   dotnet build
   ```

6. **Executar o Projeto**
   ```bash
   dotnet run
   ```

---

## Estrutura do Projeto

### Diretórios Principais

- **`Services/`**: Contém classes que gerenciam conexões e operações em diferentes bancos de dados.
- **`Models/`**: Inclui classes modelo que representam os dados manipulados.
- **`Utils/`**: Contém utilitários para manipulação de strings de conexão e validação de dados.

### Arquivos Chave
- **`DatabaseService.cs`**: Classe principal que abstrai as operações de conexão e manipulação de dados.
- **`ConnectionFactory.cs`**: Implementa o padrão Factory para criar instâncias de conexão com base no tipo de banco.
- **`appsettings.json`**: Arquivo de configuração para definir strings de conexão e outros parâmetros.

---

## Funcionalidades

### Abstração de Conexões
- Suporta conexão com múltiplos bancos de dados, como MySQL, PostgreSQL, SQLite e Microsoft Access.
- Utiliza o padrão Factory para criar conexões com base no tipo de banco especificado.

### Execução de Comandos SQL
- Permite a execução de comandos SQL dinâmicos, como consultas `SELECT`, `INSERT`, `UPDATE` e `DELETE`.
- Inclui métodos para execução direta de scripts SQL e armazenados.

### Leitura de Dados
- Recupera dados de tabelas e converte para objetos C# utilizando mapeamento dinâmico.
- Permite aplicação de filtros dinâmicos em consultas.

### Manipulação de Dados
- Suporte para inserção, atualização e remoção de registros em bancos de dados.
- Métodos padronizados para operações CRUD.

### Logs e Tratamento de Erros
- Gera logs detalhados para conexões e comandos executados.
- Inclui tratamento de exceções para falhas de conexão e erros de sintaxe SQL.

---

## Exemplos de Uso

1. **Conectar ao Banco de Dados**:
   ```csharp
   var connection = ConnectionFactory.CreateConnection("PostgreSQL", connectionString);
   connection.Open();
   ```

2. **Executar uma Consulta SQL**:
   ```csharp
   var result = DatabaseService.ExecuteQuery("SELECT * FROM tabela", connection);
   ```

3. **Inserir Dados em uma Tabela**:
   ```csharp
   DatabaseService.ExecuteCommand("INSERT INTO tabela (coluna1, coluna2) VALUES (@valor1, @valor2)", parameters, connection);
   ```

4. **Tratar Erros de Conexão**:
   ```csharp
   try
   {
       connection.Open();
   }
   catch (Exception ex)
   {
       Console.WriteLine("Erro ao conectar: " + ex.Message);
   }
   ```

---

## Possíveis Melhorias

1. **Segurança**:
   - Implementar criptografia para strings de conexão no arquivo de configuração.

2. **Testes Automatizados**:
   - Criar testes unitários para validar a integração com diferentes bancos de dados.

3. **Suporte Adicional**:
   - Expandir para incluir suporte a outros bancos de dados, como SQL Server e Oracle.

4. **Interface Gráfica**:
   - Desenvolver uma interface gráfica para facilitar a configuração e execução de comandos.

---

## Conclusão

O projeto **DBConnector** fornece uma base sólida para abstração e manipulação de múltiplos bancos de dados em C#. Sua estrutura escalável e flexível permite integrações rápidas e eficientes, sendo uma excelente escolha para aplicações que precisam lidar com diferentes SGBDs.

**Agradecimentos:** Este projeto foi desenvolvido com o objetivo de explorar a integração de bancos de dados e promover boas práticas de desenvolvimento.


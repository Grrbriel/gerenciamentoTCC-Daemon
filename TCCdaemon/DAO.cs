using System.Data;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;

public class DAO{
    readonly string _connectionString;
    private readonly ILogger<Worker> _logger;
    public DAO(ILogger<Worker> logger)
    {
        _logger = logger;
        _connectionString = "Server=192.168.3.78;Port=3307;Database=TCC;User=root;Password=gerenciamento";
    }

    public async Task<string> CreateAsync(string[]user)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            string query = "INSERT INTO TCC.Pessoa (Matricula, Nome, Senha, Funcao, Coordenador) VALUES (@Matricula, @Nome, @Senha, @Funcao, @Coordenador)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Matricula", user[1]);
                command.Parameters.AddWithValue("@Nome", user[2]);
                command.Parameters.AddWithValue("@Senha", user[3]);
                command.Parameters.AddWithValue("@Funcao", user[4]);
                command.Parameters.AddWithValue("@Coordenador", user[5]);

                try
                {
                    await command.ExecuteNonQueryAsync();
                    return "sucesso;sucesso no cadastro";
                }
                catch(Exception ex){
                    return "falha;"+ex.Message;
                }
            }
        }
    }

    // Read operation
    public async Task<bool> VerifyAsync(string matricula) //Verifica a existência do login no DB
    {
        var user = new Pessoa();
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            string query = "SELECT * FROM Pessoa WHERE Matricula = @Matricula LIMIT 1";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Matricula", matricula);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if(!reader.HasRows){
                        return true;
                    }else{
                        return false;
                    }
                }
            }
        }
    }

    public async Task<string> verifyLoginAsync(string matricula, string password){
        
        using (var connection = new MySqlConnection(_connectionString)){
            Pessoa p = new();
            
            await connection.OpenAsync();
            string query = "SELECT * FROM Pessoa WHERE Matricula = @Matricula AND Senha = @Senha LIMIT 1";
            try{
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Matricula", matricula);
                    command.Parameters.AddWithValue("@Senha", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if(!reader.HasRows){return "falha;usuario ou senha incorretos";}
                        
                        while (reader.Read()){
                            System.Console.WriteLine(reader["Matricula"]);
                            p.matricula = (string)reader["Matricula"];
                            p.nome = (string)reader["Nome"];
                            p.senha = (string)reader["Senha"];
                            p.coord = (Int16)reader["Coordenador"];
                            p.funcao = (Int16)reader["Funcao"];
                        }
                        return "sucesso;" + p.matricula + ";"+p.nome+";"+p.senha+";"+p.funcao+";"+p.coord;
                    }
                }
            }catch(Exception ex){
                return "falha;"+ex.Message;
            }
        }
    }

    public async Task<string> CreateEtapa(string[]etapa){
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            string query = "INSERT INTO TCC.Etapa (Codigo, DescEtapa, PrazoEntrega, PrazoRevisao) VALUES (@Codigo, @DescEtapa, @PrazoEntrega, @PrazoRevisao)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Codigo", etapa[1]);
                command.Parameters.AddWithValue("@DescEtapa", etapa[2]);
                command.Parameters.AddWithValue("@PrazoEntrega", etapa[3]);
                command.Parameters.AddWithValue("@PrazoRevisao", etapa[4]);
                try
                {
                    await command.ExecuteNonQueryAsync();
                    return "sucesso;sucesso no cadastro";
                }
                catch(Exception ex){
                    return "falha;"+ex.Message;
                }
            }
        }
    }

    public async Task<string> SearchPessoa(int funcao){
        string reply = "";

        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            var query = "SELECT Matricula,Nome FROM Pessoa WHERE Funcao = @Funcao";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Funcao", funcao);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reply += reader["Matricula"].ToString()+"|"+reader["Nome"].ToString()+":";
                        
                    }
                }
            }
        }
    return "sucesso;"+reply;
    }

    internal async Task<string> CreateBanca(string[] banca)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            string query = "INSERT INTO TCC.Banca (Aluno_TC, Orientador, Banca1, Banca2) VALUES (@Aluno_TC, @Orientador, @Banca1, @Banca2)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Aluno_TC", banca[1]);
                command.Parameters.AddWithValue("@Orientador", banca[2]);
                command.Parameters.AddWithValue("@Banca1", banca[3]);
                command.Parameters.AddWithValue("@Banca2", banca[4]);

                try
                {
                    await command.ExecuteNonQueryAsync();
                    return "sucesso;sucesso no registro da Banca";
                }
                catch(Exception ex){
                    return "falha;"+ex.Message;
                }
            }
        }
    }

    public async Task<string> SearchEtapas(){
        string reply = "sucesso;";
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            string query = "SELECT * FROM Etapa";
            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reply += reader["Codigo"].ToString()+"|"+reader["DescEtapa"].ToString()+"|"+reader["PrazoEntrega"]+"|"+reader["PrazoRevisao"]+"*";
                        
                    }
                }
            }
        }
        return reply;
    }

    public async Task<string> CreateTc(string[] tc){
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            string query = "INSERT INTO TCC.Tc (Aluno, Banca, Titulo, Etapa) VALUES (@Aluno, @Banca, @Titulo, @Etapa)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Aluno", tc[1]);
                command.Parameters.AddWithValue("@Banca", tc[1]);
                command.Parameters.AddWithValue("@Titulo", tc[2]);
                command.Parameters.AddWithValue("@Etapa", tc[3]);

                try
                {
                    await command.ExecuteNonQueryAsync();
                    return "sucesso;sucesso no registro do TC";
                }
                catch(Exception ex){
                    return "falha;"+ex.Message;
                }
            }
        }
    }

    public async Task<string> SearchTc(string aluno){
        string reply = "sucesso;";
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            string query = "select t.Aluno, t.Etapa, t.Titulo, t.DataEntrega, t.DataRevisao, e.PrazoEntrega FROM Tc t INNER JOIN Etapa e on(e.Codigo = t.Etapa) WHERE t.Aluno = @Aluno";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Aluno", aluno);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reply += reader["Aluno"].ToString()+"|"+reader["Etapa"].ToString()+"|"+reader["Titulo"].ToString()+"|"+reader["DataEntrega"]+"|"+reader["DataRevisao"]+"|"+reader["PrazoEntrega"]+"*";
                        
                    }
                }
            }
        }
        return reply;
    }
    
    public async Task<string> UploadArquivo(string[] tc){
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            string query = "UPDATE Tc SET Arquivo = @Arquivo WHERE Aluno = @Aluno AND Etapa = @Etapa";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Arquivo", tc[3]);
                command.Parameters.AddWithValue("@Aluno", tc[1]);
                command.Parameters.AddWithValue("@Etapa", tc[2]);

                try
                {
                    await command.ExecuteNonQueryAsync();
                    return "sucesso;sucesso no registro do arquivo";
                }
                catch(Exception ex){
                    return "falha;"+ex.Message;
                }
            }
        }
    }
}
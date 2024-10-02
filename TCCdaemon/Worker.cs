using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using NetMQ;
using NetMQ.Sockets;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly string _connectionString;

    private DAO dao;
    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        dao = new DAO(_logger);
        // Define your MySQL connection string (replace with your actual values)
        _connectionString = "Server=localhost;Database=geral;User=root;Password=gerenciamento;";
    
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var server = new ResponseSocket())
        {
            server.Bind("tcp://*:5555"); // Binding to all IP addresses on port 5555

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Waiting for message...");
                string message = server.ReceiveFrameString();
                _logger.LogInformation($"Received message: {message}");
                // Tratar mensagem recebida e enviar resposta de acordo
                var splitedMessage = message.Split(';');
                string reply = "";

                switch(splitedMessage[0]){
                    case "cadastrar":
                        if(await dao.VerifyAsync(splitedMessage[1])){
                            reply = await dao.CreateAsync(splitedMessage);
                        }else{
                            reply = "falha;usuario ja cadastrado";
                        }
                        break;

                    case "logar":
                        reply = await dao.verifyLoginAsync(splitedMessage[1], splitedMessage[2]);
                        break;
                    case "defEtapa":
                        reply = await dao.CreateEtapa(splitedMessage);
                        break;

                    case "SearchPessoa":
                        reply = await dao.SearchPessoa(int.Parse(splitedMessage[1]));
                        break;

                    case "defBanca":
                        reply = await dao.CreateBanca(splitedMessage);
                        break;

                    case "SearchEtapa":
                        reply = await dao.SearchEtapas();
                        break;

                    case "defTc":
                        reply = await dao.CreateTc(splitedMessage);
                        break;
                        
                    case "SearchTc":
                        reply = await dao.SearchTc(splitedMessage[1]);
                        break;
                    
                    case "UploadArquivo":
                        reply = await dao.UploadArquivo(splitedMessage);
                        break;

                    default:
                        reply = "Mensagem fora do padrão";
                        break;
                }

                // Send a reply back
                server.SendFrame(reply);
            }
        }
    }

    public string replyConstructor(string message){

        return "Mensagem recebida";
    }

    // Create operation
    public async Task CreateAsync(string name, int age)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            string query = "INSERT INTO users (Name, Age) VALUES (@Name, @Age)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Age", age);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}

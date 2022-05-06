using Mensageira.Model;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Mensageira.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MensageriaController : ControllerBase
    {
        private const string QUEUE_NAME = "Mensagens";
        private readonly ConnectionFactory _connectionFactory;
        public MensageriaController()
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
            };
        }

        [HttpPost]
        public IActionResult Send([FromBody] MessageDTO model)
        {
            // 1 - Criando a conexão
            using (var connection = _connectionFactory.CreateConnection())
            {
                // 2 - Criando um canal dessa conexão

                using (var channel = connection.CreateModel())
                {
                    // 3 - Declarando a fila caso ela não exista

                    // 4 - Os parâmetros da fila são
                    channel.QueueDeclare(

                            // 1 - Nome da fila
                            queue: QUEUE_NAME,

                            // 2 - Dizendo que ela não será uma fila durável, ou seja, caso haja uma reinicialização no rabbitmq a fila será apagada!
                            durable: false,

                            // 3-  Ela não exige apenas uma conexão, ou seja, permite várias conexões
                            exclusive: false,

                            // 4 - Se as pessoas que consumem essa fila pararem de usa-las ela sera apagada. Como está como false, ela NÃO será apagada
                            autoDelete: false,

                            arguments: null
                        );

                    // Formatando os dados para que eles possam ser enviados corretamente para fila

                      // 5 - Serializando o model para uma string
                    var messageString = JsonSerializer.Serialize(model);

                     // 6 - Transformando a string em um array de bytes
                    var byteArray = Encoding.UTF8.GetBytes(messageString);

                    // 7 - Canal de publicação para envio da mensagem formatada para a fila

                    channel.BasicPublish(

                        // 1 - Enviando com um campo vazio o parâmetro utilizará o metodo default do rabbitmq
                        exchange: "",

                        // 2 - O Exchange precisa saber pra qual fila ele vai, ou seja, ele irá pra fila que tem a chave da rota com o nome da fila 
                        routingKey: QUEUE_NAME,

                        // 3 - 
                        basicProperties: null,

                        // 4 - Conteudo da mensangem que foi convertido em array de bytes
                        body: byteArray
                        );
                }
            }
            return Accepted();
        }
    }
}

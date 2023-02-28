using System.Text;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using Spectre.Console;
using Spectre.Console.Json;

static class Program
{
    private static async Task Main()
    {
        // MQTT address server
        var mqttAddress = "192.168.1.201";
        // MQTT user
        var mqttBrokerUser = "mqtt";
        //MQTT password
        string mqttBrockerPassword = "mqtt";
        // MQTT topic for subscribe
        var topic = "zigbee2mqtt/Cube";
        
        var mqttFactory = new MqttFactory();
        
        using (var mqttClient = mqttFactory.CreateMqttClient())
        {
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithCredentials(mqttBrokerUser, mqttBrockerPassword)
                .WithTcpServer(mqttAddress)
                .Build();

            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                var jsonResult = Deserialize(e.ApplicationMessage.Payload);
                AnsiConsole.Write(new JsonText(jsonResult.ToString())
                    .BracesColor(Color.Red)
                    .BracketColor(Color.Green)
                    .ColonColor(Color.Blue)
                    .CommaColor(Color.Red)
                    .StringColor(Color.Green)
                    .NumberColor(Color.Blue)
                    .BooleanColor(Color.Red)
                    .NullColor(Color.Green));
                return Task.CompletedTask;
            };
            
            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            AnsiConsole.MarkupLine($"[blue]MQTT address:[/] [green]{mqttAddress}[/]");
            AnsiConsole.MarkupLine($"[blue]MQTT topic:[/] [green]{topic}[/]");
            AnsiConsole.MarkupLine($"[green]MQTT client subscribed to topic.[/]");

            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f => { f.WithTopic(topic); })
                .Build();
            await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
            Console.ReadLine();
        }
    }
    private static object Deserialize(byte[] buffer)
    {
        return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(buffer));
    }
}
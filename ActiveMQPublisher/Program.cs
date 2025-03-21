using Apache.NMS;
using Apache.NMS.ActiveMQ;

class Program
{
    static void Main()
    {
        var factory = new ConnectionFactory("tcp://localhost:61616");
        using IConnection connection = factory.CreateConnection();
        using ISession session = connection.CreateSession();
        IDestination queue = session.GetQueue("messages.queue");
        using IMessageProducer producer = session.CreateProducer(queue);

        connection.Start();

        Console.WriteLine("Enter message to send. Type 'exit' to quit.");
        while (true)
        {
            string text = Console.ReadLine();
            if (text == "exit")
            {
                break;
            }

            ITextMessage message = session.CreateTextMessage(text);
            producer.Send(message);
            Console.WriteLine("Sent message: " + text);
        }
    }
}

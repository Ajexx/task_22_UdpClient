using System.Net;
using System.Net.Sockets;
using System.Text;

int port = 5000;

IPAddress multicastAddress = IPAddress.Parse("239.1.1.1");

using UdpClient udpClient = new();

Console.Write("Enter your name: ");
string? name = (Console.ReadLine() ?? "").Trim();
while (name.Length == 0)
{
    Console.WriteLine("Name cannot be empty. Please enter a name");
    name = (Console.ReadLine() ?? "").Trim();
}
name = name.Replace('|', '_');

while (true)
{
    Console.Write("Message, /help — list of commands, exit — exit: ");

    string? message = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(message))
    {
        continue;
    }

    if (message == "exit")
    {
        break;
    }

    if (message.StartsWith('/'))
    {
        string[] parts = message.Split(' ', 2);

        string comand = parts[0].ToLower();

        switch (comand)
        {
            case "/help":
                Console.WriteLine();
                Console.WriteLine("Comands: ");
                Console.WriteLine("/help         - list of commands");
                Console.WriteLine("/time         - current local time");
                Console.WriteLine("/clear        - clear console");
                Console.WriteLine("/name newName - change name");
                Console.WriteLine();
                break;
            case "/time":
                var date = DateTime.Now.ToString("HH:mm:ss");
                Console.WriteLine(date);
                break;
            case "/clear":
                Console.Clear();
                break;
            case "/name":
                if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
                {
                    Console.WriteLine("Incorrect input. Correct example: /name newName");
                }
                else
                {
                    name = parts[1].Trim().Replace('|', '_');
                    Console.WriteLine($"Name changed to {name}");
                }
                break;

            default:
                Console.WriteLine("Unknown command. Enter /help.");
                break;
        }
        continue;
    }

    try
    {
        string? text = $"{DateTime.Now}|{name}|{message}";
        byte[] data = Encoding.UTF8.GetBytes(text);

        await udpClient.SendAsync(data, data.Length, new IPEndPoint(multicastAddress, port));
        Console.WriteLine("Message sent");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Send Error: {ex}");
    }
}

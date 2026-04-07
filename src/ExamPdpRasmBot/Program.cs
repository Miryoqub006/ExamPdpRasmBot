using ExamPdpRasmBot.Service;
class Program
{
    static void Main()
    {
        var bot = new BotService("8735654302:AAGxva81eOacJmKfp0QN3EJ3oIHJpI38_tw");
        bot.Start();
    }
}
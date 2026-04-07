using ExamPdpRasmBot.Data;
using ExamPdpRasmBot.Model;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ExamPdpRasmBot.Service;

public class BotService
{
    private readonly ITelegramBotClient _botClient;

    public BotService(string token)
    {
        _botClient = new TelegramBotClient(token);
    }

    public void Start()
    {
        _botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, new ReceiverOptions());
        Console.WriteLine(" STOP ");
        Console.ReadLine();
    }

    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        if (update.Message is not { } message) return;
        var chatId = message.Chat.Id;

        
        var user = UserDatas.GetUsers().FirstOrDefault(u => u.ChatId == chatId);

        
        if (user == null || !user.IsAuthenticated)
        {
            if (message.Type == MessageType.Contact && message.Contact != null)
            {
                var newUser = new UserModel 
                { ChatId = chatId,
                    PhoneNumber = message.Contact.PhoneNumber,
                    IsAuthenticated = true 
                };
                UserDatas.SaveUser(newUser);
                await botClient.SendMessage(
                    chatId, 
                    "Ro'yxatdan o'tdingiz! Endi botdan foydalanishingiz mumkin.", 
                    replyMarkup: 
                    new ReplyKeyboardRemove());
            }
            else
            {
                var btn = new KeyboardButton("Raqamni yuborish") 
                {
                    RequestContact = true 
                };
                await botClient.SendMessage(chatId, 
                    "Iltimos, avval telefon raqamingizni yuboring:",
                    replyMarkup: 
                    new ReplyKeyboardMarkup(btn) 
                    {
                        ResizeKeyboard = true
                    });
            }
            return;
        }

        
        await GenerateImageAsync(message, user);
    }


    private async Task GenerateImageAsync(Message message, UserModel user)
    {
        var text = message.Text;

        if (text == "/start")
        {
            await _botClient.SendMessage(user.ChatId, $"Xush kelibsiz! Raqamingiz: {user.PhoneNumber}\nRasm qidiruv uchun istalgan so'zni yozib yuboring.");
        }
        
        else if (!string.IsNullOrWhiteSpace(text) && !text.StartsWith("/"))
        {
            
            await _botClient.SendMessage(user.ChatId, "Rasmlar qidirilmoqda...");

            var images = await UserDatas.GetImagesByKeyword(text);

            if (images.Count == 0)
            {
                await _botClient.SendMessage(user.ChatId, $" \"{text}\" uchun rasm topilmadi.");
            }
            else
            {
                var mediaGroup = images.Select(url => new InputMediaPhoto(InputFile.FromUri(url))).ToList();
                await _botClient.SendMediaGroup(user.ChatId, mediaGroup);
            }
        }
        else
        {
            await _botClient.SendMessage(user.ChatId, "Siz yuborgan xabar: " + text);
        }
    }

    Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken ct)
    {
        Console.WriteLine("Xatolik: " + exception.Message);
        return Task.CompletedTask;
    }
}

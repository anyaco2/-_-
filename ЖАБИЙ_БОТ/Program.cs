using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using ЖАБИЙ_БОТ;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Args;
using System.Text.Json;
using System.Timers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;


namespace Toad_Bot
{
    
    class Program
    {
        private static string photoFolderPath = @"C:\Users\Anya\Documents\ЖАБИЙ_БОТ\ЖАБИЙ_БОТ\ЖАБКИ";
        private static bool showMenu = true;
        private static Dictionary<long, System.Timers.Timer> userTimers = new Dictionary<long, System.Timers.Timer>();
        static void Main(string[] args)
        {
            Host g4bot = new Host("7959356932:AAF2EAVJk2Ur-ewQZoY2uRLc8v4HvBI54Yw");
            g4bot.Start();
            g4bot.OnMessage += OnMessage;
            g4bot.OnCallbackQuery += OnCallbackQuery;
            Console.ReadLine();
        }

        private static async void OnMessage(ITelegramBotClient client, Update update)
        {
            if (update.Message != null && update.Message.Text != null)
            {
                Console.WriteLine($"{update.Message.Chat.Username ?? "anon"} | {update.Message.Text}");
                if (update.Message.Text.ToLower().Contains("/start"))
                {
                    await client.SendTextMessageAsync(update.Message.Chat.Id, "Приветствую, любитель жабок");
                    var menu = new InlineKeyboardMarkup(new[] {
                        new[] {
                            InlineKeyboardButton.WithCallbackData("Получить жабку сейчас"),
                            InlineKeyboardButton.WithCallbackData("Получать жабку каждую среду"),
                        },
                    });
                    await client.SendTextMessageAsync(update.Message.Chat.Id, "Что ты хочешь сделать?", replyMarkup: menu);
                    return;
                }
            }
        }
        //private static async void OnCallbackQuery(ITelegramBotClient client, CallbackQuery callbackQuery)
        //{
        //    Console.WriteLine($"CallbackQuery: {callbackQuery.Data}");

        //    string imageUrl = string.Empty;

        //    if (callbackQuery.Data == "Веселая")
        //    {
        //        await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Вот тебе веселая жабка!");
        //        imageUrl = await GetPinterestImage("Мем с веселой жабкой");
        //    }
        //    else if (callbackQuery.Data == "Милая")
        //    {
        //        await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Вот тебе милая жабка!");
        //        imageUrl = await GetPinterestImage("Милая жабка");
        //    }

        //    if (!string.IsNullOrEmpty(imageUrl))
        //    {
        //        await client.SendPhotoAsync(callbackQuery.Message.Chat.Id, imageUrl);
        //    }
        //    else
        //    {
        //        await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Изображение не найдено.");
        //    }
        //    await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Что еще ты хочешь?", replyMarkup: null);
        //    await client.DeleteMessageAsync(chatId: callbackQuery.Message.Chat.Id, messageId: callbackQuery.Message.MessageId);
        //}

        //private static async Task<string> GetPinterestImage(string query)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Add("Authorization", "Bearer 7607d3ccccf3b3a995a0e179f89f056fab6ce4d4");

        //        var response = await client.GetStringAsync($"https://api.pinterest.com/v1/search/pins/?query={query}&access_token=7607d3ccccf3b3a995a0e179f89f056fab6ce4d4");
        //        var json = JObject.Parse(response);
        //        var imageUrl = json["data"]?[0]?["image"]?["original"]?["url"]?.ToString();

        //        return imageUrl;
        //    }
        //}
        private static async void OnCallbackQuery(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            string responseMessage = "";
            string randomImagePath = GetRandomImagePath();
            long chatId = callbackQuery.Message.Chat.Id;
            Console.WriteLine($"CallbackQuery: {callbackQuery.Data}");

            if (callbackQuery.Data == "Получить жабку сейчас")
            {
                await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Держи жабу!");
                if (!string.IsNullOrEmpty(randomImagePath))
                {
                    using (var stream = new FileStream(randomImagePath, FileMode.Open, FileAccess.Read))
                    {
                        await client.SendPhotoAsync(callbackQuery.Message.Chat.Id, stream);
                    }
                }
            }
            else if (callbackQuery.Data == "Получать жабку каждую среду")
            {
                if (userTimers.ContainsKey(chatId))
                {
                    await client.SendTextMessageAsync(chatId, "Ты уже подписан на получение жабок каждую среду!");
                    return;
                }
                
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Interval = 60000;
                timer.Elapsed += async (sender, e) =>
                {
                    if (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday && DateTime.Now.Hour == 9 && DateTime.Now.Minute == 0)
                    {
                        string randomImagePath = GetRandomImagePath();
                        await client.SendPhotoAsync(chatId, randomImagePath);
                        await client.SendTextMessageAsync(chatId, "Вот тебе веселая жабка!");
                    }
                };
                timer.Start();
                userTimers[chatId] = timer;
                await client.SendTextMessageAsync(chatId, "Отлично! Каждую среду в 9:00 ты будешь получать жабку!");

            }

            else if(callbackQuery.Data == "Отписаться от жабок")
            {
                if (userTimers.ContainsKey(chatId))
                { 
                    System.Timers.Timer timer = userTimers[chatId];
                    timer.Stop();
                    userTimers.Remove(chatId);

                    await client.SendTextMessageAsync(chatId, "Ты успешно отписался от получения жабок по средам!");
                }
                else
                {
                    await client.SendTextMessageAsync(chatId, "Ты еще не подписан на получение жабок по средам!");
                }
            }


            await client.DeleteMessageAsync(chatId: callbackQuery.Message.Chat.Id, messageId: callbackQuery.Message.MessageId);
            if (showMenu)
            {
                if (userTimers.ContainsKey(chatId))
                {
                    var menu = new InlineKeyboardMarkup(new[]
                    {
                     new[]
                     {
                       InlineKeyboardButton.WithCallbackData("Получить жабку сейчас"),
                       InlineKeyboardButton.WithCallbackData("Отписаться от жабок"),
                     },
                   });
                    await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Что еще ты хочешь?", replyMarkup: menu);
                }
                else
                {
                    var menu = new InlineKeyboardMarkup(new[]
                    {
                     new[]
                     {
                       InlineKeyboardButton.WithCallbackData("Получить жабку сейчас"),
                       InlineKeyboardButton.WithCallbackData("Получать жабку каждую среду"),
                     },
                   });
                    await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Что еще ты хочешь?", replyMarkup: menu);
                }
                
                
            }

        }

        private static string GetRandomImagePath()
        {
            var images = Directory.GetFiles(photoFolderPath, "*.*")
                                  .Where(f => f.EndsWith(".jpg") || f.EndsWith(".jpeg") || f.EndsWith(".png") || f.EndsWith(".jfif"))
                                  .ToArray();

            if (images.Length == 0)
                return null;

            Random random = new Random();
            return images[random.Next(images.Length)];
        }



       
    }
}
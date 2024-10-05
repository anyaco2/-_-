using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using System.Timers;


namespace ЖАБИЙ_БОТ
{
    internal class Host
    {

        public Action<ITelegramBotClient, Update>? OnMessage;
        public Action<ITelegramBotClient, CallbackQuery> OnCallbackQuery;
        private TelegramBotClient _bot;
        

        public Host(string token)
        {
            _bot = new TelegramBotClient(token);
            
        }

        public async void Start()
        {
            
            _bot.StartReceiving(UpdateHandler,ErrorHandler);
            Console.WriteLine("Start");
        }

        private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if (update.Message != null && update.Message.Text != null)
            {
                OnMessage.Invoke(client, update);
                return;
            }
            else if (update.CallbackQuery != null)
            {
                OnCallbackQuery.Invoke(client, update.CallbackQuery);
                return;
            }

            await Task.CompletedTask;
        }

        private async Task ErrorHandler(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
        {
            Console.WriteLine("Error: " + exception.Message);
            await Task.CompletedTask;
        }
    }
}

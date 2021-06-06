using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFInal.Clases
{
    class clsRelizacion
    {
        private static TelegramBotClient Botlgrm;//Permite la realización del bot
        public async Task InicioTlgrm()//Inicia el ejemplo de tipo asincrono
        {

            Botlgrm = new TelegramBotClient("1882978954:AAG2ZN_NSefMiMitVfsdXANL7Gk59JRlbGs");//Token del bot


            var me = await Botlgrm.GetMeAsync();//Variable texto que obtiene los metodos asincronos
            Console.Title = me.Username;//Aparezca como titulo de la consola el usuario asignado


            //Reciba las opciones requeridas
            Botlgrm.OnMessage += BotOnMessageReceived;//En la variablte bottlgrm al momento de la entrada de un mensaje llame a los demas bloques
            Botlgrm.OnMessageEdited += BotOnMessageReceived;
            Botlgrm.OnCallbackQuery += BotOnCallbackQueryReceived;
            Botlgrm.OnInlineQuery += BotOnInlineQueryReceived;
            Botlgrm.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            Botlgrm.OnReceiveError += BotOnReceiveError;

            Botlgrm.StartReceiving(Array.Empty<UpdateType>());//Comienze la actualizacion con metodo array de tipo de actualizacion
            Console.WriteLine($"Comenzar @{me.Username}");

            Console.ReadLine();
            Botlgrm.StopReceiving();//Para la actualizacion
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;//Mensaque que nos está enviando con la variable message
            if (message == null || message.Type != MessageType.Text)//SI en dado caso es nulo y diferente a mensaje que retorne la rsp.
                return;

            switch (message.Text.Split(' ').First())//Devuelve el primer arreglo convertido en split
            {
                //Si el usuario manda un inline:
                case "/inline":
                    await SendInlineKeyboard(message);//mande a llamar al sendilinekeyboard
                    break;

                //Si el usuario manda un keyboard
                case "/keyboard":
                    await SendReplyKeyboard(message);//lllame a sendreplykeyboard
                    break;

                //Si el ususario pida la opcion foto
                case "/photo":
                    await SendDocument(message);//llame a sendDocument
                    break;

                //Si el usuario quiera enviar su ubicacion y contacto
                case "/request":
                    await RequestContactAndLocation(message);//llame a rquestcontactandlocation
                    break;

                default:
                    await Usage(message);//Si en dado caso coloca algo diferente a todo lo anterior, mande siempe el usage
                    break;
            }

            // comenzamos el teclado en línea
            //Puede procesar respuestas en el controlador BotOnCallbackQueryReceived
            static async Task SendInlineKeyboard(Message message)
            {
                await Botlgrm.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

             
                await Task.Delay(500);//Pausa para contestar

                var inlineKeyboard = new InlineKeyboardMarkup(new[]//Creamos el inlinekeyboard con el tipo unico de telegram
                {
                    // Primera fila de botones en linea
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(mdemojis.Ghost+"Ciencia Ficcion", "Juego de Ender"),
                        InlineKeyboardButton.WithCallbackData(mdemojis.Heavy_Division_Sign+"Matematica", "Matemática Preuniversitaria"),

                    },
                    // Segunda fila de botones en linea
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(mdemojis.Baby+"Infantil", "La oruga glotona"),
                        InlineKeyboardButton.WithCallbackData(mdemojis.Swimmer+"Deportes", "Natación para la vida"),
                    },
                    // Tercera fila de botones en linea
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(mdemojis.Earth_Africa+"Biología", "Clasificación de los seres vivos"),
                        InlineKeyboardButton.WithCallbackData(mdemojis.Statue_Of_Liberty+"Historia", "Guatemala Independiente"),
                    }


                });
                await Botlgrm.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Selecciona la categoría",
                    replyMarkup: inlineKeyboard
                );
            }
            //Comenzamos el otro telcado
            static async Task SendReplyKeyboard(Message message)
            {
                var replyKeyboardMarkup = new ReplyKeyboardMarkup(
                    new KeyboardButton[][]
                    {
                        new KeyboardButton[] { "Califica:👍😉" },
                        new KeyboardButton[] { "Califica:👎😖" },
                    },
                    resizeKeyboard: true
                );

                await Botlgrm.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Escoger",
                    replyMarkup: replyKeyboardMarkup

                );
            }
            //Proceso de la foto
            static async Task SendDocument(Message message)
            {
                await Botlgrm.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                const string Ruta = @"C:\Users\HP\Desktop\PROGRA I\Boleta.png";//La ruta de la imagen que será enviada
                using var archivo = new FileStream(Ruta, FileMode.Open, FileAccess.Read, FileShare.Read);
                var Nombrearchivo = Ruta.Split(Path.DirectorySeparatorChar).Last();
                await Botlgrm.SendPhotoAsync(
                    chatId: message.Chat.Id,
                    photo: new InputOnlineFile(archivo, Nombrearchivo),
                    caption: "Realiza la transferencia ahora"
                );
            }
            //Proceso de la ubicación y contacto
            static async Task RequestContactAndLocation(Message message)
            {
                var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]
                {
                    KeyboardButton.WithRequestLocation("📍Enviar Ubicación📍"),
                    KeyboardButton.WithRequestContact("📲Compartir mi número📲"),
                });
                await Botlgrm.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Proceder",
                    replyMarkup: RequestReplyKeyboard
                );
            }
            //Vista del usage
            static async Task Usage(Message message)
            {
                const string usage = "Usage:\n" +
                                        "/inline        ✔Pulsa aquí para ver la lista de libros\n" +
                                        "/keyboard      ✔Calificar el servicio\n" +
                                        "/photo         ✔Visualizar boletas de pago\n" +
                                        "/request       ✔Envia tu contacto o ubicacion para tomar tus datos";
                await Botlgrm.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: usage,
                    replyMarkup: new ReplyKeyboardRemove()
                );
            }
        }

        // Procesa los datos de devolución de llamada del inlinekeybard
        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            await Botlgrm.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"Libro Disponible: {callbackQuery.Data}"
            );
            
            
            await Botlgrm.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
        
               text: $"Libro Disponible: {callbackQuery.Data}"
            );
        }

        #region Inline Mode

        private static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            Console.WriteLine($"Received inline query from: {inlineQueryEventArgs.InlineQuery.From.Id}");

            InlineQueryResultBase[] results = {
                // displayed result
                new InlineQueryResultArticle(
                    id: "3",
                    title: "TgBots",
                    inputMessageContent: new InputTextMessageContent(
                        "hello"
                    )
                )
            };
            await Botlgrm.AnswerInlineQueryAsync(
                inlineQueryId: inlineQueryEventArgs.InlineQuery.Id,
                results: results,
                isPersonal: true,
                cacheTime: 0
            );
        }

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
        }

        #endregion

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("ERROR: {0} — {1}",//Marca el error y que tipo de error
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message
            );
        }


    }//fin de la clase
}

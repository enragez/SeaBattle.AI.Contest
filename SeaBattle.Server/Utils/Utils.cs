namespace SeaBattle.Server
{
    using System.Linq;
    using Entities;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public static class Utils
    {
        internal const string BotUserName = "SeaBattleAIContestBot";

        internal static bool DebugMode;

        internal static string CurrentApplicationUrl;

        internal static bool IsCommand(Update update)
        {
            return update.Message?.Entities?.Any(ent => ent?.Type == MessageEntityType.BotCommand) ?? false;
        }

        internal static string GetCommandName(Update update)
        {
            var cmdText = update.Message.EntityValues.FirstOrDefault();

            return cmdText?.Replace($"@{BotUserName}", string.Empty);
        }
        
        internal static string GetCommandArgument(Update update)
        {
            var entityValue = update.Message.EntityValues.FirstOrDefault();

            return update.Message.Text.Replace(entityValue, string.Empty);
        }

        internal static bool IsUsernameValid(string userName, out string validationMessage)
        {
            validationMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(userName))
            {
                validationMessage = "Имя не может быть пустым или состоять из пробелов";
                return false;
            }

            return true;
        }
        
        internal static string GetGameUrl(PlayedGame game)
        {
            return $"{CurrentApplicationUrl}game/get/{game.Id}";
        }
    }
}
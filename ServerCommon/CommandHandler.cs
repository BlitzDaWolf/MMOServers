using NetCommen;

namespace ServerCommon
{
    public delegate void ExecuteCommand(int clientId, string[] Arguments);
    public delegate bool ValidateUser(int userID, int clientId);

    public static class CommandHandler
    {
        /// <summary>
        /// Execute a admin command
        /// Used to controll the server
        /// </summary>
        /// <param name="clientId">Id of the client their socket</param>
        /// <param name="cmd">Command to execute</param>
        public static void ExecuteAdminCommand(int clientId, AdminCommands cmd)
        {
            if (!UserValidation(cmd.UserId, clientId))
                return;

            if (Commands.ContainsKey(cmd.CommandId))
            {
                AdminCommands[cmd.CommandId](clientId, cmd.Arguments);
            }
        }

        /// <summary>
        /// Execute normal command that all users can use
        /// </summary>
        /// <param name="clientId">Id of the client their socket</param>
        /// <param name="cmd">Command to execute</param>
        public static void ExecuteCommand(int clientId, NetworkCommand cmd)
        {
            if (Commands.ContainsKey(cmd.CommandId))
            {
                Commands[cmd.CommandId](clientId, cmd.Arguments);
            }
        }

        public static ValidateUser UserValidation = (int userID, int clientId) => false;

        public static Dictionary<int, ExecuteCommand> Commands = new Dictionary<int, ExecuteCommand>();
        public static Dictionary<int, ExecuteCommand> AdminCommands = new Dictionary<int, ExecuteCommand>();
    }
}

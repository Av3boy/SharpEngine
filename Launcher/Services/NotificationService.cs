using Launcher.UI;
using Serilog;
using Serilog.Core;

namespace Launcher.Services
{
    /// <summary>
    ///     Contains definitions for displaying notifications in the UI.
    /// </summary>
    public interface INotificationService
    {
        void Discard(string message);

        /// <summary>
        ///     Displays a notification message in the UI.
        /// </summary>
        /// <param name="message"></param>
        void Show(string message, bool writeToFile = false, params object?[] details);
    }

    /// <summary>
    ///     Handles displaying notifications in the UI.
    /// </summary>
    public class NotificationService : INotificationService
    {
        // TODO: The notification messages should be added into a separate service.
        private Dictionary<string, int> _notificationMessages = new()
        {
            { "some message", 2 },
            { "another", 2 },
        };

        private Logger _log;

        public NotificationService()
        {
            _log = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        /// <inheritdoc />
        public void Show(string message, bool writeToFile = false, params object?[] details)
        {
            if (writeToFile)
                _log.Information(message, details);

            if (_notificationMessages.TryGetValue(message, out int count))
                _notificationMessages[message] = count + 1;
            else
                _notificationMessages.Add(message, 1);
        }

        /// <inheritdoc />
        public void Discard(string message)
        {
            if (!_notificationMessages.TryGetValue(message, out int amount) || amount < 1)
                return;
 
            if (amount == 1)
                _notificationMessages.Remove(message);
            else
                _notificationMessages[message] = amount - 1;
        }
    }
}

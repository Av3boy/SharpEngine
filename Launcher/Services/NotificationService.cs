namespace Launcher.Services
{
    /// <summary>
    ///     Contains definitions for displaying notifications in the UI.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        ///     Displays a notification message in the UI.
        /// </summary>
        /// <param name="message"></param>
        void Show(string message);
    }

    /// <summary>
    ///     Handles displaying notifications in the UI.
    /// </summary>
    public class NotificationService : INotificationService
    {
        // TODO: Dictionary to store the notification messages

        /// <inheritdoc />
        public void Show(string message)
        {

        }
    }
}

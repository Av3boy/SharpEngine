﻿using Serilog;
using Serilog.Core;

namespace Launcher.Services
{
    /// <summary>
    ///     Contains definitions for displaying notifications in the UI.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>Gets or sets the notifications that are displayed in the UI.</summary>
        Dictionary<string, int> Notifications { get; set; }

        /// <summary>Sets the event executed when the notifications are changed.</summary>
        void SetOnNotificationsChanged(Action action);

        /// <summary>
        ///     Sets the action to invoke functions asynchronously.
        /// </summary>
        /// <param name="action">The action for asynchronous invocation.</param>
        void SetInvokeAsync(Func<Func<Task>, Task> action);

        /// <summary>
        ///     Discards a notification message from the UI.
        /// </summary>
        /// <param name="message">The message to be discarded.</param>
        void Discard(string message);

        /// <summary>
        ///     Displays a notification message in the UI.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="writeToFile">Determines whether the message should be logged to a file.</param>
        /// <param name="details">The details that should be logged with the message.</param>
        void Show(string message, bool writeToFile = false, params object?[] details);

        /// <summary>
        ///     Displays a notification message in the UI.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="writeToFile">Determines whether the message should be logged to a file.</param>
        /// <param name="details">The details that should be logged with the message.</param>
        /// <returns>
        ///     A <see cref="Task"/> representing an asynchronous operation.
        /// </returns>
        Task ShowAsync(string message, bool writeToFile = false, params object?[] details);
    }

    /// <summary>
    ///     Handles displaying notifications in the UI.
    /// </summary>
    public class NotificationService : INotificationService
    {
        /// <inheritdoc />
        public Dictionary<string, int> Notifications { get; set; } = [];

        private event Action? OnNotificationsChanged;
        private readonly Logger _log;

        private Func<Func<Task>, Task>? _invokeAsync;

        /// <summary>
        ///    Initializes a new instance of <see cref="NotificationService"/>.
        /// </summary>
        public NotificationService()
        {
            _log = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        /// <inheritdoc />
        public void SetOnNotificationsChanged(Action action)
            => OnNotificationsChanged = action;

        /// <inheritdoc />
        public void SetInvokeAsync(Func<Func<Task>, Task> action)
            => _invokeAsync = action;

        /// <inheritdoc />
        public async Task ShowAsync(string message, bool writeToFile = false, params object?[] details)
        {
            ShowMessage(message, writeToFile, details);

            if (_invokeAsync is not null)
                await _invokeAsync(() =>
                {
                    OnNotificationsChanged?.Invoke();
                    return Task.CompletedTask;
                });
        }

        /// <inheritdoc />
        public void Show(string message, bool writeToFile = false, params object?[] details)
        {
            ShowMessage(message, writeToFile, details);

            // Notify UI about the change
            OnNotificationsChanged?.Invoke();
        }

        private void ShowMessage(string message, bool writeToFile = false, params object?[] details)
        {
            if (writeToFile)
                _log.Information(message, details);

            if (Notifications.TryGetValue(message, out int count))
                Notifications[message] = count + 1;
            else
                Notifications.Add(message, 1);
        }

        /// <inheritdoc />
        public void Discard(string message)
        {
            if (!Notifications.TryGetValue(message, out int amount) || amount < 1)
                return;
 
            if (amount == 1)
                Notifications.Remove(message);
            else
                Notifications[message] = amount - 1;
        }
    }
}

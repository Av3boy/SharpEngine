namespace Launcher.UI.Components.Notification
{
    /// <summary>
    ///     Extension methods for <see cref="NotificationType"/>.
    /// </summary>
    public static class NotificationTypeExtensions
    {
        /// <summary>
        ///     Converts the <see cref="NotificationType"/> to a CSS class.
        /// </summary>
        /// <param name="type">The notification type.</param>
        /// <returns>The corresponding CSS class.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the notification type is out of range.</exception>
        public static string ToCssClass(this NotificationType type) => type switch
        {
            NotificationType.Primary => "primary",
            NotificationType.Secondary => "secondary",
            NotificationType.Success => "success",
            NotificationType.Danger => "danger",
            NotificationType.Warning => "warning",
            NotificationType.Info => "info",
            NotificationType.Light => "light",
            NotificationType.Dark => "dark",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }
}

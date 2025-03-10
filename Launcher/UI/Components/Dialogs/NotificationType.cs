using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.UI.Components
{
    /// <summary>
    ///     Represents the type of notification.
    /// </summary>
    public enum NotificationType
    {
        /// <summary>Primary notification type.</summary>
        Primary,

        /// <summary>Secondary notification type.</summary>
        Secondary,

        /// <summary>Success notification type.</summary>
        Success,

        /// <summary>Danger notification type.</summary>
        Danger,

        /// <summary>Warning notification type.</summary>
        Warning,

        /// <summary>Info notification type.</summary>
        Info,

        /// <summary>Light notification type.</summary>
        Light,

        /// <summary>Dark notification type.</summary>
        Dark,
    }

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

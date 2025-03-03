using System.Collections.Generic;

namespace BlockTower.Localization
{
    public class DefaultLocalizationProvider : ILocalizationProvider
    {
        public string GetString(string key, params object[] args)
        {
            // В реальном проекте здесь будет обращение к системе локализации
            var defaultStrings = new Dictionary<string, string>
            {
                { LocalizationKeys.Notifications.CUBE_PLACED, "Кубик установлен! +{0} очков" },
                { LocalizationKeys.Notifications.CUBE_REMOVED, "Кубик удален! -{0} очков" },
                { LocalizationKeys.Notifications.CUBE_DISAPPEARED, "Кубик пропал!" },
                { LocalizationKeys.Notifications.TOWER_HEIGHT_LIMIT, "Достигнут предел высоты!" }
            };

            if (defaultStrings.TryGetValue(key, out string value))
            {
                return string.Format(value, args);
            }
            return key;
        }
    }
} 
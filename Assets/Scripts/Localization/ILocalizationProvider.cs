namespace BlockTower.Localization
{
    public interface ILocalizationProvider
    {
        string GetString(string key, params object[] args);
    }
} 
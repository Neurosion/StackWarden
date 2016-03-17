namespace StackWarden.Core.Configuration
{
    public interface IConfigurationReader
    {
        T Read<T>(string path);
        T Read<T>(string path, T definition);
    }
}
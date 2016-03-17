namespace StackWarden.Core.Configuration
{
    public interface ICompositeConfiguration : IConfiguration
    {
        string Name { get; set; }
    }
}
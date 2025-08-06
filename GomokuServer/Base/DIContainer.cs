using Microsoft.Extensions.DependencyInjection;

public static class DIContainer
{
    public static IServiceProvider ServiceProvider { get; private set; }

    public static void Init(IServiceProvider provider)
    {
        ServiceProvider = provider;
    }

    public static T Get<T>() where T : class
    {
        return ServiceProvider.GetService<T>();
    }
}
using System;
using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Planificateur.UI;

public static class Extensions
{
    public static IServiceProvider GetServiceProvider(this IResourceHost control)
    {
        Application application = Application.Current ?? throw new InvalidOperationException(" Application.Current is null");
        object resource = application.FindResource(typeof(IServiceProvider)) ?? throw new InvalidOperationException("IServiceProvider not found in application resources");
        return (IServiceProvider)resource;
    }

    public static T CreateInstance<T>(this IResourceHost control)
    {
        return control.GetServiceProvider().GetService<T>() ??
               throw new InvalidOperationException($"Viewmodel of type {typeof(T).Name} not in DI");
    }
}
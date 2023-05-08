using System;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Planificateur.UI;

public static class Extensions
{
    public static IServiceProvider GetServiceProvider(this IResourceHost control)
    {
        return (IServiceProvider)App.Current.FindResource(typeof(IServiceProvider));
    }

    public static T CreateInstance<T>(this IResourceHost control)
    {
        return control.GetServiceProvider().GetService<T>() ??
               throw new InvalidOperationException($"Viewmodel of type {typeof(T).Name} not in DI");
    }
}
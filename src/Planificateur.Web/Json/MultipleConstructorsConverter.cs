using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Planificateur.Web.Json;

public class MultipleConstructorsConverter<T> : JsonConverter<T>
{
    /// <inheritdoc />
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            reader.Read();
        }

        var values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        DeserializePropertyValues(ref reader, typeToConvert, options, values);

        var instance = CreateInstanceUsingAConstructor(typeToConvert, values);
        if (instance == null)
        {
            throw new InvalidOperationException(
                $"Failed to find all parameters to any of the constructors: {typeToConvert}.");
        }

        FillProperties(instance, values);
        return (T)instance;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        throw new NotSupportedException("Do not use this converter for serialization.");
    }

    private static object? CreateInstanceUsingAConstructor(
        Type typeToConvert,
        IReadOnlyDictionary<string, object> values)
    {
        object? instance = null;
        foreach (var constructor in typeToConvert.GetConstructors())
        {
            var parameterValues = new List<object>();
            foreach (var parameter in constructor.GetParameters())
            {
                if (values.TryGetValue(parameter.Name, out var value))
                {
                    parameterValues.Add(value);
                }
            }

            if (parameterValues.Count == constructor.GetParameters().Length)
            {
                instance = constructor.Invoke(parameterValues.ToArray());
            }
        }

        return instance;
    }

    private static void DeserializePropertyValues(
        ref Utf8JsonReader reader,
        IReflect typeToConvert,
        JsonSerializerOptions options,
        IDictionary<string, object> values)
    {
        while (reader.TokenType is not JsonTokenType.EndObject)
        {
#pragma warning disable SA1009 // Closing parenthesis should be spaced correctly
            var propertyName = reader.GetString()!;
#pragma warning restore SA1009 // Closing parenthesis should be spaced correctly

            // Move past property name
            reader.Read();

#pragma warning disable IDE0079
#pragma warning disable S3011
            var prop = typeToConvert
                .GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(x => x.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
#pragma warning restore

            if (prop == null)
            {
                // Skip value
                reader.Skip();

                // Read past value
                reader.Read();
                continue;
            }

            var value = System.Text.Json.JsonSerializer.Deserialize(ref reader, prop.PropertyType, options);
            if (value != null)
            {
                values[prop.Name] = value;
            }

            if (reader.TokenType is not JsonTokenType.PropertyName)
            {
                reader.Read();
            }
        }
    }

    private static void FillProperties(object instance, IReadOnlyDictionary<string, object> values)
    {
        if (values == null)
        {
            throw new ArgumentNullException(nameof(values));
        }

        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        foreach (var property in instance.GetType().GetProperties())
        {
            if (!values.TryGetValue(property.Name, out var value))
            {
                continue;
            }

           // this is an extension method that tries the set-method and fallbacks
           // to assigning the backing field if it fails (to support get-only properties)
            property.SetValue(instance, value);
        }
    }
}
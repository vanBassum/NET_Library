using STDLib.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace STDLib.Saveable
{
    /*
     * 
     //https://github.com/dotnet/orleans/issues/1269
        public static class JsonHelper
        {
            private static readonly Type[] _specialNumericTypes = { typeof(ulong), typeof(uint), typeof(ushort), typeof(sbyte) };

            /// <summary>
            /// Converts values that were deserialized from JSON with weak typing (e.g. into <see cref="object"/>) back into
            /// their strong type, according to the specified target type.
            /// </summary>
            /// <param name="value">The value to convert.</param>
            /// <param name="targetType">The type the value should be converted into.</param>
            /// <returns>The converted value.</returns>
            public static object ConvertWeaklyTypedValue(object value, Type targetType)
            {
                if (targetType == null)
                    throw new ArgumentNullException(nameof(targetType));

                if (value == null)
                    return null;

                if (targetType.IsInstanceOfType(value))
                    return value;

                var paramType = Nullable.GetUnderlyingType(targetType) ?? targetType;

                if (paramType.IsEnum)
                {
                    if (value is string)
                        return Enum.Parse(paramType, (string)value);
                    else
                        return Enum.ToObject(paramType, value);
                }

                if (paramType == typeof(Guid))
                {
                    return Guid.Parse((string)value);
                }

                if (_specialNumericTypes.Contains(paramType))
                {
                    if (value is BigInteger)
                        return (ulong)(BigInteger)value;
                    else
                        return Convert.ChangeType(value, paramType);
                }

                if (value is long)
                {
                    return Convert.ChangeType(value, paramType);
                }

                throw new ArgumentException($"Cannot convert a value of type {value.GetType()} to {targetType}.");
            }
        }
     */

    public class BaseSettingsV2<T1> where T1 : BaseSettingsV2<T1>
    {
        public static readonly string defaultDataFolder = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? Path.Combine("/data", "vanBassum", System.Reflection.Assembly.GetEntryAssembly().GetName().Name) : "data";
        private Dictionary<string, object> fields = new Dictionary<string, object>();
        private static readonly Serializer serializer = new JSON();
        public static string SettingsFile = Path.Combine(defaultDataFolder, $"{typeof(T1).Name}.json");

        public static T1 Items { get; set; }

        public static void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsFile));
            using (Stream stream = File.Open(SettingsFile, FileMode.Create, FileAccess.ReadWrite))
                serializer.Serialize(Items, stream);
        }

        public static void Load()
        {
            if (!File.Exists(SettingsFile))
            {
                Items = Activator.CreateInstance<T1>();
                Save();
            }
            else
            {
                Items = serializer.Deserialize<T1>(SettingsFile);
            }
        }

        protected void SetPar<T2>(T2 value, [CallerMemberName] string propertyName = null)
        {
            lock (fields)
            {
                fields[propertyName] = value;
            }
        }

        protected T2 GetPar<T2>(T2 defVal = default(T2), [CallerMemberName] string propertyName = null)
        {

            T2 val = defVal;
#if !DEBUG
            try
            {
#endif
            lock (fields)
            {
                if (!fields.ContainsKey(propertyName))
                    fields[propertyName] = defVal;
                val = (T2)fields[propertyName];
            }
#if !DEBUG
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
#endif
            return val;
        }
    }
}


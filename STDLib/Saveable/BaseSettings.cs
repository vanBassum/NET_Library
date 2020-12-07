using STDLib.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace STDLib.Saveable
{
    /// <summary>
    /// Used to create settings that are static and therefore accessable across the whole project.
    /// </summary>
    /// <typeparam name="T1">The type of the settings object.</typeparam>
    public class BaseSettings<T1>
    {
        static readonly string defaultDataFolder = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? Path.Combine("/data", "vanBassum", System.Reflection.Assembly.GetEntryAssembly().GetName().Name) : "data";
        private static Dictionary<string, object> fields = new Dictionary<string, object>();
        private readonly static Serializer serializer = new JSON();

        /// <summary>
        /// Depending on windows or linux.
        /// </summary>
        public static string DataFolder { get { return GetPar<string>(defaultDataFolder); } set { SetPar(value); } }

        /// <summary>
        /// The file to store the settings to.
        /// </summary>
        public static string SettingsFile = Path.Combine(defaultDataFolder, "settings.json");

        /// <summary>
        /// Save the current state of the settings to <see cref="SettingsFile"/>.
        /// </summary>
        public static void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsFile));
            using (Stream stream = File.Open(SettingsFile, FileMode.Create, FileAccess.Write))
                Save(stream);
        }

        /// <summary>
        /// Load the settings from a file.
        /// </summary>
        /// Save the current state of the settings to <see cref="SettingsFile"/>.
        /// <param name="createIfNotExist">When true, a new file with default values will be created  when the file doens't exist. Otherwise this will throw an exception if the file doensn't exist.</param>
        public static void Load(bool createIfNotExist = true)
        {
            if (File.Exists(SettingsFile))
            {
                using (Stream stream = File.Open(SettingsFile, FileMode.Open, FileAccess.Read))
                    Load(stream);
            }
            else
            {
                if(createIfNotExist)
                    CreateDefaultSettings(SettingsFile);
                else
                    throw new System.Exception($"File not found '{SettingsFile}'");
            }
        }

        private static void CreateDefaultSettings(string file)
        {
            var v = typeof(T1).GetProperties();

            foreach (var pi in v)
                pi.GetValue(null);

            Directory.CreateDirectory(Path.GetDirectoryName(file));
            using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
                Save(stream);
        }

        /// <summary>
        /// Save the settings to a stream.
        /// </summary>
        /// <param name="stream"></param>
        public static void Save(Stream stream)
        {
            serializer.Serialize(fields, stream);
        }

        /// <summary>
        /// Load the settings from a stream.
        /// </summary>
        /// <param name="stream"></param>
        public static void Load(Stream stream)
        {
            fields = serializer.Deserialize<Dictionary<string, object>>(stream);
            if (fields == null)
                fields = new Dictionary<string, object>();
        }

        /// <summary>
        /// Set the value of a property.
        /// </summary>
        /// <typeparam name="T2">Type of the property to set.</typeparam>
        /// <param name="value">Value of the property to set.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        protected static void SetPar<T2>(T2 value, [CallerMemberName] string propertyName = null)
        {
            lock (fields)
            {
                fields[propertyName] = value;
            }

        }

        /// <summary>
        /// Get the value of a property.
        /// </summary>
        /// <typeparam name="T2">Type of the property to get.</typeparam>
        /// <param name="defVal">The default value to use if the property has no value.</param>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <returns></returns>
        protected static T2 GetPar<T2>(T2 defVal = default(T2), [CallerMemberName] string propertyName = null)
        {
            lock (fields)
            {
                T2 val = defVal;
                try
                {
                    if (!fields.ContainsKey(propertyName))
                        fields[propertyName] = defVal;
                    
                    if(typeof(T2) == fields[propertyName].GetType())
                        val = (T2)fields[propertyName];
                    else
                        val = (T2)Convert.ChangeType(fields[propertyName], typeof(T2));



                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return val;
            }
        }        
    }


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

}


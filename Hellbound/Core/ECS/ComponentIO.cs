using HellTrail.Core.Combat;
using HellTrail.Core.ECS.Components;
using HellTrail.Core.UI;
using HellTrail.Extensions;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Net.WebSockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace HellTrail.Core.ECS
{
    public static partial class ComponentIO
    {
        /// <summary>
        /// Returns string containing serialized component
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        [Obsolete("SerializeComponent is obsolete, use New_Serialize instead")]
        public static string SerializeComponent(IComponent component)
        {
            StringBuilder sb = new StringBuilder();
            Type type = component.GetType();
            sb.Append($"{type.Name}");

            FieldInfo[] fields = type.GetFields();

            if (fields.Length > 0)
                sb.Append(":\n\t\t\t[");
            else
                sb.Append(";");

            for(int i = 0; i < fields.Length; i++)
            {
                FieldToText(component, sb, fields[i], i == fields.Length-1);
            }

            if (fields.Length > 0)
                sb.Append("];");


            return sb.ToString();
        }

        [Obsolete("Method is obsolete, please use New_FieldToText")]
        public static void FieldToText(IComponent component, StringBuilder sb, FieldInfo field, bool isLast = true)
        {
            if (field.FieldType.IsArray)
            {
                var values = field.GetValue(component) as Array;

                sb.Append('{');
                if (values != null)
                {
                    for (int index = 0; index < values.Length; index++)
                    {
                        sb.Append($"\"{values.GetValue(index)}\"{(index == values.Length - 1 ? "" : ", ")}");
                    }
                }
                sb.Append($"}}{(isLast ? "" : "; ")}");

            } else
            {
                sb.Append($"\"{field.GetValue(component)}\"{(isLast ? "" : "; ")}");
            }
        }

        /// <summary>
        /// Returns component built from serialized component string
        /// </summary>
        /// <param name="component">Must contain name of component & parameters. Do not use this unless you know what you are doing</param>
        /// <returns></returns>
        [Obsolete("Method is obsolete, use New_Deserialize instead")]
        public static IComponent DeserializeComponent(string component)
        {
            string trimmed = component.TrimStart();

            string name = Regex.Match(trimmed, $"^[^:{Environment.NewLine}]*", RegexOptions.Singleline).Value;

            var value = BetweenSquareBrackets().Match(trimmed).Groups[1].Value;

            Type type = Context.ComponentTypeByName[name];

            FieldInfo[] fields = type.GetFields();

            IComponent instance = (IComponent)RuntimeHelpers.GetUninitializedObject(type);

            MatchCollection coll = BetweenSwirlyBracketsExcludingThem().Matches(value);

            string[] values = Regex.Split(value, "; ", RegexOptions.Singleline);

            for (int i= 0; i < fields.Length; i++)
            {
                if (values.Length > i)
                    TextToFieldValue(fields[i], instance, values[i]);
                else
                    TextToFieldValue(fields[i], instance, RuntimeHelpers.GetUninitializedObject(type).ToString());
            }

            return instance;
        }

        public static void SetDefaultField(IComponent component, FieldInfo field)
        {
            if(field.FieldType.IsGenericTypeDefinition)
            {
                Type[] types = field.FieldType.GetGenericArguments();

                dynamic dict = Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(types));

                field.SetValue(component, dict);
            }
            else if (field.FieldType.IsArray)
            {
                var elementType = field.FieldType.GetElementType();
                var emptyObject = elementType == typeof(string) ? string.Empty : Activator.CreateInstance(elementType);
                var arr = Array.CreateInstance(elementType, 1);
                arr.SetValue(emptyObject, 0);
                field.SetValue(component, arr);
            } else
            {
                if(field.FieldType == typeof(string))
                    field.SetValue(component, string.Empty);
                else
                    field.SetValue(component, RuntimeHelpers.GetUninitializedObject(field.FieldType));
            }
        }

        public static string New_Serialize(IComponent component)
        {
            StringBuilder sb = new StringBuilder();
            Type type = component.GetType();
            sb.Append($"{type.Name}");

            FieldInfo[] fields = type.GetFields();

            if (fields.Length > 0)
                sb.Append(":\n\t[");
            else
                sb.Append(";");

            for(int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                sb.Append(New_FieldToText(field, component));

                if(i < fields.Length - 1)
                {
                    sb.Append("; ");
                }
            }
            if (fields.Length > 0)
                sb.Append("];");

            return sb.ToString();
        }

        public static string New_FieldToText(FieldInfo field, IComponent instance)
        {
            StringBuilder sb = new StringBuilder();
            if(field.FieldType.IsArray)
            {
                sb.Append('[');
                var values = field.GetValue(instance) as Array;
                for(int i = 0; i < values.Length; i++)
                {
                    string appendage = "";
                    if (values.GetValue(i) != null)
                        appendage = values.GetValue(i).ToString();
                        sb.Append(appendage + $"{(i == values.Length - 1 ? "" : ", ")}");
                }
                sb.Append(']');
            } else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                Type[] types = field.FieldType.GetGenericArguments();

                sb.Append('[');

                dynamic dict = Convert.ChangeType(field.GetValue(instance), typeof(Dictionary<,>).MakeGenericType(types));
                int index = 0;
                foreach(var kvp in dict)
                {
                    sb.Append($"\"{kvp.Key}\"=[{kvp.Value}]");
                    index++;

                    if (index < dict.Count)
                        sb.Append(", ");
                }

                sb.Append(']');
            }
            else if(field.FieldType == typeof(string))
            {
                sb.Append($"\"{field.GetValue(instance)}\"");
            }
            else
            {
                sb.Append(field.GetValue(instance).ToString());
            }

            return sb.ToString();
        }

        public static IComponent New_Deserialize(string readString)
        {
            string trimmed = readString.Trim();

            string componentType = Regex.Replace(Regex.Match(trimmed, @"^[^[]*").Value, "[^a-zA-Z]", "").Trim();

            string values = Regex.Match(trimmed, @"\[(.*)\]").Groups[1].Value;

            ///NewAnimationComponent:["Idle"; ["Idle"=[{0 1}], "Idle2"=[{1 2}]]; [{32 32 5}, {16 16 4}, {32 32 6}]];
            string[] valuesSeparatedPerField = values.Split(';', StringSplitOptions.TrimEntries);

            Type type = Context.ComponentTypeByName[componentType];

            FieldInfo[] fields = type.GetFields();

            IComponent instance = (IComponent)RuntimeHelpers.GetUninitializedObject(type);

            for(int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if(valuesSeparatedPerField.Length <= i)
                    TextToFieldValue(field, instance, Activator.CreateInstance(field.FieldType).ToString());
                else
                    TextToFieldValue(field, instance, valuesSeparatedPerField[i]);
            }

            return instance;
        }

        private static Dictionary<Type, TryParserContainer> _tryParserCache = [];

        public static TryParserContainer FindTryParser(Type type)
        {
            if (_tryParserCache.TryGetValue(type, out TryParserContainer tryParser))
            {
                return tryParser;
            }

            var nativeImpl = type.GetMethod("TryParse", new[] { typeof(string), type.MakeByRefType() });
            if (nativeImpl != null)
            {
                TryParserContainer container = new(nativeImpl, true);
                _tryParserCache.Add(type, container);
                return container;
            }

            Assembly assembly = Assembly.GetEntryAssembly();

             var query = from ftype in assembly.GetTypes() // search for custom implementation that's not extension
                        where !type.IsGenericType && type.IsSealed
                        from method in ftype.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.GetParameters().Length >= 2
                        && method.GetParameters()[0].ParameterType == typeof(string)
                        && method.GetParameters()[1].ParameterType == type.MakeByRefType()
                        select method;

            if (!query.Any()) // if haven't found one, try looking for extensions
                query = from ftype in assembly.GetTypes()
                        where !type.IsGenericType && type.IsSealed
                        from method in ftype.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.IsDefined(typeof(ExtensionAttribute), false)
                        where method.GetParameters().Length >= 2 && method.GetParameters()[0].ParameterType == type
                        select method;

            if (query.Any())
            {
                _tryParserCache.Add(type, new (query.First(), false));
            }

            return new(query.First(), false);
        }

        public static bool InvokeTryParser(TryParserContainer parser, out object result, string parsedString, Type targetType)
        {
            object[] parameters = [parsedString, Activator.CreateInstance(targetType)];
            if (parser.isNativeImplementation && parser.methodInfo.Invoke(null, parameters) != null)
            {
                result = parameters[1];
                return true;
            } else if(parser.methodInfo != null)
            {
                parser.methodInfo.Invoke(null, parameters);
                result = parameters[1];
                return true;
            }

            result = default;
            return false;
        }


        //public static bool TryParse<T>(string toConvert, Type implicitType, out T result)
        //{
        //    if (toConvert == null)
        //        throw new ArgumentNullException(nameof(toConvert));

        //    // Initialize result
        //    result = default;

        //    var method = implicitType.GetMethod("TryParse", new[] { typeof(string), implicitType.MakeByRefType() });

        //    if (method == null)
        //        throw new InvalidOperationException("Type does not contain a try-parse");
        //    // Prepare an object array for the parameters
        //    object[] parameters = new object[] { toConvert, null };

        //    // Invoke the method using reflection
        //    bool success = (bool)method.Invoke(null, parameters); // 'null' because TryParse is static

        //    // Set the out parameter
        //    result = (T)parameters[1];

        //    return success;
        //}

        // TO-DO: Deprecate, write smth better to allow arrays of arrays/dictionaries
        public static void TextToFieldValue(FieldInfo field, IComponent instance, string value)
        {
            Type t = field.FieldType;
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                string[] splitInput = Regex.Match(value, @"\[(.*)\]").Groups[1].Value.Split(", ");
                Type[] types = t.GetGenericArguments();
                dynamic dic = typeof(Dictionary<,>).MakeGenericType(types);

                Type valueType = types[1];
                string[] keys = splitInput.Select(x => Regex.Match(x, "(.*)=").Groups[1].Value).ToArray();
                string[] values = splitInput.Select(x => Regex.Match(x, @"\[(.*)\]").Groups[1].Value).ToArray();

                dynamic dict = Activator.CreateInstance(dic);

                //TryParse(values[0], valueType, out dynamic result);
                dynamic v = FindTryParser(valueType);
                for (int i = 0; i < keys.Length; i++)
                {
                    dynamic result = RuntimeHelpers.GetUninitializedObject(valueType);
                    InvokeTryParser(v, out result, values[i], valueType);

                    dict.Add(keys[i], result);
                }

                field.SetValue(instance, dict);
            } else if (field.FieldType.IsArray)
            {
                //if (values.Length <= i)
                //    continue;

                Type elementType = field.FieldType.GetElementType();

                string val = value;
                string[] elements = Regex.Match(val, @"\[(.*)\]").Groups[1].Value.Split(", ");

                var arr = Array.CreateInstance(elementType, elements.Length);

                if (elements.Length <= 1 && string.IsNullOrWhiteSpace(elements[0]))
                {
                    arr = Array.CreateInstance(elementType, 1);
                    arr.SetValue(default, 0);
                    field.SetValue(instance, arr);
                    return;
                }

                for (int elementIndex = 0; elementIndex < elements.Length; elementIndex++)
                {
                    string noQuotes = BetweenSwirlyBracketsRegex().Replace(elements[elementIndex], "");
                    if (elementType.GetInterface("IConvertible") != null)
                    {
                        var element = Convert.ChangeType(noQuotes, elementType);
                        arr.SetValue(element, elementIndex);
                    } else
                    {
                        dynamic element = RuntimeHelpers.GetUninitializedObject(elementType);
                        var parser = FindTryParser(elementType);
                        InvokeTryParser(parser, out element, noQuotes, elementType);
                        arr.SetValue(element, elementIndex);
                    }

                    //if (elementType == typeof(Vector2))
                    //{
                    //    noQuotes.TryVector2(out Vector2 vector);
                    //    arr.SetValue(vector, elementIndex);
                    //} else if (elementType == typeof(FrameData))
                    //{
                    //    noQuotes.TryFrameData(out var frameData);
                    //    arr.SetValue(frameData, elementIndex);
                    //} else if (elementType == typeof(Color))
                    //{
                    //    noQuotes.TryColor(out var color);
                    //    arr.SetValue(color, elementIndex);
                    //} else if (elementType == typeof(IndexTuple))
                    //{
                    //    IndexTuple.TryParse(noQuotes, out var indexTuple);
                    //    arr.SetValue(indexTuple, elementIndex);
                    //} else
                    //{
                    //    arr.SetValue(Convert.ChangeType(elements[elementIndex], elementType), elementIndex);
                    //}
                }
                field.SetValue(instance, Convert.ChangeType(arr, field.FieldType));
            } else
            {
                string noQuotes = BetweenSwirlyBracketsRegex().Replace(value, "");
                dynamic element = field.FieldType == typeof(string) ? string.Empty : RuntimeHelpers.GetUninitializedObject(field.FieldType);

                if (field.FieldType.GetInterface("IConvertible") != null)
                {
                    element = Convert.ChangeType(noQuotes, field.FieldType);
                } else
                {
                    var parser = FindTryParser(field.FieldType);
                    InvokeTryParser(parser, out element, noQuotes, field.FieldType);
                }

                field.SetValue(instance, element);
            }
        }

        [GeneratedRegex(@"[{""}]")]
        private static partial Regex BetweenSwirlyBracketsRegex();
        [GeneratedRegex(@"\{([^{}]*)\}")]
        private static partial Regex BetweenSwirlyBracketsExcludingThem();
        [GeneratedRegex(@"\[(.*)\]", RegexOptions.Singleline)]
        private static partial Regex BetweenSquareBrackets();
    }
}
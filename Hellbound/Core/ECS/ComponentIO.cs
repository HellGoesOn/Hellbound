using HellTrail.Core.Combat;
using HellTrail.Extensions;
using Microsoft.Xna.Framework;
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
                if (values.Length >= i)
                    TextToFieldValue(fields[i], instance, values[i]);
                else
                    TextToFieldValue(fields[i], instance, RuntimeHelpers.GetUninitializedObject(type).ToString());
            }

            return instance;
        }

        public static void TextToFieldValue(FieldInfo field, IComponent instance, string value)
        {
            if (field.FieldType.IsArray)
            {
                //if (values.Length <= i)
                //    continue;

                Type elementType = field.FieldType.GetElementType();

                string val = value;
                string[] elements = BetweenSwirlyBracketsRegex().Replace(val, "").Split(", ");

                if (elements.Length <= 1 && string.IsNullOrWhiteSpace(elements[0]))
                {
                    field.SetValue(instance, null);
                    return;
                }

                var arr = Array.CreateInstance(elementType, elements.Length);

                for (int elementIndex = 0; elementIndex < elements.Length; elementIndex++)
                {
                    string noQuotes = BetweenSwirlyBracketsRegex().Replace(elements[elementIndex], "");

                    if (elementType == typeof(Vector2))
                    {
                        noQuotes.TryVector2(out Vector2 vector);
                        arr.SetValue(vector, elementIndex);
                    } else if (elementType == typeof(FrameData))
                    {
                        noQuotes.TryFrameData(out var frameData);
                        arr.SetValue(frameData, elementIndex);
                    } else if (elementType == typeof(Color))
                    {
                        noQuotes.TryColor(out var color);
                        arr.SetValue(color, elementIndex);
                    } else
                    {
                        arr.SetValue(Convert.ChangeType(elements[elementIndex], elementType), elementIndex);
                    }
                }
                field.SetValue(instance, Convert.ChangeType(arr, field.FieldType));
            } else
            {
                string noQuotes = BetweenSwirlyBracketsRegex().Replace(value, "");
                if (field.FieldType == typeof(Vector2))
                {
                    noQuotes.TryVector2(out Vector2 vector);
                    field.SetValue(instance, vector);
                } else if (field.FieldType == typeof(FrameData))
                {
                    noQuotes.TryFrameData(out var frameData);

                    field.SetValue(instance, frameData);
                } else
                {
                    field.SetValue(instance, Convert.ChangeType(noQuotes, field.FieldType));
                }
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
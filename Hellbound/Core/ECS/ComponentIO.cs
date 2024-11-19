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
    public static class ComponentIO
    {
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
                FieldInfo field = fields[i];
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
                    sb.Append($"}}{(i == fields.Length - 1 ? "" : "; ")}");

                } else
                {
                    sb.Append($"\"{field.GetValue(component)}\"{(i == fields.Length - 1 ? "" : "; ")}");
                }
            }

            if (fields.Length > 0)
                sb.Append("];");


            return sb.ToString();
        }

        public static IComponent DeserializeComponent(string component)
        {
            string trimmed = component.TrimStart();

            string name = Regex.Match(trimmed, $"^[^:{Environment.NewLine}]*", RegexOptions.Singleline).Value;

            var value = Regex.Match(trimmed, @"\[(.*)\]", RegexOptions.Singleline).Groups[1].Value;

            Type type = Context.ComponentTypeByName[name];

            FieldInfo[] fields = type.GetFields();

            IComponent instance = (IComponent)RuntimeHelpers.GetUninitializedObject(type);

            MatchCollection coll = Regex.Matches(value, @"\{([^{}]*)\}");

            string[] values = Regex.Split(value, "; ", RegexOptions.Singleline);

            for (int i= 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                if (field.FieldType.IsArray)
                {
                    if (values.Length <= i)
                        continue;

                    Type elementType = field.FieldType.GetElementType();

                    string val = values[i];
                    string[] elements = Regex.Replace(val, @"[{""}]", "").Split(", ");

                    if (elements.Length <= 1 && string.IsNullOrWhiteSpace(elements[0]))
                    {
                        field.SetValue(instance, null);
                        continue;
                    }

                    var arr = Array.CreateInstance(elementType, elements.Length);

                    for (int elementIndex = 0; elementIndex < elements.Length; elementIndex++)
                    {
                        string noQuotes = Regex.Replace(elements[elementIndex], @"[{""}]", "");
                        if (elementType == typeof(Vector2))
                        {
                            noQuotes.TryVector2(out Vector2 vector);
                            arr.SetValue(vector, elementIndex);
                        } else if (elementType == typeof(FrameData))
                        {
                            noQuotes.TryFrameData(out var frameData);
                            arr.SetValue(frameData, elementIndex);
                        } else if(elementType == typeof(Color))
                        {
                            noQuotes.TryColor(out var color);
                            arr.SetValue(color, elementIndex);
                        } else
                        {
                            arr.SetValue(Convert.ChangeType(elements[elementIndex], elementType), elementIndex);
                        }
                    }


                    field.SetValue(instance, Convert.ChangeType(arr, fields[i].FieldType));
                } else
                {
                    string noQuotes = Regex.Replace(values[i], @"[{""}]", "");
                    if (field.FieldType == typeof(Vector2))
                    {
                        noQuotes.TryVector2(out Vector2 vector);
                        fields[i].SetValue(instance, vector);
                    } else if(field.FieldType == typeof(FrameData))
                    {
                        noQuotes.TryFrameData(out var frameData);

                        fields[i].SetValue(instance, frameData);
                    }
                    else
                    {
                        fields[i].SetValue(instance, Convert.ChangeType(noQuotes, fields[i].FieldType));
                    }
                }
            }

            return instance;
        }
    }
}
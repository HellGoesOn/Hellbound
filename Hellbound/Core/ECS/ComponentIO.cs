using HellTrail.Extensions;
using Microsoft.Xna.Framework;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

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
                    object[] values = (object[])field.GetValue(component);

                    if (values != null)
                    {
                        sb.Append('{');
                        for (int index = 0; index < values.Length; index++)
                        {
                            sb.Append($"\"{values[index]}\"{(index == values.Length - 1 ? "" : ", ")}");
                        }
                        sb.Append($"}}{(i == fields.Length - 1 ? "" : ", ")}");
                    }

                } else
                {
                    sb.Append($"\"{field.GetValue(component)}\"{(i == fields.Length - 1 ? "" : ", ")}");
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

            string[] values = value.Split(", ");

            Type type = Context.ComponentTypeByName[name];

            FieldInfo[] fields = type.GetFields();

            IComponent instance = (IComponent)RuntimeHelpers.GetUninitializedObject(type);

            for(int i= 0; i < fields.Length;i++)
            {
                string noQuotes = Regex.Replace(values[i], @"[{""}]", "");
                if (fields[i].FieldType.IsArray)
                {
                    Type elementType = fields[i].FieldType.GetElementType();
                    MatchCollection coll = Regex.Matches(value, @"\{([^{}]*)\}");

                    for (int index = 0; index < coll.Count; index++)
                    {
                        string val = coll[index].Value;
                        string[] elements = Regex.Replace(val, @"[{""}]", "").Split(", ");
                        var arr = Array.CreateInstance(elementType, elements.Length);

                        for (int elementIndex = 0; elementIndex < elements.Length; elementIndex++)
                        {
                            arr.SetValue(Convert.ChangeType(elements[elementIndex], elementType), elementIndex);
                        }

                        fields[index].SetValue(instance, Convert.ChangeType(arr, fields[i].FieldType));
                    }
                } 
                else
                {
                    if (noQuotes.TryVector2(out Vector2 vector))
                    {
                        fields[i].SetValue(instance, vector);
                    } else
                    {
                        fields[i].SetValue(instance, Convert.ChangeType(noQuotes, fields[i].FieldType));
                    }
                }
            }

            return instance;
        }
    }
}
//using HellTrail.Core.ECS.Attributes;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Text.Json;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

//namespace HellTrail.Core.ECS
//{
//    public static class ComponentIO
//    {
//        public static string WriteComponent(IComponent component)
//        {
//            StringBuilder sb = new StringBuilder();

//            string jsonString = JsonSerializer.Serialize(component);

//            sb.AppendLine(jsonString);

//            return sb.ToString();
//        }

//        public static string WriteComponent(IComponent component, Context context)
//        {
//            FieldInfo[] infos = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

//            StringBuilder sb = new StringBuilder();

//            sb.AppendLine(Context.ComponentNameByType[component.GetType()]);
//            sb.AppendLine($"\t[");

//            foreach (FieldInfo info in infos)
//            {
//                if (info.GetCustomAttribute<IgnoreAttribute>() != null)
//                    continue;

//                string value = null;

//                SaveCustomAttribute data = info.GetCustomAttribute<SaveCustomAttribute>();
//                if (data != null)
//                {
//                    value = data.onSave?.Invoke(component);
//                } else
//                {
//                    object obj = info.GetValue(component);
//                    value ??= obj != null ? obj.ToString() : "";
//                }
//                sb.AppendLine($"\t\t{info.Name}={value};");
//            }

//            sb.AppendLine("\t];");

//            return sb.ToString();
//        }

//        public static void WritePreFab(Entity entity, Context context)
//        {       
//            StringBuilder sb = new StringBuilder();

//            foreach(IComponent component in entity._components.Where(x=>x != null))
//            {
//                sb.AppendLine(WriteComponent(component));
//            }

//            string path = Environment.CurrentDirectory + "\\Prefabs\\";
//            if (!Directory.Exists(path))
//                Directory.CreateDirectory(path);
//            File.WriteAllText(path + $"{entity}.fab", sb.ToString());
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="path">Starts at [Current Directory]/Prefabs/</param>
//        /// <param name="context"></param>
//        public static void LoadFromFab(string path, Context context, string componentPath = "HellTrail.Core.ECS.Components.")
//        {
//            var entity = context.Create();

//            string str = File.ReadAllText(Environment.CurrentDirectory + "\\Prefabs\\" + path + ".fab");
//            str = Regex.Replace(str, "\n", "").Replace("\t", "").Replace("\r", "");
//            string[] components = Regex.Split(str, @"(?<=];)");

//            foreach (var component in components)
//            {
//                if (string.IsNullOrWhiteSpace(component))
//                    continue;

//                string pattern = "^[^[]*";

//                string componentName = Regex.Match(component, pattern).Value;
//                string componentValue = Regex.Match(component, "=(.*)").Value;

//                Type com = Assembly.GetExecutingAssembly().GetType(componentPath + componentName, true);

//                FieldInfo[] fields = com.GetFields();

//                foreach (var item in fields)
//                {
//                    LoadCustomAttribute attr = item.GetCustomAttribute<LoadCustomAttribute>();
//                    if (attr != null)
//                    {
//                        //attr.onLoad.Invoke(component, componentValue);
//                    }
//                }

//                //entity.AddComponent((IComponent)Activator.CreateInstance(com));

//                bool shit = true;
//            }

//        }
//    }
//}

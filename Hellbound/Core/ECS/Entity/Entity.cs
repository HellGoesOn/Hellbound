using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HellTrail.Core.ECS
{
    public class Entity
    {
        public int id;
        public bool enabled;
        private readonly int _maxComponents;
        IComponent[] _components;

        public EntityChangedHandler OnComponentRemoved;
        public EntityChangedHandler OnComponentChanged;
        public EntityChangedHandler OnComponentAdded;

        public event EntityDestroyEvent OnDestroy;

        public Entity(int id, int maxComponents)
        {
            this.id = id;
            this._maxComponents = maxComponents;
            _components = new IComponent[_maxComponents];
        }

        public void Reuse(int id)
        {
            enabled = true;
            this.id = id;
            _components = new IComponent[_maxComponents];
        }

        public T GetComponent<T>() where T : IComponent
        {
            int id = Context.ComponentId[typeof(T)];
            if (HasComponent<T>())
            {
                return (T)_components[id];
            }

            throw new Exception($"{this} does not contain {typeof(T)}");
        }

        public bool HasComponent<T>()
        {
            int id = Context.ComponentId[typeof(T)];
            return _components[id] != null;
        }

        public bool HasComponents(params int[] indexes)
        {
            for (int i = 0; i < indexes.Length; i++)
            {
                int index = indexes[i];
                if (_components[index] == null)
                    return false;
            }

            return true;
        }

        public bool HasAnyComponent() => _components != null && _components.Any(x => x != null);

        public void AddComponent(IComponent component)
        {
            HandleComponent(component.GetType(), component);
            OnComponentAdded?.Invoke(this, component);
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            HandleComponent(typeof(T), null);
        }

        public void RemoveComponent(Type type)
        {
            HandleComponent(type, null);
        }

        void HandleComponent(Type type, IComponent component)
        {
            int id = Context.ComponentId[type];
            var previousComponent = _components[id];
            if (previousComponent != null)
            {
                //context.componentPools[id].Push(previousComponent);
                OnComponentChanged?.Invoke(this, component);
            } else
            {
                OnComponentAdded?.Invoke(this, component);
            }

            _components[id] = component;
            if (_components[id] == null)
            {
                OnComponentRemoved?.Invoke(this, previousComponent);
            }
        }

        public void Destroy(Context context)
        {
            enabled = false;
            if (_components != null)
            {
                for (int i = 0; i < _maxComponents; i++)
                {
                    var component = _components[i];
                    if (component == null)
                        continue;

                    //GetComponentPool(Context.ComponentId[component.GetType()]).Push(component);
                    component = null;
                }
            }

            OnDestroy?.Invoke(this);
        }

        internal void InternalDestroy()
        {
            OnComponentAdded = null;
            OnComponentChanged = null;
            OnComponentRemoved = null;

            _components = null;
            OnDestroy = null;
        }

        /*public TComponent CreateComponent<TComponent>() where TComponent : IComponent, new()
        {
            var componentPool = GetComponentPool(Context.ComponentId[typeof(TComponent)]);
            return componentPool.Count > 0 ? (TComponent)componentPool.Pop() : new TComponent();
        }

        protected Stack<IComponent> GetComponentPool(int id)
        {
            return context.componentPools[id];
        }*/

        public IComponent[] GetAllComponents() => _components.Where(x => x != null).ToArray();

        public override string ToString()
        {
            return $"Entity_{id} \n{string.Join("\n", GetAllComponents().Select(x => x.ToString()))}";
        }

        public static Entity Deserialize(string text, Context context = null)
        {
            string preSplit = Regex.Match(text, @"{(.*)}", RegexOptions.Singleline).Groups[1].Value.Trim();
            string[] components = preSplit.Split($";{Environment.NewLine}");
            string entityLine = Regex.Match(text, $"Entity_.*[^{Environment.NewLine}]", RegexOptions.Multiline).Value;
            string id = Regex.Replace(entityLine, "[^0-9]", "");

            Entity e;
            if (context != null)
            {
                Entity checkExisting = context.entities[int.Parse(id)];
                if (checkExisting != null && checkExisting.enabled)
                {
                    context.Destroy(checkExisting.id);
                }
                e = context.Create();
            }
            else
            {
                e = new Entity(0, Context._maxComponents);
            }

            for (int componentIndex = 0; componentIndex < components.Length; componentIndex++)
            {
                e.AddComponent(ComponentIO.DeserializeComponent(components[componentIndex]));
            }

            return e;
        }

        public static Entity[] DeserializeAll(string text, Context context)
        {
            string[] entityTexts = Regex.Split(text, @$" ;{Environment.NewLine}", RegexOptions.Singleline);

            Entity[] entities = new Entity[entityTexts.Length];
            for (int i = 0; i < entityTexts.Length; i++)
            {
                Deserialize(entityTexts[i], context);
            }

            return entities;
        }


        /// <summary>
        /// Returns string containing serialized entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string Serialize(Entity entity)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Entity_{entity.id}{Environment.NewLine}\t{{");
            foreach (IComponent c in entity.GetAllComponents())
            {
                sb.AppendLine($"\t\t{ComponentIO.SerializeComponent(c)}");
            }
            sb.Append($"\t}} ;");
            
            return sb.ToString();
        }
    }

    public delegate void EntityChangedHandler(Entity entity, IComponent component);

    public delegate void EntityDestroyEvent(Entity entity);
}

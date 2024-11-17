using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public class Entity
    {
        public int id;
        public bool enabled;
        private readonly int _maxComponents;
        IComponent[] _components;
        Context context;

        public EntityChangedHandler OnComponentRemoved;
        public EntityChangedHandler OnComponentChanged;
        public EntityChangedHandler OnComponentAdded;

        public event EntityDestroyEvent OnDestroy;

        public Entity(int id, int maxComponents, Context context)
        {
            this.id = id;
            this._maxComponents = maxComponents;
            _components = new IComponent[_maxComponents];
            this.context = context;
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
                return (T) _components[id];
            }

            throw new Exception($"{this} does not contain {typeof(T)}" );
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

        public bool HasAnyComponent() => _components != null && _components.Any(x=>x != null);

        public void AddComponent(IComponent component)
        {
            HandleComponent(component);
            OnComponentAdded?.Invoke(this, component);
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            HandleComponent(null);
        }

        void HandleComponent(IComponent component)
        {
            int id = Context.ComponentId[component.GetType()];
            var previousComponent = _components[id];
            if (previousComponent != null)
            {
                context.componentPools[id].Push(previousComponent);
                OnComponentAdded?.Invoke(this, previousComponent);
            }
            else
            {
                OnComponentRemoved?.Invoke(this, previousComponent);
            }

            _components[id] = component;
        }

        public void Destroy(Context context)
        {
            enabled = false;
            if (_components != null)
            {
                for(int i = 0; i < _maxComponents; i++)
                {
                    var component = _components[i];
                    if (component == null)
                        continue;

                    GetComponentPool(Context.ComponentId[component.GetType()]).Push(component);
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

        public TComponent CreateComponent<TComponent>() where TComponent : IComponent, new()
        {
            var componentPool = GetComponentPool(Context.ComponentId[typeof(TComponent)]);
            return componentPool.Count > 0 ? (TComponent)componentPool.Pop() : new TComponent();
        }

        protected Stack<IComponent> GetComponentPool(int id)
        {
            return context.componentPools[id];
        }

        public override string ToString()
        {
            return $"Entity_{id}, A={enabled}, CC={string.Join(",", _components.Where(x=> x!=null).Select(x=> x.ToString()))}";
        }
    }

    public delegate void EntityChangedHandler(Entity entity, IComponent component);

    public delegate void EntityDestroyEvent(Entity entity);
}

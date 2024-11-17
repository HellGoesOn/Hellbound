﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public class Context
    {
        internal static int _maxComponents;
        public static readonly Dictionary<Type, int> ComponentId = [];
        public static readonly Dictionary<string, int> ComponentIdByName = [];
        public static readonly Dictionary<int, string> ComponentNameById = [];
        public static void InitializeAll()
        {
            IEnumerable<Type> types = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsClass && x.GetInterface("IComponent") != null);
            _maxComponents = types.Count();
            int id = 0;
            foreach (Type type in types)
            {
                ComponentId.Add(type, id);
                ComponentIdByName.Add(type.Name, id);
                ComponentNameById.Add(id, type.Name);
                id++;
            }
        }

        public static int GetComponentId<T>() where T : IComponent => ComponentId[typeof(T)];

        public int entityCount;

        public Entity[] entities;

        public Stack<IComponent>[] componentPools;
        private Stack<Entity> _entityPool;
        public Stack<int> activeEntityIds;

        event EntityChangedHandler _onComponentAdded;
        event EntityChangedHandler _onComponentRemoved;
        event EntityChangedHandler _onComponentChanged;

        internal readonly Dictionary<Matcher<Entity>, Group<Entity>> _groups = [];

        public Context(int maxEntities = 1000)
        {
            activeEntityIds = new Stack<int>();
            entities = new Entity[maxEntities];
            _entityPool = new Stack<Entity>();
            componentPools = new Stack<IComponent>[_maxComponents];

            for (int i = 0; i < _maxComponents; i++)
            {
                componentPools[i] = new Stack<IComponent>();
            }

            _onComponentAdded = (entity, component) =>
            {
                foreach(var group in _groups)
                {
                    group.Value.HandleEntity(entity);
                }
            };

            _onComponentChanged = (entity, component) =>
            {
                foreach (var group in _groups)
                {
                    group.Value.HandleEntity(entity);
                }
            };

            _onComponentRemoved = (entity, component) =>
            {
                foreach (var group in _groups)
                {
                    group.Value.HandleEntity(entity);
                }
            };
        }

        public Entity Create()
        {
            if (!_entityPool.TryPeek(out Entity entity))
            {
                entity = new(entityCount, _maxComponents, this)
                {
                    enabled = true
                };
            } else
            {
                _entityPool.Pop();
                entity.Reuse(entity.id);
            }

            entity.OnComponentAdded += _onComponentAdded;
            entity.OnComponentRemoved += _onComponentRemoved;
            entity.OnComponentChanged += _onComponentChanged;
            entity.OnDestroy += (e) =>
            {
                foreach (var group in _groups)
                {
                    group.Value.HandleEntity(e);
                }
                e.InternalDestroy();
            };

            entities[entity.id] = entity;
            activeEntityIds.Push(entity.id);
            entityCount++;
            return entities[entity.id];
        }

        public void Destroy(int id)
        {
            entityCount--;
            Entity e = entities[id];
            if (!e.enabled)
                return;

            foreach(var group in _groups)
            {
                group.Value.HandleEntity(e);
            }
            e.Destroy(this);
            activeEntityIds.Pop();
            _entityPool.Push(e);
        }

        public string ListEntities()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Entity e in entities.Where(x => x != null && x.enabled))
            {
                sb.AppendLine(e.ToString());
            }

            return sb.ToString();
        }

        public void Destroy(Entity e) => Destroy(e.id);

        public Group<Entity> GetGroup(Matcher<Entity> matcher)
        {
            if(!_groups.TryGetValue(matcher, out var group))
            {
                group = new Group<Entity>(matcher);
                for(int i = 0; i < entityCount; i++)
                {
                    group.HandleEntity(entities[i]);
                }

                _groups.Add(matcher, group);
            }

            return group;
        }

        public int LastActiveEntity
        {
            get
            {
                if(activeEntityIds.TryPeek(out int e))
                {
                    return e;
                }

                return -1;
            }
        }
    }
}

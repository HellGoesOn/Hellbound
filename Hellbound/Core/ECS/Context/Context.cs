using System;
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
        public static void InitializeAll()
        {
            IEnumerable<Type> types = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsClass && x.GetInterface("IComponent") != null);
            _maxComponents = types.Count();
            int id = 0;
            foreach (Type type in types)
            {
                ComponentId.Add(type, id);
                ComponentIdByName.Add(type.Name, id);
                id++;
            }
        }

        public int entityCount;

        public Entity[] entities;

        public Stack<IComponent>[] componentPools;
        private Stack<Entity> _entityPool;
        public Stack<int> activeEntityIds;

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
        }

        public Entity Create()
        {
            entityCount++;
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
            entities[entity.id] = entity;
            activeEntityIds.Push(entity.id);
            return entities[entity.id];
        }

        public void Destroy(int id)
        {
            entityCount--;
            Entity e = entities[id];
            if (!e.enabled)
                return;

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public class Matcher<TEntity> : IMatcher<TEntity> where TEntity : Entity
    {
        readonly int[] _indexes;

        public Matcher(params int[] requiredComponents)
        {
            _indexes = requiredComponents;
        }

        public bool Matches(TEntity entity)
        {
            return entity.HasComponents(_indexes);
        }

        public static Matcher<Entity> AllOf(params int[] ids)
        {
            return new Matcher<Entity>(ids);
        }

        public static Matcher<Entity> AllOf(params Type[] ids)
        {
            int[] realIds = new int[ids.Length];

            for(int i = 0; i < realIds.Length; i++)
            {
                realIds[i] = Context.ComponentId[ids[i]];
            }

            return new Matcher<Entity>(realIds);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < _indexes.Length; i++)
            {
                sb.Append($"{Context.ComponentNameById[_indexes[i]]},");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}

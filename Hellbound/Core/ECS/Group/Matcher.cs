using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public class Matcher<TEntity> : IMatcher<TEntity> where TEntity : Entity
    {
        int[] _indexes;
        int[] _noneOfIndexes;

        public Matcher()
        {
        }

        public bool Matches(TEntity entity)
        {
            return (_indexes == null || entity.HasComponents(_indexes)) 
                && (_noneOfIndexes == null || !entity.HasComponents(_noneOfIndexes));
        }

        public static Matcher<Entity> AllOf(params Type[] ids)
        {
            Matcher<Entity> builder = new();
            int[] realIds = new int[ids.Length];

            for(int i = 0; i < realIds.Length; i++)
            {
                realIds[i] = Context.ComponentId[ids[i]];
            }
            builder._indexes = realIds;

            return builder;
        }

        public Matcher<Entity> NoneOf(params Type[] ids)
        {
            Matcher<Entity> builder = new();

            builder._indexes = _indexes;

            int[] realIds = new int[ids.Length];
            for (int i = 0; i < realIds.Length; i++)
            {
                realIds[i] = Context.ComponentId[ids[i]];
            }
            builder._noneOfIndexes = realIds;
            return builder;
        }

        public override string ToString()
        {
            StringBuilder sb = new("[");
            for (int i = 0; i < _indexes.Length; i++)
            {
                sb.Append($"{Context.ComponentNameById[_indexes[i]]},");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}

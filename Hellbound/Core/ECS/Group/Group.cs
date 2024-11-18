using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public class Group<TEntity> : IGroup<TEntity> where TEntity : Entity
    {
        private readonly List<TEntity> _entities = [];
        public List<TEntity> Entities => _entities;
        public IMatcher<TEntity> matcher;

        public int Count => _entities.Count;

        public TEntity this[int index] => _entities[index];

        public Group(IMatcher<TEntity> matcher)
        {
            this.matcher = matcher;
        }

        public void HandleEntity(TEntity entity)
        {
            if (entity.enabled && entity.HasAnyComponent() && matcher.Matches(entity) && !_entities.Contains(entity))
            {
                _entities.Add(entity);
            } 
            else if (_entities.Contains(entity))
            {
                _entities.Remove(entity);
            }
        }

        public void UpdateEntity(TEntity entity)
        {
            if (!matcher.Matches(entity))
            {
                _entities.Remove(entity);
            }
        }
    }
}

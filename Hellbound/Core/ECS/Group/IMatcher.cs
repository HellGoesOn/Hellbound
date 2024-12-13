namespace Casull.Core.ECS
{
    public interface IMatcher<T> where T : Entity
    {
        bool Matches(T entity);
    }
}

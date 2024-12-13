namespace Casull.Core.ECS
{
    public interface IGroup<T> where T : Entity
    {
        static List<T> Entities { get; }
    }
}

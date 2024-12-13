namespace Casull.Core.ECS
{
    public interface IExecute : ISystem
    {
        void Execute(Context context);
    }
}

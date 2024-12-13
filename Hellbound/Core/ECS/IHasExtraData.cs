namespace Casull.Core.ECS
{
    public interface IHasExtraData
    {
        string SaveExtraData(Context context);
        IComponent LoadExtraData(Context context);
    }
}

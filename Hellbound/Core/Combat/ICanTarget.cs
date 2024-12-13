namespace Casull.Core.Combat
{
    public interface ICanTarget
    {
        ValidTargets CanTarget();
        bool AoE();
    }
}

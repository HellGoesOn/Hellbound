using Microsoft.Xna.Framework;

namespace Casull.Core.UI
{
    public interface IUIElement
    {
        void Append(IUIElement element);
        void Disown(IUIElement element, bool plannedToAdopt = false);
        List<UIElement> Children { get; }
        Vector2 GetPosition();
    }
}

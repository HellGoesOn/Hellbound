using Casull.Core.ECS.Components;
using Casull.Core.Overworld;
using Casull.Extensions;
using Casull.Render;
using Microsoft.Xna.Framework.Graphics;

namespace Casull.Core.ECS
{
    public class DrawSystem : IDraw
    {
        readonly Group<Entity> _group;

        public DrawSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(Transform), typeof(TextureComponent)).NoneOf(typeof(NewAnimationComponent)));
        }


        public void Draw(Context context, SpriteBatch spriteBatch)
        {
            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++) {
                var entity = entities[i];
                TextureComponent tex = entity.GetComponent<TextureComponent>();
                Transform transform = entity.GetComponent<Transform>();

                float depth = 1f + transform.position.Y + DisplayTileLayer.TILE_SIZE + 8 + transform.layer * DisplayTileLayer.TILE_SIZE + 12 * transform.layer;
                //float depth = transform.layer + transform.position.Y;
                Renderer.Draw(Assets.GetTexture(tex.textureName), transform.position, null, tex.color, 0f, tex.origin, tex.scale, SpriteEffects.None, depth, tex.solidColor ? ShaderParam.FullBright : ShaderParam.None);
            }
        }
    }
}

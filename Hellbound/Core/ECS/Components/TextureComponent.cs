using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS.Components
{
    public class TextureComponent : IComponent
    {
        public readonly string textureName;
        public Vector2 origin;
        public Vector2 scale;

        public TextureComponent(string texture, Vector2? origin = null, Vector2? scale = null) 
        {
            this.scale = scale ?? Vector2.One;
            this.origin = origin ?? Vector2.Zero;
            this.textureName = texture;
        }

        public override string ToString()
        {
            return $"Texture:[\"{textureName}\", {origin}, {scale}]";
        }
    }
}

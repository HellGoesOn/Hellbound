﻿using Microsoft.Xna.Framework;
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
        public readonly Texture2D texture;
        public Vector2 origin;
        public Vector2 scale;

        public TextureComponent(string texture) 
        {
            scale = Vector2.One;
            this.textureName = texture;
            this.texture = Assets.Textures[textureName];
        }

        public override string ToString()
        {
            return $"[{textureName}]";
        }
    }
}

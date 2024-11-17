using HellTrail.Core.ECS.Components;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public class ObesitySystem : IExecute
    {
        readonly Group<Entity> _group;

        public ObesitySystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(ShawtyObese), typeof(TextureComponent)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                TextureComponent tex = entity.GetComponent<TextureComponent>();
                ShawtyObese transform = entity.GetComponent<ShawtyObese>();

                tex.scale.Y = (float)Math.Max(1, 0.5f+ Math.Abs(Math.Sin(Main.totalTime * 0.5f)));
                tex.scale.X = (float)Math.Cos(Main.totalTime * 0.5f);
            }
        }
    }
}

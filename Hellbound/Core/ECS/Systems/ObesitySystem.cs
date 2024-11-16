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
        public void Execute(Context context)
        {
            var entities = context.entities.Where(x => x != null && x.enabled && x.HasComponent<ShawtyObese>() && x.HasComponent<TextureComponent>()).ToArray();

            for (int i = 0; i < entities.Length; i++)
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

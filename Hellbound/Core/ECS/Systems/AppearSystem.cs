using Casull.Core.ECS.Components;
using Casull.Core.Overworld;
using Casull.Render;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Casull.Core.ECS
{
    public class AppearSystem : IExecute
    {
        readonly Group<Entity> _group;

        public AppearSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(AppearComponent), typeof(TextureComponent)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++) {
                var entity = entities[i];
                TextureComponent tex = entity.GetComponent<TextureComponent>();
                AppearComponent appear = entity.GetComponent<AppearComponent>();

                tex.scale.X = appear.progress;
                appear.progress += 0.1f;

                if (tex.scale.X >= 1) {
                    tex.scale.X = 1;
                    entity.RemoveComponent<AppearComponent>();
                }
            }
        }
    }
}

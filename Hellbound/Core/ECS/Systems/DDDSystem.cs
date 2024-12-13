using Casull.Core.ECS.Components;

namespace Casull.Core.ECS
{
    public class DDDSystem : IExecute
    {
        readonly Group<Entity> _group;

        public DDDSystem(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(DanceDanceDance), typeof(TextureComponent)));
        }

        public void Execute(Context context)
        {
            var entities = _group.Entities;

            for (int i = 0; i < entities.Count; i++) {
                var entity = entities[i];
                TextureComponent tex = entity.GetComponent<TextureComponent>();
                DanceDanceDance transform = entity.GetComponent<DanceDanceDance>();

                tex.scale.Y = (float)Math.Max(1, 0.5f + Math.Abs(Math.Sin(Main.totalTime * 0.5f)));
                tex.scale.X = (float)Math.Cos(Main.totalTime * 0.5f);
            }
        }
    }
}

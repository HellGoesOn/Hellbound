using HellTrail.Core.Combat;
using HellTrail.Core.Combat.Abilities;
using HellTrail.Core.Combat.Scripting;
using HellTrail.Core.DialogueSystem;
using HellTrail.Core.ECS.Components;
using HellTrail.Core.NeoCombat;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public class CreateBattleOnContactSystemNew : IExecute
    {
        readonly Group<Entity> _group;

        public CreateBattleOnContactSystemNew(Context context)
        {
            _group = context.GetGroup(Matcher<Entity>.AllOf(typeof(HasCollidedMarker), typeof(CreateBattleOnContact)));
        }

        public void Execute(Context context)
        {
            for(int i = 0; i < _group.Entities.Count; i++)
            {
                Entity entity = _group.Entities[i];
                int otherId = entity.GetComponent<HasCollidedMarker>().otherId;
                Entity otherEntity = context.entities[otherId];

                if (otherEntity == null || !otherEntity.enabled || !otherEntity.HasComponent<PlayerMarker>())
                    return;

                Main.instance.battleNew = NeoBattle.Create([otherEntity], [entity, entity]);

                SoundEngine.StartMusic("BossMusic", true);
                SoundEngine.PlaySound("Begin", 0.5f);
                GameStateManager.SetState(GameState.CombatNew, new TrippingBalls(Renderer.SaveFrame()));

            }
        }
    }
}

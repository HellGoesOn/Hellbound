using HellTrail.Core.Combat;
using HellTrail.Core.ECS.Components;
using HellTrail.Render;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS
{
    public class CreateBattleOnContactSystem : IExecute
    {
        readonly Group<Entity> _group;

        public CreateBattleOnContactSystem(Context context)
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

                CreateBattleOnContact component = entity.GetComponent<CreateBattleOnContact>();
                string[] enemyDefintions = component.enemies;
                string[] trialDefinitions = component.trialCharacters;

                List<Unit> enemyUnits = [];
                List<Unit> trialUnits = [];
                int offset = 0;
                foreach(var unit in enemyDefintions)
                {
                    Unit enemy = UnitDefinitions.Get(unit);
                    enemyUnits.Add(enemy);
                    enemy.BattleStation = new Vector2(220 + offset * 8 + ((offset / 3) * 24), 60 + offset * 32 - (offset / 3 * 86));

                    offset++;
                }

                if (trialDefinitions != null)
                    foreach (var unit in trialDefinitions)
                    {
                        trialUnits.Add(UnitDefinitions.Get(unit));
                    }
                Battle battle = Battle.Create(enemyUnits, trialUnits.Count > 0 ? trialUnits : null);
                Main.instance.battle = battle;
                battle.OnBattleEnd = () =>
                {
                    context.Destroy(entity);
                    foreach (Unit unit in GlobalPlayer.ActiveParty)
                    {
                        unit.currentAnimation = unit.defaultAnimation;
                    }
                };

                GameStateManager.SetState(GameState.Combat, new TrippingBalls(Renderer.SaveFrame()));

            }
        }
    }
}

﻿using HellTrail.Core.Combat;
using HellTrail.Core.Combat.Abilities;
using HellTrail.Core.Combat.Scripting;
using HellTrail.Core.DialogueSystem;
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
                BattleBackground bg = new BattleBackground(component.bgName);
                int offset = 0;
                foreach(var unit in enemyDefintions)
                {
                    Unit enemy = UnitDefinitions.Get(unit);
                    enemyUnits.Add(enemy);
                    enemy.BattleStation = new Vector2(220 + offset * 8 + ((offset / 3) * 24), 60 + offset * 32 - (offset / 3 * 86));

                    if (enemy.ai == null)
                        enemy.ai = new BasicAI();

                    offset++;
                }

                if (trialDefinitions != null)
                {
                    foreach (var unit in trialDefinitions)
                    {
                        trialUnits.Add(UnitDefinitions.Get(unit));
                    }
                    GlobalPlayer.DefaultBattleStations(trialUnits);
                }

                
                Battle battle = Battle.Create(enemyUnits, trialUnits.Count > 0 ? trialUnits : null);
                Main.instance.battle = battle;
                battle.bg = bg;
                battle.OnBattleEnd = () =>
                {
                    context.Destroy(entity.id);
                    SoundEngine.StartMusic("Relax", true);
                    foreach (Unit unit in GlobalPlayer.ActiveParty)
                    {
                        unit.currentAnimation = unit.defaultAnimation;
                    }
                };

                Script triedToHitThePeas = new()
                {
                    condition = (b) =>
                    {
                        Unit unit = b.unitsHitLastRound.FirstOrDefault(x => x.name == "Peas");
                        return unit != null && unit.stats.HP == unit.stats.MaxHP && b.State == BattleState.VictoryCheck;
                    },
                    action = (b) =>
                    {
                        var page1Portrait = new Portrait("EndLife", new FrameData(0, 0, 32, 32))
                        {
                            scale = new Vector2(-16, 16)
                        };
                        var page3Portrait = new Portrait("EndLife", new FrameData(0, 32, 32, 32))
                        {
                            scale = new Vector2(16, 16)
                        };
                        Dialogue dialogue = Dialogue.Create();
                        DialoguePage page = new()
                        {
                            portraits = [page1Portrait],
                            title = battle.playerParty[0].name,
                            text = "It's completely impervious to our attacks.."
                        };
                        DialoguePage page2 = new()
                        {
                            title = "Peas",
                            text = "(Pea noises)"
                        };
                        DialoguePage page3 = new()
                        {
                            portraits = [page3Portrait],
                            title = battle.playerParty[0].name,
                            text = "There's only one thing that could work.."
                        };
                        dialogue.pages.AddRange([page, page2, page3]);
                        var disturb = new Disturb()
                        {
                            spCost = 0,
                            canTarget = ValidTargets.Enemy
                        };

                        battle.playerParty[0].ClearEffects();
                        battle.playerParty[0].abilities.Add(disturb);
                        b.weaknessHitLastRound = true;
                    }
                };

                battle.scripts.Add(triedToHitThePeas);

                GameStateManager.SetState(GameState.Combat, new TrippingBalls(Renderer.SaveFrame()));

            }
        }
    }
}

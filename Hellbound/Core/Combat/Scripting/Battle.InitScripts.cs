using Casull.Core.Combat.Abilities;
using Casull.Core.Combat.Scripting;
using Casull.Core.Combat.Sequencer;
using Casull.Core.Combat.Status;
using Casull.Core.DialogueSystem;
using Casull.Core.Overworld;
using Casull.Core.UI;
using Casull.Render;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casull.Core.Combat
{
    public partial class Battle : IGameState
    {
        public Script CreateScript()
        {
            var script = new Script();
            scripts.Add(script);
            return script;
        }

        public void InitTutorialScripts()
        {
            var tutorialScript = CreateScript();

            tutorialScript.condition = (_) => !World.CheckFlag("ranTutorial");

            tutorialScript.action = (sender) => {
                World.RaiseFlag("ranTutorial");
                var sequence = CreateSequence();

                sequence.Add(new OneActionSequence(() => {
                    var dialogue = Dialogue.Create();
                    var page = new DialoguePage {
                        text = "( Welcome to the <FFFF00/Tutorial Battle> )",
                        textColor = Color.LimeGreen,
                        fillColor = Color.Black * 0.5f,
                        borderColor = Color.Transparent
                    };
                    var page2 = new DialoguePage {
                        text = "( You probably are mighty confused how you ended up in a mess like this all of a sudden! )",
                        textColor = Color.LimeGreen,
                        fillColor = Color.Black * 0.5f,
                        borderColor = Color.Transparent
                    };
                    var page3 = new DialoguePage {
                        text = "( ..or not. Either way, I won't answer that question yet. )",
                        textColor = Color.LimeGreen,
                        fillColor = Color.Black * 0.5f,
                        borderColor = Color.Transparent
                    };
                    var page4 = new DialoguePage {
                        text = "( I will however, guide you through some <FFFF00/Basic Mechanics> over upcoming short course )",
                        textColor = Color.LimeGreen,
                        fillColor = Color.Black * 0.5f,
                        borderColor = Color.Transparent
                    };
                    var page5 = new DialoguePage {
                        text = "( For now, let's start with <FFFF00/Attacking>.\nIts as easy as picking the <FFFF00/Attack command> & selecting the option of your liking with <FFFF00/[E]>" +
                        "\nYou can move the cursor around the menu with <FFFF00/[W] & [S]> )",
                        textColor = Color.LimeGreen,
                        fillColor = Color.Black * 0.5f,
                        borderColor = Color.Transparent
                    };
                    var page6 = new DialoguePage {
                        text = "( Right now, your character is filled with rage and will only be able to attack, but later you will unlock <FFFF00/other options> )",
                        textColor = Color.LimeGreen,
                        fillColor = Color.Black * 0.5f,
                        borderColor = Color.Transparent
                    };


                    dialogue.pages.AddRange([page, page2, page3, page4, page5, page6]);
                }));
            };

            var nowPickATarget = CreateScript();

            nowPickATarget.condition = (battle) => !World.CheckFlag("pickATarget") && battle.isPickingTarget;

            nowPickATarget.action = (_) => {
                var dialogue = Dialogue.Create();
                var page = new DialoguePage {
                    text = "( Now press <FFFF00/[E]> to select target. Later, you will fight more than one enemy at once and will be able to pick the one you want to beat up with <FFFF00/[W] & [S]> )",
                    textColor = Color.LimeGreen,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent
                };

                World.RaiseFlag("pickATarget");

                dialogue.pages.AddRange([page]);
            };

            var youShotFirst = CreateScript();

            youShotFirst.condition = (battle) => !World.CheckFlag("shotFirst") && World.CheckFlag("pickATarget") && battle.State == BattleState.VictoryCheck;

            youShotFirst.action = (_) => {
                var dialogue = Dialogue.Create();
                var page = new DialoguePage {
                    text = "( It will now be <FF0000/Enemy> turn. Turn order is based on <FFFF00/Speed> stat of the characters. Be careful & plan your moves accordingly! )",
                    textColor = Color.LimeGreen,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent
                };

                World.RaiseFlag("shotFirst");

                dialogue.pages.AddRange([page]);
            };

            var firstLowHp = CreateScript();

            firstLowHp.condition = (battle) => !World.CheckFlag("firstLowHp") && battle.ActingUnit.Stats.HP <= 20 && battle.State == BattleState.BeginTurn && battle.ActingUnit.team == Team.Player;

            firstLowHp.action = (_) => {
                var dialogue = Dialogue.Create();
                var page = new DialoguePage {
                    text = "( Watch out, your HP is too low! At this rate, you will be defeated! )",
                    textColor = Color.LimeGreen,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent
                };
                var page2 = new DialoguePage {
                    text = $"( Fortunately, {_.ActingUnit.name}, had the foresight of packing some <FFFF00/supplies>! )",
                    textColor = Color.LimeGreen,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent
                };
                var page3 = new DialoguePage {
                    text = $"( You can select <FFFF00/Item> command to use your <FFFF00/Chocolate Bar> to restore your HP! )",
                    textColor = Color.LimeGreen,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent
                };
                World.RaiseFlag("firstLowHp");
                UIManager.combatUI.tutorialProgress++;

                dialogue.pages.AddRange([page, page2, page3]);
            };

            var firstHeal = CreateScript();

            firstHeal.condition = (battle) => !World.CheckFlag("firstHeal") && World.CheckFlag("firstLowHp") && battle.ActingUnit.Stats.HP > 20 && battle.State == BattleState.BeginTurn && battle.ActingUnit.team == Team.Player;

            firstHeal.action = (_) => {
                var dialogue = Dialogue.Create();
                var page = new DialoguePage {
                    text = "( You will unlock other healing options later, but for now be careful. )",
                    textColor = Color.LimeGreen,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent
                };
                var page2 = new DialoguePage {
                    text = $"( If you end up being defeated in a battle, you will respawn at the last zone entrance )",
                    textColor = Color.LimeGreen,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent
                };
                var page3 = new DialoguePage {
                    text = $"( You will lose any items you consumed during the fight, so if you feel like you might lose, it may be a better idea to cut your losses and accept defeat )",
                    textColor = Color.LimeGreen,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent
                };
                World.RaiseFlag("firstHeal");
                UIManager.combatUI.tutorialProgress++;

                dialogue.pages.AddRange([page, page2, page3]);
            };

            var firstWeaknessExploit = CreateScript();

            firstWeaknessExploit.condition = (battle) => !World.CheckFlag("firstWeaknessHit") && battle.weaknessHitLastRound;

            firstWeaknessExploit.action = (_) => {
                World.RaiseFlag("firstWeaknessHit");

                // TO-DO: MC reacts?
            };

            var explainWeaknessHits = CreateScript();

            explainWeaknessHits.condition = (battle) => !World.CheckFlag("explainedWeakness") && World.CheckFlag("firstWeaknessHit") && battle.ActingUnit.team == Team.Player;

            explainWeaknessHits.action = (_) => {
                var dialogue = Dialogue.Create();
                var page = new DialoguePage {
                    text = "( Uh-oh, this enemy exploits your <FF0000/Weakness>! )",
                    textColor = Color.LimeGreen,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent
                };
                var page2 = new DialoguePage {
                    text = Ability.MarkdownElements($"( There are several main types of <FFFFFF/Elements>: Phys, Fire, Ice, Elec & Wind )"),
                    textColor = Color.LimeGreen,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent
                };
                var page3 = new DialoguePage {
                    text = $"( Each character has their own set of elemental affinities. " +
                    $"\nPossible Affinities are: <EFFFFF/Neutral>, <FF0000/Weak>, <333333/Resist>, <666666/Block> & <EFFFFF/Repel>. )",
                    textColor = Color.LimeGreen,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent
                };

                var page4 = new DialoguePage {
                    text = $"( You were hit with an element you are <FF0000/Weak> to." +
                    $"\nThis <ffff00/increases the damage you take>, as well as granting you a <ffff00/extra>\n <ffff00/action> )",
                    textColor = Color.LimeGreen,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent
                };

                var page5 = new DialoguePage {
                    text = $"( You can only gain <FFFF00/ONE> extra action per exploited weakness. Hitting <ffff00/same> enemy repeatedly <ff0000/won't grant you extra actions> )",
                    textColor = Color.LimeGreen,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent
                };

                var page6 = new DialoguePage {
                    text = $"( You can however, prevent yourself from having your weakness exploited by using <FFFF00/Guard> command." +
                    $" It will reduce the damage you take & prevent enemy from getting extra actions. Try it out! )",
                    textColor = Color.LimeGreen,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent
                };

                dialogue.pages.AddRange([page, page2, page3, page4, page5, page6]);

                World.RaiseFlag("explainedWeakness");
                UIManager.combatUI.tutorialProgress = 3;
            };

            var guarded = CreateScript();

            guarded.condition = (battle) => World.CheckFlag("explainedWeakness") && !World.CheckFlag("guarded") && ActingUnit.team == Team.Enemy && battle.State == BattleState.TurnProgression;

            guarded.action = (_) => {
                var dialogue = Dialogue.Create();
                var page = new DialoguePage {
                    text = "( Perfect, you prevented your weakness from being exploited )",
                    textColor = Color.LimeGreen,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent
                };
                var page2 = new DialoguePage {
                    text = "( Its perfect opportunity to repay in kind. )",
                    textColor = Color.LimeGreen,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent
                };
                var page3 = new DialoguePage {
                    text = $"( You might've noticed the Lighters you have in your inventory. )",
                    textColor = Color.LimeGreen,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent
                };

                var page4 = new DialoguePage {
                    text = Ability.MarkdownElements($"( Just this once I will let you on a secret: this enemy is weak to Fire!)"),
                    textColor = Color.LimeGreen,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent
                };

                dialogue.pages.AddRange([page, page2, page3, page4]);

                World.RaiseFlag("guarded");
                UIManager.combatUI.tutorialProgress++;
            };

            var connectionTerminated = CreateScript();

            connectionTerminated.condition = (battle) => !World.CheckFlag("connectionTerminated") && World.CheckFlag("guarded") && battle.weaknessHitLastRound && battle.State == BattleState.BeginTurn;

            connectionTerminated.action = (battle) => {
                var dialogue = Dialogue.Create();
                var page = new DialoguePage {
                    text = "( Great, now you know all the core basics you will ever need here! )",
                    textColor = Color.LimeGreen,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent
                };
                var page2 = new DialoguePage {
                    text = "( Don't forget what you learnt here today! I won't be able to teach you twice. )",
                    textColor = Color.LimeGreen,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent
                };
                var page3 = new DialoguePage {
                    text = $"--Connection Terminated--",
                    textColor = Color.Crimson,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent,
                    onPageBegin = (_) => {
                        SoundEngine.SetTargetMusicVolume(0.0f);
                    },
                    onPageEnd = (_) => {
                        SoundEngine.SetTargetMusicVolume(1.0f);
                    }
                };

                var page4 = new DialoguePage {
                    text = $"( ? )",
                    textColor = Color.Cyan,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent,
                };

                World.RaiseFlag("connectionTerminated");
                World.RaiseFlag("killedBird");

                dialogue.pages.AddRange([page, page2, page3, page4]);

                UIManager.combatUI.tutorialProgress++;
            };
        }
    }
}

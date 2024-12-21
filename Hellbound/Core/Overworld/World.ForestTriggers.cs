using Casull.Core.Combat;
using Casull.Core.DialogueSystem;
using Casull.Core.ECS.Components;
using Casull.Core.Graphics;
using Casull.Core.UI;
using Cyotek.Drawing.BitmapFont;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casull.Core.Overworld
{
    public partial class World : IGameState
    {
        public static void InitForestTriggers()
        {
            var aboutBoulder = new Dialogue();

            var page1 = new DialoguePage() {
                text = "Hello?",
                title = GlobalPlayer.ActiveParty[0].name
            };

            var page2 = new DialoguePage() {
                text = "'Sup",
                title = "???"
            };

            var page3 = new DialoguePage() {
                text = "So uhh.. about this boulder..",
                title = GlobalPlayer.ActiveParty[0].name
            };

            var page4 = new DialoguePage() {
                text = "'Tis piece 'o junk flew outta nowhere and landed on 'hat bridge",
                title = "???"
            };

            var page5 = new DialoguePage() {
                text = "Is anyone gonna remove it?",
                title = GlobalPlayer.ActiveParty[0].name
            };

            var page6 = new DialoguePage() {
                text = "Oy lad, 'hat anyone is right in front ye'",
                title = "???"
            };

            var page7 = new DialoguePage() {
                text = "..and how long is it gonna take you?",
                title = GlobalPlayer.ActiveParty[0].name
            };

            var page8 = new DialoguePage() {
                text = "Wouldn't take me more than a swing of ol' trusty Betsy, but I lost 'omewhere at the <ffFF00/Northen side of meadows>",
                title = "???"
            };

            var page9 = new DialoguePage() {
                text = "It's not that dangerous out there, you could just go & pick it up",
                title = GlobalPlayer.ActiveParty[0].name
            };

            var page10 = new DialoguePage() {
                text = "Lad, I would if I could. But that bloody monster would beat me to a pulp. Don't believe me? Go see for yerself",
                title = "???"
            };

            var page11 = new DialoguePage() {
                text = "Tsk.. If I bring you your 'Betsy', will you clear out the road?",
                title = GlobalPlayer.ActiveParty[0].name
            };

            var page12 = new DialoguePage() {
                text = "Sure, lad",
                title = "???"
            };

            aboutBoulder.pages.AddRange([page1, page2, page3, page4, page5, page6, page7, page8, page9, page10, page11, page12]);

            var aboutBoulderTrigger = AddTrigger("talkAboutBoulder");

            var boulderBreak = new Dialogue();
            var bb_page1 = new DialoguePage() {
                text = "Hey, I've got your 'Betsy'. Can you clear the road now?",
                title = GlobalPlayer.ActiveParty[0].name
            };
            var bb_page2 = new DialoguePage() {
                text = "Sure, lad",
                title = "???",
                onPageEnd = (sender) => {
                    UIManager.dialogueUI.darkeningPanel.fillColor = Color.Black;
                }
            };
            var bb_page3 = new DialoguePage() {
                text = "( I am very skeptical he can actually clear it in one hit.. I mean, with a rusty pickaxe?.. )",
                title = GlobalPlayer.ActiveParty[0].name,
                textColor = Color.Cyan,
                onPageEnd = (sender) => {
                    UIManager.dialogueUI.darkeningPanel.fillColor = Color.Black * 0;

                    var rock = Main.instance.ActiveWorld.context.GetAllEntities().FirstOrDefault(x => x != null && x.GetComponent<Tags>().Has("Boulder"));

                    if(rock != null) {
                        SoundEngine.PlaySound("GunShot");
                        for (int i = 0; i < 250; i++) {
                            var position = new Vector3(rock.GetComponent<Transform>().position, 0);
                            var xx = Main.rand.NextSingle() * (Main.rand.Next() % 2 == 0 ? -1 : 1) * Main.rand.Next(50, 150) * 0.01f;
                            var yy = Main.rand.NextSingle() * (Main.rand.Next() % 2 == 0 ? -1 : 1) * Main.rand.Next(25, 75) * 0.01f;
                            var zz = Main.rand.NextSingle() * -Main.rand.Next(255, 355) * 0.01f;
                            var velocity = new Vector3(xx, yy, zz);
                            var part = ParticleManager.NewParticle(position, velocity, 300, 0.1f, true);
                            if (part != null) {
                                part.diesToGravity = true;
                                part.color = Color.Lerp(Color.Yellow, Color.OrangeRed, Main.rand.NextSingle());
                                part.scale = Vector2.One * Main.rand.Next(1, 5);
                            }
                        }
                    }

                    Main.instance.ActiveWorld.context.Destroy(rock);
                }
            };
            var bb_page4 = new DialoguePage() {
                text = "HOLY SH--!!",
                title = GlobalPlayer.ActiveParty[0].name,
                onPageEnd = (sender) => {
                    UIManager.dialogueUI.darkeningPanel.fillColor = Color.Black * 0.25f;
                    RaiseFlag("boulderBroken");
                }
            };
            var bb_page5 = new DialoguePage() {
                text = "Done, lad. Road's clear.",
                title = "???"
            }; 
            var bb_page6 = new DialoguePage() {
                text = "Hey uhm, have you ever though of a becoming an adventurer?",
                title = GlobalPlayer.ActiveParty[0].name
            };

            var bb_page7 = new DialoguePage() {
                text = "Not today lad. This rock was a tough one. I need a nap after it.",
                title = "???"
            };

            boulderBreak.pages.AddRange([bb_page1, bb_page2, bb_page3, bb_page4, bb_page5, bb_page6, bb_page7]);

            aboutBoulderTrigger.condition = (_) => false;

            aboutBoulderTrigger.repeatadble = true;

            aboutBoulderTrigger.action = (_) => {
                if(CheckFlag("boulderBroken")) {

                    aboutBoulderTrigger.repeatadble = false;
                    return;
                }

                if(!GlobalPlayer.Inventory.Any(x=>x.name == "'Betsy'"))
                    UIManager.dialogueUI.dialogues.Add(aboutBoulder);
                else {
                    UIManager.dialogueUI.dialogues.Add(boulderBreak);
                    aboutBoulderTrigger.repeatadble = false;
                }
            };
        }
    }
}

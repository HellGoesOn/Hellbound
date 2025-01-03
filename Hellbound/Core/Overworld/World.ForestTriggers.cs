using Casull.Core.Combat;
using Casull.Core.DialogueSystem;
using Casull.Core.ECS.Components;
using Casull.Core.Graphics;
using Casull.Core.UI;
using Casull.Render;
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

            var sign_FoA = AddTrigger("sign_FoA");
            sign_FoA.repeatadble = true;
            sign_FoA.condition = (_) => false;

            sign_FoA.action = (_) => {
                var diag = Dialogue.Create();

                if(!CheckFlag("SunflowerIsDead"))
                diag.pages.AddRange(
                    [
                    new()
                    {
                        text = "Dangerous monster up ahead. Please turn around.",
                        fillColor = new Color(137, 41, 27),
                        borderColor = new Color(62, 7, 7)
                    }
                    ]
                    );
                else
                    diag.pages.AddRange(
                    [
                    new()
                    {
                        text = $"Dangerous monster *WAS* up ahead. I've slain him, you are welcome ;)\n\n\n-{GlobalPlayer.ActiveParty[0].name}",
                        fillColor = new Color(137, 41, 27),
                        borderColor = new Color(62, 7, 7),
                        textColor = Color.White
                    }
                    ]
                    );
            };

            var goInsideHouse = AddTrigger("loadHouse");

            goInsideHouse.repeatadble = true;
            goInsideHouse.condition = (_) => false;

            goInsideHouse.action = (_) => {

                var find = Main.instance.ActiveWorld.context.GetAllEntities().FirstOrDefault(x => x != null && x.GetComponent<Tags>().Has("loadHouseZone"));
                var plr = Main.instance.ActiveWorld.context.GetAllEntities().FirstOrDefault(x => x != null && x.GetComponent<Tags>().Has("Player"));

                if (find!= null && plr != null) {
                    find.GetComponent<Transform>().position = plr.GetComponent<Transform>().position;
                    return;
                }
            };

            var sleepyTime = AddTrigger("sleepyTime");

            sleepyTime.repeatadble = false;

            sleepyTime.condition = (_) => false;

            sleepyTime.action = (_) => {
                var diag = Dialogue.Create();

                var page1 = new DialoguePage() {
                    text = "Pink sheets?.. Who would bring a color so hard to maintain into a house that looks like its going to crumble at any moment now?",
                    title = GlobalPlayer.ActiveParty[0].name
                };
                
                var page2 = new DialoguePage() {
                    text = "And it looks like its fresh too. Someone must've changed those very recently.. I haven't seen anyone around yet though. And the house is pretty run-down. Surely they won't mind if I take a nap here, right?",
                    title = GlobalPlayer.ActiveParty[0].name
                };

                var page2_2 = new DialoguePage() {
                    text = "Pink..",
                    title = GlobalPlayer.ActiveParty[0].name
                };

                var page3 = new DialoguePage() {
                    text = "( As soon as you lay down, memories of a person you hold dear in your heart flood your wandering mind. )",
                    textColor = Color.Cyan,
                    onPageBegin = (_) => {
                        UIManager.dialogueUI.darkeningPanel.fillColor = Color.Black;
                        SoundEngine.SetTargetMusicVolume(0.0f);
                        SoundEngine.hiddenMusicVolumeSpeed = 0.015f;
                    }
                };

                var page4 = new DialoguePage() {
                    text = "( You remember your vow to make them happy no matter what it takes. )",
                    textColor = Color.Cyan
                };

                var page5 = new DialoguePage() {
                    text = "( But does that 'vow' of yours actually hold any value? Can you really fulfill it now that you've detoured so far away? Was or will there EVER be a point when you could fulfill it?)",
                    textColor = Color.Cyan
                };

                var page6 = new DialoguePage() {
                    text = "( ..does that person even want you to do that? )",
                    textColor = Color.Cyan
                };


                var page7 = new DialoguePage() {
                    text = "( ... )",
                    textColor = Color.Cyan
                };

                var page8 = new DialoguePage() {
                    text = "( ..answers to those questions don't really matter to you. Only whether you can achieve the result you hope for does. But first, you have to deal with your own unfinished business. )",
                    textColor = Color.Cyan
                };

                var page9 = new DialoguePage() {
                    text = "( Trying to get some rest is a long overdue. )",
                    textColor = Color.Cyan
                };

                var page10 = new DialoguePage() {
                    text = "..even if it will take me years, I'll still try..",
                    title = GlobalPlayer.ActiveParty[0].name,
                    onPageEnd = (_) => {
                        if(GlobalPlayer.ActiveParty.Count <= 1)
                        EndDiag();
                    }
                };

                diag.pages.AddRange([page1, page2, page3, page4, page5, page6, page7, page8, page9, page10]);

                if (GlobalPlayer.ActiveParty.Count > 1) {
                    var page11 = new DialoguePage() {
                        text = "What are you talking about?",
                        title = GlobalPlayer.ActiveParty[1].name,
                        onPageBegin = (_) => {
                            UIManager.dialogueUI.darkeningPanel.fillColor = Color.Black;
                            SoundEngine.SetTargetMusicVolume(0.0f);
                            SoundEngine.hiddenMusicVolumeSpeed = 0.015f;
                        }
                    };
                    var page12 = new DialoguePage() {
                        text = "Huh- wha- oh right, you are here too. Don't worry about that",
                        title = GlobalPlayer.ActiveParty[0].name
                    };
                    var page13 = new DialoguePage() {
                        text = "I don't really care what is it that's on your mind, but it sounded awfully personal and.. uh.. how do I put it? It was rather cringeworthy.",
                        title = GlobalPlayer.ActiveParty[1].name
                    };
                    var page14 = new DialoguePage() {
                        text = "Because it WAS personal. I forgot you were here. Now shut up. Or I'll force some chocolate down your throat, you nosy mutt.",
                        title = GlobalPlayer.ActiveParty[0].name
                    };
                    var page15 = new DialoguePage() {
                        text = "Wow, someone's got a stick up their bum. Hope it passes by the time morning hits. And calling me 'nosy mutt' when its YOU talking your mind outloud is stupid. Goodnight, 'Richard'",
                        title = GlobalPlayer.ActiveParty[1].name
                    };
                    var page16 = new DialoguePage() {
                        text = "Night, 'Scoob'",
                        title = GlobalPlayer.ActiveParty[0].name,
                        onPageEnd = (_) => {
                            EndDiag();
                        }
                    };

                    diag.pages.AddRange([page11, page12, page13, page14, page15, page16]);
                }

            };

            var shitGoBack = AddTrigger("shitGoBack");

            shitGoBack.condition = (_) => Main.currentZone == "SleepyTime" && !Main.instance.ActiveWorld.context.GetAllEntities().Any(x=>x != null && x.HasComponent<TextureComponent>() && x.GetComponent<TextureComponent>().textureName == "Peas");

            shitGoBack.action = (_) => {
                Main.instance.ActiveWorld = LoadFromFile("\\Content\\Scenes", "InsideHouse");

                var dialogue = Dialogue.Create();
                var pg1 = new DialoguePage() {
                    text = "That was.. certainly a dream",
                    title = GlobalPlayer.ActiveParty[0].name,
                };

                var pg2 = new DialoguePage() {
                    text = "What was it all about?",
                    title = GlobalPlayer.ActiveParty[1].name,
                };

                var pg3 = new DialoguePage() {
                    text = "I don't know how to explain it.. But I never saw it coming to say the least..",
                    title = GlobalPlayer.ActiveParty[0].name,
                };

                dialogue.pages.AddRange([pg1, pg2, pg3]);
            };
        }

        static void EndDiag()
        {
            UIManager.dialogueUI.darkeningPanel.fillColor = Color.Black * 0.25f;
            SoundEngine.SetTargetMusicVolume(1.0f);
            SoundEngine.hiddenMusicVolumeSpeed = 0.015f;
            Main.instance.transitions.Add(new BlackFadeInFadeOut(Renderer.SaveFrame(true)));
            Main.instance.ActiveWorld = World.LoadFromFile("\\Content\\Scenes", "SleepyTime");

            var dialogue = Dialogue.Create();

            SoundEngine.StartMusic("LWC", true);

            var pg1 = new DialoguePage() {
                text = "( For the first time in a while, you are seeing a different dream. Details are blurry and its sadly not a lucid dream.)",
                textColor = Color.Cyan
            };

            var pg2 = new DialoguePage() {
                text = "Alright, he got nowhere to run, the time for my <ff0000/vengeance> has come!",
                title = GlobalPlayer.ActiveParty[0].name,
            };

            dialogue.pages.AddRange([pg1, pg2]);
        }
    }
}

using Casull.Core.Combat;
using Casull.Core.Combat.Items.Consumables;
using Casull.Core.DialogueSystem;
using Casull.Core.ECS;
using Casull.Core.ECS.Components;
using Casull.Core.UI;
using Casull.Render;
using Microsoft.Xna.Framework;

namespace Casull.Core.Overworld
{
    public partial class World
    {
        public static void InitTriggers()
        {
            Trigger recruitDog = new("recruitDog") {
                action = (world) => {
                    var d = Dialogue.Create();

                    d.pages.AddRange(
                        [
                        new()
                        {
                        text = "*gasp* OMG!!! Who's a good boy?",
                        title = $"{GlobalPlayer.ActiveParty[0].name}"
                        },
                        new()
                        {
                        text = "Do you want to come along with me and help me annihilate my enemies with magic?",
                        title = $"{GlobalPlayer.ActiveParty[0].name}"
                        },
                        new()
                        {
                            text = "WOOF WOOF!!! WOOF WOOF!! WOOF!",
                            title = "Wolfie"
                        },
                        new()
                        {
                            text = "What do you mean you \"don't know how to use magic?\" You are a good boy in a magical forest surrounded by monsters, if you didn't know how to use magic, you'd be dead by now!",
                            title = $"{GlobalPlayer.ActiveParty[0].name}",
                        },
                        new ()
                        {
                            text = "Touche. Alright I am in",
                            title = "Wolfie",
                            onPageEnd = (dial) => {
                                var e = Main.instance.ActiveWorld.context.GetAllEntities().First(x=>x.enabled && x.HasComponent<TextureComponent>() && x.GetComponent<TextureComponent>().textureName == "WhatDaDogDoin2");
                                if (e != null)
                                    Main.instance.ActiveWorld.context.Destroy(e);
                                GlobalPlayer.AddPartyMember(UnitDefinitions.Get("Dog"));
                                flags.Add("recruittedDog");
                            }
                        },
                        new ()
                        {
                            text = "(Dog has joined your party!)"
                        }
                        ]
                        );
                }
            };

            recruitDog.condition = (w) => false;

            triggers.Add(recruitDog);

            Trigger triggerSunflower = new Trigger("sunflowerTransform") {
                repeatadble = true
            };
            triggerSunflower.condition = (w) => false;
            triggerSunflower.action = (w) => {
                var entity = w.context.GetAllEntities().FirstOrDefault(x => x.HasComponent<Tags>() && x.GetComponent<Tags>().Has("Sunflower"));

                if(entity != null) {
                    entity.AddComponent(new CameraMarker());

                    if (entity.HasComponent<NewAnimationComponent>()) {
                        var anim = entity.GetComponent<NewAnimationComponent>();

                        anim.currentAnimation = "Transformation";
                        Main.instance.GetGameState().GetCamera().SetZoomTarget(6);
                        SoundEngine.SetTargetMusicVolume(0.0f);
                        SoundEngine.hiddenMusicVolumeSpeed = 0.01f;
                    }
                }
            };

            Trigger sunflowerFight = new Trigger("sunflowerFight") {
                repeatadble = true
            };

            sunflowerFight.condition = (w) => {

                if (flags.Contains("SunflowerIsDead")) {
                    sunflowerFight.repeatadble = false;
                    sunflowerFight.activated = true;
                    return false;
                }
                    

                var e = w.context.GetAllEntities().FirstOrDefault(x => x != null && x.enabled && x.HasComponent<Tags>() && x.GetComponent<Tags>().Has("Sunflower") && x.HasComponent<NewAnimationComponent>());

                if (e != null) {
                    var anim = e.GetComponent<NewAnimationComponent>();
                    if (anim.currentAnimation == "Transformation" && anim.currentFrame == 18) {
                        return true;
                    }
                }

                return false;
            };

            sunflowerFight.action = (w) => {

                var player = w.context.GetAllEntities().FirstOrDefault(x => x.HasComponent<PlayerMarker>());
                var sunflower = w.context.GetAllEntities().FirstOrDefault(x => x.HasComponent<Tags>() && x.GetComponent<Tags>().Has("BossFight"));
                if (player != null && sunflower != null) {
                    sunflower.GetComponent<Transform>().position = player.GetComponent<Transform>().position;
                }

            };

            triggers.Add(triggerSunflower);
            triggers.Add(sunflowerFight);

            var makeRun = AddTrigger("forceRun");
            makeRun.repeatadble = false;

            makeRun.action = (sender) => {

                var entity = sender.context.GetAllEntities().FirstOrDefault(x => x != null && x.GetComponent<Tags>().Has("Player"));

                if (entity != null) {
                    var cutscene = new Cutscene();

                    var diag = new Dialogue();
                    var page1 = new DialoguePage() {
                        text = "Welp..",
                        title = GlobalPlayer.ActiveParty[0].name,
                        portraits = [new("EndLife", new FrameData(0, 32, 32, 32))
                        {
                            scale = new Vector2(10)
                        }]
                    };
                    var page2 = new DialoguePage() {
                        text = "I have officially got myself lost..",
                        title = GlobalPlayer.ActiveParty[0].name,
                        portraits = [new("EndLife", new FrameData(0, 0, 32, 32))
                        {
                            scale = new Vector2(10)
                        }]
                    };
                    var page3 = new DialoguePage() {
                        text = "Its.. No-no-no, its--its all fine all good yeah..",
                        title = GlobalPlayer.ActiveParty[0].name,
                        portraits = [new("EndLife", new FrameData(0, 32, 32, 32))
                        {
                            scale = new Vector2(-10, 10)
                        }]
                    };
                    var page4 = new DialoguePage() {
                        text = "FUCK!",
                        textColor = Color.Red,
                        textScale = new Vector2(4),
                        title = GlobalPlayer.ActiveParty[0].name,
                        portraits = [new("Dumbass_Jump", new FrameData(0, 32, 32, 32))
                        {
                            scale = new Vector2(10, 10)
                        }],
                    }; 
                    var page5 = new DialoguePage() {
                        text = "God fucking damn it..",
                        textColor = Color.Red,
                        title = GlobalPlayer.ActiveParty[0].name,
                        portraits = [new("Dumbass_Facepalm", new FrameData(0, 32, 32, 32))
                        {
                            scale = new Vector2(10, 10)
                        }],
                    }; 
                    var page6 = new DialoguePage() {
                        text = "I guess I'll just keep going on and eventually end up somewhere",
                        textColor = Color.White,
                        title = GlobalPlayer.ActiveParty[0].name,
                        portraits = [new("Dumbass_Facepalm", new FrameData(0, 32, 32, 32))
                        {
                            scale = new Vector2(10, 10)
                        }],
                    };
                    var page7 = new DialoguePage() {
                        text = "Not the best plan I could have, but its the only one I get..",
                        textColor = Color.White,
                        title = GlobalPlayer.ActiveParty[0].name,
                        portraits = [new("EndLife", new FrameData(0, 32, 32, 32))
                        {
                            scale = new Vector2(10, 10)
                        }],
                    };
                    var page8 = new DialoguePage() {
                        text = "..this day couldn't get any worse",
                        textColor = Color.White,
                        title = GlobalPlayer.ActiveParty[0].name,
                        portraits = [new("EndLife", new FrameData(0, 0, 32, 32))
                        {
                            scale = new Vector2(10, 10)
                        }],
                    };
                    diag.pages.AddRange(
                        [
                            page1,
                            page2,
                            page3,
                            page4,
                            page5,
                            page6,
                            page7,
                            page8
                        ]);
                    cutscene.BeginTransition(new BlackFadeInFadeOut(Renderer.SaveFrame()));
                    cutscene.Add(new SetPosition(entity, new Vector2(5 * 32, 14 * 32-10)));
                    cutscene.Add(new SetComponent(entity, new PlayerMarker() { preserveSpeed = true}));
                    cutscene.SetFollowing(entity, 1);
                    cutscene.Add(new SetComponent(entity, new Velocity(1f, 0)));
                    cutscene.Add(new Timer(180));
                    cutscene.Add(new SetComponent(entity, new PlayerMarker() { preserveSpeed = false }));
                    cutscene.Add(new StartDialogue(diag));
                    cutscene.Add(new SpawnEntityFromPrefab(Main.instance.ActiveWorld.context, "Slime", new Vector2(18 * 32, 14 * 32 - 10), out var e));
                    cutscene.Add(new FireAction(() => {
                        e.GetComponent<TextureComponent>().scale = new Vector2(-1, 1);
                        e.GetComponent<Encounter>().enemies = ["Dud", "Slime"];
                    }));
                    cutscene.SetFollowing(e, 0.05f);
                    cutscene.Add(new SetComponent(e, new Velocity(-0.5f, 0)));
                    cutscene.Add(new Timer(120));
                    cutscene.Add(new SetComponent(e, new Velocity(0, 0)));
                    cutscene.Add(new Timer(60));
                    cutscene.SetFollowing(entity, 0.1f);

                    var rage1 = new DialoguePage() {
                        text = "Oh, would you look at that!",
                        textColor = Color.White,
                        title = GlobalPlayer.ActiveParty[0].name,
                        portraits = [new("EndLife", new FrameData(0, 0, 32, 32))
                        {
                            scale = new Vector2(10, 10)
                        }]
                        }; 
                    var rage2 = new DialoguePage() {
                            text = "A punching bag to take out my UNYIELDING RAGE on",
                            textColor = Color.Yellow,
                            textScale = new Vector2(2),
                            title = GlobalPlayer.ActiveParty[0].name,
                            portraits = [new("EndLife", new FrameData(0, 0, 32, 32))
                        {
                            scale = new Vector2(10, 10)
                        }]
                        };
                    var rage3 = new DialoguePage() {
                        text = "GET YOUR SLIMY ASS OVER HERE, YOU CUNT",
                        textColor = Color.Red,
                        textScale = new Vector2(2),
                        title = GlobalPlayer.ActiveParty[0].name,
                        portraits = [new("EndLife", new FrameData(0, 0, 32, 32))
                        {
                            scale = new Vector2(10, 10)
                        }]
                    };
                    cutscene.Add(new StartDialogue(new() {
                        pages =
                        [
                            rage1, rage2, rage3
                        ]
                    }));
                    cutscene.Add(new SetComponent(entity, new Velocity(2, 0)));
                    cutscene.Add(new SetComponent(e, new Velocity(0.5f, 0)));
                    cutscene.Add(new FireAction(() => {
                        e.GetComponent<TextureComponent>().scale = new Vector2(1, 1);
                    }));
                    cutscene.Add(new SetComponent(entity, new PlayerMarker() { preserveSpeed = true }));
                    cutscene.Add(new Timer(160));
                    cutscene.Add(new SetComponent(entity, new PlayerMarker() { preserveSpeed = false }));


                    StartCutscene(cutscene);
                }
            };

            var trigger = AddTrigger("unlockInventory");

            trigger.condition = (sender) => UIManager.combatUI.tutorialProgress > 1 && !CheckFlag("unlockInventory");

            trigger.action = (sender) => {

                var unlockedInventory = Dialogue.Create();
                var page = new DialoguePage {
                    text = "( You can now access your <ffff00/Inventory> & <ffff00/Party Menu> by pressing <ffff00/[ESC]>. Use it to heal or otherwise prepare out of battle when neccessary! )",
                    textColor = Color.LimeGreen,
                    fillColor = Color.Black * 0.5f,
                    borderColor = Color.Transparent
                };

                RaiseFlag("unlockInventory");

                unlockedInventory.pages.Add(page);
                var cutscene = new Cutscene();
                cutscene.Add(new Timer(60));
                cutscene.Add(new StartDialogue(unlockedInventory));
                GlobalPlayer.AddItem(new Lighter() { count = 3});

                StartCutscene(cutscene);

            };

            var pizza = AddTrigger("pizza");

            pizza.condition = (sender) => false;

            pizza.action = (sender) => {

                GlobalPlayer.AddItem(new InfinitePizza());
                SoundEngine.PlaySound("Pickup", 1f);
                RaiseFlag("pizza");

            };

            var meadowBirdAttack = AddTrigger("meadowBirdAttack");

            meadowBirdAttack.repeatadble = true;

            meadowBirdAttack.condition = (sender) => Main.currentZone == "ForestMeadow" && Main.instance.transitions.Count <= 0 && !CheckFlag("killedBird") && cutscenes.Count <= 0;

            meadowBirdAttack.action = (sender) => {

                var cutscene = new Cutscene();

                var diag = new Dialogue();
                var page = new DialoguePage() {
                    text = "Okay, now I am getting somewhere I've yet to check out.",
                    title = GlobalPlayer.ActiveParty[0].name,
                    portraits = [new("Dumbass", new FrameData(0, 0, 32, 32))
                        {
                            scale = new Vector2(10)
                        }]
                };

                var page2 = new DialoguePage() {
                    text = "Doesn't seem too bad.",
                    title = GlobalPlayer.ActiveParty[0].name,
                    portraits = [new("Dumbass", new FrameData(0, 0, 32, 32))
                        {
                            scale = new Vector2(10)
                        }]
                };

                var page3 = new DialoguePage() {
                    text = "*HAWK*",
                    title = "???"
                };

                var page4 = new DialoguePage() {
                    text = "What was that?!",
                    title = GlobalPlayer.ActiveParty[0].name,
                    portraits = [new("Dumbass", new FrameData(0, 0, 32, 32))
                        {
                            scale = new Vector2(10)
                        }]
                };

                var player = sender.context.GetAllEntities().FirstOrDefault(x => x.HasComponent<PlayerMarker>());

                diag.pages.AddRange([page, page2, page3, page4]);

                cutscene.Add(new Timer(120));
                cutscene.Add(new StartDialogue(diag));
                cutscene.Add(new SpawnEntityFromPrefab(sender.context, "Bird", Vector2.Zero, out var bird));
                cutscene.Add(new FireAction(() => {
                    bird.GetComponent<TextureComponent>().scale.X = -1;
                }));
                cutscene.Add(new SetComponent(bird, new Transform(player.GetComponent<Transform>().position + new Vector2(240, -34))));
                cutscene.Add(new SetComponent(bird, new Velocity(-4, 0.5f)));
                cutscene.SetFollowing(bird, 0.25f);
                cutscene.Add(new Timer(120));

                StartCutscene(cutscene);
            };
        }
    }
}

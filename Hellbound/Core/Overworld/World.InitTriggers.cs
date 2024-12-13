using Casull.Core.Combat;
using Casull.Core.DialogueSystem;
using Casull.Core.ECS.Components;
using Microsoft.Xna.Framework;

namespace Casull.Core.Overworld
{
    public partial class World
    {
        public static void InitTriggers()
        {
            Trigger whatTheFuckDialogue = new("diag1") {
                action = (world) => {
                    var d = Dialogue.Create();

                    var firstPage = new DialoguePage {
                        text = "A lovely day today innit?",
                        title = $"{GlobalPlayer.ActiveParty[0].name}"
                    };

                    var secondPage = new DialoguePage {
                        textColor = Color.Yellow,
                        text = "Sure hope nothing bad happens",
                        title = $"{GlobalPlayer.ActiveParty[0].name}"
                    };

                    d.pages.AddRange([firstPage, secondPage]);
                }
            };

            whatTheFuckDialogue.condition = (w) => false;

            triggers.Add(whatTheFuckDialogue);

            Trigger whatTheFuckDialogue2 = new("diag2") {
                action = (world) => {
                    var d = Dialogue.Create();

                    var firstPage = new DialoguePage {
                        text = "By the heavens, what an ugly bastard",
                        title = $"{GlobalPlayer.ActiveParty[0].name}"
                    };

                    var secondPage = new DialoguePage {
                        text = "I should slay it for its own good",
                        title = $"{GlobalPlayer.ActiveParty[0].name}"
                    };

                    d.pages.AddRange([firstPage, secondPage]);
                }
            };

            whatTheFuckDialogue2.condition = (w) => false;

            triggers.Add(whatTheFuckDialogue2);

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
                                var e = Main.instance.ActiveWorld.context.entities.First(x=>x.enabled && x.HasComponent<TextureComponent>() && x.GetComponent<TextureComponent>().textureName == "WhatDaDogDoin2");
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
        }
    }
}

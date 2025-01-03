using Casull.Core.Combat;
using Casull.Core.DialogueSystem;
using Casull.Core.ECS.Components;
using Casull.Core.UI;
using Casull.Render;
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
        static float opacityChange = 1.0f;
        public static void InitNightmare()
        {
            var nightmare = new Cutscene();

            var diag = new Dialogue();

            var page1 = new DialoguePage() {
                text = "For years, I've slept dreamless nights. My mind <ffff00/was> sound.",
                title = $"???",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var page2 = new DialoguePage() {
                text = "..until very recently.",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };
            var page3 = new DialoguePage() {
                text = "A <ff0000/nightmare> has begun haunting me.",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            diag.pages.AddRange([page1, page2, page3]);
            nightmare.Add(new FireAction(() => {
                nightmare.InternalSetFollowing(Main.instance.ActiveWorld.context.GetById(0), 1f);
            }));
                nightmare.Add(new FireAction(() => {
                SoundEngine.StartMusic("Nightmare", true);
                UIManager.dialogueUI.darkeningPanel.fillColor = Color.Transparent;
            }));
            nightmare.Add(new Timer(180));
            nightmare.Add(new StartDialogue(diag));
            nightmare.Add(new FireActionFor(() => {
                if (opacityChange > 0)
                    opacityChange -= 0.1f;

                Main.instance.ActiveWorld.context.GetAllEntities().FirstOrDefault(x => x != null && x.GetComponent<Tags>().Has("Darkness")).GetComponent<TextureComponent>().color = Color.Black * opacityChange;
            }, 60));

            var describe = new Dialogue();

            var describePage1 = new DialoguePage() {
                text = "A tall shadowy figure with no discernible features. Not any that I was able to remember upon waking.",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var describePage2 = new DialoguePage() {
                text = "It.. glares at me. Its intents unclear, but a sense of <ff0000/danger> overruns my mind. And then, it speaks.",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            describe.pages.AddRange([describePage1, describePage2]);

            nightmare.Add(new StartDialogue(describe));


            var diag2 = new Dialogue();

            var diag2Page1 = new DialoguePage() {
                text = "...",
                title = $"???",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                speakerColor = Color.Crimson,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var diag2Page2 = new DialoguePage() {
                text = "..I..",
                title = $"???",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                speakerColor = Color.Crimson,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var diag2Page3 = new DialoguePage() {
                text = "..see..",
                title = $"???",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                speakerColor = Color.Crimson,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var diag2Page4 = new DialoguePage() {
                text = "..<ff0000/you>..",
                title = $"???",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                speakerColor = Color.Crimson,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent,
                onPageBegin = (sender) => {

                    Main.instance.ActiveWorld.context.GetAllEntities().FirstOrDefault(x => x != null && x.GetComponent<Tags>().Has("Shadow")).GetComponent<NewAnimationComponent>().frames = [new(0, 48, 48, 48, 30)];
                }
            };

            diag2.pages.AddRange([diag2Page1, diag2Page2, diag2Page3, diag2Page4]);

            nightmare.Add(new StartDialogue(diag2)); 
            nightmare.Add(new FireActionFor(() => {
                if (opacityChange < 1.0f)
                    opacityChange += 0.1f;
                Main.instance.ActiveWorld.context.GetAllEntities().FirstOrDefault(x => x != null && x.GetComponent<Tags>().Has("Darkness")).GetComponent<TextureComponent>().color = Color.Black * opacityChange;
            }, 60));

            var diag3 = new Dialogue();

            var diag3Page = new DialoguePage() {
                text = "Its eyes glow. Mouth doesn't quite 'open'. Instead, it 'rips' & distorts. Rest of its body twitches and changes shape.",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var diag3Page0 = new DialoguePage() {
                text = ".. and then I wake up.",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var diag3Page1 = new DialoguePage() {
                text = "I tried drugs, therapy, every sort of treatment I could think of. Nothing worked. I stopped believing in it being caused by my mind.",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var diag3Page2 = new DialoguePage() {
                text = "Rather, I believe it to be <ffff00/supernatural>.",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var diag3Page3 = new DialoguePage() {
                text = "I've since found obscure forums & similar cases of 'insomnia' from the past during search for any other treatment. Most of it was just internet junk.",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var diag3Page4 = new DialoguePage() {
                text = "But in a sudden turn of events, just a few weeks since I began my research, I've found a <ffff00/promising clue>",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };
            
            var diag3Page5 = new DialoguePage() {
                text = "Multiple similar cases have been reported in last 5 years in an obscure town. Local clinic's patients reporting having exactly the same nigthmare.",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };
            
            var diag3Page6 = new DialoguePage() {
                text = "So called '<ffff00/Volchiy>'. A town otherwise so unremarkable that most maps fail to acknowledge its very existence.",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var diag3Page7 = new DialoguePage() {
                text = "I was fortunate enough to pin point its approximate location. I've since set off to find it. ",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var diag3Page8 = new DialoguePage() {
                text = "Its entirely surrounded by a <ffff00/monster infested Forest>. Its the only known entrance to the town. There is an issue however..",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };


            diag3.pages.AddRange([diag3Page, diag3Page0, diag3Page1, diag3Page2, diag3Page3, diag3Page4, diag3Page5, diag3Page6, diag3Page7, diag3Page8]);

            nightmare.Add(new StartDialogue(diag3));

            nightmare.Add(new Timer(30));

            nightmare.Add(new FireAction(() => {
                SoundEngine.SetTargetMusicVolume(1.0f);
                Main.instance.ActiveWorld = LoadFromFile("\\Content\\Scenes\\", "Forest3");
                GameStateManager.SetState(GameState.Overworld, new BlackFadeInFadeOut(Renderer.SaveFrame()));
                UIManager.dialogueUI.darkeningPanel.fillColor = Color.Black * 0.25f;
                SoundEngine.StartMusic("ChangingSeasons", true);

            }));

            var triggerNightmare = AddTrigger("Nightmare");

            triggerNightmare.condition = (sender) => Main.currentZone == "Nightmare" && !CheckFlag("nigthmare!");

            triggerNightmare.action = (sender) => {
                StartCutscene(nightmare);
            };

            var nightmare2 = new Cutscene();
            nightmare2.Add(new FireAction(() => {
                nightmare.InternalSetFollowing(Main.instance.ActiveWorld.context.GetById(0), 1f);
            }));
            nightmare2.Add(new FireAction(() => {
                SoundEngine.StartMusic("Nightmare", true);
                UIManager.dialogueUI.darkeningPanel.fillColor = Color.Transparent;
            }));

            var n2_diag1 = new Dialogue();

            var n2_diag1_page1 = new DialoguePage() {
                text = "...",
                title = $"???",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                speakerColor = Color.Crimson,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var n2_diag1_page2 = new DialoguePage() {
                text = "..fascinating..",
                title = $"???",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                speakerColor = Color.Crimson,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var n2_diag1_page3 = new DialoguePage() {
                text = "..after all those years you've finally made a friend..",
                title = $"???",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                speakerColor = Color.Crimson,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var n2_diag1_page4 = new DialoguePage() {
                text = "..defeated a 'formidable' foe..",
                title = $"???",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                speakerColor = Color.Crimson,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var n2_diag1_page5 = new DialoguePage() {
                text = "..and you've come closer than ever to me..",
                title = $"???",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                speakerColor = Color.Crimson,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var n2_diag1_page6 = new DialoguePage() {
                text = "..ha..hahahahahaha..",
                title = $"???",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                speakerColor = Color.Crimson,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var n2_diag1_page7 = new DialoguePage() {
                text = "..I am looking forward to seeing you <ffff00/in person>..",
                title = $"???",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                speakerColor = Color.Crimson,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            n2_diag1.pages.AddRange([n2_diag1_page1, n2_diag1_page2, n2_diag1_page3, n2_diag1_page4, n2_diag1_page5, n2_diag1_page6, n2_diag1_page7]);

            nightmare2.Add(new Timer(180));
            nightmare2.Add(new StartDialogue(diag));
            nightmare2.Add(new FireActionFor(() => {
                if (opacityChange > 0)
                    opacityChange -= 0.1f;

                Main.instance.ActiveWorld.context.GetAllEntities().FirstOrDefault(x => x != null && x.GetComponent<Tags>().Has("Darkness")).GetComponent<TextureComponent>().color = Color.Black * opacityChange;
            }, 60));

            nightmare2.Add(new FireActionFor(() => {
                if (opacityChange < 1.0f)
                    opacityChange += 0.1f;
                Main.instance.ActiveWorld.context.GetAllEntities().FirstOrDefault(x => x != null && x.GetComponent<Tags>().Has("Darkness")).GetComponent<TextureComponent>().color = Color.Black * opacityChange;
            }, 60));
        }
    }
}

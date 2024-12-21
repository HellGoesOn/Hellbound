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
                text = "Every night without a fail..",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var page2 = new DialoguePage() {
                text = ".. I see the same godforsaken dream..",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            diag.pages.AddRange([page1, page2]);
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
                text = "..a tall shadowy figure with an unsettling aura..",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var describePage2 = new DialoguePage() {
                text = "..taunting my very existance..",
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
                text = "..do you really believe that you control your <FF0000/destiny>?..",
                title = $"???",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                speakerColor = Color.Crimson,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var diag2Page3 = new DialoguePage() {
                text = "..if you truly are that naive..",
                title = $"???",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                speakerColor = Color.Crimson,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var diag2Page4 = new DialoguePage() {
                text = "..come find me..",
                title = $"???",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                speakerColor = Color.Crimson,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var diag2Page5 = new DialoguePage() {
                text = "..we'll see what that puny excuse of '<ffff00/free will>' of your can really achieve..",
                title = $"???",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                speakerColor = Color.Crimson,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent,
                onPageBegin = (sender) => {
                    Main.instance.ActiveWorld.context.GetById(0).GetComponent<NewAnimationComponent>().frames = [new(0, 0, 48, 48, 0)];
                },
                onPageEnd = (sender) => {
                    Main.instance.ActiveWorld.context.GetById(0).GetComponent<NewAnimationComponent>().frames = [new(0, 48, 48, 48, 0)];
                }
            };

            diag2.pages.AddRange([diag2Page1, diag2Page2, diag2Page3, diag2Page4, diag2Page5]);

            nightmare.Add(new StartDialogue(diag2)); 
            nightmare.Add(new FireActionFor(() => {
                if (opacityChange < 1.0f)
                    opacityChange += 0.1f;
                Main.instance.ActiveWorld.context.GetAllEntities().FirstOrDefault(x => x != null && x.GetComponent<Tags>().Has("Darkness")).GetComponent<TextureComponent>().color = Color.Black * opacityChange;
            }, 60));

            var diag3 = new Dialogue();

            var diag3Page = new DialoguePage() {
                text = "..the details after that are a total blur..",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var diag3Page0 = new DialoguePage() {
                text = "..but whatever happens to me next is most certainly <ff0000/death>..",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var diag3Page1 = new DialoguePage() {
                text = "If I want those nightmares to end, I'll have to find that <FF0000/bastard>!!..",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var diag3Page2 = new DialoguePage() {
                text = "..I've looked through every single database that I could to find any information about <FF0000/him>..",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var diag3Page3 = new DialoguePage() {
                text = "..and after years of torture, I have finally found a promising lead..",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            var diag3Page4 = new DialoguePage() {
                text = "..there is just one problem though..",
                title = $"{GlobalPlayer.ActiveParty[0].name}",
                borderColor = Color.Transparent,
                fillColor = Color.Black * 0.35f,
                textColor = Color.Cyan,
                speakerFillColor = Color.Transparent,
                speakerBorderColor = Color.Transparent
            };

            diag3.pages.AddRange([diag3Page, diag3Page0, diag3Page1, diag3Page2, diag3Page3, diag3Page4]);

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

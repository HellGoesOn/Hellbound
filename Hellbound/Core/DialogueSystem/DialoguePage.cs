﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Casull.Core.DialogueSystem
{
    public class DialoguePage
    {
        public int progress;
        public int elapsedTime;
        public int timePerLetter;
        public int currentResponse;
        public bool active;
        public bool finishedScrolling;
        public string text;
        public string title;

        int autoDelay;

        public int TimeLeft => autoDelay / 60;

        public Color textColor;
        public Color speakerColor;
        public Color borderColor;
        public Color fillColor;
        public Color speakerBorderColor;
        public Color speakerFillColor;
        public List<Response> responses = [];
        public List<Portrait> portraits = [];
        public ResponseDelegate onPageEnd;
        public ResponseDelegate onPageBegin;

        public Vector2 textScale;

        bool began = false;

        public DialoguePage()
        {
            autoDelay = 60;
            textScale = Vector2.One;
            textColor = Color.White;
            speakerColor = Color.White;
            speakerBorderColor = borderColor = Color.White;
            speakerFillColor = fillColor = Color.DarkBlue;
            text = "";
            title = "";
            timePerLetter = 0;
        }

        public void Update(Dialogue dialogue)
        {
            if(!began) {
                began = true;
                onPageBegin?.Invoke(dialogue);
            }

            if (!finishedScrolling && ++elapsedTime >= timePerLetter) {
                if (Input.HeldKey(Keys.E)) {
                    for(int i = 0; i < 4; i++) {
                        progress++;
                        if (progress < text.Length && text[progress] == '<') {
                            while (progress < text.Length && text[progress] != '>')
                                progress++;
                        }
                    }
                }

                if (progress < text.Length && text[progress] == '<') {
                    while(progress < text.Length && text[progress] != '>')
                        progress++;
                }

                if (++progress >= text.Length) {
                    progress = text.Length;
                    finishedScrolling = true;
                }
                elapsedTime = 0;
            }

            if (Dialogue.auto && finishedScrolling)
                autoDelay--;
            else
                autoDelay = 200;

            if ((finishedScrolling && Input.PressedKey(Keys.E)) || (Dialogue.auto && autoDelay <= 0) || (Dialogue.auto && Input.HeldKey(Keys.Space))) {
                if (responses.Count > 0) {
                    responses[currentResponse].OnUseResponse(dialogue);
                }
                else {
                    onPageEnd?.Invoke(dialogue);
                    if (++dialogue.currentPage > dialogue.pages.Count - 1) {
                        dialogue.currentPage = dialogue.pages.Count - 1;
                        dialogue.hasEnded = true;
                    }
                }
            }
        }

        public string VisibleText => text[..progress];
    }
}

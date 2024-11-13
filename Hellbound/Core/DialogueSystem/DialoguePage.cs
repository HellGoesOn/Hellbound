﻿using Microsoft.Xna.Framework.Input;

namespace HellTrail.Core.DialogueSystem
{
    public class DialoguePage
    {
        public int progress;
        public int elapsedTime;
        public int timePerLetter;
        public bool active;
        public bool finishedScrolling;
        public string text;
        public string title;

        public int currentResponse;
        public List<Response> responses = [];

        public DialoguePage()
        {
            text = "";
            title = "";
            timePerLetter = 0;
        }

        public void Update(Dialogue dialogue)
        {
            if (!finishedScrolling && ++elapsedTime >= timePerLetter)
            {
                if (Input.HeldKey(Keys.E))
                    progress += 4;

                if(++progress >= text.Length)
                {
                    progress = text.Length;
                    finishedScrolling = true;
                }
                elapsedTime = 0;
            }

            if (finishedScrolling && Input.PressedKey(Keys.E))
            {
                if(responses.Count > 0)
                {
                    responses[currentResponse].OnUseResponse(dialogue);
                }
                else
                {
                    if(++dialogue.currentPage > dialogue.pages.Count-1)
                    {
                        dialogue.currentPage = dialogue.pages.Count-1;
                        dialogue.hasEnded = true;
                    }
                }
            }
        }

        public string VisibleText => text[..progress];
    }
}

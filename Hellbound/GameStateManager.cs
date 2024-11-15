using HellTrail.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail
{
    public static class GameStateManager
    {
        public static GameState State { get; set; } = GameState.Combat;

        public static void SetState(GameState newState, Transition transition = null)
        {
            Main.instance.transitions.Add(transition ?? new(Renderer.SaveFrame()));
            State = newState;
            switch(State)
            {
                case GameState.MainMenu:
                    // change to main menu, play neccessary animations, do setup etc
                    break;
                case GameState.Combat:
                    // transition to doing combat
                    break;
                case GameState.Overworld:
                    // transition to OW
                    break;
            }
        }
    }

    public enum GameState
    {
        MainMenu,
        Overworld,
        Combat
    }
}

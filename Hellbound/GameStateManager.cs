using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail
{
    public static class GameStateManager
    {
        public static GameState State { get; set; } = GameState.MainMenu;

        public static void ChangeState(GameState newState)
        {
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

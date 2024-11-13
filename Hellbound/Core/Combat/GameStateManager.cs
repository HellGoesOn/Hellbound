using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat
{
    public static class GameStateManager
    {
        public static GameState GameState { get; private set; }

        public static void ChangeState(GameState newState)
        {
            GameState = newState;
            switch (GameState)
            {
                case GameState.MainMenu:
                    break;
                case GameState.Overworld:
                    break;
                case GameState.Battle:
                    break;
            }       
        }
    }

    public enum GameState
    {
        MainMenu,
        Overworld,
        Battle
    }
}

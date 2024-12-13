using Casull.Core.UI;
using Casull.Render;

namespace Casull
{
    public static class GameStateManager
    {
        public static GameState State { get; set; } = GameState.Combat;

        private static GameState _nextState;

        public static void SetState(GameState newState, Transition transition = null)
        {
            Main.instance.transitions.Add(transition ?? new(Renderer.SaveFrame()));
            _nextState = newState;
            switch (newState) {
                case GameState.MainMenu:
                    var state = UIManager.GetStateByName("MainMenu");
                    if (state != null) {
                        (state as MainMenuUI).mainMenu = null;
                        UIManager.UIStates.Remove(state);
                    }
                    Main.instance.mainMenu = new MainMenu();
                    // change to main menu, play neccessary animations, do setup etc
                    break;
                case GameState.Combat:
                    // transition to doing combat
                    break;
                case GameState.Overworld:
                    // transition to OW
                    break;
                case GameState.CombatNew:
                    break;
            }
        }

        public static void Update()
        {
            if (_nextState != GameState.Dummy) {
                State = _nextState;
                _nextState = GameState.Dummy;
            }
        }
    }

    public enum GameState
    {
        Dummy,
        MainMenu,
        Overworld,
        Combat,
        CombatNew
    }
}

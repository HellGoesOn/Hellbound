namespace Casull.Core.ECS.Components
{
    public class Encounter : IComponent
    {
        public string bgName;
        public string[] enemies;
        public string[] trialCharacters;

        public Encounter(string bgName, string[] enemies, string[] trialCharacters)
        {
            this.bgName = bgName ?? "TestBG";
            this.enemies = enemies;
            this.trialCharacters = trialCharacters;
        }

        public override string ToString()
        {
            string enemy = enemies != null ? string.Join(", ", enemies) : "";
            string trial = trialCharacters != null ? string.Join(", ", trialCharacters) : "";
            return $"Combat: [{enemy}], [{trial}]";
        }
    }
}

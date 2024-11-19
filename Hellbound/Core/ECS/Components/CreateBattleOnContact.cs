using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.ECS.Components
{
    public class CreateBattleOnContact : IComponent
    {
        public string[] enemies;
        public string[] trialCharacters;

        public CreateBattleOnContact(string[] enemies, string[] trialCharacters)
        {
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

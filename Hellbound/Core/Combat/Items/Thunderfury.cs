using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat.Items.Consumables
{
    public class Thunderfury : Item
    {
        public Thunderfury() : base("[Thunderfury, Blessed Blade of the Windseeker]", "Did someone say...")
        {
            icon = "thunderfuryl";
            frames = [new FrameData(0, 0, 52, 48)];
            iconScale *= 2;
            canTarget = ValidTargets.World;
            //iconRotation = MathHelper.PiOver4;
            iconOrigin = new Vector2(26, 24);

            onViewed = (item) =>
            {
                item.iconScale *= 1.05f;
            };
        }

        protected override void OnUse(Unit caster, Battle battle, List<Unit> targets)
        {
            base.OnUse(caster, battle, targets);

            SoundEngine.PlaySound("clangberserk", 3);
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellTrail.Core.Combat
{
    public class Ability
    {
        public string Name;
        public string Description;
        public bool aoe;
        public ValidTargets canTarget;

        public Ability(string name, string description)
        {
            aoe = false;
            Name = name;
            Description = description;
            canTarget = ValidTargets.Active;
        }

        public virtual void Use(Unit caster, Battle battle, List<Unit> targets)
        {

        }
    }

    public class SewerSide : Ability
    {
        public SewerSide() : base("Sewerside", "You should harm yourself NOW")
        {
            canTarget = ValidTargets.Ally;
        }

        public override void Use(Unit caster, Battle battle, List<Unit> targets)
        {
            targets[0].HP = Math.Max(0, targets[0].HP - 200);
            battle.lastAction = $"{caster.name} used {Name}!";
        }
    }

    public class Bite : Ability
    {
        public Bite() : base("Bite", "Damage")
        {
            aoe = false;
            canTarget = ValidTargets.Enemy;
        }

        public override void Use(Unit caster, Battle battle, List<Unit> targets)
        {
            battle.lastAction = $"{caster.name} used {Name}!";

            int markiplier = targets[0].BattleStation.X < caster.BattleStation.X ? -1 : 1;

            Sequence sequence = new Sequence(battle);
            sequence.Add(new MoveActorSequence(caster, targets[0].position - new Vector2(32 * markiplier, 0)));
            sequence.Add(new DelaySequence(20));
            sequence.Add(new PlaySoundSequence("GunShot", 0.75f));
            sequence.Add(new DoDamageSequence(targets[0], 25));
            sequence.Add(new DelaySequence(20));
            sequence.Add(new MoveActorSequence(caster, caster.BattleStation));
            battle.sequences.Add(sequence);
        }
    }

    public class Megidolaon : Ability
    {
        public Megidolaon() : base("Super Bite", "I BRING YOU: MEGIDOLAON")
        {
            aoe = true;
            canTarget = ValidTargets.Enemy;
        }

        public override void Use(Unit caster, Battle battle, List<Unit> targets)
        {
            battle.lastAction = $"{caster.name} used {Name}!";

            Sequence sequence = new Sequence(battle);
            sequence.Add(new MoveActorSequence(caster, new Vector2(160, 90)));
            sequence.Add(new PlaySoundSequence("WhatDaDogDoin", 0.25f));
            sequence.Add(new DelaySequence(60));
            foreach (Unit target in targets)
            {
                sequence.Add(new DelaySequence(10));
                sequence.Add(new DoDamageSequence(target, 25));
                sequence.Add(new PlaySoundSequence("GunShot", 0.5f));
            }
            sequence.Add(new DelaySequence(60));
            sequence.Add(new MoveActorSequence(caster, caster.BattleStation));
            battle.sequences.Add(sequence);

        }
    }

    public class GrandSeal : Ability
    {
        public GrandSeal() : base("The Grand Seal", "I BRING YOU: MEGIDOLAON")
        {
            canTarget = ValidTargets.Ally;
        }

        public override void Use(Unit caster, Battle battle, List<Unit> targets)
        {
            battle.lastAction = $"{caster.name} used {Name}!";

            Sequence sequence = new Sequence(battle);
            sequence.Add(new MoveActorSequence(caster, new Vector2(160, 90)));
            sequence.Add(new DelaySequence(60));
            foreach (Unit target in targets)
            {
                sequence.Add(new DelaySequence(4));
                sequence.Add(new DoDamageSequence(target, 200));
                sequence.Add(new PlaySoundSequence("Death", 0.25f));
            }
            sequence.Add(new DelaySequence(60));
            sequence.Add(new MoveActorSequence(caster, caster.BattleStation));
            battle.sequences.Add(sequence);

        }
    }
}

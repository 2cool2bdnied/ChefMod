﻿using ChefMod;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityStates.Chef
{
    public class PrepBlaze : PrepSear
    {
        public override void NextState()
        {
            this.outer.SetNextState(new FireBlaze());
        }
    }

    public class FireBlaze : FireSear
    {
        public static new float damageCoefficient = 4.2f;
        public override void ModifyBullet(BulletAttack ba)
        {
            ba.damage = base.damageStat * FireBlaze.damageCoefficient;
            ba.AddModdedDamageType(chefPlugin.chefFireballOnHit);
            ba.AddModdedDamageType(chefPlugin.chefSear);
        }
    }
}

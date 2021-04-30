﻿using System;
using System.Collections.Generic;
using EntityStates;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using ChefMod;
using System.Runtime.InteropServices;

namespace EntityStates.Chef
{
    class Julienne : BaseBoostedSkillState
    {
        private int returnCounter = 0;
        private int throwCounter = 0;
        private int frameCounter = 0;
        private float stabcount;
        //private HurtBox victim;

        private ChildLocator childLocator;
        //private HuntressTracker tracker;

        public override void OnEnter()
        {
            base.OnEnter();

            stabcount = 10 * attackSpeedStat;
            //tracker = base.characterBody.GetComponent<HuntressTracker>();

            childLocator = base.GetModelChildLocator();

            CoomerangProjectile.Returned += setReturned;

            base.StartAimMode(2f, false);

            base.PlayAnimation("Gesture, Override", "AltPrimary");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            //victim = tracker.GetTrackingTarget();

            if (returnCounter > stabcount)// || !victim)
            {
                this.outer.SetNextStateToMain();
                return;
            }

            if (frameCounter % 5 == 0 && throwCounter <= stabcount)
            {
                Throw();
            }
        }


        private void Throw()
        {
            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();
                Vector3 right = new Vector3(aimRay.direction.z, 0, -1 * aimRay.direction.x).normalized;

                Vector3 shoulderPos = childLocator.FindChild("RightShoulder").position;
                //Vector3 difference = victim.transform.position - shoulderPos;

                FireProjectileInfo info = new FireProjectileInfo()
                {
                    projectilePrefab = chefPlugin.knifePrefab,
                    position = shoulderPos,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction), //Util.QuaternionSafeLookRotation(difference),
                    owner = base.gameObject,
                    damage = base.characterBody.damage * 0.90f,
                    force = (1.5f + base.attackSpeedStat) * 3f,
                    crit = base.RollCrit(),
                    damageColorIndex = DamageColorIndex.Default,
                    //target = victim.gameObject,
                    speedOverride = 160f,
                    fuseOverride = -1f
                };

                childLocator.FindChild("RightShoulder").gameObject.SetActive(false);

                ProjectileManager.instance.FireProjectile(info);

                Util.PlaySound("CleaverThrow", base.gameObject);
            }
        }

        public override void OnExit()
        {
            childLocator.FindChild("RightShoulder").gameObject.SetActive(true);
            CoomerangProjectile.Returned -= setReturned;
            base.PlayAnimation("Gesture, Override", "AltPrimaryEnd");

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        private void setReturned()
        {
            returnCounter++;
        }
    }
}
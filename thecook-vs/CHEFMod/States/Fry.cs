﻿using System;
using System.Collections.Generic;
using ChefMod;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Chef
{
    class Fry : BaseSkillState
    {
        public float baseDuration = 0.3f;
        public float throwTime = 0.38f;

        private float duration;
        private bool hasThrown;
        private Vector3 direction;

        List<CharacterBody> victimList = new List<CharacterBody>();
        Ray aimRay;
        public override void OnEnter() {
            base.OnEnter();
            aimRay = base.GetAimRay();
            this.duration = this.baseDuration;
            //temp until we get pan animation
            base.PlayCrossfade("Gesture, Override", "Primary", "PrimaryCleaver.playbackRate", duration, 0.05f);

            base.StartAimMode(2f, false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge > duration * throwTime && !hasThrown) {
                hasThrown = true;
                Throw();
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        private void Throw() {
            if (base.isAuthority) {
                base.StartAimMode(0.2f, false);

                BlastAttack blastAttack = new BlastAttack();
                blastAttack.radius = 5f;
                blastAttack.procCoefficient = 1f;
                blastAttack.position = aimRay.origin + aimRay.direction * 2.5f;
                blastAttack.attacker = base.gameObject;
                blastAttack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
                blastAttack.baseDamage = 0.1f;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.baseForce = 3f;
                blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                blastAttack.damageType = DamageType.Stun1s;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHit;
                //blastAttack.Fire();

                Vector3 horizontal = new Vector3(aimRay.direction.x, 0, aimRay.direction.z);
                horizontal = horizontal.normalized;
                direction = new Vector3(horizontal.x, 0.5f, horizontal.z);

                getVictim(blastAttack);

                foreach (CharacterBody victim in victimList)
                {
                    DamageInfo damInfo = new DamageInfo 
                    {
                        attacker = base.gameObject,
                        crit = base.RollCrit(),
                        damage = 5f * base.damageStat,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageType.IgniteOnHit,
                        force = Vector3.forward,
                        inflictor = base.gameObject,
                        procCoefficient = 1f
                    };

                    if (victim.characterMotor)
                    {
                        launch(victim);
                        var fl = victim.gameObject.AddComponent<FryLanding>();
                        fl.damageInfo = damInfo;
                        Util.PlaySound("PanHit", base.gameObject);
                    }
                }
                //else
                //{
                //    FireProjectileInfo info = new FireProjectileInfo()
                //    {
                //        projectilePrefab = ChefMod.chefPlugin.drippingPrefab,
                //        position = aimRay.origin + 1.5f * aimRay.direction,
                //        rotation = Util.QuaternionSafeLookRotation(direction),
                //        owner = base.gameObject,
                //        damage = 2 * base.damageStat,
                //        force = 5f,
                //        crit = base.RollCrit(),
                //        damageColorIndex = DamageColorIndex.Default,
                //        target = null,
                //        fuseOverride = -1f
                //    };
                //    ProjectileManager.instance.FireProjectile(info);
                //}
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority() 
        {
            if (hasThrown)
                return InterruptPriority.Any;

            return InterruptPriority.PrioritySkill;
        }

        private void getVictim(BlastAttack ba)
        {
            Collider[] array = Physics.OverlapSphere(ba.position, ba.radius, LayerIndex.defaultLayer.mask);
            int num = 0;
            int num2 = 0;
            while (num < array.Length && num2 < 12)
            {
                HealthComponent component = array[num].GetComponent<HealthComponent>();
                if (component)
                {
                    TeamComponent component2 = component.GetComponent<TeamComponent>();
                    if (component2.teamIndex != TeamComponent.GetObjectTeam(base.gameObject))
                    {
                        this.Compare(component.body);
                        num2++;
                    }
                }
                num++;
            }
        }

        private void Compare(CharacterBody candidate)
        {
            if (!victimList.Contains(candidate))
            {
                victimList.Add(candidate);
            }
        }

        private void launch(CharacterBody charB)
        {
            float speed = 35f;
            if (charB.characterMotor.mass > 300f) speed = 1f;
            charB.characterMotor.rootMotion.y += 1f;
            charB.characterMotor.velocity += speed * direction.normalized;
        }
    }
}
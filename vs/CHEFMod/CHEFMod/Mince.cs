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
    class Mince : BaseSkillState
    {
        public float baseDuration = 0.5f;
        private float duration;
        private List<CharacterBody> victimBodyList = new List<CharacterBody>();
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();

                var coom = chefPlugin.cleaverPrefab.GetComponent<CoomerangProjectile>();
                coom.fieldComponent = characterBody.GetComponent<FieldComponent>();
                coom.followRet = false;

                getHitList(characterBody.corePosition, 40f);

                foreach (CharacterBody victim in victimBodyList)
                {
                    Vector3 direction = victim.corePosition - characterBody.corePosition;
                    direction = direction.normalized;

                    FireProjectileInfo info = new FireProjectileInfo()
                    {
                        projectilePrefab = ChefMod.chefPlugin.cleaverPrefab,
                        position = characterBody.corePosition + 1.5f * direction,
                        rotation = Util.QuaternionSafeLookRotation(direction) * Quaternion.FromToRotation(Vector3.left, Vector3.up),
                        owner = base.gameObject,
                        damage = base.characterBody.damage,
                        force = 50f,
                        crit = base.RollCrit(),
                        damageColorIndex = DamageColorIndex.Default,
                        target = null,
                        speedOverride = 16f,
                        fuseOverride = -1f
                    };

                    ProjectileManager.instance.FireProjectile(info);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            skillLocator.primary.SetBaseSkill(chefPlugin.primaryDef);
            if (skillLocator.secondary.baseSkill == chefPlugin.boostedSecondaryDef)
            {
                skillLocator.secondary.SetBaseSkill(chefPlugin.secondaryDef);
            }
            if (skillLocator.secondary.baseSkill == chefPlugin.boostedAltSecondaryDef)
            {
                skillLocator.secondary.SetBaseSkill(chefPlugin.altSecondaryDef);
            }
            skillLocator.utility.SetBaseSkill(chefPlugin.utilityDef);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        private void getHitList(Vector3 position, float radius)
        {
            Collider[] array = Physics.OverlapSphere(position, radius, LayerIndex.defaultLayer.mask);
            int num = 0;
            int num2 = 0;
            while (num < array.Length && num2 < 12)
            {
                HealthComponent component = array[num].GetComponent<HealthComponent>();
                if (component)
                {
                    TeamComponent component2 = component.GetComponent<TeamComponent>();
                    if (component2.teamIndex != characterBody.teamComponent.teamIndex)
                    {
                        this.AddToList(component.body);
                        num2++;
                    }
                }
                num++;
            }
        }

        private void AddToList(CharacterBody component)
        {
            if (!this.victimBodyList.Contains(component))
            {
                this.victimBodyList.Add(component);
            }
        }
    }
}
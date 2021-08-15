﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using RoR2.Projectile;
using R2API;
using EntityStates.Chef;
using ChefMod.Components;
using ThreeEyedGames;

namespace ChefMod
{
    public static class BuildProjectiles
    {
        public static void BuildCleaver()
        {
            GameObject cleaverGhost = Assets.chefAssetBundle.LoadAsset<GameObject>("CleaverParent").InstantiateClone("CleaverGhost", false);
            cleaverGhost.AddComponent<ProjectileGhostController>();
            GameObject cleaverPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/Sawmerang").InstantiateClone("CHEFCleaver", true);
            GameObject cleaverImpactEffect = Resources.Load<GameObject>("Prefabs/Effects/OmniEffect/omniimpactvfx").InstantiateClone("ChefCleaverImpactEffect", false);
            cleaverImpactEffect.GetComponent<EffectComponent>().soundName = "Play_ChefMod_Cleaver_Hit";
            ChefContent.effectDefs.Add(new EffectDef(cleaverImpactEffect));

            BoomerangProjectile boo = cleaverPrefab.GetComponent<BoomerangProjectile>();
            CoomerangProjectile cum = cleaverPrefab.AddComponent<CoomerangProjectile>();
            cum.impactSpark = cleaverImpactEffect;
            cum.transitionDuration = boo.transitionDuration;
            cum.travelSpeed = boo.travelSpeed;
            cum.charge = boo.charge;
            cum.canHitCharacters = boo.canHitCharacters;
            cum.canHitWorld = boo.canHitWorld;
            cum.distanceMultiplier = boo.distanceMultiplier;
            UnityEngine.Object.Destroy(boo);

            HitBox hit = cleaverPrefab.GetComponentInChildren<HitBox>();
            hit.transform.localScale = new Vector3(hit.transform.localScale.x, 0.69f, hit.transform.localScale.z);

            cleaverPrefab.transform.localScale = 7f * Vector3.one;
            cleaverGhost.transform.localScale = cleaverPrefab.transform.localScale;

            ProjectileController projcont = cleaverPrefab.GetComponent<ProjectileController>();
            projcont.procCoefficient = 1f;
            projcont.allowPrediction = false;

            projcont.ghostPrefab = cleaverGhost;
            ProjectileOverlapAttack poa = cleaverPrefab.GetComponent<ProjectileOverlapAttack>();
            poa.impactEffect = cleaverImpactEffect;
            poa.resetInterval = 60f;
            poa.damageCoefficient = 1f;

            UnityEngine.Object.Destroy(cleaverPrefab.GetComponent<ProjectileDotZone>());

            cleaverPrefab.layer = LayerIndex.noCollision.intVal;
            ChefContent.projectilePrefabs.Add(cleaverPrefab);
            Cleaver.projectilePrefab = cleaverPrefab;
        }
   
        public static void BuildKnife()
        {
            GameObject knifeGhost = Assets.chefAssetBundle.LoadAsset<GameObject>("KnifeParent").InstantiateClone("KnifeGhost", false);
            knifeGhost.AddComponent<ProjectileGhostController>();

            GameObject knifePrefab = Cleaver.projectilePrefab.InstantiateClone("CHEFKnife", true);
            //knifePrefab.AddComponent<ProjectileTargetComponent>();
            var kum = knifePrefab.GetComponent<CoomerangProjectile>();
            kum.isKnife = true;
            kum.distanceMultiplier *= 0.2f;
            knifePrefab.layer = LayerIndex.projectile.intVal;

            UnityEngine.Object.Destroy(knifePrefab.GetComponent<ProjectileOverlapAttack>());

            var pojcont = knifePrefab.GetComponent<ProjectileController>();
            pojcont.ghostPrefab = knifeGhost;
            pojcont.allowPrediction = false;

            //This handles the visuals for the arm.
            knifePrefab.AddComponent<KnifeProjectileController>();
            var lr = knifePrefab.AddComponent<LineRenderer>();
            lr.textureMode = LineTextureMode.Tile;
            lr.numCornerVertices = 4;
            lr.enabled = false;
            lr.startWidth *= 0.35f;
            lr.endWidth *= 0.35f;
            lr.alignment = LineAlignment.View;
            lr.SetMaterials(new Material[1] { Assets.armmat }, 1);

            knifePrefab.transform.localScale = 7f * Vector3.one;
            knifeGhost.transform.localScale = knifePrefab.transform.localScale;

            Slice.projectilePrefab = knifePrefab;
            ChefContent.projectilePrefabs.Add(knifePrefab);

            GameObject juliennePrefab = knifePrefab.InstantiateClone("ChefKnifeBoosted", true);
            KnifeProjectileController kpc = juliennePrefab.GetComponent<KnifeProjectileController>();
            kpc.notifyKnifeHandler = false;
            UnityEngine.Object.Destroy(juliennePrefab.GetComponent<ProjectileOverlapAttack>());
            Julienne.projectilePrefab = juliennePrefab;
            ChefContent.projectilePrefabs.Add(juliennePrefab);
        }

        public static void BuildOil()
        {
            GameObject acid = Resources.Load<GameObject>("Prefabs/CharacterBodies/commandobody").GetComponent<CharacterBody>().preferredPodPrefab;
            GameObject oilfab = acid.GetComponentInChildren<ThreeEyedGames.Decal>().gameObject.InstantiateClone("OilCum", false);
            var dekal = oilfab.GetComponent<ThreeEyedGames.Decal>();

            oilfab.transform.localScale *= 4f;

            var firetrail = Resources.Load<GameObject>("Prefabs/FireTrail");
            Material firepart = firetrail.GetComponent<DamageTrail>().segmentPrefab.GetComponent<ParticleSystemRenderer>().material;

            var chumStain = Resources.Load<GameObject>("prefabs/projectiles/LunarExploderProjectileDotZone");
            GameObject firefab = chumStain.GetComponentInChildren<AlignToNormal>().gameObject.InstantiateClone("ChefFire", false);

            DestroyOnTimer ffDT = firefab.AddComponent<DestroyOnTimer>();
            ffDT.duration = Fireee.burnTime + 3f;

            UnityEngine.Object.Destroy(firefab.GetComponentInChildren<TeamAreaIndicator>().gameObject);
            var decal = firefab.GetComponentInChildren<Decal>();
            Material fireMat = new Material(decal.Material);
            fireMat.SetColor("_Color", Color.red);
            decal.Material = fireMat;

            var systems = firefab.GetComponentsInChildren<ParticleSystemRenderer>();
            UnityEngine.Object.Destroy(systems[0].gameObject);
            Material bluefire = new Material(systems[1].material);
            systems[1].material = firepart;
            systems[2].material = firepart;

            firefab.GetComponentInChildren<Light>().color = Color.red;

            GameObject oilPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/BeetleCrystalBody").InstantiateClone("OilSlick", true);
            oilPrefab.transform.localScale *= 3f;

            var hc = oilPrefab.GetComponent<HealthComponent>();
            hc.dontShowHealthbar = true;

            oilPrefab.GetComponent<CharacterBody>().baseNameToken = "OilBeetle";
            oilPrefab.GetComponent<TeamComponent>().teamIndex = TeamIndex.Neutral;
            oilPrefab.layer = LayerIndex.debris.intVal;
            oilPrefab.name = "OilBeetle";

            foreach (Component comp in oilPrefab.GetComponents<Component>()) if (comp.GetType().Name == "KinematicCharacterMotor")
                {
                    UnityEngine.Object.Destroy(comp);
                }
            UnityEngine.Object.Destroy(oilPrefab.GetComponent<CharacterMotor>());
            var rig = oilPrefab.GetComponent<Rigidbody>();
            rig.isKinematic = false;
            rig.useGravity = true;
            rig.freezeRotation = true;

            ModelLocator modelLocator = oilPrefab.GetComponent<ModelLocator>();
            modelLocator.modelBaseTransform.localScale *= 0.1f;

            var cap = oilPrefab.GetComponent<CapsuleCollider>();
            cap.radius *= 0.6f;
            cap.height = 0;
            cap.material.staticFriction = 1;

            UnityEngine.Object.Destroy(oilPrefab.GetComponent<InteractionDriver>());
            UnityEngine.Object.Destroy(oilPrefab.GetComponent<InputBankTest>());
            UnityEngine.Object.Destroy(oilPrefab.GetComponent<CameraTargetParams>());
            UnityEngine.Object.Destroy(oilPrefab.GetComponent<EntityStateMachine>());
            foreach (GenericSkill skill in oilPrefab.GetComponents<GenericSkill>()) UnityEngine.Object.Destroy(skill);
            UnityEngine.Object.Destroy(oilPrefab.GetComponent<NetworkStateMachine>());
            UnityEngine.Object.Destroy(oilPrefab.GetComponent<Interactor>());
            UnityEngine.Object.Destroy(oilPrefab.GetComponent<EquipmentSlot>());
            UnityEngine.Object.Destroy(oilPrefab.GetComponent<CharacterDeathBehavior>());
            UnityEngine.Object.Destroy(oilPrefab.GetComponent<DeathRewards>());
            UnityEngine.Object.Destroy(oilPrefab.GetComponent<CharacterEmoteDefinitions>());
            UnityEngine.Object.Destroy(oilPrefab.GetComponent<SfxLocator>());

            oilPrefab.AddComponent<Fireee>();
            oilPrefab.AddComponent<ProjectileController>();
            oilPrefab.AddComponent<TeamFilter>();
            oilPrefab.AddComponent<ProjectileDamage>();
            DestroyOnTimer oilDT = oilPrefab.AddComponent<DestroyOnTimer>();
            oilDT.duration = Fireee.oilTime + Fireee.burnTime + 3f;

            On.RoR2.GlobalEventManager.OnTeamLevelUp += (orig, team) =>
            {
                if (team != TeamIndex.Neutral)
                {
                    orig(team);
                    return;
                }
                var hack = oilPrefab.GetComponent<TeamComponent>();
                hack.teamIndex = TeamIndex.Player;
                orig(team);
                hack.teamIndex = TeamIndex.Neutral;
            };

            /*On.RoR2.GlobalEventManager.OnHitEnemy += (orig, self, damageInfo, victim) =>
            {
                if (victim.name == "OilBeetle(Clone)")
                {
                    bool flag5 = (damageInfo.damageType & DamageType.IgniteOnHit) > DamageType.Generic;
                    bool flag6 = (damageInfo.damageType & DamageType.PercentIgniteOnHit) != DamageType.Generic;
                    bool flag7 = false;
                    if (damageInfo.attacker) flag7 = damageInfo.attacker.GetComponent<CharacterBody>().HasBuff(RoR2Content.Buffs.AffixRed);
                    if (flag5 || flag6 || flag7)
                    {
                        DotController.InflictDot(victim, damageInfo.attacker, flag6 ? DotController.DotIndex.PercentBurn : DotController.DotIndex.Burn, 4f * damageInfo.procCoefficient, 1f);
                    }
                    damageInfo.procCoefficient = 0f;
                }

                orig(self, damageInfo, victim);
            };*/

            Fireee.oilFirePrefab = firefab;
            Fireee.oilGroundPrefab = oilfab;
            OilSlick.projectilePrefab = oilPrefab;
            ChefContent.projectilePrefabs.Add(oilPrefab);
        }
    }
}

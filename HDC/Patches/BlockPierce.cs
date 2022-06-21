using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using HDC.Extentions;

namespace HDC.Patches
{
    [Serializable]
    [HarmonyPatch(typeof(Block),"blocked")]
    class BlockPierce
    {
        private static void Prefix(Block __instance, GameObject projectile, Vector3 forward, Vector3 hitPos)
        {
            bool destroy = false;

            ProjectileHit proj = projectile.GetComponent<ProjectileHit>();
            HealthHandler healthHandler = (HealthHandler)Traverse.Create(__instance).Field("health").GetValue();

            float piercePerc = proj.ownPlayer.data.stats.GetAdditionalData().piercePercent;

            if (piercePerc > 0f)
            {
                Vector2 damage = (proj.bulletCanDealDeamage ? proj.damage : 1f) * piercePerc * forward.normalized;
                healthHandler.TakeDamage(damage, hitPos, proj.projectileColor, proj.ownWeapon, proj.ownPlayer, true);

                destroy = true;
            }

            if (destroy)
            {
                UnityEngine.GameObject.Destroy(projectile);
            }
        }
        
    }
}

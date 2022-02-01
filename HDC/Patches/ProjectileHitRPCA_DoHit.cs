using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using HarmonyLib;
using UnboundLib.Cards;
using UnityEngine;
using Photon.Pun;
using HDC.Extentions;
using HDC.Cards;


namespace HDC.Patches
{
    [HarmonyPatch(typeof(ProjectileHit), "RPCA_DoHit")]
    [HarmonyPriority(Priority.First)]
    class ProjectileHitRPCA_DoHit
    {
        private static void Prefix(ProjectileHit __instance, Vector2 hitPoint, Vector2 hitNormal, Vector2 vel, int viewID, int colliderID, ref bool wasBlocked)
        {
            HitInfo hitInfo = new HitInfo();
            hitInfo.point = hitPoint;
            hitInfo.normal = hitNormal;
            hitInfo.collider = null;            
            if (viewID != -1)
            {
                PhotonView photonView = PhotonNetwork.GetPhotonView(viewID);
                hitInfo.collider = photonView.GetComponentInChildren<Collider2D>();
                hitInfo.transform = photonView.transform;
            }
            else if (colliderID != -1)
            {
                hitInfo.collider = MapManager.instance.currentMap.Map.GetComponentsInChildren<Collider2D>()[colliderID];
                hitInfo.transform = hitInfo.collider.transform;
            }

            HealthHandler healthHandler = null;
            if (hitInfo.transform)
            {
                healthHandler = hitInfo.transform.GetComponent<HealthHandler>();
            }
            if (healthHandler && healthHandler.GetComponent<CharacterData>() && healthHandler.GetComponent<Block>())
            {
                Block block = healthHandler.GetComponent<Block>();
                //Stego Stuff
                int plates = healthHandler.GetComponent<CharacterData>().stats.GetAdditionalData().stegoPlates;
                if ((plates > 0))
                {
                    float damage_reduction = Mathf.Pow(Stegosaurus.plate_reduction, plates); //Reduces damage by half to the power of the number of plates 1 plate = 50% 2 plates = 75% etc
                    __instance.damage *= damage_reduction;                                    
                }
            }

        }
    }
}

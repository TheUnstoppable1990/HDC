using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ModdingUtils.MonoBehaviours;
using HDC.Extentions;
using UnboundLib;
using Photon.Pun;
using HarmonyLib;
using ModdingUtils.GameModes;


namespace HDC.MonoBehaviours
{
    class Plesiosaur_Effect : ReversibleEffect, IPointStartHookHandler
    {
        //private Player player;
        private OutOfBoundsHandler ooB;
        //private CharacterData data;
        //public float outOfBoundsTime = 0f;
        public int floorBounces = 0;
        private int bouncesLeft = 0;
        //private float timePass = 0f;
        private bool bounceActive = false;

        public override void OnStart()
        {
            InterfaceGameModeHooksManager.instance.RegisterHooks(this);
            //player = GetComponent<Player>();
            //data = GetComponent<CharacterData>();
            ooB = data.GetAdditionalData().outOfBoundsHandler;
            bouncesLeft = floorBounces;
            base.SetLivesToEffect(int.MaxValue);
        }
        public void OnPointStart()
        {
            HDC.Log("New Point starting? Reset should occur here");
            bouncesLeft = floorBounces;
            HDC.Log("Bounces Left: " + bouncesLeft);
        }
        public override void OnUpdate()
        {
            
            if ((bool)Traverse.Create(ooB).Field("outOfBounds").GetValue() && PlayerStatus.PlayerAlive(player) && (transform.position.y <= -19) && bouncesLeft > 0)
            {
                data.block.sinceBlock = 0.299f;
                if (!bounceActive)
                {
                    bounceActive = true;
                    Unbound.Instance.ExecuteAfterSeconds(0.5f, () =>
                    {
                        bouncesLeft--;
                        HDC.Log("OUT OF BOUNDS");
                        HDC.Log("Floor Bounces Left: " + bouncesLeft);
                        bounceActive = false;
                    });
                }
                
            }
            
            if((bool)Traverse.Create(ooB).Field("almostOutOfBounds").GetValue() && PlayerStatus.PlayerAlive(player) && (transform.position.y <= -19) && bouncesLeft > 0)
            {
                data.block.sinceBlock = 0.299f;
                /*
                Unbound.Instance.ExecuteAfterSeconds(0.5f, () =>
                {
                    bouncesLeft--;
                    HDC.Log("ALMOST OUT OF BOUNDS");
                    HDC.Log("Floor Bounces Left: " + bouncesLeft);
                });*/
                //not gonna trigger the count change on almost out of bounds for reasons
            }


        }

               

    }
}

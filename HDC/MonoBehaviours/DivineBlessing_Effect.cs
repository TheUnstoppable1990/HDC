using UnboundLib.Cards;
using UnityEngine;
using UnboundLib.Networking;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Sonigon;
using Sonigon.Internal;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;

namespace HDC.MonoBehaviours
{
    //Depreciated
    /*
    class DivineBlessing_Effect : MonoBehaviour
    {
        public Block block;
        public Player player;
        public CharacterData data;
        private Action<BlockTrigger.BlockTriggerType> divineBlessingAction;

        private int healAmount = 10;
        public float healRatio = 0.15f;
        private void Start()
        {
            if (block)
            {
                divineBlessingAction = new Action<BlockTrigger.BlockTriggerType>(this.GetDoBlockAction(player, block, data));
                block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(block.BlockAction, divineBlessingAction);
                
            }
        }
        
        public Action<BlockTrigger.BlockTriggerType> GetDoBlockAction(Player player, Block block, CharacterData data)
        {
            return delegate (BlockTrigger.BlockTriggerType trigger)
            {
                //healAmount = data.maxHealth * healRatio;
                data.healthHandler.Heal(healAmount);
                
            };
        }
        private void OnDestroy()
        {
            block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(block.BlockAction, divineBlessingAction);
            
        }
        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
            block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(block.BlockAction, divineBlessingAction);
        }
    }*/
}

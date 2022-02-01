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
    class BehindYou_Effect : MonoBehaviour 
    {
        public Block block;
        public Player player;
        public CharacterData data;
        private Action<BlockTrigger.BlockTriggerType> behindYouAction;
        private float offset = 3f;
        private void Start()
        {
            if (block)
            {
                behindYouAction = new Action<BlockTrigger.BlockTriggerType>(this.GetDoBlockAction(player, block, data));
                block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(block.BlockAction, behindYouAction);
                //block.BlockAction += behindYouAction;
            }
        }
        public Action<BlockTrigger.BlockTriggerType> GetDoBlockAction(Player player, Block block, CharacterData data)
        {
            return delegate (BlockTrigger.BlockTriggerType trigger)
            {
                List<Player> enemies = PlayerManager.instance.players.Where(p => (p.teamID != player.teamID) && !p.data.dead).ToList();
                Vector3 playerPosition = player.transform.position;
                if (enemies.Count < 1)
                {
                    UnityEngine.Debug.Log("No Enemies");
                    return;
                }
                System.Random rand = new System.Random();
                int randomNumber = rand.Next(0, enemies.Count);
                Player randomEnemy = enemies[randomNumber];
                Vector3 randomEnemyPosition = randomEnemy.transform.position;


                
                playerPosition.y = randomEnemyPosition.y;

                if (randomEnemyPosition.x < playerPosition.x)
                {
                    playerPosition.x = randomEnemyPosition.x - this.offset;
                }
                else
                {
                    playerPosition.x = randomEnemyPosition.x + this.offset;
                }
                player.GetComponentInParent<PlayerCollision>().IgnoreWallForFrames(2);
                player.transform.position = playerPosition;


            };
        }
        private void OnDestroy()
        {
            block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(block.BlockAction, behindYouAction);
            //block.BlockAction -= behindYouAction;
        }
        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
            block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(block.BlockAction, behindYouAction);
        }

    }
}

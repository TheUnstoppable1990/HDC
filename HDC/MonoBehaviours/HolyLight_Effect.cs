using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using UnboundLib.Networking;
using System.Collections;
using System.ComponentModel;
using Sonigon;
using Sonigon.Internal;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using Photon.Pun;
using Photon.Realtime;
using ModdingUtils.MonoBehaviours;

namespace HDC.MonoBehaviours
{
    class HolyLight_Effect : MonoBehaviour
    {
        public Player player;
        public CharacterData data;
        public float rangeOfEffect = 10f;
        public float damageRatio = 0.1f;
        private float previous_health = 0.0f;
        private float damage_charge = 0f;
        public Block block;
        private Action<BlockTrigger.BlockTriggerType> holyLightAction;
        private HolyGlow holyGlow = null;

        private void Start()
        {
            
            if (block)
            {
                holyLightAction = new Action<BlockTrigger.BlockTriggerType>(this.GetDoBlockAction());
                block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(block.BlockAction, holyLightAction);
            }
        }
        private void Update()
        {
            if (this.data.health > this.previous_health && this.previous_health > 0) // checks if healing occured since last tic and not dead
            {
                Charge(this.damageRatio * (this.data.health - this.previous_health));
            }
            this.previous_health = this.data.health;
        }

        private void Charge(float amount)
        {
            this.damage_charge += amount;
        }

        public void Discharge()
        {   
            StartCoroutine(GlowEffect());
            if (RangeCheck()) {
                foreach (Player enemy in GetEnemyPlayers())
                {
                    CharacterData enemyData = enemy.GetComponent<CharacterData>();
                    Vector2 enemyPos = enemyData.playerVel.position;
                    float dist = Vector2.Distance(this.data.playerVel.position, enemyPos);
                    if (dist <= rangeOfEffect)// checks range
                    {
                        Vector2 damage = new Vector2(0, -1 * this.damage_charge);
                        enemyData.healthHandler.DoDamage(damage, enemyPos, Color.yellow,null, this.player, false, true, true);
                        
                    }
                }
                this.damage_charge = 0f;
            }
        }
        public Action<BlockTrigger.BlockTriggerType> GetDoBlockAction()
        {
            return delegate (BlockTrigger.BlockTriggerType trigger)
            {
                this.Discharge();
            };
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
            block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(block.BlockAction, holyLightAction);
        }
        private void Awake()
        {
            this.player = gameObject.GetComponent<Player>(); //might make the player setting in the card redundant but oh well at this point
            this.data = this.player.GetComponent<CharacterData>();
        }

        public List<Player> GetEnemyPlayers()
        {
            return PlayerManager.instance.players.Where(player => player.teamID != this.player.teamID).ToList();
        }
        private bool RangeCheck()
        {
            foreach (Player e in GetEnemyPlayers())
            {
                CharacterData ed = e.GetComponent<CharacterData>();
                float dist = Vector2.Distance(this.data.playerVel.position, ed.playerVel.position);
                if(dist <= rangeOfEffect)
                {
                    return true;
                }
            }
            return false;
        }
        private void OnDisable()
        {

        }
        private void OnDestroy()
        {
            block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(block.BlockAction, holyLightAction);
            
        }
        IEnumerator GlowEffect()
        {
            holyGlow = this.player.gameObject.AddComponent<HolyGlow>();
            yield return new WaitForSeconds(1f);
            if (holyGlow != null)
            {
                UnityEngine.GameObject.Destroy(holyGlow);
                holyGlow = null;
            }
        }
       
    }
    public class HolyGlow : ReversibleEffect //Thanks Pykess for this Utility
    {
        private readonly Color color = Color.yellow; //light bluish i think?
        private ReversibleColorEffect colorEffect = null;

        public override void OnOnEnable()
        {
            if (this.colorEffect != null)
            {
                this.colorEffect.Destroy();
            }
        }
        public override void OnStart()
        {
            this.colorEffect = base.player.gameObject.AddComponent<ReversibleColorEffect>();
            this.colorEffect.SetColor(this.color);
            this.colorEffect.SetLivesToEffect(1);
        }
        public override void OnOnDisable()
        {
            if (this.colorEffect != null)
            {
                this.colorEffect.Destroy();
            }
        }
        public override void OnOnDestroy()
        {
            if (this.colorEffect != null)
            {
                this.colorEffect.Destroy();
            }
        }

    }
}

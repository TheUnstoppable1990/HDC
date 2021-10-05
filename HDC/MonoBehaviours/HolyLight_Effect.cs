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
                    if (dist <= HLConst.range && PlayerManager.instance.CanSeePlayer(this.player.transform.position, enemy).canSee)// checks range
                    {
                        Vector2 damage = new Vector2(0, -1 * (this.damage_charge + 10f));
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
                if(dist <= HLConst.range && PlayerManager.instance.CanSeePlayer(this.player.transform.position,e).canSee)
                {
                    return true;
                }
            }
            return false;
        }
        private void OnDisable()
        {
            if (holyGlow != null)
            {
                holyGlow.OnOnDestroy();
                UnityEngine.GameObject.Destroy(holyGlow);
                holyGlow = null;
            }
        }
        private void OnDestroy()
        {
            block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(block.BlockAction, holyLightAction);
            
        }
        IEnumerator GlowEffect()
        {
            holyGlow = this.player.gameObject.AddComponent<HolyGlow>();
            holyGlow.range = HLConst.range;
            yield return new WaitForSeconds(0.5f);
            if (holyGlow != null)
            {
                holyGlow.OnOnDestroy();
                UnityEngine.GameObject.Destroy(holyGlow);
                holyGlow = null;
            }
        }
       
    }
    public class HolyGlow : ReversibleEffect //Thanks Pykess for this Utility
    {        
        private ReversibleColorEffect colorEffect = null;
        private HolyRadiance radiance = null;
        public float range = HLConst.range;

        public override void OnOnEnable()
        {
            KillItDead();
        }
        public override void OnStart()
        {
            this.colorEffect = base.player.gameObject.AddComponent<ReversibleColorEffect>();
            this.colorEffect.SetColor(HLConst.color);
            this.radiance = base.player.gameObject.AddComponent<HolyRadiance>();
            this.colorEffect.SetLivesToEffect(1);
        }
        public override void OnOnDisable()
        {
            KillItDead();
        }
        public override void OnOnDestroy()
        {
            KillItDead();
        }
        private void KillItDead()
        {
            if (this.colorEffect != null)
            {
                this.colorEffect.Destroy();
            }
            if (this.radiance != null)
            {
                this.radiance.Destroy();
            }
        }

    }
    public class HolyRadiance : MonoBehaviour
    {
        private static GameObject lineEffect = null;
        private Player player = null;
        public GameObject holyEffect = null;
        public GameObject holyLightObj = null;
        
        
        public void Start()
        {
            player = gameObject.GetComponent<Player>();
            
            holyLightObj = new GameObject();            
            holyLightObj.transform.SetParent(player.transform);
            holyLightObj.transform.position = player.transform.position;
           
            if(lineEffect == null)
            {
                GetLineEffect();
            }
            holyEffect = Instantiate(lineEffect, holyLightObj.transform);
            var effect = holyEffect.GetComponentInChildren<LineEffect>();
            effect.colorOverTime = new Gradient()
            {
                alphaKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1,0)
                },
                colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(HLConst.color,0)
                },
                mode = GradientMode.Fixed
            };
            effect.widthMultiplier = 1f;
            effect.radius = HLConst.range;
            effect.raycastCollision = true;
            effect.useColorOverTime = true;
            
        }
        private void GetLineEffect()
        {
            var card = CardChoice.instance.cards.First(c => c.name.Equals("ChillingPresence"));
            var statMods = card.gameObject.GetComponentInChildren<CharacterStatModifiers>();
            lineEffect = statMods.AddObjectToPlayer.GetComponentInChildren<LineEffect>().gameObject;
        }
        public void Destroy()
        {
            UnityEngine.GameObject.Destroy(this.holyLightObj);
            UnityEngine.GameObject.Destroy(this.holyEffect);
            UnityEngine.GameObject.Destroy(this);
        }
        
    }

    //need to make sure the asset gets removed everytime, probably hard code a removal timeout

    static class HLConst
    {
        public static float range = 10f;//keeping it like this cuz i feel like its the range of chilling presence
        public static Color color = new Color(0f, 0.75f, 1f);
    }
}

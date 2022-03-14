using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnityEngine;
using ModdingUtils.MonoBehaviours;
using HDC.Utilities;
using HDC.Extentions;

namespace HDC.MonoBehaviours
{
	class HolyLight_Effect : MonoBehaviour
	{
		public Player player;
		public CharacterData data;
		public float damageRatio = 0.1f;
		public Block block;
		private Action<BlockTrigger.BlockTriggerType> holyLightAction;
		private HolyGlow holyGlow = null;

		private void Start()
		{			
			if (this.block)
			{
				this.holyLightAction = new Action<BlockTrigger.BlockTriggerType>(this.GetDoBlockAction().Invoke);
				this.block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(this.block.BlockAction, this.holyLightAction);
			}
			HealthHandler healthHandler = this.data.healthHandler;
			healthHandler.reviveAction = (Action)Delegate.Combine(healthHandler.reviveAction, new Action(this.ResetStuff));
		}	
		

		private void ResetStuff()
        {
			this.data.stats.GetAdditionalData().holyLightCharge = 0f;
        }		
			
		public void Discharge()
		{
			base.StartCoroutine(this.GlowEffect());			
			if (this.RangeCheck())
			{
				var holyLightDamage = player.data.stats.GetAdditionalData().holyLightCharge / 2;				
				HDC.Log($"Holy Light Damage: {holyLightDamage}");
				foreach (Player player in this.GetEnemyPlayers())
				{
					
					CharacterData component = player.GetComponent<CharacterData>();
					Vector2 position = component.playerVel.position;
					float num = Vector2.Distance(this.data.playerVel.position, position);					
					if (num <= HLConst.range && PlayerManager.instance.CanSeePlayer(this.player.transform.position, player).canSee)
					{
						//Vector2 damage = new Vector2(0f, -1f * (this.damage_charge + 10f));
						Vector2 damage;
						if(holyLightDamage > 10f)
                        {
							damage = new Vector2(0f, -1f * holyLightDamage);
                        }
                        else
                        {
							damage = new Vector2(0f, -10f); //minimum damage
                        }
						//Vector2 damage = new Vector2(0f, -1f * (this.damage_charge + 10f));
						component.healthHandler.DoDamage(damage, position, Color.yellow, null, this.player, false, true, true);
					}
				}
				//this.damage_charge = 0f;
				player.data.stats.GetAdditionalData().holyLightCharge -= holyLightDamage;
			}
		}

		
		public void ResetHealthCharge()
		{			
			player.data.stats.GetAdditionalData().holyLightCharge = 0f;
			HDC.Log("Attempting to reset damage charge");
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
			this.block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(this.block.BlockAction, this.holyLightAction);
			HealthHandler healthHandler = this.data.healthHandler;
			healthHandler.reviveAction = (Action)Delegate.Remove(healthHandler.reviveAction, new Action(this.ResetStuff));
			player.data.stats.GetAdditionalData().holyLightCharge = 0f;
		}

		private void Awake()
		{
			this.player = base.gameObject.GetComponent<Player>();
			this.data = this.player.GetComponent<CharacterData>();
		}

		public List<Player> GetEnemyPlayers()
		{
			return (from player in PlayerManager.instance.players
					where player.teamID != this.player.teamID
					select player).ToList<Player>();
		}

		private bool RangeCheck()
		{
			foreach (Player player in this.GetEnemyPlayers())
			{
				CharacterData component = player.GetComponent<CharacterData>();
				float num = Vector2.Distance(this.data.playerVel.position, component.playerVel.position);				
				if (num <= HLConst.range && PlayerManager.instance.CanSeePlayer(this.player.transform.position, player).canSee)
				{
					return true;
				}
			}
			return false;
		}

		private void OnDisable()
		{
			if (this.holyGlow != null)
			{
				this.holyGlow.OnOnDestroy();
				UnityEngine.Object.Destroy(this.holyGlow);
				this.holyGlow = null;
			}
		}

		private void OnDestroy()
		{
			this.block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(this.block.BlockAction, this.holyLightAction);
			player.data.stats.GetAdditionalData().holyLightCharge = 0f;
			HealthHandler healthHandler = this.data.healthHandler;
			healthHandler.reviveAction = (Action)Delegate.Remove(healthHandler.reviveAction, new Action(this.ResetStuff));			
		}

		private IEnumerator GlowEffect()
		{
			this.holyGlow = ExtensionMethods.GetOrAddComponent<HolyGlow>(this.player.gameObject, false);
			this.holyGlow.range = HLConst.range;
			yield return new WaitForSeconds(0.5f);			
			if (this.holyGlow != null)
			{
				this.holyGlow.OnOnDestroy();
				UnityEngine.Object.Destroy(this.holyGlow);
				this.holyGlow = null;
			}
			yield break;
		}
	
		
	}
	public class HolyGlow : ReversibleEffect
	{
		public override void OnOnEnable()
		{
			this.KillItDead();
		}

		public override void OnStart()
		{
			this.colorEffect = this.player.gameObject.AddComponent<ReversibleColorEffect>();
			this.colorEffect.SetColor(HLConst.color);
			this.radiance = this.player.gameObject.AddComponent<HolyRadiance>();
			this.colorEffect.SetLivesToEffect(1);
			this.TimeDestruction();
		}

		private IEnumerator TimeDestruction()
		{
			yield return new WaitForSeconds(0.5f);
			this.KillItDead();
			yield break;
		}

		public override void OnOnDisable()
		{
			this.KillItDead();
		}

		public override void OnOnDestroy()
		{
			this.KillItDead();
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

		private ReversibleColorEffect colorEffect = null;

		private HolyRadiance radiance = null;

		public float range = HLConst.range;
	}
	public class HolyRadiance : MonoBehaviour
	{
		public void Start()
		{
			this.player = base.gameObject.GetComponent<Player>();
			this.holyLightObj = new GameObject();
			this.holyLightObj.transform.SetParent(this.player.transform);
			this.holyLightObj.transform.position = this.player.transform.position;
			if (HolyRadiance.lineEffect == null)
			{
				HolyRadiance.lineEffect = AssetTools.GetLineEffect("ChillingPresence");
			}
			this.holyEffect = UnityEngine.Object.Instantiate<GameObject>(HolyRadiance.lineEffect, this.holyLightObj.transform);
			LineEffect componentInChildren = this.holyEffect.GetComponentInChildren<LineEffect>();
			componentInChildren.colorOverTime = new Gradient
			{
				alphaKeys = new GradientAlphaKey[]
				{
					new GradientAlphaKey(1f, 0f)
				},
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(HLConst.color, 0f)
				},
				mode = GradientMode.Fixed
			};
			componentInChildren.widthMultiplier = 1f;
			componentInChildren.radius = HLConst.range;
			componentInChildren.raycastCollision = true;
			componentInChildren.useColorOverTime = true;
		}

		public void Destroy()
		{
			UnityEngine.Object.Destroy(this.holyLightObj);
			UnityEngine.Object.Destroy(this.holyEffect);
			UnityEngine.Object.Destroy(this);
		}

		private static GameObject lineEffect;

		private Player player = null;

		public GameObject holyEffect = null;

		public GameObject holyLightObj = null;
	}
	internal static class HLConst
	{
		public static float range = 10f;
		public static Color color = new Color(0f, 0.75f, 1f);
	}
}

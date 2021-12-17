using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnityEngine;
using ModdingUtils.MonoBehaviours;
using HDC.Utilities;

namespace HDC.MonoBehaviours
{
	// Token: 0x0200000B RID: 11
	class HolyLight_Effect : MonoBehaviour
	{
		
		private void Start()
		{
			bool flag = this.block;
			if (flag)
			{
				this.holyLightAction = new Action<BlockTrigger.BlockTriggerType>(this.GetDoBlockAction().Invoke);
				this.block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(this.block.BlockAction, this.holyLightAction);
			}
		}

		
		private void Update()
		{
			bool flag = this.data.health > this.previous_health && this.previous_health > 0f;
			if (flag)
			{
				this.Charge(this.damageRatio * (this.data.health - this.previous_health));
			}
			this.previous_health = this.data.health;
		}

		
		private void Charge(float amount)
		{
			this.damage_charge += amount;
		}

		
		public void Discharge()
		{
			base.StartCoroutine(this.GlowEffect());
			bool flag = this.RangeCheck();
			if (flag)
			{
				foreach (Player player in this.GetEnemyPlayers())
				{
					CharacterData component = player.GetComponent<CharacterData>();
					Vector2 position = component.playerVel.position;
					float num = Vector2.Distance(this.data.playerVel.position, position);
					bool flag2 = num <= HLConst.range && PlayerManager.instance.CanSeePlayer(this.player.transform.position, player).canSee;
					if (flag2)
					{
						Vector2 damage = new Vector2(0f, -1f * (this.damage_charge + 10f));
						component.healthHandler.DoDamage(damage, position, Color.yellow, null, this.player, false, true, true);
					}
				}
				this.damage_charge = 0f;
			}
		}

		
		public void ResetHealthCharge()
		{
			this.damage_charge = 0f;
			UnityEngine.Debug.Log("Attempting to reset damage charge");
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00003110 File Offset: 0x00001310
		public Action<BlockTrigger.BlockTriggerType> GetDoBlockAction()
		{
			return delegate (BlockTrigger.BlockTriggerType trigger)
			{
				this.Discharge();
			};
		}

		// Token: 0x06000031 RID: 49 RVA: 0x0000312E File Offset: 0x0000132E
		public void Destroy()
		{
			UnityEngine.Object.Destroy(this);
			this.block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(this.block.BlockAction, this.holyLightAction);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x0000315E File Offset: 0x0000135E
		private void Awake()
		{
			this.player = base.gameObject.GetComponent<Player>();
			this.data = this.player.GetComponent<CharacterData>();
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003184 File Offset: 0x00001384
		public List<Player> GetEnemyPlayers()
		{
			return (from player in PlayerManager.instance.players
					where player.teamID != this.player.teamID
					select player).ToList<Player>();
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000031B8 File Offset: 0x000013B8
		private bool RangeCheck()
		{
			foreach (Player player in this.GetEnemyPlayers())
			{
				CharacterData component = player.GetComponent<CharacterData>();
				float num = Vector2.Distance(this.data.playerVel.position, component.playerVel.position);
				bool flag = num <= HLConst.range && PlayerManager.instance.CanSeePlayer(this.player.transform.position, player).canSee;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003274 File Offset: 0x00001474
		private void OnDisable()
		{
			bool flag = this.holyGlow != null;
			if (flag)
			{
				this.holyGlow.OnOnDestroy();
				UnityEngine.Object.Destroy(this.holyGlow);
				this.holyGlow = null;
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000032B3 File Offset: 0x000014B3
		private void OnDestroy()
		{
			this.block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(this.block.BlockAction, this.holyLightAction);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000032DC File Offset: 0x000014DC
		private IEnumerator GlowEffect()
		{
			this.holyGlow = ExtensionMethods.GetOrAddComponent<HolyGlow>(this.player.gameObject, false);
			this.holyGlow.range = HLConst.range;
			yield return new WaitForSeconds(0.5f);
			bool flag = this.holyGlow != null;
			if (flag)
			{
				this.holyGlow.OnOnDestroy();
				UnityEngine.Object.Destroy(this.holyGlow);
				this.holyGlow = null;
			}
			yield break;
		}

		// Token: 0x04000031 RID: 49
		public Player player;

		// Token: 0x04000032 RID: 50
		public CharacterData data;

		// Token: 0x04000033 RID: 51
		public float damageRatio = 0.1f;

		// Token: 0x04000034 RID: 52
		private float previous_health = 0f;

		// Token: 0x04000035 RID: 53
		private float damage_charge = 0f;

		// Token: 0x04000036 RID: 54
		public Block block;

		// Token: 0x04000037 RID: 55
		private Action<BlockTrigger.BlockTriggerType> holyLightAction;

		// Token: 0x04000038 RID: 56
		private HolyGlow holyGlow = null;
	}
	public class HolyGlow : ReversibleEffect
	{
		// Token: 0x0600003B RID: 59 RVA: 0x0000333E File Offset: 0x0000153E
		public override void OnOnEnable()
		{
			this.KillItDead();
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003348 File Offset: 0x00001548
		public override void OnStart()
		{
			this.colorEffect = this.player.gameObject.AddComponent<ReversibleColorEffect>();
			this.colorEffect.SetColor(HLConst.color);
			this.radiance = this.player.gameObject.AddComponent<HolyRadiance>();
			this.colorEffect.SetLivesToEffect(1);
			this.TimeDestruction();
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000033A7 File Offset: 0x000015A7
		private IEnumerator TimeDestruction()
		{
			yield return new WaitForSeconds(0.5f);
			this.KillItDead();
			yield break;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x000033B6 File Offset: 0x000015B6
		public override void OnOnDisable()
		{
			this.KillItDead();
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000033C0 File Offset: 0x000015C0
		public override void OnOnDestroy()
		{
			this.KillItDead();
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000033CC File Offset: 0x000015CC
		private void KillItDead()
		{
			bool flag = this.colorEffect != null;
			if (flag)
			{
				this.colorEffect.Destroy();
			}
			bool flag2 = this.radiance != null;
			if (flag2)
			{
				this.radiance.Destroy();
			}
		}

		// Token: 0x04000039 RID: 57
		private ReversibleColorEffect colorEffect = null;

		// Token: 0x0400003A RID: 58
		private HolyRadiance radiance = null;

		// Token: 0x0400003B RID: 59
		public float range = HLConst.range;
	}
	public class HolyRadiance : MonoBehaviour
	{
		// Token: 0x06000042 RID: 66 RVA: 0x00003438 File Offset: 0x00001638
		public void Start()
		{
			this.player = base.gameObject.GetComponent<Player>();
			this.holyLightObj = new GameObject();
			this.holyLightObj.transform.SetParent(this.player.transform);
			this.holyLightObj.transform.position = this.player.transform.position;
			bool flag = HolyRadiance.lineEffect == null;
			if (flag)
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

		// Token: 0x06000043 RID: 67 RVA: 0x00003565 File Offset: 0x00001765
		public void Destroy()
		{
			UnityEngine.Object.Destroy(this.holyLightObj);
			UnityEngine.Object.Destroy(this.holyEffect);
			UnityEngine.Object.Destroy(this);
		}

		// Token: 0x0400003C RID: 60
		private static GameObject lineEffect;

		// Token: 0x0400003D RID: 61
		private Player player = null;

		// Token: 0x0400003E RID: 62
		public GameObject holyEffect = null;

		// Token: 0x0400003F RID: 63
		public GameObject holyLightObj = null;
	}
	internal static class HLConst
	{
		// Token: 0x04000040 RID: 64
		public static float range = 10f;

		// Token: 0x04000041 RID: 65
		public static Color color = new Color(0f, 0.75f, 1f);
	}
}

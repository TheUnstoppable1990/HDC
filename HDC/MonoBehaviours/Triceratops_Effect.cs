using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnityEngine;
using ModdingUtils.MonoBehaviours;
using HDC.Utilities;
using HDC.Extentions;
using HDC.Cards;

namespace HDC.MonoBehaviours
{
    class Triceratops_Effect : MonoBehaviour
    {
		public Player player;
		public CharacterData data;
		public float damageRatio = 0.1f;
		public Block block;
		private Action<BlockTrigger.BlockTriggerType> hornAttackAction;

		private Horns_Effect horn_attack;

		private bool hornsActive;

		private float damage = 0f;
		private float duration = 0f;

		public static float range = 3f;

		private float timePass = 0f;

		private void Awake()
		{
			player = base.gameObject.GetComponent<Player>();
			data = player.GetComponent<CharacterData>();
		}
		//continue to utilize holy light code and use the saw effect instead
		private void Start()
		{
			if (block)
			{
				hornAttackAction = new Action<BlockTrigger.BlockTriggerType>(GetDoBlockAction().Invoke);
				block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(block.BlockAction, hornAttackAction);
			}
			//HealthHandler healthHandler = this.data.healthHandler;
			//healthHandler.reviveAction = (Action)Delegate.Combine(healthHandler.reviveAction, new Action(this.ResetStuff));
		}

		public Action<BlockTrigger.BlockTriggerType> GetDoBlockAction()
		{
			return delegate (BlockTrigger.BlockTriggerType trigger)
			{
				HornAttack();
			};
		}
		public void Destroy()
        {
			Destroy(this);
			this.block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(block.BlockAction, hornAttackAction);
        }
		public void HornAttack()
		{
			int trikes = data.stats.GetAdditionalData().trikes;
			duration = trikes * Triceratops.horns_duration;
			damage = (float)trikes / (float)(trikes + 1) * Triceratops.damage_const;
			if (!hornsActive)
			{
				horn_attack = player.gameObject.GetOrAddComponent<Horns_Effect>();
				hornsActive = true;
				HDC.Log($"Performing Attack for {damage} damge at {duration} duration");
				
			}
			Unbound.Instance.ExecuteAfterSeconds(duration, DestroyHorns);
        }
		public List<Player> GetEnemyPlayers()
		{
			return (from player in PlayerManager.instance.players
					where player.teamID != this.player.teamID
					select player).ToList<Player>();
		}

		private void DestroyHorns()
        {
			if (player.gameObject.GetComponent<Horns_Effect>() != null)
			{
				HDC.Log("Time Delayed Destruction");
				//Destroy(player.gameObject.GetComponent<Horns_Effect>());
				player.gameObject.GetComponent<Horns_Effect>().Destroy();
				horn_attack = null;
				hornsActive = false;
			}
        }

		private void Update()
        {
            if (hornsActive)
            {
				timePass += Time.deltaTime;
				if (timePass > 0.5f) //every half second
				{
					DoDamage();
					timePass = 0f;
				}
            }
        }
		private void DoDamage()
        {
			foreach (Player player in GetEnemyPlayers())
			{
				CharacterData component = player.GetComponent<CharacterData>();
				Vector2 position = component.playerVel.position;
				float num = Vector2.Distance(data.playerVel.position, position);
				float rangeOfEffect = AssetTools.GetLineEffectRadius(horn_attack.hornsLineEffect);

				if (num <= rangeOfEffect && PlayerManager.instance.CanSeePlayer(this.player.transform.position, player).canSee)
				{
					Vector2 effectDamage = new Vector2(0f, -1f * damage);
					component.healthHandler.CallTakeDamage(effectDamage, position, null, this.player, true);
				}
			}
		}


	}

	public class Horns_Effect : MonoBehaviour
    {
		public static GameObject lineEffect;

		public static GameObject sawObj;

		private Player player = null;

		public GameObject hornsEffect = null;

		public GameObject hornsObj = null;

		public LineEffect hornsLineEffect;

		public void Start()
        {

			player = base.gameObject.GetComponent<Player>();
			hornsObj = new GameObject();
			hornsObj.transform.SetParent(player.transform);
			hornsObj.transform.position = player.transform.position;


			if(lineEffect == null)
            {
				lineEffect = AssetTools.GetLineEffect("Lifestealer"); //using Chilling presence for testing, will later change to saw
            }
			

			hornsEffect = Instantiate<GameObject>(lineEffect, hornsObj.transform);
			hornsLineEffect = hornsEffect.GetComponentInChildren<LineEffect>();
			hornsLineEffect.colorOverTime = new Gradient
			{
				alphaKeys = new GradientAlphaKey[]
				{
					new GradientAlphaKey(0.5f, 0f)
				},
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(Color.green, 0f)
				},
				mode = GradientMode.Fixed
			};
			hornsLineEffect.widthMultiplier = 1f;
			hornsLineEffect.radius = Triceratops_Effect.range;
			hornsLineEffect.raycastCollision = true; // i think i want it to go through walls
			hornsLineEffect.useColorOverTime = true;
			
        }

		public void Destroy()
        {
			Destroy(hornsObj);
			Destroy(hornsEffect);
			Destroy(this);
        }

    }
}

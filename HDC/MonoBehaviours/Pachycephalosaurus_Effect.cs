using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ModdingUtils.MonoBehaviours;
using UnboundLib.Cards;
using UnboundLib;
using ModdingUtils.Extensions;
using HDC.Cards;
using Sonigon;
using Photon.Pun;

namespace HDC.MonoBehaviours
{
    class Pachycephalosaurus_Effect : MonoBehaviour
    {
	
		public float forceMult = 500f;
		public float knockbackMult = 0f;
		private Vector2 forceDir;
		public float range = 0f;
		private float damageMult = 0.1f;
        private CharacterData data;
		public Player player;
		private bool headbuttActive = false;
		private float enemyXDistance = 0f;
		private float enemyYDistance = 0f;
		private float rangeCoeff = 11f;

		public string GetStats()
        {
			return
				$"Knockback Multiplyer: {knockbackMult}\n" +
				$"Force Multiplyer: {forceMult}\n" +
				$"Damage Multiplyer: {damageMult}\n"+
				$"Range: {range}";
        }
        private void Start()
        {
            this.data = base.GetComponentInParent<CharacterData>();
            PlayerCollision component = this.data.GetComponent<PlayerCollision>();
            component.collideWithPlayerAction = (Action<Vector2, Vector2, Player>)Delegate.Combine(component.collideWithPlayerAction, new Action<Vector2, Vector2, Player>(this.Collide));
            base.GetComponentInParent<ChildRPC>().childRPCsVector2Vector2Int.Add("PachyHeadbutt", new Action<Vector2, Vector2, int>(this.RPCA_Collide));
            Block componentInParent = base.GetComponentInParent<Block>();
            componentInParent.SuperFirstBlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(componentInParent.SuperFirstBlockAction, new Action<BlockTrigger.BlockTriggerType>(this.DoBlock));
        }
        private void OnDestroy()
        {
			HDC.Log("DESTROYING PACHY EFFECT");
            PlayerCollision component = this.data.GetComponent<PlayerCollision>();
            component.collideWithPlayerAction = (Action<Vector2, Vector2, Player>)Delegate.Remove(component.collideWithPlayerAction, new Action<Vector2, Vector2, Player>(this.Collide));
            base.GetComponentInParent<ChildRPC>().childRPCsVector2Vector2Int.Remove("PachyHeadbutt");
            Block componentInParent = base.GetComponentInParent<Block>();
            componentInParent.SuperFirstBlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(componentInParent.SuperFirstBlockAction, new Action<BlockTrigger.BlockTriggerType>(this.DoBlock));
        }

		public void Collide(Vector2 pos, Vector2 colDir, Player player)
		{
			//player is the player being collided with			
			base.GetComponentInParent<ChildRPC>().CallFunction("PachyHeadbutt", pos, colDir, player.playerID);
		}
		public void RPCA_Collide(Vector2 pos, Vector2 colDir, int playerID)
		{
			CharacterData enemyData = GetPlayerWithID(PlayerManager.instance.players, playerID).gameObject.GetComponentInParent<CharacterData>();
		
			if (enemyData && headbuttActive)
			{
				float damage = this.data.maxHealth * this.damageMult;
				float knockBack = this.data.maxHealth * this.knockbackMult;
				enemyData.healthHandler.TakeDamage(this.forceDir * damage, this.data.playerVel.position,null,this.player,true);
				enemyData.healthHandler.TakeForce(this.forceDir * knockBack);
				headbuttActive = false;
			}

		}
		private Player GetPlayerWithID(List<Player> players, int playerID)
		{
			for (int i = 0; i < players.Count; i++)
			{
				if (players[i].playerID == playerID)
				{
					return players[i];
				}
			}
			return null;
		}
		public void Destroy()
        {
			Destroy(this);
        }
	
		public void DoBlock(BlockTrigger.BlockTriggerType trigger)
		{
			
			this.forceDir = GetTarget();			
			this.data.playerVel.SetFieldValue("velocity", this.forceDir.normalized * this.range * rangeCoeff);
			headbuttActive = true;
			base.StartCoroutine(resetHeadbutt());
		}
		public IEnumerator resetHeadbutt()
        {
			yield return new WaitForSeconds(1f);
			this.headbuttActive = false;
			yield break;
        }
		public List<Player> GetEnemyPlayers()
		{
			return (from player in PlayerManager.instance.players
					where player.teamID != this.player.teamID
					select player).ToList<Player>();
		}
		private Vector2 GetTarget()
		{
			float shortest = this.range;
			Player target = null;
			enemyXDistance = range * this.data.aimDirection.normalized.x;
			enemyYDistance = range * this.data.aimDirection.normalized.y;
			Vector2 aimDir = this.data.aimDirection;
			foreach (Player player in this.GetEnemyPlayers())
			{
				CharacterData component = player.GetComponent<CharacterData>();
				float num = Vector2.Distance(this.data.playerVel.position, component.playerVel.position);
				if (num <= shortest && PlayerManager.instance.CanSeePlayer(this.player.transform.position, player).canSee && !player.data.dead)
				{
					shortest = num;
					target = player;					
				}
				//calculates the closest enemy that can be seen thats not dead

			}
			if (target != null)
            {
				enemyXDistance = target.data.playerVel.position.x - this.data.playerVel.position.x;
				enemyYDistance = target.data.playerVel.position.y - this.data.playerVel.position.y;
				aimDir = new Vector2(enemyXDistance, enemyYDistance);
            }
			return aimDir;
		}
	}

	}

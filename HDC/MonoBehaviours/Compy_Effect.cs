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
using ModdingUtils.Extensions;


namespace HDC.MonoBehaviours
{
    class Compy_Effect : MonoBehaviour
    {
		private MoveTransform move;
		private readonly float updateDelay = 0.2f;
		private float startTime;
		public int state;
		public bool detected;
		private FlickerEvent[] flicks;
		private PhotonView view;
		//public RotSpring rot1;
		//public RotSpring rot2;
		public float x;
		public float y;
		//public static SoundEvent fieldsound;
		//private SoundParameterIntensity soundParameterIntensity = new SoundParameterIntensity(0.5f, UpdateMode.Continuous);

		private void Start()
		{
			this.move = base.GetComponentInParent<MoveTransform>();
			this.flicks = base.GetComponentsInChildren<FlickerEvent>();
			this.view = base.GetComponentInParent<PhotonView>();
			base.GetComponentInParent<SyncProjectile>().active = true;
			this.state = 0;
			this.x = this.move.velocity.x;
			this.y = this.move.velocity.y;
			this.ResetTimer();
			this.detected = false;
		}

		public void Destroy()
		{
			UnityEngine.Object.Destroy(this);
		}
		private void ResetTimer()
		{
			this.startTime = Time.time;
		}
		private void Update()
		{
			if (Time.time >= this.startTime + this.updateDelay && base.gameObject.transform.parent != null)
			{
				this.ResetTimer();
				if (this.state == 0)
				{
					this.state = 1;
					this.move.velocity.z = 0f;
					MoveTransform moveTransform = this.move;
					moveTransform.velocity.x = moveTransform.velocity.x * 1f;
					MoveTransform moveTransform2 = this.move;
					moveTransform2.velocity.y = moveTransform2.velocity.y * 1f;
					return;
				}
				if (this.state == 1)
				{
					Player closestPlayer = PlayerManager.instance.GetClosestPlayer(base.transform.position, true);
					Player ownPlayer = base.GetComponentInParent<ProjectileHit>().ownPlayer;
					if (closestPlayer && closestPlayer != ownPlayer)
					{
						this.detected = true;
					}
					if (this.detected)
					{						
						this.move.simulateGravity--;
						MoveTransform moveTransform3 = this.move;
						moveTransform3.velocity.x = moveTransform3.velocity.x * 0.01f;
						MoveTransform moveTransform4 = this.move;
						moveTransform4.velocity.y = moveTransform4.velocity.y * 0.01f;
						this.move.velocity.z = 0f;
						this.state = 2;
						Vector3 a = closestPlayer.transform.position + base.transform.right * this.move.selectedSpread * Vector3.Distance(base.transform.position, closestPlayer.transform.position);
						float d = Vector3.Angle(base.transform.root.forward, a - base.transform.position);
						this.move.velocity -= this.move.velocity * d;
						this.move.velocity -= this.move.velocity;
						this.move.velocity += Vector3.ClampMagnitude(a - base.transform.position, 10f) * TimeHandler.deltaTime * this.move.localForce.magnitude;
						this.move.velocity.z = 0f;
						for (int i = 0; i < this.flicks.Length; i++)
						{
							this.flicks[i].isOn = true;
						}
						return;
					}
				}
				else
				{
					if (this.state == 2)
					{
						MoveTransform moveTransform5 = this.move;
						moveTransform5.velocity.x = moveTransform5.velocity.x * 15f;
						MoveTransform moveTransform6 = this.move;
						moveTransform6.velocity.y = moveTransform6.velocity.y * 15f;
						this.move.velocity.z = 0f;
						this.state = 3;
						return;
					}
					if (this.state == 3)
					{
						this.move.simulateGravity++;
						this.move.velocity.z = 0f;
						this.state = 3;
					}
				}
			}
		}

		
	}
}

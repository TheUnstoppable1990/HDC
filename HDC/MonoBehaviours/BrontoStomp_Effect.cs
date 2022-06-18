using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using HDC.Extentions;

namespace HDC.MonoBehaviours
{
    class BrontoStomp_Effect : MonoBehaviour
    {
        private Player thisPlayer;
        private CharacterStatModifiers theseStats;

        private float timeOfLastStomp= -1f;
        private readonly float range = 1.5f;
        private readonly float angleThreshold = 30f;
        private readonly float minTimeBetweenStomps = 0.05f;
        private readonly float minMassFactor = 2f;

        void Awake()
        {
            thisPlayer = gameObject.GetComponent<Player>();
            theseStats = gameObject.GetComponent<CharacterStatModifiers>();
        }

        void Start()
        {

        }

        void Update()
        {
            if(PlayerStatus.PlayerAliveAndSimulated(thisPlayer) && Time.time >= this.timeOfLastStomp + this.minTimeBetweenStomps)
            {
                List<Player> enemyPlayers = PlayerManager.instance.players.Where(player => PlayerStatus.PlayerAliveAndSimulated(player) && (player.teamID != thisPlayer.teamID)).ToList();

                Vector2 displacement;

                foreach (Player enemyPlayer in enemyPlayers)
                {
                    //displacement vector stuff and mass stuff
                    float mass = (float)Traverse.Create(thisPlayer.data.playerVel).Field("mass").GetValue();
                    float enemyMass = (float)Traverse.Create(enemyPlayer.data.playerVel).Field("mass").GetValue();

                    if( mass >= minMassFactor * enemyMass) //player has to be significantly bigger than enemy
                    {
                        displacement = thisPlayer.transform.position - enemyPlayer.transform.position; 
                        if(displacement.magnitude <= range && Vector2.Angle(Vector2.up,displacement) <= Math.Abs(this.angleThreshold / 2))
                        {
                            //player is within range and above enemy
                            //damage will not be instakill but rather a mass ratio 5x mass will be instakill
                            float damage = mass/enemyMass/5 * enemyPlayer.data.maxHealth;

                            //enemyPlayer.data.healthHandler.TakeDamage(new Vector2(0, -1 * damage), enemyPlayer.transform.position, null, thisPlayer, true, false);
                            enemyPlayer.data.healthHandler.CallTakeDamage(new Vector2(0, -1 * damage), enemyPlayer.transform.position, null, thisPlayer, true);
                            timeOfLastStomp = Time.time;
                            return;
                        }
                    }
                }
            }
        }

        public void Destroy()
        {
            Destroy(this);
        }
    }
}

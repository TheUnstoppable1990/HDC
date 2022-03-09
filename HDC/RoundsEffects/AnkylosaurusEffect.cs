using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModdingUtils.MonoBehaviours;
using ModdingUtils.RoundsEffects;
using UnityEngine;

namespace HDC.RoundsEffects
{
    class AnkylosaurusEffect : WasHitEffect
    {
        public float damage_percent = 0f;
        public override void WasDealtDamage(Vector2 damage, bool selfDamage)
        {
            Player attacking_player = this.gameObject.GetComponent<Player>().data.lastSourceOfDamage;
            if (!selfDamage &&  attacking_player != null)
            {
                Vector2 horns_damage = new Vector2(-damage_percent * damage.x, -damage_percent * damage.y);
                Vector2 enemy_pos = attacking_player.data.playerVel.position;                
                attacking_player.data.healthHandler.DoDamage(horns_damage, enemy_pos, Color.blue);                
            }
        }
    }
}

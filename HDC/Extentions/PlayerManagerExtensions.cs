using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HDC.Extentions
{
    static class PlayerManagerExtensions
    {
        public static void ForEachPlayer(this PlayerManager instance, Action<Player> action)
        {
            foreach (Player player in instance.players)
            {
                action(player);
            }
        }
    }
}

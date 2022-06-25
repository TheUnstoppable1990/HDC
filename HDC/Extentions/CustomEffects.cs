using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HDC.Cards;
using HDC.MonoBehaviours;
using HDC.RoundsEffects;

namespace HDC.Extentions
{
    public static class CustomEffects
    {
        public static void DestroyAllEffects(GameObject gameObject)
        {
            DestroyAllAppliedEffects(gameObject);
        }
        public static void DestroyAllAppliedEffects(GameObject gameObject)
        {            
            Paladin_Effect[] paladin_Effects = gameObject.GetComponents<Paladin_Effect>();
            foreach (Paladin_Effect pe in paladin_Effects)
            {
                if(pe != null)
                {
                    pe.Destroy();
                }
            }
            CelestialCountdown_Effect_2[] cc_Effects = gameObject.GetComponents<CelestialCountdown_Effect_2>();
            foreach (CelestialCountdown_Effect_2 cce in cc_Effects)
            {
                if(cce != null)
                {
                    cce.Destroy();

                }
            }
            Meditation_Effect[] meditation_Effects = gameObject.GetComponents<Meditation_Effect>();
            foreach (Meditation_Effect me in meditation_Effects)
            {
                if(me != null)
                {
                    me.Destroy();
                }
            }
            BehindYou_Effect[] by_Effects = gameObject.GetComponents<BehindYou_Effect>();
            foreach (BehindYou_Effect bye in by_Effects)
            {
                if(bye != null)
                {
                    bye.Destroy();
                }
            }
            /*
            DivineBlessing_Effect[] db_Effects = gameObject.GetComponents<DivineBlessing_Effect>();
            foreach (DivineBlessing_Effect dbe in db_Effects)
            {
                if(dbe != null)
                {
                    dbe.Destroy();
                }
            }
            */
            HolyLight_Effect[] hl_Effects = gameObject.GetComponents<HolyLight_Effect>();
            foreach (HolyLight_Effect hle in hl_Effects)
            {
                if(hle != null)
                {
                    hle.Destroy();
                }
            }
            AnkylosaurusEffect[] tr_Effects = gameObject.GetComponents<AnkylosaurusEffect>();
            foreach (AnkylosaurusEffect tre in tr_Effects)
            {
                if(tre != null)
                {
                    tre.Destroy();
                }
            }
            Pachycephalosaurus_Effect[] pachycephalosaurus_Effects = gameObject.GetComponents<Pachycephalosaurus_Effect>();
            foreach (Pachycephalosaurus_Effect pe in pachycephalosaurus_Effects)
            {
                if(pe != null)
                {
                    pe.Destroy();
                }
            }
            Parasaurolophus_Effect[] parasaurolophus_Effects = gameObject.GetComponents<Parasaurolophus_Effect>();
            foreach (Parasaurolophus_Effect pse in parasaurolophus_Effects)
            {
                if (pse != null)
                {
                    pse.Destroy();
                }
            }
        }
       
    }
}

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

namespace HDC.Utilities
{
    static class AssetTools
    {
        static public GameObject GetLineEffect(string name)
        {
            var card = CardChoice.instance.cards.First(c => c.name.Equals(name));
            var statMods = card.gameObject.GetComponentInChildren<CharacterStatModifiers>();
            return statMods.AddObjectToPlayer.GetComponentInChildren<LineEffect>().gameObject;
        }
        static public ProceduralImage GetProceduralImage(string name)
        {
            var card = CardChoice.instance.cards.First(c => c.name.Equals(name));
            var statMods = card.gameObject.GetComponentInChildren<CharacterStatModifiers>();
            var image = statMods.AddObjectToPlayer.GetComponentInChildren<ProceduralImage>();
            return image;
        }
    }
}

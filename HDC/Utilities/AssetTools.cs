using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;
using UnboundLib.Utils;
using UnboundLib;

namespace HDC.Utilities
{
	// Token: 0x02000004 RID: 4
	internal static class AssetTools
	{
		// Token: 0x0600000D RID: 13 RVA: 0x00002400 File Offset: 0x00000600
		public static GameObject GetLineEffect(string name)
		{
			//CardInfo cardInfo = CardChoice.instance.cards.First((CardInfo c) => c.name.Equals(name));
			CardInfo cardInfo = CardManager.cards.Values.Select(card => card.cardInfo).First(c => c.name.Equals(name));
			CharacterStatModifiers componentInChildren = cardInfo.gameObject.GetComponentInChildren<CharacterStatModifiers>();
			return componentInChildren.AddObjectToPlayer.GetComponentInChildren<LineEffect>().gameObject;
		}

		public static GameObject GetSawEffect(string name)
        {
			CardInfo cardInfo = CardManager.cards.Values.Select(card => card.cardInfo).First(c => c.name.Equals(name));
			CharacterStatModifiers componentInChildren = cardInfo.gameObject.GetComponentInChildren<CharacterStatModifiers>();
			GameObject objAddedToPlayer = componentInChildren.AddObjectToPlayer; //A_NewSaw
			return objAddedToPlayer.GetComponentInChildren<ParticleSystem>().gameObject;
			//return componentInChildren.AddObjectToPlayer.GetComponentInChildren<LineEffect>().gameObject;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002458 File Offset: 0x00000658
		public static ProceduralImage GetProceduralImage(string name)
		{
			//CardInfo cardInfo = CardChoice.instance.cards.First((CardInfo c) => c.name.Equals(name));
			CardInfo cardInfo = CardManager.cards.Values.Select(card => card.cardInfo).First(c => c.name.Equals(name));
			CharacterStatModifiers componentInChildren = cardInfo.gameObject.GetComponentInChildren<CharacterStatModifiers>();
			return componentInChildren.AddObjectToPlayer.GetComponentInChildren<ProceduralImage>();
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000024B0 File Offset: 0x000006B0
		public static ProceduralImage GetProceduralImage(string name, int index)
		{
			//CardInfo cardInfo = CardChoice.instance.cards.First((CardInfo c) => c.name.Equals(name));
			CardInfo cardInfo = CardManager.cards.Values.Select(card => card.cardInfo).First(c => c.name.Equals(name));
			CharacterStatModifiers componentInChildren = cardInfo.gameObject.GetComponentInChildren<CharacterStatModifiers>();
			ProceduralImage[] componentsInChildren = componentInChildren.AddObjectToPlayer.GetComponentsInChildren<ProceduralImage>();
			foreach (ProceduralImage proceduralImage in componentsInChildren)
			{
				HDC.Log(proceduralImage.ToString());
			}
			return componentsInChildren[index];
		}
		public static float GetLineEffectRadius(LineEffect lineEffect)
        {
			//LineEffect test = new LineEffect();
			//var range = typeof(LineEffect).GetMethod("GetRadius", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(lineEffect, new object[] { });
			//return (float)range;
			return (float)lineEffect.InvokeMethod("GetRadius");
		}
	}
}
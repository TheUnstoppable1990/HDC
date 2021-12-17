using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

namespace HDC.Utilities
{
	// Token: 0x02000004 RID: 4
	internal static class AssetTools
	{
		// Token: 0x0600000D RID: 13 RVA: 0x00002400 File Offset: 0x00000600
		public static GameObject GetLineEffect(string name)
		{
			CardInfo cardInfo = CardChoice.instance.cards.First((CardInfo c) => c.name.Equals(name));
			CharacterStatModifiers componentInChildren = cardInfo.gameObject.GetComponentInChildren<CharacterStatModifiers>();
			return componentInChildren.AddObjectToPlayer.GetComponentInChildren<LineEffect>().gameObject;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002458 File Offset: 0x00000658
		public static ProceduralImage GetProceduralImage(string name)
		{
			CardInfo cardInfo = CardChoice.instance.cards.First((CardInfo c) => c.name.Equals(name));
			CharacterStatModifiers componentInChildren = cardInfo.gameObject.GetComponentInChildren<CharacterStatModifiers>();
			return componentInChildren.AddObjectToPlayer.GetComponentInChildren<ProceduralImage>();
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000024B0 File Offset: 0x000006B0
		public static ProceduralImage GetProceduralImage(string name, int index)
		{
			CardInfo cardInfo = CardChoice.instance.cards.First((CardInfo c) => c.name.Equals(name));
			CharacterStatModifiers componentInChildren = cardInfo.gameObject.GetComponentInChildren<CharacterStatModifiers>();
			ProceduralImage[] componentsInChildren = componentInChildren.AddObjectToPlayer.GetComponentsInChildren<ProceduralImage>();
			foreach (ProceduralImage proceduralImage in componentsInChildren)
			{
				UnityEngine.Debug.Log(proceduralImage.ToString());
			}
			return componentsInChildren[index];
		}
	}
}
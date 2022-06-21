using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassesManagerReborn;
using System.Collections;
using HDC.Cards;


namespace HDC.Class
{
    internal class PaleontologistClass : ClassHandler
    {
        internal static string name = "Paleontologist";

        public override IEnumerator Init()
        {
            while (!(Paleontologist.card && DigSite.card && Carnivore.card && Herbivore.card && PiercingTeeth.card && RendingClaws.card && ArmorPlates.card)) yield return null;
            ClassesRegistry.Register(Paleontologist.card, CardType.Entry);
            ClassesRegistry.Register(DigSite.card, CardType.Card, Paleontologist.card);
            ClassesRegistry.Register(Carnivore.card, CardType.SubClass, Paleontologist.card);
            ClassesRegistry.Register(Herbivore.card, CardType.SubClass, Paleontologist.card);
            ClassesRegistry.Register(PiercingTeeth.card, CardType.Card, Carnivore.card);
            ClassesRegistry.Register(RendingClaws.card, CardType.Card, Carnivore.card);
            ClassesRegistry.Register(ArmorPlates.card, CardType.Card, Herbivore.card);
            ClassesRegistry.Register(DefensiveSpines.card, CardType.Card, Herbivore.card);
        }
    }
}

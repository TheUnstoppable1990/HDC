# Hatchet Daddy's Cards (HDC)
-------------------------------
This is a BepInEx compatible mod for ROUNDS that adds a few cards to the game.

Thank you to everyone in the ROUNDS modding community, who helped me, especially Pykess, Ascyst and Willis.

## Easy Installation Instructions
---------------------------------

Download [r2modman](https://rounds.thunderstore.io/package/ebkr/r2modman/), set up a Rounds profile, and add `HDC` to the profile. All dependencies will be automatically installed. Just click `Start Modded` to start playing!

## Cards Added
-------------

### Basic Cards

---------------

#### Lil Defensive

-Reduces block cooldown

#### Lil Offensive

-Increase maximum ammo

<details>
	<summary><h3>Angel Cards</h3></summary>
	
	Divine Blessing: Reduces block cooldown and heals the player a percentage of their max HP on block
	
	Meditation: Heal as you stand still (healing increases the longer you stand still)
	
	Celestial Countdown: Stand still to unleash divine gun powers
	
	Paladin's Bulwark: Heals based on enemies in aura and heals your allies 
		(if you have more than half health whether or not enemies are present)
	
	Holy Light:Charge by healing, smite by blocking
	
</details>
	
---

<details>
	<summary><h3>Dino Pun Cards</h3></summary>
	
	Paleontologist: Grabs a Random Dinosaur card to give to the player 

	Ankylosaurus: Does Thorns damage, lots of knockback

	Parasaurolophus: Increases stats a crazy amount when near an enemy

	Pachycephalosaurus: Reduces block cooldown, adds health, and gives you a BIG headbutt on block

	Brontosaurus: Adds a metric crap ton of health, regeneration and decay, but makes you extremely slow and offensively incapable

	Stegosaurus: Adds additional blocks and damage reducing plates, each plate reduces damage by 50% its like doubling your health 

	Pterodactyl: Adds flight and a minor speed boost

	Raptor: Adds speed speed speed

	Rex: Adds size and power!

	Triceratops: Adds plenty of defense and block

</details>

---

### Signature Cards

---------------

#### Behind You

-Teleports you behind an enemy player when you block. Increases Block Cooldown by 50% (Best Card)

#### Point Blank

-Very short range, very deadly


### Version Notes

-----------------

### v1.1.0

---------

-Changes
	
	-Added Ankylosaurus and Parasaurolophus

	-Tweaked Pachycephalosaurus and Triceratops

### v1.0.4

---------

-Changes

	-Rework to Pachy

	-Removed block patch as dependency

### v1.0.2

---------

-Changes

	-Added Pachycephalosaurus

### v1.0.2

---------

-Fixes

	-Adjusted the Dinosaur Art

	-Complete rebalance of Dinosaur Card Stats

-Changes
	
	-Added Paleontologist, gives a random Dinosaur Card 

### v1.0.1

---------

-Fixes
	
	-Tweaked Holy Light and the way it works, should be more balanced and less buggy

	-Tweaked the background of some of the Art

-To Do

	-Fix Growing countdown thing on celestial countdown, but seems to just be aesthetic so nbd at the moment

### v1.0.0

---------

-Changes

	-ALL CARDS HAVE ART

	-All cards going forward will have art

	-Minor tweaks here and there

	-Did I mention ART?

### v0.6.0

---------

-Changes

	-Changed Rarity/Balance of Lil Offensive and Defensive 

-Fixes

	-fixed celestial countdown persistence bug and added art

### v0.5.5

---------

-Fixes

	-willuwontu fixed Celestial Countdown so it actually has the cooldown disk now

### v0.5.4

---------

-Fixes

	-Adressing the "Bronto Problem"

### v0.5.3

----------

-Changes

	-Added Brontosaurus because we need more overpowered cards

### v0.5.2

----------

-Changes

	Added Stegosaurus Because MOAR BLOCK CARDS

### v0.5.0 - v0.5.1

----------

-Changes

	Just a minor update, dun worry about it. just working on stuff shhhh

### v0.1.5

-----------

-Changes

	DESTROYED EVERYTHING PAST 0.1.2 AND HAD TO GET IT BACK FROM A DECOMPILE

### v0.1.4

-----------

-Fixes

	-Got shamed into fixing the healing mechanics by willuwontu. Looks much better and functions like other healing cards now. (thanks will)

	-Fixed the line of sight issue with Paladin as well making it more ~balanced~ and ~fair~ I guess :P

	-Needs more tweaking but too lazy right now. It works, can just work better in future 

### v0.1.3

-----------

-Fixes

	-willuwontu, helped me fix Holy Light so its not devestatingly at full power after a death... probably still applies to phoenix revivals but i wanna leave that as a feature ;) 

### v0.1.2

-----------

-New Card

	-Got bullied into adding another card. Pterodactyl - Has unlimited flight and a minor speed boost

### v0.1.1

------------

-Changes

	-Updated Triceratops, now has horns damage

### v0.1.0

------------

-New Cards

	-Three new cards that are dinosaur themed and have different flavor text everytime you play the game
	
	-Try to find all the puns and ruin your sense of humor forever :D

### v0.0.14

-------------

-Changes

	-Update for new Unbound update

### v0.0.13

-------------

-Changes

	-Fixed more holy light stuff

### v0.0.12

-------------

-Changes

	-Improved Visual effect for Holy Light

### v0.0.10

-------------

-Changes

	-Two new COMMON cards. The first common cards to be included in the HDC pack

### v0.0.9

-------------

-Changes

	-Using Modding Utils to add a glow effect for Celestial Countdown for a temporary visual indicator

### v0.0.8

-------------

-Fixes
	
	-Forgot to call a method for resetting effects. Thanks to Pykes for pointing this out XD

	-Effects should no longer persist in instances like Rare Glitched card 

#### v0.0.7

-------------

-Changes

	-Buffed Paladin with more HP

	-Minor code cleanup

#### v0.0.6

-------------

-Fixes
	
	-Solved the persistence issue between rounds (I think)

#### v0.0.5

-------------

-Changes

	-Set Meditation to a flat number (not based on max health) with ramping hp every second you are standing still

	-Paladins Bulwark is now called Paladins Endurance and heals based on enemies in aura and heals your allies (if you have more than half health whether or not enemies are present)


#### v0.0.3

-------------

-Changes

	-just some cleaning up of the code to try and reduce lag from certain effects

	-nerfed some of the card effects to help balance them

-Known issues

	-Effect persistence between games (I am working on it)

-Thanks Willis for some of the code improvements implemented in this update :)

#### v0.0.1

-------------

-5 New Cards

	-Divine Blessing

		-Reduced block cooldown and healing on block

	-Meditation

		-Heal as you stand still (healing increases the longer you stand still)

	-Celestial Countdown

		-Stand still to unleash divine gun powers

	-Paladin's Bulwark

		-Gain health the more enemies that are around you

	-Behind You

		-Teleport behind enemies when you block

#### To-Do List

---------------

~-Add Card art~

-Add particle effects 

~-Add countdown effect to Celestial Countdown~

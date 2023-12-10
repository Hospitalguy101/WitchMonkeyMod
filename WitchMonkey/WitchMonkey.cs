using MelonLoader;
using BTD_Mod_Helper;
using WitchMonkey;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Towers;
using Il2CppAssets.Scripts.Data.Gameplay.Mods;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Unity;
using static UnityEngine.ExpressionEvaluator;
using UnityEngine;

using BTD_Mod_Helper.Extensions;
using static Il2CppAssets.Scripts.ObjectId;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using System.Linq;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppSystem.Linq;

[assembly: MelonInfo(typeof(WitchMonkey.WitchMonkeyMod), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace WitchMonkey;

public class WitchMonkeyMod : BloonsTD6Mod
{
    public override void OnApplicationStart()
    {
        ModHelper.Msg<WitchMonkeyMod>("WitchMonkey loaded!");
    }

    public class Main : BloonsTD6Mod
    {
        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            ModHelper.Msg<Main>("Mod Loaded!");
        }
    }

    public class WitchMonkey : ModTower
    {
        public override TowerSet TowerSet => TowerSet.Magic;
        public override string BaseTower => TowerType.WizardMonkey;
        public override int Cost => 350;
        public override int TopPathUpgrades => 5;
        public override int MiddlePathUpgrades => 5;
        public override int BottomPathUpgrades => 5;
        public override string DisplayName => "Witch Monkey";
        public override string Description => "Curses nearby bloons, forcing them to take damage over time.";

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            var wizardModel = Game.instance.model.GetTowerFromId("WizardMonkey").GetAttackModel().Duplicate();
            var baseWeapon = Game.instance.model.GetTowerFromId("DartlingGunner-200").GetAttackModel().weapons[0].Duplicate();
            var attackModel = towerModel.GetAttackModel();

            towerModel.GetAttackModel().weapons[0] = baseWeapon;
            attackModel.weapons[0].Rate = wizardModel.weapons[0].Rate;
            attackModel.weapons[0].projectile.display = wizardModel.weapons[0].projectile.display;
            attackModel.weapons[0].projectile.GetDamageModel().damage = 0;
            attackModel.weapons[0].projectile.GetBehavior<TravelStraitModel>().Speed = 400;
            attackModel.weapons[0].projectile.GetBehavior<AddBehaviorToBloonModel>().isUnique = false;
        }
    }

    public class WitchMonkeyDisplay : ModTowerDisplay<WitchMonkey>
    {
        public override string BaseDisplay => GetDisplay(TowerType.WizardMonkey, 0, 0, 3);
        public override bool UseForTower(int[] tiers)
        {
            return true;
        }
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            node.GetRenderers<SkinnedMeshRenderer>()[0].material.mainTexture = GetTexture(mod, "WitchMonkeyDisplay");
        }
    }

    public class PowerfulCurse : ModUpgrade<WitchMonkey>
    {
        public override string DisplayName => "Powerful Curse";
        public override int Path => TOP;
        public override int Tier => 1;
        public override int Cost => 400;
        public override string Description => "More powerful curse increases damage.";
        public override string Portrait => base.Portrait;
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var projectile = towerModel.GetAttackModel().weapons[0].projectile;
            projectile.GetBehavior<AddBehaviorToBloonModel>().GetBehavior<DamageOverTimeModel>().damage++;
        }
    }

    public class VeryPowerfulCurse : ModUpgrade<WitchMonkey>
    {
        public override string DisplayName => "Very Powerful Curse";
        public override int Path => TOP;
        public override int Tier => 2;
        public override int Cost => 550;
        public override string Description => "Even stronger curse damages bloons even more.";
        public override string Portrait => base.Portrait;
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var projectile = towerModel.GetAttackModel().weapons[0].projectile;
            projectile.GetBehavior<AddBehaviorToBloonModel>().GetBehavior<DamageOverTimeModel>().damage++;
        }
    }

    public class EnhancedWitchcraft : ModUpgrade<WitchMonkey>
    {
        public override string DisplayName => "Enhanced Witchcraft";
        public override int Path => TOP;
        public override int Tier => 3;
        public override int Cost => 900;
        public override string Description => "Unlocks the secrets of dark magic, greatly enhancing attack speed.";
        public override string Portrait => base.Portrait;
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetAttackModel().weapons[0].Rate /= 2f;
        }
    }

    public class BlackMagic : ModUpgrade<WitchMonkey>
    {
        public override string DisplayName => "Black Magic";
        public override int Path => TOP;
        public override int Tier => 4;
        public override int Cost => 3000;
        public override string Description => "Black Magic lasts longer and does more damage.";
        public override string Portrait => base.Portrait;
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var projectile = towerModel.GetAttackModel().weapons[0].projectile;
            projectile.GetBehavior<AddBehaviorToBloonModel>().lifespan *= 2;
            if (projectile.GetBehaviors<SlowModel>().Count > 0) projectile.GetBehavior<SlowModel>().lifespan = projectile.GetBehavior<AddBehaviorToBloonModel>().lifespan;
            projectile.GetBehavior<AddBehaviorToBloonModel>().GetBehavior<DamageOverTimeModel>().Interval = 0.5f;
            projectile.GetBehavior<AddBehaviorToBloonModel>().GetBehavior<DamageOverTimeModel>().damage += 5;
        }
    }

    public class WitchKingOfBloonmar : ModUpgrade<WitchMonkey>
    {
        public override string DisplayName => "Witchking of Bloonmar";
        public override int Path => TOP;
        public override int Tier => 5;
        public override int Cost => 38000;
        public override string Description => "With a heart as black as death, he destroys all before him with uncomprimising conviction.";
        public override string Portrait => base.Portrait;
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var projectile = towerModel.GetAttackModel().weapons[0].projectile;
            projectile.GetBehavior<AddBehaviorToBloonModel>().lifespan *= 9999999999;
            if (projectile.GetBehaviors<SlowModel>().Count > 0) projectile.GetBehavior<SlowModel>().lifespan = projectile.GetBehavior<AddBehaviorToBloonModel>().lifespan;
            projectile.GetBehavior<AddBehaviorToBloonModel>().GetBehavior<DamageOverTimeModel>().Interval = 0.01f;
            projectile.GetBehavior<AddBehaviorToBloonModel>().GetBehavior<DamageOverTimeModel>().damage = 3;
            towerModel.GetAttackModel().weapons[0].projectile.pierce += 20;
            towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().immuneBloonProperties = 0;
        }
    }

    public class QuickCurse : ModUpgrade<WitchMonkey>
    {
        public override string DisplayName => "Quick Curse";
        public override int Path => BOTTOM;
        public override int Tier => 1;
        public override int Cost => 350;
        public override string Description => "Shoots curses out faster.";
        public override string Portrait => base.Portrait;
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetAttackModel().weapons[0].Rate /= 1.2f;
        }
    }

    public class StrongMagic : ModUpgrade<WitchMonkey>
    {
        public override string DisplayName => "Strong Magic";
        public override int Path => BOTTOM;
        public override int Tier => 2;
        public override int Cost => 500;
        public override string Description => "Curses bloons faster and can hit more bloons per shot.";
        public override string Portrait => base.Portrait;
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetAttackModel().weapons[0].Rate /= 1.2f;
            towerModel.GetAttackModel().weapons[0].projectile.pierce += 2;
        }
    }

    public class BookOfStrength : ModUpgrade<WitchMonkey>
    {
        public override string DisplayName => "Book of Strength";
        public override int Path => BOTTOM;
        public override int Tier => 3;
        public override int Cost => 700;
        public override string Description => "Equip a powerful book that increases range and can hit any bloon type, even camo.";
        public override string Portrait => base.Portrait;
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.AddBehavior(new OverrideCamoDetectionModel("WitchMonkey", true));
            towerModel.range += 20;
            towerModel.GetAttackModel().range += 20;
            towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().immuneBloonProperties = 0;
        }
    }

    public class ForgottenSecrets : ModUpgrade<WitchMonkey>
    {
        public override string DisplayName => "Forgotten Secrets";
        public override int Path => BOTTOM;
        public override int Tier => 4;
        public override int Cost => 2900;
        public override string Description => "Ancient techniques allow the Witch monkey's curses to seek out and rip through bloons.";
        public override string Portrait => base.Portrait;
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var ricochet = Game.instance.model.GetTowerFromId("BoomerangMonkey-300").GetAttackModel().weapons[0].projectile.GetBehavior<RetargetOnContactModel>().Duplicate();
            towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(ricochet);
            towerModel.GetAttackModel().weapons[0].projectile.pierce += 25;
        }
    }

    public class LoreMaster : ModUpgrade<WitchMonkey>
    {
        public override string DisplayName => "Lore Master";
        public override int Path => BOTTOM;
        public override int Tier => 5;
        public override int Cost => 20000;
        public override string Description => "For he who knows all shall fear none.";
        public override string Portrait => base.Portrait;
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var projectile = towerModel.GetAttackModel().weapons[0].projectile;
            var dotModel = projectile.GetBehavior<AddBehaviorToBloonModel>().GetBehavior<DamageOverTimeModel>();
            towerModel.GetAttackModel().range = 9999999;
            towerModel.range = 20;
            towerModel.isGlobalRange = true;
            towerModel.GetAttackModel().weapons[0].Rate /= 2.5f;
            projectile.pierce = 99999999;
            dotModel.damage += 15;

        }
    }

    public class CorruptingBlasts : ModUpgrade<WitchMonkey>
    {
        public override string DisplayName => "Corrupting Blasts";
        public override int Path => MIDDLE;
        public override int Tier => 1;
        public override int Cost => 300;
        public override string Description => "Blasts now corrupt bloons, slowing them down.";
        public override string Portrait => base.Portrait;
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var slowModel = Game.instance.model.GetTowerFromId("GlueGunner").GetAttackModel().weapons[0].projectile.GetBehavior<SlowModel>().Duplicate();
            slowModel.lifespan = towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<AddBehaviorToBloonModel>().lifespan;
            slowModel.multiplier = 0.6f;
            towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(slowModel);
        }
    }

    public class Desecration : ModUpgrade<WitchMonkey>
    {
        public override string DisplayName => "Entrapment";
        public override int Path => MIDDLE;
        public override int Tier => 2;
        public override int Cost => 400;
        public override string Description => "Curse lasts for much longer.";
        public override string Portrait => base.Portrait;
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var projectile = towerModel.GetAttackModel().weapons[0].projectile;
            projectile.GetBehavior<AddBehaviorToBloonModel>().lifespan *= 2;
            projectile.GetBehavior<SlowModel>().lifespan = projectile.GetBehavior<AddBehaviorToBloonModel>().lifespan;
        }
    }

    public class WeakeningPotion : ModUpgrade<WitchMonkey>
    {
        public override string DisplayName => "Weakening Potion";
        public override int Path => MIDDLE;
        public override int Tier => 3;
        public override int Cost => 1900;
        public override string Description => "Cursed bloons take more damage from all sources.";
        public override string Portrait => base.Portrait;
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var projectile = towerModel.GetAttackModel().weapons[0].projectile;
            var debuff = new AddBonusDamagePerHitToBloonModel("weaknesspotion", "idk", projectile.GetBehavior<AddBehaviorToBloonModel>().lifespan, 1, 999999, true, false, true, "0");
            projectile.AddBehavior(debuff);
        }
    }

    public class PotionOfPower : ModUpgrade<WitchMonkey>
    {
        public override string DisplayName => "Potion of Power";
        public override int Path => MIDDLE;
        public override int Tier => 4;
        public override int Cost => 4000;
        public override string Description => "Potion of Power Ability: Grants all allies in radius increased damage.";
        public override string Portrait => base.Portrait;
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var ability = Game.instance.model.GetTowerFromId("PatFusty 3").GetBehavior<AbilityModel>().Duplicate();
            ability.displayName = "Potion of Power";
            ability.description = "Grants all allies in radius increased damage.";
            ability.name = "Potion of Power";
            ability.icon = GetSpriteReference(Icon);
            ability.GetBehavior<ActivateTowerDamageSupportZoneModel>().range = towerModel.range;
            towerModel.AddBehavior(ability);
        }
    }

    public class ThePerfectPotion : ModUpgrade<WitchMonkey>
    {
        public override string DisplayName => "The Perfect Potion";
        public override int Path => MIDDLE;
        public override int Tier => 5;
        public override int Cost => 26000;
        public override string Description => "Potion of Power ability extends map-wide and applies weakness potion to all bloons on screen.";
        public override string Portrait => base.Portrait;
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var ability = towerModel.GetAbility();
            ability.GetBehavior<ActivateTowerDamageSupportZoneModel>().range += 99999999999;
            ability.icon = GetSpriteReference(Icon);
            ability.cooldown = 30f;
            var glueAbility = Game.instance.model.GetTowerFromId("GlueGunner-050").GetAbility().GetBehavior<ActivateAttackModel>();
            glueAbility.attacks[0].weapons[0].projectile.RemoveBehavior<SlowModel>();
            glueAbility.attacks[0].RemoveBehavior<DisplayModel>();
            ability.AddBehavior(glueAbility);
            towerModel.GetAttackModel().weapons[0].Rate /= 2f;
        }
    }
}
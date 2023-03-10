using System;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;

namespace NyxCombo
{
    internal class Program
    {
        private static bool _useCombo;

        private static void Main(string[] args)
        {
            Game.OnUpdate += Game_OnUpdate;
            Player.OnExecuteOrder += Player_OnExecuteOrder;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            var myHero = ObjectManager.LocalHero;
            if (myHero == null || myHero.ClassId != ClassId.CDOTA_Unit_Hero_Nyx_Assassin)
            {
                return;
            }

            if (!_useCombo)
            {
                return;
            }

            if (myHero.IsStunned() || myHero.IsSilenced())
            {
                return;
            }

            if (!myHero.HasModifier("modifier_nyx_assassin_vendetta"))
            {
                return;
            }

            var impale = myHero.Spellbook.SpellQ;
            if (impale == null || !impale.CanBeCasted() || myHero.Mana < impale.ManaCost)
            {
                return;
            }

            var target = TargetSelector.ClosestToMouse(myHero);
            if (target == null)
            {
                return;
            }

            var order = new Order(myHero, OrderType.AttackTarget, target);
            myHero.Stop();
            myHero.Attack(target);
            myHero.Stop();
            myHero.Cast(impale, false, true);
            Player.Order(order);
        }

        private static void Player_OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (sender.Hero == null || sender.Hero.ClassId != ClassId.CDOTA_Unit_Hero_Nyx_Assassin)
            {
                return;
            }

            if (args.OrderId == OrderId.Ability && args.Ability.Id == AbilityId.nyx_assassin_impale)
            {
                _useCombo = false;
            }

            if (args.OrderId == OrderId.AttackTarget || args.OrderId == OrderId.AttackLocation)
            {
                _useCombo = true;
            }
        }
    }
}
using System;
using BepInEx;
using UnityEngine;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using System.Collections.Generic;
using System.Collections;
using Smoke;
using MonoMod.RuntimeDetour;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using MoreSlugcats;
using HUD;
using Expedition;
using Watcher;
using Menu.Remix.MixedUI;
using RWCustom;
using Vinki;
using GravelSlug;

namespace GravelSlug
{
    public class PlayerHooks
    {
        public void ApplyHooks()
        {
            On.Player.Jump += Player_Jump;
        }

        public void Player_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);
            if (IsGravelFeral(self))
            {
                // All your existing logic...
                if (self.jumpBoost == 6)
                {
                    self.mainBodyChunk.vel *= 1.5f;
                    self.mainBodyChunk.vel.x *= 2.2f;
                    GravelCough(self, true);
                }

                // etc.
            }
        }

        private bool IsGravelFeral(Player player)
        {
            if (player.SlugCatClass.value == "Gravelslug")
            {
                if (!GravelHasAbilities(player))
                {
                    return false;
                }
                if (player.room != null && player.room.game.IsStorySession)
                {
                    return player.Malnourished || (player.room.world.region != null && player.room.world.name == "HR");
                }
                if (player.room != null && player.room.game.IsArenaSession)
                {
                    return player.playerState.permanentDamageTracking >= 0.05;
                }
                return false;
            }
            return false;
        }

        private static void GravelCough(Player self, bool loud)
        {
            // Add whatever this function does
        }

        private bool GravelHasAbilities(Player player)
        {
            if (player.SlugCatClass.value == "Gravelslug" && player.room != null)
            {
                if (player.room.game.IsArenaSession || (player.room.game.IsStorySession && (ModManager.Expedition && player.room.game.rainWorld.ExpeditionMode)))
                {
                    return true;
                }
                return player.room.game.IsStorySession && (player.room.game.GetStorySession.characterStats.name.value != "Gravelslug" || player.room.game.GetStorySession.saveState.deathPersistentSaveData.theMark || Plugin.GravelOptionsMenu.KeepAbilities.Value);
            }
            return false;
        }
    }
}
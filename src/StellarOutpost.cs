using System;
using BepInEx;
using UnityEngine;
using Pom;
using EffExt;

namespace StellarOutpost
{
    [BepInPlugin(MOD_ID, "Stellar Outpost", "0.1.0")]

    class StellarOutpost : BaseUnityPlugin
    {
        private const string MOD_ID = "kingmaxthe2.StellarOutpost";

        public void OnEnable()
        {
            //On.AboveCloudsView.ctor += AboveCloudsView_ctor;
            On.Room.Loaded += Room_Loaded;
            On.AntiGravity.Update += AntiGravity_Update;
            On.AntiGravity.BrokenAntiGravity.Update += BrokenAntiGravity_Update;
            On.Watcher.WatcherEnums.RoomEffectType.RegisterValues += RoomEffectType_RegisterValues1;
            On.Watcher.WatcherEnums.RoomEffectType.UnregisterValues += RoomEffectType_UnregisterValues1;
            On.DevInterface.RoomSettingsPage.DevEffectGetCategoryFromEffectType += RoomSettingsPage_DevEffectGetCategoryFromEffectType;
        }

        private void BrokenAntiGravity_Update(On.AntiGravity.BrokenAntiGravity.orig_Update orig, AntiGravity.BrokenAntiGravity self)
        {
            orig(self);
            if (self.game.world.name == "WSOK" && self.game.world.rainCycle != null)
            {
                if ((float)self.game.world.rainCycle.preTimer > 0f)
                {
                    if (self.on)
                    {
                        self.counter = 0;
                    }
                }
                else if (self.game.world.rainCycle.AmountLeft <= 0f)
                {
                    if (self.on)
                    {
                        self.counter--;
                    }
                    else
                    {
                        self.counter = 10;
                    }
                }
            }
        }

        private void Room_Loaded(On.Room.orig_Loaded orig, Room self)
        {
            orig(self);
            if (self.game == null)
            {
                return;
            }
            for (int num3 = 0; num3 < self.roomSettings.effects.Count; num3++)
            {
                if (self.roomSettings.effects[num3].type == SpaceEffects.Space)
                {
                    self.AddObject(new SpaceView(self));
                    /*BackgroundScene stars = new BackgroundScene(self);
                    self.AddObject(stars);
                    stars.AddElement(new BackgroundScene.Simple2DBackgroundIllustration(stars, "atc_space", new Vector2(683f, 384f)));*/
                }
            }
        }

        private void RoomEffectType_UnregisterValues1(On.Watcher.WatcherEnums.RoomEffectType.orig_UnregisterValues orig)
        {
            orig();
            SpaceEffects.UnregisterValues();
        }

        private void RoomEffectType_RegisterValues1(On.Watcher.WatcherEnums.RoomEffectType.orig_RegisterValues orig)
        {
            orig();
            SpaceEffects.RegisterValues();
        }

        private DevInterface.RoomSettingsPage.DevEffectsCategories RoomSettingsPage_DevEffectGetCategoryFromEffectType(On.DevInterface.RoomSettingsPage.orig_DevEffectGetCategoryFromEffectType orig, DevInterface.RoomSettingsPage self, RoomSettings.RoomEffect.Type type)
        {
            if (type == SpaceEffects.Space)
            {
                return DevInterface.RoomSettingsPage.DevEffectsCategories.Decorations;
            }
            if (type == SpaceEffects.InvertBrokenZeroG)
            {
                return DevInterface.RoomSettingsPage.DevEffectsCategories.Gameplay;
            }
            return orig.Invoke(self, type);
        }

        private void AntiGravity_Update(On.AntiGravity.orig_Update orig, AntiGravity self, bool eu)
        {

            orig(self, eu);


            if (self.room.roomSettings.GetEffectAmount(SpaceEffects.InvertBrokenZeroG) > 0f && self.room.world.rainCycle.brokenAntiGrav != null && self.room.roomSettings.GetEffectAmount(RoomSettings.RoomEffect.Type.BrokenZeroG) > 0f)
            {
                float num = self.room.roomSettings.GetEffectAmount(RoomSettings.RoomEffect.Type.ZeroG);
                if (num < 1f)
                {
                    num = Mathf.Lerp(0f, 0.85f, num);
                }
                if (!self.room.world.rainCycle.brokenAntiGrav.on)
                {
                    num = num * ((1f - self.room.world.rainCycle.brokenAntiGrav.CurrentAntiGravity));
                }   
                else
                {
                    num = num * (1f - (self.room.world.rainCycle.brokenAntiGrav.CurrentAntiGravity * self.room.roomSettings.GetEffectAmount(RoomSettings.RoomEffect.Type.BrokenZeroG)));
                }
                self.room.gravity = 1f - num;
            }
        }

        private void AboveCloudsView_ctor(On.AboveCloudsView.orig_ctor orig, AboveCloudsView self, Room room, RoomSettings.RoomEffect effect)
        {
            orig(self, room, effect);
            if (self.room.roomSettings.GetEffectAmount(SpaceEffects.Space) > 0f)
            {
                var space = new BackgroundScene.Simple2DBackgroundIllustration(self, "atc_space", new Vector2(683f, 384f));

                self.AddElement(space);

                //self.daySky = self.nightSky;
                //self.duskSky = self.nightSky;
                //self.nightSky = space;
            }
        }
    }

    public class SpaceEffects
    {
        public static void RegisterValues()
        {
            Space = new RoomSettings.RoomEffect.Type("Space", true);
            InvertBrokenZeroG = new RoomSettings.RoomEffect.Type("InvertBrokenZeroG", true);
        }

        // Token: 0x06003E4E RID: 15950 RVA: 0x0046C284 File Offset: 0x0046A484
        public static void UnregisterValues()
        {
            RoomSettings.RoomEffect.Type invertbrokenzerog = InvertBrokenZeroG;
            if (invertbrokenzerog != null)
            {
                invertbrokenzerog.Unregister();
            }
            InvertBrokenZeroG = null;
            RoomSettings.RoomEffect.Type space = Space;
            if (space != null)
            {
                space.Unregister();
            }
            Space = null;
        }

        public static RoomSettings.RoomEffect.Type Space;
        public static RoomSettings.RoomEffect.Type InvertBrokenZeroG;
    }
}
﻿using System;
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
using Menu.Remix.MixedUI;
using Vinki;
using SprayCans;
using static SlugBase.JsonAny;
using JetBrains.Annotations;
using SlugBase;

namespace GravelSlug
{
    [BepInPlugin(MOD_ID, "Gravel Slug", "0.1.0")]

    class Plugin : BaseUnityPlugin
    {
        private const string MOD_ID = "kingmaxthe2.gravelslug";

        public static readonly PlayerFeature<bool> GravelGut = PlayerBool("gravelslug/gravel_gut");

        // Add hooks
        public void OnEnable()
        {
            //On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

            // Put your custom hooks here!

            On.Player.Jump += Player_Jump; //cool epic jumps
            On.Player.UpdateBodyMode += Player_UpdateBodyMode;
            On.Player.Die += Player_Die; //fsssss... KA-BOOM!!
            On.Player.SwallowObject += Player_SwallowObject; //gives food n stuff on swallow
            On.Player.CanBeSwallowed += MoreSwallows; //allows swallowing additional objects such as spears and edibles
            On.Player.ThrownSpear += ThrowFire; //doubles spear damage
            On.Player.Stun += VoidFit; //the coughing fit
            On.Player.CanMaulCreature += MaulToggle; //toggles mauling
            On.Player.Grabbed += NeverGiveUp; //delays game over from grab
            On.Player.DeathByBiteMultiplier += ThickSkinned; //lizard bite resistance
            On.Player.Update += Player_Update; //dissolve timers and such
            On.Player.ctor += Player_ctor; //applies things on spawn
            On.Player.Destroy += Player_Destroy; //BOOM!!
            On.Player.ObjectEaten += Player_ObjectEaten; //additional time for food
            On.Player.SleepUpdate += Player_SleepUpdate; //spit out tummyitem when waking up
            On.Player.SlugSlamConditions += BoxORocks; //gourmand smash
            On.PlayerGraphics.Update += PlayerGraphics_Update; //removes mark visibility and adds shaky
            On.Player.Regurgitate += Player_Regurgitate; //spit up rocks
            On.Player.GrabUpdate += Player_GrabUpdate; //allows for spitting up of rocks
            On.Player.SpitOutOfShortCut += Player_SpitOutOfShortCut;
            On.Player.StomachGlowLightColor += Player_StomachGlowLightColor;
            On.PlayerGraphics.ColoredBodyPartList += PlayerGraphics_ColoredBodyPartList;
            On.PlayerGraphics.DefaultBodyPartColorHex += PlayerGraphics_DefaultBodyPartColorHex;
            On.PlayerGraphics.DefaultSlugcatColor += PlayerGraphics_DefaultSlugcatColor;
            On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
            On.PlayerGraphics.JollyFaceColorMenu += PlayerGraphics_JollyFaceColorMenu;
            On.Player.SetMalnourished += Player_SetMalnourished;
            On.Creature.Violence += Creature_Violence;
            On.Player.JumpOnChunk += Player_JumpOnChunk; // suppossed to boost off of creatures but currently isnt working
            On.Player.Grabability += Player_Grabability;
            On.Player.MaulingUpdate += Player_MaulingUpdate; // mauling effects for GE
            On.Player.EatMeatUpdate += Player_EatMeatUpdate; // food tier increase from corpse eating

            On.RainWorld.OnModsInit += RainWorld_OnModsInit;

            On.SlugcatStats.NourishmentOfObjectEaten += SlugcatStats_NourishmentOfObjectEaten; // increase food amount per quest completion
            On.SlugcatStats.getSlugcatStoryRegions += SlugcatStats_getSlugcatStoryRegions; //sets story regions
            On.SlugcatStats.getSlugcatOptionalRegions += SlugcatStats_getSlugcatOptionalRegions; //sets optional regions
            On.SlugcatStats.SlugcatFoodMeter += SlugcatStats_SlugcatFoodMeter;
            On.SlugcatStats.ctor += SlugcatStats_ctor;

            On.Menu.CharacterSelectPage.UpdateSelectedSlugcat += CharacterSelectPage_UpdateSelectedSlugcat;

            IL.PlayerGraphics.Update += PlayerGraphics_Update; //enables spitting animation for rocks

            On.Spear.HitSomething += Spear_HitSomething; //spear resistance
            On.Weapon.Thrown += Weapon_Thrown; //boost weapons for gravelslug

            On.Region.GetProperRegionAcronym += Region_GetProperRegionAcronym; //sets sh as cl
            On.MoreSlugcats.MSCRoomSpecificScript.AddRoomSpecificScript += GravelStart; //add room specific scripts
            On.MiscWorldSaveData.ctor += MiscWorldSaveData_ctor; //
            On.RoomRain.ThrowAroundObjects += RoomRain_ThrowAroundObjects;
            On.RainCycle.GetDesiredCycleLength += RainCycle_GetDesiredCycleLength;
            //On.RegionGate.customOEGateRequirements += RegionGate_customOEGateRequirements; // enables OE gate for GE  !!ENABLE WHEN OE IS DONE!!
            On.OverWorld.WorldLoaded += OverWorld_WorldLoaded;
            On.RainWorldGame.GoToRedsGameOver += RainWorldGame_GoToRedsGameOver;
            On.RainWorldGame.RestartGame += RainWorldGame_RestartGame;
            On.RainWorldGame.ExitToVoidSeaSlideShow += RainWorldGame_ExitToVoidSeaSlideShow;

            On.RainCycle.Update += RainCycle_Update; // dissolve update

            On.MoreSlugcats.MoreSlugcatsEnums.GhostID.RegisterValues += GhostID_RegisterValues;
            On.MoreSlugcats.MoreSlugcatsEnums.GhostID.UnregisterValues += GhostID_UnregisterValues;

            On.MoreSlugcats.MoreSlugcatsEnums.TickerID.RegisterValues += TickerID_RegisterValues;
            On.MoreSlugcats.MoreSlugcatsEnums.TickerID.UnregisterValues += TickerID_UnregisterValues;
            On.Menu.StoryGameStatisticsScreen.GetDataFromGame += StoryGameStatisticsScreen_GetDataFromGame;
            On.Menu.StoryGameStatisticsScreen.TickerIsDone += StoryGameStatisticsScreen_TickerIsDone;
            IL.Menu.StoryGameStatisticsScreen.GetDataFromGame += StoryGameStatisticsScreen_GetDataFromGame1;

            On.MoreSlugcats.MoreSlugcatsEnums.SlideShowID.RegisterValues += SlideShowID_RegisterValues;
            On.MoreSlugcats.MoreSlugcatsEnums.SlideShowID.UnregisterValues += SlideShowID_UnregisterValues;
            On.Menu.SlideShow.ctor += SlideShow_ctor;

            On.Menu.SlugcatSelectMenu.CheckJollyCoopAvailable += SlugcatSelectMenu_CheckJollyCoopAvailable; // disables jolly coop for GE campaign
            On.Menu.SleepAndDeathScreen.AddPassageButton += SleepAndDeathScreen_AddPassageButton; //removes passagng
            //On.Menu.SlugcatSelectMenu.SlugcatPageContinue.ctor += SlugcatPageContinue_ctor; // food limit updates for menu {NOT IMPLEMENTED}
            On.Menu.SleepAndDeathScreen.GetDataFromGame += SleepAndDeathScreen_GetDataFromGame;

            //On.MoreSlugcats.MoreSlugcatsEnums.Tutorial.RegisterValues += Tutorial_RegisterValues;
            //On.MoreSlugcats.MoreSlugcatsEnums.Tutorial.UnregisterValues += Tutorial_UnregisterValues;

            //!!!MAKE THESE STANDALONE!!!

            On.MoreSlugcats.DustWave.Update += DustWave_Update;
            On.PhysicalObject.WeatherInertia += PhysicalObject_WeatherInertia;

            //!!!                       !!!

            On.DartMaggot.ShotUpdate += NoStick;
            On.GarbageWorm.Update += GarbageHate;
            On.EggBug.ctor += EggBug_ctor;
            On.VoidSea.VoidWorm.MainWormBehavior.Update += MainWormBehavior_Update;
            On.VoidSea.VoidSeaScene.ArtificerEndUpdate += VoidSeaScene_ArtificerEndUpdate;
            On.TubeWorm.JumpButton += TubeWorm_JumpButton;

            On.ShelterDoor.Update += ShelterDoor_Update;

            On.Expedition.PearlDeliveryChallenge.ValidForThisSlugcat += PearlDeliveryChallenge_ValidForThisSlugcat;
            //On.Expedition.VistaChallenge.Modify

            On.DaddyGraphics.HunterDummy.ApplyPalette += HunterDummy_ApplyPalette; // Gravel long legs
            On.DaddyGraphics.ApplyPalette += DaddyGraphics_ApplyPalette;
            On.DaddyGraphics.DaddyTubeGraphic.ApplyPalette += DaddyTubeGraphic_ApplyPalette;
            On.DaddyGraphics.DaddyDangleTube.ApplyPalette += DaddyDangleTube_ApplyPalette;
            On.DaddyGraphics.DaddyDeadLeg.ApplyPalette += DaddyDeadLeg_ApplyPalette;
            //On.DaddyLongLegs.ctor += DaddyLongLegs_ctor;
            On.DaddyGraphics.ctor += DaddyGraphics_ctor;
            On.DaddyCorruption.ctor += DaddyCorruption_ctor;
            //On.CreatureSymbol.ColorOfCreature += CreatureSymbol_ColorOfCreature; // color for GravelEaten {NOT IMPLEMENTED}

            On.MoreSlugcats.CLOracleBehavior.InitateConversation += CLOracleBehavior_InitateConversation; // pebbles stuffs
            On.MoreSlugcats.CLOracleBehavior.InterruptRain += CLOracleBehavior_InterruptRain;
            On.MoreSlugcats.CLOracleBehavior.TalkToDeadPlayer += CLOracleBehavior_TalkToDeadPlayer;
            On.GhostConversation.AddEvents += GhostOVerride; // echo dialouge
            //On.World.SpawnGhost += World_SpawnGhost; // Karma Protection condition for GE
            On.SLOrcacleState.ctor += SLOrcacleState_ctor; // moon stuff
            On.SLOracleBehaviorHasMark.MoonConversation.AddEvents += MoonOverride;
            On.SLOracleBehaviorHasMark.InterruptRain += SLOracleBehaviorHasMark_InterruptRain;
            On.SLOracleBehaviorHasMark.SpecialEvent += SLOracleBehaviorHasMark_SpecialEvent;
            On.SSOracleBehavior.PebblesConversation.AddEvents += PebblesConversation_AddEvents; // Solventicon Pebbles
            On.SSOracleBehavior.SSOracleRubicon.Update += SSOracleRubicon_Update;
            On.SLOracleBehaviorHasMark.TypeOfMiscItem += SLOracleBehaviorHasMark_TypeOfMiscItem;
            On.SLOracleBehaviorHasMark.WillingToInspectItem += SLOracleBehaviorHasMark_WillingToInspectItem;

            On.WormGrass.WormGrassPatch.InteractWithCreature += WormGrassPatch_InteractWithCreature; //wormgrass immunitty
            On.WormGrass.WormGrassPatch.Update += WormGrassPatch_Update;
            On.WormGrass.WormGrassPatch.AlreadyTrackingCreature += WormGrassPatch_AlreadyTrackingCreature;

            On.MoreSlugcats.DatingSim.ctor += DatingSim_ctor;
            On.MoreSlugcats.DatingSim.InitNextFile += DatingSim_InitNextFile;
            On.ProcessManager.Update += ProcessManager_Update;


            new Hook(typeof(RainCycle).GetMethod("get_MusicAllowed"), (Func<RainCycle, bool> orig, RainCycle cycle) => GravelThreatCheck(cycle) || orig(cycle)); // allows threat music post cycle for GE
            //new Hook(typeof(RoomRain).GetMethod("get_OutsidePushAround"), (Func<RoomRain, float> orig, RoomRain roomRain) => SandstormPush(roomRain));
            //new Hook(typeof(SlugcatGhost).GetMethod("get_MainColor"), (Func<SlugcatGhost, Color> orig, SlugcatGhost ghost) => SlugGhostColor(ghost));
            new Hook(typeof(SlugcatGhost).GetMethod("get_SecondaryColor"), (Func<SlugcatGhost, Color> orig, SlugcatGhost ghost) => SlugGhostColor(ghost)); // slugcat ghost colors

        }

        private void SlugcatStats_ctor(On.SlugcatStats.orig_ctor orig, SlugcatStats self, SlugcatStats.Name slugcat, bool malnourished)
        {
            orig(self, slugcat, malnourished);
            if (slugcat.value == "Gravelslug")
            {
                self.runspeedFac = 1.2f;
                self.bodyWeightFac = 1.12f;
                self.generalVisibilityBonus = 0.1f;
                self.visualStealthInSneakMode = 0.3f;
                self.loudnessFac = 1.35f;
                self.throwingSkill = 2;
                self.poleClimbSpeedFac = 1.25f;
                self.corridorClimbSpeedFac = 1.2f;
                if (ModManager.Expedition && RWCustom.Custom.rainWorld.ExpeditionMode && ExpeditionGame.activeUnlocks.Contains("unl-agility"))
                {
                    self.lungsFac = 0.15f;
                    self.runspeedFac = 1.75f;
                    self.poleClimbSpeedFac = 1.8f;
                    self.corridorClimbSpeedFac = 1.6f;
                }
                if (malnourished)
                {
                    if (ModManager.Expedition && RWCustom.Custom.rainWorld.ExpeditionMode && ExpeditionGame.activeUnlocks.Contains("unl-agility"))
                    {
                        self.runspeedFac = 1.27f;
                        self.poleClimbSpeedFac = 1.1f;
                        self.corridorClimbSpeedFac = 1.2f;
                        return;
                    }
                }
            }
        }

        private void StoryGameStatisticsScreen_GetDataFromGame1(ILContext il)
        {
            
        }

        private void StoryGameStatisticsScreen_TickerIsDone(On.Menu.StoryGameStatisticsScreen.orig_TickerIsDone orig, Menu.StoryGameStatisticsScreen self, Menu.StoryGameStatisticsScreen.Ticker ticker)
        {
            orig(self, ticker);
            if (ticker.ID == GravelTickerID.GravelQuestFinished)
            {
                self.scoreKeeper.AddScoreAdder(ticker.getToValue, 300);
            }
        }

        private void StoryGameStatisticsScreen_GetDataFromGame(On.Menu.StoryGameStatisticsScreen.orig_GetDataFromGame orig, Menu.StoryGameStatisticsScreen self, Menu.KarmaLadderScreen.SleepDeathScreenDataPackage package)
        {
            orig(self, package);
            if(package.saveState.saveStateNumber.value == "Gravelslug")
            {
                if (package.saveState.deathPersistentSaveData.theMark)
                {
                    Vector2 vector = new Vector2(self.ContinueAndExitButtonsXPos - 160f, 535f);
                    int num = 2;
                    num += package.saveState.miscWorldSaveData.SLOracleState.significantPearls.Count > 0 ? 1 : 0;
                    num += package.saveState.deathPersistentSaveData.friendsSaved > 0 ? 1 : 0;
                    num += package.saveState.miscWorldSaveData.SSaiConversationsHad > 0 ? 1 : 0;
                    num += package.saveState.miscWorldSaveData.SLOracleState.playerEncounters > 0 ? 1 : 0;

                    Menu.StoryGameStatisticsScreen.Popper item = new Menu.StoryGameStatisticsScreen.Popper(self, self.pages[0], vector + new Vector2(0f, -30f * (float)num), "< " + self.Translate("Balance achieved") + ">", GravelTickerID.GravelQuestFinished);
                    self.allTickers.Add(item);
                    self.pages[0].subObjects.Add(item);
                }

            }
        }

        private void TickerID_UnregisterValues(On.MoreSlugcats.MoreSlugcatsEnums.TickerID.orig_UnregisterValues orig)
        {
            orig();
            GravelTickerID.UnregisterValues();
        }

        private void TickerID_RegisterValues(On.MoreSlugcats.MoreSlugcatsEnums.TickerID.orig_RegisterValues orig)
        {
            orig();
            GravelTickerID.RegisterValues();
        }

        private RWCustom.IntVector2 SlugcatStats_SlugcatFoodMeter(On.SlugcatStats.orig_SlugcatFoodMeter orig, SlugcatStats.Name slugcat)
        {
            RWCustom.IntVector2 result = orig.Invoke(slugcat);
            if (slugcat.value == "Gravelslug")
            {
                //min
                result.y = 8;
                //max
                result.x = 8;
            }
            return result;
        }

        private void ProcessManager_Update(On.ProcessManager.orig_Update orig, ProcessManager self, float deltaTime)
        {
            orig(self, deltaTime);
            if (ModManager.MSC && self.currentMainLoop != null && self.rainWorld.progression != null && self.rainWorld.progression.miscProgressionData.currentlySelectedSinglePlayerSlugcat.value == "Gravelslug")
            {
                AudioSource[] array = UnityEngine.Object.FindObjectsOfType<AudioSource>();
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].clip != null && array[i].pitch != 0.5f && (array[i].clip.name.StartsWith("RW_") || array[i].clip.name.StartsWith("NA_")) && self.currentMainLoop.ID == MoreSlugcatsEnums.ProcessID.DatingSim)
                    {
                        array[i].pitch = 0.5f;
                    }
                }
            }
        }

        private void SlideShow_ctor(On.Menu.SlideShow.orig_ctor orig, Menu.SlideShow self, ProcessManager manager, Menu.SlideShow.SlideShowID slideShowID)
        {
            orig(self, manager, slideShowID);
            if(slideShowID == GravelSlideshowID.GravelOutro)
            {
                if(manager.oldProcess.ID == ProcessManager.ProcessID.Game && (manager.oldProcess as RainWorldGame).GetStorySession.saveState.deathPersistentSaveData.theMark)
                {

                }
                else
                {

                }
            }else if (slideShowID == GravelSlideshowID.GravelAltEnd)
            {
                if (manager.oldProcess.ID == ProcessManager.ProcessID.Game && (manager.oldProcess as RainWorldGame).GetStorySession.saveState.deathPersistentSaveData.theMark)
                {

                }
                else
                {

                }
            }
        }

        private void SlideShowID_UnregisterValues(On.MoreSlugcats.MoreSlugcatsEnums.SlideShowID.orig_UnregisterValues orig)
        {
            orig();
            GravelSlideshowID.UnregisterValues();
        }

        private void SlideShowID_RegisterValues(On.MoreSlugcats.MoreSlugcatsEnums.SlideShowID.orig_RegisterValues orig)
        {
            orig();
            GravelSlideshowID.RegisterValues();
        }

        private void DatingSim_InitNextFile(On.MoreSlugcats.DatingSim.orig_InitNextFile orig, DatingSim self, string filename)
        {
            orig(self, filename);
            if (filename == "gravelstart.txt")
            {
                self.slugcat.pos.y = self.manager.rainWorld.options.ScreenSize.y * 0.7f - 20f;
            }
        }

        private void RainCycle_Update(On.RainCycle.orig_Update orig, RainCycle self)
        {
            orig(self);
            Room room = self.world.game.cameras[0].room;
            if(room == null)
            {
                return;
            }
            if (!GravelOptionsMenu.DisableTimer.Value && !room.game.IsArenaSession && !(room.IsGateRoom() && (room.regionGate.mode == RegionGate.Mode.ClosingAirLock || room.regionGate.mode == RegionGate.Mode.Waiting || room.regionGate.mode == RegionGate.Mode.ClosingMiddle)) && room.abstractRoom.shelter == false && room.abstractRoom.name != "HR_FINAL" && room.abstractRoom.name != "HR_AI" && room.abstractRoom.name != "SB_A06GRAV" && !room.game.devToolsActive)
            {
                GravelDissolveUpdate(self.world.game);
            }
        }

        private Color PlayerGraphics_JollyFaceColorMenu(On.PlayerGraphics.orig_JollyFaceColorMenu orig, SlugcatStats.Name slugName, SlugcatStats.Name reference, int playerNumber)
        {
            if (slugName.value == "Gravelslug")
            {
                if (RWCustom.Custom.rainWorld.options.jollyColorMode == Options.JollyColorMode.DEFAULT || (playerNumber == 0 && RWCustom.Custom.rainWorld.options.jollyColorMode != Options.JollyColorMode.CUSTOM))
                {
                    return new Color(0.7098f, 0.80784f, 0.78823f);
                }
            }
            return orig.Invoke(slugName, reference, playerNumber);
        }

        private void SleepAndDeathScreen_GetDataFromGame(On.Menu.SleepAndDeathScreen.orig_GetDataFromGame orig, Menu.SleepAndDeathScreen self, Menu.KarmaLadderScreen.SleepDeathScreenDataPackage package)
        {
            orig(self, package);
            if (package.characterStats.name.value == "Gravelslug" && self.IsSleepScreen)
            {
                if (package.saveState.denPosition.Contains("CL_") || package.saveState.denPosition.Contains("HR_"))
                {
                    return;
                }
                if (package.saveState.denPosition.Contains("MS_") || package.saveState.denPosition.Contains("SI_"))
                {
                    if (self.soundLoop != null)
                    {
                        self.soundLoop.Destroy();
                    }
                    self.mySoundLoopID = MoreSlugcatsEnums.MSCSoundID.Sleep_Blizzard_Loop;
                }
                else
                {
                    if (self.soundLoop != null)
                    {
                        self.soundLoop.Destroy();
                    }
                    self.mySoundLoopID = SoundID.MENU_Main_Menu_LOOP;
                }
            }
        }

        private void DatingSim_ctor(On.MoreSlugcats.DatingSim.orig_ctor orig, DatingSim self, ProcessManager manager)
        {
            orig(self, manager);
            if (manager.oldProcess.ID == ProcessManager.ProcessID.Game && (manager.oldProcess as RainWorldGame).session.characterStats.name.value == "Gravelslug")
            {
                self.InitNextFile("gravelstart.txt");
            }
        }

        private void Player_EatMeatUpdate(On.Player.orig_EatMeatUpdate orig, Player self, int graspIndex)
        {
            orig(self, graspIndex);
            if(self.SlugCatClass.value == "Gravelslug" && self.room.game.IsStorySession)
            {
                if (self.eatMeat > 40 && self.eatMeat % 15 == 3 && self.grasps[graspIndex].grabbed != null && (self.grasps[graspIndex].grabbed as Creature).State.meatLeft > 0 && ((self.FoodInStomach < self.MaxFoodInStomach) || GravelFatty))
                {
                    GravelDissolveAdd(10f);
                }
            }
        }

        private void GravelCough(Creature player, bool big)
        {
            if (player is Player && (player as Player).lungsExhausted)
            {
                return;
            }
            if (!big && !(ModManager.Expedition && player.room.game.rainWorld.ExpeditionMode) && player.room.game.IsStorySession && player.room.game.GetStorySession.characterStats.name.value == "Gravelslug" && player.room.game.GetStorySession.saveState.cycleNumber != 0 && player.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW] < -4)
            {
                if (UnityEngine.Random.value <= 0.25 * (-player.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW] - 4))
                {
                    return;
                }
            }
            bool flag = false;
            bool isplr = false;
            Color color = new Color(0f, 1f, 1f);
            if (player.Submersion == 1f)
            {
                flag = true;
                player.room.AddObject(new Bubble(player.firstChunk.pos, player.firstChunk.vel + RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f) * (big ? 8f : 6f), false, false));
            }
            if (player is Player)
            {
                isplr = true;
                color = GravelFireColor(player as Player);
                Fire_Breath(player as Player, true, true);
            }
            if (isplr && !player.room.game.IsArenaSession)
            {
                GravelDissolveSubtract(big ? 3.2f : 0.08f, player.room.game, true);
            }
            player.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.Throw_FireSpear, player.firstChunk.pos, big ? 1f: 0.8f, UnityEngine.Random.Range(big ? 0.8f : 1.2f, big ? 1.2f : 1.6f));
            player.room.AddObject(new Explosion.ExplosionLight(player.firstChunk.pos, big ? 280f : 80f, 1f, 7, Color.white));
            player.room.AddObject(new ExplosionSpikes(player.room, player.firstChunk.pos, big ? 14 : 7, big ? 15f : 10f, 9f, big ? 5f : 4f, big ? 90f :45f, color));
            //player.room.InGameNoise(new Noise.InGameNoise(player.firstChunk.pos, big ? 6000f : 4500f, player, 1f));
            float num = big ? 20f : 10f * (float)(player as Player).playerState.permanentDamageTracking;
            for (int m = 0; m < player.room.physicalObjects.Length; m++)
            {
                foreach (PhysicalObject physicalObject in player.room.physicalObjects[m])
                {
                    if (physicalObject != player && !(physicalObject is Weapon && (physicalObject as Weapon).thrownBy == player && (physicalObject as Weapon).mode == Weapon.Mode.Thrown && physicalObject is not ScavengerBomb))
                    {
                        foreach (BodyChunk bodyChunk in physicalObject.bodyChunks)
                        {
                            float num2 = 1f + bodyChunk.submersion * player.firstChunk.submersion * 4.5f;
                            if (RWCustom.Custom.DistLess(bodyChunk.pos, player.firstChunk.pos, num * num2 + bodyChunk.rad + player.firstChunk.rad) && player.room.VisualContact(bodyChunk.pos, player.firstChunk.pos))
                            {
                                float num3 = Mathf.InverseLerp(num * num2 + bodyChunk.rad + player.firstChunk.rad, (num * num2 + bodyChunk.rad + player.firstChunk.rad) / 2f, Vector2.Distance(bodyChunk.pos, player.firstChunk.pos));
                                bodyChunk.vel += RWCustom.Custom.DirVec(player.firstChunk.pos + new Vector2(0f, player.IsTileSolid(1, 0, -1) ? -20f : 0f), bodyChunk.pos) * num3 * num2 * 3f / bodyChunk.mass;
                                if (ModManager.MSC && physicalObject is Player && (physicalObject as Player).SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Saint)
                                {
                                    (physicalObject as Player).SaintStagger(big ? 300 : 80);
                                }
                                else if (physicalObject is Player)
                                {
                                    (physicalObject as Player).Stun((int)(10f * num3));
                                }
                                else if (physicalObject is Creature)
                                {
                                    if (physicalObject is TubeWorm)
                                    {
                                        for (int i = 0; i < (physicalObject as TubeWorm).grabbedBy.Count; i++)
                                        {
                                            if ((physicalObject as TubeWorm).grabbedBy[i].grabber is Player)
                                            {
                                                return;
                                            }
                                        }
                                    }
                                    (physicalObject as Creature).Stun((int)(((ModManager.MMF && MMF.cfgIncreaseStuns.Value) ? 100f : 10f) * num3));
                                }
                                if (physicalObject is Leech)
                                {
                                    if ((UnityEngine.Random.value < 0.033333335f || !flag) || RWCustom.Custom.DistLess(player.firstChunk.pos, bodyChunk.pos, player.firstChunk.rad + bodyChunk.rad + 2f))
                                    {
                                        (physicalObject as Leech).Die();
                                    }
                                    else
                                    {
                                        (physicalObject as Leech).Stun((int)(num3 * bodyChunk.submersion * Mathf.Lerp(800f, 900f, UnityEngine.Random.value)));
                                    }
                                }
                                if (!flag)
                                {
                                    if (physicalObject is ScavengerBomb && (UnityEngine.Random.value < 0.35f || big))
                                    {
                                        //(physicalObject as ScavengerBomb).ignited = true;
                                        (physicalObject as ScavengerBomb).InitiateBurn();
                                    }
                                    if (physicalObject is ExplosiveSpear && (UnityEngine.Random.value < 0.35f || big))
                                    {
                                        (physicalObject as ExplosiveSpear).Ignite();
                                    }
                                    if (physicalObject is Spear && (physicalObject as Spear).IsNeedle && (UnityEngine.Random.value < 0.35f || big))
                                    {
                                        physicalObject.room.AddObject(new ExplosiveSpear.SpearFragment(physicalObject.firstChunk.pos, physicalObject.firstChunk.vel));
                                        physicalObject.room.AddObject(new Smolder(physicalObject.room, physicalObject.firstChunk.pos, physicalObject.firstChunk, null));
                                        (physicalObject as Spear).Destroy();
                                    }
                                    if (physicalObject is FirecrackerPlant && (UnityEngine.Random.value < 0.35f || big))
                                    {
                                        (physicalObject as FirecrackerPlant).Ignite();
                                    }
                                    if (physicalObject is SporePlant && (UnityEngine.Random.value < 0.35f || big))
                                    {
                                        (physicalObject as SporePlant).BeeTrigger();
                                    }
                                    if (physicalObject is SporePlant.AttachedBee)
                                    {
                                        (physicalObject as SporePlant.AttachedBee).BreakStinger();
                                    }
                                    if (physicalObject is Spider || physicalObject is Fly)
                                    {
                                        if (RWCustom.Custom.DistLess(player.firstChunk.pos, bodyChunk.pos, player.firstChunk.rad + bodyChunk.rad + 2f))
                                        {
                                            (physicalObject as Creature).Die();
                                        }
                                    }
                                    if (physicalObject is BubbleGrass)
                                    {
                                        (physicalObject as BubbleGrass).AbstrBubbleGrass.oxygenLeft -= big ? 0.4f : 0.04f;
                                        physicalObject.room.AddObject(new Smolder(physicalObject.room, physicalObject.firstChunk.pos, physicalObject.firstChunk, null));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool SLOracleBehaviorHasMark_WillingToInspectItem(On.SLOracleBehaviorHasMark.orig_WillingToInspectItem orig, SLOracleBehaviorHasMark self, PhysicalObject item)
        {
            if(item is Oracle)
            {
                return false;
            }
            return orig(self, item);
        }

        private SLOracleBehaviorHasMark.MiscItemType SLOracleBehaviorHasMark_TypeOfMiscItem(On.SLOracleBehaviorHasMark.orig_TypeOfMiscItem orig, SLOracleBehaviorHasMark self, PhysicalObject testItem)
        {
            if(self.oracle.room.world.game.GetStorySession.saveStateNumber.value == "Gravelslug")
            {
                if (ModManager.MSC && testItem is Spear && (testItem as Spear).bugSpear)
                {
                    return MoreSlugcatsEnums.MiscItemType.MoonCloak;
                }
            }
            return orig.Invoke(self, testItem);
        }

        private void CharacterSelectPage_UpdateSelectedSlugcat(On.Menu.CharacterSelectPage.orig_UpdateSelectedSlugcat orig, Menu.CharacterSelectPage self, int num)
        {
            orig(self, num);
            SlugcatStats.Name name = ExpeditionGame.playableCharacters[num];
            if (name.value == "Gravelslug")
            {
                self.slugcatScene = MoreSlugcatsEnums.MenuSceneID.Landscape_VS;
            }
        }

        private GravelConfig GravelOptionsMenu;
        public static bool GravelFatty = false;
        public static bool GravelFoodBar = false;
        public static bool GravelVinki = false;

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);

            this.GravelOptionsMenu = new GravelConfig(this);
            try
            {
                MachineConnector.SetRegisteredOI("kingmaxthe2.gravelslug", this.GravelOptionsMenu);
            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("Remix Menu Gravelslug: Hook_OnModsInit options failed init error {0}{1}", this.GravelOptionsMenu, ex));
                base.Logger.LogError(ex);
                base.Logger.LogMessage("WHOOPS");
            }

            for (int i = 0; i < ModManager.ActiveMods.Count; i++)
            {
                if (ModManager.ActiveMods[i].id == "willowwisp.bellyplus")
                {
                    GravelFatty = true;
                }
                if (ModManager.ActiveMods[i].id == "sprobgik.individualfoodbars")
                {
                    GravelFoodBar = true;
                }
                if (ModManager.ActiveMods[i].id == "olaycolay.thevinki")
                {
                    GravelVinki = true;
                }
            }
        }

        private void ShelterDoor_Update(On.ShelterDoor.orig_Update orig, ShelterDoor self, bool eu)
        {
            float num = self.closedFac;
            orig(self, eu);
            if(self.room.game.StoryCharacter.value == "Gravelslug")
            {
                if (num >= 0.04f && self.closedFac < 0.04f)
                {
                    //TummyItem(player);
                    if (self.room.game.IsStorySession && (!ModManager.Expedition || (ModManager.Expedition && !self.room.game.rainWorld.ExpeditionMode)) && self.room.world.region != null && (CheckForRegionSlugcatGhost(self.room.world.region.name) != GhostWorldPresence.GhostID.NoGhost) || self.room.world.region.name == "MS")
                    {
                        GhostWorldPresence.GhostID ghostID = CheckForRegionSlugcatGhost(self.room.world.region.name);
                        if (!(self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ghostsTalkedTo.ContainsKey(ghostID) || (self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ghostsTalkedTo[ghostID] < 1 || self.room.world.region.name == "MS")
                        {
                            //self.room.AddObject(new GhostPing(self.room));
                            self.room.AddObject(new SlugcatGhostPing(self.room));
                        }
                    }
                }
            }
        }

        private static GhostWorldPresence.GhostID CheckForRegionSlugcatGhost(string region)
        {
            if (region != null)
            {
                if(region == "SB")
                {
                    return GravelGhostID.GEGour;
                } else if (region == "CC")
                {
                    return GravelGhostID.GESaint;
                }
                else if (region == "SI")
                {
                    return GravelGhostID.GESpear;
                }
                else if (region == "GW")
                {
                    return GravelGhostID.GEArti;
                }
                else if (region == "OE")
                {
                    return GravelGhostID.GEYellow;
                }
                else if (region == "SU")
                {
                    return GravelGhostID.GEWhite;
                }
                else if (region == "DS")
                {
                    return GravelGhostID.GERiv;
                }
                else if (region == "LF")
                {
                    return GravelGhostID.GERed;
                }
            }
            return GhostWorldPresence.GhostID.NoGhost;
        }

        /*private Color CreatureSymbol_ColorOfCreature(On.CreatureSymbol.orig_ColorOfCreature orig, IconSymbol.IconSymbolData iconData)
        {
            if (iconData.critType == MoreSlugcatsEnums.CreatureTemplateType.HunterDaddy)
            {
                return Color.Lerp(PlayerGraphics.DefaultSlugcatColor("Gravelslug"), Color.gray, 0.4f);
            }
            return orig.Invoke(iconData);
        }*/

        private void Player_MaulingUpdate(On.Player.orig_MaulingUpdate orig, Player self, int graspIndex)
        {
            orig(self, graspIndex);
            if (self.grasps[graspIndex] == null || (self.grasps[graspIndex].grabbed is not Oracle))
            {
                return;
            }
            if (self.maulTimer > 15 && self.grasps[graspIndex].grabbed is Oracle)
            {
                self.standing = false;
                self.Blink(5);
                if (self.maulTimer % 3 == 0)
                {
                    Vector2 b = RWCustom.Custom.RNV() * 3f;
                    self.mainBodyChunk.pos += b;
                    self.mainBodyChunk.vel += b;
                }
                Vector2 vector = self.grasps[graspIndex].grabbedChunk.pos * self.grasps[graspIndex].grabbedChunk.mass;
                float num = self.grasps[graspIndex].grabbedChunk.mass;
                for (int i = 0; i < self.grasps[graspIndex].grabbed.bodyChunkConnections.Length; i++)
                {
                    if (self.grasps[graspIndex].grabbed.bodyChunkConnections[i].chunk1 == self.grasps[graspIndex].grabbedChunk)
                    {
                        vector += self.grasps[graspIndex].grabbed.bodyChunkConnections[i].chunk2.pos * self.grasps[graspIndex].grabbed.bodyChunkConnections[i].chunk2.mass;
                        num += self.grasps[graspIndex].grabbed.bodyChunkConnections[i].chunk2.mass;
                    }
                    else if (self.grasps[graspIndex].grabbed.bodyChunkConnections[i].chunk2 == self.grasps[graspIndex].grabbedChunk)
                    {
                        vector += self.grasps[graspIndex].grabbed.bodyChunkConnections[i].chunk1.pos * self.grasps[graspIndex].grabbed.bodyChunkConnections[i].chunk1.mass;
                        num += self.grasps[graspIndex].grabbed.bodyChunkConnections[i].chunk1.mass;
                    }
                }
                vector /= num;
                self.mainBodyChunk.vel += RWCustom.Custom.DirVec(self.mainBodyChunk.pos, vector) * 0.5f;
                self.bodyChunks[1].vel -= RWCustom.Custom.DirVec(self.mainBodyChunk.pos, vector) * 0.6f;
                if (self.graphicsModule != null)
                {
                    if (!RWCustom.Custom.DistLess(self.grasps[graspIndex].grabbedChunk.pos, (self.graphicsModule as PlayerGraphics).head.pos, self.grasps[graspIndex].grabbedChunk.rad))
                    {
                        (self.graphicsModule as PlayerGraphics).head.vel += RWCustom.Custom.DirVec(self.grasps[graspIndex].grabbedChunk.pos, (self.graphicsModule as PlayerGraphics).head.pos) * (self.grasps[graspIndex].grabbedChunk.rad - Vector2.Distance(self.grasps[graspIndex].grabbedChunk.pos, (self.graphicsModule as PlayerGraphics).head.pos));
                    }
                    else if (self.maulTimer % 5 == 3)
                    {
                        (self.graphicsModule as PlayerGraphics).head.vel += RWCustom.Custom.RNV() * 4f;
                    }
                    if (self.maulTimer > 10 && self.maulTimer % 8 == 3)
                    {
                        self.mainBodyChunk.pos += RWCustom.Custom.DegToVec(Mathf.Lerp(-90f, 90f, UnityEngine.Random.value)) * 4f;
                        self.grasps[graspIndex].grabbedChunk.vel += RWCustom.Custom.DirVec(vector, self.mainBodyChunk.pos) * 0.9f / self.grasps[graspIndex].grabbedChunk.mass;
                        for (int j = UnityEngine.Random.Range(0, 3); j >= 0; j--)
                        {
                            self.room.AddObject(new WaterDrip(Vector2.Lerp(self.grasps[graspIndex].grabbedChunk.pos, self.mainBodyChunk.pos, UnityEngine.Random.value) + self.grasps[graspIndex].grabbedChunk.rad * RWCustom.Custom.RNV() * UnityEngine.Random.value, RWCustom.Custom.RNV() * 6f * UnityEngine.Random.value + RWCustom.Custom.DirVec(vector, (self.mainBodyChunk.pos + (self.graphicsModule as PlayerGraphics).head.pos) / 2f) * 7f * UnityEngine.Random.value + RWCustom.Custom.DegToVec(Mathf.Lerp(-90f, 90f, UnityEngine.Random.value)) * UnityEngine.Random.value * self.EffectiveRoomGravity * 7f, false));
                        }
                        return;
                    }
                }
            }
        }

        private Player.ObjectGrabability Player_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {
            if (obj is Oracle)
            {
                if((obj as Oracle).ID == Oracle.OracleID.SL)
                {
                    if(!RWCustom.Custom.DistLess(self.bodyChunks[1].pos, new Vector3(1555,145), 475f))
                    {
                        self.ReleaseGrasp(0);
                    }
                    return Player.ObjectGrabability.TwoHands;
                }
                if ((obj as Oracle).ID == MoreSlugcatsEnums.OracleID.CL)
                {
                    if (!RWCustom.Custom.DistLess(self.bodyChunks[1].pos, new Vector3(2555, 155), 150f))
                    {
                        self.ReleaseGrasp(0);
                    }
                    return Player.ObjectGrabability.TwoHands;
                }
                return Player.ObjectGrabability.CantGrab;
            }
            return orig.Invoke(self, obj);
        }

        private bool TubeWorm_JumpButton(On.TubeWorm.orig_JumpButton orig, TubeWorm self, Player plr)
        {
            orig(self, plr);
            if(plr.SlugCatClass.value == "Gravelslug" && IsGravelFeral(plr) && self.tongues[0].Attached && plr.canJump < 1 && plr.bodyMode == Player.BodyModeIndex.Default)
            {
                if(plr.bodyChunks[0].vel.y < 0 && plr.bodyChunks[1].vel.y < 0)
                {
                    plr.bodyChunks[0].vel.y *= -1;
                    plr.bodyChunks[1].vel.y *= -1;
                }
                plr.bodyChunks[0].vel.y += 8f;
                plr.bodyChunks[1].vel.y += 7f;
                self.mainBodyChunk.vel = self.mainBodyChunk.vel * 1.6f;
                //plr.animation = Player.AnimationIndex.Flip;
                GravelCough(plr, true);
                return false;
            }
            return orig.Invoke(self, plr);
        }

        private void World_SpawnGhost(On.World.orig_SpawnGhost orig, World self)
        {
            orig(self);
            if(self.game.StoryCharacter.value == "Gravelslug" && self.worldGhost == null)
            {
                if (self.game.setupValues.ghosts < 0)
                {
                    return;
                }
                if (!World.CheckForRegionGhost((self.game.session as StoryGameSession).saveStateNumber, self.region.name))
                {
                    return;
                }
                GhostWorldPresence.GhostID ghostID = GhostWorldPresence.GetGhostID(self.region.name);
                if (ghostID == GhostWorldPresence.GhostID.NoGhost)
                {
                    return;
                }
                int ghostPreviouslyEncountered = 0;
                if ((self.game.session as StoryGameSession).saveState.deathPersistentSaveData.ghostsTalkedTo.ContainsKey(ghostID))
                {
                    ghostPreviouslyEncountered = (self.game.session as StoryGameSession).saveState.deathPersistentSaveData.ghostsTalkedTo[ghostID];
                }
                Debug.Log("Save state properly loaded: " + (self.game.session as StoryGameSession).saveState.loaded.ToString());
                Debug.Log("GHOST TALKED TO " + ghostID.ToString() + " " + ghostPreviouslyEncountered.ToString());
                Debug.Log("Karma: " + (self.game.session as StoryGameSession).saveState.deathPersistentSaveData.karma.ToString());
                bool flag = self.game.setupValues.ghosts > 0 || GhostWorldPresence.SpawnGhost(ghostID, (self.game.session as StoryGameSession).saveState.deathPersistentSaveData.karma, (self.game.session as StoryGameSession).saveState.deathPersistentSaveData.karmaCap, ghostPreviouslyEncountered, self.game.StoryCharacter == SlugcatStats.Name.Red);
                if (ModManager.MSC && (!ModManager.Expedition || (ModManager.Expedition && !self.game.rainWorld.ExpeditionMode)))
                {
                    if (!flag)
                    {
                        if (!(self.game.session as StoryGameSession).saveState.deathPersistentSaveData.ghostsTalkedTo.ContainsKey(ghostID) || (self.game.session as StoryGameSession).saveState.deathPersistentSaveData.ghostsTalkedTo[ghostID] != 1)
                        {
                            flag = false;
                        }
                        else if (((self.game.session as StoryGameSession).saveState.deathPersistentSaveData.karma == (self.game.session as StoryGameSession).saveState.deathPersistentSaveData.karmaCap && (self.game.session as StoryGameSession).saveState.deathPersistentSaveData.reinforcedKarma && ghostID != MoreSlugcatsEnums.GhostID.CL) || (ModManager.Expedition && self.game.rainWorld.ExpeditionMode && (self.game.session as StoryGameSession).saveState.deathPersistentSaveData.karma == (self.game.session as StoryGameSession).saveState.deathPersistentSaveData.karmaCap))
                        {
                            flag = true;
                        }
                    }
                }
                if (self.game.rainWorld.safariMode)
                {
                    flag = false;
                }
                if (flag)
                {
                    self.worldGhost = new GhostWorldPresence(self, ghostID);
                    self.migrationInfluence = self.worldGhost;
                    Debug.Log("Ghost in region");
                    return;
                }
                Debug.Log("No ghost in region");
            }
        }

        private bool PearlDeliveryChallenge_ValidForThisSlugcat(On.Expedition.PearlDeliveryChallenge.orig_ValidForThisSlugcat orig, Expedition.PearlDeliveryChallenge self, SlugcatStats.Name slugcat)
        {
            if(slugcat.value == "Gravelslug")
            {
                return false;
            }
            return orig(self, slugcat);
        }

        private void SlugcatPageContinue_ctor(On.Menu.SlugcatSelectMenu.SlugcatPageContinue.orig_ctor orig, Menu.SlugcatSelectMenu.SlugcatPageContinue self, Menu.Menu menu, Menu.MenuObject owner, int pageIndex, SlugcatStats.Name slugcatNumber)
        {
            orig(self, menu, owner, pageIndex, slugcatNumber);
            if(slugcatNumber.value == "Gravelslug")
            {
                //self.CurrentFood;
                
            }
        }

        private void SleepAndDeathScreen_AddPassageButton(On.Menu.SleepAndDeathScreen.orig_AddPassageButton orig, Menu.SleepAndDeathScreen self, bool buttonBlack)
        {
            if (self.saveState != null && self.saveState.saveStateNumber.value == "Gravelslug")
            {
                return;
            }
            else
            {
                orig(self, buttonBlack);
            }
        }

        private void CLOracleBehavior_TalkToDeadPlayer(On.MoreSlugcats.CLOracleBehavior.orig_TalkToDeadPlayer orig, CLOracleBehavior self)
        {
            if (self.oracle.room.game.StoryCharacter.value == "Gravelslug")
            {
                if (!self.deadTalk && self.oracle.room.ViewedByAnyCamera(self.oracle.firstChunk.pos, 0f))
                {
                    if (self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.halcyonStolen || self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiThrowOuts > 0)
                    {
                        self.dialogBox.Interrupt(self.Translate("..."), 60);
                        return;
                    }
                    float value = UnityEngine.Random.value;
                    if (value <= 0.33f)
                    {
                        self.dialogBox.Interrupt(self.Translate("Little Experiment?"), 60);
                    }
                    else if (value <= 0.67f)
                    {
                        self.dialogBox.Interrupt(self.Translate("Alone again..."), 60);
                    }
                    else
                    {
                        self.dialogBox.Interrupt(self.Translate("Oh..."), 60);
                    }
                    self.deadTalk = true;
                }
                return;
            }
            orig(self);
        }

        private void CLOracleBehavior_InterruptRain(On.MoreSlugcats.CLOracleBehavior.orig_InterruptRain orig, CLOracleBehavior self)
        {
            if (self.oracle.room.game.StoryCharacter.value == "Gravelslug") 
            {
                if (self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.halcyonStolen || self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiThrowOuts > 0)
                {
                    return;
                }
                if (UnityEngine.Random.value < 0.15f)
                {
                    self.dialogBox.Interrupt(self.Translate("You should leave quickly before the rain crushes you."), 60);
                    return;
                }
                if (UnityEngine.Random.value < 0.15f)
                {
                    self.dialogBox.Interrupt(self.Translate("Quickly, find a shelter nearby. It will protect you from my rushing waters."), 60);
                    return;
                }
                self.dialogBox.Interrupt(self.Translate("Please find some safety for yourself. I want to see you again."), 60);
                return;
            }
            orig(self);
        }

        private bool SlugcatSelectMenu_CheckJollyCoopAvailable(On.Menu.SlugcatSelectMenu.orig_CheckJollyCoopAvailable orig, Menu.SlugcatSelectMenu self, SlugcatStats.Name slugcat)
        {
            if(slugcat.value == "Gravelslug")
            {
                return self.forceActivateMSCJolly;
            }
            return orig(self, slugcat);
        }

        private void Creature_Violence(On.Creature.orig_Violence orig, Creature self, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus)
        {
            if(self is Player && (self as Player).SlugCatClass.value == "Gravelslug")
            {
                if(!IsGravelFeral(self as Player))
                {
                    if(damage < 1.2)
                    {
                        bool fatal = false;
                        if (damage >= 1)
                        {
                            fatal = true;
                            GravelRetaliate(self as Player);
                        }
                        damage *= .4f;
                        if (fatal)
                        {
                            (self as Player).playerState.permanentDamageTracking += damage;
                        }
                        if((self as Player).playerState.permanentDamageTracking >= 1f)
                        {
                            (self as Player).Die();
                        }
                    }
                }
            }
            orig(self, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
        }

        private Color PlayerGraphics_DefaultSlugcatColor(On.PlayerGraphics.orig_DefaultSlugcatColor orig, SlugcatStats.Name i)
        {
            if(i.value == "Gravelslug")
            {
                return new Color(0.03921f, 0.7647f, 0.6549f);
            }
            else
            {
                return orig.Invoke(i);
            }
        }

        private void Player_SetMalnourished(On.Player.orig_SetMalnourished orig, Player self, bool m)
        {
            orig(self, m);
            if(self.SlugCatClass.value == "Gravelslug" && m)
            {
                if(self.room.game.cameras[0].hud != null && self.room.game.cameras[0].hud.foodMeter.survivalLimit != self.slugcatStats.foodToHibernate)
                {
                    FoodMeter foodbar = self.room.game.cameras[0].hud.foodMeter;
                    foodbar.survivalLimit = self.slugcatStats.foodToHibernate;
                    foodbar.MoveSurvivalLimit(foodbar.maxFood, true);
                    //self.room.game.cameras[0].hud.foodMeter = new FoodMeter(foodbar.hud, self.slugcatStats.maxFood, self.slugcatStats.foodToHibernate);
                }
            }
        }

        public int GravelQuestProgress(RainWorldGame game)
        {
            if(game.IsStorySession && !(ModManager.Expedition && game.rainWorld.ExpeditionMode) && game.GetStorySession.characterStats.name.value == "Gravelslug")
            {
                DeathPersistentSaveData e = game.GetStorySession.saveState.deathPersistentSaveData;
                return e.ghostsTalkedTo[GravelGhostID.GEWhite] + e.ghostsTalkedTo[GravelGhostID.GEYellow] + 
                    e.ghostsTalkedTo[GravelGhostID.GERed] + e.ghostsTalkedTo[GravelGhostID.GESaint] + 
                    e.ghostsTalkedTo[GravelGhostID.GERiv] + e.ghostsTalkedTo[GravelGhostID.GEArti] + 
                    e.ghostsTalkedTo[GravelGhostID.GEGour] + e.ghostsTalkedTo[GravelGhostID.GESpear];
            }
            return 0;
        }

        private bool GravelHasAbilities(Player player) => player.room.game.IsArenaSession || (player.room.game.IsStorySession && IsGravelFeral(player) && !GravelOptionsMenu.KeepAbilities.Value);

        private static bool IsGravelFeral(Player player)
        {
            //return player.SlugCatClass.value == "Gravelslug" && player.Malnourished;
            if (player.SlugCatClass.value == "Gravelslug")
            {
                if(player.room != null && player.room.game.IsStorySession)
                {
                    return player.Malnourished || (player.room.world.region != null && player.room.world.name == "HR");
                }
                else if(player.room != null && player.room.game.IsArenaSession)
                {
                    return player.playerState.permanentDamageTracking >= 0.05;
                }
                return false;
            }
            return false;
            //return player.SlugCatClass.value == "Gravelslug" && (player.Malnourished || (player.playerState.permanentDamageTracking >= 0.05 && player.room.game.IsArenaSession) || (player.room.game.IsStorySession && player.room.world.region != null && player.room.world.name == "HR"));
        }
        private void GravelRetaliate(Player player)
        {
            if(player.playerState.permanentDamageTracking <= 0)
            {
                Vector2 pos = player.firstChunk.pos;
                //Color color = (player.graphicsModule as PlayerGraphics).CharacterForColor
                player.room.AddObject(new Explosion(player.room, player, pos, 5, 200f, 10f, 0.25f, 60f, 0.3f, player, 0.8f, 0f, 0.7f));
                for (int i = 0; i < 14; i++)
                {
                    player.room.AddObject(new Explosion.ExplosionSmoke(pos, RWCustom.Custom.RNV() * 5f * UnityEngine.Random.value, 1f));
                }
                player.room.AddObject(new Explosion.ExplosionLight(pos, 160f, 1f, 3, Color.white));
                player.room.AddObject(new ShockWave(pos, 300f, 0.165f, 4, false));
                for (int j = 0; j < 20; j++)
                {
                    Vector2 a = RWCustom.Custom.RNV();
                    player.room.AddObject(new Spark(pos + a * UnityEngine.Random.value * 40f, a * Mathf.Lerp(4f, 30f, UnityEngine.Random.value), Color.white, null, 4, 18));
                }
                player.room.ScreenMovement(new Vector2?(pos), default(Vector2), 0.7f);
                for (int k = 0; k < player.abstractPhysicalObject.stuckObjects.Count; k++)
                {
                    player.abstractPhysicalObject.stuckObjects[k].Deactivate();
                }
                player.room.PlaySound(SoundID.Fire_Spear_Explode, pos);
                //player.room.InGameNoise(new Noise.InGameNoise(pos, 8000f, player, 1f));
                player.Stun(80);
                for (int i = 0; i < player.room.abstractRoom.creatures.Count; i++)
                {
                    if (player.room.abstractRoom.creatures[i].realizedCreature != null && (player.room.abstractRoom.creatures[i].creatureTemplate.type == CreatureTemplate.Type.BrotherLongLegs || player.room.abstractRoom.creatures[i].creatureTemplate.type == CreatureTemplate.Type.DaddyLongLegs || player.room.abstractRoom.creatures[i].creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.TerrorLongLegs))
                    {
                        player.room.abstractRoom.creatures[i].realizedCreature.stun = Math.Max(player.room.abstractRoom.creatures[i].realizedCreature.stun, 100 + (int)(UnityEngine.Random.value * 200));
                    }
                }
                if (player.room.game.IsStorySession)
                {
                    player.SetMalnourished(true);
                }
            }
            player.playerState.permanentDamageTracking += 0.1f;
        }

        private int SlugcatStats_NourishmentOfObjectEaten(On.SlugcatStats.orig_NourishmentOfObjectEaten orig, SlugcatStats.Name slugcatIndex, IPlayerEdible eatenobject)
        {
            if(slugcatIndex.value == "Gravelslug" && !(ModManager.Expedition && (eatenobject as PhysicalObject).room.game.rainWorld.ExpeditionMode) && !(eatenobject as PhysicalObject).room.game.IsArenaSession && (eatenobject as PhysicalObject).room.game.GetStorySession.saveState.cycleNumber != 0 && (eatenobject as PhysicalObject).room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW] < -4)
            {
                if (eatenobject is not FireEgg){
                    int num = orig.Invoke(slugcatIndex, eatenobject);
                    num *= 2 * (-(eatenobject as PhysicalObject).room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW] - 4);
                    return num;
                }
            }
            return orig.Invoke(slugcatIndex, eatenobject);
        }

        public Color SlugGhostColor(SlugcatGhost self)
        {
            if(self.room.game.IsStorySession && self.room.game.GetStorySession.characterStats.name.value == "Gravelslug")
            {
                name = self.room.abstractRoom.name;
                if (name == "SI_A07")
                {
                    return PlayerGraphics.DefaultSlugcatColor(MoreSlugcatsEnums.SlugcatStatsName.Spear);
                }
                if (name == "LF_H01")
                {
                    return PlayerGraphics.DefaultSlugcatColor(SlugcatStats.Name.Red);
                }
                if (name == "CC_H01SAINT")
                {
                    return PlayerGraphics.DefaultSlugcatColor(MoreSlugcatsEnums.SlugcatStatsName.Saint);
                }
                if (name == "GW_A25")
                {
                    return PlayerGraphics.DefaultSlugcatColor(MoreSlugcatsEnums.SlugcatStatsName.Artificer);
                }
                if (name == "DS_RIVSTART")
                {
                    return PlayerGraphics.DefaultSlugcatColor(MoreSlugcatsEnums.SlugcatStatsName.Rivulet);
                }
                if (name == "OE_CAVE02")
                {
                    return PlayerGraphics.DefaultSlugcatColor(SlugcatStats.Name.Yellow);
                }
                if (name == "SB_GOR02RIV")
                {
                    return PlayerGraphics.DefaultSlugcatColor(MoreSlugcatsEnums.SlugcatStatsName.Gourmand);
                }
                if (name == "SU_A53")
                {
                    return PlayerGraphics.DefaultSlugcatColor(SlugcatStats.Name.White);
                }
                if (name == "MS_COMMS")
                {
                    return PlayerGraphics.DefaultSlugcatColor(MoreSlugcatsEnums.SlugcatStatsName.Sofanthiel);
                }
            }
            return RainWorld.GoldRGB;
        }

        private void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            if (!rCam.room.game.DEBUGMODE)
            {
                if (ModManager.MSC)
                {
                    if (self.player.room != null && self.player.SlugCatClass.value == "Gravelslug")
                    {
                        //sLeaser.sprites[9].color = RainWorld.GoldRGB;
                        Color eyeColor = new Color(0.7098f, 0.80784f, 0.78823f);
                        Color fireColor = GravelFireColor(self.player);
                        if (!self.player.room.game.setupValues.arenaDefaultColors && !ModManager.CoopAvailable)
                        {
                            switch (self.player.playerState.playerNumber)
                            {
                                case 0:
                                    if (self.player.room.game.IsArenaSession && self.player.room.game.GetArenaGameSession.arenaSitting.gameTypeSetup.gameType != MoreSlugcatsEnums.GameTypeID.Challenge)
                                    {
                                        eyeColor = new Color(0.06275f, 0.07529f, 0.07373f);
                                    }
                                    break;
                                case 1:
                                    eyeColor = new Color(0.06275f, 0.07529f, 0.07373f);
                                    break;
                                case 2:
                                    eyeColor = new Color(0.06275f, 0.07529f, 0.07373f);
                                    break;
                                case 3:
                                    eyeColor = new Color(0.7098f, 0.80784f, 0.78823f);
                                    break;
                            }
                        }
                        if ((self.player.graphicsModule as PlayerGraphics).useJollyColor)
                        {
                            eyeColor = PlayerGraphics.JollyColor((self.player).playerState.playerNumber, 1);
                        }
                        if (PlayerGraphics.CustomColorsEnabled()) 
                        {
                            eyeColor = PlayerGraphics.CustomColorSafety(1);
                        }
                        eyeColor = Color.Lerp(eyeColor, fireColor, self.player.dead ? 0 : (float)self.player.playerState.permanentDamageTracking/2 + (float)self.player.playerState.permanentDamageTracking / 4 + (float)self.player.playerState.permanentDamageTracking / 8);
                        sLeaser.sprites[9].color = eyeColor;
                        if (rCam.ghostMode > 0f && self.player.room.world.worldGhost.ghostID != MoreSlugcatsEnums.GhostID.CL)
                        {
                            //sLeaser.sprites[9].color = RainWorld.GoldRGB;
                            sLeaser.sprites[9].color = Color.Lerp(eyeColor, RainWorld.GoldRGB, rCam.ghostMode);
                        }
                        /*if (self.player.sceneFlag && self.player.room != null && self.player.room.abstractRoom.name == "CL_GRAVEL")
                        {
                            rCam.ghostMode = 1f;
                        }*/
                    }
                }
            }
        }

        /*private void Tutorial_UnregisterValues(On.MoreSlugcats.MoreSlugcatsEnums.Tutorial.orig_UnregisterValues orig)
        {
            orig();
            GravelTutorial.UnregisterValues();
        }

        private void Tutorial_RegisterValues(On.MoreSlugcats.MoreSlugcatsEnums.Tutorial.orig_RegisterValues orig)
        {
            orig();
            GravelTutorial.RegisterValues();
        }*/

        private List<string> PlayerGraphics_DefaultBodyPartColorHex(On.PlayerGraphics.orig_DefaultBodyPartColorHex orig, SlugcatStats.Name slugcatID)
        {
            if (ModManager.MSC && slugcatID.value == "Gravelslug")
            {
                List<string> list = new List<string>();
                Color col = PlayerGraphics.DefaultSlugcatColor(slugcatID);
                list.Add(RWCustom.Custom.colorToHex(col));
                //list.Add("FFFFFF");
                list.Add("b5cec9");
                list.Add("00FFFF");
                return list;
            }
            else
            {
                return orig.Invoke(slugcatID);
            }
        }

        private List<string> PlayerGraphics_ColoredBodyPartList(On.PlayerGraphics.orig_ColoredBodyPartList orig, SlugcatStats.Name slugcatID)
        {
            List<string> list = orig(slugcatID);
            if (ModManager.MSC && slugcatID.value == "Gravelslug")
            {
                list.Add("Breath");
            }
            return list;
        }

        private Color? Player_StomachGlowLightColor(On.Player.orig_StomachGlowLightColor orig, Player self)
        {
            if (self.slugcatStats.name.value == "Gravelslug" && self.objectInStomach == null && IsGravelFeral(self) && !self.dead)
            {
                Color color = GravelFireColor(self);
                color.a = 0.22f;
                return new Color?(color);
            }
            else
            {
                return orig(self);
            }
        }

        private void RainWorldGame_ExitToVoidSeaSlideShow(On.RainWorldGame.orig_ExitToVoidSeaSlideShow orig, RainWorldGame self)
        {
            if (self.StoryCharacter.value == "Gravelslug")
            {
                if (self.manager.upcomingProcess != null)
                {
                    return;
                }
                RainWorldGame.BeatGameMode(self, true);
                if (!ModManager.MMF)
                {
                    self.ExitGame(false, false);
                }
                //self.manager.nextSlideshow = MoreSlugcatsEnums.SlideShowID.;
                RainWorldGame.ForceSaveNewDenLocation(self, "SB_S01", true);
                self.manager.statsAfterCredits = true;
                self.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.Credits);
            }
            else
            {
                orig(self);
            }
        }

        private void RainWorldGame_GoToRedsGameOver(On.RainWorldGame.orig_GoToRedsGameOver orig, RainWorldGame self)
        {
            if (self.manager.upcomingProcess != null)
            {
                return;
            }
            if (self.GetStorySession.saveState.saveStateNumber.value == "Gravelslug")
            {
                if (self.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW] == -8)
                {
                    self.manager.desiredCreditsSong = "RW_64 - Daze";
                }
                self.manager.statsAfterCredits = true;
                //self.manager.nextSlideshow = MoreSlugcats.MoreSlugcatsEnums.SlideShowID.InvOutro;
                self.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.Credits);
                return;
            }
            orig(self);
        }
        private void SLOracleBehaviorHasMark_InterruptRain(On.SLOracleBehaviorHasMark.orig_InterruptRain orig, SLOracleBehaviorHasMark self)
        {
            if (self.oracle.room.game.StoryCharacter.value == "Gravelslug")
            {
                switch (self.State.neuronsLeft)
                {
                    case 2:
                        self.dialogBox.Interrupt("...", 5);
                        self.dialogBox.Interrupt(self.Translate("...Stars..."), 5);
                        self.dialogBox.NewMessage(self.Translate("pretty"), 10);
                        break;
                    case 3:
                        self.dialogBox.Interrupt("...", 5);
                        self.dialogBox.NewMessage(self.Translate("...Night... coming... Soon..."), 10);
                        return;
                    case 4:
                        self.dialogBox.Interrupt("...", 5);
                        self.dialogBox.NewMessage(self.Translate("Night... You can stay. It will be fine."), 10);
                        return;
                    case 5:
                        if (self.State.GetOpinion == SLOrcacleState.PlayerOpinion.Dislikes)
                        {
                            self.dialogBox.Interrupt(self.Translate("Night is coming. Now, leave me alone."), 5);
                            if (ModManager.MSC && self.CheckSlugpupsInRoom())
                            {
                                self.dialogBox.NewMessage(self.Translate("Take your offspring with you."), 10);
                                return;
                            }
                            if (ModManager.MMF && self.CheckStrayCreatureInRoom() != CreatureTemplate.Type.StandardGroundCreature)
                            {
                                self.dialogBox.NewMessage(self.Translate("Take your pet with you."), 10);
                                return;
                            }
                        }
                        else
                        {
                            self.dialogBox.Interrupt("...", 5);
                            self.dialogBox.NewMessage(self.Translate("I think night is approaching."), 20);
                            self.dialogBox.NewMessage(self.Translate("You can stay if you like, <PlayerName>! It will be fine.<LINE>The stars are beautiful, somtimes it's hard to see them through the rain and snow.."), 0);
                            if (ModManager.MSC && self.CheckSlugpupsInRoom())
                            {
                                self.dialogBox.NewMessage(self.Translate("Your family is welcome to stargaze too!"), 10);
                                return;
                            }
                            if (ModManager.MMF && self.CheckStrayCreatureInRoom() != CreatureTemplate.Type.StandardGroundCreature)
                            {
                                self.dialogBox.NewMessage(self.Translate("Your friend is welcome to stargaze too!"), 10);
                                return;
                            }
                        }
                        break;
                    default:
                        return;
                }
                return;
            }
            orig(self);
        }
        private void SSOracleRubicon_Update(On.SSOracleBehavior.SSOracleRubicon.orig_Update orig, SSOracleBehavior.SSOracleRubicon self)
        {
            orig(self);
            if (self.oracle.room.game.GetStorySession.characterStats.name.value == "Gravelslug")
            {
                if (self.noticedPlayer && self.player != null && self.finalGhostFade < 34)
                {
                    /*if (self.player.Malnourished)
                    {
                        self.player.SetMalnourished(false);
                    }*/
                    if (self.player.controller == null)
                    {
                        self.player.controller = new Player.NullController();
                    }
                    self.player.Blink(10);
                    //self.player.Stun(40);
                }
                if (self.finalGhostFade == 34)
                {
                    if (self.player.controller != null)
                    {
                        self.player.controller = null;
                    }
                    self.player.aerobicLevel = 1;
                    self.player.exhausted = true;
                }
            }
        }

        private void PebblesConversation_AddEvents(On.SSOracleBehavior.PebblesConversation.orig_AddEvents orig, SSOracleBehavior.PebblesConversation self)
        {
            if (self.owner.oracle.room.game.GetStorySession.characterStats.name.value == "Gravelslug" && self.id == MoreSlugcatsEnums.ConversationID.Pebbles_HR)
            {
                self.events = new List<Conversation.DialogueEvent>();
                self.events.Add(new Conversation.TextEvent(self, 0, "Why won't you wake up?", 8));
                self.events.Add(new Conversation.TextEvent(self, 0, "I've tried again, and again to stimulate your brain to function with no avail.", 12));
                self.events.Add(new Conversation.TextEvent(self, 0, "I have just about lost hope of ever creating a solution to my very own problem.", 12));
                self.events.Add(new Conversation.TextEvent(self, 0, "I can feel everything falling away from me. My memories, my work, my hope... All lost to a foolish mistake...", 14));
                self.events.Add(new Conversation.TextEvent(self, 0, "I'm so close to slowing- Even halting my sickness!", 12));
                self.events.Add(new Conversation.TextEvent(self, 0, "Oh Little Experiment... Please... wake up...", 12));
                self.events.Add(new Conversation.TextEvent(self, 0, "This is no way for a god to die...", 12));
                return;
            }
            orig(self);
        }

        private void DaddyGraphics_ctor(On.DaddyGraphics.orig_ctor orig, DaddyGraphics self, PhysicalObject ow)
        {
            orig(self, ow);
            if (self.daddy.room.game.IsStorySession && self.daddy.room.game.GetStorySession.characterStats.name.value == "Gravelslug" && self.daddy.room.world.name == "HR")
            {
                if (self.daddy.HDmode)
                {
                    self.daddy.effectColor = Color.cyan;
                    self.daddy.eyeColor = Color.cyan;
                }
            }
        }

        private void DaddyLongLegs_ctor(On.DaddyLongLegs.orig_ctor orig, DaddyLongLegs self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (world.game.IsStorySession && world.game.GetStorySession.characterStats.name.value == "Gravelslug" && world.name == "HR")
            {
                if (self.HDmode)
                {
                    self.effectColor = Color.cyan;
                }
            }
        }

        Creature GravelEaten;
        private IEnumerator GravelPursuer(Room room, RWCustom.IntVector2 pos)
        {
            yield return new WaitForSeconds(2f);
            if (GravelEaten != null && (GravelEaten.room != room || room.abstractRoom.name == "HR_AI" || room.abstractRoom.shelter))
            {

                GravelEaten.Destroy();
                //GravelEaten.PlaceInRoom(newRoom);
            }
            if (GravelEaten == null || (GravelEaten.room != room && room.abstractRoom.name != "HR_AI" && !room.abstractRoom.shelter))
            {
                AbstractCreature abstractCreature = new AbstractCreature(room.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.HunterDaddy), null, room.GetWorldCoordinate(pos), room.game.GetNewID());
                abstractCreature.saveCreature = false;
                abstractCreature.ignoreCycle = true;
                abstractCreature.voidCreature = true;
                abstractCreature.lavaImmune = true;
                room.abstractRoom.AddEntity(abstractCreature);
                abstractCreature.RealizeInRoom();
                (abstractCreature.realizedCreature as DaddyLongLegs).SpitOutOfShortCut(pos, room, false);
                //(abstractCreature.realizedCreature as DaddyLongLegs).Stun(20);
                GravelEaten = abstractCreature.realizedCreature;
                room.AddObject(new ShockWave(new Vector2(GravelEaten.mainBodyChunk.pos.x, GravelEaten.mainBodyChunk.pos.y), 300f, 0.2f, 15, false));
                room.PlaySound(SoundID.SS_AI_Give_The_Mark_Boom, new Vector2(GravelEaten.mainBodyChunk.pos.x, GravelEaten.mainBodyChunk.pos.y));
                Debug.Log("Teleported GLL to " + room.abstractRoom.name);
            }
        }
        private void Player_SpitOutOfShortCut(On.Player.orig_SpitOutOfShortCut orig, Player self, RWCustom.IntVector2 pos, Room newRoom, bool spitOutAllSticks)
        {
            orig(self, pos, newRoom, spitOutAllSticks);
            if (newRoom.dustStorm && DustStormSound != null)
            {
                CurrentDustSoundRoom = newRoom;
            }
            if (newRoom.game.IsStorySession && newRoom.game.GetStorySession.characterStats.name.value == "Gravelslug")
            {
                if (newRoom.world.name == "HR" && newRoom.abstractRoom.name != "HR_FINAL")
                {
                    //self.SetMalnourished(true);
                    StartCoroutine(GravelPursuer(newRoom, pos));
                }
            }
        }

        private void VoidSeaScene_ArtificerEndUpdate(On.VoidSea.VoidSeaScene.orig_ArtificerEndUpdate orig, VoidSea.VoidSeaScene self, Player player, int timer)
        {
            if (self.room.game.GetStorySession.characterStats.name.value == "Gravelslug")
            {
                /*if (timer == 720)
                {
                    player.room.game.setupValues.invincibility = false;
                }*/
                int num = 320;
                int num2 = 600;
                if (timer > num)
                {
                    /*if (self.eggLoop == null)
                    {
                        self.eggLoop = new DisembodiedDynamicSoundLoop(self);
                        self.eggLoop.sound = SoundID.Void_Sea_The_Core_LOOP;
                        self.eggLoop.VolumeGroup = 1;
                        self.theEgg = null;
                    }
                    self.eggLoop.Volume = Mathf.Max(0f, (float)(timer - num) / (float)num2 * 0.9f);
                    self.eggLoop.Update();*/
                    //player.dissolved = Mathf.Max(0.01f, (float)(timer - num) / (float)num2 * 0.4f);
                    //float num4 = timer / (num2 - 100);
                    float num3 = Mathf.InverseLerp(num + num2 - 60, num2, timer);
                    //float num3 = player.airInLungs;
                    //num3 -= 1f / (40f * (player.lungsExhausted ? 4.5f : 9f) * ((player.input[0].y == 1 && player.input[0].x == 0 && num3 < 0.33333334f) ? 1.5f : 1f) * ((float)player.room.game.setupValues.lungs / 100f));
                    if (timer < num + num2)
                    {
                        player.swimCycle = num3;
                        if (num3 < 0.6666667f && UnityEngine.Random.value > num3)
                        {
                            player.room.AddObject(new Bubble(player.firstChunk.pos, player.firstChunk.vel, false, false));
                        }
                        if (num3 <= 0f)
                        {
                            //num3 = 0f;
                            if (player.controller == null)
                            {
                                player.controller = new Player.NullController();
                            }
                            player.Stun(10);
                        }
                        else if (num3 < 0.33333334f)
                        {
                            if (player.slowMovementStun < 1)
                            {
                                player.slowMovementStun = 1;
                            }
                            if (UnityEngine.Random.value < 0.5f)
                            {
                                player.firstChunk.vel += RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f) * UnityEngine.Random.value;
                            }
                            if (player.input[0].y < 1)
                            {
                                player.bodyChunks[1].vel *= Mathf.Lerp(1f, 0.9f, Mathf.InverseLerp(0f, 0.33333334f, num3));
                            }
                            if ((UnityEngine.Random.value > num3 * 2f || num3 == 0f) && UnityEngine.Random.value > 0.5f)
                            {
                                player.room.AddObject(new Bubble(player.firstChunk.pos, player.firstChunk.vel + RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f) * Mathf.Lerp(6f, 0f, num3), false, false));
                            }
                        }
                        player.submerged = true;
                        player.airInLungs = num3;
                    }
                }
                if (timer >= num + num2)
                {
                    //player.dead = true;
                    player.swimCycle = 0f;
                    player.Blink(10);
                    //player.room.AddObject(new GhostPing(player.room));
                    if (self.blackFade == null)
                    {
                        self.blackFade = new FadeOut(self.room, Color.black, 200f, false);
                        self.room.AddObject(self.blackFade);
                    }
                    //self.eggLoop.Volume = 0.9f - Mathf.Max(0f, (float)(timer - (num + num2)) / 200f * 0.9f);
                    //self.eggLoop.Update();
                }
                if (self.blackFade != null && self.blackFade.IsDoneFading())
                {
                    if (!self.endingSavedFlag)
                    {
                        self.room.game.ExitToVoidSeaSlideShow();
                    }
                    self.endingSavedFlag = true;
                }
            }
            else
            {
                orig(self, player, timer);
            }
        }

        private void GhostID_UnregisterValues(On.MoreSlugcats.MoreSlugcatsEnums.GhostID.orig_UnregisterValues orig)
        {
            orig();
            GravelGhostID.UnregisterValues();
        }

        private void GhostID_RegisterValues(On.MoreSlugcats.MoreSlugcatsEnums.GhostID.orig_RegisterValues orig)
        {
            orig();
            GravelGhostID.RegisterValues();
        }
        /*private void StoryGameSession_ctor(On.StoryGameSession.orig_ctor orig, StoryGameSession self, SlugcatStats.Name saveStateNumber, RainWorldGame game)
        {
            orig(self, saveStateNumber, game);
            if (saveStateNumber.value == "Gravelslug" && game. && !(self.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW] > 0))
            {
                self.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW] = 0;
            }
        }*/
        DisembodiedDynamicSoundLoop DustStormSound;
        Room CurrentDustSoundRoom;
        private void DustWave_Update(On.MoreSlugcats.DustWave.orig_Update orig, MoreSlugcats.DustWave self, bool eu)
        {
            orig(self, eu);
            if (self.room.ViewedByAnyCamera(self.pos, 3000f))
            {
                if (CurrentDustSoundRoom == null)
                {
                    CurrentDustSoundRoom = self.room;
                }
                if (DustStormSound == null || self.room != CurrentDustSoundRoom)
                {
                    DustStormSound = new DisembodiedDynamicSoundLoop(self);
                    DustStormSound.sound = SoundID.Blizzard_Wind_LOOP;
                    DustStormSound.Volume = 0f;
                    DustStormSound.Pitch = 0.9f;
                }
                else
                {
                    DustStormSound.Update();
                    DustStormSound.Volume = 0.2f + Mathf.InverseLerp((float)self.room.world.rainCycle.cycleLength, 0f, (float)self.room.world.rainCycle.TimeUntilRain) * (self.room.roomSettings.RainIntensity * self.room.roomSettings.GetEffectAmount(MoreSlugcats.MoreSlugcatsEnums.RoomEffectType.DustWave));
                }
            }
        }

        private void PhysicalObject_WeatherInertia(On.PhysicalObject.orig_WeatherInertia orig, PhysicalObject self)
        {
            orig(self);
            if (self.room != null && self.room.dustStorm && self.room.roomSettings.DangerType == RoomRain.DangerType.Thunder && UnityEngine.Random.value < 0.1f)
            {
                foreach (BodyChunk bodyChunk in self.bodyChunks)
                {
                    float x = self.room.game.globalRain.OutsidePushAround * self.room.roomSettings.GetEffectAmount(MoreSlugcats.MoreSlugcatsEnums.RoomEffectType.DustWave) * UnityEngine.Random.Range(6f, 8f);
                    float y = self.room.game.globalRain.OutsidePushAround * self.room.roomSettings.GetEffectAmount(MoreSlugcats.MoreSlugcatsEnums.RoomEffectType.DustWave) * UnityEngine.Random.Range(10f, -10f);
                    Vector2 a = new Vector2(-4f * UnityEngine.Random.Range(0.75f, 1.25f) - x, 0.1f + y);
                    a *= Mathf.InverseLerp((float)self.room.world.rainCycle.cycleLength, 0f, (float)self.room.world.rainCycle.TimeUntilRain) * (self.room.roomSettings.RainIntensity * self.room.roomSettings.GetEffectAmount(MoreSlugcats.MoreSlugcatsEnums.RoomEffectType.DustWave));
                    bodyChunk.vel += Vector2.Lerp(a, a * 0.08f, self.Submersion) * Mathf.InverseLerp(40f, 1f, self.TotalMass);
                    if (bodyChunk.owner is Creature && self.room.world.rainCycle.deathRainHasHit && self.room.game.globalRain.Intensity >= 1)
                    {
                        if (Mathf.Pow(UnityEngine.Random.value, 1.2f) * 2f * (float)bodyChunk.owner.bodyChunks.Length < 20f)
                        {
                            (bodyChunk.owner as Creature).Stun(UnityEngine.Random.Range(1, 1 + (int)(9f * 20f)));
                        }
                        if (bodyChunk == (bodyChunk.owner as Creature).mainBodyChunk)
                        {
                            (bodyChunk.owner as Creature).rainDeath += 1f;
                        }
                        if ((bodyChunk.owner as Creature).rainDeath > 1f && UnityEngine.Random.value < 0.025f)
                        {
                            (bodyChunk.owner as Creature).Die();
                        }
                    }
                    if (bodyChunk.owner is Player)
                    {
                        (bodyChunk.owner as Player).Blink(80);
                        //(bodyChunk.owner as Player).playerState.permanentDamageTracking += 0.01;
                    }
                    /*if (bodyChunk.owner is Creature && self.room.world.rainCycle.sunDownStartTime >= self.room.world.rainCycle.timer)
                    {
                        (bodyChunk.owner as Creature).Die();
                    }*/
                }
            }
        }

        public float SandstormPush(RoomRain self)
        {
            if (self.dangerType == RoomRain.DangerType.AerieBlizzard)
            {
                return self.globalRain.OutsidePushAround;
            }
            if (self.dangerType == RoomRain.DangerType.Rain || self.dangerType == RoomRain.DangerType.FloodAndRain)
            {
                return self.globalRain.OutsidePushAround * self.room.roomSettings.RainIntensity;
            }
            if (/*self.room.dustStorm && */self.dangerType == RoomRain.DangerType.Thunder)
            {
                return self.globalRain.OutsidePushAround * self.room.roomSettings.RainIntensity;
            }
            return 0f;
        }

        private void RainWorldGame_RestartGame(On.RainWorldGame.orig_RestartGame orig, RainWorldGame self)
        {
            orig(self);
            GravelGutDissolveTimer = 120f;
        }

        private void DaddyCorruption_ctor(On.DaddyCorruption.orig_ctor orig, DaddyCorruption self, Room room)
        {
            orig(self, room);
            if (room.game.IsStorySession && room.world.region != null && room.game.GetStorySession.saveStateNumber.value == "Gravelslug" && room.world.name == "HR")
            {
                self.effectColor = room.world.region.regionParams.corruptionEffectColor;
                self.eyeColor = room.world.region.regionParams.corruptionEyeColor;
            }
        }

        private int RainCycle_GetDesiredCycleLength(On.RainCycle.orig_GetDesiredCycleLength orig, RainCycle self)
        {
            int num = orig.Invoke(self);
            if (!self.world.singleRoomWorld && self.world.game.IsStorySession && (self.world.game.session as StoryGameSession).saveState.saveStateNumber.value == "Gravelslug")
            {
                if (self.world.region.name == "HR")
                {
                    num = (int)((float)num * 0.3f);
                }
            }
            return num;
        }

        private void MainWormBehavior_Update(On.VoidSea.VoidWorm.MainWormBehavior.orig_Update orig, VoidSea.VoidWorm.MainWormBehavior self)
        {
            orig(self);
            Player player = self.voidSea.room.game.FirstRealizedPlayer;
            if (player.slugcatStats.name.value == "Gravelslug" && self.worm.voidSea.room.world.region.name == "HR")
            {
                if (self.phase == VoidSea.VoidWorm.MainWormBehavior.Phase.Looking && self.timeInPhase > 600)
                {
                    self.SwitchPhase(VoidSea.VoidWorm.MainWormBehavior.Phase.SwimDown);
                }
                if (self.phase == VoidSea.VoidWorm.MainWormBehavior.Phase.SwimDown)
                {
                    if (self.worm.chunks[0].pos.y < -35000f)
                    {
                        self.voidSea.fadeOutLights = true;
                    }
                }

                /*if (self.phase == VoidSea.VoidWorm.MainWormBehavior.Phase.SwimUp)
                    {
                        self.goalPos = new UnityEngine.Vector2(self.voidSea.sceneOrigo.x, self.voidSea.voidWormsAltitude + 7000f);
                        if (self.worm.chunks[0].pos.y > self.voidSea.voidWormsAltitude + 7000f)
                        {
                            self.SwitchPhase(VoidSea.VoidWorm.MainWormBehavior.Phase.SwimDown);
                        }
                    }
                    if (self.phase == VoidSea.VoidWorm.MainWormBehavior.Phase.SwimDown)
                    {
                        self.goalPos = self.worm.chunks[0].pos + new UnityEngine.Vector2(0f, 100000f);
                        if (!self.voidSea.secondSpace)
                        {
                            if (self.worm.chunks[0].pos.y > 17000f && (int)self.voidSea.deepDivePhase > (int)VoidSea.VoidSeaScene.DeepDivePhase.CeilingDestroyed)
                            {
                                self.voidSea.DestroyCeiling();
                            }
                            if (self.worm.chunks[0].pos.y > 35000f && (int)self.voidSea.deepDivePhase > (int)VoidSea.VoidSeaScene.DeepDivePhase.CloseWormsDestroyed)
                            {
                                self.voidSea.DestroyAllWormsExceptMainWorm();
                            }
                            if (self.worm.chunks[0].pos.y > 200000f && (int)self.voidSea.deepDivePhase > (int)VoidSea.VoidSeaScene.DeepDivePhase.DistantWormsDestroyed)
                            {
                                self.voidSea.DestroyDistantWorms();
                            }
                            if (self.worm.chunks[0].pos.y > 440000f && (int)self.voidSea.deepDivePhase > (int)VoidSea.VoidSeaScene.DeepDivePhase.MovedIntoSecondSpace)
                            {
                                self.voidSea.MovedToSecondSpace();
                            }
                        }
                        else if (self.worm.chunks[0].pos.y > 11000f)
                        {
                            self.SwitchPhase(VoidSea.VoidWorm.MainWormBehavior.Phase.DepthReached);
                        }
                        if (self.worm.chunks[0].pos.y > 35000f)
                        {
                            self.voidSea.fadeOutLights = true;
                        }

                    }*/
            }
        }

        private void Player_Regurgitate(On.Player.orig_Regurgitate orig, Player self)
        {
            if (self.slugcatStats.name.value == "Gravelslug")
            {
                if (self.objectInStomach == null)
                {
                    if (self.FoodInStomach > 0)
                    {
                        self.objectInStomach = new AbstractConsumable(self.room.world, AbstractPhysicalObject.AbstractObjectType.Rock, null, self.room.GetWorldCoordinate(self.firstChunk.pos), self.room.game.GetNewID(), -1, -1, null);
                        Debug.Log("Player spit out gravel.");
                    }
                }
            }
            orig(self);
        }
        private void Player_GrabUpdate(On.Player.orig_GrabUpdate orig, Player self, bool eu)
        {
            if (self.slugcatStats.name.value == "Gravelslug")
            {
                if (self.spearOnBack != null)
                {
                    self.spearOnBack.Update(eu);
                }
                if ((ModManager.MSC || ModManager.CoopAvailable) && self.slugOnBack != null)
                {
                    self.slugOnBack.Update(eu);
                }
                bool flag = ((self.input[0].x == 0 && self.input[0].y == 0 && !self.input[0].jmp && !self.input[0].thrw) || (ModManager.MMF && self.input[0].x == 0 && self.input[0].y == 1 && !self.input[0].jmp && !self.input[0].thrw && (self.bodyMode != Player.BodyModeIndex.ClimbingOnBeam || self.animation == Player.AnimationIndex.BeamTip || self.animation == Player.AnimationIndex.StandOnBeam))) && (self.mainBodyChunk.submersion < 0.5f || self.isRivulet);
                bool flag2 = false;
                bool flag3 = false;
                self.craftingObject = false;
                int num = -1;
                int num2 = -1;
                bool flag4 = false;
                if (self.input[0].pckp && !self.input[1].pckp && self.switchHandsProcess == 0f && !self.isSlugpup)
                {
                    bool flag5 = self.grasps[0] != null || self.grasps[1] != null;
                    if (self.grasps[0] != null && (self.Grabability(self.grasps[0].grabbed) == Player.ObjectGrabability.TwoHands || self.Grabability(self.grasps[0].grabbed) == Player.ObjectGrabability.Drag))
                    {
                        flag5 = false;
                    }
                    if (flag5)
                    {
                        if (self.switchHandsCounter == 0)
                        {
                            self.switchHandsCounter = 15;
                        }
                        else
                        {
                            self.room.PlaySound(SoundID.Slugcat_Switch_Hands_Init, self.mainBodyChunk);
                            self.switchHandsProcess = 0.01f;
                            self.wantToPickUp = 0;
                            self.noPickUpOnRelease = 20;
                        }
                    }
                    else
                    {
                        self.switchHandsProcess = 0f;
                    }
                }
                if (self.switchHandsProcess > 0f)
                {
                    float num3 = self.switchHandsProcess;
                    self.switchHandsProcess += 0.083333336f;
                    if (num3 < 0.5f && self.switchHandsProcess >= 0.5f)
                    {
                        self.room.PlaySound(SoundID.Slugcat_Switch_Hands_Complete, self.mainBodyChunk);
                        self.SwitchGrasps(0, 1);
                    }
                    if (self.switchHandsProcess >= 1f)
                    {
                        self.switchHandsProcess = 0f;
                    }
                }
                int num4 = -1;
                int num5 = -1;
                int num6 = -1;
                if (flag)
                {
                    int num7 = -1;
                    if (ModManager.MSC)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            if (self.grasps[i] != null)
                            {
                                if (self.grasps[i].grabbed is JokeRifle)
                                {
                                    num2 = i;
                                }
                                else if (JokeRifle.IsValidAmmo(self.grasps[i].grabbed))
                                {
                                    num = i;
                                }
                            }
                        }
                    }
                    int num8 = 0;
                    while (num5 < 0 && num8 < 2)
                    {
                        if (self.grasps[num8] != null && self.grasps[num8].grabbed is IPlayerEdible && (self.grasps[num8].grabbed as IPlayerEdible).Edible && self.input[0].y <= 0)
                        {
                            num5 = num8;
                        }
                        num8++;
                    }
                    if ((num5 == -1 || (!(self.grasps[num5].grabbed is KarmaFlower) && !(self.grasps[num5].grabbed is Mushroom))) && (self.objectInStomach == null || self.CanPutSpearToBack || self.CanPutSlugToBack))
                    {
                        int num9 = 0;
                        while (num7 < 0 && num4 < 0 && num6 < 0 && num9 < 2)
                        {
                            if (self.grasps[num9] != null)
                            {
                                if ((self.CanPutSlugToBack && self.grasps[num9].grabbed is Player && !(self.grasps[num9].grabbed as Player).dead) || self.CanIPutDeadSlugOnBack(self.grasps[num9].grabbed as Player))
                                {
                                    num6 = num9;
                                }
                                else if (self.CanPutSpearToBack && self.grasps[num9].grabbed is Spear)
                                {
                                    num4 = num9;
                                }
                                else if (self.CanBeSwallowed(self.grasps[num9].grabbed))
                                {
                                    num7 = num9;
                                }
                            }
                            num9++;
                        }
                    }
                    if (num5 > -1 && self.noPickUpOnRelease < 1)
                    {
                        if (!self.input[0].pckp)
                        {
                            int num10 = 1;
                            while (num10 < 10 && self.input[num10].pckp)
                            {
                                num10++;
                            }
                            if (num10 > 1 && num10 < 10)
                            {
                                self.PickupPressed();
                            }
                        }
                    }
                    else if (self.input[0].pckp && !self.input[1].pckp)
                    {
                        self.PickupPressed();
                    }
                    if (self.input[0].pckp)
                    {
                        if (ModManager.MSC && (self.FreeHand() == -1 || self.SlugCatClass == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Artificer) && self.GraspsCanBeCrafted())
                        {
                            self.craftingObject = true;
                            flag3 = true;
                            num5 = -1;
                        }
                        if (num6 > -1 || self.CanRetrieveSlugFromBack)
                        {
                            self.slugOnBack.increment = true;
                        }
                        else if (num4 > -1 || self.CanRetrieveSpearFromBack)
                        {
                            self.spearOnBack.increment = true;
                        }
                        flag3 = true;
                        if (num > -1 && num2 > -1)
                        {
                            flag4 = true;
                        }
                    }
                    if (num5 > -1 && self.wantToPickUp < 1 && (self.input[0].pckp || self.eatCounter <= 15) && self.Consious && RWCustom.Custom.DistLess(self.mainBodyChunk.pos, self.mainBodyChunk.lastPos, 3.6f))
                    {
                        if (self.graphicsModule != null)
                        {
                            (self.graphicsModule as PlayerGraphics).LookAtObject(self.grasps[num5].grabbed);
                        }
                        flag2 = true;
                        if (self.FoodInStomach < self.MaxFoodInStomach || self.grasps[num5].grabbed is KarmaFlower || self.grasps[num5].grabbed is Mushroom)
                        {
                            flag3 = false;
                            if (self.spearOnBack != null)
                            {
                                self.spearOnBack.increment = false;
                            }
                            if ((ModManager.MSC || ModManager.CoopAvailable) && self.slugOnBack != null)
                            {
                                self.slugOnBack.increment = false;
                            }
                            if (self.eatCounter < 1)
                            {
                                self.eatCounter = 15;
                                self.BiteEdibleObject(eu);
                            }
                        }
                        else if (self.eatCounter < 20 && self.room.game.cameras[0].hud != null)
                        {
                            self.room.game.cameras[0].hud.foodMeter.RefuseFood();
                        }
                    }
                }
                else if (self.input[0].pckp && !self.input[1].pckp)
                {
                    self.PickupPressed();
                }
                else
                {
                    if (self.CanPutSpearToBack)
                    {
                        for (int m = 0; m < 2; m++)
                        {
                            if (self.grasps[m] != null && self.grasps[m].grabbed is Spear)
                            {
                                num4 = m;
                                break;
                            }
                        }
                    }
                    if (self.CanPutSlugToBack)
                    {
                        for (int n = 0; n < 2; n++)
                        {
                            if (self.grasps[n] != null && self.grasps[n].grabbed is Player && !(self.grasps[n].grabbed as Player).dead)
                            {
                                num6 = n;
                                break;
                            }
                        }
                    }
                    if (self.input[0].pckp && (num6 > -1 || self.CanRetrieveSlugFromBack))
                    {
                        self.slugOnBack.increment = true;
                    }
                    if (self.input[0].pckp && (num4 > -1 || self.CanRetrieveSpearFromBack))
                    {
                        self.spearOnBack.increment = true;
                    }
                }
                int num11 = 0;
                if (ModManager.MMF && (self.grasps[0] == null || !(self.grasps[0].grabbed is Creature)) && self.grasps[1] != null && self.grasps[1].grabbed is Creature)
                {
                    num11 = 1;
                }
                if (ModManager.MSC && SlugcatStats.SlugcatCanMaul(self.SlugCatClass))
                {
                    if (self.input[0].pckp && self.grasps[num11] != null && (((self.grasps[num11].grabbed is Creature && (self.CanMaulCreature(self.grasps[num11].grabbed as Creature))) || (self.grasps[num11].grabbed is Oracle && IsGravelFeral(self))) || self.maulTimer > 0))
                    {
                        self.maulTimer++;
                        if(UnityEngine.Random.value > 0.75)
                        {
                            Fire_Breath(self, true, true);
                        }
                        if(UnityEngine.Random.value > 0.95)
                        {
                            GravelCough(self, false);
                        }
                        if (self.grasps[num11].grabbed is Creature)
                        {
                            (self.grasps[num11].grabbed as Creature).Stun(60);
                        }
                        self.MaulingUpdate(num11);
                        if (self.spearOnBack != null)
                        {
                            self.spearOnBack.increment = false;
                            self.spearOnBack.interactionLocked = true;
                        }
                        if (self.slugOnBack != null)
                        {
                            self.slugOnBack.increment = false;
                            self.slugOnBack.interactionLocked = true;
                        }
                        if (self.grasps[num11] != null && self.maulTimer % 40 == 0)
                        {
                            if(self.grasps[num11].grabbed is Oracle)
                            {
                                self.room.PlaySound(SoundID.Spear_Bounce_Off_Creauture_Shell, self.mainBodyChunk);
                                if ((self.grasps[num11].grabbed as Oracle).ID == Oracle.OracleID.SL)
                                {
                                    ((self.grasps[num11].grabbed as Oracle).oracleBehavior as SLOracleBehaviorHasMark).Pain();
                                }
                                if ((self.grasps[num11].grabbed as Oracle).ID == MoreSlugcatsEnums.OracleID.CL)
                                {
                                    ((self.grasps[num11].grabbed as Oracle).oracleBehavior as CLOracleBehavior).Pain();
                                }
                            }
                            else
                            {
                                self.room.PlaySound(SoundID.Slugcat_Eat_Meat_B, self.mainBodyChunk);
                                self.room.PlaySound(SoundID.Drop_Bug_Grab_Creature, self.mainBodyChunk, false, 1f, 0.76f);
                            }
                            if (RainWorld.ShowLogs)
                            {
                                Debug.Log("Mauled target");
                            }
                            if (self.grasps[num11].grabbed is Creature)
                            {
                                if (!(self.grasps[num11].grabbed as Creature).dead)
                                {
                                    for (int num12 = UnityEngine.Random.Range(8, 14); num12 >= 0; num12--)
                                    {
                                        self.room.AddObject(new WaterDrip(UnityEngine.Vector2.Lerp(self.grasps[num11].grabbedChunk.pos, self.mainBodyChunk.pos, UnityEngine.Random.value) + self.grasps[num11].grabbedChunk.rad * RWCustom.Custom.RNV() * UnityEngine.Random.value, RWCustom.Custom.RNV() * 6f * UnityEngine.Random.value + RWCustom.Custom.DirVec(self.grasps[num11].grabbed.firstChunk.pos, (self.mainBodyChunk.pos + (self.graphicsModule as PlayerGraphics).head.pos) / 2f) * 7f * UnityEngine.Random.value + RWCustom.Custom.DegToVec(Mathf.Lerp(-90f, 90f, UnityEngine.Random.value)) * UnityEngine.Random.value * self.EffectiveRoomGravity * 7f, false));
                                    }
                                    Creature creature = self.grasps[num11].grabbed as Creature;
                                    creature.SetKillTag(self.abstractCreature);
                                    creature.Violence(self.bodyChunks[0], new UnityEngine.Vector2?(new UnityEngine.Vector2(0f, 0f)), self.grasps[num11].grabbedChunk, null, Creature.DamageType.Bite, 1f, 15f);
                                    creature.stun = 5;
                                    if (creature.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.Inspector)
                                    {
                                        creature.Die();
                                    }
                                }
                            }
                            self.maulTimer = 0;
                            self.wantToPickUp = 0;
                            if (self.grasps[num11] != null)
                            {
                                self.TossObject(num11, eu);
                                self.ReleaseGrasp(num11);
                            }
                            self.standing = true;
                        }
                        return;
                    }
                    if (self.grasps[num11] != null && self.grasps[num11].grabbed is Creature && (self.grasps[num11].grabbed as Creature).Consious && !self.IsCreatureLegalToHoldWithoutStun(self.grasps[num11].grabbed as Creature))
                    {
                        if (RainWorld.ShowLogs)
                        {
                            Debug.Log("Lost hold of live mauling target");
                        }
                        self.maulTimer = 0;
                        self.wantToPickUp = 0;
                        self.ReleaseGrasp(num11);
                        return;
                    }
                }
                if (self.input[0].pckp && self.grasps[num11] != null && self.grasps[num11].grabbed is Creature && self.CanEatMeat(self.grasps[num11].grabbed as Creature) && (self.grasps[num11].grabbed as Creature).Template.meatPoints > 0)
                {
                    self.eatMeat++;
                    self.EatMeatUpdate(num11);
                    if (!ModManager.MMF)
                    {
                    }
                    if (self.spearOnBack != null)
                    {
                        self.spearOnBack.increment = false;
                        self.spearOnBack.interactionLocked = true;
                    }
                    if ((ModManager.MSC || ModManager.CoopAvailable) && self.slugOnBack != null)
                    {
                        self.slugOnBack.increment = false;
                        self.slugOnBack.interactionLocked = true;
                    }
                    if (self.grasps[num11] != null && self.eatMeat % 80 == 0 && ((self.grasps[num11].grabbed as Creature).State.meatLeft <= 0 || self.FoodInStomach >= self.MaxFoodInStomach))
                    {
                        self.eatMeat = 0;
                        self.wantToPickUp = 0;
                        self.TossObject(num11, eu);
                        self.ReleaseGrasp(num11);
                        self.standing = true;
                    }
                    return;
                }
                if (!self.input[0].pckp && self.grasps[num11] != null && self.eatMeat > 60)
                {
                    self.eatMeat = 0;
                    self.wantToPickUp = 0;
                    self.TossObject(num11, eu);
                    self.ReleaseGrasp(num11);
                    self.standing = true;
                    return;
                }
                self.eatMeat = RWCustom.Custom.IntClamp(self.eatMeat - 1, 0, 50);
                self.maulTimer = RWCustom.Custom.IntClamp(self.maulTimer - 1, 0, 20);
                if (!ModManager.MMF || self.input[0].y == 0)
                {
                    if (flag2 && self.eatCounter > 0)
                    {
                        /*if (ModManager.MSC)
                        {
                            if (num5 <= -1 || self.grasps[num5] == null || !(self.grasps[num5].grabbed is MoreSlugcats.GooieDuck) || (self.grasps[num5].grabbed as MoreSlugcats.GooieDuck).bites != 6 || self.timeSinceSpawned % 2 == 0)
                            {
                                self.eatCounter--;
                            }
                            if (num5 > -1 && self.grasps[num5] != null && self.grasps[num5].grabbed is MoreSlugcats.GooieDuck && (self.grasps[num5].grabbed as MoreSlugcats.GooieDuck).bites == 6 && self.FoodInStomach < self.MaxFoodInStomach)
                            {
                                (self.graphicsModule as PlayerGraphics).BiteStruggle(num5);
                            }
                        }
                        else
                        {
                            self.eatCounter--;
                        }*/
                        self.eatCounter--;
                    }
                    else if (!flag2 && self.eatCounter < 40)
                    {
                        self.eatCounter++;
                    }
                }
                if (flag4 && self.input[0].y == 0)
                {
                    self.reloadCounter++;
                    if (self.reloadCounter > 40)
                    {
                        (self.grasps[num2].grabbed as JokeRifle).ReloadRifle(self.grasps[num].grabbed);
                        BodyChunk mainBodyChunk = self.mainBodyChunk;
                        mainBodyChunk.vel.y = mainBodyChunk.vel.y + 4f;
                        self.room.PlaySound(SoundID.Gate_Clamp_Lock, self.mainBodyChunk, false, 0.5f, 3f + UnityEngine.Random.value);
                        AbstractPhysicalObject abstractPhysicalObject = self.grasps[num].grabbed.abstractPhysicalObject;
                        self.ReleaseGrasp(num);
                        abstractPhysicalObject.realizedObject.RemoveFromRoom();
                        abstractPhysicalObject.Room.RemoveEntity(abstractPhysicalObject);
                        self.reloadCounter = 0;
                    }
                }
                else
                {
                    self.reloadCounter = 0;
                }
                if (ModManager.MMF && self.mainBodyChunk.submersion >= 0.5f)
                {
                    flag3 = false;
                }
                if (flag3)
                {
                    if (self.craftingObject)
                    {
                        self.swallowAndRegurgitateCounter++;
                        if (self.swallowAndRegurgitateCounter > 105)
                        {
                            self.SpitUpCraftedObject();
                            self.swallowAndRegurgitateCounter = 0;
                        }
                    }
                    else if (!ModManager.MMF || self.input[0].y >= 0)
                    {
                        self.swallowAndRegurgitateCounter++;
                        if (num5 > -1 && self.grasps[num5] != null && self.FoodInStomach < self.MaxFoodInStomach && self.objectInStomach == null)
                        {
                            (self.graphicsModule as PlayerGraphics).BiteStruggle(num5);
                        }
                        if (IsGravelFeral(self))
                        {
                            self.swallowAndRegurgitateCounter++;
                        }
                        if (self.swallowAndRegurgitateCounter > 110)
                        {
                            bool flag6 = false;
                            if (self.objectInStomach == null)
                            {
                                flag6 = true;
                            }
                            if (!flag6 || (flag6 && self.FoodInStomach >= 1))
                            {
                                // THE IMPORTANT BIT!!
                                if ((self.input[0].y == 0 || flag6) && !(self.input[0].y > 0))
                                {
                                    self.Regurgitate();
                                }
                                if (flag6 || (self.input[0].y > 0))
                                {
                                    if (flag6 && self.input[0].y > 0)
                                    {
                                        GravelRetaliate(self);
                                    }
                                    if (self.room.game.IsArenaSession)
                                    {
                                        GravelArenaDissolve(self);
                                    }
                                    else
                                    {
                                        GravelDissolveCraft(self);
                                    }
                                }
                                else
                                {
                                    GravelDissolveSubtract(20f, self.room.game, true);
                                }
                            }
                            else
                            {
                                if (self.input[0].y > 0)
                                {
                                    GravelRetaliate(self);
                                }
                                if (self.room.game.IsArenaSession)
                                {
                                    GravelArenaDissolve(self);
                                }
                                else
                                {
                                    GravelDissolveCraft(self);
                                }
                                if (!self.Malnourished && self.room.game.IsStorySession)
                                {
                                    //self.Regurgitate();
                                    self.SetMalnourished(true);
                                }
                                self.firstChunk.vel += new UnityEngine.Vector2(UnityEngine.Random.Range(-1f, 1f), 0f);
                                self.Stun(20);
                            }
                            if (self.spearOnBack != null)
                            {
                                self.spearOnBack.interactionLocked = true;
                            }
                            if ((ModManager.MSC || ModManager.CoopAvailable) && self.slugOnBack != null)
                            {
                                self.slugOnBack.interactionLocked = true;
                            }
                            self.swallowAndRegurgitateCounter = 0;
                        }
                        else if (self.objectInStomach == null && self.swallowAndRegurgitateCounter > 90)
                        {
                            for (int num13 = 0; num13 < 2; num13++)
                            {
                                if (self.grasps[num13] != null && self.CanBeSwallowed(self.grasps[num13].grabbed))
                                {
                                    self.bodyChunks[0].pos += RWCustom.Custom.DirVec(self.grasps[num13].grabbed.firstChunk.pos, self.bodyChunks[0].pos) * 2f;
                                    self.SwallowObject(num13);
                                    if (self.spearOnBack != null)
                                    {
                                        self.spearOnBack.interactionLocked = true;
                                    }
                                    if ((ModManager.MSC || ModManager.CoopAvailable) && self.slugOnBack != null)
                                    {
                                        self.slugOnBack.interactionLocked = true;
                                    }
                                    self.swallowAndRegurgitateCounter = 0;
                                    (self.graphicsModule as PlayerGraphics).swallowing = 20;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (self.swallowAndRegurgitateCounter > 0)
                        {
                            self.swallowAndRegurgitateCounter--;
                        }
                        if (self.eatCounter > 0)
                        {
                            self.eatCounter--;
                        }
                    }
                }
                else
                {
                    self.swallowAndRegurgitateCounter = 0;
                }
                for (int num14 = 0; num14 < self.grasps.Length; num14++)
                {
                    if (self.grasps[num14] != null && self.grasps[num14].grabbed.slatedForDeletetion)
                    {
                        self.ReleaseGrasp(num14);
                    }
                }
                if (self.grasps[0] != null && self.Grabability(self.grasps[0].grabbed) == Player.ObjectGrabability.TwoHands)
                {
                    self.pickUpCandidate = null;
                }
                else
                {
                    PhysicalObject physicalObject = (self.dontGrabStuff < 1) ? self.PickupCandidate(20f) : null;
                    if (self.pickUpCandidate != physicalObject && physicalObject != null && physicalObject is PlayerCarryableItem)
                    {
                        (physicalObject as PlayerCarryableItem).Blink();
                    }
                    self.pickUpCandidate = physicalObject;
                }
                if (self.switchHandsCounter > 0)
                {
                    self.switchHandsCounter--;
                }
                if (self.wantToPickUp > 0)
                {
                    self.wantToPickUp--;
                }
                if (self.wantToThrow > 0)
                {
                    self.wantToThrow--;
                }
                if (self.noPickUpOnRelease > 0)
                {
                    self.noPickUpOnRelease--;
                }
                if (self.input[0].thrw && !self.input[1].thrw && (!ModManager.MSC || !self.monkAscension))
                {
                    self.wantToThrow = 5;
                }
                if (self.wantToThrow > 0)
                {
                    if (ModManager.MSC && MoreSlugcats.MMF.cfgOldTongue.Value && self.grasps[0] == null && self.grasps[1] == null && self.SaintTongueCheck())
                    {
                        UnityEngine.Vector2 vector2 = new UnityEngine.Vector2((float)self.flipDirection, 0.7f);
                        UnityEngine.Vector2 normalized = vector2.normalized;
                        if (self.input[0].y > 0)
                        {
                            normalized = new UnityEngine.Vector2(0f, 1f);
                        }
                        normalized = (normalized + self.mainBodyChunk.vel.normalized * 0.2f).normalized;
                        self.tongue.Shoot(normalized);
                        self.wantToThrow = 0;
                    }
                    else
                    {
                        for (int num15 = 0; num15 < 2; num15++)
                        {
                            if (self.grasps[num15] != null && self.IsObjectThrowable(self.grasps[num15].grabbed))
                            {
                                self.ThrowObject(num15, eu);
                                self.wantToThrow = 0;
                                break;
                            }
                        }
                    }
                    if ((ModManager.MSC || ModManager.CoopAvailable) && self.wantToThrow > 0 && self.slugOnBack != null && self.slugOnBack.HasASlug)
                    {
                        Player slugcat = self.slugOnBack.slugcat;
                        self.slugOnBack.SlugToHand(eu);
                        self.ThrowObject(0, eu);
                        float num16 = (self.ThrowDirection >= 0) ? Mathf.Max(self.bodyChunks[0].pos.x, self.bodyChunks[1].pos.x) : Mathf.Min(self.bodyChunks[0].pos.x, self.bodyChunks[1].pos.x);
                        for (int num17 = 0; num17 < slugcat.bodyChunks.Length; num17++)
                        {
                            slugcat.bodyChunks[num17].pos.y = self.firstChunk.pos.y + 20f;
                            if (self.ThrowDirection < 0)
                            {
                                if (slugcat.bodyChunks[num17].pos.x > num16 - 8f)
                                {
                                    slugcat.bodyChunks[num17].pos.x = num16 - 8f;
                                }
                                if (slugcat.bodyChunks[num17].vel.x > 0f)
                                {
                                    slugcat.bodyChunks[num17].vel.x = 0f;
                                }
                            }
                            else if (self.ThrowDirection > 0)
                            {
                                if (slugcat.bodyChunks[num17].pos.x < num16 + 8f)
                                {
                                    slugcat.bodyChunks[num17].pos.x = num16 + 8f;
                                }
                                if (slugcat.bodyChunks[num17].vel.x < 0f)
                                {
                                    slugcat.bodyChunks[num17].vel.x = 0f;
                                }
                            }
                        }
                    }
                }
                if (self.wantToPickUp > 0)
                {
                    bool flag7 = true;
                    if (self.animation == Player.AnimationIndex.DeepSwim)
                    {
                        if (self.grasps[0] == null && self.grasps[1] == null)
                        {
                            flag7 = false;
                        }
                        else
                        {
                            for (int num18 = 0; num18 < 10; num18++)
                            {
                                if (self.input[num18].y > -1 || self.input[num18].x != 0)
                                {
                                    flag7 = false;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int num19 = 0; num19 < 5; num19++)
                        {
                            if (self.input[num19].y > -1)
                            {
                                flag7 = false;
                                break;
                            }
                        }
                    }
                    if (ModManager.MSC)
                    {
                        if (self.grasps[0] != null && self.grasps[0].grabbed is MoreSlugcats.EnergyCell && self.mainBodyChunk.submersion > 0f)
                        {
                            flag7 = false;
                        }
                        else if (self.grasps[0] != null && self.grasps[0].grabbed is MoreSlugcats.EnergyCell && self.canJump <= 0 && self.bodyMode != Player.BodyModeIndex.Crawl && self.bodyMode != Player.BodyModeIndex.CorridorClimb && self.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut && self.animation != Player.AnimationIndex.HangFromBeam && self.animation != Player.AnimationIndex.ClimbOnBeam && self.animation != Player.AnimationIndex.AntlerClimb && self.animation != Player.AnimationIndex.VineGrab && self.animation != Player.AnimationIndex.ZeroGPoleGrab)
                        {
                            (self.grasps[0].grabbed as MoreSlugcats.EnergyCell).Use(false);
                        }
                    }
                    if (!ModManager.MMF && self.grasps[0] != null && self.HeavyCarry(self.grasps[0].grabbed))
                    {
                        flag7 = true;
                    }
                    if (flag7)
                    {
                        int num20 = -1;
                        for (int num21 = 0; num21 < 2; num21++)
                        {
                            if (self.grasps[num21] != null)
                            {
                                num20 = num21;
                                break;
                            }
                        }
                        if (num20 > -1)
                        {
                            self.wantToPickUp = 0;
                            if (!ModManager.MSC || self.SlugCatClass != MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Artificer || !(self.grasps[num20].grabbed is Scavenger))
                            {
                                self.pyroJumpDropLock = 0;
                            }
                            if (self.pyroJumpDropLock == 0 && (!ModManager.MSC || self.wantToJump == 0))
                            {
                                self.ReleaseObject(num20, eu);
                                return;
                            }
                        }
                        else
                        {
                            if (self.spearOnBack != null && self.spearOnBack.spear != null && self.mainBodyChunk.ContactPoint.y < 0)
                            {
                                self.room.socialEventRecognizer.CreaturePutItemOnGround(self.spearOnBack.spear, self);
                                self.spearOnBack.DropSpear();
                                return;
                            }
                            if ((ModManager.MSC || ModManager.CoopAvailable) && self.slugOnBack != null && self.slugOnBack.slugcat != null && self.mainBodyChunk.ContactPoint.y < 0)
                            {
                                self.room.socialEventRecognizer.CreaturePutItemOnGround(self.slugOnBack.slugcat, self);
                                self.slugOnBack.DropSlug();
                                self.wantToPickUp = 0;
                                return;
                            }
                            if (ModManager.MSC && self.room != null && self.room.game.IsStorySession && self.room.game.GetStorySession.saveState.wearingCloak && self.AI == null)
                            {
                                self.room.game.GetStorySession.saveState.wearingCloak = false;
                                AbstractConsumable abstractConsumable = new AbstractConsumable(self.room.game.world, MoreSlugcats.MoreSlugcatsEnums.AbstractObjectType.MoonCloak, null, self.room.GetWorldCoordinate(self.mainBodyChunk.pos), self.room.game.GetNewID(), -1, -1, null);
                                self.room.abstractRoom.AddEntity(abstractConsumable);
                                abstractConsumable.pos = self.abstractCreature.pos;
                                abstractConsumable.RealizeInRoom();
                                (abstractConsumable.realizedObject as MoreSlugcats.MoonCloak).free = true;
                                for (int num22 = 0; num22 < abstractConsumable.realizedObject.bodyChunks.Length; num22++)
                                {
                                    abstractConsumable.realizedObject.bodyChunks[num22].HardSetPosition(self.mainBodyChunk.pos);
                                }
                                self.dontGrabStuff = 15;
                                self.wantToPickUp = 0;
                                self.noPickUpOnRelease = 20;
                                return;
                            }
                        }
                    }
                    else if (self.pickUpCandidate != null)
                    {
                        if (self.pickUpCandidate is Spear && self.CanPutSpearToBack && ((self.grasps[0] != null && self.Grabability(self.grasps[0].grabbed) >= Player.ObjectGrabability.BigOneHand) || (self.grasps[1] != null && self.Grabability(self.grasps[1].grabbed) >= Player.ObjectGrabability.BigOneHand) || (self.grasps[0] != null && self.grasps[1] != null)))
                        {
                            Debug.Log("spear straight to back");
                            self.room.PlaySound(SoundID.Slugcat_Switch_Hands_Init, self.mainBodyChunk);
                            self.spearOnBack.SpearToBack(self.pickUpCandidate as Spear);
                        }
                        else if (self.CanPutSlugToBack && self.pickUpCandidate is Player && (!(self.pickUpCandidate as Player).dead || self.CanIPutDeadSlugOnBack(self.pickUpCandidate as Player)) && ((self.grasps[0] != null && (self.Grabability(self.grasps[0].grabbed) > Player.ObjectGrabability.BigOneHand || self.grasps[0].grabbed is Player)) || (self.grasps[1] != null && (self.Grabability(self.grasps[1].grabbed) > Player.ObjectGrabability.BigOneHand || self.grasps[1].grabbed is Player)) || (self.grasps[0] != null && self.grasps[1] != null) || self.bodyMode == Player.BodyModeIndex.Crawl))
                        {
                            Debug.Log("slugpup/player straight to back");
                            self.room.PlaySound(SoundID.Slugcat_Switch_Hands_Init, self.mainBodyChunk);
                            self.slugOnBack.SlugToBack(self.pickUpCandidate as Player);
                        }
                        else
                        {
                            int num23 = 0;
                            for (int num24 = 0; num24 < 2; num24++)
                            {
                                if (self.grasps[num24] == null)
                                {
                                    num23++;
                                }
                            }
                            if (self.Grabability(self.pickUpCandidate) == Player.ObjectGrabability.TwoHands && num23 < 4)
                            {
                                for (int num25 = 0; num25 < 2; num25++)
                                {
                                    if (self.grasps[num25] != null)
                                    {
                                        self.ReleaseGrasp(num25);
                                    }
                                }
                            }
                            else if (num23 == 0)
                            {
                                for (int num26 = 0; num26 < 2; num26++)
                                {
                                    if (self.grasps[num26] != null && self.grasps[num26].grabbed is Fly)
                                    {
                                        self.ReleaseGrasp(num26);
                                        break;
                                    }
                                }
                            }
                            int num27 = 0;
                            while (num27 < 2)
                            {
                                if (self.grasps[num27] == null)
                                {
                                    if (self.pickUpCandidate is Creature)
                                    {
                                        self.room.PlaySound(SoundID.Slugcat_Pick_Up_Creature, self.pickUpCandidate.firstChunk, false, 1f, 1f);
                                    }
                                    else if (self.pickUpCandidate is PlayerCarryableItem)
                                    {
                                        for (int num28 = 0; num28 < self.pickUpCandidate.grabbedBy.Count; num28++)
                                        {
                                            if (self.pickUpCandidate.grabbedBy[num28].grabber.room == self.pickUpCandidate.grabbedBy[num28].grabbed.room)
                                            {
                                                self.pickUpCandidate.grabbedBy[num28].grabber.GrabbedObjectSnatched(self.pickUpCandidate.grabbedBy[num28].grabbed, self);
                                            }
                                            else
                                            {
                                                string str = "Item theft room mismatch? ";
                                                AbstractPhysicalObject abstractPhysicalObject2 = self.pickUpCandidate.grabbedBy[num28].grabbed.abstractPhysicalObject;
                                                Debug.Log(str + ((abstractPhysicalObject2 != null) ? abstractPhysicalObject2.ToString() : null));
                                            }
                                            self.pickUpCandidate.grabbedBy[num28].grabber.ReleaseGrasp(self.pickUpCandidate.grabbedBy[num28].graspUsed);
                                        }
                                        (self.pickUpCandidate as PlayerCarryableItem).PickedUp(self);
                                    }
                                    else
                                    {
                                        self.room.PlaySound(SoundID.Slugcat_Pick_Up_Misc_Inanimate, self.pickUpCandidate.firstChunk, false, 1f, 1f);
                                    }
                                    self.SlugcatGrab(self.pickUpCandidate, num27);
                                    if (self.pickUpCandidate.graphicsModule != null && self.Grabability(self.pickUpCandidate) < (Player.ObjectGrabability)5)
                                    {
                                        self.pickUpCandidate.graphicsModule.BringSpritesToFront();
                                        break;
                                    }
                                    break;
                                }
                                else
                                {
                                    num27++;
                                }
                            }
                        }
                        self.wantToPickUp = 0;
                    }
                }
            }
            else
            {
                orig(self, eu);
            }
        }
        private static void PlayerGraphics_Update(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            // match for `player.objectInStomach != null`
            c.GotoNext(MoveType.After,
                i => i.MatchLdarg(0),
                i => i.MatchLdfld<PlayerGraphics>("player"),
                i => i.MatchLdfld<Player>("objectInStomach"),
                i => i.Match(OpCodes.Brtrue_S)
            );

            // match for `player.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Gourmand ...`
            c.GotoNext(MoveType.After,
                i => i.MatchLdarg(0),
                i => i.MatchLdfld<PlayerGraphics>("player"),
                i => i.MatchLdfld<Player>("SlugCatClass"),
                i => i.MatchLdsfld<MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName>("Gourmand"),
                i => i.Match(OpCodes.Call), // this is a mess of generics; not matching this, but it's the equation call
                i => i.Match(OpCodes.Brtrue_S)
            );

            ILLabel proceedCond = c.Prev.Operand as ILLabel;

            // insert our condition
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<PlayerGraphics, bool>>(playerGraphics =>
            {
                return playerGraphics.player.SlugCatClass.value == "Gravelslug";
            });
            // if it's true, proceed as usual
            c.Emit(OpCodes.Brtrue_S, proceedCond);
        }
        private void DaddyDeadLeg_ApplyPalette(On.DaddyGraphics.DaddyDeadLeg.orig_ApplyPalette orig, DaddyGraphics.DaddyDeadLeg self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            orig(self, sLeaser, rCam, palette);
            Color color = palette.blackColor;
            if (self.owner.owner.room.game.IsStorySession && self.owner.owner.room.world.game.GetStorySession.saveStateNumber.value == "Gravelslug")
            {
                if (self.owner.daddy.HDmode)
                {
                    color = Color.Lerp(PlayerGraphics.DefaultSlugcatColor(self.owner.owner.room.world.game.GetStorySession.characterStats.name), Color.gray, 0.4f);
                }
                for (int i = 0; i < (sLeaser.sprites[self.firstSprite] as TriangleMesh).vertices.Length; i++)
                {
                    float floatPos = Mathf.InverseLerp(0.3f, 1f, (float)i / (float)((sLeaser.sprites[self.firstSprite] as TriangleMesh).vertices.Length - 1));
                    (sLeaser.sprites[self.firstSprite] as TriangleMesh).verticeColors[i] = Color.Lerp(color, self.owner.EffectColor, self.OnTubeEffectColorFac(floatPos));
                }
                int num = 0;
                for (int j = 0; j < self.bumps.Length; j++)
                {
                    sLeaser.sprites[self.firstSprite + 1 + j].color = Color.Lerp(color, self.owner.EffectColor, self.OnTubeEffectColorFac(self.bumps[j].pos.y));
                    if (self.bumps[j].eyeSize > 0f)
                    {
                        sLeaser.sprites[self.firstSprite + 1 + self.bumps.Length + num].color = (self.owner.colorClass ? (self.owner.EffectColor * Mathf.Lerp(0.5f, 0.2f, self.deadness)) : color);
                        num++;
                    }
                }
            }
        }

        private void DaddyDangleTube_ApplyPalette(On.DaddyGraphics.DaddyDangleTube.orig_ApplyPalette orig, DaddyGraphics.DaddyDangleTube self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            orig(self, sLeaser, rCam, palette);
            Color color = palette.blackColor;
            if (self.owner.owner.room.game.IsStorySession && self.owner.owner.room.world.game.GetStorySession.saveStateNumber.value == "Gravelslug")
            {
                if (self.owner.daddy.HDmode)
                {
                    color = Color.Lerp(PlayerGraphics.DefaultSlugcatColor(self.owner.owner.room.world.game.GetStorySession.characterStats.name), Color.gray, 0.4f);
                }
                for (int i = 0; i < (sLeaser.sprites[self.firstSprite] as TriangleMesh).vertices.Length; i++)
                {
                    float floatPos = Mathf.InverseLerp(0.3f, 1f, (float)i / (float)((sLeaser.sprites[self.firstSprite] as TriangleMesh).vertices.Length - 1));
                    (sLeaser.sprites[self.firstSprite] as TriangleMesh).verticeColors[i] = Color.Lerp(color, self.owner.EffectColor, self.OnTubeEffectColorFac(floatPos));
                }
                sLeaser.sprites[self.firstSprite].color = color;
                for (int j = 0; j < self.bumps.Length; j++)
                {
                    sLeaser.sprites[self.firstSprite + 1 + j].color = Color.Lerp(color, self.owner.EffectColor, self.OnTubeEffectColorFac(self.bumps[j].pos.y));
                }
            }
        }

        private void DaddyTubeGraphic_ApplyPalette(On.DaddyGraphics.DaddyTubeGraphic.orig_ApplyPalette orig, DaddyGraphics.DaddyTubeGraphic self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            orig(self, sLeaser, rCam, palette);
            Color color = palette.blackColor;
            if (self.owner.owner.room.game.IsStorySession && self.owner.owner.room.world.game.GetStorySession.saveStateNumber.value == "Gravelslug")
            {
                if (self.owner.daddy.HDmode)
                {
                    color = Color.Lerp(PlayerGraphics.DefaultSlugcatColor(self.owner.owner.room.world.game.GetStorySession.characterStats.name), Color.gray, 0.4f);
                }
                for (int i = 0; i < (sLeaser.sprites[self.firstSprite] as TriangleMesh).vertices.Length; i++)
                {
                    float floatPos = Mathf.InverseLerp(0.3f, 1f, (float)i / (float)((sLeaser.sprites[self.firstSprite] as TriangleMesh).vertices.Length - 1));
                    (sLeaser.sprites[self.firstSprite] as TriangleMesh).verticeColors[i] = Color.Lerp(color, self.owner.EffectColor, self.OnTubeEffectColorFac(floatPos));
                }
                int num = 0;
                for (int j = 0; j < self.bumps.Length; j++)
                {
                    sLeaser.sprites[self.firstSprite + 1 + j].color = Color.Lerp(color, self.owner.EffectColor, self.OnTubeEffectColorFac(self.bumps[j].pos.y));
                    if (self.bumps[j].eyeSize > 0f)
                    {
                        sLeaser.sprites[self.firstSprite + 1 + self.bumps.Length + num].color = (self.owner.colorClass ? self.owner.EffectColor : color);
                        num++;
                    }
                }
            }
        }

        private void DaddyGraphics_ApplyPalette(On.DaddyGraphics.orig_ApplyPalette orig, DaddyGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            orig(self, sLeaser, rCam, palette);
            if (self.owner.room.game.IsStorySession && self.owner.room.world.game.GetStorySession.saveStateNumber.value == "Gravelslug")
            {
                for (int i = 0; i < self.daddy.bodyChunks.Length; i++)
                {
                    if (self.daddy.HDmode)
                    {
                        sLeaser.sprites[self.BodySprite(i)].color = Color.Lerp(PlayerGraphics.DefaultSlugcatColor(self.owner.room.world.game.GetStorySession.characterStats.name), Color.gray, 0.4f);
                    }
                    else
                    {
                        sLeaser.sprites[self.BodySprite(i)].color = self.blackColor;
                    }
                }
            }
        }

        private void HunterDummy_ApplyPalette(On.DaddyGraphics.HunterDummy.orig_ApplyPalette orig, DaddyGraphics.HunterDummy self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            orig(self, sLeaser, rCam, palette);
            if (self.owner.owner.room.game.IsStorySession && self.owner.owner.room.world.game.GetStorySession.saveStateNumber.value == "Gravelslug")
            {
                Color color = Color.Lerp(PlayerGraphics.DefaultSlugcatColor(self.owner.owner.room.world.game.GetStorySession.characterStats.name), Color.gray, 0.4f);
                for (int i = 0; i < self.numberOfSprites - 1; i++)
                {
                    sLeaser.sprites[self.startSprite + i].color = color;
                }
                sLeaser.sprites[self.startSprite + 5].color = Color.cyan;
            }
        }
        private void EggBug_ctor(On.EggBug.orig_ctor orig, EggBug self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (self.FireBug && (world.game.IsStorySession && world.game.GetStorySession.saveStateNumber.value == "Gravelslug") || (world.game.IsArenaSession && abstractCreature.Room.name == "comatose"))
            {
                self.hue = 1f;
            }
        }

        private void OverWorld_WorldLoaded(On.OverWorld.orig_WorldLoaded orig, OverWorld self)
        {
            orig(self);
            if (self.currentSpecialWarp == OverWorld.SpecialWarpType.WARP_VS_HR && self.game.GetStorySession.saveStateNumber.value == "Gravelslug")
            {
                MoreSlugcats.MSCRoomSpecificScript.RoomWarp(self.game.FirstRealizedPlayer, self.game.FirstRealizedPlayer.room, "HR_GRAVINTRO", default(WorldCoordinate), true);
            }
        }

        private bool RegionGate_customOEGateRequirements(On.RegionGate.orig_customOEGateRequirements orig, RegionGate self)
        {
            if ((self.room.world.game.GetStorySession.saveStateNumber.value == "Gravelslug"))
            {
                return true;
            }
            return orig.Invoke(self);
        }

        private void Weapon_Thrown(On.Weapon.orig_Thrown orig, Weapon self, Creature thrownBy, UnityEngine.Vector2 thrownPos, UnityEngine.Vector2? firstFrameTraceFromPos, RWCustom.IntVector2 throwDir, float frc, bool eu)
        {
            orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
            if (thrownBy is Player)
            {
                if (IsGravelFeral(thrownBy as Player))
                {
                    self.firstChunk.vel *= 1.17f;
                    GravelCough(thrownBy, true);
                    if (self is Spear && (self as Spear).bugSpear)
                    {
                        self.room.AddObject(new ShockWave(thrownBy.bodyChunks[1].pos, 300f, 0.05f, 5, false));
                        Debug.Log("Super Armor Piercing Throw!");
                    }
                }
            }
            else
            {
                return;
            }
        }

        private void Player_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self)
        {
            bool flag = self.canCorridorJump > 0;
            orig(self);
            if (IsGravelFeral(self) && self.bodyMode == Player.BodyModeIndex.CorridorClimb)
            {
                /*if(self.bodyMode == Player.BodyModeIndex.ZeroG && self.animation == Player.AnimationIndex.ZeroGSwim)
                {
                    if(self.canJump < 5 && self.input[0].jmp && !self.input[1].jmp)
                    {
                        self.mainBodyChunk.vel = self.mainBodyChunk.vel * 2f;
                        GravelCough(self, true);
                    }
                }*/
                if((self.horizontalCorridorSlideCounter == 25 || self.verticalCorridorSlideCounter == 22) && flag && self.input[0].jmp && !self.input[1].jmp)
                {
                    self.mainBodyChunk.vel = self.mainBodyChunk.vel * 1.6f;
                    GravelCough(self, true);
                }
            }
        }

        private void Player_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);
            if (IsGravelFeral(self))
            {
                if (self.jumpBoost == 6)
                {
                    self.mainBodyChunk.vel *= 1.5f;
                    self.mainBodyChunk.vel.x *= 2.6f;
                    GravelCough(self, true);
                }

                if (self.animation == Player.AnimationIndex.GrapplingSwing || self.animation == Player.AnimationIndex.Flip || self.animation == Player.AnimationIndex.RocketJump || self.whiplashJump)
                {
                    self.jumpBoost *= 2.2f;
                    if (self.animation == Player.AnimationIndex.RocketJump || self.whiplashJump)
                    {
                        self.mainBodyChunk.vel *= 1.6f;
                    }
                    /*if (self.animation == Player.AnimationIndex.ZeroGSwim)
                    {
                        self.mainBodyChunk.vel *= 1.1f;
                    }*/
                    GravelCough(self, true);
                }

                //coyote jump boost
                if (self.canJump < 10 && self.bodyMode == Player.BodyModeIndex.Default && self.animation == Player.AnimationIndex.None && self.standing && !(self.IsTileSolid(0, 0, -1) || self.IsTileSolid(1, 0, -1)))
                {
                    Debug.Log("coyote jump boosted with player.canJump " + self.canJump);
                    self.jumpBoost *= 1.4f;
                    self.mainBodyChunk.vel.x *= 2.5f;
                    self.animation = Player.AnimationIndex.Flip;
                    GravelCough(self, true);
                }
            }
        }

        private void Player_JumpOnChunk(On.Player.orig_JumpOnChunk orig, Player self)
        {
            bool flag = self.jumpChunk != null && self.jumpChunk.owner.room != self.room && self.jumpChunk.owner is Creature && !(self.jumpChunk.owner as Creature).dead;
            orig(self);
            if (IsGravelFeral(self))
            {
                if(flag
                        //&& self.jumpChunk == (self.jumpChunk.owner as Creature).firstChunk
                    )
                {
                    self.jumpBoost *= 2f;
                    self.animation = Player.AnimationIndex.Flip;
                    GravelCough(self, true);
                }
            }
        }
        private void Player_Die(On.Player.orig_Die orig, Player self)
        {
            bool wasDead = self.dead;

            orig(self);

            if (!wasDead && self.dead
                && GravelGut.TryGet(self, out bool explode)
                && explode)
            {
                /*if(self.abstractCreature != null)
                {
                    self.abstractCreature.tentacleImmune = false;
                }*/
                StartCoroutine(VoidFireSpasm(self, true, false));
                if (GravelEaten != null && !GravelEaten.dead)
                {
                    GravelEaten.Die();
                }
            }
        }
        private void Player_SwallowObject(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            orig(self, grasp);
            bool flag = false;
            bool flag2 = false;
            if (GravelGut.TryGet(self, out bool tum) && tum && self.input[0].y > 0 && self.FoodInStomach > 0)
            {
                if (self.objectInStomach.type != AbstractPhysicalObject.AbstractObjectType.ScavengerBomb
                    && self.objectInStomach.type != AbstractPhysicalObject.AbstractObjectType.Spear
                    && self.objectInStomach.type != MoreSlugcatsEnums.AbstractObjectType.SingularityBomb)
                {
                    GravelDissolveCraft(self);
                    return;
                }
                else if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.Spear)
                {
                    flag = true;
                }
                flag2 = true;
            }
            if (tum && ((self.FoodInStomach < self.MaxFoodInStomach) || (ModManager.ModdingEnabled && GravelFatty)) || flag2)
            {
                if(self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.NSHSwarmer
                    || self.objectInStomach.type == MoreSlugcatsEnums.AbstractObjectType.Spearmasterpearl)
                {
                    self.Regurgitate();
                }
                if (self.FoodInStomach >= self.MaxFoodInStomach)
                {
                    GravelDissolveReset(self.room.game);
                }
                GravelDissolveAdd(20f);
                if (self.objectInStomach != null)
                {
                    if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.Rock
                    || self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.WaterNut)
                    {
                        //self.AddFood(1);
                        self.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.Duck_Pop, self.firstChunk.pos, 1f, 1f + UnityEngine.Random.value);
                        GravelBiteParticle(self, 1);
                        if (self.room.game.IsArenaSession)
                        {
                            self.AddFood(1);
                        }
                        else
                        {
                            self.AddQuarterFood();
                            self.AddQuarterFood();
                            self.AddQuarterFood();
                        }
                    }
                    else if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.DataPearl
                        || self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.PebblesPearl
                        || self.objectInStomach.type == MoreSlugcatsEnums.AbstractObjectType.HalcyonPearl
                        || self.objectInStomach.type == MoreSlugcatsEnums.AbstractObjectType.Spearmasterpearl
                        || self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.OverseerCarcass
                        || self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.Spear)
                    {
                        if (self.objectInStomach is AbstractSpear s)
                        {
                            if (!flag)
                            {
                                self.room.PlaySound(SoundID.Spear_Bounce_Off_Creauture_Shell, self.firstChunk.pos, 1f, 1.5f + UnityEngine.Random.value);
                                GravelBiteParticle(self, 2);
                            }
                            if (s.electric && s.electricCharge == 0)
                            {
                                if (!self.room.game.IsArenaSession)
                                {
                                    self.AddFood(1);
                                    self.AddQuarterFood();
                                    self.AddQuarterFood();
                                }
                                else
                                {
                                    self.AddFood(2);
                                }
                            }
                            else
                            {
                                if ((s.electric && s.electricCharge > 0) || s.explosive)
                                {
                                    if (s.electric)
                                    {
                                        self.room.AddObject(new ZapCoil.ZapFlash(self.firstChunk.pos, 10f));
                                        self.room.PlaySound(SoundID.Zapper_Zap, self.firstChunk.pos, 1f, 1.5f + UnityEngine.Random.value * 1.5f);
                                        if (self.Submersion > 0.5f)
                                        {
                                            self.room.AddObject(new UnderwaterShock(self.room, null, self.firstChunk.pos, 10, 800f, 2f, self, new Color(0.8f, 0.8f, 1f)));
                                        }
                                        self.Stun(200);
                                        self.room.AddObject(new CreatureSpasmer(self, true, 200));
                                    }
                                    else if (s.explosive)
                                    {
                                        self.room.PlaySound(SoundID.Firecracker_Disintegrate, self.firstChunk.pos, 0.6f, UnityEngine.Random.Range(0.8f, 1.2f));
                                    }
                                    self.Die();
                                }
                            }
                            if (flag)
                            {
                                GravelDissolveCraft(self);
                                return;
                            }
                            else
                            {

                            }
                            if (!s.needle)
                            {
                                self.AddFood(1);
                            }
                        }
                        else
                        {
                            self.AddFood(1);
                            self.room.PlaySound(SoundID.Snail_Warning_Click, self.firstChunk.pos, 1f, 1.5f + UnityEngine.Random.value);
                            if (self.objectInStomach.type != AbstractPhysicalObject.AbstractObjectType.OverseerCarcass)
                            {
                                //make scavs angry
                                for (int i = 0; i < self.room.abstractRoom.creatures.Count; i++)
                                {
                                    if (self.room.abstractRoom.creatures[i].creatureTemplate.type == CreatureTemplate.Type.Scavenger && self.room.abstractRoom.creatures[i].realizedCreature != null && self.room.abstractRoom.creatures[i].realizedCreature.Consious)
                                    {
                                        float num = self.room.game.session.creatureCommunities.LikeOfPlayer(CreatureCommunities.CommunityID.Scavengers, self.room.game.world.RegionNumber, self.playerState.playerNumber);
                                        if (num < 0.9f)
                                        {
                                            self.room.game.session.creatureCommunities.InfluenceLikeOfPlayer(CreatureCommunities.CommunityID.Scavengers, self.room.game.world.RegionNumber, self.playerState.playerNumber, RWCustom.Custom.LerpMap(num, -0.5f, 0.9f, -0.3f, 0f), 0.5f, 0f);
                                            Debug.Log("pearl desecration noticed!");
                                        }
                                    }
                                }
                            }
                            GravelBiteParticle(self, 0);
                        }
                    }
                    else if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.ScavengerBomb
                        || self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant
                        || self.objectInStomach.type == MoreSlugcatsEnums.AbstractObjectType.SingularityBomb)
                    {
                        if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.ScavengerBomb
                        || self.objectInStomach.type == MoreSlugcatsEnums.AbstractObjectType.SingularityBomb)
                        {
                            self.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.Duck_Pop, self.firstChunk.pos, 1f, 1f + UnityEngine.Random.value);
                            GravelBiteParticle(self, 1);
                        }
                        if (self.objectInStomach.type == MoreSlugcatsEnums.AbstractObjectType.SingularityBomb)
                        {
                            AbstractPhysicalObject abstractPhysicalObject = new(self.abstractCreature.Room.world, MoreSlugcatsEnums.AbstractObjectType.SingularityBomb, null, self.room.GetWorldCoordinate(self.mainBodyChunk.pos), self.abstractCreature.Room.world.game.GetNewID());
                            abstractPhysicalObject.RealizeInRoom();
                            if (self.room != null)
                            {
                                self.room.PlaySound(SoundID.Slugcat_Throw_Bomb, (abstractPhysicalObject.realizedObject as SingularityBomb).firstChunk);
                            }
                            (abstractPhysicalObject.realizedObject as SingularityBomb).thrownBy = self;
                            (abstractPhysicalObject.realizedObject as SingularityBomb).ignited = true;
                            (abstractPhysicalObject.realizedObject as SingularityBomb).CreateFear();
                        }
                        else
                        {
                            if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.ScavengerBomb)
                            {
                                AbstractPhysicalObject abstractPhysicalObject = new AbstractConsumable(self.room.world, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null, self.abstractCreature.pos, self.room.game.GetNewID(), -1, -1, null);
                                abstractPhysicalObject.RealizeInRoom();
                                (abstractPhysicalObject.realizedObject as ScavengerBomb).thrownBy = self;
                                (abstractPhysicalObject.realizedObject as ScavengerBomb).Explode(self.firstChunk);
                            }
                            else
                            {
                                self.room.PlaySound(SoundID.Firecracker_Disintegrate, self.firstChunk.pos, 0.6f, UnityEngine.Random.Range(0.8f, 1.2f));
                            }
                            self.Die();
                        }
                    }
                    else if (self.objectInStomach is AbstractCreature)
                    {
                        self.room.PlaySound(SoundID.Vulture_Tentacle_Grab_Terrain, self.firstChunk.pos, 1f, 1.5f + UnityEngine.Random.value);
                        if (!self.room.game.rainWorld.ExpeditionMode && !self.room.game.IsArenaSession && self.room.game.GetStorySession.saveState.cycleNumber != 0 && self.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW] < -4)
                        {
                            for (int i = 0; i > (self.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW] + 4); i--)
                            {
                                self.AddQuarterFood();
                            }
                        }
                        else
                        {
                            self.AddFood(1);
                        }
                    }
                    else
                    {
                        if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.FlyLure)
                        {
                            GravelDissolveAdd(40f);
                        }
                        if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.PuffBall)
                        {
                            AbstractPhysicalObject abstractPhysicalObject = new AbstractPhysicalObject(self.abstractCreature.Room.world, AbstractPhysicalObject.AbstractObjectType.PuffBall, null, self.room.GetWorldCoordinate(self.mainBodyChunk.pos), self.abstractCreature.Room.world.game.GetNewID());
                            abstractPhysicalObject.RealizeInRoom();
                            (abstractPhysicalObject.realizedObject as PuffBall).Explode();
                            self.Stun(15);
                        }
                        /*if (GravelVinki && self.objectInStomach.type == )
                        {
                            AbstractPhysicalObject abstractPhysicalObject = new AbstractPhysicalObject(self.abstractCreature.Room.world, AbstractPhysicalObject.AbstractObjectType.PuffBall, null, self.room.GetWorldCoordinate(self.mainBodyChunk.pos), self.abstractCreature.Room.world.game.GetNewID());
                            abstractPhysicalObject.RealizeInRoom();
                            (abstractPhysicalObject.realizedObject as Spraycan).Explode();
                            self.Stun(15);
                        }*/
                        if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.NeedleEgg)
                        {
                            AbstractCreature abstractCreature;
                            abstractCreature = new AbstractCreature(self.abstractCreature.Room.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.SmallNeedleWorm), null, self.room.GetWorldCoordinate(self.mainBodyChunk.pos), self.abstractCreature.Room.world.game.GetNewID());
                            abstractCreature.RealizeInRoom();
                            abstractCreature.realizedCreature.dead = true;
                        }
                        if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.FlareBomb)
                        {
                            AbstractPhysicalObject abstractFlareBomb = self.objectInStomach;
                            self.Regurgitate();
                            (abstractFlareBomb.realizedObject as FlareBomb).StartBurn();
                            self.Stun(15);
                        }
                        if (self.room.game.IsArenaSession)
                        {
                            self.AddFood(1);
                        }
                        else
                        {
                            self.AddQuarterFood();
                        }
                        self.room.PlaySound(SoundID.Vulture_Tentacle_Grab_Terrain, self.firstChunk.pos, 1f, 1.5f + UnityEngine.Random.value);
                    }
                    Debug.Log("Player ate " + self.objectInStomach.type);
                    //self.SessionRecord.AddEat(self.objectInStomach.realizedObject);
                    self.objectInStomach = null;
                }
            }
        }

        private void GravelBiteParticle(Creature player, int type)
        {
            if(type == 1)
            {
                int amount = UnityEngine.Random.Range(1, 4);
                for (int i = 0; i < amount; i++)
                {
                    player.room.AddObject(new ScavengerBomb.BombFragment(player.firstChunk.pos, RWCustom.Custom.RNV() * Mathf.Lerp(1f, 5f, UnityEngine.Random.value)));
                }
            }
            else if (type == 2)
            {
                player.room.AddObject(new ExplosiveSpear.SpearFragment(player.firstChunk.pos, RWCustom.Custom.RNV() * Mathf.Lerp(1f, 5f, UnityEngine.Random.value)));
            }
            for (int i = 0; i < 3; i++)
            {
                player.room.AddObject(new WaterDrip(player.firstChunk.pos, RWCustom.Custom.DegToVec(UnityEngine.Random.value * 360f) * Mathf.Lerp(1f, 10f, UnityEngine.Random.value), false));
            }
        }
        private void SLOracleBehaviorHasMark_SpecialEvent(On.SLOracleBehaviorHasMark.orig_SpecialEvent orig, SLOracleBehaviorHasMark self, string eventName)
        {
            orig(self, eventName);
            if(self.oracle.room.world.game.GetStorySession.saveStateNumber.value == "Gravelslug")
            {
                if (eventName == "RivScreen1")
                {
                    self.rivEnding.displayImage = self.oracle.myScreen.AddImage("AIimg2_DM");
                }
            }
        }
        private void CLOracleBehavior_InitateConversation(On.MoreSlugcats.CLOracleBehavior.orig_InitateConversation orig, MoreSlugcats.CLOracleBehavior self)
        {
            orig(self);
            if (self.oracle.room.world.game.GetStorySession.saveStateNumber.value == "Gravelslug")
            {
                self.dialogBox.Interrupt(self.Translate("..."), 200);
                if (self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.halcyonStolen)
                {
                    if (UnityEngine.Random.value < 0.15f)
                    {
                        self.dialogBox.NewMessage(self.Translate("Why have you taken all i have?"), 60);
                        return;
                    }
                    if (UnityEngine.Random.value < 0.15f)
                    {
                        self.dialogBox.NewMessage(self.Translate("Give it to me."), 60);
                        return;
                    }
                    self.dialogBox.NewMessage(self.Translate("Please bring it back to me."), 60);
                    return;
                }
                else if (self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiThrowOuts > 0)
                {
                    if (UnityEngine.Random.value < 0.15f)
                    {
                        self.dialogBox.NewMessage(self.Translate("Go away"), 60);
                        return;
                    }
                    if (UnityEngine.Random.value < 0.15f)
                    {
                        self.dialogBox.NewMessage(self.Translate("I have not forgotten the pain you have given me."), 60);
                        return;
                    }
                    if (UnityEngine.Random.value < 0.15f)
                    {
                        self.dialogBox.NewMessage(self.Translate("I have so little left. Why is it that you hurt me?"), 60);
                        return;
                    }
                    self.dialogBox.NewMessage(self.Translate("Please leave me alone."), 60);
                    return;
                }
                else
                {
                    if (self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad == 1)
                    {
                        self.dialogBox.NewMessage(self.Translate("Oh, little experiment!"), 60);
                        self.dialogBox.NewMessage(self.Translate("..."), 60);
                        self.dialogBox.NewMessage(self.Translate("I... don't even know what to say."), 60);
                        self.dialogBox.NewMessage(self.Translate("Hm? You can understand me, can't you?"), 60);
                        self.dialogBox.NewMessage(self.Translate("..."), 60);
                        self.dialogBox.NewMessage(self.Translate("Some solution you were while in that vegetative state."), 60);
                        self.dialogBox.NewMessage(self.Translate("I tried so hard to wake you; you were my last hope."), 60);
                        self.dialogBox.NewMessage(self.Translate("..."), 60);
                        self.dialogBox.NewMessage(self.Translate("But who am I to put such trust in a simple creature?"), 60);
                        self.dialogBox.NewMessage(self.Translate("There is nothing you can do now, nothing left to save."), 60);
                        self.dialogBox.NewMessage(self.Translate("Though I would enjoy the company for a while longer before you move on, if you please."), 60);
                        self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad++;
                        return;
                    }
                    if (self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad == 2)
                    {
                        self.dialogBox.NewMessage(self.Translate("Hello again, little one."), 60);
                        self.dialogBox.NewMessage(self.Translate("I bet you're wondering how it came to be that you are my creation."), 60);
                        self.dialogBox.NewMessage(self.Translate("I took note of the persistence and... annoyance... of your kind repeatedly entering the floor of my chamber."), 60);
                        self.dialogBox.NewMessage(self.Translate("No easy feat for just any creature, might I add."), 60);
                        self.dialogBox.NewMessage(self.Translate("So, I deep-scanned one for myself to create a template and used the DNA scan to replicate my own."), 60);
                        self.dialogBox.NewMessage(self.Translate("A method used by some of my old peers, you see..."), 60);
                        self.dialogBox.NewMessage(self.Translate("You were the most promising iteration by far. A good balance between Void Fluid and enhanced regrowing tissue to counteract."), 60);
                        self.dialogBox.NewMessage(self.Translate("However, your brain activity was minimal, and any stimulus I could apply was ineffective."), 60);
                        self.dialogBox.NewMessage(self.Translate("I'm almost certain I would have been able to wake you if my equipment had been in a better state."), 60);
                        self.dialogBox.NewMessage(self.Translate("I lost contact with that sector soon after, however..."), 60);
                        self.dialogBox.NewMessage(self.Translate("I guess the cryo chamber held up even after I collapsed, which is quite the surprise."), 60);
                        self.dialogBox.NewMessage(self.Translate("Well... Welcome to the waking, rotting world."), 60);

                        self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad++;
                        return;
                    }
                    if (UnityEngine.Random.value < 0.15f)
                    {
                        self.dialogBox.NewMessage(self.Translate("Why is it that you return?"), 60);
                        self.dialogBox.NewMessage(self.Translate("There is nothing here for you."), 60);
                        self.dialogBox.NewMessage(self.Translate("Though it is nice having someone to talk to."), 60);
                        self.dialogBox.NewMessage(self.Translate("..."), 60);
                        self.dialogBox.NewMessage(self.Translate("Being alone with your own thoughts can drive you mad."), 60);
                        self.dialogBox.NewMessage(self.Translate("I compute several complex processes at once to distract myself."), 60);
                        self.dialogBox.NewMessage(self.Translate("That and this hymn that the ancients used to play..."), 60);
                        self.dialogBox.NewMessage(self.Translate("..."), 60);
                        self.dialogBox.NewMessage(self.Translate("Even if it's not much, thank you for listening to me."), 60);
                        return;
                    }
                    if (UnityEngine.Random.value < 0.15f)
                    {
                        self.dialogBox.NewMessage(self.Translate("It has become noticeably colder as of late."), 60);
                        self.dialogBox.NewMessage(self.Translate("You should go west where it is warmer."), 60);
                        self.dialogBox.NewMessage(self.Translate("You might even find more beings similar to yourself."), 60);
                        self.dialogBox.NewMessage(self.Translate("..."), 60);
                        self.dialogBox.NewMessage(self.Translate("Was... that a glint of emotion?"), 60);
                        self.dialogBox.NewMessage(self.Translate("Guess I finally cracked the code to that poker face."), 60);
                        return;
                    }
                    if (UnityEngine.Random.value < 0.15f)
                    {
                        self.dialogBox.NewMessage(self.Translate("Little experiment. Hello."), 60);
                        self.dialogBox.NewMessage(self.Translate("That scan I took was near perfect, no abnormalities."), 60);
                        self.dialogBox.NewMessage(self.Translate("Plenty of room to work with."), 60);
                        self.dialogBox.NewMessage(self.Translate("The organisms I made had inherited a few colors that strayed from the expected white tone."), 60);
                        self.dialogBox.NewMessage(self.Translate("If it wasn't white, it was usually yellow."), 60);
                        self.dialogBox.NewMessage(self.Translate("But you, continuing to be an oddity, turned out to be a blue shade."), 60);
                        self.dialogBox.NewMessage(self.Translate("Must have been a recessive trait."), 60);
                        return;
                    }
                    if (UnityEngine.Random.value < 0.15f)
                    {
                        self.dialogBox.NewMessage(self.Translate("Nice to see that you are in good health."), 60);
                        self.dialogBox.NewMessage(self.Translate("Or... at least in an alive way."), 60);
                        self.dialogBox.NewMessage(self.Translate("Though I imagine true death would be preferred to constant dissolving of your flesh."), 60);
                        self.dialogBox.NewMessage(self.Translate("That void fluid in your stomach is problematic if not tended to, as I'm sure you are already aware."), 60);
                        self.dialogBox.NewMessage(self.Translate("The sheer fact you are alive means that you have found a sustainable solution."), 60);
                        self.dialogBox.NewMessage(self.Translate("Or a way to temporarily stave off the pain."), 60);
                        self.dialogBox.NewMessage(self.Translate("You creatures always seem to find a way..."), 60);
                        return;
                    }
                    self.dialogBox.NewMessage(self.Translate("Thank you for the company."), 60);
                    self.dialogBox.NewMessage(self.Translate("I've been alone since my collapse."), 60);
                    self.dialogBox.NewMessage(self.Translate("I treasured the time I had with my close neighbor, Moon."), 60);
                    self.dialogBox.NewMessage(self.Translate("Big sis..."), 60);
                    self.dialogBox.NewMessage(self.Translate("Say hello for me if you visit her, if you haven't already."), 60);
                    self.dialogBox.NewMessage(self.Translate("She's not in much better condition than I am."), 60);

                    return;
                }
            }
        }
        private string[] SlugcatStats_getSlugcatStoryRegions(On.SlugcatStats.orig_getSlugcatStoryRegions orig, SlugcatStats.Name i)
        {
            if (i.value == "Gravelslug")
            {
                return new string[]
                {
            "SU",
            "HI",
            "DS",
            "CC",
            "GW",
            "VS",
            "CL",
            "SL",
            "SI",
            "LF",
            "SB"
                };
            }
            else
            {
                return orig(i);
            }
        }
        private string[] SlugcatStats_getSlugcatOptionalRegions(On.SlugcatStats.orig_getSlugcatOptionalRegions orig, SlugcatStats.Name i)
        {
            if (i.value == "Gravelslug")
            {
                return new string[]
                {
                "OE",
                "MS"
                };
            }
            else
            {
                return orig(i);
            }
        }
        private void Player_SleepUpdate(On.Player.orig_SleepUpdate orig, Player self)
        {
            orig(self);
            if (self.slugcatStats.name.value == "Gravelslug")
            {
                if (self.sleepCounter == 10)
                {
                    TummyItem(self);
                }
            }
        }
        private void PlayerGraphics_Update(On.PlayerGraphics.orig_Update orig, PlayerGraphics self)
        {
            orig(self);
            if (self.player.slugcatStats.name.value == "Gravelslug")
            {
                self.markAlpha = 0f;
                if(self.player.room.game.IsStorySession && self.player.room.game.GetStorySession.characterStats.name.value == "Gravelslug")
                {
                    self.lastMarkAlpha = 0f;
                }
                if (IsGravelFeral(self.player) && !self.player.dead && self.player.stun < 10)
                {
                    self.drawPositions[0, 0] += RWCustom.Custom.RNV() * UnityEngine.Random.value * 0.4f * 2f;
                    self.drawPositions[0, 1] += RWCustom.Custom.RNV() * UnityEngine.Random.value * 0.4f * 2f;
                    self.head.pos += RWCustom.Custom.RNV() * UnityEngine.Random.value * 0.4f * 1f;
                }
                if (!self.player.dead && self.player.FoodInStomach < self.player.MaxFoodInStomach && self.objectLooker.currentMostInteresting != null && !(self.player.bodyMode == Player.BodyModeIndex.Crawl || self.player.bodyMode == Player.BodyModeIndex.WallClimb || self.player.animation == Player.AnimationIndex.StandOnBeam || self.player.animation == Player.AnimationIndex.AntlerClimb) && self.objectLooker.currentMostInteresting is Rock && RWCustom.Custom.DistLess(self.player.mainBodyChunk.pos, self.objectLooker.mostInterestingLookPoint, 80f) && self.player.room.VisualContact(self.player.mainBodyChunk.pos, self.objectLooker.mostInterestingLookPoint))
                {
                    int num19 = -1;
                    for (int n = 0; n < 2; n++)
                    {
                        if (self.player.grasps[n] == null && self.hands[1 - n].reachedSnapPosition)
                        {
                            num19 = n;
                        }
                    }
                    if (self.objectLooker.currentMostInteresting is Rock && self.player.input[0].x != 0 && self.objectLooker.currentMostInteresting.bodyChunks[0].pos.x < self.player.mainBodyChunk.pos.x == self.player.input[0].x > 0)
                    {
                        num19 = -1;
                    }
                    if (num19 > -1)
                    {
                        self.hands[num19].reachingForObject = true;
                        self.hands[num19].absoluteHuntPos = self.objectLooker.mostInterestingLookPoint;
                        if (self.player.Consious && self.objectLooker.currentMostInteresting != null && !(self.objectLooker.currentMostInteresting is Creature && self.objectLooker.currentMostInteresting is not Fly))
                        {
                            self.drawPositions[0, 0] += RWCustom.Custom.DirVec(self.drawPositions[0, 0], self.objectLooker.mostInterestingLookPoint) * 5f;
                            self.head.vel += RWCustom.Custom.DirVec(self.drawPositions[0, 0], self.objectLooker.mostInterestingLookPoint) * 2f;
                        }
                    }
                }
            }
        }
        public bool GravelThreatCheck(RainCycle self)
        {
            return self.world.game.IsArenaSession || self.TimeUntilRain >= 2400 || (ModManager.MSC && self.world.game.IsStorySession && (self.world.game.GetStorySession.saveStateNumber == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Saint || self.world.game.GetStorySession.saveStateNumber == MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Rivulet || self.world.game.GetStorySession.saveStateNumber.value == "Gravelslug")) || (self.world.game.IsStorySession && self.world.region != null && (self.world.region.name == "SS" || (ModManager.MSC && (self.world.region.name == "RM" || self.world.region.name == "DM" || self.world.region.name == "LC" || self.world.region.name == "OE"))));
        }
        private static void MoonOverride(On.SLOracleBehaviorHasMark.MoonConversation.orig_AddEvents orig, SLOracleBehaviorHasMark.MoonConversation self)
        {
            if (self.currentSaveFile.value == "Gravelslug")
            {
                if (self.id == Conversation.ID.MoonRecieveSwarmer)
                {
                    if (self.State.neuronGiveConversationCounter < 2)
                    {
                        if (self.State.neuronGiveConversationCounter == 0)
                        {
                            Debug.Log("moon recieve first neuron. Has neurons: " + self.State.neuronsLeft.ToString());
                            self.events = new List<Conversation.DialogueEvent>();
                            self.LoadEventsFromFile(130);
                        }
                        else if (self.State.neuronGiveConversationCounter == 1)
                        {
                            self.LoadEventsFromFile(159);
                        }
                        SLOrcacleState state = self.State;
                        int neuronGiveConversationCounter = state.neuronGiveConversationCounter;
                        state.neuronGiveConversationCounter = neuronGiveConversationCounter + 1;
                        return;
                    }
                }
            }
            orig(self);
            if (self.currentSaveFile.value == "Gravelslug")
            {
                if (self.id == Conversation.ID.MoonFirstPostMarkConversation)
                {
                    self.events = new List<Conversation.DialogueEvent>();
                    self.events.Add(new Conversation.TextEvent(self, 0, "Hello Little Creature! You can understand me, can’t you?", 7));
                    self.events.Add(new Conversation.TextEvent(self, 0, "You seem unwell...", 7));
                    self.events.Add(new Conversation.TextEvent(self, 0, "let's see...", 8));
                    self.events.Add(new Conversation.SpecialEvent(self, 2, "RivScreen1"));
                    self.events.Add(new Conversation.TextEvent(self, 0, "Facinating. Your stomach is full of void fluid.", 7));
                    self.events.Add(new Conversation.TextEvent(self, 0, "It's highly unlikely that you gained this adaptation naturally. <LINE>Especially as it seems to give you great discomfort!", 14));
                    self.events.Add(new Conversation.TextEvent(self, 0, "My only assumption is that you were created by another iterator. But who, and what purpose is a mystery to me...", 12));
                    self.events.Add(new Conversation.TextEvent(self, 0, "You can stay as long as you like, the rain isnt as deadly as it once was. <LINE>It however has become a little cold as of late, so i advise you keep warm", 15));
                }
                if (self.id == Conversation.ID.MoonSecondPostMarkConversation)
                {
                    self.events = new List<Conversation.DialogueEvent>();
                    switch (Mathf.Clamp(self.State.neuronsLeft, 0, 7)) //this gets the number of neurons left for moon.
                    {
                        case 1:
                            self.events.Add(new Conversation.TextEvent(self, 0, "...", 7));
                            return;
                        case 2:
                            self.events.Add(new Conversation.TextEvent(self, 0, "...leave...", 7));
                            return;
                        case 3:
                            self.events.Add(new Conversation.TextEvent(self, 0, "You...", 7));
                            self.events.Add(new Conversation.TextEvent(self, 0, "Please don’t... take... more from me... Go.", 7));
                            return;
                        case 4:
                            self.events.Add(new Conversation.TextEvent(self, 0, "Oh. You.", 7));
                            return;
                        case 5:
                            self.events.Add(new Conversation.TextEvent(self, 0, "Oh so you've returned.", 7));
                            self.events.Add(new Conversation.TextEvent(self, 0, "You have taken from me what many like you have gifted.", 7));
                            self.events.Add(new Conversation.TextEvent(self, 0, "It can never be returned.", 7));
                            self.events.Add(new Conversation.TextEvent(self, 0, "Leave with the guilt that you are the worst of your kind.", 7));

                            return;
                        case 6:
                            self.events.Add(new Conversation.TextEvent(self, 0, "Oh so you've returned.", 7));
                            self.events.Add(new Conversation.TextEvent(self, 0, "You have hurt me whether you realize it or not.", 7));
                            self.events.Add(new Conversation.TextEvent(self, 0, "I do not know what you gained from your fruitless efforts of partially consuming my lifeline.", 8));
                            self.events.Add(new Conversation.TextEvent(self, 0, "But please be on your way.", 7));

                            return;
                        case 7:
                            self.events.Add(new Conversation.TextEvent(self, 0, "Oh, Hello!", 7));
                            self.events.Add(new Conversation.TextEvent(self, 0, "As i said there is nothing i can do for you other than wish for your safety.", 8));
                            self.events.Add(new Conversation.TextEvent(self, 0, "You can stay as long as you like. The stars are beautiful at night, but you don't need to stay that long.", 9));
                            self.events.Add(new Conversation.TextEvent(self, 0, "I wish the best for your future travels.", 7));
                            return;
                    }
                }
                if (self.id == Conversation.ID.Moon_Misc_Item)
                {
                    if (ModManager.MMF && self.myBehavior.isRepeatedDiscussion)
                    {
                        return;
                    }
                    if (self.describeItem == SLOracleBehaviorHasMark.MiscItemType.Rock)
                    {
                        self.events = new List<Conversation.DialogueEvent>();
                        self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("It's a rock. Unfortunately, I don't have any tea to go along with your unorthidox diet unfortunately."), 0));
                        return;
                    }
                    if (self.describeItem == SLOracleBehaviorHasMark.MiscItemType.Spear)
                    {
                        self.events = new List<Conversation.DialogueEvent>();
                        self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("It's a piece of sharpened rebar... You seem capable of consuming it.<LINE>Careful not to hurt yourself by attempting to do so!"), 0));
                        return;
                    }
                    if (self.describeItem == MoreSlugcatsEnums.MiscItemType.MoonCloak)
                    {
                        self.events = new List<Conversation.DialogueEvent>();
                        self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("Is this... Void Fluid? On a weapon like this would prove to be deadly.<LINE>Clever use of your condition little one!"), 0));
                        return;
                    }
                }
                    
            }
        }
        private static void GhostOVerride(On.GhostConversation.orig_AddEvents orig, GhostConversation self)
        {
            orig(self);
            if (self.currentSaveFile.value == "Gravelslug") //check if this game has custom conversations
            {
                if (self.id == Conversation.ID.Ghost_SB) //check for which ghost this is
                {
                    self.events = new List<Conversation.DialogueEvent>(); //remove all events already existing
                    self.events.Add(new Conversation.TextEvent(self, 0, "A little beast!", 6)); // Add your own text events!
                    self.events.Add(new Conversation.TextEvent(self, 0, "But are you not quite a beast past that veil?", 7));
                    self.events.Add(new Conversation.TextEvent(self, 0, "I know the struggles you bear, the burdens your vessel embodies.", 7));
                    self.events.Add(new Conversation.TextEvent(self, 0, "An old method may still apply; you are already following some steps.", 7));
                    self.events.Add(new Conversation.TextEvent(self, 0, "The struggles will be unending, but the significance is your choice alone.", 8));
                    self.events.Add(new Conversation.TextEvent(self, 0, "What will you do with this new opportunity?", 7));
                }
                if (self.id == Conversation.ID.Ghost_LF) //check for which ghost this is
                {
                    self.events = new List<Conversation.DialogueEvent>(); //remove all events already existing
                    self.events.Add(new Conversation.TextEvent(self, 0, "Curious...", 6));
                    self.events.Add(new Conversation.TextEvent(self, 0, "A being such as I, not anchored to one place...", 7));
                    self.events.Add(new Conversation.TextEvent(self, 0, "How have you surpassed the length of your chain?", 7));
                    self.events.Add(new Conversation.TextEvent(self, 0, "Fled the reins that grasped your being to linger there?", 7));
                    self.events.Add(new Conversation.TextEvent(self, 0, "To have escaped your eternal imprisonment.", 7));
                    self.events.Add(new Conversation.TextEvent(self, 0, "And yet you still linger.", 7));
                }
                if (self.id == Conversation.ID.Ghost_SI) //check for which ghost this is
                {
                    self.events = new List<Conversation.DialogueEvent>(); //remove all events already existing
                    self.events.Add(new Conversation.TextEvent(self, 0, "Do you hear it?", 6));
                    self.events.Add(new Conversation.TextEvent(self, 0, "The voice that calls from your very being?", 7));
                    self.events.Add(new Conversation.TextEvent(self, 0, "I'm familiar with it. It's pronounced within the air...", 7));
                    self.events.Add(new Conversation.TextEvent(self, 0, "But now, it's fainter. Calling from another place...", 7));
                    self.events.Add(new Conversation.TextEvent(self, 0, "How you have relocated your buzz is a mystery to me.", 7));
                }
                if (self.id == Conversation.ID.Ghost_CC) //check for which ghost this is
                {
                    self.events = new List<Conversation.DialogueEvent>(); //remove all events already existing
                    self.events.Add(new Conversation.TextEvent(self, 0, "From my perch, I have seen many things, as have you.", 7));
                    self.events.Add(new Conversation.TextEvent(self, 0, "We've seen the rain fade as cold begins to bite the air.", 7));
                    self.events.Add(new Conversation.TextEvent(self, 0, "How you have found a way to see further beyond your own perch beguiles me.", 8));
                    self.events.Add(new Conversation.TextEvent(self, 0, "A vessel of flesh not of your own.", 7));
                    self.events.Add(new Conversation.TextEvent(self, 0, "Are your memories the same?", 7));
                }
                if (self.id == MoreSlugcatsEnums.ConversationID.Ghost_CL) //check for which ghost this is
                {
                    self.events = new List<Conversation.DialogueEvent>(); //remove all events already existing
                    self.events.Add(new Conversation.TextEvent(self, 0, "My presence has been revealed to you now, little one.", 7));
                    self.events.Add(new Conversation.TextEvent(self, 0, "Your attunement has become much closer to my own.", 7));
                    self.events.Add(new Conversation.TextEvent(self, 0, "Through awakening you, I have seen through your eyes and met your grievances.", 8));
                    self.events.Add(new Conversation.TextEvent(self, 0, "And here I am again, to set forth another simple creature.", 7));
                    self.events.Add(new Conversation.TextEvent(self, 0, "I will continue to be alongside you until the time is right.", 7));
                    self.events.Add(new Conversation.TextEvent(self, 0, "What you decide to do with your new chance at life is yours alone.", 8));
                }
                /*if (self.id == MoreSlugcats.MoreSlugcatsEnums.ConversationID.Ghost_UG) //check for which ghost this is
                {
                    self.events = new List<Conversation.DialogueEvent>(); //remove all events already existing
                    self.events.Add(new Conversation.TextEvent(self, 0, "You see the same as me don't you?", 4));
                    self.events.Add(new Conversation.TextEvent(self, 0, "The beauty of life ever giving.", 4));
                    self.events.Add(new Conversation.TextEvent(self, 0, "We did not have the will to depart, nor the desire.", 5));
                    self.events.Add(new Conversation.TextEvent(self, 0, "Why did they always search for an escape, as if we were imprisoned?", 6));
                    self.events.Add(new Conversation.TextEvent(self, 0, "What offering from the void could usurp the gift of life already given?", 6));
                    self.events.Add(new Conversation.TextEvent(self, 0, "This moment, right here! It is where we are meant to be.", 5));
                }*/
            }
        }
        private void Player_ObjectEaten(On.Player.orig_ObjectEaten orig, Player self, IPlayerEdible edible)
        {
            orig(self, edible);
            if (self.slugcatStats.name.value == "Gravelslug")
            {
                if (edible.GetType() == typeof(KarmaFlower))
                {
                    GravelDissolveAdd(200f);
                }
                else
                {
                    GravelDissolveAdd(20f);
                }
            }
        }
        private void RoomRain_ThrowAroundObjects(On.RoomRain.orig_ThrowAroundObjects orig, RoomRain self)
        {
            if (self.room.roomSettings.DangerType != RoomRain.DangerType.AerieBlizzard)
            {
                if (ModManager.MSC && self.room.game.IsStorySession && self.room.world.region != null && self.room.world.game.GetStorySession.saveStateNumber.value == "Gravelslug" && self.room.roomSettings.RainIntensity <= 0.2f)
                {
                    return;
                }
            }
            /*if (self.room.dustStorm && self.dangerType == RoomRain.DangerType.Thunder && self.room.world.name == "SI")
            {
                Debug.Log("gabbagoo");
                for (int i = 0; i < self.room.physicalObjects.Length; i++)
                {
                    for (int j = 0; j < self.room.physicalObjects[i].Count; j++)
                    {
                        for (int k = 0; k < self.room.physicalObjects[i][j].bodyChunks.Length; k++)
                        {
                            BodyChunk bodyChunk = self.room.physicalObjects[i][j].bodyChunks[k];
                            RWCustom.IntVector2 tilePosition = self.room.GetTilePosition(bodyChunk.pos + new Vector2(Mathf.Lerp(-bodyChunk.rad, bodyChunk.rad, UnityEngine.Random.value), Mathf.Lerp(-bodyChunk.rad, bodyChunk.rad, UnityEngine.Random.value)));
                            float num = self.room.blizzardGraphics.blizzardIntensity;
                            bool flag = false;
                            if (self.rainReach[RWCustom.Custom.IntClamp(tilePosition.x, 0, self.room.TileWidth - 1)] < tilePosition.y)
                            {
                                flag = true;
                                num = Mathf.Max(self.intensity);
                            }
                            if (num > 0f)
                            {
                                if (bodyChunk.ContactPoint.y < 0)
                                {
                                    bodyChunk.vel += RWCustom.Custom.DegToVec(Mathf.Lerp(-30f, 0f, UnityEngine.Random.value) + (float)(num * 16)) * UnityEngine.Random.value * (flag ? 9f : 4f) * num / bodyChunk.mass;
                                }
                                else
                                {
                                    BodyChunk bodyChunk2 = bodyChunk;
                                    bodyChunk2.vel.y = bodyChunk2.vel.y - Mathf.Pow(UnityEngine.Random.value, 5f) * 16.5f * num / bodyChunk.mass;
                                }
                                if (bodyChunk.owner is Creature)
                                {
                                    if (Mathf.Pow(UnityEngine.Random.value, 1.2f) * 2f * (float)bodyChunk.owner.bodyChunks.Length < num)
                                    {
                                        (bodyChunk.owner as Creature).Stun(UnityEngine.Random.Range(1, 1 + (int)(9f * num)));
                                    }
                                    if (bodyChunk == (bodyChunk.owner as Creature).mainBodyChunk)
                                    {
                                        (bodyChunk.owner as Creature).rainDeath += num / 20f;
                                    }
                                    if (num > 0.5f && (bodyChunk.owner as Creature).rainDeath > 1f && UnityEngine.Random.value < 0.025f)
                                    {
                                        (bodyChunk.owner as Creature).Die();
                                    }
                                }
                                bodyChunk.vel += RWCustom.Custom.DegToVec(Mathf.Lerp(90f, 270f, UnityEngine.Random.value)) * UnityEngine.Random.value * 5f * num;
                            }
                        }
                    }
                }
                return;
            }*/
            orig(self);
        }
        private void SLOrcacleState_ctor(On.SLOrcacleState.orig_ctor orig, SLOrcacleState self, bool isDebugState, SlugcatStats.Name saveStateNumber)
        {
            orig(self, isDebugState, saveStateNumber);
            if (saveStateNumber.value == "Gravelslug")
            {
                self.neuronsLeft = 7;
            }
        }
        private void MiscWorldSaveData_ctor(On.MiscWorldSaveData.orig_ctor orig, MiscWorldSaveData self, SlugcatStats.Name saveStateNumber)
        {
            orig(self, saveStateNumber);
            if (saveStateNumber.value == "Gravelslug")
            {
                self.moonHeartRestored = true;
            }
        }
        private void GarbageHate(On.GarbageWorm.orig_Update orig, GarbageWorm self, bool eu)
        {
            orig(self, eu);
            AbstractCreature firstAlivePlayer = self.room.game.FirstAlivePlayer;
            Player player = firstAlivePlayer.realizedCreature as Player;

            for (int i = 0; i < self.room.abstractRoom.creatures.Count; i++)
            {
                if (self.room.abstractRoom.creatures[i].creatureTemplate.type == CreatureTemplate.Type.Slugcat && (player.slugcatStats.name.value == "Gravelslug") && !self.State.angryAt.Contains(self.room.abstractRoom.creatures[i].ID))
                {
                    self.State.angryAt.Add(self.room.abstractRoom.creatures[i].ID);
                }
            }

        }
        private void Player_Destroy(On.Player.orig_Destroy orig, Player self)
        {
            orig(self);
            if (GravelGut.TryGet(self, out bool explode)
                && explode)
            {
                GravelKaboom(self, true);
                if (GravelEaten != null && !GravelEaten.dead)
                {
                    GravelEaten.Die();
                }
            }
            
        }
        float GravelGutDissolveTimer = 120f;

        private void GravelDissolveUpdate(RainWorldGame game)
        {
            AbstractCreature firstAlivePlayer = game.FirstAlivePlayer;
            if (!(game.Players.Count > 0 && firstAlivePlayer != null && firstAlivePlayer.realizedCreature != null))
            {
                return;
            }
            bool anygravel = game.FirstRealizedPlayer.SlugCatClass.value == "Gravelslug";
            bool feral = false;
            for (int m = 0; m < game.AlivePlayers.Count; m++)
            {
                Player player = game.AlivePlayers[m].realizedCreature as Player;
                if (player.SlugCatClass.value == "Gravelslug" && !player.inShortcut && !player.dead)
                {
                    anygravel = true;
                    if (GravelGutDissolveTimer <= 0f && player.inShortcut != true)
                    {
                        if (player.playerState.foodInStomach > 0)
                        {
                            StartCoroutine(VoidFireSpasm(player, false, false));
                            player.room.PlaySound(SoundID.Fire_Spear_Ignite, player.firstChunk.pos, 0.4f, UnityEngine.Random.Range(0.8f, 1.2f));
                            if (player.objectInStomach != null)
                            {
                                TummyItem(player);
                            }
                            if (GravelFoodBar)
                            {
                                player.SubtractFood(1);
                            }
                        }
                        else 
                        {
                            GravelDissolveReset(player.room.game);
                            if (!(ModManager.Expedition && player.room.game.rainWorld.ExpeditionMode) && player.room.game.GetStorySession.characterStats.name.value == "Gravelslug" && player.room.game.GetStorySession.saveState.cycleNumber != 0 && (player.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW] == -8 && !player.Malnourished)
                            {
                                player.Stun(60);
                            }
                            else
                            {
                                if (IsGravelFeral(player))
                                {
                                    player.playerState.permanentDamageTracking += 0.2;
                                }
                                else
                                {
                                    if (player.room.game.IsStorySession)
                                    {
                                        player.SetMalnourished(true);
                                    }
                                    else
                                    {
                                        player.playerState.permanentDamageTracking += 0.1;
                                    }
                                }
                                player.aerobicLevel = 0.6f + ((float)player.playerState.permanentDamageTracking);

                                player.room.game.cameras[0].hud.foodMeter.refuseCounter += 80;
                                StartCoroutine(VoidFireSpasm(player, false, false));
                                player.room.PlaySound(SoundID.Fire_Spear_Ignite, player.firstChunk.pos, 0.4f, UnityEngine.Random.Range(0.8f, 1.2f));
                                if (player.playerState.permanentDamageTracking >= 1.0)
                                {
                                    player.Die();
                                }
                            }
                        }
                    }
                    

                    if (!player.sceneFlag && GravelGutDissolveTimer <= 20f)
                    {
                        if (GravelGutDissolveTimer <= 10f)
                        {
                            player.room.game.cameras[0].hud.foodMeter.refuseCounter = 10 - (int)GravelGutDissolveTimer;
                        }
                        if (UnityEngine.Random.value < 0.01f)
                        {
                            player.room.PlaySound(SoundID.Bro_Digestion_Init, player.firstChunk.pos, 0.5f, UnityEngine.Random.Range(1.2f, 1.6f));
                        }
                    }

                    if (IsGravelFeral(player))
                    {
                        feral = true;
                    }
                    else
                    {
                        if (!player.sceneFlag && GravelGutDissolveTimer > 20f)
                        {
                            if (UnityEngine.Random.value < 0.005f && player.playerState.permanentDamageTracking > 0)
                            {
                                player.playerState.permanentDamageTracking -= 0.01;
                                if (UnityEngine.Random.value < 0.01f && player.playerState.permanentDamageTracking < 0)
                                {
                                    player.playerState.permanentDamageTracking = 0;
                                }
                                Debug.Log("Gravel regen to " + player.playerState.permanentDamageTracking);
                            }
                        }
                        
                    }
                }
            }
            if (!anygravel)
            {
                return;
            }
            if (GravelGutDissolveTimer <= 0f && (game.FirstAlivePlayer.realizedCreature as Player).inShortcut != true)
            {
                if (!GravelFoodBar)
                {
                    if ((game.FirstAlivePlayer.realizedCreature as Player).playerState.foodInStomach > 0)
                    {
                        game.cameras[0].hud.foodMeter.refuseCounter += 50;
                        (game.FirstAlivePlayer.realizedCreature as Player).SubtractFood(1);
                    }
                    else
                    {
                        game.cameras[0].hud.foodMeter.refuseCounter += 80;
                    }
                }
                    
                GravelDissolveReset(game);
            }
            if (feral)
            {
                GravelDissolveSubtract(0.05f, game, false);
            }
            else
            {
                GravelDissolveSubtract(0.04f, game, false);
            }
        }

        private void GravelDissolveCraft(Player player)
        {
            StartCoroutine(VoidFireSpasm(player, false, false));
            player.room.PlaySound(SoundID.Fire_Spear_Ignite, player.firstChunk.pos, 0.3f, UnityEngine.Random.Range(1.0f, 1.4f));
            GravelDissolveReset(player.room.game);
            TummyItem(player);
            player.SubtractFood(1);
        }

        private void GravelDissolveReset(RainWorldGame game)
        {
            float limit = 200f;
            GravelGutDissolveTimer = 40f;
            GravelDissolveAdd(UnityEngine.Random.value * 100);
            if ((game.FirstAlivePlayer.realizedCreature as Player).KarmaCap >= 4)
            {
                GravelGutDissolveTimer += ((game.FirstAlivePlayer.realizedCreature as Player).KarmaCap * 10f);
            }
            if (GravelGutDissolveTimer > limit)
            {
                GravelGutDissolveTimer = limit;
            }
            if (game.world.rainCycle.TimeUntilSunset <= 0)
            {
                GravelGutDissolveTimer *= 0.75f;
            }
            Debug.Log("Reset Gravel Dissolve Timer to " + Math.Round(GravelGutDissolveTimer/80 * (game.IsStorySession && game.GetStorySession.characterStats.malnourished ? 0.75 : 1), 2) + " Minutes!");
        }
        private void GravelDissolveAdd(float amount)
        {
            if (GravelOptionsMenu.DisableTimer.Value)
            {
                return;
            }
            float limit = 200f;
            if (GravelGutDissolveTimer <= limit)
            {
                GravelGutDissolveTimer += amount;
                Debug.Log("Added " + amount/40*30 + " Seconds to Gravel Dissolve Timer");
                if (GravelGutDissolveTimer > limit)
                {
                    GravelGutDissolveTimer = limit;
                }
            }
        }

        private void GravelArenaDissolve(Player player)
        {
            StartCoroutine(VoidFireSpasm(player, false, false));
            if (player.FoodInStomach <= 0)
            {
                player.playerState.permanentDamageTracking += 0.35f;
                if (player.playerState.permanentDamageTracking >= 1f)
                {
                    player.Die();
                }
            }
            else
            {
                player.SubtractFood(1);
            }
        }
        private void GravelDissolveSubtract(float amount, RainWorldGame game, bool ignoreReduction)
        {
            if (GravelOptionsMenu.DisableTimer.Value)
            {
                return;
            }
            if (amount >= 200)
            {
                GravelGutDissolveTimer = 0;
                return;
            }
            if (game.GetStorySession.characterStats.name.value == "Gravelslug" && game.GetStorySession.saveState.cycleNumber != 0 && !(ModManager.Expedition && game.rainWorld.ExpeditionMode))
            {
                if (ignoreReduction)
                {
                    GravelGutDissolveTimer -= amount;
                }
                else
                {
                    GravelGutDissolveTimer -= amount - (amount * (-game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW] / 8));
                }
            }
            else
            {
                GravelGutDissolveTimer -= amount;
            }
            if (amount >= 20)
            {
                Debug.Log("Removed " + amount / 40 * 30 + " Seconds from Gravel Dissolve Timer");
            }
            if (amount < 0.4f && GravelGutDissolveTimer >= 20 && Math.Round(GravelGutDissolveTimer, 1) % (game.IsStorySession && game.GetStorySession.characterStats.malnourished ? 50 : 40) == 0 && Math.Round(GravelGutDissolveTimer, 2) > GravelGutDissolveTimer - amount)
            {
                Debug.Log("Gravel Dissolve Timer at " + Math.Round(GravelGutDissolveTimer / (game.IsStorySession && game.GetStorySession.characterStats.malnourished ? 100 : 80), 2) + " Minutes!");
            }
        }

        private void Fire_Breath(Player player, bool reset, bool eu)
        {
            Color FireColor = GravelFireColor(player);
            FireSmoke coughSmoke = new FireSmoke(player.room);
            if (reset)
            {
                player.Blink(30);
            }
            if (coughSmoke != null)
            {
                coughSmoke.Update(eu);

                if (player.room.ViewedByAnyCamera(player.firstChunk.pos, 300f) && player.Submersion != 1f)
                {
                    coughSmoke.EmitSmoke(player.firstChunk.pos, RWCustom.Custom.RNV(), FireColor, 20);
                }
                //coughSmoke.Destroy();
            }
        }
        private void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);
            if (self.SlugCatClass.value == "Gravelslug")
            {
                /*if(self.playerState.permanentDamageTracking >= 5 && self.abstractCreature.tentacleImmune)
                {
                    self.abstractCreature.tentacleImmune = false;
                }else if (!self.abstractCreature.tentacleImmune)
                {
                    self.abstractCreature.tentacleImmune = true;
                }*/
                if (self.room.game.IsStorySession && !(ModManager.Expedition && self.room.game.rainWorld.ExpeditionMode) && self.abstractCreature.world.game.StoryCharacter.value == "Gravelslug" && self.AI == null && self.room.game.session is StoryGameSession && (self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ArtificerMaulTutorial && !(self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ArtificerTutorialMessage && !self.Malnourished)
                {
                    (self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ArtificerTutorialMessage = true;
                    //self.room.AddObject(new GhostHunch(self.room, null));
                    self.room.game.cameras[0].hud.textPrompt.AddMessage(self.room.game.manager.rainWorld.inGameTranslator.Translate("You are now nourished and full of gravel, lessening the pain within."), 40, 240, false, true);
                    self.room.game.cameras[0].hud.textPrompt.AddMessage(self.room.game.manager.rainWorld.inGameTranslator.Translate("You are heavier and more durable in this state, you sink in water and can withstand some projectiles."), 0, 240, false, true);
                    self.room.game.cameras[0].hud.textPrompt.AddMessage(self.room.game.manager.rainWorld.inGameTranslator.Translate("You can spit out rocks and empty your stomach to reduce weight and return to the Malnourished state."), 0, 240, false, true);
                }
                /*if(self.room.game.IsStorySession && self.room.game.GetStorySession.saveState.cycleNumber != 0 && (self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ghostsTalkedTo[MoreSlugcatsEnums.GhostID.CL] == 2 && self.wantToJump > 0 && self.input[0].pckp && self.canJump <= 0 && self.input[0].y > 0 && self.KarmaIsReinforced)
                {
                    self.room.game.GetStorySession.saveState.deathPersistentSaveData.reinforcedKarma = false;
                    self.room.game.cameras[0].hud.karmaMeter.reinforceAnimation = 0;
                    self.room.game.cameras[0].ghostMode = 1f;
                    self.room.AddObject(new GhostPing(self.room));
                    self.room.AddObject(new ShockWave(new Vector2(self.mainBodyChunk.pos.x, self.mainBodyChunk.pos.y), 300f, 0.2f, 15, false));
                    self.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.Chain_Lock, new Vector2(self.mainBodyChunk.pos.x, self.mainBodyChunk.pos.y));
                    for (int i = 0; i < self.room.abstractRoom.creatures.Count; i++)
                    {
                        if (self.room.abstractRoom.creatures[i].realizedCreature != null && self.room.abstractRoom.creatures[i].creatureTemplate.type != CreatureTemplate.Type.Slugcat && (!ModManager.MSC || self.room.abstractRoom.creatures[i].creatureTemplate.type != MoreSlugcatsEnums.CreatureTemplateType.SlugNPC))
                        {
                            self.room.abstractRoom.creatures[i].realizedCreature.stun = Math.Max(self.room.abstractRoom.creatures[i].realizedCreature.stun, 100 + (int)(UnityEngine.Random.value * 200));
                        }
                    }
                }*/
                if (GravelEaten != null && GravelEaten.abstractCreature.abstractAI != null && self != null)
                {
                    GravelEaten.abstractCreature.abstractAI.SetDestination(self.room.GetWorldCoordinate(self.mainBodyChunk.pos));
                    (GravelEaten as DaddyLongLegs).AI.preyTracker.AddPrey((GravelEaten as DaddyLongLegs).AI.tracker.RepresentationForCreature(self.abstractCreature, true));
                }
                /*if (IsGravelFeral(self))
                {
                    self.dynamicRunSpeed[0] = self.slugcatStats.runspeedFac - (self.FoodInStomach * 0.5f);
                    self.dynamicRunSpeed[1] = self.slugcatStats.runspeedFac - (self.FoodInStomach * 0.5f);
                }
                else
                {
                    self.dynamicRunSpeed[0] = self.slugcatStats.runspeedFac + (self.FoodInStomach * 0.5f);
                    self.dynamicRunSpeed[1] = self.slugcatStats.runspeedFac + (self.FoodInStomach * 0.5f);
                }*/
                //float buoy = 1f - (self.FoodInStomach * 0.3f);
                //self.buoyancy -= self.FoodInStomach;
                /*if (self.room.game.IsStorySession && !self.dead)
                {
                    GravelDissolveUpdate(self);
                }*/
                if (IsGravelFeral(self) && !self.inShortcut && self.Submersion < 0.5f && self.aerobicLevel > 0.4f)
                {
                    Vector2 pos = self.firstChunk.pos;
                    if (self.graphicsModule != null)
                    {
                        PlayerGraphics playerGraphics = self.graphicsModule as PlayerGraphics;
                        float num = Mathf.Sin(playerGraphics.breath * 3.1415927f * 2f);
                        float num2 = Mathf.Sin(playerGraphics.lastBreath * 3.1415927f * 2f);
                        if (playerGraphics != null && num < num2 && num < 0.5f && num > -0.5f)
                        {
                            Vector2 vector = playerGraphics.lookDirection * 8f;
                            Vector2 b = new Vector2(0f, 5f);
                            Color color = GravelFireColor(self);
                            if ((self as Player).bodyMode == Player.BodyModeIndex.Crawl)
                            {
                                vector = playerGraphics.lookDirection * 16f;
                                b.x = (float)(self as Player).flipDirection * 20f;
                            }
                            GravelFlameBreath firebreath = new GravelFlameBreath(pos + b + vector, RWCustom.Custom.RNV() * 0.2f + vector * 0.1f + self.firstChunk.vel * 0.25f, UnityEngine.Random.value * 20f + 5f, color);
                            self.room.AddObject(firebreath);
                        }
                    }
                }
                if (self.room.game.IsStorySession)
                {
                    if (self.Submersion > 0.1f && self.room.abstractRoom.name != "HR_FINAL")
                    {
                        if (self.room.waterObject != null && self.room.waterObject.WaterIsLethal && !self.dead && !self.room.game.devToolsActive)
                        {
                            GravelDissolveSubtract(0.4f, self.room.game, true);
                            self.room.game.cameras[0].hud.foodMeter.RefuseFood();
                        }
                        float foodPercentage = self.FoodInStomach / self.MaxFoodInStomach;
                        if (!GravelOptionsMenu.NoSinking.Value && foodPercentage > 0.5f)
                        {
                            if (IsGravelFeral(self))
                            {
                                self.mainBodyChunk.vel.y -= ((8 * foodPercentage) * 0.03f);
                            }
                            else
                            {
                                self.slowMovementStun = Math.Max(self.slowMovementStun, (int)RWCustom.Custom.LerpMap(self.aerobicLevel, 0.7f, 0.4f, 6f, 0f));
                                self.mainBodyChunk.vel.y -= ((8 * foodPercentage) * 0.04f);
                            }
                        }
                    }
                }
                if (!IsGravelFeral(self) && self.animation == Player.AnimationIndex.Roll)
                {
                    //self.animation = Player.AnimationIndex.DownOnFours;
                    self.Stun(40);
                    Debug.Log("Denied roll");
                }
            }
        }
        private void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            /*if (self.slugcatStats.name.value == "Gravelslug" && world.game.IsArenaSession)
            {
                self.SetMalnourished(true);
            }*/
            if(self.slugcatStats.name.value == "Gravelslug")
            {
                //self.abstractCreature.tentacleImmune = true;
                if (world.game.IsStorySession && (!ModManager.Expedition || (ModManager.Expedition && !self.room.game.rainWorld.ExpeditionMode)))
                {
                    if (world.game.GetStorySession.characterStats.name.value == "Gravelslug")
                    {
                        GravelDissolveReset(world.game);
                        if (world.game.GetStorySession.saveState.cycleNumber != 0 && !world.game.GetStorySession.saveState.progression.currentSaveState.malnourished)
                        {
                            if (self.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW] < -4)
                            {
                                self.slugcatStats.foodToHibernate = 4;
                                if (self.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW] <= -8)
                                {
                                    self.slugcatStats.maxFood -= 1;
                                }
                                //self.slugcatStats.maxFood += 5 + self.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW];
                            }
                            else
                            {
                                self.slugcatStats.foodToHibernate += self.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW];
                            }
                            /*if (self.KarmaCap > 5)
                            {
                                self.slugcatStats.foodToHibernate -= self.KarmaCap - 5;
                            }
                            //self.slugcatStats.foodToHibernate = (9 - self.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW]);
                            //self.slugcatStats.foodToHibernate = (7 + self.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW]);*/
                        }
                        /*if (self.room.abstractRoom.name == "SL_AI")
                        {
                            self.sceneFlag = false;
                        }*/
                        //self.sceneFlag = true;
                        if (self.room.world.region.name == "HR")
                        {
                            world.game.GetStorySession.saveState.deathPersistentSaveData.ripPebbles = true;
                            self.SetMalnourished(true);
                        }
                        else if (world.game.GetStorySession.saveState.deathPersistentSaveData.ripPebbles == true)
                        {
                            world.game.GetStorySession.saveState.deathPersistentSaveData.ripPebbles = false;
                        }
                    }
                    if (self.KarmaCap == 9)
                    {
                        self.abstractCreature.lavaImmune = true;
                    }
                }
            }
        }
        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
        }
        private void NoStick(On.DartMaggot.orig_ShotUpdate orig, DartMaggot self)
        {
            orig(self);
            UnityEngine.Vector2 vector = self.firstChunk.pos + self.firstChunk.vel;
            SharedPhysics.CollisionResult collisionResult = SharedPhysics.TraceProjectileAgainstBodyChunks(null, self.room, self.firstChunk.pos, ref vector, 1f, 1, self.shotBy, false);
            if (self.Stuck && ((collisionResult.chunk.owner as Player).SlugCatClass.value == "Gravelslug") && !IsGravelFeral(collisionResult.chunk.owner as Player) && self.sleepCounter <= 300)
            {
                self.Unstuck();
                self.firstChunk.vel /= 8;
                Debug.Log("dart maggot failed to pierce Gravel Eater");
            }
        }
        private bool BoxORocks(On.Player.orig_SlugSlamConditions orig, Player self, PhysicalObject otherObject)
        {
            if (self.SlugCatClass.value == "Gravelslug")
            {
                if (IsGravelFeral(self))
                {
                    return false;
                }
                if ((otherObject as Creature).abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC)
                {
                    return false;
                }
                if (self.gourmandAttackNegateTime > 0)
                {
                    return false;
                }
                if (self.gravity == 0f)
                {
                    return false;
                }
                if (self.cantBeGrabbedCounter > 0)
                {
                    return false;
                }
                if (self.forceSleepCounter > 0)
                {
                    return false;
                }
                if (self.timeSinceInCorridorMode < 5)
                {
                    return false;
                }
                if (self.submerged)
                {
                    return false;
                }
                if (self.enteringShortCut != null || (self.animation != Player.AnimationIndex.BellySlide && self.canJump >= 5))
                {
                    return false;
                }
                if (self.animation == Player.AnimationIndex.CorridorTurn || self.animation == Player.AnimationIndex.CrawlTurn || self.animation == Player.AnimationIndex.ZeroGSwim || self.animation == Player.AnimationIndex.ZeroGPoleGrab || self.animation == Player.AnimationIndex.GetUpOnBeam || self.animation == Player.AnimationIndex.ClimbOnBeam || self.animation == Player.AnimationIndex.AntlerClimb || self.animation == Player.AnimationIndex.BeamTip)
                {
                    return false;
                }
                UnityEngine.Vector2 vel = self.bodyChunks[0].vel;
                if (self.bodyChunks[1].vel.magnitude < vel.magnitude)
                {
                    vel = self.bodyChunks[1].vel;
                }
                if (self.animation != Player.AnimationIndex.BellySlide && vel.y >= -10f && vel.magnitude <= 25f)
                {
                    return false;
                }
                Creature creature = otherObject as Creature;
                foreach (Creature.Grasp grasp in self.grabbedBy)
                {
                    if (grasp.pacifying || grasp.grabber == creature)
                    {
                        return false;
                    }
                }
                return !ModManager.CoopAvailable || !(otherObject is Player) || RWCustom.Custom.rainWorld.options.friendlyFire;
            }
            else
            {
                return orig(self, otherObject);
            }

        }
        private static void GravelStart(On.MoreSlugcats.MSCRoomSpecificScript.orig_AddRoomSpecificScript orig, Room room)
        {
            orig(room);
            string name = room.abstractRoom.name;
            if (name == "CL_GRAVEL" && room.game.IsStorySession && room.abstractRoom.firstTimeRealized && room.game.GetStorySession.saveState.cycleNumber == 0 && room.game.GetStorySession.characterStats.name.value == "Gravelslug" && !(ModManager.Expedition && room.game.manager.rainWorld.ExpeditionMode))
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("It's Gravel-Eatin' Time!");
                }
                room.AddObject(new CL_GRAVEL(room));
            }
            /*if (name == "SL_AI" && room.game.IsStorySession && room.game.GetStorySession.lastEverMetMoon && room.game.GetStorySession.saveState.denPosition == "SL_AI" && room.game.GetStorySession.characterStats.name.value == "Gravelslug")
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("Tea Time!");
                }
                room.AddObject(new SL_AI_GRAVEL(room));
            }*/
            if (room.game.IsStorySession && (name == "MS_COMMS" || name == "SI_A07" || name == "LF_H01" || name == "CC_H01SAINT" || name == "GW_A25" || name == "DS_RIVSTART" || name == "OE_CAVE02" || name == "SB_GOR02RIV" || name == "SU_A53") && room.game.GetStorySession.characterStats.name.value == "Gravelslug" && !(ModManager.Expedition && room.game.manager.rainWorld.ExpeditionMode))
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("Slugcat Ghost placed");
                }
                room.AddObject(new SlugGhostVision(room));
            }
            /*if (name == "DS_C02GRAVEL" && room.game.IsStorySession && room.abstractRoom.firstTimeRealized)
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("Blue Flower Placed");
                }
                room.AddObject(new BlueFlower(room));
            }*/
            if (name == "SB_A06GRAV" && room.game.IsStorySession && room.game.GetStorySession.characterStats.name.value == "Gravelslug" && !(ModManager.Expedition && room.game.manager.rainWorld.ExpeditionMode))
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("Gravel end room applied");
                }
                room.AddObject(new SB_A06GRAV(room));
            }
            if (name == "HR_GRAVINTRO" && room.game.IsStorySession && room.game.GetStorySession.saveState.denPosition == "HR_GRAVINTRO" && room.game.GetStorySession.characterStats.name.value == "Gravelslug" && !(ModManager.Expedition && room.game.manager.rainWorld.ExpeditionMode))
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("It's nacho Time!");
                }
                room.AddObject(new HR_GravelIntro(room));
            }
            /*if (name == "HR_GRAV11" && room.game.IsStorySession)
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("Gravel Eater Standoff");
                }
                room.AddObject(new GRAVEL_FINAL(room));
            }*/
        }
        private float ThickSkinned(On.Player.orig_DeathByBiteMultiplier orig, Player self)
        {
            if ((self.SlugCatClass.value == "Gravelslug") && self.Malnourished == false)
            {
                return 0.15f;
            }
            return orig.Invoke(self);
        }
        private void NeverGiveUp(On.Player.orig_Grabbed orig, Player self, Creature.Grasp grasp)
        {
            orig(self, grasp);
            if (self.slugcatStats.name.value == "Gravelslug" && !IsGravelFeral(self))
            {
                /*if (!IsGravelFeral(self))
                {
                    GravelRetaliate(self);
                }*/
                if (grasp.grabber is Lizard || grasp.grabber is BigSpider || grasp.grabber is DropBug)
                {
                    self.dangerGraspTime = -600;
                    self.dangerGrasp = grasp;
                    Debug.Log("Delay Gravel Eater grabbed game over!");
                }
            }
        }
        private bool MaulToggle(On.Player.orig_CanMaulCreature orig, Player self, Creature crit)
        {
            orig(self, crit);
            bool flag = true;
            if (ModManager.CoopAvailable)
            {
                Player player = crit as Player;
                if (player != null && (player.isNPC || !RWCustom.Custom.rainWorld.options.friendlyFire))
                {
                    flag = false;
                }
            }
            return !(crit is Fly) && !crit.dead && (!(crit is IPlayerEdible) || (crit is Centipede && !(crit as Centipede).Edible) || self.FoodInStomach >= self.MaxFoodInStomach) && flag && (crit.Stunned || (!(crit is Cicada) && !(crit is Player) && self.IsCreatureLegalToHoldWithoutStun(crit))) && SlugcatStats.SlugcatCanMaul(self.SlugCatClass) && !(self.SlugCatClass.value == "Gravelslug" && !IsGravelFeral(self));;
        }
        private void VoidFit(On.Player.orig_Stun orig, Player self, int st)
        {
            orig(self, st);
            if(self.slugcatStats.name.value == "Gravelslug" && !self.chatlog)
            {
                if (GravelEaten != null && self != null)
                {
                    GravelEaten.Stun(st);
                }
                if (IsGravelFeral(self) && (st > 10 || (st > 5 && self.exhausted) || (GravelFatty && st > 3 && self.exhausted)) && self.airInLungs != 0)
                {
                    StartCoroutine(VoidFireSpasm(self, false, false));
                }
                else if (!IsGravelFeral(self) && self.Hypothermia < 1)
                {
                    st /= 2;
                    self.stun = st;
                }
            }
        }
        public FirecrackerPlant.ScareObject spookyCough;
        private void SpookBoom(Player player)
        {
            spookyCough = new FirecrackerPlant.ScareObject(player.firstChunk.pos);
            player.room.AddObject(spookyCough);

            /*if (spookyCough == null)
            {
                spookyCough = new FirecrackerPlant.ScareObject(player.firstChunk.pos);
                player.room.AddObject(spookyCough);
            }
            else
            {
                spookyCough.pos = player.firstChunk.pos;
            }
            spookyCough.lifeTime = 300;*/
        }
        private IEnumerator VoidFireSpasm(Player player, bool ded, bool boom)
        {
            if ((ded || !player.dead) && (!boom || !(player.input[0].pckp && player.input[0].y > 0)) && player.airInLungs != 0 && !player.slatedForDeletetion)
            {
                int numFlames = UnityEngine.Random.Range(2, 5);
                for (int i = 0; i < numFlames; i++)
                {
                    float delay = UnityEngine.Random.Range(0.4f, 1.2f);
                    if (player != null)
                    {
                        GravelCough(player, false);
                    }
                    if (GravelEaten != null)
                    {
                        GravelCough(GravelEaten, false);
                    }
                    if (player.Malnourished)
                    {
                        SpookBoom(player);
                    }
                    if (ded)
                    {
                        player.room.AddObject(new CreatureSpasmer(player, true, 100));
                        player.firstChunk.vel += new UnityEngine.Vector2(UnityEngine.Random.Range(-3f, 3f), 5f);
                    }
                    else
                    {
                        player.room.AddObject(new CreatureSpasmer(player, false, 75));
                    }
                    yield return new WaitForSeconds(delay);
                }
            }
            if (ded)
            {
                GravelKaboom(player, false);
            }
        }

        private Color GravelFireColor(Player player)
        {
            var color = Color.cyan;
            if (!player.room.game.setupValues.arenaDefaultColors && !ModManager.CoopAvailable)
            {
                switch (player.playerState.playerNumber)
                {
                    case 0:
                        if (player.room.game.IsArenaSession && player.room.game.GetArenaGameSession.arenaSitting.gameTypeSetup.gameType != MoreSlugcatsEnums.GameTypeID.Challenge)
                        {
                            color = new Color(0.25f, 0.65f, 0.82f);
                        }
                        break;
                    case 1:
                        color = new Color(0.31f, 0.73f, 0.26f);
                        break;
                    case 2:
                        color = new Color(0.6f, 0.16f, 0.6f);
                        break;
                    case 3:
                        color = new Color(0.96f, 0.75f, 0.95f);
                        break;
                }
            }
            if ((player.graphicsModule as PlayerGraphics).useJollyColor)
            {
                color = PlayerGraphics.JollyColor((player).playerState.playerNumber, 2);
            }
            if (PlayerGraphics.CustomColorsEnabled())
            {
                color = PlayerGraphics.CustomColorSafety(2);
            }
            return color;
        }
        private void GravelKaboom(Player player, bool destroyed)
        {
            var room = player.room;
            var pos = player.mainBodyChunk.pos;
            var color = GravelFireColor(player);
            room.AddObject(new Explosion(room, player, pos, 7, 225f, 4.2f, 50f, 280f, 0.25f, player, 0.7f, 160f, 1f));
            room.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
            room.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
            room.AddObject(new ExplosionSpikes(room, pos, 14, 30f, 9f, 7f, 170f, color));
            //room.InGameNoise(new Noise.InGameNoise(pos, 9000f, player, 1f));
            room.ScreenMovement(pos, default, 1.3f);
            for (int k = 0; k < player.abstractPhysicalObject.stuckObjects.Count; k++)
            {
                player.abstractPhysicalObject.stuckObjects[k].Deactivate();
            }
            room.PlaySound(SoundID.Fire_Spear_Explode, pos);
            room.PlaySound(SoundID.Bomb_Explode, pos);
            if (!destroyed)
            {
                for (int i = 0; i < 14; i++)
                {
                    room.AddObject(new Explosion.ExplosionSmoke(pos, RWCustom.Custom.RNV() * 5f * UnityEngine.Random.value, 1f));
                }
                for (int j = 0; j < 20; j++)
                {
                    Vector2 a = RWCustom.Custom.RNV();
                    room.AddObject(new Spark(pos + a * UnityEngine.Random.value * 40f, a * Mathf.Lerp(4f, 30f, UnityEngine.Random.value), color, null, 4, 18));
                }
                room.AddObject(new SootMark(room, pos, 80f, true));
                room.AddObject(new ShockWave(pos, 450f, 0.165f, 5, false));
                room.AddObject(new Smolder(room, pos, player.firstChunk, null));
                player.firstChunk.vel += new UnityEngine.Vector2(UnityEngine.Random.Range(-15f, 15f), 30f);
                
            }
            Debug.Log("Gravel KaBOOM!");
        }
        private void TummyItem(Player self)
        {
            if (self.objectInStomach != null)
            {
                float color;
                Color.RGBToHSV(GravelFireColor(self), out color, out float S, out float V);
                if (color > 0.5f)
                {
                    color -= 0.5f;
                }
                else
                {
                    color += 0.5f;
                }
                AbstractPhysicalObject craftedObject;
                if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.Spear
                    || self.objectInStomach.type == MoreSlugcats.MoreSlugcatsEnums.AbstractObjectType.LillyPuck)
                {
                    //self.objectInStomach.type = AbstractPhysicalObject.AbstractObjectType.Spear;
                    craftedObject = new AbstractSpear(self.room.world, null, self.abstractCreature.pos, self.room.game.GetNewID(), false, color);
                }
                else if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant)
                {
                    craftedObject = new AbstractConsumable(self.room.world, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null, self.abstractCreature.pos, self.room.game.GetNewID(), -1, -1, null)
                    {
                        isFresh = false,
                        isConsumed = true
                    };
                }
                else if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.FlyLure)
                {
                    craftedObject = new AbstractConsumable(self.room.world, AbstractPhysicalObject.AbstractObjectType.KarmaFlower, null, self.abstractCreature.pos, self.room.game.GetNewID(), -1, -1, null)
                    {
                        isFresh = false,
                        isConsumed = true
                    };
                }
                else if (self.objectInStomach.type == MoreSlugcatsEnums.AbstractObjectType.GlowWeed)
                {
                    craftedObject = new BubbleGrass.AbstractBubbleGrass(self.room.world, null, self.abstractCreature.pos, self.room.game.GetNewID(), 1f, -1, -1, null)
                    {
                        isFresh = false,
                        isConsumed = true
                    };
                }
                else if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.BubbleGrass)
                {
                    craftedObject = new AbstractConsumable(self.room.world, AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant, null, self.abstractCreature.pos, self.room.game.GetNewID(), -1, -1, null)
                    {
                        isFresh = false,
                        isConsumed = true
                    };
                }
                else if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.FlareBomb
                    || self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.DangleFruit)
                {
                    craftedObject = new AbstractConsumable(self.room.world, AbstractPhysicalObject.AbstractObjectType.Lantern, null, self.abstractCreature.pos, self.room.game.GetNewID(), -1, -1, null)
                    {
                        isFresh = false,
                        isConsumed = true
                    };
                }
                else if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.OverseerCarcass
                    || self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.NeedleEgg)
                {
                    craftedObject = new AbstractConsumable(self.room.world, AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer, null, self.abstractCreature.pos, self.room.game.GetNewID(), -1, -1, null)
                    {
                        isFresh = false,
                        isConsumed = true
                    };
                }
                else if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.DataPearl
                    || self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.PebblesPearl
                    || self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.Rock
                    || self.objectInStomach.type == MoreSlugcatsEnums.AbstractObjectType.HalcyonPearl
                    || self.objectInStomach.type == MoreSlugcatsEnums.AbstractObjectType.Spearmasterpearl
                    || self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.NSHSwarmer
                    || self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.SLOracleSwarmer
                    || self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer)
                {
                    self.Regurgitate();
                    Debug.Log("Gravel Eater cannot craft with " + self.objectInStomach.type + "!");
                    return;
                }
                else
                {
                    if (self.objectInStomach is AbstractCreature)
                    {
                        self.objectInStomach = null;
                    }
                    //self.objectInStomach.type = MoreSlugcats.MoreSlugcatsEnums.AbstractObjectType.FireEgg;
                    craftedObject = new FireEgg.AbstractBugEgg(self.room.world, null, self.abstractCreature.pos, self.room.game.GetNewID(), color);
                }
                self.objectInStomach = craftedObject;
                Debug.Log("Coverted Gravel Stomach item from " + self.objectInStomach.type + " to " + craftedObject.type);
                self.Regurgitate();
            }
        }
        private bool MoreSwallows(On.Player.orig_CanBeSwallowed orig, Player self, PhysicalObject testObj)
        {
            if (self.slugcatStats.name.value == "Gravelslug")
            {
                return (testObj is Spear && (self.input[0].y > 0 || self.FoodInStomach < self.MaxFoodInStomach)
                    || (testObj is IPlayerEdible && (self.input[0].y > 0 || (self.FoodInStomach >= self.MaxFoodInStomach && testObj is not Creature)))
                    || testObj is NeedleEgg || testObj is Rock || testObj is DataPearl || testObj is FlareBomb || testObj is Lantern || testObj is FirecrackerPlant
                    || (testObj is VultureGrub && !(testObj as VultureGrub).dead)
                    || (testObj is Hazer && !(testObj as Hazer).dead && !(testObj as Hazer).hasSprayed)
                    || (testObj is SSOracleSwarmer && self.FoodInStomach >= self.MaxFoodInStomach)
                    || (ModManager.MSC && testObj is FireEgg && self.FoodInStomach >= self.MaxFoodInStomach)
                    || testObj is FlyLure || testObj is ScavengerBomb || testObj is PuffBall || testObj is SporePlant || testObj is BubbleGrass || testObj is NSHSwarmer || testObj is OverseerCarcass
                    || (ModManager.MSC && testObj is SingularityBomb && !(testObj as SingularityBomb).activateSingularity && !(testObj as MoreSlugcats.SingularityBomb).activateSucktion));
            }
            return orig.Invoke(self, testObj);
        }
        private static string Region_GetProperRegionAcronym(On.Region.orig_GetProperRegionAcronym orig, SlugcatStats.Name character, string baseAcronym)
        {
            if (character.value == "Gravelslug")
            {
                if (baseAcronym == "SH") baseAcronym = "CL";
            }
            return orig(character, baseAcronym);
        }

        //NOT MY CODE!! ↓   ↓
        public void WormGrassPatch_InteractWithCreature(On.WormGrass.WormGrassPatch.orig_InteractWithCreature orig, WormGrass.WormGrassPatch self, WormGrass.WormGrassPatch.CreatureAndPull creatureAndPull)
        {
            Player player = creatureAndPull.creature as Player;
            bool flag = player == null || !(player.slugcatStats.name.value == "Gravelslug") && !IsGravelFeral(player);
            if (flag)
            {
                orig.Invoke(self, creatureAndPull);
            }
        }
        public void WormGrassPatch_Update(On.WormGrass.WormGrassPatch.orig_Update orig, WormGrass.WormGrassPatch self)
        {
            orig.Invoke(self);
            for (int i = self.trackedCreatures.Count - 1; i >= 0; i--)
            {
                Player player = self.trackedCreatures[i].creature as Player;
                bool flag = player != null && (player.slugcatStats.name.value == "Gravelslug") && !IsGravelFeral(player);
                if (flag)
                {
                    self.trackedCreatures.RemoveAt(i);
                }
            }
        }
        public bool WormGrassPatch_AlreadyTrackingCreature(On.WormGrass.WormGrassPatch.orig_AlreadyTrackingCreature orig, WormGrass.WormGrassPatch self, Creature creature)
        {
            for (int i = self.trackedCreatures.Count - 1; i >= 0; i--)
            {
                Player player = self.trackedCreatures[i].creature as Player;
                bool flag = player != null && (player.slugcatStats.name.value == "Gravelslug") && !IsGravelFeral(player);
                if (flag)
                {
                    return true;
                }
            }
            return orig.Invoke(self, creature);
        }
        //              ↑   ↑

        private void ThrowFire(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
        {
            orig(self, spear);
            if (IsGravelFeral(self))
            {
                spear.spearDamageBonus = 2f;
                /*BodyChunk firstChunk3 = spear.firstChunk;
                firstChunk3.vel.x *= 1.17f;
                self.room.PlaySound(MoreSlugcats.MoreSlugcatsEnums.MSCSoundID.Throw_FireSpear, self.firstChunk.pos, 1f, UnityEngine.Random.Range(0.8f, 1.2f));
                Fire_Breath(self);
                self.room.AddObject(new Explosion.ExplosionLight(self.firstChunk.pos, 280f, 1f, 7, Color.white));
                self.room.AddObject(new ExplosionSpikes(self.room, self.firstChunk.pos, 14, 15f, 9f, 5f, 90f, RWCustom.Custom.HSL2RGB(RWCustom.Custom.Decimal(1f + EggBugGraphics.HUE_OFF), 1f, 0.5f)));*/
            }
        }
        private static bool Spear_HitSomething(On.Spear.orig_HitSomething orig, Spear self, SharedPhysics.CollisionResult result, bool eu)
        {
            bool result2;
            float num = (self.spearDamageBonus);
            if (result.obj == null)
            {
                return orig.Invoke(self, result, eu);
            }
            if (result.obj is Player && ((result.obj as Player).slugcatStats.name.value == "Gravelslug") && !IsGravelFeral(result.obj as Player))
            {
                (result.obj as Creature).Violence(self.firstChunk, new UnityEngine.Vector2?(self.firstChunk.vel * self.firstChunk.mass * 2f), result.chunk, result.onAppendagePos, Creature.DamageType.Stab, num * 0.4f, 20f);
                self.LodgeInCreature(result, eu);
                (result.obj as Player).playerState.permanentDamageTracking += (num * 0.4f / (result.obj as Player).Template.baseDamageResistance);
                self.room.PlaySound(SoundID.Spear_Stick_In_Creature, self.firstChunk);
                result2 = true;
                (result.obj as Player).Stun(1);
                return result2;
            }
            if (self.thrownBy is Player && IsGravelFeral(self.thrownBy as Player) && self.bugSpear)
            {
                result2 = orig.Invoke(self, result, eu); ;
                if (result.obj is Creature && (result.obj as Creature).SpearStick(self, Mathf.Lerp(0.55f, 0.62f, UnityEngine.Random.value), result.chunk, result.onAppendagePos, self.firstChunk.vel))
                {
                    self.room.AddObject(new CreatureSpasmer((result.obj as Creature), false, 100));
                    if (result2 == false && result.onAppendagePos == null)
                    {
                        (result.obj as Creature).Violence(self.firstChunk, new UnityEngine.Vector2?(self.firstChunk.vel * self.firstChunk.mass * 2f), result.chunk, result.onAppendagePos, Creature.DamageType.Stab, (num *= 0.5f), 20f);
                        self.LodgeInCreature(result, true);
                        self.room.PlaySound(SoundID.Seed_Cob_Open, self.firstChunk);
                        self.room.PlaySound(SoundID.Spear_Stick_In_Ground, self.firstChunk);
                        self.room.AddObject(new Smolder(self.room, result.chunk.pos, result.chunk, null));
                        result2 = true;
                        Debug.Log("Pierced Creature Armor!");
                    }
                }
                else if (result.obj is SeedCob)
                {
                    (result.obj as PhysicalObject.IHaveAppendages).ApplyForceOnAppendage(result.onAppendagePos, self.firstChunk.vel * self.firstChunk.mass);
                    (result.obj as SeedCob).Open();
                    self.room.PlaySound(SoundID.Spear_Stick_In_Creature, self.firstChunk);
                    self.LodgeInCreature(result, eu);
                    result2 = true;
                    Debug.Log("Woah! Secret Message for you! :3");
                }
            }
            else
            {
                return orig.Invoke(self, result, eu);
            }
            return result2;
        }
    }
    /*public class BlueFlower : UpdatableAndDeletable
    {
        public BlueFlower(Room room)
        {
            this.room = room;
        }
        private Player player;
        private int counter;
        public override void Update(bool eu)
        {
            base.Update(eu);
            AbstractCreature firstAlivePlayer = this.room.game.FirstAlivePlayer;
            if (firstAlivePlayer == null)
            {
                return;
            }
            if (this.player == null && this.room.game.Players.Count > 0 && firstAlivePlayer.realizedCreature != null && firstAlivePlayer.realizedCreature.room == this.room)
            {
                this.player = (firstAlivePlayer.realizedCreature as Player);
            }
            if (this.player != null && this.player.room != null && this.player.room.abstractRoom.index == this.room.abstractRoom.index)
            {
                this.counter++;
                if (this.counter == 1)
                {
                    AbstractConsumable flower = new AbstractConsumable(this.room.world, AbstractPhysicalObject.AbstractObjectType.KarmaFlower, null, this.room.GetWorldCoordinate(new Vector2(422f, 1075f)), this.room.game.GetNewID(), 0, 0, null);
                    this.room.abstractRoom.AddEntity(flower);
                    //flower.RealizeInRoom();
                    flower.Realize();
                    (flower.realizedObject as KarmaFlower).color = Color.cyan;
                    (flower.realizedObject as KarmaFlower).stalkColor = Color.cyan;
                    (flower.realizedObject as KarmaFlower).PlaceInRoom(this.room);
                    if (AbstractPhysicalObject.UsesAPersistantTracker(flower))
                    {
                        this.room.game.GetStorySession.AddNewPersistentTracker(flower);
                    }
                }
                if (this.counter > 1)
                {
                    this.Destroy();
                }
            }
        }
    }*/
    public class SlugGhostVision : UpdatableAndDeletable
    {
        public SlugGhostVision(Room room)
        {
            this.room = room;
        }
        private Vector2 pos;
        private Player player;
        private bool canSee = false;
        private bool isInv = false;
        private string moonPresence;
        private SlugcatGhost ghost;
        //private Color BodyColor;
        //private Color EyeColor;
        private GhostWorldPresence.GhostID slugcat;
        public override void Update(bool eu)
        {
            base.Update(eu);
            AbstractCreature firstAlivePlayer = this.room.game.FirstAlivePlayer;
            if (firstAlivePlayer == null)
            {
                return;
            }
            if (this.player == null && this.room.game.Players.Count > 0 && firstAlivePlayer.realizedCreature != null && firstAlivePlayer.realizedCreature.room == this.room)
            {
                this.player = (firstAlivePlayer.realizedCreature as Player);
            }
            if (this.player != null && this.player.abstractCreature.Room == this.room.abstractRoom && this.player.room != null)
            {
                if (!this.canSee)
                {
                    /*if (this.room.abstractRoom.name == "CL_D05")
                    {
                        // Test
                        this.pos = new Vector2(790f, 1050f);
                    }*/
                    if (this.room.abstractRoom.name == "SI_A07" && this.room.game.cameras[0] != null && this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GESpear] == 0)
                    {
                        // Spearmaster
                        slugcat = GravelGhostID.GESpear;
                        this.pos = new Vector2(540f, 147f);
                        this.moonPresence = "Dutiful";
                        canSee = true;
                        /*AbstractSpear abstractSpear = new AbstractSpear(this.room.world, null, room.GetWorldCoordinate(this.pos), room.game.GetNewID(), false);
                        this.room.abstractRoom.AddEntity(abstractSpear);
                        (abstractSpear.realizedObject as Spear).Spear_makeNeedle(0, false);
                        (abstractSpear.realizedObject as Spear).spearmasterNeedle_fadecounter = 0;
                        /*abstractSpear.RealizeInRoom();
                        (abstractSpear.realizedObject as Spear).spearmasterNeedle = true;
                        (abstractSpear.realizedObject as Spear).spearmasterNeedle_hasConnection = false;
                        (abstractSpear.realizedObject as Spear).spearmasterNeedle_fadecounter = 0;
                        abstractSpear.needle = true;*/
                    }
                    else if (this.room.abstractRoom.name == "LF_H01" && this.room.game.cameras[0] != null && this.room.game.cameras[0].currentCameraPosition == 7 && this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GERed] == 0)
                    {
                        // Hunter
                        slugcat = GravelGhostID.GERed;
                        this.pos = new Vector2(7760f, 260f);
                        this.moonPresence = "Skillful";
                        canSee = true;
                    }
                    else if (/*this.room.abstractRoom.name == "SI_SAINTINTRO"*/ this.room.abstractRoom.name == "CC_H01SAINT" && this.room.game.cameras[0] != null && this.room.game.cameras[0].currentCameraPosition == 3 && this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GESaint] == 0)
                    {
                        // Saint
                        slugcat = GravelGhostID.GESaint;
                        //this.pos = new Vector2(360f, 90f);
                        this.pos = new Vector2(3270f, 190f);
                        this.moonPresence = "Kind";
                        canSee = true;
                    }
                    else if (this.room.abstractRoom.name == "GW_A25" && this.room.game.cameras[0] != null && this.room.game.cameras[0].currentCameraPosition == 0 && this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GEArti] == 0)
                    {
                        // Artificer
                        slugcat = GravelGhostID.GEArti;
                        this.pos = new Vector2(300f, 1790f);
                        this.moonPresence = "Mournful";
                        canSee = true;
                        if (player.glowing)
                        {
                            AbstractPhysicalObject abstractPhysicalObject = new AbstractPhysicalObject(this.room.world, MoreSlugcatsEnums.AbstractObjectType.SingularityBomb, null, room.GetWorldCoordinate(this.pos), room.game.GetNewID());
                            this.room.abstractRoom.AddEntity(abstractPhysicalObject);
                            abstractPhysicalObject.RealizeInRoom();
                        }
                    }
                    else if (this.room.abstractRoom.name == "DS_RIVSTART" && this.room.game.cameras[0] != null && this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GERiv] == 0)
                    {
                        // Rivulet
                        slugcat = GravelGhostID.GERiv;
                        this.pos = new Vector2(2535f, 590f);
                        this.moonPresence = "Carefree";
                        canSee = true;
                    }
                    else if (this.room.abstractRoom.name == "OE_CAVE02" && this.room.game.cameras[0] != null && this.room.game.cameras[0].currentCameraPosition == 1 && this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GEYellow] == 0)
                    {
                        // Monk
                        slugcat = GravelGhostID.GEYellow;
                        this.pos = new Vector2(1235f, 245f);
                        this.moonPresence = "Compassionate";
                        canSee = true;
                    }
                    else if (this.room.abstractRoom.name == "SB_GOR02RIV" && this.room.game.cameras[0] != null && this.room.game.cameras[0].currentCameraPosition == 1 && this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GEGour] == 0)
                    {
                        // Gourmand
                        slugcat = GravelGhostID.GEGour;
                        this.pos = new Vector2(1055f, 330f);
                        this.moonPresence = "Brave";
                        canSee = true;
                    }
                    else if (this.room.abstractRoom.name == "SU_A53" && this.room.game.cameras[0] != null && this.room.game.cameras[0].currentCameraPosition == 0 && this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GEWhite] == 0)
                    {
                        // Survivor
                        slugcat = GravelGhostID.GEWhite;
                        this.pos = new Vector2(495f, 85f);
                        this.moonPresence = "Instinctive";
                        canSee = true;
                    }
                    else if (this.room.abstractRoom.name == "MS_COMMS" && this.room.game.cameras[0] != null && this.room.game.cameras[0].currentCameraPosition == 2)
                    {
                        // Survivor
                        this.isInv = true;
                        this.pos = new Vector2(173f, 1128f);
                        this.moonPresence = "Lustful";
                        canSee = true;
                    }
                    else //  ADD INV FOR SECRET DATING SIM ENDING!!!
                    {
                        canSee = false;
                        ghost = null;
                    }
                }
                if (this.canSee && player.glowing)
                {
                    if(this.ghost == null)
                    {
                        this.ghost = new SlugcatGhost(this.pos, this.room);
                        this.room.AddObject(this.ghost);
                    }
                    if (RWCustom.Custom.DistLess(player.firstChunk.pos, this.pos, 75))
                    {
                        if (isInv)
                        {
                            this.room.game.manager.RequestMainProcessSwitch(MoreSlugcatsEnums.ProcessID.DatingSim);
                            this.Destroy();
                        }
                        this.room.game.GetStorySession.saveState.deathPersistentSaveData.reinforcedKarma = true;
                        this.room.game.cameras[0].hud.karmaMeter.reinforceAnimation = 0;
                        if (player.room.game.devToolsActive)
                        {
                            this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[slugcat] += 1;
                            this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW] -= 1;
                        }
                        //this.player.PlayHUDSound(SoundID.MENU_Dream_Switch);
                        this.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.BM_GOR02, 0f, 0.8f, 1f);
                        if (!this.room.world.game.GetStorySession.saveState.progression.currentSaveState.malnourished)
                        {
                            if (this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW] >= -4 && !player.room.game.devToolsActive)
                            {
                                this.player.slugcatStats.foodToHibernate -= 1;
                                FoodMeter foodbar = this.room.game.cameras[0].hud.foodMeter;
                                foodbar.survivalLimit = this.player.slugcatStats.foodToHibernate;
                                foodbar.MoveSurvivalLimit(this.player.slugcatStats.foodToHibernate, true);
                            }
                            if (this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW] <= -8)
                            {
                                //CUTSCENE!!
                            }
                        }
                        Debug.Log("GE Met Slug Ghost!");
                        //this.room.AddObject(new GhostHunch(this.room, null));
                        this.Destroy();
                    }
                    else
                    {
                        this.ghost.counter = 0;
                    }
                }
                else if(this.canSee && !player.glowing)
                {
                    //this.player.PlayHUDSound(MoreSlugcatsEnums.MSCSoundID.DreamDN);
                    this.room.AddObject(new GhostHunch(this.room, null));
                    this.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.BM_GOR01, 0f, 0.8f, 1f);
                    Debug.Log("GE Can't Meet Slug Ghost Without Neuron Glow!");
                    this.room.game.cameras[0].hud.textPrompt.AddMessage(this.room.game.manager.rainWorld.inGameTranslator.Translate("There is a " + this.moonPresence + " presence here, seek enlightenment little one"), 40, 240, false, true);
                    this.Destroy();
                }
            }
        }
    }

    public class GravelTickerID
    {
        public static void RegisterValues()
        {
            GravelQuestFinished = new Menu.StoryGameStatisticsScreen.TickerID("GourmandQuestFinished", true);
        }

        public static void UnregisterValues()
        {
            Menu.StoryGameStatisticsScreen.TickerID gravelquestfinished = GravelQuestFinished;
            if (gravelquestfinished != null)
            {
                gravelquestfinished.Unregister();
            }
            GravelQuestFinished = null;
        }

        public static Menu.StoryGameStatisticsScreen.TickerID GravelQuestFinished;
    }

    public class GravelSlideshowID
    {
        public static void RegisterValues()
        {
            GravelAltEnd = new Menu.SlideShow.SlideShowID("GravelAltEnd", true);
            GravelOutro = new Menu.SlideShow.SlideShowID("GravelOutro", true);
        }

        public static void UnregisterValues()
        {
            Menu.SlideShow.SlideShowID gravelaltend = GravelAltEnd;
            if (gravelaltend != null)
            {
                gravelaltend.Unregister();
            }
            GravelAltEnd = null;
            Menu.SlideShow.SlideShowID graveloutro = GravelOutro;
            if (graveloutro != null)
            {
                graveloutro.Unregister();
            }
            GravelOutro = null;
        }

        public static Menu.SlideShow.SlideShowID GravelAltEnd;
        public static Menu.SlideShow.SlideShowID GravelOutro;
    }
    public class GravelGhostID
    {
        // Token: 0x06003E4D RID: 15949 RVA: 0x0046C224 File Offset: 0x0046A424
        public static void RegisterValues()
        {
            GEWhite = new GhostWorldPresence.GhostID(SlugcatStats.Name.White.value, true);
            GERed = new GhostWorldPresence.GhostID(SlugcatStats.Name.Red.value, true);
            GEYellow = new GhostWorldPresence.GhostID(SlugcatStats.Name.Yellow.value, true);
            GERiv = new GhostWorldPresence.GhostID(MoreSlugcatsEnums.SlugcatStatsName.Rivulet.value, true);
            GEGour = new GhostWorldPresence.GhostID(MoreSlugcatsEnums.SlugcatStatsName.Gourmand.value, true);
            GEArti = new GhostWorldPresence.GhostID(MoreSlugcatsEnums.SlugcatStatsName.Artificer.value, true);
            GESaint = new GhostWorldPresence.GhostID(MoreSlugcatsEnums.SlugcatStatsName.Saint.value, true);
            GESpear = new GhostWorldPresence.GhostID(MoreSlugcatsEnums.SlugcatStatsName.Spear.value, true);
        }

        // Token: 0x06003E4E RID: 15950 RVA: 0x0046C284 File Offset: 0x0046A484
        public static void UnregisterValues()
        {
            GhostWorldPresence.GhostID white = GEWhite;
            if (white != null)
            {
                white.Unregister();
            }
            GEWhite = null;
            GhostWorldPresence.GhostID red = GERed;
            if (red != null)
            {
                red.Unregister();
            }
            GERed = null;
            GhostWorldPresence.GhostID yellow = GEYellow;
            if (yellow != null)
            {
                yellow.Unregister();
            }
            GEYellow = null;
            GhostWorldPresence.GhostID riv = GERiv;
            if (riv != null)
            {
                riv.Unregister();
            }
            GERiv = null;
            GhostWorldPresence.GhostID gor = GEGour;
            if (gor != null)
            {
                gor.Unregister();
            }
            GEGour = null;
            GhostWorldPresence.GhostID art = GEArti;
            if (art != null)
            {
                art.Unregister();
            }
            GEArti = null;
            GhostWorldPresence.GhostID sai = GESaint;
            if (sai != null)
            {
                sai.Unregister();
            }
            GESaint = null;
            GhostWorldPresence.GhostID spe = GESpear;
            if (spe != null)
            {
                spe.Unregister();
            }
            GESpear = null;
        }

        public static GhostWorldPresence.GhostID GEWhite;
        public static GhostWorldPresence.GhostID GERed;
        public static GhostWorldPresence.GhostID GEYellow;
        public static GhostWorldPresence.GhostID GERiv;
        public static GhostWorldPresence.GhostID GEGour;
        public static GhostWorldPresence.GhostID GEArti;
        public static GhostWorldPresence.GhostID GESaint;
        public static GhostWorldPresence.GhostID GESpear;
    }
    /*public class GravelTutorial
    {
        // Token: 0x06003E92 RID: 16018 RVA: 0x0046EE68 File Offset: 0x0046D068
        public static void RegisterValues()
        {
            GravelFull = new DeathPersistentSaveData.Tutorial("GravelFull", true);
            GravelEmpty = new DeathPersistentSaveData.Tutorial("GravelEmpty", true);
        }

        // Token: 0x06003E93 RID: 16019 RVA: 0x0046EEE8 File Offset: 0x0046D0E8
        public static void UnregisterValues()
        {
            DeathPersistentSaveData.Tutorial gravelFull = GravelFull;
            if (gravelFull != null)
            {
                gravelFull.Unregister();
            }
            GravelFull = null;
            DeathPersistentSaveData.Tutorial gravelEmpty = GravelEmpty;
            if (gravelEmpty != null)
            {
                gravelEmpty.Unregister();
            }
            GravelEmpty = null;
        }


        public static DeathPersistentSaveData.Tutorial GravelFull;

        public static DeathPersistentSaveData.Tutorial GravelEmpty;
    }*/
    public class CL_GRAVEL : UpdatableAndDeletable
    {

        public CL_GRAVEL(Room room)
        {
            this.room = room;
            this.timer = 0;
            this.tutor = false;
        }
        private int timer;
        private int message;
        private bool tutor;
        public override void Update(bool eu)
        {
            base.Update(eu);
            AbstractCreature firstAlivePlayer = this.room.game.FirstAlivePlayer;
            if (this.room.game.session is StoryGameSession && this.room.game.Players.Count > 0 && firstAlivePlayer != null && firstAlivePlayer.realizedCreature != null && firstAlivePlayer.realizedCreature.room == this.room && this.room.game.GetStorySession.saveState.cycleNumber == 0)

            {
                Player player = firstAlivePlayer.realizedCreature as Player;

                if (this.timer == 0)
                {
                    player.sceneFlag = true;
                    if (player.controller == null)
                    {
                        player.controller = new Player.NullController();
                    }
                    player.SuperHardSetPosition(new UnityEngine.Vector2(1625f, 720f));
                }

                if (this.timer > 0 && this.timer < 500)
                {
                    player.Blink(10);
                }

                if (this.timer == 300)
                {
                    player.Stun(20);
                    player.mainBodyChunk.vel = new UnityEngine.Vector2(-25f, 2f);
                }

                if (this.timer == 400)
                {
                    this.room.AddObject(new MoreSlugcats.GhostPing(this.room));
                }

                if (this.timer == 500)
                {
                    player.sceneFlag = false;
                    player.ActivateAscension();
                    player.SetMalnourished(true);
                    if (player.controller != null)
                    {
                        player.controller = null;
                    } 
                    player.Stun(15);
                    player.aerobicLevel = 1f;
                    player.exhausted = true;
                    Debug.Log("Arise, Little one!");
                    this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW] = 0;
                    //this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[MoreSlugcatsEnums.GhostID.CL] = 1;
                    this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GEWhite] = 0;
                    this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GEYellow] = 0;
                    this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GERed] = 0;
                    this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GERiv] = 0;
                    this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GEGour] = 0;
                    this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GEArti] = 0;
                    this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GESpear] = 0;
                    this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GESaint] = 0;
                }

                if ((player.mainBodyChunk.pos.y < 500f || tutor) && !(this.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ArtificerMaulTutorial)
                {
                    tutor = true;
                    if (this.room.game.session.Players[0].realizedCreature == null || this.room.game.cameras[0].hud == null || this.room.game.cameras[0].hud.textPrompt == null || this.room.game.cameras[0].hud.textPrompt.messages.Count >= 1)
                    {
                        return;
                    }
                    switch (this.message)
                    {
                        case 0:
                            //this.room.AddObject(new GhostHunch(this.room, null));
                            this.room.game.cameras[0].hud.textPrompt.AddMessage(this.room.game.manager.rainWorld.inGameTranslator.Translate("You are starving, find rocks and other objects to consume."), 40, 240, false, true);
                            this.message++;
                            return;
                        case 1:
                            this.room.game.cameras[0].hud.textPrompt.AddMessage(this.room.game.manager.rainWorld.inGameTranslator.Translate("While in the starved state, you have boosts in attacks and movement."), 0, 240, false, true);
                            this.message++;
                            return;
                        case 2:
                            this.room.game.cameras[0].hud.textPrompt.AddMessage(this.room.game.manager.rainWorld.inGameTranslator.Translate("Holding UP while swallowing or while an object is in your stomach may craft something new."), 0, 240, false, true);
                            this.message++;
                            return;
                        case 3:
                            this.room.game.GetStorySession.saveState.deathPersistentSaveData.ArtificerMaulTutorial = true;
                            //this.room.game.rainWorld.progression.miscProgressionData.starvationTutorialCounter = -1;
                            this.Destroy();
                            return;
                        default:
                            return;
                    }
                }
                else if(this.room.game.GetStorySession.saveState.deathPersistentSaveData.ArtificerMaulTutorial)
                {
                    this.Destroy();
                    return;
                }
                if (this.timer < 550)
                {
                    this.timer++;
                }
            }
        }
    }
    public class SB_A06GRAV : UpdatableAndDeletable
    {
        public SB_A06GRAV(Room room)
        {
            this.room = room;
            if(room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW] == -8)
            {
                room.roomSettings.EffectColorA = 13;
                for (int i = 0; i < room.roomSettings.effects.Count; i++)
                {
                    if (room.roomSettings.effects[i].type == RoomSettings.RoomEffect.Type.FairyParticles)
                    {
                        room.roomSettings.effects[i].amount = 0f;
                    }
                }
                for (int i = 0; i < room.roomSettings.ambientSounds.Count; i++)
                {
                    if (room.roomSettings.ambientSounds[i].sample == "Weird Deep Hole.wav")
                    {
                        room.roomSettings.ambientSounds[i].volume = 0f;
                    }
                }
            }
            for (int i = 0; i < room.roomSettings.effects.Count; i++)
            {
                if (room.roomSettings.effects[i].type == RoomSettings.RoomEffect.Type.VoidSpawn)
                {
                    this.StoredEffect = room.roomSettings.effects[i];
                    return;
                }
            }
            this.clearedSpawn = false;
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            for (int i = 0; i < this.room.game.Players.Count; i++)
            {
                if (this.room.game.Players[i].realizedCreature != null && (this.room.game.Players[i].realizedCreature as Player).room == this.room)
                {
                    Player player = this.room.game.Players[i].realizedCreature as Player;
                    player.allowOutOfBounds = true;
                    if (player.mainBodyChunk.pos.x < -248f)
                    {
                        player.SuperHardSetPosition(new UnityEngine.Vector2(this.room.RoomRect.right + 232f, player.mainBodyChunk.pos.y));
                    }
                    if (player.mainBodyChunk.pos.x > this.room.RoomRect.right + 248f)
                    {
                        player.SuperHardSetPosition(new UnityEngine.Vector2(-232f, player.mainBodyChunk.pos.y));
                    }
                    if (player.KarmaCap >= 9)
                    {
                        if (this.room.game.cameras[0].paletteBlend != this.target_blend)
                        {
                            if (Mathf.Abs(this.room.game.cameras[0].paletteBlend - this.target_blend) < 0.01f)
                            {
                                this.room.game.cameras[0].ChangeFadePalette(this.room.game.cameras[0].paletteB, this.target_blend);
                            }
                            else
                            {
                                this.room.game.cameras[0].ChangeFadePalette(this.room.game.cameras[0].paletteB, Mathf.Lerp(this.room.game.cameras[0].paletteBlend, this.target_blend, 0.1f));
                            }
                        }
                        if (player.mainBodyChunk.pos.y < -118f)
                        {
                            if(room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW] == -8)
                            {
                                WorldCoordinate newCoord = new WorldCoordinate(this.room.world.GetAbstractRoom("SB_E01GRAV").index, 175, 40, -1);
                                MSCRoomSpecificScript.RoomWarp(player, room, "SB_E01GRAV", newCoord, false);
                                return;
                            }
                            this.target_blend = Mathf.Clamp(this.target_blend + 0.1f, 0f, 1f);
                            this.room.game.cameras[0].ChangeFadePalette(this.room.game.cameras[0].paletteB, Mathf.Clamp(this.room.game.cameras[0].paletteBlend + 0.05f, 0f, 1f));
                            player.SuperHardSetPosition(new UnityEngine.Vector2(player.mainBodyChunk.pos.x, 700f));
                            if (player.Malnourished == false)
                            {
                                player.SetMalnourished(true);
                            }
                            player.Stun(300);
                            this.ClearAllVoidSpawn();
                            this.StoredEffect.amount = 0f;
                            for (int j = 0; j < player.bodyChunks.Length; j++)
                            {
                                player.bodyChunks[j].vel.y = Mathf.Clamp(player.bodyChunks[j].vel.y, -15f, 15f);
                            }
                        }
                        if (player.mainBodyChunk.pos.y > 730f && this.target_blend > 0f)
                        {
                            if (this.target_blend < 1f)
                            {
                                this.target_blend = Mathf.Clamp(this.target_blend - 0.1f, 0f, 1f);
                            }
                            player.SuperHardSetPosition(new UnityEngine.Vector2(player.mainBodyChunk.pos.x, -102f));
                            player.Stun(300);
                            this.ClearAllVoidSpawn();
                            this.StoredEffect.amount = 0f;
                        }
                        if (this.target_blend == 1f && this.fadeObj == null)
                        {
                            this.fadeObj = new MoreSlugcats.FadeOut(this.room, Color.cyan, 130f, false);
                            this.room.AddObject(this.fadeObj);
                        }
                        if (this.fadeObj != null && this.fadeObj.IsDoneFading() && !this.loadStarted)
                        {
                            this.loadStarted = true;
                            //this.room.game.overWorld.InitiateSpecialWarp(OverWorld.SpecialWarpType.WARP_VS_HR, this);
                            //MoreSlugcats.MSCRoomSpecificScript.RoomWarp(player, this.room, "HR_GRAVINTRO", default(WorldCoordinate), true);
                            //player.Die();
                            this.room.game.CustomEndGameSaveAndRestart(false);
                            RainWorldGame.ForceSaveNewDenLocation(this.room.game, "HR_GRAVINTRO", true);
                            return;
                        }
                    }
                }
            }
        }

        public Room getSourceRoom()
        {
            return this.room;
        }

        public void NewWorldLoaded()
        {
        }

        private void ClearAllVoidSpawn()
        {
            if (this.clearedSpawn)
            {
                return;
            }
            this.clearedSpawn = true;
            for (int i = 0; i < this.room.updateList.Count; i++)
            {
                if (this.room.updateList[i] is VoidSpawn)
                {
                    this.room.updateList[i].slatedForDeletetion = true;
                }
            }
        }

        public float target_blend;

        public bool loadStarted;

        public MoreSlugcats.FadeOut fadeObj;

        private RoomSettings.RoomEffect StoredEffect;

        private bool clearedSpawn;

        public MoreSlugcats.KarmaVectorX karmaObj;

        public int phaseTimer;

        public int karmaSymbolWait;
    }
    public class HR_GravelIntro : UpdatableAndDeletable
    {
        public HR_GravelIntro(Room room)
        {
            this.room = room;
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            if (this.fadeObj == null)
            {
                this.fadeObj = new MoreSlugcats.FadeOut(this.room, Color.cyan, 130f, true);
                this.room.AddObject(this.fadeObj);
            }
            if (this.room.game.AllPlayersRealized)
            {
                /*if (firstTimeLoad)
                {
                    firstTimeLoad = false;
                    this.room.world.rainCycle.GetDesiredCycleLength();
                    this.room.world.game.globalRain.ResetRain();
                }*/
                for (int i = 0; i < this.room.game.Players.Count; i++)
                {
                    Player player = this.room.game.Players[i].realizedCreature as Player;
                    if (this.waitBeforeDrop < 45)
                    {
                        player.SuperHardSetPosition(new UnityEngine.Vector2(420f, 760f));
                        for (int j = 0; j < player.bodyChunks.Length; j++)
                        {
                            player.bodyChunks[j].vel.y = Mathf.Clamp(player.bodyChunks[j].vel.y, -5f, 5f);
                            player.bodyChunks[j].vel.x = 0f;
                        }
                    }
                    else if (this.waitBeforeDrop == 45)
                    {
                        for (int k = 0; k < player.bodyChunks.Length; k++)
                        {
                            player.bodyChunks[k].vel.y = -35f;
                            player.bodyChunks[k].vel.x = 0f;
                        }
                        player.SetMalnourished(true);
                        player.graphicsModule.Reset();
                    }
                }
                if (this.waitBeforeDrop == 45)
                {
                    this.Destroy();
                }
                if (this.fadeObj != null && this.fadeObj.IsDoneFading())
                {
                    this.waitBeforeDrop++;
                }
                if (this.fadeObj != null && this.waitBeforeFade < 30)
                {
                    this.fadeObj.fade = 1f;
                }
                this.waitBeforeFade++;
            }
        }

        public int waitBeforeDrop;

        public int waitBeforeFade;

        public MoreSlugcats.FadeOut fadeObj;

        //public bool firstTimeLoad = true;
    }
    public class GRAVEL_FINAL : UpdatableAndDeletable
    {
        public GRAVEL_FINAL(Room room)
        {
            this.room = room;
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            AbstractCreature firstAlivePlayer = this.room.game.FirstAlivePlayer;
            if (firstAlivePlayer == null)
            {
                return;
            }
            if (this.player == null && this.room.game.Players.Count > 0 && firstAlivePlayer.realizedCreature != null && firstAlivePlayer.realizedCreature.room == this.room)
            {
                this.player = (firstAlivePlayer.realizedCreature as Player);
            }
            if (this.room.game.GetStorySession.saveState.deathPersistentSaveData.altEnding && !this.triggeredBoss && !this.endingTriggered)
            {
            }
            else
            {
                if (this.king != null && this.king.dead)
                {
                    if (this.player == null && this.room.game.Players.Count > 0 && firstAlivePlayer.realizedCreature != null)
                    {
                        this.player = (firstAlivePlayer.realizedCreature as Player);
                    }
                    if (this.player != null)
                    {
                        this.timeSinceDead++;
                        for (int i = 0; i < this.player.grasps.Length; i++)
                        {
                            if (this.player.grasps[i] != null && this.player.grasps[i].grabbed == this.king)
                            {
                                this.TriggerFadeToEnding();
                            }
                        }
                        if (this.player.room == null || this.player.room != this.room)
                        {
                            this.TriggerFadeToEnding();
                        }
                        if (this.timeSinceDead > 1200)
                        {
                            this.TriggerFadeToEnding();
                        }
                    }
                }
                if (this.player != null && this.player.abstractCreature.Room == this.room.abstractRoom && this.player.room != null)
                {
                    if (this.player.room.game.cameras[0] != null && !this.player.sceneFlag)
                    {
                        this.TriggerBossFight();
                    }
                }
                else
                {
                    this.player = null;
                }
                if (this.triggeredBoss && this.king != null)
                {
                    this.counter++;
                    if (this.player != null && this.player.abstractCreature.Room == this.room.abstractRoom && this.player.enteringShortCut != null)
                    {
                        Debug.Log("DENY GRAVEL EXIT");
                        this.player.enteringShortCut = null;
                        this.player.firstChunk.vel = new Vector2(10f, 2f);
                        this.player.Stun(5);
                    }
                }
                if (this.endingTriggered)
                {
                    this.endingTriggerTime++;
                    if (this.endingTriggerTime == 80)
                    {
                        RainWorldGame.ForceSaveNewDenLocation(this.room.game, "SB_S01", true);
                        this.room.game.GoToRedsGameOver();
                        RainWorldGame.BeatGameMode(this.room.game, true);
                    }
                }
            }
        }

        public void TriggerBossFight()
        {
            if (!this.triggeredBoss)
            {
                this.triggeredBoss = true;
                this.player.sceneFlag = true;
                this.room.TriggerCombatArena();
                WorldCoordinate pos = new WorldCoordinate(this.room.abstractRoom.index, 41, 13, -1);
                AbstractCreature abstractCreature = new AbstractCreature(this.room.world, StaticWorld.GetCreatureTemplate(MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.HunterDaddy), null, pos, this.room.game.GetNewID());
                abstractCreature.ignoreCycle = true;
                abstractCreature.voidCreature = true;
                this.room.abstractRoom.AddEntity(abstractCreature);
                abstractCreature.RealizeInRoom();
                this.king = (abstractCreature.realizedCreature as DaddyLongLegs);
            }
        }

        public void TriggerFadeToEnding()
        {
            this.player.controller = new Player.NullController();
            this.room.game.manager.sceneSlot = this.room.game.GetStorySession.saveStateNumber;
            this.endingTriggered = true;
        }

        private bool triggeredBoss;

        public Player player;

        public DaddyLongLegs king;

        public bool endingTriggered;

        public int endingTriggerTime;

        public int counter;

        public MoreSlugcats.FadeOut fadeIn;

        public bool firstSummon;

        public int timeSinceDead;
    }

    public class GravelFlameBreath : CosmeticSprite
    {
        public GravelFlameBreath(Vector2 position, Vector2 velocity, float lifeTime, Color fireColor)
        {
            this.pos = position;
            this.lastPos = position;
            this.velo = velocity;
            this.life = lifeTime;
            this.lifeTime = lifeTime;
            this.FireColor = fireColor;
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            this.pos += this.velo;
            this.velo -= this.velo * 0.1f;
            this.startAlpha = 1f;
            this.life -= 1f;
            /*if (coughSmoke != null)
            {
                coughSmoke.Update(eu);
                coughSmoke.EmitSmoke(this.pos, RWCustom.Custom.RNV(), RWCustom.Custom.HSL2RGB(RWCustom.Custom.Decimal(1f + EggBugGraphics.HUE_OFF), 1f, 0.5f), 20);
            }*/
            if (this.life <= 0f)
            {
                this.Destroy();
            }
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[1];
            if (UnityEngine.Random.value < 0.2f)
            {
                sLeaser.sprites[0] = new FSprite("Futile_White", true);
                sLeaser.sprites[0].shader = rCam.room.game.rainWorld.Shaders["FlatLight"];
                this.startAlpha = 0.5f;
            }
            else
            {
                sLeaser.sprites[0] = new FSprite("Pebble" + UnityEngine.Random.Range(1, 10).ToString(), true);
                sLeaser.sprites[0].shader = rCam.room.game.rainWorld.Shaders["Hologram"];
            }
            sLeaser.sprites[0].rotation = UnityEngine.Random.value * 360f;
            this.AddToContainer(sLeaser, rCam, rCam.ReturnFContainer("ForegroundLights"));
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 vector = new Vector2(Mathf.Lerp(this.lastPos.x, this.pos.x, timeStacker), Mathf.Lerp(this.lastPos.y, this.pos.y, timeStacker)) - camPos;
            sLeaser.sprites[0].x = vector.x;
            sLeaser.sprites[0].y = vector.y;
            sLeaser.sprites[0].color = this.FireColor;
            sLeaser.sprites[0].alpha = Mathf.Lerp(0f, this.startAlpha, (this.life - timeStacker) / this.lifeTime);
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            //coughSmoke = new FireSmoke(this.room);
        }

        public Vector2 velo;

        public float life;

        public float lifeTime;

        public float startAlpha;

        public Color FireColor;
    }
    public class GravelFullTutorial : UpdatableAndDeletable
    {
        public GravelFullTutorial(Room room)
        {
            this.room = room;
            if (room.game.session.Players[0].pos.room != room.abstractRoom.index)
            {
                this.Destroy();
            }
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            if (this.room.game.session.Players[0].realizedCreature == null || this.room.game.cameras[0].hud == null || this.room.game.cameras[0].hud.textPrompt == null || this.room.game.cameras[0].hud.textPrompt.messages.Count >= 1)
            {
                return;
            }
            switch (this.message)
            {
                case 0:
                    this.room.AddObject(new GhostHunch(this.room, null));
                    this.room.game.cameras[0].hud.textPrompt.AddMessage(this.room.game.manager.rainWorld.inGameTranslator.Translate("You are now nourished and full of gravel."), 40, 160, false, true);
                    this.message++;
                    return;
                case 1:
                    this.room.game.cameras[0].hud.textPrompt.AddMessage(this.room.game.manager.rainWorld.inGameTranslator.Translate("You are more heavy and durable in this state, and can withstand some damage."), 0, 160, false, true);
                    this.message++;
                    return;
                case 2:
                    this.room.game.cameras[0].hud.textPrompt.AddMessage(this.room.game.manager.rainWorld.inGameTranslator.Translate("You can spit out rocks and empty your stomach to return to the Malnourished state."), 0, 160, false, true);
                    this.message++;
                    return;
                case 3:
                    this.room.game.rainWorld.progression.miscProgressionData.starvationTutorialCounter = -1;
                    this.Destroy();
                    return;
                default:
                    return;
            }
        }

        // Token: 0x04001EE9 RID: 7913
        public int message;
    }

    public class SlugcatGhostPing : GhostPing
    {
        public SlugcatGhostPing(Room room) : base(room)
        {
            this.room = room;
            this.go = false;
            this.goAt = 40;
            this.speed = 0.0125f;
            this.alpha = 0.5f;
            if (!this.room.abstractRoom.shelter)
            {
                this.goAt = 80;
                this.speed = 0.04f;
                this.alpha = 0.3f;
            }
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            if (!this.room.BeingViewed)
            {
                if (this.go)
                {
                    this.Destroy();
                }
                return;
            }
            this.counter++;
            if (!this.go && this.counter >= this.goAt)
            {
                this.go = true;
                //this.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.Ghost_Ping_Start, 0f, 0.6f, 1f + UnityEngine.Random.value * 0.5f);
                this.room.PlaySound(SoundID.MENU_Enter_Death_Screen, 0f, 0.8f, 1.5f + UnityEngine.Random.value * 0.5f);
                //this.room.PlaySound(SoundID.Slugcat_Ghost_Dissappear, 0f, 1f, 1f);
                this.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.Ghost_Ping_Base, 0f, 1f, 1f);
                this.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.Ghost_Ping_Base, 0f, 1f, 1f);
            }
            this.lastProg = this.prog;
            if (this.go)
            {
                this.prog = Mathf.Min(1f, this.prog + this.speed);
                if (this.prog >= 1f && this.lastProg >= 1f)
                {
                    this.Destroy();
                }
            }
        }
    }
    public class GravelConfig : OptionInterface
    {
        public GravelConfig(BaseUnityPlugin plugin)
        {
            this.DisableTimer = this.config.Bind<bool>("Gravelslug_Disable_Timer", false);
            this.KeepAbilities = this.config.Bind<bool>("Gravelslug_Keep_Abilities", false);
            this.NoSinking = this.config.Bind<bool>("Gravelslug_No_Sinking", false);
        }

        public override void Initialize()
        {
            OpTab opTab = new OpTab(this, "Options");
            this.Tabs = new OpTab[]
            {
                opTab
            };
            OpContainer opContainer = new OpContainer(new Vector2(0f, 0f));
            opTab.AddItems(new UIelement[]
            {
                opContainer
            });
            UIelement[] elements = new UIelement[]
            {
                new OpLabel(0f, 550f, "Gravel Eater Config", true),
                new OpCheckBox(this.DisableTimer, 50f, 500f),
                new OpLabel(80f, 500f, "Disable Dissolve Timer", false),
                new OpCheckBox(this.NoSinking, 50f, 450f),
                new OpLabel(80f, 450f, "Disable Sinking", false),
                /*new OpCheckBox(this.KeepAbilities, 50f, 400f),
                new OpLabel(80f, 400f, "Disable Dissolve Timer", false),*/
            };
            opTab.AddItems(elements);
        }

        public Configurable<bool> DisableTimer;
        public Configurable<bool> NoSinking;
        public Configurable<bool> KeepAbilities;
        public Configurable<bool> MountainsAbound;
        public Configurable<bool> RainbowFire;
    }

    /*public class TemplateModOptions : OptionInterface
    {
        private readonly ManualLogSource Logger;

        public TemplateModOptions(TemplateMod modInstance, ManualLogSource loggerSource)
        {
            Logger = loggerSource;
            PlayerSpeed = this.config.Bind<float>("PlayerSpeed", 1f, new ConfigAcceptableRange<float>(0f, 100f));
        }

        public readonly Configurable<float> PlayerSpeed;
        private UIelement[] UIArrPlayerOptions;


        public override void Initialize()
        {
            var opTab = new OpTab(this, "Options");
            this.Tabs = new[]
            {
            opTab
        };

            UIArrPlayerOptions = new UIelement[]
            {
            new OpLabel(10f, 550f, "Options", true),
            new OpLabel(10f, 520f, "Player run speed factor"),
            new OpUpdown(PlayerSpeed, new Vector2(10f,490f), 100f, 1),

            new OpLabel(10f, 460f, "Gotta go fast!", false){ color = new Color(0.2f, 0.5f, 0.8f) }
            };
            opTab.AddItems(UIArrPlayerOptions);
        }

        public override void Update()
        {
            if (((OpUpdown)UIArrPlayerOptions[2]).GetValueFloat() > 10)
            {
                ((OpLabel)UIArrPlayerOptions[3]).Show();
            }
            else
            {
                ((OpLabel)UIArrPlayerOptions[3]).Hide();
            }
        }

    }*/

    /*public class FireNut : Rock
    {
        // Token: 0x170002DB RID: 731
        // (get) Token: 0x060011DD RID: 4573 RVA: 0x00126F11 File Offset: 0x00125111
        public FireNut.AbstractFireNut AbstrNut
        {
            get
            {
                return this.abstractPhysicalObject as FireNut.AbstractFireNut;
            }
        }
        public override void Update(bool eu)
        {
            base.Update(eu);
        }
        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[2];
            sLeaser.sprites[0] = new FSprite("JetFishEyeA", true);
            sLeaser.sprites[0].scaleX = 1.2f;
            sLeaser.sprites[0].scaleY = 1.4f;
            sLeaser.sprites[1] = new FSprite("tinyStar", true);
            sLeaser.sprites[1].scaleY = 2f;
            this.AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 vector = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);
            if (this.vibrate > 0)
            {
                vector += Custom.DegToVec(Random.value * 360f) * 2f * Random.value;
            }
            sLeaser.sprites[0].x = vector.x - camPos.x;
            sLeaser.sprites[0].y = vector.y - camPos.y;
            sLeaser.sprites[1].x = vector.x - camPos.x;
            sLeaser.sprites[1].y = vector.y - camPos.y;
            float rotation = Custom.VecToDeg(Vector3.Slerp(this.lastRotation, this.rotation, timeStacker));
            if (this.blink > 0 && Random.value < 0.5f)
            {
                sLeaser.sprites[1].color = base.blinkColor;
            }
            else
            {
                sLeaser.sprites[1].color = this.color;
            }
            sLeaser.sprites[0].rotation = rotation;
            sLeaser.sprites[1].rotation = rotation;
            if (base.slatedForDeletetion || this.room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            sLeaser.sprites[0].color = palette.blackColor;
            this.color = Color.Lerp(new Color(0f, 0.4f, 1f), palette.blackColor, Mathf.Lerp(0f, 0.5f, rCam.PaletteDarkness()));
        }

        public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            if (newContatiner == null)
            {
                newContatiner = rCam.ReturnFContainer("Items");
            }
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].RemoveFromContainer();
                newContatiner.AddChild(sLeaser.sprites[i]);
            }
        }

        public class AbstractFireNut : AbstractConsumable
        {
            public AbstractWaterNut(World world, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID, int originRoom, int consumableIndex, PlacedObject.ConsumableObjectData consumableData, bool swollen) : base(world, AbstractPhysicalObject.AbstractObjectType.Rock, realizedObject, pos, ID, originRoom, consumableIndex, consumableData)
            {
                this.swollen = swollen;
            }
        }
    }*/
}
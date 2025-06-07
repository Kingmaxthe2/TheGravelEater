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
using DevInterface;
using static MonoMod.InlineRT.MonoModRule;

namespace GravelSlug
{
    [BepInPlugin(MOD_ID, "Gravel Slug", "0.1.0")]

    class Plugin : BaseUnityPlugin
    {
        private const string MOD_ID = "kingmaxthe2.gravelslug";

        // Add hooks
        public void OnEnable()
        {
            //On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

            // Put your custom hooks here!

            //PlayerHooks.ApplyHooks();
            RoomscriptHooks.ApplyHooks();

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

            IL.PlayerGraphics.Update += PlayerGraphics_Update; //enables spitting animation for rocks

            On.RainWorld.OnModsInit += RainWorld_OnModsInit;

            On.SaveState.ctor += SaveState_ctor;
            On.RainWorldGame.GhostShutDown += RainWorldGame_GhostShutDown;
            On.GhostConversation.AddEvents += GhostOVerride; // echo dialouge
            On.GhostHunch.Update += GhostHunch_Update;
            On.World.SpawnGhost += World_SpawnGhost; // Karma Protection condition for GE

            On.SlugcatStats.NourishmentOfObjectEaten += SlugcatStats_NourishmentOfObjectEaten; // increase food amount per quest completion
            On.SlugcatStats.getSlugcatStoryRegions += SlugcatStats_getSlugcatStoryRegions; //sets story regions
            On.SlugcatStats.getSlugcatOptionalRegions += SlugcatStats_getSlugcatOptionalRegions; //sets optional regions
            On.SlugcatStats.SlugcatFoodMeter += SlugcatStats_SlugcatFoodMeter;
            On.SlugcatStats.ctor += SlugcatStats_ctor;

            On.Spear.HitSomething += Spear_HitSomething; //spear resistance and feral armor peircing
            On.Weapon.Thrown += Weapon_Thrown; //boosts thrown velocity for gravelslug
            On.Spear.Update += Spear_Update;
            On.Weapon.HitAnotherThrownWeapon += Weapon_HitAnotherThrownWeapon;

            On.Region.GetProperRegionAcronym_Timeline_string += Region_GetProperRegionAcronym; //sets sh as cl
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

            On.MoreSlugcats.MoreSlugcatsEnums.DreamID.RegisterValues += DreamID_RegisterValues;
            On.MoreSlugcats.MoreSlugcatsEnums.DreamID.UnregisterValues += DreamID_UnregisterValues;
            //On.DreamsState.StaticEndOfCycleProgress += DreamsState_StaticEndOfCycleProgress;

            On.MoreSlugcats.MoreSlugcatsEnums.SlideShowID.RegisterValues += SlideShowID_RegisterValues;
            On.MoreSlugcats.MoreSlugcatsEnums.SlideShowID.UnregisterValues += SlideShowID_UnregisterValues;
            On.Menu.SlideShow.ctor += SlideShow_ctor;
            On.MoreSlugcats.MoreSlugcatsEnums.MenuSceneID.RegisterValues += MenuSceneID_RegisterValues;
            On.MoreSlugcats.MoreSlugcatsEnums.MenuSceneID.UnregisterValues += MenuSceneID_UnregisterValues;
            On.Menu.MenuScene.BuildMSCScene += MenuScene_BuildMSCScene;

            On.Menu.CharacterSelectPage.UpdateSelectedSlugcat += CharacterSelectPage_UpdateSelectedSlugcat;
            On.Menu.SlugcatSelectMenu.CheckJollyCoopAvailable += SlugcatSelectMenu_CheckJollyCoopAvailable; // disables jolly coop for GE campaign
            On.Menu.SleepAndDeathScreen.AddPassageButton += SleepAndDeathScreen_AddPassageButton; //removes passagng
            //On.Menu.SlugcatSelectMenu.SlugcatPageContinue.ctor += SlugcatPageContinue_ctor; // food limit updates for menu {NOT IMPLEMENTED}
            On.Menu.SleepAndDeathScreen.GetDataFromGame += SleepAndDeathScreen_GetDataFromGame;

            //On.MoreSlugcats.MoreSlugcatsEnums.Tutorial.RegisterValues += Tutorial_RegisterValues;
            //On.MoreSlugcats.MoreSlugcatsEnums.Tutorial.UnregisterValues += Tutorial_UnregisterValues;

            //!!!MAKE THESE STANDALONE!!!

            On.MoreSlugcats.DustWave.Update += DustWave_Update;
            On.PhysicalObject.WeatherInertia += PhysicalObject_WeatherInertia;

            //!!!   ^      ^     ^    !!!

            On.DartMaggot.ShotUpdate += NoStick;
            On.GarbageWorm.Update += GarbageHate;
            On.EggBug.ctor += EggBug_ctor;
            On.VoidSea.VoidWorm.MainWormBehavior.Update += MainWormBehavior_Update;
            On.VoidSea.VoidSeaScene.ArtificerEndUpdate += VoidSeaScene_ArtificerEndUpdate;
            On.TubeWorm.JumpButton += TubeWorm_JumpButton;

            //On.ShelterDoor.Update += ShelterDoor_Update; // Slugcat memory ping
            On.ShelterDoor.DoorClosed += ShelterDoor_DoorClosed;

            On.Expedition.PearlDeliveryChallenge.ValidForThisSlugcat += PearlDeliveryChallenge_ValidForThisSlugcat;
            //On.Expedition.PearlHoardChallenge.Generate += PearlHoardChallenge_Generate;
            //On.Expedition.VistaChallenge.Modify

            On.DaddyGraphics.HunterDummy.ApplyPalette += HunterDummy_ApplyPalette; // Gravel long legs
            On.DaddyGraphics.ApplyPalette += DaddyGraphics_ApplyPalette;
            On.DaddyGraphics.RotBodyColor += DaddyGraphics_RotBodyColor;
            On.DaddyGraphics.ctor += DaddyGraphics_ctor; // temp colors override for HR
            On.DaddyCorruption.ctor += DaddyCorruption_ctor;
            On.DaddyCorruption.InitiateSprites += DaddyCorruption_InitiateSprites;
            On.DaddyLongLegs.Update += DaddyLongLegs_Update; // GravelEaten teleportation
            On.DaddyLongLegs.Stun += DaddyLongLegs_Stun;
            On.DaddyLongLegs.Die += DaddyLongLegs_Die;
            On.DaddyLongLegs.SpitOutOfShortCut += DaddyLongLegs_SpitOutOfShortCut;
            On.HUD.Map.Draw += Map_Draw;
            On.Menu.StoryGameStatisticsScreen.KillsTable.ctor += KillsTable_ctor;

            On.MoreSlugcats.CLOracleBehavior.InitateConversation += CLOracleBehavior_InitateConversation; // pebbles stuffs
            On.MoreSlugcats.CLOracleBehavior.InterruptRain += CLOracleBehavior_InterruptRain;
            On.MoreSlugcats.CLOracleBehavior.TalkToDeadPlayer += CLOracleBehavior_TalkToDeadPlayer;
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

            //On.SaveState.ApplyCustomEndGame += SaveState_ApplyCustomEndGame;


            new Hook(typeof(RainCycle).GetMethod("get_MusicAllowed"), (Func<RainCycle, bool> orig, RainCycle cycle) => GravelThreatCheck(cycle) || orig(cycle)); // allows threat music post cycle for GE
            new Hook(typeof(SlugcatGhost).GetMethod("get_SecondaryColor"), (Func<SlugcatGhost, Color> orig, SlugcatGhost ghost) => SlugGhostColor(ghost)); // slugcat ghost colors

        }

        private void GhostHunch_Update(On.GhostHunch.orig_Update orig, GhostHunch self, bool eu)
        {
            orig(self, eu);
            if (self.room.game.GetStorySession.characterStats.name.value == "Gravelslug" && self.ghostNumber != null && self.go && self.ghostNumber == MoreSlugcatsEnums.GhostID.CL)
            {
                if (self.room.game.session is StoryGameSession && ((self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ghostsTalkedTo[self.ghostNumber] == 1 && (self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.karmaCap < 8))
                {
                    (self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ghostsTalkedTo[self.ghostNumber] = 0;
                }
            }
        }

        private void DaddyLongLegs_SpitOutOfShortCut(On.DaddyLongLegs.orig_SpitOutOfShortCut orig, DaddyLongLegs self, IntVector2 pos, Room newRoom, bool spitOutAllSticks)
        {
            orig(self, pos, newRoom, spitOutAllSticks);
            if (self.room.game.IsStorySession && self.room.game.StoryCharacter.value == "Gravelslug" && self.HDmode)
            {
                ChunkDynamicSoundLoop loop = new ChunkDynamicSoundLoop(GravelEaten.mainBodyChunk)
                {
                    sound = SoundID.Void_Sea_Worms_Bkg_LOOP,
                    Volume = 0.55f,
                    Pitch = 0.75f
                };
                loop.InitSound();
            }
        }

        private void DaddyLongLegs_Die(On.DaddyLongLegs.orig_Die orig, DaddyLongLegs self)
        {
            orig(self);
            if(self.room.game.IsStorySession && self.room.game.StoryCharacter.value == "Gravelslug" && self.HDmode)
            {
                self.room.AddObject(new ShockWave(new Vector2(GravelEaten.mainBodyChunk.pos.x, GravelEaten.mainBodyChunk.pos.y), 300f, 0.2f, 15, false));
                self.room.PlaySound(SoundID.Coral_Circuit_Break, self.mainBodyChunk.pos, 1f, 0.5f);
                self.Destroy();
            }
        }

        private void SaveState_ApplyCustomEndGame(On.SaveState.orig_ApplyCustomEndGame orig, SaveState self, RainWorldGame game, bool addFiveCycles)
        {
            if(game.StoryCharacter.value == "Gravelslug")
            {
                self.BringUpToDate(game);
                self.deathPersistentSaveData.karma = self.deathPersistentSaveData.karmaCap;
                self.deathPersistentSaveData.karmaFlowerPosition = null;

                game.GetStorySession.AppendTimeOnCycleEnd(false);

                self.deathPersistentSaveData.winState.CycleCompleted(game);

                //self.deathPersistentSaveData.winState.ConsumeEndGame();
                //self.food = SlugcatStats.SlugcatFoodMeter(self.saveStateNumber).x - SlugcatStats.SlugcatFoodMeter(self.saveStateNumber).y;
                game.rainWorld.progression.SaveWorldStateAndProgression(false);
                return;
            }
            orig(self, game, addFiveCycles);
        }

        private void DreamsState_StaticEndOfCycleProgress(On.DreamsState.orig_StaticEndOfCycleProgress orig, SaveState saveState, string currentRegion, string denPosition, ref int cyclesSinceLastDream, ref int cyclesSinceLastFamilyDream, ref int cyclesSinceLastGuideDream, ref int inGWOrSHCounter, ref DreamsState.DreamID upcomingDream, ref DreamsState.DreamID eventDream, ref bool everSleptInSB, ref bool everSleptInSB_S01, ref bool guideHasShownHimselfToPlayer, ref int guideThread, ref bool guideHasShownMoonThisRound, ref int familyThread)
        {
            orig(saveState, currentRegion, denPosition, ref cyclesSinceLastDream, ref cyclesSinceLastFamilyDream, ref cyclesSinceLastGuideDream, ref inGWOrSHCounter, ref upcomingDream, ref eventDream, ref everSleptInSB, ref everSleptInSB_S01, ref guideHasShownHimselfToPlayer, ref guideThread, ref guideHasShownMoonThisRound, ref familyThread);
            
            if (saveState.saveStateNumber.value == "Gravelslug" &&
                saveState.progression.rainWorld.processManager.currentMainLoop is RainWorldGame loop)
            {
                upcomingDream = new DreamsState.DreamID(ExtEnum<DreamsState.DreamID>.values.GetEntry(loop.setupValues.artificerDreamTest), false);
                return;
            }
        }

        private void DreamID_UnregisterValues(On.MoreSlugcats.MoreSlugcatsEnums.DreamID.orig_UnregisterValues orig)
        {
            orig();
            GravelDreamID.UnregisterValues();
        }

        private void DreamID_RegisterValues(On.MoreSlugcats.MoreSlugcatsEnums.DreamID.orig_RegisterValues orig)
        {
            orig();
            GravelDreamID.RegisterValues();
        }

        private void ShelterDoor_DoorClosed(On.ShelterDoor.orig_DoorClosed orig, ShelterDoor self)
        {
            if(self.room.game.IsStorySession && self.room.game.GetStorySession.saveStateNumber.value == "Gravelslug" && self.room.game.GetStorySession.saveState.cycleNumber < 0)
            {
                self.room.game.GetStorySession.saveState.cycleNumber = 0;
            }
            orig(self);
        }

        private void RainWorldGame_GhostShutDown(On.RainWorldGame.orig_GhostShutDown orig, RainWorldGame self, GhostWorldPresence.GhostID ghostID)
        {
            if(self.IsStorySession && !(ModManager.Expedition && self.manager.rainWorld.ExpeditionMode) && self.GetStorySession.characterStats.name.value == "Gravelslug" && ghostID == MoreSlugcatsEnums.GhostID.CL && self.GetStorySession.saveState.deathPersistentSaveData.karmaCap < 8)
            {
                if (self.manager.upcomingProcess != null)
                {
                    return;
                }
                if (self.GetStorySession.saveState.deathPersistentSaveData.GoExploreMessage == false)
                {
                    self.GetStorySession.saveState.deathPersistentSaveData.GoExploreMessage = true;
                }
                if (self.GetStorySession.saveState.cycleNumber < 0)
                {
                    self.GetStorySession.saveState.ApplyCustomEndGame(self.GetStorySession.game, false);
                    self.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[MoreSlugcatsEnums.GhostID.CL] = 0;
                    self.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.KarmaToMaxScreen);
                    RainWorldGame.ForceSaveNewDenLocation(self, "CL_GRAVEL", true);
                    return;
                }
                self.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[MoreSlugcatsEnums.GhostID.CL] = 0;
                self.GetStorySession.saveState.dreamsState.InitiateEventDream(GravelDreamID.GravelIntroDream);
                self.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.Dream);
                return;
            }
            orig(self, ghostID);
        }

        private void SaveState_ctor(On.SaveState.orig_ctor orig, SaveState self, SlugcatStats.Name saveStateNumber, PlayerProgression progression)
        {
            if(saveStateNumber.value == "Gravelslug")
            {
                self.cycleNumber = -1;
            }
            orig(self, saveStateNumber, progression);
            if (saveStateNumber.value == "Gravelslug")
            {
                if (GravelOptionsMenu.SkipTutorial.Value)
                {
                    self.deathPersistentSaveData.ArtificerMaulTutorial = true;
                }
                self.deathPersistentSaveData.GoExploreMessage = false;
                self.deathPersistentSaveData.ghostsTalkedTo[MoreSlugcatsEnums.GhostID.CL] = 1;
                self.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GEWhite] = 0;
                self.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GEYellow] = 0;
                self.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GERed] = 0;
                self.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GERiv] = 0;
                self.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GEGour] = 0;
                self.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GEArti] = 0;
                self.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GESpear] = 0;
                self.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GESaint] = 0;
            }
        }

        private void MenuScene_BuildMSCScene(On.Menu.MenuScene.orig_BuildMSCScene orig, Menu.MenuScene self)
        {
            if (self.sceneID == GravelSceneID.GravelApproach)
            {
                BuildGravelAltEnd(1, self);
                return;
            }
            if (self.sceneID == GravelSceneID.GravelWeary)
            {
                BuildGravelAltEnd(2, self);
                return;
            }
            if (self.sceneID == GravelSceneID.GravelOffer)
            {
                BuildGravelAltEnd(3, self);
                return;
            }
            if (self.sceneID == GravelSceneID.GravelAccept)
            {
                BuildGravelAltEnd(4, self);
                return;
            }
            orig(self);
        }

        private void BuildGravelAltEnd(int sceneID, Menu.MenuScene self)
        {
            string text = "Outro Gravelslug 1_b - Approach";
            if (sceneID == 2)
            {
                text = "Outro Gravelslug 2_b - Weary";
            }
            else if (sceneID == 3)
            {
                text = "Outro Gravelslug 3_b - Offer";
            }
            else if (sceneID == 4)
            {
                text = "Outro Gravelslug 4_b - Accept";
            }
            self.sceneFolder = "Scenes" + System.IO.Path.DirectorySeparatorChar.ToString() + text;
            if (self.flatMode)
            {
                self.AddIllustration(new Menu.MenuIllustration(self.menu, self, self.sceneFolder, string.Concat(new string[]
                {
            text,
            " - Flat"
                }), new Vector2(683f, 384f), false, true));
                return;
            }
            if(sceneID == 1)
            {
                self.AddIllustration(new Menu.MenuDepthIllustration(self.menu, self, self.sceneFolder, "Outro Gravelslug 1_b - 4", new Vector2(71f, 49f), 2.5f, Menu.MenuDepthIllustration.MenuShader.LightEdges));
                self.AddIllustration(new Menu.MenuDepthIllustration(self.menu, self, self.sceneFolder, "Outro Gravelslug 1_b - 3", new Vector2(71f, 49f), 2.2f, Menu.MenuDepthIllustration.MenuShader.LightEdges));
                self.AddIllustration(new Menu.MenuDepthIllustration(self.menu, self, self.sceneFolder, "Outro Gravelslug 1_b - 2", new Vector2(71f, 49f), 1.8f, Menu.MenuDepthIllustration.MenuShader.LightEdges));
                self.AddIllustration(new Menu.MenuDepthIllustration(self.menu, self, self.sceneFolder, "Outro Gravelslug 1_b - 1", new Vector2(71f, 49f), 1.6f, Menu.MenuDepthIllustration.MenuShader.LightEdges));
            }
            if(sceneID == 2)
            {
                self.AddIllustration(new Menu.MenuDepthIllustration(self.menu, self, self.sceneFolder, "Outro Gravelslug 2_b - 5", new Vector2(71f, 49f), 4.5f, Menu.MenuDepthIllustration.MenuShader.LightEdges));
                self.AddIllustration(new Menu.MenuDepthIllustration(self.menu, self, self.sceneFolder, "Outro Gravelslug 2_b - 4", new Vector2(71f, 49f), 2.5f, Menu.MenuDepthIllustration.MenuShader.LightEdges));
                self.AddIllustration(new Menu.MenuDepthIllustration(self.menu, self, self.sceneFolder, "Outro Gravelslug 2_b - 3", new Vector2(71f, 49f), 2.5f, Menu.MenuDepthIllustration.MenuShader.LightEdges));
                self.AddIllustration(new Menu.MenuDepthIllustration(self.menu, self, self.sceneFolder, "Outro Gravelslug 2_b - 2", new Vector2(71f, 49f), 1.5f, Menu.MenuDepthIllustration.MenuShader.LightEdges));
                self.AddIllustration(new Menu.MenuDepthIllustration(self.menu, self, self.sceneFolder, "Outro Gravelslug 2_b - 1", new Vector2(71f, 49f), 1.0f, Menu.MenuDepthIllustration.MenuShader.LightEdges));
            }
        }

        private void MenuSceneID_UnregisterValues(On.MoreSlugcats.MoreSlugcatsEnums.MenuSceneID.orig_UnregisterValues orig)
        {
            orig();
            GravelSceneID.UnregisterValues();
        }

        private void MenuSceneID_RegisterValues(On.MoreSlugcats.MoreSlugcatsEnums.MenuSceneID.orig_RegisterValues orig)
        {
            orig();
            GravelSceneID.RegisterValues();
        }

        private void DaddyLongLegs_Update(On.DaddyLongLegs.orig_Update orig, DaddyLongLegs self, bool eu)
        {
            orig(self, eu);
            if (self.room != null && self.room.game.IsStorySession && self.room.game.GetStorySession.saveStateNumber.value == "Gravelslug")
            {
                if (!self.isHD || self.room == null || GravelEaten == null || self.room.game.FirstRealizedPlayer == null || self.dead)
                {
                    return;
                }

                if (self.room.game.devToolsActive && Input.GetKey(KeyCode.Backspace))
                {
                    GravelEatenBanish();
                }

                if (!self.tentaclesHoldOn && !self.Stunned)
                {
                    self.mainBodyChunk.vel += Custom.DirVec(self.mainBodyChunk.pos, self.room.game.FirstRealizedPlayer.mainBodyChunk.pos) * Mathf.Lerp(0, 1, Custom.Dist(self.mainBodyChunk.pos, self.room.game.FirstRealizedPlayer.mainBodyChunk.pos) / 500) / 4;
                }
                self.AI.behavior = DaddyAI.Behavior.Hunt;
                self.AI.tracker.SeeCreature(self.abstractCreature);
                self.AI.preyTracker.AddPrey(self.AI.tracker.RepresentationForCreature(self.room.game.FirstRealizedPlayer.abstractCreature, true));

                /*Creature prey = self.room.game.FirstRealizedPlayer;
                if (!self.dead && !self.Stunned && self.room.world.game.StoryCharacter.value == "Gravelslug" && prey.room == self.room && (prey is Player) && !prey.inShortcut && !self.inShortcut && self.shortcutDelay == 0)
                {
                    Vector2 preyPos = prey.firstChunk.pos;
                    float plyrDist = Vector2.Distance(preyPos, self.firstChunk.pos);
                    if (plyrDist < 500) return;
                    Vector2 exit = self.firstChunk.pos;
                    Vector2 exitcheck = exit;
                    IntVector2[] index = self.room.shortcutsIndex;

                    //Finds closest shortcut to player
                    if(index.Length > 1)
                    {
                        float closestDistance = float.MaxValue;
                        for (int i = 0; i < index.Length; i++)
                        {
                            Vector2 shortcutPos = index[i].ToVector2();
                            if (self.room.shortcuts[i].shortCutType == ShortcutData.Type.Normal || self.room.shortcuts[i].shortCutType == ShortcutData.Type.RoomExit)
                            {
                                continue;
                            }
                            float distanceToPlayer = Vector2.Distance(shortcutPos, preyPos);
                            if (distanceToPlayer < closestDistance && distanceToPlayer > 150)
                            {
                                closestDistance = distanceToPlayer;
                                exit = shortcutPos;
                            }
                        }

                    }
                    if (exit == exitcheck) return;
                    if (Vector2.Distance(preyPos, exit) < plyrDist)
                    {
                        self.room.AddObject(new ShockWave(new Vector2(GravelEaten.mainBodyChunk.pos.x, GravelEaten.mainBodyChunk.pos.y), 300f, 0.2f, 15, false));
                        self.SpitOutOfShortCut(IntVector2.FromVector2(exit), self.room, false);
                        self.room.AddObject(new ShockWave(new Vector2(GravelEaten.mainBodyChunk.pos.x, GravelEaten.mainBodyChunk.pos.y), 300f, 0.2f, 15, false));
                        self.room.PlaySound(SoundID.SS_AI_Give_The_Mark_Boom, new Vector2(GravelEaten.mainBodyChunk.pos.x, GravelEaten.mainBodyChunk.pos.y));
                        Debug.Log("Teleported GLL to " + self.room.shortcutData(exit).shortCutType + " " + (plyrDist - Vector2.Distance(self.firstChunk.pos, preyPos) + " distance towards player"));
                    }
                }*/
            }
        }
        private void DaddyLongLegs_Stun(On.DaddyLongLegs.orig_Stun orig, DaddyLongLegs self, int st)
        {
            orig(self, st);
            if (self.room.world.game.IsStorySession && self.room.world.game.GetStorySession.saveState.saveStateNumber.value == "Gravelslug" && self.isHD)
            {
                st /= 2;
                self.stun = st;
            }
        }
        private Challenge PearlHoardChallenge_Generate(On.Expedition.PearlHoardChallenge.orig_Generate orig, PearlHoardChallenge self)
        {
            if(ExpeditionData.slugcatPlayer.value == "Gravelslug")
            {
                List<string> list = new List<string>();
                list.AddRange(SlugcatStats.SlugcatStoryRegions(ExpeditionData.slugcatPlayer));
                list.Remove("HR");
                string text = list[UnityEngine.Random.Range(0, list.Count)];
                return new PearlHoardChallenge
                {
                    common = true,
                    amount = (int)Mathf.Lerp(2f, 5f, ExpeditionData.challengeDifficulty),
                    region = text
                };
            }
            return orig.Invoke(self);
        }

        private void SlugcatStats_ctor(On.SlugcatStats.orig_ctor orig, SlugcatStats self, SlugcatStats.Name slugcat, bool malnourished)
        {
            orig(self, slugcat, malnourished);
            if (slugcat.value == "Gravelslug")
            {
                self.bodyWeightFac = 2f;
                self.generalVisibilityBonus = 0.2f;
                self.visualStealthInSneakMode = 0.3f;
                self.loudnessFac = 1.8f;
                if (!(ModManager.Expedition && Custom.rainWorld.ExpeditionMode && ExpeditionGame.activeUnlocks.Contains("unl-agility")))
                {
                    self.runspeedFac = 0.85f;
                    self.lungsFac = (ModManager.MMF && MMF.cfgMonkBreathTime.Value) ? 0.8f : 1f;
                    self.poleClimbSpeedFac = 1.1f;
                    self.corridorClimbSpeedFac = 0.85f;
                    
                }
                if (malnourished)
                {
                    self.bodyWeightFac = 1.3f;
                    self.visualStealthInSneakMode = 0.15f;
                    self.loudnessFac = 1.4f;
                    self.throwingSkill = 1;
                    if (!(ModManager.Expedition && Custom.rainWorld.ExpeditionMode && ExpeditionGame.activeUnlocks.Contains("unl-agility")))
                    {
                        self.runspeedFac = 1.1f;
                        self.poleClimbSpeedFac = 1.1f;
                        self.corridorClimbSpeedFac = 1.05f;
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

        private IntVector2 SlugcatStats_SlugcatFoodMeter(On.SlugcatStats.orig_SlugcatFoodMeter orig, SlugcatStats.Name slugcat)
        {
            IntVector2 result = orig.Invoke(slugcat);
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
            if (self.currentMainLoop != null && self.rainWorld.progression != null && self.rainWorld.progression.miscProgressionData.currentlySelectedSinglePlayerSlugcat.value == "Gravelslug")
            {
                AudioSource[] array = FindObjectsOfType<AudioSource>();
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
            if (slideShowID == GravelSlideshowID.GravelOutro || slideShowID == GravelSlideshowID.GravelAltEnd)
            {
                self.slideShowID = slideShowID;
                self.pages.Add(new Menu.Page(self, null, "main", 0));
                self.playList = new List<Menu.SlideShow.Scene>();
                if (slideShowID == GravelSlideshowID.GravelOutro)
                {
                    if (manager.oldProcess.ID == ProcessManager.ProcessID.Game && (manager.oldProcess as RainWorldGame).GetStorySession.saveState.deathPersistentSaveData.theMark)
                    {

                    }
                    else
                    {

                    }
                }
                else if (slideShowID == GravelSlideshowID.GravelAltEnd)
                {
                    if (manager.musicPlayer != null)
                    {
                        self.waitForMusic = "RW_Outro_Theme_B";
                        self.stall = true;
                        manager.musicPlayer.MenuRequestsSong(self.waitForMusic, 1.5f, 10f);
                    }
                    self.playList.Add(new Menu.SlideShow.Scene(Menu.MenuScene.SceneID.Empty, 0f, 0f, 0f));
                    self.playList.Add(new Menu.SlideShow.Scene(GravelSceneID.GravelApproach, self.ConvertTime(0, 1, 20), self.ConvertTime(0, 4, 0), self.ConvertTime(0, 16, 2)));
                    self.playList.Add(new Menu.SlideShow.Scene(GravelSceneID.GravelWeary, self.ConvertTime(0, 18, 20), self.ConvertTime(0, 22, 0), self.ConvertTime(0, 34, 2)));
                    self.playList.Add(new Menu.SlideShow.Scene(Menu.MenuScene.SceneID.Empty, self.ConvertTime(0, 35, 0), self.ConvertTime(0, 35, 0), self.ConvertTime(0, 39, 0)));
                    for (int num7 = 1; num7 < self.playList.Count; num7++)
                        for (int num8 = 1; num8 < self.playList.Count; num8++)
                    {
                        self.playList[num8].startAt -= 1.1f;
                        self.playList[num8].fadeInDoneAt -= 1.1f;
                        self.playList[num8].fadeOutStartAt -= 1.1f;
                    }
                    self.manager.desiredCreditsSong = "RW_64 - Daze";
                    self.processAfterSlideShow = ProcessManager.ProcessID.Credits;
                    //manager.statsAfterCredits = true;
                    /*if (manager.oldProcess.ID == ProcessManager.ProcessID.Game && (manager.oldProcess as RainWorldGame).GetStorySession.saveState.deathPersistentSaveData.theMark)
                    {

                    }
                    else
                    {
                        self.playList.Add(new Menu.SlideShow.Scene(GravelSceneID.GravelApproach, 0f, 0f, 0f));
                    }*/
                }
                self.preloadedScenes = new Menu.SlideShowMenuScene[self.playList.Count];
                if (Application.platform != RuntimePlatform.Switch)
                {
                    for (int num10 = 0; num10 < self.preloadedScenes.Length; num10++)
                    {
                        self.preloadedScenes[num10] = new Menu.SlideShowMenuScene(self, self.pages[0], self.playList[num10].sceneID);
                        self.preloadedScenes[num10].Hide();
                    }
                }
                manager.RemoveLoadingLabel();
                self.NextScene();
                return;
            }
            //orig(self, manager, slideShowID);
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
            if (!GravelOptionsMenu.DisableTimer.Value && !room.game.IsArenaSession && !self.world.game.GameOverModeActive && !(room.IsGateRoom() && (room.regionGate.mode == RegionGate.Mode.ClosingAirLock || room.regionGate.mode == RegionGate.Mode.Waiting || room.regionGate.mode == RegionGate.Mode.ClosingMiddle)) && room.abstractRoom.shelter == false && room.abstractRoom.name != "HR_FINAL" && room.abstractRoom.name != "HR_AI" && room.abstractRoom.name != "SB_A06GRAV" && !room.game.devToolsActive)
            {
                GravelDissolveUpdate(self.world.game);
            }
        }

        private Color PlayerGraphics_JollyFaceColorMenu(On.PlayerGraphics.orig_JollyFaceColorMenu orig, SlugcatStats.Name slugName, SlugcatStats.Name reference, int playerNumber)
        {
            if (slugName.value == "Gravelslug")
            {
                if (Custom.rainWorld.options.jollyColorMode == Options.JollyColorMode.DEFAULT || (playerNumber == 0 && Custom.rainWorld.options.jollyColorMode != Options.JollyColorMode.CUSTOM))
                {
                    return new Color(0.7098f, 0.80784f, 0.78823f);
                }
            }
            return orig.Invoke(slugName, reference, playerNumber);
        }

        private void SleepAndDeathScreen_GetDataFromGame(On.Menu.SleepAndDeathScreen.orig_GetDataFromGame orig, Menu.SleepAndDeathScreen self, Menu.KarmaLadderScreen.SleepDeathScreenDataPackage package)
        {
            orig(self, package);
            if (package.characterStats.name.value == "Gravelslug" && !self.IsAnyDeath && self.IsSleepScreen)
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
                GravelOptionsMenu.BeatGravelDate.Value = true;
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
            if (player != null && player.room != null)
            {
                bool flag1 = player is Player;
                if (flag1 && ((player as Player).lungsExhausted || (player as Player).inShortcut))
                {
                    return;
                }
                if (flag1 && !big && !(ModManager.Expedition && player.room.game.rainWorld.ExpeditionMode) && player.room.game.IsStorySession && player.room.game.StoryCharacter.value == "Gravelslug" && GravelQuestProgress(player.room.game) > 4)
                {
                    if (UnityEngine.Random.value <= 0.25 * (GravelQuestProgress(player.room.game) - 4))
                    {
                        return;
                    }
                }
                bool flag = false;
                Color color = new Color(0f, 1f, 1f);
                if (player.Submersion == 1f)
                {
                    flag = true;
                    player.room.AddObject(new Bubble(player.firstChunk.pos, player.firstChunk.vel + Custom.DegToVec(UnityEngine.Random.value * 360f) * (big ? 8f : 6f), false, false));
                }
                if (flag1)
                {
                    color = GravelFireColor(player as Player);
                    if (GravelOptionsMenu.RainbowFire.Value)
                    {
                        color = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1f, 1f);
                    }
                    if (!player.room.game.IsArenaSession)
                    {
                        GravelDissolveSubtract(big ? 3.2f : 0.08f, player.room.game, true);
                    }
                    (player.graphicsModule as PlayerGraphics).head.vel += Custom.DirVec((player.graphicsModule as PlayerGraphics).head.pos, (player.graphicsModule as PlayerGraphics).lookDirection) * 2f;
                }
                Fire_Breath(player, false, true);
                player.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.Throw_FireSpear, player.firstChunk.pos, big ? 1f : 0.8f, UnityEngine.Random.Range(big ? 0.8f : 1.2f, big ? 1.2f : 1.6f));
                player.room.AddObject(new Explosion.ExplosionLight(player.firstChunk.pos, big ? 280f : 80f, 1f, 7, Color.white));
                player.room.AddObject(new ExplosionSpikes(player.room, player.firstChunk.pos, big ? 14 : 7, big ? 15f : 10f, 9f, big ? 5f : 4f, big ? 90f : 45f, color));
                player.room.InGameNoise(new Noise.InGameNoise(player.firstChunk.pos, big ? 6000f : 4500f, (player as PhysicalObject), big ? 4f : 3f));

                if (player is Player plr && (plr.bodyMode == Player.BodyModeIndex.ZeroG || plr.room.gravity == 0f || plr.gravity == 0f))
                {
                    float num3 = (float)plr.input[0].x;
                    float num4 = (float)plr.input[0].y;
                    while (num3 == 0f && num4 == 0f)
                    {
                        num3 = (float)(((double)UnityEngine.Random.value <= 0.33) ? 0 : (((double)UnityEngine.Random.value <= 0.5) ? 1 : -1));
                        num4 = (float)(((double)UnityEngine.Random.value <= 0.33) ? 0 : (((double)UnityEngine.Random.value <= 0.5) ? 1 : -1));
                    }
                    plr.bodyChunks[0].vel.x = (big ? 2.2f : 1.5f) * num3;
                    plr.bodyChunks[0].vel.y = (big ? 2.2f : 1.5f) * num4;
                }

                if (!flag)
                {
                    return;
                }

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
                                if (Custom.DistLess(bodyChunk.pos, player.firstChunk.pos, num * num2 + bodyChunk.rad + player.firstChunk.rad) && player.room.VisualContact(bodyChunk.pos, player.firstChunk.pos))
                                {
                                    float num3 = Mathf.InverseLerp(num * num2 + bodyChunk.rad + player.firstChunk.rad, (num * num2 + bodyChunk.rad + player.firstChunk.rad) / 2f, Vector2.Distance(bodyChunk.pos, player.firstChunk.pos));
                                    bodyChunk.vel += Custom.DirVec(player.firstChunk.pos + new Vector2(0f, player.IsTileSolid(1, 0, -1) ? -20f : 0f), bodyChunk.pos) * num3 * num2 * 3f / bodyChunk.mass;
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
                                        if (physicalObject is TubeWorm grapple)
                                        {
                                            for (int i = 0; i < (physicalObject as TubeWorm).grabbedBy.Count; i++)
                                            {
                                                if (grapple.grabbedBy[i].grabber is Player)
                                                {
                                                    return;
                                                }
                                            }
                                        }
                                        (physicalObject as Creature).Stun((int)(((ModManager.MMF && MMF.cfgIncreaseStuns.Value) ? 100f : 10f) * num3));
                                    }
                                    if (physicalObject is Leech leech)
                                    {
                                        if ((UnityEngine.Random.value < 0.033333335f || !flag) || Custom.DistLess(player.firstChunk.pos, bodyChunk.pos, player.firstChunk.rad + bodyChunk.rad + 2f))
                                        {
                                            leech.Die();
                                        }
                                        else
                                        {
                                            leech.Stun((int)(num3 * bodyChunk.submersion * Mathf.Lerp(800f, 900f, UnityEngine.Random.value)));
                                        }
                                    }
                                    if (ModManager.Watcher && physicalObject is Barnacle barnacle)
                                    {
                                        barnacle.LoseShell();
                                    }
                                    if (!flag)
                                    {
                                        if (physicalObject is ScavengerBomb bomb && (UnityEngine.Random.value < 0.35f || big))
                                        {
                                            //(physicalObject as ScavengerBomb).ignited = true;
                                            bomb.InitiateBurn();
                                        }
                                        if (physicalObject is ExplosiveSpear boomstick && (UnityEngine.Random.value < 0.35f || big))
                                        {
                                            boomstick.Ignite();
                                        }
                                        if (physicalObject is FirecrackerPlant cracker && (UnityEngine.Random.value < 0.35f || big))
                                        {
                                            cracker.Ignite();
                                        }
                                        if (physicalObject is SporePlant hive && (UnityEngine.Random.value < 0.35f || big))
                                        {
                                            hive.BeeTrigger();
                                        }
                                        if (physicalObject is SporePlant.AttachedBee bee)
                                        {
                                            bee.BreakStinger();
                                        }
                                        if (physicalObject is Spider || physicalObject is Fly)
                                        {
                                            if (Custom.DistLess(player.firstChunk.pos, bodyChunk.pos, player.firstChunk.rad + bodyChunk.rad + 2f))
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
                if (ModManager.MSC && testItem is Spear spear && spear.bugSpear)
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

        public GravelConfig GravelOptionsMenu;
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
                Logger.LogError(ex);
                Logger.LogMessage("WHOOPS");
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

        private void Player_MaulingUpdate(On.Player.orig_MaulingUpdate orig, Player self, int graspIndex)
        {
            orig(self, graspIndex);
            if (self.grasps[graspIndex] == null || (self.grasps[graspIndex].grabbed is not Oracle))
            {
                return;
            }
            if (self.maulTimer > 15 && self.grasps[graspIndex].grabbed is Oracle && GravelHasAbilities(self))
            {
                self.standing = false;
                self.Blink(5);
                if (self.maulTimer % 3 == 0)
                {
                    Vector2 b = Custom.RNV() * 3f;
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
                self.mainBodyChunk.vel += Custom.DirVec(self.mainBodyChunk.pos, vector) * 0.5f;
                self.bodyChunks[1].vel -= Custom.DirVec(self.mainBodyChunk.pos, vector) * 0.6f;
                if (self.graphicsModule != null)
                {
                    if (!Custom.DistLess(self.grasps[graspIndex].grabbedChunk.pos, (self.graphicsModule as PlayerGraphics).head.pos, self.grasps[graspIndex].grabbedChunk.rad))
                    {
                        (self.graphicsModule as PlayerGraphics).head.vel += Custom.DirVec(self.grasps[graspIndex].grabbedChunk.pos, (self.graphicsModule as PlayerGraphics).head.pos) * (self.grasps[graspIndex].grabbedChunk.rad - Vector2.Distance(self.grasps[graspIndex].grabbedChunk.pos, (self.graphicsModule as PlayerGraphics).head.pos));
                    }
                    else if (self.maulTimer % 5 == 3)
                    {
                        (self.graphicsModule as PlayerGraphics).head.vel += Custom.RNV() * 4f;
                    }
                    if (self.maulTimer > 10 && self.maulTimer % 8 == 3)
                    {
                        self.mainBodyChunk.pos += Custom.DegToVec(Mathf.Lerp(-90f, 90f, UnityEngine.Random.value)) * 4f;
                        self.grasps[graspIndex].grabbedChunk.vel += Custom.DirVec(vector, self.mainBodyChunk.pos) * 0.9f / self.grasps[graspIndex].grabbedChunk.mass;
                        for (int j = UnityEngine.Random.Range(0, 3); j >= 0; j--)
                        {
                            self.room.AddObject(new WaterDrip(Vector2.Lerp(self.grasps[graspIndex].grabbedChunk.pos, self.mainBodyChunk.pos, UnityEngine.Random.value) + self.grasps[graspIndex].grabbedChunk.rad * Custom.RNV() * UnityEngine.Random.value, Custom.RNV() * 6f * UnityEngine.Random.value + Custom.DirVec(vector, (self.mainBodyChunk.pos + (self.graphicsModule as PlayerGraphics).head.pos) / 2f) * 7f * UnityEngine.Random.value + Custom.DegToVec(Mathf.Lerp(-90f, 90f, UnityEngine.Random.value)) * UnityEngine.Random.value * self.EffectiveRoomGravity * 7f, false));
                        }
                        return;
                    }
                }
            }
        }

        private Player.ObjectGrabability Player_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {
            if (obj is Oracle oracle)
            {
                if(oracle.ID == Oracle.OracleID.SL)
                {
                    if(!Custom.DistLess(self.bodyChunks[1].pos, new Vector3(1555,145), 475f))
                    {
                        self.ReleaseGrasp(0);
                    }
                    return Player.ObjectGrabability.TwoHands;
                }
                if (oracle.ID == MoreSlugcatsEnums.OracleID.CL)
                {
                    if (!Custom.DistLess(self.bodyChunks[1].pos, new Vector3(2555, 155), 150f))
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
            if(GravelHasAbilities(plr) && plr.SlugCatClass.value == "Gravelslug" && IsGravelFeral(plr) && self.tongues[0].Attached && plr.canJump < 1 && plr.bodyMode == Player.BodyModeIndex.Default)
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
            if (self.game.StoryCharacter.value == "Gravelslug" && self.worldGhost == null)
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
                        if (!(self.game.session as StoryGameSession).saveState.deathPersistentSaveData.ghostsTalkedTo.ContainsKey(ghostID) || (self.game.session as StoryGameSession).saveState.deathPersistentSaveData.ghostsTalkedTo[ghostID] != 1 || ((self.game.session as StoryGameSession).saveState.deathPersistentSaveData.karmaCap < 8 && ghostID == MoreSlugcatsEnums.GhostID.CL))
                        {
                            flag = false;
                        }
                        else if (((self.game.session as StoryGameSession).saveState.deathPersistentSaveData.karma == (self.game.session as StoryGameSession).saveState.deathPersistentSaveData.karmaCap && (self.game.session as StoryGameSession).saveState.deathPersistentSaveData.reinforcedKarma && ghostID != MoreSlugcatsEnums.GhostID.CL)
                            || (ModManager.Expedition && self.game.rainWorld.ExpeditionMode && (self.game.session as StoryGameSession).saveState.deathPersistentSaveData.karma == (self.game.session as StoryGameSession).saveState.deathPersistentSaveData.karmaCap))
                        {
                            flag = true;
                        }
                        if ((self.game.session as StoryGameSession).saveState.cycleNumber < 0 && ghostID == MoreSlugcatsEnums.GhostID.CL)
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

        private bool PearlDeliveryChallenge_ValidForThisSlugcat(On.Expedition.PearlDeliveryChallenge.orig_ValidForThisSlugcat orig, PearlDeliveryChallenge self, SlugcatStats.Name slugcat)
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
                        if (!GravelHasAbilities(self.oracle.room.game.FirstRealizedPlayer))
                        {
                            return;
                        }
                        self.dialogBox.Interrupt(self.Translate("..."), 60);
                        return;
                    }
                    if (!GravelHasAbilities(self.oracle.room.game.FirstRealizedPlayer))
                    {
                        self.AirVoice(SoundID.SS_AI_Talk_1);
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
                if (!GravelHasAbilities(self.oracle.room.game.FirstRealizedPlayer))
                {
                    self.AirVoice(SoundID.SS_AI_Talk_5);
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
            if(self is Player player && player.SlugCatClass.value == "Gravelslug" && GravelHasAbilities(player))
            {
                if(!IsGravelFeral(player))
                {
                    if(damage < 1.2)
                    {
                        bool fatal = false;
                        if (damage >= 1)
                        {
                            fatal = true;
                            GravelRetaliate(player);
                        }
                        damage *= .4f;
                        if (fatal)
                        {
                            (player).playerState.permanentDamageTracking += damage;
                        }
                        if((player).playerState.permanentDamageTracking >= 1f)
                        {
                            (player).Die();
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
                    bool flag = GravelQuestProgress(self.room.world.game) >= 8 && GravelOptionsMenu.KeepAbilities.Value;
                    int food = self.slugcatStats.foodToHibernate - (flag ? 1 : 0);
                    FoodMeter foodbar = self.room.game.cameras[0].hud.foodMeter;
                    foodbar.survivalLimit = food;
                    foodbar.MoveSurvivalLimit(foodbar.maxFood - (flag ? 1 : 0), true);
                }
            }
        }

        public int GravelQuestProgress(RainWorldGame game)
        {
            if(game.IsStorySession && game.GetStorySession.saveState.cycleNumber > 0 && !(ModManager.Expedition && game.rainWorld.ExpeditionMode) && game.StoryCharacter.value == "Gravelslug")
            {
                DeathPersistentSaveData e = game.GetStorySession.saveState.deathPersistentSaveData;
                return e.ghostsTalkedTo[GravelGhostID.GEWhite] + e.ghostsTalkedTo[GravelGhostID.GEYellow] + 
                    e.ghostsTalkedTo[GravelGhostID.GERed] + e.ghostsTalkedTo[GravelGhostID.GESaint] + 
                    e.ghostsTalkedTo[GravelGhostID.GERiv] + e.ghostsTalkedTo[GravelGhostID.GEArti] + 
                    e.ghostsTalkedTo[GravelGhostID.GEGour] + e.ghostsTalkedTo[GravelGhostID.GESpear];
            }
            return 0;
        }

        private bool GravelHasAbilities(Player player)
        {
            if (player.SlugCatClass.value == "Gravelslug" && player.room != null) 
            {
                if (player.room.game.IsArenaSession || (player.room.game.IsStorySession && (ModManager.Expedition && player.room.game.rainWorld.ExpeditionMode)))
                {
                    return true;
                }
                return player.room.game.IsStorySession && (player.room.game.GetStorySession.characterStats.name.value != "Gravelslug" || player.room.game.GetStorySession.saveState.deathPersistentSaveData.theMark || GravelOptionsMenu.KeepAbilities.Value);
            }
            return false;
        }

        private bool IsGravelFeral(Player player)
        {
            if (player.SlugCatClass.value == "Gravelslug")
            {
                if (!GravelHasAbilities(player))
                {
                    return false;
                }
                if(player.room != null && player.room.game.IsStorySession)
                {
                    return player.Malnourished/* || (player.room.world.region != null && player.room.world.name == "HR")*/;
                }
                if(player.room != null && player.room.game.IsArenaSession)
                {
                    return player.playerState.permanentDamageTracking >= 0.05;
                }
                return false;
            }
            return false;
        }
        private void GravelRetaliate(Player player)
        {
            if(player.playerState.permanentDamageTracking <= 0)
            {
                Vector2 pos = player.firstChunk.pos;
                //Color color = (player.graphicsModule as PlayerGraphics).CharacterForColor
                player.room.AddObject(new Explosion(player.room, player, pos, 5, 200f, 10f, 0.25f, 60f, 0.5f, player, 0.8f, 0f, 0.7f));
                for (int i = 0; i < 14; i++)
                {
                    player.room.AddObject(new Explosion.ExplosionSmoke(pos, Custom.RNV() * 5f * UnityEngine.Random.value, 1f));
                }
                player.room.AddObject(new Explosion.ExplosionLight(pos, 160f, 1f, 3, Color.white));
                player.room.AddObject(new ShockWave(pos, 300f, 0.165f, 4, false));
                for (int j = 0; j < 20; j++)
                {
                    Vector2 a = Custom.RNV();
                    player.room.AddObject(new Spark(pos + a * UnityEngine.Random.value * 40f, a * Mathf.Lerp(4f, 30f, UnityEngine.Random.value), Color.white, null, 4, 18));
                }
                player.room.ScreenMovement(new Vector2?(pos), default(Vector2), 0.7f);
                for (int k = 0; k < player.abstractPhysicalObject.stuckObjects.Count; k++)
                {
                    player.abstractPhysicalObject.stuckObjects[k].Deactivate();
                }
                player.room.PlaySound(SoundID.Fire_Spear_Explode, pos);
                player.room.PlaySound(SoundID.Coral_Circuit_Break, pos, 1f, 0.5f);
                player.room.InGameNoise(new Noise.InGameNoise(pos, 8000f, player, 5f));
                player.Stun(80);
                for (int i = 0; i < player.room.abstractRoom.creatures.Count; i++)
                {
                    if (player.room.abstractRoom.creatures[i].realizedCreature != null && player.room.abstractRoom.creatures[i].realizedCreature is DaddyLongLegs daddy)
                    {
                        //player.room.abstractRoom.creatures[i].realizedCreature.stun = Math.Max(player.room.abstractRoom.creatures[i].realizedCreature.stun, 100 + (int)(UnityEngine.Random.value * 200));
                        daddy.room.AddObject(new Explosion(daddy.room, daddy, daddy.mainBodyChunk.pos, 1, 50f, 0f, 0.01f, 260f, 1f, player, 0.6f, 100f, 0f));
                    }
                }
                if (player.room.game.IsStorySession)
                {
                    player.SetMalnourished(true);
                }
            }
            SpookBoom(player);
            GravelTakeDamage(player);
        }

        private int SlugcatStats_NourishmentOfObjectEaten(On.SlugcatStats.orig_NourishmentOfObjectEaten orig, SlugcatStats.Name slugcatIndex, IPlayerEdible eatenobject)
        {
            if(slugcatIndex.value == "Gravelslug" && !(ModManager.Expedition && (eatenobject as PhysicalObject).room.game.rainWorld.ExpeditionMode) && !(eatenobject as PhysicalObject).room.game.IsArenaSession && GravelQuestProgress((eatenobject as PhysicalObject).room.game) > 4)
            {
                if (eatenobject is not FireEgg){
                    int num = orig.Invoke(slugcatIndex, eatenobject);
                    num *= 2 * (GravelQuestProgress((eatenobject as PhysicalObject).room.game) - 4);
                    return num;
                }
            }
            return orig.Invoke(slugcatIndex, eatenobject);
        }

        public Color SlugGhostColor(SlugcatGhost self)
        {
            if(self.room.game.IsStorySession && self.room.game.StoryCharacter.value == "Gravelslug")
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
                        /*if (GravelOptionsMenu.RainbowFire.Value)
                        {
                            if(UnityEngine.Random.value > 0.2)
                            {
                                fireColor = sLeaser.sprites[9].color;
                            }
                            else
                            {
                                fireColor = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1f, 1f);
                            }
                        }*/
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
                        if (!GravelHasAbilities(self.player))
                        {
                            sLeaser.sprites[9].color = Color.white;
                            return;
                        }
                        eyeColor = Color.Lerp(eyeColor, fireColor, self.player.dead ? 0 : (float)self.player.playerState.permanentDamageTracking/2 + (float)self.player.playerState.permanentDamageTracking / 4 + (float)self.player.playerState.permanentDamageTracking / 8);
                        sLeaser.sprites[9].color = eyeColor;
                        if (rCam.ghostMode > 0f && self.player.room.world.region.name != "CL")
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
                list.Add(Custom.colorToHex(col));
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
                if (GravelOptionsMenu.RainbowFire.Value)
                {
                    color = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1f, 1f);
                }
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
                GravelOptionsMenu.BeatGravel.Value = true;
                RainWorldGame.ForceSaveNewDenLocation(self, "SB_S01", true);
                self.manager.statsAfterCredits = true;
                self.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.Credits);
                return;
            }
            orig(self);
        }

        private void RainWorldGame_GoToRedsGameOver(On.RainWorldGame.orig_GoToRedsGameOver orig, RainWorldGame self)
        {
            if (self.manager.upcomingProcess != null)
            {
                return;
            }
            if (self.GetStorySession.saveState.saveStateNumber.value == "Gravelslug")
            {
                GravelOptionsMenu.BeatGravel.Value = true;
                if (GravelQuestProgress(self) >= 8)
                {
                    GravelOptionsMenu.BeatGravelAlt.Value = true;
                    self.manager.nextSlideshow = GravelSlideshowID.GravelAltEnd;
                    RainWorldGame.ForceSaveNewDenLocation(self, "OE_SEXTRA", true);
                    self.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.SlideShow);
                    return;
                }
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
            if (self.oracle.room.game.StoryCharacter.value == "Gravelslug")
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
            if (self.owner.oracle.room.game.StoryCharacter.value == "Gravelslug" && self.id == MoreSlugcatsEnums.ConversationID.Pebbles_HR)
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
            if (self.daddy.room.game.IsStorySession && self.daddy.room.game.StoryCharacter.value == "Gravelslug" && (self.daddy.room.world.name == "HR" || self.daddy.HDmode))
            {
                if (self.daddy.HDmode)
                {
                    self.daddy.effectColor = Color.cyan;
                    self.daddy.eyeColor = Color.cyan;
                    /*if (ModManager.Watcher)
                    {
                        MaskSource mask = MaskMaker.MakeSource("RippleGrab", "PlayerRippleTrail", false, null, null, null, null);
                        mask.SetProperty(0, 1f);
                        mask.SetPos(self.daddy.mainBodyChunk.pos, false);
                        mask.SetScale(Vector3.one * 7f, false);
                    }*/
                }
            }
        }

        Creature GravelEaten;

        private void GravelEatenBanish()
        {
            (GravelEaten as DaddyLongLegs).eatObjects.Add(new DaddyLongLegs.EatObject(GravelEaten.mainBodyChunk, Vector2.Distance((GravelEaten as DaddyLongLegs).MiddleOfBody, GravelEaten.mainBodyChunk.pos)));
            (GravelEaten as DaddyLongLegs).room.PlaySound(SoundID.Daddy_Digestion_LOOP, (GravelEaten as DaddyLongLegs).mainBodyChunk, false, 1f, 2f);
        }
        private IEnumerator GravelPursuer(Room room, IntVector2 pos)
        {
            bool roomFlag = room.abstractRoom.shelter || room.abstractRoom.gate || room.abstractRoom.name.Contains("_AI")/* || room.abstractRoom.altSubregionName.Contains("Depths") || room.abstractRoom.altSubregionName.Contains("Deep Reservoir")*/;

            if (GravelEaten != null && (GravelEaten.room != room || roomFlag))
            {
                GravelEaten.Destroy();
                GravelEaten = null;
            }

            yield return new WaitForSeconds(2f);

            if (GravelEaten == null && !roomFlag)
            {
                AbstractCreature abstractCreature = new AbstractCreature
                    (
                    room.world,
                    StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.HunterDaddy),
                    null,
                    room.GetWorldCoordinate(pos),
                    room.game.GetNewID()
                    )
                {
                    saveCreature = false,
                    ignoreCycle = true,
                    voidCreature = true,
                    HypothermiaImmune = true,
                    lavaImmune = true,
                    tentacleImmune = true

                };
                room.abstractRoom.AddEntity(abstractCreature);
                abstractCreature.RealizeInRoom();
                GravelEaten = abstractCreature.realizedCreature;
                GravelEaten.SetLocalGravity(0);
                (GravelEaten as DaddyLongLegs).squeezeFac = 0f;
                (GravelEaten as DaddyLongLegs).SpitOutOfShortCut(pos, room, false);
                room.AddObject(new ShockWave(new Vector2(GravelEaten.mainBodyChunk.pos.x, GravelEaten.mainBodyChunk.pos.y), 300f, 0.2f, 15, false));
                room.PlaySound(SoundID.SS_AI_Give_The_Mark_Boom, new Vector2(GravelEaten.mainBodyChunk.pos.x, GravelEaten.mainBodyChunk.pos.y));
                Debug.Log("Teleported GLL to " + room.abstractRoom.name);
            }
        }
        private void Player_SpitOutOfShortCut(On.Player.orig_SpitOutOfShortCut orig, Player self, IntVector2 pos, Room newRoom, bool spitOutAllSticks)
        {
            orig(self, pos, newRoom, spitOutAllSticks);
            if (newRoom.dustStorm && DustStormSound != null)
            {
                CurrentDustSoundRoom = newRoom;
            }
            if (newRoom.game.IsStorySession && newRoom.game.StoryCharacter.value == "Gravelslug" && !newRoom.game.devToolsActive)
            {
                float num = 1400f;
                float num3 = ((float)newRoom.world.rainCycle.dayNightCounter - num) / num;
                if ((newRoom.world.name == "HR" || num3 >= 1f) && newRoom.abstractRoom.name != "HR_FINAL")
                {
                    StartCoroutine(GravelPursuer(newRoom, pos));
                }
            }
        }

        private void VoidSeaScene_ArtificerEndUpdate(On.VoidSea.VoidSeaScene.orig_ArtificerEndUpdate orig, VoidSea.VoidSeaScene self, Player player, int timer)
        {
            if (self.room.game.StoryCharacter.value == "Gravelslug")
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
                                player.firstChunk.vel += Custom.DegToVec(UnityEngine.Random.value * 360f) * UnityEngine.Random.value;
                            }
                            if (player.input[0].y < 1)
                            {
                                player.bodyChunks[1].vel *= Mathf.Lerp(1f, 0.9f, Mathf.InverseLerp(0f, 0.33333334f, num3));
                            }
                            if ((UnityEngine.Random.value > num3 * 2f || num3 == 0f) && UnityEngine.Random.value > 0.5f)
                            {
                                player.room.AddObject(new Bubble(player.firstChunk.pos, player.firstChunk.vel + Custom.DegToVec(UnityEngine.Random.value * 360f) * Mathf.Lerp(6f, 0f, num3), false, false));
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
        DisembodiedDynamicSoundLoop DustStormSound;
        Room CurrentDustSoundRoom;
        private void DustWave_Update(On.MoreSlugcats.DustWave.orig_Update orig, DustWave self, bool eu)
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
                    DustStormSound.Volume = 0.2f + Mathf.InverseLerp((float)self.room.world.rainCycle.cycleLength, 0f, (float)self.room.world.rainCycle.TimeUntilRain) * (self.room.roomSettings.RainIntensity * self.room.roomSettings.GetEffectAmount(DLCSharedEnums.RoomEffectType.DustWave));
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
                    float x = self.room.game.globalRain.OutsidePushAround * self.room.roomSettings.GetEffectAmount(DLCSharedEnums.RoomEffectType.DustWave) * UnityEngine.Random.Range(6f, 8f);
                    float y = self.room.game.globalRain.OutsidePushAround * self.room.roomSettings.GetEffectAmount(DLCSharedEnums.RoomEffectType.DustWave) * UnityEngine.Random.Range(10f, -10f);
                    Vector2 a = new Vector2(-4f * UnityEngine.Random.Range(0.75f, 1.25f) - x, 0.1f + y);
                    a *= Mathf.InverseLerp((float)self.room.world.rainCycle.cycleLength, 0f, (float)self.room.world.rainCycle.TimeUntilRain) * (self.room.roomSettings.RainIntensity * self.room.roomSettings.GetEffectAmount(DLCSharedEnums.RoomEffectType.DustWave));
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
                    }
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
            GravelEaten = null;
        }
        private void DaddyCorruption_InitiateSprites(On.DaddyCorruption.orig_InitiateSprites orig, DaddyCorruption self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            if (rCam.room.world.game.IsStorySession && rCam.room.world.game.GetStorySession.saveStateNumber.value == "Gravelslug")
            {
                sLeaser.sprites = new FSprite[self.totalSprites];
                if (rCam.room.world.name == "HR")
                {
                    self.effectColor = Color.cyan;
                    self.eyeColor = self.effectColor;
                }
                else if (rCam.room.world.region != null)
                {
                    self.effectColor = rCam.room.world.region.regionParams.corruptionEffectColor;
                    self.eyeColor = rCam.room.world.region.regionParams.corruptionEyeColor;
                }
                else
                {
                    self.effectColor = new Color(0f, 0f, 1f);
                    self.eyeColor = self.effectColor;
                }
                foreach (DaddyCorruption.Bulb bulb in self.allBulbs)
                {
                    bulb.InitiateSprites(sLeaser, rCam);
                }
                for (int i = 0; i < self.climbTubes.Count; i++)
                {
                    self.climbTubes[i].graphic.InitiateSprites(sLeaser, rCam);
                }
                for (int j = 0; j < self.restrainedDaddies.Count; j++)
                {
                    self.restrainedDaddies[j].graphic.InitiateSprites(sLeaser, rCam);
                }
                if (ModManager.MSC)
                {
                    for (int k = 0; k < self.neuronLegs.Count; k++)
                    {
                        self.neuronLegs[k].graphic.InitiateSprites(sLeaser, rCam);
                    }
                }
                self.AddToContainer(sLeaser, rCam, null);
                self.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
            }
        }
        private void DaddyCorruption_ctor(On.DaddyCorruption.orig_ctor orig, DaddyCorruption self, Room room)
        {
            orig(self, room);
            if (room.game.IsStorySession && room.world.region != null && room.game.GetStorySession.saveStateNumber.value == "Gravelslug" && (room.world.name == "HR"))
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
            if (self.slugcatStats.name.value == "Gravelslug" && GravelHasAbilities(self))
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
                        if (ModManager.MSC && (self.FreeHand() == -1 || self.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Artificer) && self.GraspsCanBeCrafted())
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
                    if (num5 > -1 && self.wantToPickUp < 1 && (self.input[0].pckp || self.eatCounter <= 15) && self.Consious && Custom.DistLess(self.mainBodyChunk.pos, self.mainBodyChunk.lastPos, 3.6f))
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
                            Fire_Breath(self, false, true);
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
                                        self.room.AddObject(new WaterDrip(Vector2.Lerp(self.grasps[num11].grabbedChunk.pos, self.mainBodyChunk.pos, UnityEngine.Random.value) + self.grasps[num11].grabbedChunk.rad * Custom.RNV() * UnityEngine.Random.value, Custom.RNV() * 6f * UnityEngine.Random.value + Custom.DirVec(self.grasps[num11].grabbed.firstChunk.pos, (self.mainBodyChunk.pos + (self.graphicsModule as PlayerGraphics).head.pos) / 2f) * 7f * UnityEngine.Random.value + Custom.DegToVec(Mathf.Lerp(-90f, 90f, UnityEngine.Random.value)) * UnityEngine.Random.value * self.EffectiveRoomGravity * 7f, false));
                                    }
                                    Creature creature = self.grasps[num11].grabbed as Creature;
                                    creature.SetKillTag(self.abstractCreature);
                                    creature.Violence(self.bodyChunks[0], new UnityEngine.Vector2?(new Vector2(0f, 0f)), self.grasps[num11].grabbedChunk, null, Creature.DamageType.Bite, 1f, 15f);
                                    creature.stun = 5;
                                    if (creature.abstractCreature.creatureTemplate.type == DLCSharedEnums.CreatureTemplateType.Inspector)
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
                self.eatMeat = Custom.IntClamp(self.eatMeat - 1, 0, 50);
                self.maulTimer = Custom.IntClamp(self.maulTimer - 1, 0, 20);
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
                                self.firstChunk.vel += new Vector2(UnityEngine.Random.Range(-1f, 1f), 0f);
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
                                    self.bodyChunks[0].pos += Custom.DirVec(self.grasps[num13].grabbed.firstChunk.pos, self.bodyChunks[0].pos) * 2f;
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
                    if (ModManager.MSC && MMF.cfgOldTongue.Value && self.grasps[0] == null && self.grasps[1] == null && self.SaintTongueCheck())
                    {
                        Vector2 vector2 = new Vector2((float)self.flipDirection, 0.7f);
                        Vector2 normalized = vector2.normalized;
                        if (self.input[0].y > 0)
                        {
                            normalized = new Vector2(0f, 1f);
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
                        if (self.grasps[0] != null && self.grasps[0].grabbed is EnergyCell && self.mainBodyChunk.submersion > 0f)
                        {
                            flag7 = false;
                        }
                        else if (self.grasps[0] != null && self.grasps[0].grabbed is EnergyCell && self.canJump <= 0 && self.bodyMode != Player.BodyModeIndex.Crawl && self.bodyMode != Player.BodyModeIndex.CorridorClimb && self.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut && self.animation != Player.AnimationIndex.HangFromBeam && self.animation != Player.AnimationIndex.ClimbOnBeam && self.animation != Player.AnimationIndex.AntlerClimb && self.animation != Player.AnimationIndex.VineGrab && self.animation != Player.AnimationIndex.ZeroGPoleGrab)
                        {
                            (self.grasps[0].grabbed as EnergyCell).Use(false);
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
                            if (!ModManager.MSC || self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Artificer || !(self.grasps[num20].grabbed is Scavenger))
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
                                AbstractConsumable abstractConsumable = new AbstractConsumable(self.room.game.world, MoreSlugcatsEnums.AbstractObjectType.MoonCloak, null, self.room.GetWorldCoordinate(self.mainBodyChunk.pos), self.room.game.GetNewID(), -1, -1, null);
                                self.room.abstractRoom.AddEntity(abstractConsumable);
                                abstractConsumable.pos = self.abstractCreature.pos;
                                abstractConsumable.RealizeInRoom();
                                (abstractConsumable.realizedObject as MoonCloak).free = true;
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
                i => i.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>("Gourmand"),
                i => i.Match(OpCodes.Call), // this is a mess of generics; not matching this, but it's the equation call
                i => i.Match(OpCodes.Brtrue_S)
            );

            ILLabel proceedCond = c.Prev.Operand as ILLabel;

            // insert our condition
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<PlayerGraphics, bool>>(playerGraphics =>
            {
                return (playerGraphics.player.SlugCatClass.value == "Gravelslug");
            });
            // if it's true, proceed as usual
            c.Emit(OpCodes.Brtrue_S, proceedCond);
        }

        private Color DaddyGraphics_RotBodyColor(On.DaddyGraphics.orig_RotBodyColor orig, DaddyGraphics self)
        {
            if (self.owner.room != null && self.owner.room.game.IsStorySession && self.owner.room.world.game.GetStorySession.saveStateNumber.value == "Gravelslug" && self.daddy.HDmode)
            {
                return Color.Lerp(PlayerGraphics.DefaultSlugcatColor(self.owner.room.world.game.GetStorySession.characterStats.name), Color.gray, 0.4f);
            }
            return orig.Invoke(self);
        }

        private void DaddyGraphics_ApplyPalette(On.DaddyGraphics.orig_ApplyPalette orig, DaddyGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            orig(self, sLeaser, rCam, palette);
            if (self.owner.room != null && self.owner.room.game.IsStorySession && self.owner.room.world.game.GetStorySession.saveStateNumber.value == "Gravelslug")
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
            if (self.owner.owner.room != null && self.owner.owner.room.game.IsStorySession && self.owner.owner.room.world.game.GetStorySession.saveStateNumber.value == "Gravelslug")
            {
                Color color = Color.Lerp(PlayerGraphics.DefaultSlugcatColor(self.owner.owner.room.world.game.GetStorySession.characterStats.name), Color.gray, 0.4f);
                for (int i = 0; i < self.numberOfSprites - 1; i++)
                {
                    sLeaser.sprites[self.startSprite + i].color = color;
                }
                sLeaser.sprites[self.startSprite + 5].color = Color.cyan;
            }
        }

        private void Map_Draw(On.HUD.Map.orig_Draw orig, Map self, float timeStacker)
        {
            orig(self, timeStacker);
            if (self.mapData.world.game.GetStorySession.characterStats.name.value == "Gravelslug")
            {
                if ((MMF.cfgCreatureSense.Value || (ModManager.MSC && self.hud.rainWorld.safariMode)) && self.visible && (self.hud.owner as Creature).room != null)
                {
                    for (int num2 = 0; num2 < self.creatureSymbols.Count; num2++)
                    {
                        CreatureSymbol critSym = self.creatureSymbols[num2];
                        if (critSym.iconData.critType == MoreSlugcatsEnums.CreatureTemplateType.HunterDaddy)
                        {
                            Color color = Color.Lerp(PlayerGraphics.DefaultSlugcatColor(self.mapData.world.game.GetStorySession.characterStats.name), Color.gray, 0.4f);
                            critSym.myColor = color;
                        }
                    }
                }
            }
        }

        private void KillsTable_ctor(On.Menu.StoryGameStatisticsScreen.KillsTable.orig_ctor orig, Menu.StoryGameStatisticsScreen.KillsTable self, Menu.Menu menu, Menu.MenuObject owner, Vector2 pos, List<KeyValuePair<IconSymbol.IconSymbolData, int>> killsData)
        {
            orig(self, menu, owner, pos, killsData);
            if ((menu as Menu.KarmaLadderScreen).saveState.saveStateNumber.value == "Gravelslug")
            {
                for (int i = 0; i < self.killCounts.Count; i++)
                {
                    if (self.killCounts[i].symbol.iconData.critType == MoreSlugcatsEnums.CreatureTemplateType.HunterDaddy)
                    {
                        Color color = Color.Lerp(PlayerGraphics.DefaultSlugcatColor((menu as Menu.KarmaLadderScreen).saveState.saveStateNumber), Color.gray, 0.4f);
                        self.killCounts[i].symbol.myColor = color;
                    }
                }
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

        private void OverWorld_WorldLoaded(On.OverWorld.orig_WorldLoaded orig, OverWorld self, bool warpUsed)
        {
            orig(self, warpUsed);
            if (self.currentSpecialWarp == OverWorld.SpecialWarpType.WARP_VS_HR && self.game.GetStorySession.saveStateNumber.value == "Gravelslug")
            {
                MSCRoomSpecificScript.RoomWarp(self.game.FirstRealizedPlayer, self.game.FirstRealizedPlayer.room, "HR_GRAVINTRO", default(WorldCoordinate), true);
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

        private void Weapon_Thrown(On.Weapon.orig_Thrown orig, Weapon self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
        {
            orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
            if (thrownBy is Player && GravelHasAbilities(thrownBy as Player))
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
        }

        private void Player_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self)
        {
            bool flag = self.canCorridorJump > 0;
            orig(self);
            if (IsGravelFeral(self) && self.bodyMode == Player.BodyModeIndex.CorridorClimb)
            {
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
                    self.mainBodyChunk.vel.x *= 2.2f;
                    GravelCough(self, true);
                }

                if (self.animation == Player.AnimationIndex.GrapplingSwing || self.animation == Player.AnimationIndex.Flip || self.animation == Player.AnimationIndex.RocketJump || self.whiplashJump)
                {
                    self.jumpBoost *= 2.2f;
                    if (self.animation == Player.AnimationIndex.RocketJump || self.whiplashJump)
                    {
                        self.mainBodyChunk.vel *= 1.6f;
                    }
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

            if (!wasDead && GravelHasAbilities(self))
            {
                /*if(self.abstractCreature != null)
                {
                    self.abstractCreature.tentacleImmune = false;
                }*/
                StartCoroutine(VoidFireSpasm(self, true, false));
                if (GravelEaten != null && !GravelEaten.dead)
                {
                    GravelEatenBanish();
                }
            }
        }
        private void Player_SwallowObject(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            orig(self, grasp);
            if (self.SlugCatClass.value == "Gravelslug" && !GravelHasAbilities(self))
            {
                return;
            }
            bool flag = false;
            bool flag2 = false;
            if (self.SlugCatClass.value == "Gravelslug" && self.input[0].y > 0 && self.FoodInStomach > 0)
            {
                if (self.objectInStomach.type != AbstractPhysicalObject.AbstractObjectType.ScavengerBomb
                    && self.objectInStomach.type != AbstractPhysicalObject.AbstractObjectType.Spear
                    && self.objectInStomach.type != DLCSharedEnums.AbstractObjectType.SingularityBomb)
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
            if (self.SlugCatClass.value == "Gravelslug" && ((self.FoodInStomach < self.MaxFoodInStomach) || (ModManager.ModdingEnabled && GravelFatty)) || flag2)
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
                        self.room.PlaySound(DLCSharedEnums.SharedSoundID.Duck_Pop, self.firstChunk.pos, 1f, 1f + UnityEngine.Random.value);
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
                            if (s.poison > 0f)
                            {
                                self.room.AddObject(new PoisonInjecter(self, 0.22f, (10f + UnityEngine.Random.value * 8f) * (4.4f -(s.poison * 3.4f)), new HSLColor(s.poisonHue, 1f, 0.5f).rgb));
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
                                            self.room.game.session.creatureCommunities.InfluenceLikeOfPlayer(CreatureCommunities.CommunityID.Scavengers, self.room.game.world.RegionNumber, self.playerState.playerNumber, Custom.LerpMap(num, -0.5f, 0.9f, -0.3f, 0f), 0.5f, 0f);
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
                        || self.objectInStomach.type == DLCSharedEnums.AbstractObjectType.SingularityBomb)
                    {
                        if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.ScavengerBomb
                        || self.objectInStomach.type == DLCSharedEnums.AbstractObjectType.SingularityBomb)
                        {
                            self.room.PlaySound(DLCSharedEnums.SharedSoundID.Duck_Pop, self.firstChunk.pos, 1f, 1f + UnityEngine.Random.value);
                            GravelBiteParticle(self, 1);
                        }
                        if (self.objectInStomach.type == DLCSharedEnums.AbstractObjectType.SingularityBomb)
                        {
                            AbstractPhysicalObject abstractPhysicalObject = new(self.abstractCreature.Room.world, DLCSharedEnums.AbstractObjectType.SingularityBomb, null, self.room.GetWorldCoordinate(self.mainBodyChunk.pos), self.abstractCreature.Room.world.game.GetNewID());
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
                        if (self.room.game.IsArenaSession && self.room.game.GetStorySession.playerSessionRecords != null)
                        {
                            self.room.game.GetStorySession.playerSessionRecords[self.playerState.playerNumber].AddEat(self.objectInStomach.realizedObject);
                        }
                        if (!self.room.game.rainWorld.ExpeditionMode && !self.room.game.IsArenaSession && GravelQuestProgress(self.room.game) > 4)
                        {
                            for (int i = 0; i < (GravelQuestProgress(self.room.game) - 4); i++)
                            {
                                self.AddQuarterFood();
                            }
                        }
                        else if (self.room.game.IsArenaSession)
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
                        if (GravelVinki && self.objectInStomach.type.value == "SprayCan")
                        {
                            /*self.room.abstractRoom.AddEntity(self.objectInStomach);
                            self.objectInStomach.RealizeInRoom();
                            self.objectInStomach.pos = self.abstractCreature.pos;
                            (self.objectInStomach.realizedObject as Weapon).HitByWeapon((self.objectInStomach.realizedObject as Weapon));*/
                            self.room.PlaySound(SoundID.Firecracker_Disintegrate, self.firstChunk.pos, 0.6f, UnityEngine.Random.Range(0.8f, 1.2f));
                            int amount = UnityEngine.Random.Range(2, 5);
                            if (GravelOptionsMenu.AteVinkiCan.Value != true)
                            {
                                GravelOptionsMenu.AteVinkiCan.Value = true;
                                GravelOptionsMenu.RainbowFire.Value = true;
                                self.room.game.cameras[0].hud.textPrompt.AddMessage(self.room.game.manager.rainWorld.inGameTranslator.Translate("Unlocked new config for Gravel Eater!"), 40, 240, false, false);
                            }
                            for (int i = 0; i < amount; i++)
                            {
                                Fire_Breath(self, true, true);
                            }
                            self.AddQuarterFood();
                            self.Stun(15);
                        }
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
                    player.room.AddObject(new ScavengerBomb.BombFragment(player.firstChunk.pos, Custom.RNV() * Mathf.Lerp(1f, 5f, UnityEngine.Random.value)));
                }
            }
            else if (type == 2)
            {
                player.room.AddObject(new ExplosiveSpear.SpearFragment(player.firstChunk.pos, Custom.RNV() * Mathf.Lerp(1f, 5f, UnityEngine.Random.value)));
            }
            for (int i = 0; i < 3; i++)
            {
                player.room.AddObject(new WaterDrip(player.firstChunk.pos, Custom.DegToVec(UnityEngine.Random.value * 360f) * Mathf.Lerp(1f, 10f, UnityEngine.Random.value), false));
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
        private void CLOracleBehavior_InitateConversation(On.MoreSlugcats.CLOracleBehavior.orig_InitateConversation orig, CLOracleBehavior self)
        {
            if (self.oracle.room.world.game.GetStorySession.saveStateNumber.value == "Gravelslug" && !GravelHasAbilities(self.oracle.room.game.FirstRealizedPlayer))
            {
                self.AirVoice(SoundID.SS_AI_Talk_2);
                return;
            }
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
                "MS",
                "HR"
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
            if (self.slugcatStats.name.value == "Gravelslug" && GravelHasAbilities(self))
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
            if (self.player.slugcatStats.name.value == "Gravelslug" && self.player.room != null)
            {
                self.markAlpha = 0f;
                if (self.player.room.game.IsStorySession && self.player.room.game.StoryCharacter.value == "Gravelslug")
                {
                    self.lastMarkAlpha = 0f;
                }
                if (IsGravelFeral(self.player) && !self.player.dead && self.player.stun < 10)
                {
                    self.drawPositions[0, 0] += Custom.RNV() * UnityEngine.Random.value * 0.4f * 2f;
                    self.drawPositions[0, 1] += Custom.RNV() * UnityEngine.Random.value * 0.4f * 2f;
                    self.head.pos += Custom.RNV() * UnityEngine.Random.value * 0.4f * 1f;
                }
                if (GravelHasAbilities(self.player) && !self.player.dead && self.player.FoodInStomach < self.player.MaxFoodInStomach && self.objectLooker.currentMostInteresting != null && !(self.player.bodyMode == Player.BodyModeIndex.Crawl || self.player.bodyMode == Player.BodyModeIndex.WallClimb || self.player.animation == Player.AnimationIndex.StandOnBeam || self.player.animation == Player.AnimationIndex.AntlerClimb) && self.objectLooker.currentMostInteresting is Rock && Custom.DistLess(self.player.mainBodyChunk.pos, self.objectLooker.mostInterestingLookPoint, 80f) && self.player.room.VisualContact(self.player.mainBodyChunk.pos, self.objectLooker.mostInterestingLookPoint))
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
                            self.drawPositions[0, 0] += Custom.DirVec(self.drawPositions[0, 0], self.objectLooker.mostInterestingLookPoint) * 5f;
                            self.head.vel += Custom.DirVec(self.drawPositions[0, 0], self.objectLooker.mostInterestingLookPoint) * 2f;
                        }
                    }
                }
            }
        }
        public bool GravelThreatCheck(RainCycle self)
        {
            return self.world.game.IsArenaSession || self.TimeUntilRain >= 2400 || (ModManager.MSC && self.world.game.IsStorySession && (self.world.game.GetStorySession.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Saint || self.world.game.GetStorySession.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Rivulet || self.world.game.GetStorySession.saveStateNumber.value == "Gravelslug")) || (self.world.game.IsStorySession && self.world.region != null && (self.world.region.name == "SS" || (ModManager.MSC && (self.world.region.name == "RM" || self.world.region.name == "DM" || self.world.region.name == "LC" || self.world.region.name == "OE"))));
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
                    if (self.ghost.room.game.GetStorySession.saveState.deathPersistentSaveData.GoExploreMessage == false)
                    {
                        self.events.Add(new Conversation.TextEvent(self, 0, "Hello, little one.", 7));
                        self.events.Add(new Conversation.TextEvent(self, 0, "I have awakened you from your deep slumber.", 7));
                        self.events.Add(new Conversation.TextEvent(self, 0, "A being such as I cannot get another chance such as this.", 8));
                        self.events.Add(new Conversation.TextEvent(self, 0, "Here I am again, to set forth another simple creature.", 8));
                        self.events.Add(new Conversation.TextEvent(self, 0, "I will continue to be alongside you until the time is right.", 8));
                        self.events.Add(new Conversation.TextEvent(self, 0, "What you decide to do with your new chance at life is yours alone.", 8));
                        self.events.Add(new Conversation.TextEvent(self, 0, "Return to me when you need me most.", 8));
                        //self.ghost.room.game.GetStorySession.saveState.deathPersistentSaveData.GoExploreMessage = true;
                    }
                    else if (self.ghost.room.game.GetStorySession.saveState.deathPersistentSaveData.karma < 8)
                    {

                        self.events.Add(new Conversation.TextEvent(self, 0, "My presence has been revealed to you once again, little one.", 8));
                        self.events.Add(new Conversation.TextEvent(self, 0, "I will help you to the best of my abilities.", 7));
                        self.events.Add(new Conversation.TextEvent(self, 0, "Seek memories of your kind, visions of times beyond your own.", 8));
                        self.events.Add(new Conversation.TextEvent(self, 0, "I have your sight, and you have mine.", 7));
                        self.events.Add(new Conversation.TextEvent(self, 0, "Use it well.", 7));
                    }
                    else
                    {
                        self.events.Add(new Conversation.TextEvent(self, 0, "My full presence has been revealed to you now, little one.", 7));
                        self.events.Add(new Conversation.TextEvent(self, 0, "Your attunement has become much closer to our own.", 7));
                        self.events.Add(new Conversation.TextEvent(self, 0, "Through awakening you, I have seen through your eyes and met your grievances.", 8));
                        self.events.Add(new Conversation.TextEvent(self, 0, "And here I am again, to set you forth to a world beyond my reach.", 8));
                        self.events.Add(new Conversation.TextEvent(self, 0, "Take care of yourself little one.", 8));
                    }
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
            if (self.slugcatStats.name.value == "Gravelslug" && GravelHasAbilities(self))
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
                if (self.room.abstractRoom.creatures[i].creatureTemplate.type == CreatureTemplate.Type.Slugcat && (player.slugcatStats.name.value == "Gravelslug" && GravelHasAbilities(player)) && !self.State.angryAt.Contains(self.room.abstractRoom.creatures[i].ID))
                {
                    self.State.angryAt.Add(self.room.abstractRoom.creatures[i].ID);
                }
            }

        }
        private void Player_Destroy(On.Player.orig_Destroy orig, Player self)
        {
            orig(self);
            if (self.SlugCatClass.value == "Gravelslug" && GravelHasAbilities(self))
            {
                GravelKaboom(self, true);
                if (GravelEaten != null && !GravelEaten.dead)
                {
                    GravelEatenBanish();
                }
            }
            
        }
        static float GravelGutDissolveTimer = 120f;

        private void GravelDissolveUpdate(RainWorldGame game)
        {
            AbstractCreature firstAlivePlayer = game.FirstAlivePlayer;
            if (!(game.Players.Count > 0 && firstAlivePlayer != null && firstAlivePlayer.realizedCreature != null) && !(game.IsStorySession && game.StoryCharacter.value == "Gravelslug" && !game.GetStorySession.saveState.deathPersistentSaveData.theMark))
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
                            if (!(ModManager.Expedition && player.room.game.rainWorld.ExpeditionMode) && player.room.game.StoryCharacter.value == "Gravelslug" && GravelQuestProgress(game) >= 8 && !player.Malnourished)
                            {
                                player.Stun(60);
                            }
                            else
                            {
                                GravelTakeDamage(player);
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
            if (player.FoodInStomach > 0)
            {
                TummyItem(player);
                player.SubtractFood(1);
            }
            else
            {
                GravelTakeDamage(player);
            }
            
        }

        private void GravelTakeDamage(Player player)
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
                player.slugcatStats.bodyWeightFac = 1.3f;
                player.slugcatStats.visualStealthInSneakMode = 0.15f;
                player.slugcatStats.loudnessFac = 1.4f;
                player.slugcatStats.throwingSkill = 1;
                player.slugcatStats.runspeedFac = 1.1f;
                player.slugcatStats.poleClimbSpeedFac = 1.1f;
                player.slugcatStats.corridorClimbSpeedFac = 1.05f;
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
            if (GravelOptionsMenu.DisableTimer.Value || GravelQuestProgress(game) >= 8)
            {
                return;
            }
            if (amount >= 200)
            {
                GravelGutDissolveTimer = 0;
                return;
            }
            if (game.StoryCharacter.value == "Gravelslug" && GravelQuestProgress(game) != 0 && !(ModManager.Expedition && game.rainWorld.ExpeditionMode))
            {
                if (ignoreReduction)
                {
                    GravelGutDissolveTimer -= amount;
                }
                else
                {
                    GravelGutDissolveTimer -= amount - (amount * (GravelQuestProgress(game) / 8));
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

        private void Fire_Breath(Creature crit, bool rainbow, bool eu)
        {
            Color FireColor = Color.cyan;
            if (crit is Player player)
            {
                FireColor = GravelFireColor(player);
                player.Blink(30);
            }
            if (rainbow || GravelOptionsMenu.RainbowFire.Value)
            {
                FireColor = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1f, 1f);
            }
            FireSmoke coughSmoke = new FireSmoke(crit.room);
            
            if (coughSmoke != null)
            {
                coughSmoke.Update(eu);

                if (crit.room.ViewedByAnyCamera(crit.firstChunk.pos, 300f) && crit.Submersion != 1f)
                {
                    coughSmoke.EmitSmoke(crit.firstChunk.pos, Custom.RNV(), FireColor, 20);
                }
            }
        }
        private void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);
            if (self.SlugCatClass.value == "Gravelslug" && self.room != null)
            {

                //Nourishment tutorial

                if (!GravelOptionsMenu.SkipTutorial.Value && self.room.game.IsStorySession && 
                    !(ModManager.Expedition && self.room.game.rainWorld.ExpeditionMode) && self.abstractCreature.world.game.StoryCharacter.value == "Gravelslug" && 
                    self.AI == null && self.room.game.session is StoryGameSession && 
                    (self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ArtificerMaulTutorial && 
                    !(self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ArtificerTutorialMessage &&
                    (self.room.game.session as StoryGameSession).saveState.cycleNumber >= 0 && !self.Malnourished)
                {
                    (self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ArtificerTutorialMessage = true;
                    //self.room.AddObject(new GhostHunch(self.room, null));
                    self.room.game.cameras[0].hud.textPrompt.AddMessage(self.room.game.manager.rainWorld.inGameTranslator.Translate("You are now nourished and full of gravel, lessening the pain within."), 40, 240, false, true);
                    self.room.game.cameras[0].hud.textPrompt.AddMessage(self.room.game.manager.rainWorld.inGameTranslator.Translate("You are heavier and more durable in this state, you sink in water and can withstand some projectiles."), 0, 240, false, true);
                    self.room.game.cameras[0].hud.textPrompt.AddMessage(self.room.game.manager.rainWorld.inGameTranslator.Translate("You can spit out rocks and empty your stomach to reduce weight and return to the Malnourished state."), 0, 240, false, true);
                }

                //Mountains Abound ability

                if(GravelOptionsMenu.MountainsAbound.Value && self.room.game.IsStorySession && 
                    self.KarmaIsReinforced && !IsGravelFeral(self) && self.wantToJump > 0 && 
                    self.input[0].pckp && self.canJump <= 0 && self.input[0].y > 0)
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
                }


                if (!GravelHasAbilities(self)) //ability check
                {
                    return;
                }

                //cosmetic flame breath

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
                            if (GravelOptionsMenu.RainbowFire.Value)
                            {
                                color = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1f, 1f);
                            }
                            if (self.bodyMode == Player.BodyModeIndex.Crawl)
                            {
                                vector = playerGraphics.lookDirection * 16f;
                                b.x = (float)self.flipDirection * 20f;
                            }
                            GravelFlameBreath firebreath = new GravelFlameBreath(pos + b + vector, Custom.RNV() * 0.2f + vector * 0.1f + self.firstChunk.vel * 0.25f, UnityEngine.Random.value * 20f + 5f, color);
                            self.room.AddObject(firebreath);
                        }
                    }
                }

                //Sinking mechanic

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
                                self.slowMovementStun = Math.Max(self.slowMovementStun, (int)Custom.LerpMap(self.aerobicLevel, 0.7f, 0.4f, 6f, 0f));
                                self.mainBodyChunk.vel.y -= ((8 * foodPercentage) * 0.04f);
                                if(self.airInLungs <= 0 && foodPercentage == 1 && GravelOptionsMenu.Sank.Value != true)
                                {
                                    GravelOptionsMenu.Sank.Value = true;
                                    self.room.game.cameras[0].hud.textPrompt.AddMessage(self.room.game.manager.rainWorld.inGameTranslator.Translate("Unlocked new config for Gravel Eater!"), 40, 240, false, false);
                                }
                            }
                        }
                    }
                }

                //Roll Removal for nourished GE

                if (!IsGravelFeral(self) && self.animation == Player.AnimationIndex.Roll)
                {
                    self.Stun(40);
                    Debug.Log("Denied roll");
                }
            }
        }
        private void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (GravelEaten != null)
            {
                GravelEaten = null;
            }
            if(self.slugcatStats.name.value == "Gravelslug")
            {
                if (world.game.IsStorySession && (!ModManager.Expedition || (ModManager.Expedition && !self.room.game.rainWorld.ExpeditionMode)))
                {
                    if (world.game.StoryCharacter.value == "Gravelslug")
                    {
                        GravelDissolveReset(world.game);
                        if (world.game.GetStorySession.saveState.cycleNumber <= 0)
                        {
                            if (GravelQuestProgress(world.game) > 4)
                            {
                                self.slugcatStats.foodToHibernate = 4;
                                if (GravelQuestProgress(world.game) >= 8)
                                {
                                    //temp fix for old quest tracking method
                                    if (world.game.GetStorySession.saveState.deathPersistentSaveData.karmaCap >= 8 && self.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[MoreSlugcatsEnums.GhostID.CL] == 0)
                                    {
                                        self.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[MoreSlugcatsEnums.GhostID.CL] = 1;
                                    }

                                    if (self.room.game.GetStorySession.saveState.deathPersistentSaveData.theMark)
                                    {
                                        self.room.game.GetStorySession.saveState.deathPersistentSaveData.theMark = false;
                                    }
                                    self.slugcatStats.runspeedFac = 1f;
                                    self.slugcatStats.bodyWeightFac = 1f;
                                    self.slugcatStats.visualStealthInSneakMode = 0.5f;
                                    self.slugcatStats.loudnessFac = 1f;
                                    self.slugcatStats.poleClimbSpeedFac = 1f;
                                    self.slugcatStats.corridorClimbSpeedFac = 1f;
                                    self.slugcatStats.maxFood -= 1;
                                    if (world.game.GetStorySession.saveState.progression.currentSaveState.malnourished)
                                    {
                                        self.slugcatStats.foodToHibernate = 7;
                                        self.slugcatStats.throwingSkill = 0;
                                        self.slugcatStats.corridorClimbSpeedFac = 1f;
                                        self.slugcatStats.bodyWeightFac = Mathf.Min(self.slugcatStats.bodyWeightFac, 0.9f);
                                        self.slugcatStats.runspeedFac = 0.875f;
                                        self.slugcatStats.poleClimbSpeedFac = 0.8f;
                                        self.slugcatStats.corridorClimbSpeedFac = 0.86f;
                                    }
                                }
                            }
                            else
                            {
                                self.slugcatStats.foodToHibernate -= GravelQuestProgress(world.game);
                            }
                        }
                        if (self.room.world.region.name == "HR")
                        {
                            world.game.GetStorySession.saveState.deathPersistentSaveData.ripPebbles = true;
                            //self.SetMalnourished(true);
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
            if(self.mode == DartMaggot.Mode.StuckInChunk && self.stuckInChunk.owner is Player player)
            {
                if(GravelHasAbilities(player) && (player.SlugCatClass.value == "Gravelslug") && !IsGravelFeral(player))
                {
                    self.Unstuck();
                    self.firstChunk.vel *= Custom.RNV() * Mathf.Lerp(1f, 5f, UnityEngine.Random.value);
                    Debug.Log("dart maggot failed to pierce Gravel Eater");
                }
            }
        }
        private bool BoxORocks(On.Player.orig_SlugSlamConditions orig, Player self, PhysicalObject otherObject)
        {
            if (self.SlugCatClass.value == "Gravelslug" && GravelHasAbilities(self))
            {
                if (IsGravelFeral(self))
                {
                    return false;
                }
                if ((otherObject as Creature).abstractCreature.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.SlugNPC)
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
                Vector2 vel = self.bodyChunks[0].vel;
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
                return !ModManager.CoopAvailable || !(otherObject is Player) || Custom.rainWorld.options.friendlyFire;
            }
            else
            {
                return orig(self, otherObject);
            }

        }
        private float ThickSkinned(On.Player.orig_DeathByBiteMultiplier orig, Player self)
        {
            if (GravelHasAbilities(self) && !IsGravelFeral(self))
            {
                return 0.15f;
            }
            return orig.Invoke(self);
        }
        private void NeverGiveUp(On.Player.orig_Grabbed orig, Player self, Creature.Grasp grasp)
        {
            orig(self, grasp);
            if (GravelHasAbilities(self) && !IsGravelFeral(self))
            {
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
                if (player != null && (player.isNPC || !Custom.rainWorld.options.friendlyFire))
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
                if (!GravelHasAbilities(self))
                {
                    return;
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
            GravelCough(player, true);
            spookyCough = new FirecrackerPlant.ScareObject(player.firstChunk.pos);
            player.room.AddObject(spookyCough);
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
                    if (ded)
                    {
                        player.room.AddObject(new CreatureSpasmer(player, true, 100));
                        player.firstChunk.vel += new Vector2(UnityEngine.Random.Range(-3f, 3f), 5f);
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

        public static Color GravelFireColor(Player player)
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
            if (GravelOptionsMenu.RainbowFire.Value)
            {
                color = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1f, 1f);
            }
            room.AddObject(new Explosion(room, player, pos, 7, 225f, 4.2f, 50f, 280f, 0.25f, player, 0.7f, 160f, 1f));
            room.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
            room.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
            room.AddObject(new ExplosionSpikes(room, pos, 14, 30f, 9f, 7f, 170f, color));
            room.InGameNoise(new Noise.InGameNoise(pos, 9000f, player, 5f));
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
                    room.AddObject(new Explosion.ExplosionSmoke(pos, Custom.RNV() * 5f * UnityEngine.Random.value, 1f));
                }
                for (int j = 0; j < 20; j++)
                {
                    Vector2 a = Custom.RNV();
                    room.AddObject(new Spark(pos + a * UnityEngine.Random.value * 40f, a * Mathf.Lerp(4f, 30f, UnityEngine.Random.value), color, null, 4, 18));
                }
                room.AddObject(new SootMark(room, pos, 80f, true));
                room.AddObject(new ShockWave(pos, 450f, 0.165f, 5, false));
                room.AddObject(new Smolder(room, pos, player.firstChunk, null));
                player.firstChunk.vel += new Vector2(UnityEngine.Random.Range(-15f, 15f), 30f);
                
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
                if (GravelOptionsMenu.RainbowFire.Value)
                {
                    color = UnityEngine.Random.Range(0f, 1f);
                }
                AbstractPhysicalObject craftedObject;
                if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.Spear)
                {
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
                else if (self.objectInStomach.type == DLCSharedEnums.AbstractObjectType.GlowWeed)
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
                    if((self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.NSHSwarmer
                    || self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.SLOracleSwarmer
                    || self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer) && GravelOptionsMenu.CraftSingularity.Value)
                    {
                        craftedObject = new AbstractConsumable(self.room.world, DLCSharedEnums.AbstractObjectType.SingularityBomb, null, self.abstractCreature.pos, self.room.game.GetNewID(), -1, -1, null)
                        {
                            isFresh = false,
                            isConsumed = true
                        };
                    }
                    else //add inv egg crafting
                    {
                        Debug.Log("Gravel Eater cannot craft with " + self.objectInStomach.type + "!");
                        self.Regurgitate();
                        if(self.objectInStomach.type != AbstractPhysicalObject.AbstractObjectType.NSHSwarmer
                    && self.objectInStomach.type != AbstractPhysicalObject.AbstractObjectType.SLOracleSwarmer
                    && self.objectInStomach.type != AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer)
                        {
                            SpookBoom(self);
                        }
                        return;
                    }
                }
                else
                {
                    if (self.objectInStomach is AbstractCreature)
                    {
                        self.objectInStomach = null;
                    }
                    craftedObject = new FireEgg.AbstractBugEgg(self.room.world, null, self.abstractCreature.pos, self.room.game.GetNewID(), color);
                }
                self.objectInStomach = craftedObject;
                Debug.Log("Coverted Gravel Stomach item from " + self.objectInStomach.type + " to " + craftedObject.type);
                self.Regurgitate();
            }
        }
        private bool MoreSwallows(On.Player.orig_CanBeSwallowed orig, Player self, PhysicalObject testObj)
        {
            if (GravelHasAbilities(self))
            {
                return (testObj is Spear && (self.input[0].y > 0 || self.FoodInStomach < self.MaxFoodInStomach)
                    || (testObj is IPlayerEdible && (self.input[0].y > 0 || (self.FoodInStomach >= self.MaxFoodInStomach && testObj is not Creature)))
                    || testObj is NeedleEgg || testObj is Rock || testObj is DataPearl || testObj is FlareBomb || testObj is Lantern || testObj is FirecrackerPlant
                    || (testObj is VultureGrub && !(testObj as VultureGrub).dead)
                    || (testObj is Hazer && !(testObj as Hazer).dead && !(testObj as Hazer).hasSprayed)
                    || (testObj is SSOracleSwarmer && self.FoodInStomach >= self.MaxFoodInStomach)
                    || (ModManager.MSC && testObj is FireEgg && self.FoodInStomach >= self.MaxFoodInStomach)
                    || testObj is FlyLure || testObj is ScavengerBomb || testObj is PuffBall || testObj is SporePlant || testObj is BubbleGrass || testObj is NSHSwarmer || testObj is OverseerCarcass
                    || (ModManager.MSC && testObj is SingularityBomb && !(testObj as SingularityBomb).activateSingularity && !(testObj as SingularityBomb).activateSucktion));
            }
            return orig.Invoke(self, testObj);
        }
        private static string Region_GetProperRegionAcronym(On.Region.orig_GetProperRegionAcronym_Timeline_string orig, SlugcatStats.Timeline time, string baseAcronym)
        {
            if (time.value  == "Gravelslug")
            {
                if (baseAcronym == "SH") baseAcronym = "CL";
            }
            return orig(time, baseAcronym);
        }

        //NOT MY CODE!! ↓   ↓
        public void WormGrassPatch_InteractWithCreature(On.WormGrass.WormGrassPatch.orig_InteractWithCreature orig, WormGrass.WormGrassPatch self, WormGrass.WormGrassPatch.CreatureAndPull creatureAndPull)
        {
            Player player = creatureAndPull.creature as Player;
            bool flag = player == null || !(player.slugcatStats.name.value == "Gravelslug" && GravelHasAbilities(player)) && !IsGravelFeral(player);
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
                bool flag = player != null && GravelHasAbilities(player) && !IsGravelFeral(player);
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
                bool flag = player != null && (GravelHasAbilities(player)) && !IsGravelFeral(player);
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
            }
        }
        private void Spear_Update(On.Spear.orig_Update orig, Spear self, bool eu)
        {
            orig(self, eu);
            if (self.mode == Weapon.Mode.StuckInWall && self.vibrate == 10 && self.thrownBy is Player player && IsGravelFeral(player) && self.bugSpear)
            {
                IntVector2 firstpos = self.abstractPhysicalObject.pos.Tile;
                self.stuckInWall = new Vector2?(self.room.MiddleOfTile(self.firstChunk.pos + (self.rotation * 20f)));

                foreach (AbstractWorldEntity abstractWorldEntity in self.room.abstractRoom.entities)
                {
                    if (abstractWorldEntity is AbstractSpear stuckspear && stuckspear.realizedObject != null && (stuckspear.realizedObject as Weapon).mode == Weapon.Mode.StuckInWall && (abstractWorldEntity.pos.Tile == self.abstractPhysicalObject.pos.Tile || abstractWorldEntity.pos.Tile == firstpos))
                    {
                        self.room.AddObject(new ExplosiveSpear.SpearFragment(stuckspear.realizedObject.firstChunk.pos, Custom.DegToVec(UnityEngine.Random.value * 360f)));
                        abstractWorldEntity.Destroy();
                    }
                }

                self.room.PlaySound(SoundID.Seed_Cob_Open, self.firstChunk);
                self.room.AddObject(new Smolder(self.room, self.firstChunk.pos, self.firstChunk, null));
                self.abstractSpear.stuckInWallCycles *= 3;
            }
        }

        private void Weapon_HitAnotherThrownWeapon(On.Weapon.orig_HitAnotherThrownWeapon orig, Weapon self, Weapon obj)
        {
            if (self is Spear spear && spear.bugSpear && self.thrownBy != null && self.thrownBy is Player player && player.SlugCatClass.value == "Gravelslug")
            {
                if (obj.firstChunk.pos.x - obj.firstChunk.lastPos.x < 0f == self.firstChunk.pos.x - self.firstChunk.lastPos.x < 0f)
                {
                    return;
                }
                if (self.abstractPhysicalObject.world.game.IsArenaSession)
                {
                    self.abstractPhysicalObject.world.game.GetArenaGameSession.arenaSitting.players[0].parries++;
                }
                Vector2 vector = Vector2.Lerp(obj.firstChunk.lastPos, self.firstChunk.lastPos, 0.5f);
                int num = 5;
                if (obj is Spear)
                {
                    num += 2;
                }
                for (int i = 0; i < num; i++)
                {
                    self.room.AddObject(new Spark(vector + Custom.DegToVec(UnityEngine.Random.value * 360f) * 5f * UnityEngine.Random.value, Custom.DegToVec(UnityEngine.Random.value * 360f) * Mathf.Lerp(2f, 7f, UnityEngine.Random.value) * (float)num, new Color(1f, 1f, 1f), null, 10, 170));
                }
                Vector2 vector2 = Custom.DegToVec(UnityEngine.Random.value * 360f);
                obj.WeaponDeflect(vector, -vector2, self.firstChunk.vel.magnitude);
                self.room.PlaySound(SoundID.Spear_Bounce_Off_Creauture_Shell, vector, self.abstractPhysicalObject);
                return;
            }
            orig(self, obj);
        }
        private bool Spear_HitSomething(On.Spear.orig_HitSomething orig, Spear self, SharedPhysics.CollisionResult result, bool eu)
        {
            bool result2 = orig.Invoke(self, result, eu);
            float num = (self.spearDamageBonus);
            if (result.obj == null)
            {
                return result2;
            }
            if (result.obj.abstractPhysicalObject.rippleLayer != self.abstractPhysicalObject.rippleLayer && !result.obj.abstractPhysicalObject.rippleBothSides && !self.abstractPhysicalObject.rippleBothSides)
            {
                return result2;
            }
            if (result.obj is Player player && (GravelHasAbilities(player)) && !IsGravelFeral(player))
            {
                player.Violence(self.firstChunk, new UnityEngine.Vector2?(self.firstChunk.vel * self.firstChunk.mass * 2f), result.chunk, result.onAppendagePos, Creature.DamageType.Stab, num * 0.4f, 20f);
                self.LodgeInCreature(result, eu);
                player.playerState.permanentDamageTracking += (num * 0.4f / player.Template.baseDamageResistance);
                self.room.PlaySound(SoundID.Spear_Stick_In_Creature, self.firstChunk);
                result2 = true;
                player.Stun(1);
                return result2;
            }
            if (self.thrownBy is Player player1 && IsGravelFeral(player1) && self.bugSpear)
            {
                result2 = orig.Invoke(self, result, eu);
                if (result.obj is Creature crit && (crit).SpearStick(self, Mathf.Lerp(0.55f, 0.62f, UnityEngine.Random.value), result.chunk, result.onAppendagePos, self.firstChunk.vel))
                {
                    self.room.AddObject(new CreatureSpasmer(crit, false, 100));
                    if (ModManager.Watcher && crit is Barnacle barnacle)
                    {
                        barnacle.LoseShell();
                    }
                    if (result2 == false && result.onAppendagePos == null)
                    {
                        crit.Violence(self.firstChunk, new UnityEngine.Vector2?(self.firstChunk.vel * self.firstChunk.mass * 2f), result.chunk, result.onAppendagePos, Creature.DamageType.Stab, (num *= 0.5f), 20f);
                        self.LodgeInCreature(result, true);
                        self.room.PlaySound(SoundID.Seed_Cob_Open, self.firstChunk);
                        self.room.PlaySound(SoundID.Spear_Stick_In_Ground, self.firstChunk);
                        self.room.AddObject(new Smolder(self.room, result.chunk.pos, result.chunk, null));
                        result2 = true;
                        Debug.Log("Pierced Creature Armor!");
                    }
                }
                else if (result.obj is SeedCob cob)
                {
                    cob.ApplyForceOnAppendage(result.onAppendagePos, self.firstChunk.vel * self.firstChunk.mass);
                    cob.Open();
                    self.room.PlaySound(SoundID.Spear_Stick_In_Creature, self.firstChunk);
                    self.LodgeInCreature(result, eu);
                    result2 = true;
                    Debug.Log("Woah! Secret Message for you! :3");
                }
            }
            return result2;
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

    public class GravelSceneID
    {
        public static void RegisterValues()
        {
            GravelApproach = new Menu.MenuScene.SceneID("GravelApproach", true);
            GravelWeary = new Menu.MenuScene.SceneID("GravelWeary", true);
            GravelOffer = new Menu.MenuScene.SceneID("GravelOffer", true);
            GravelAccept = new Menu.MenuScene.SceneID("GravelAccept", true);
        }

        public static void UnregisterValues()
        {
            Menu.MenuScene.SceneID Gravelapproach = GravelApproach;
            if (Gravelapproach != null)
            {
                Gravelapproach.Unregister();
            }
            GravelApproach = null;
            Menu.MenuScene.SceneID Gravelweary = GravelWeary;
            if (Gravelweary != null)
            {
                Gravelweary.Unregister();
            }
            GravelWeary = null;
            Menu.MenuScene.SceneID Graveloffer = GravelOffer;
            if (Graveloffer != null)
            {
                Graveloffer.Unregister();
            }
            GravelOffer = null;
            Menu.MenuScene.SceneID Gravelaccept = GravelAccept;
            if (Gravelaccept != null)
            {
                Gravelaccept.Unregister();
            }
            GravelAccept = null;
        }

        public static Menu.MenuScene.SceneID GravelApproach;
        public static Menu.MenuScene.SceneID GravelWeary;
        public static Menu.MenuScene.SceneID GravelOffer;
        public static Menu.MenuScene.SceneID GravelAccept;
    }
    public class GravelDreamID
    {
        public static void RegisterValues()
        {
            GravelIntroDream = new DreamsState.DreamID("GravelIntroDream", true);
            GravelMemoryDream = new DreamsState.DreamID("GravelMemoryDream", true);
        }

        public static void UnregisterValues()
        {
            DreamsState.DreamID gravelintrodream = GravelIntroDream;
            if (gravelintrodream != null)
            {
                gravelintrodream.Unregister();
            }
            GravelIntroDream = null;
            DreamsState.DreamID gravelmemorydream = GravelMemoryDream;
            if (gravelmemorydream != null)
            {
                gravelmemorydream.Unregister();
            }
            GravelIntroDream = null;
        }
        public static DreamsState.DreamID GravelIntroDream;
        public static DreamsState.DreamID GravelMemoryDream;
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
            this.DisableTimer = this.config.Bind("Gravelslug_Disable_Timer", false);
            this.TimerMult = this.config.Bind("Gravelslug_Timer_Multiplier", 1f);
            this.KeepAbilities = this.config.Bind("Gravelslug_Keep_Abilities", false);
            this.NoSinking = this.config.Bind("Gravelslug_No_Sinking", false);
            this.MountainsAbound = this.config.Bind("Gravelslug_Secret_Ability", false);
            this.RainbowFire = this.config.Bind("Gravelslug_Rainbow_Fire", false);
            this.SkipTutorial = this.config.Bind("Gravelslug_Skip_Tutorial", false);
            this.CraftSingularity = this.config.Bind("Gravelslug_Neuron_Craft", false);

            this.BeatGravel = this.config.Bind("Gravelslug_End", false);
            this.BeatGravelAlt = this.config.Bind("Gravelslug_True_End", false);
            this.BeatGravelDate = this.config.Bind("Gravelslug_Met_Inv", false);
            this.AteVinkiCan = this.config.Bind("Gravelslug_Vinki", false);
            this.Sank = this.config.Bind("Gravelslug_Sank", false);
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
            UIelement[] elements1 = new UIelement[]
            {
                new OpLabel(0f, 550f, "Gravel Eater Config", true),
                new OpCheckBox(this.SkipTutorial, 50f, 500f),
                new OpLabel(80f, 500f, "Skip tutorial", false),
            };
            opTab.AddItems(elements1);
            if (Sank.Value)
            {
                UIelement[] elements3 = new UIelement[]
            {
                new OpCheckBox(this.NoSinking, 50f, 450f),
                new OpLabel(80f, 450f, "Disable Sinking", false),
            };
                opTab.AddItems(elements3);
            }
            else
            {
                UIelement[] elements5 = new UIelement[]
                {
                    new OpRect(new Vector2(50f, 450f), new Vector2(25f, 25f), 0.1f)
                {
                    doesBump = true,
                },
                    new OpLabel(80f, 450f, "LOCKED (Sink like a rock)", false),
                };
                opTab.AddItems(elements5);
            }
            if (BeatGravel.Value || BeatGravelAlt.Value)
            {
                UIelement[] elements2 = new UIelement[]
            {
                new OpUpdown(this.TimerMult, new Vector2(50f, 180f), 50f, 1),
                new OpLabel(110f, 180f, "Dissolve Timer Drain Multiplier", false),
            };
                opTab.AddItems(elements2);
            }
            if (BeatGravel.Value)
            {
                UIelement[] elements3 = new UIelement[]
            {
                new OpCheckBox(this.MountainsAbound, 50f, 400f),
                new OpLabel(80f, 400f, "Unlocks secret cut ability", false),
            };
                opTab.AddItems(elements3);
            }
            else
            {
                UIelement[] elements5 = new UIelement[]
                {
                    new OpRect(new Vector2(50f, 400f), new Vector2(25f, 25f), 0.1f)
                {
                    doesBump = true,
                },
                    new OpLabel(80f, 400f, "LOCKED (beat campain)", false),
                };
                opTab.AddItems(elements5);
            }
            if (BeatGravelAlt.Value)
            {
                UIelement[] elements4 = new UIelement[]
            {
                new OpCheckBox(this.KeepAbilities, 50f, 350f),
                new OpLabel(80f, 350f, "Keep abilities post quest", false),
            };
                opTab.AddItems(elements4);
            }
            else
            {
                UIelement[] elements5 = new UIelement[]
                {
                    new OpRect(new Vector2(50f, 350f), new Vector2(25f, 25f), 0.1f)
                {
                    doesBump = true,
                },
                    new OpLabel(80f, 350f, "LOCKED (unlockable soon...)", false),
                };
                opTab.AddItems(elements5);
            }
            if (AteVinkiCan.Value)
            {
                UIelement[] elements5 = new UIelement[]
                {
                    new OpCheckBox(this.RainbowFire, 50f, 300f),
                    new OpLabel(80f, 300f, "Enables rainbow fire particles", false),
                };
                opTab.AddItems(elements5);
            }
            else
            {
                UIelement[] elements5 = new UIelement[]
                {
                    new OpRect(new Vector2(50f, 300f), new Vector2(25f, 25f), 0.1f)
                {
                    doesBump = true,
                },
                    new OpLabel(80f, 300f, "LOCKED (Da Vinki??)", false),
                };
                opTab.AddItems(elements5);
            }
            if (BeatGravelDate.Value)
            {
                UIelement[] elements5 = new UIelement[]
                {
                    new OpCheckBox(this.CraftSingularity, 50f, 250f),
                    new OpLabel(80f, 250f, "Craft Singularity Bombs with Neurons", false),
                };
                opTab.AddItems(elements5);
            }
            else
            {
                UIelement[] elements5 = new UIelement[]
                {
                    new OpRect(new Vector2(50f, 250f), new Vector2(25f, 25f), 0.1f)
                {
                    doesBump = true,
                },
                    new OpLabel(80f, 250f, "LOCKED (???)", false),
                };
                opTab.AddItems(elements5);
            }
        }

        public Configurable<bool> DisableTimer;
        public Configurable<float> TimerMult;
        public Configurable<bool> NoSinking;
        public Configurable<bool> KeepAbilities;
        public Configurable<bool> MountainsAbound;
        public Configurable<bool> RainbowFire;
        public Configurable<bool> CraftSingularity;
        public Configurable<bool> HardMode;
        public Configurable<bool> SkipTutorial;
        //unlocks
        public Configurable<bool> BeatGravel;
        public Configurable<bool> BeatGravelAlt;
        public Configurable<bool> BeatGravelDate;
        public Configurable<bool> Sank;
        public Configurable<bool> AteVinkiCan;
    }

}
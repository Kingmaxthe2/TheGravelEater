using UnityEngine;
using Smoke;
using MoreSlugcats;
using HUD;
using RWCustom;

namespace GravelSlug
{
    public static class RoomScripts
    {
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
            private int questcounter;
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
                        if (this.room.abstractRoom.name == "SI_A07" && this.room.game.cameras[0] != null && this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GESpear] == 0)
                        {
                            // Spearmaster
                            if (this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GESpear] == 0)
                            {
                                slugcat = GravelGhostID.GESpear;
                                this.moonPresence = "Dutiful";
                                canSee = true;
                            }
                            this.pos = new Vector2(540f, 147f);
                        }
                        else if (this.room.abstractRoom.name == "LF_H01" && this.room.game.cameras[0] != null && this.room.game.cameras[0].currentCameraPosition == 7 && this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GERed] == 0)
                        {
                            // Hunter
                            if (this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GERed] == 0)
                            {
                                slugcat = GravelGhostID.GERed;
                                this.moonPresence = "Skillful";
                                canSee = true;
                            }
                            this.pos = new Vector2(7760f, 260f);
                        }
                        else if (this.room.abstractRoom.name == "CC_H01SAINT" && this.room.game.cameras[0] != null && this.room.game.cameras[0].currentCameraPosition == 3 && this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GESaint] == 0)
                        {
                            // Saint
                            if (this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GESaint] == 0)
                            {
                                slugcat = GravelGhostID.GESaint;
                                this.moonPresence = "Kind";
                                canSee = true;
                            }
                            this.pos = new Vector2(3270f, 190f);
                        }
                        else if (this.room.abstractRoom.name == "GW_A25" && this.room.game.cameras[0] != null && this.room.game.cameras[0].currentCameraPosition == 0 && this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GEArti] == 0)
                        {
                            // Artificer
                            if (this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GEArti] == 0)
                            {
                                slugcat = GravelGhostID.GEArti;
                                this.moonPresence = "Mournful";
                                canSee = true;
                                if (player.glowing)
                                {
                                    AbstractPhysicalObject abstractPhysicalObject = new AbstractPhysicalObject(this.room.world, DLCSharedEnums.AbstractObjectType.SingularityBomb, null, room.GetWorldCoordinate(this.pos), room.game.GetNewID());
                                    this.room.abstractRoom.AddEntity(abstractPhysicalObject);
                                    abstractPhysicalObject.RealizeInRoom();
                                }
                            }
                            this.pos = new Vector2(300f, 1790f);
                        }
                        else if (this.room.abstractRoom.name == "DS_RIVSTART" && this.room.game.cameras[0] != null && this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GERiv] == 0)
                        {
                            // Rivulet
                            if (this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GERiv] == 0)
                            {
                                slugcat = GravelGhostID.GERiv;
                                this.moonPresence = "Carefree";
                                canSee = true;
                            }
                            this.pos = new Vector2(2535f, 590f);
                        }
                        else if (this.room.abstractRoom.name == "OE_CAVE02" && this.room.game.cameras[0] != null && this.room.game.cameras[0].currentCameraPosition == 1 && this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GEYellow] == 0)
                        {
                            // Monk
                            if (this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GEYellow] == 0)
                            {
                                slugcat = GravelGhostID.GEYellow;
                                this.moonPresence = "Compassionate";
                                canSee = true;
                            }
                            this.pos = new Vector2(1235f, 245f);
                        }
                        else if (this.room.abstractRoom.name == "SB_GOR02RIV" && this.room.game.cameras[0] != null && this.room.game.cameras[0].currentCameraPosition == 1)
                        {
                            // Gourmand
                            if (this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GEGour] == 0)
                            {
                                slugcat = GravelGhostID.GEGour;
                                this.moonPresence = "Brave";
                                canSee = true;
                            }
                            this.pos = new Vector2(1055f, 330f);
                        }
                        else if (this.room.abstractRoom.name == "SU_A53" && this.room.game.cameras[0] != null && this.room.game.cameras[0].currentCameraPosition == 0)
                        {
                            // Survivor
                            if (this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GravelGhostID.GEWhite] == 0)
                            {
                                slugcat = GravelGhostID.GEWhite;
                                this.moonPresence = "Instinctive";
                                canSee = true;
                            }
                            this.pos = new Vector2(495f, 85f);
                        }
                        else if (this.room.abstractRoom.name == "MS_COMMS" && this.room.game.cameras[0] != null && this.room.game.cameras[0].currentCameraPosition == 2)
                        {
                            // Survivor
                            this.isInv = true;
                            this.pos = new Vector2(173f, 1128f);
                            this.moonPresence = "Lustful";
                            canSee = true;
                        }
                        else
                        {
                            canSee = false;
                            ghost = null;
                        }
                    }
                    if (this.room.game.GetStorySession.saveState.denPosition == this.room.abstractRoom.name && this.room.world.rainCycle.timer < 400)
                    {
                        player.SuperHardSetPosition(this.pos);
                        player.abstractCreature.pos = this.room.ToWorldCoordinate(this.pos);
                        player.sleepCurlUp = 1f;
                        player.sleepCounter = 99;
                        this.Destroy();
                    }
                    if (this.canSee && (player.glowing || ((this.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.karma == (this.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.karmaCap)))
                    {
                        if (this.ghost == null)
                        {
                            this.ghost = new SlugcatGhost(this.pos, this.room);
                            this.room.AddObject(this.ghost);
                        }
                        if (Custom.DistLess(player.firstChunk.pos, this.pos, 30 - 0))
                        {
                            if (isInv)
                            {
                                this.room.game.manager.RequestMainProcessSwitch(MoreSlugcatsEnums.ProcessID.DatingSim);
                                this.Destroy();
                            }
                            this.room.game.GetStorySession.saveState.deathPersistentSaveData.reinforcedKarma = true;
                            this.room.game.cameras[0].hud.karmaMeter.reinforceAnimation = 0;
                            if (!player.room.game.devToolsActive)
                            {
                                this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[slugcat] += 1;
                            }
                            DeathPersistentSaveData e = room.game.GetStorySession.saveState.deathPersistentSaveData;
                            this.questcounter = e.ghostsTalkedTo[GravelGhostID.GEWhite] + e.ghostsTalkedTo[GravelGhostID.GEYellow] +
                                e.ghostsTalkedTo[GravelGhostID.GERed] + e.ghostsTalkedTo[GravelGhostID.GESaint] +
                                e.ghostsTalkedTo[GravelGhostID.GERiv] + e.ghostsTalkedTo[GravelGhostID.GEArti] +
                                e.ghostsTalkedTo[GravelGhostID.GEGour] + e.ghostsTalkedTo[GravelGhostID.GESpear];
                            if (questcounter == 0)
                            {
                                questcounter = 1;
                            }
                            this.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.BM_GOR02, 0f, 0.8f, 1f);
                            if (!player.room.game.devToolsActive && questcounter <= 4 && !(this.room.world.game.GetStorySession.saveState.progression.currentSaveState.malnourished || player.Malnourished))
                            {
                                this.player.slugcatStats.foodToHibernate -= 1;
                                FoodMeter foodbar = this.room.game.cameras[0].hud.foodMeter;
                                foodbar.survivalLimit = this.player.slugcatStats.foodToHibernate;
                                foodbar.MoveSurvivalLimit(this.player.slugcatStats.foodToHibernate, true);
                            }
                            if (questcounter <= 5)
                            {
                                FireSmoke smoke = new FireSmoke(this.room);
                                for (int i = 0; i < (6 - questcounter); i++)
                                {
                                    smoke.EmitSmoke(this.player.firstChunk.pos, Custom.RNV(), Plugin.GravelFireColor(player), 20);
                                }
                            }
                            if (questcounter >= 8)
                            {
                                if (!player.room.game.devToolsActive)
                                {
                                    this.room.game.GetStorySession.saveState.deathPersistentSaveData.theMark = false;
                                    this.room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW] = 2;
                                }
                                this.room.game.GetStorySession.saveState.denPosition = this.room.abstractRoom.name;
                                this.room.game.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.GhostScreen);
                                this.Destroy();
                            }
                            else
                            {
                                player.SaintStagger(80 / questcounter);
                                Vector2 vector = player.bodyChunks[0].pos;
                                Vector2 a = Custom.DirVec(player.bodyChunks[1].pos, player.bodyChunks[0].pos);
                                if (Mathf.Abs(player.bodyChunks[0].pos.y - player.bodyChunks[1].pos.y) > Mathf.Abs(player.bodyChunks[0].pos.x - player.bodyChunks[1].pos.x) && player.bodyChunks[0].pos.y > player.bodyChunks[1].pos.y)
                                {
                                    vector += Custom.DirVec(player.bodyChunks[1].pos, player.bodyChunks[0].pos) * 5f;
                                    a *= -1f;
                                    a.x += 0.4f * (float)player.flipDirection;
                                    a.Normalize();
                                }
                                player.bodyChunks[0].pos -= a * 2f;
                                player.bodyChunks[0].vel -= a * 2f;
                                if (player.graphicsModule != null)
                                {
                                    (player.graphicsModule as PlayerGraphics).head.vel += Custom.RNV() * UnityEngine.Random.value * 3f;
                                }
                                for (int i = 0; i < 7; i++)
                                {
                                    this.room.AddObject(new WaterDrip(vector + Custom.RNV() * UnityEngine.Random.value * 1.5f, Custom.RNV() * 3f * UnityEngine.Random.value + a * Mathf.Lerp(2f, 6f, UnityEngine.Random.value), false));
                                }
                                this.room.PlaySound(SoundID.Slugcat_Regurgitate_Item, player.mainBodyChunk);
                                this.room.PlaySound(SoundID.Red_Lizard_Spit_Hit_Player, player.mainBodyChunk);
                            }
                            Debug.Log("GE Met Slug Ghost!");
                            this.Destroy();
                        }
                        else
                        {
                            this.ghost.counter = 0;
                        }
                    }
                    else if (this.canSee && !player.glowing)
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
                if (this.room.game.session is StoryGameSession story && this.room.game.Players.Count > 0 && firstAlivePlayer != null && firstAlivePlayer.realizedCreature != null && firstAlivePlayer.realizedCreature.room == this.room && this.room.game.GetStorySession.saveState.cycleNumber <= 0)
                {

                    Player player = firstAlivePlayer.realizedCreature as Player;

                    //this.room.game.wasAnArtificerDream = true;
                    DreamsState dreamsState = this.room.game.GetStorySession.saveState.dreamsState;

                    if (dreamsState != null)
                    {
                        dreamsState.InitiateEventDream(GravelDreamID.GravelIntroDream);
                    }

                    if (this.timer == 0)
                    {
                        player.sceneFlag = true;
                        if (player.controller == null)
                        {
                            player.controller = new Player.NullController();
                        }
                        player.SuperHardSetPosition(new Vector2(1625f, 720f));
                    }

                    if (this.timer > 0 && this.timer < 500)
                    {
                        player.Blink(10);
                    }

                    if (this.timer == 300)
                    {
                        player.Stun(20);
                        player.mainBodyChunk.vel = new Vector2(-25f, 2f);
                    }

                    if (this.room.game.GetStorySession.saveState.cycleNumber == 0 && this.timer == 400)
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
                    }

                    if ((player.mainBodyChunk.pos.y < 500f || tutor) && this.room.game.GetStorySession.saveState.cycleNumber == 0 && !story.saveState.deathPersistentSaveData.ArtificerMaulTutorial)
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
                                this.Destroy();
                                return;
                            default:
                                return;
                        }
                    }
                    else if (story.saveState.deathPersistentSaveData.ArtificerMaulTutorial && timer == 550)
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
        public class CL_D05 : UpdatableAndDeletable
        {
            // Token: 0x06004E8A RID: 20106 RVA: 0x00545C52 File Offset: 0x00543E52
            public CL_D05(Room room)
            {
                this.room = room;
            }

            // Token: 0x06004E8B RID: 20107 RVA: 0x00545C64 File Offset: 0x00543E64
            public override void Update(bool eu)
            {
                if (this.room.shortCutsReady)
                {
                    if (this.room.game.GetStorySession.saveState.cycleNumber < 0)
                    {
                        this.room.shortcuts[1].shortCutType = ShortcutData.Type.DeadEnd;
                    }
                }
            }
        }
        public class SB_A06GRAV : UpdatableAndDeletable
        {
            public SB_A06GRAV(Room room)
            {
                this.room = room;
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
                        if (this.room.game.rainWorld.processManager.musicPlayer != null)
                        {
                            this.room.game.rainWorld.processManager.musicPlayer.FadeOutAllSongs(60f);
                        }
                        if (player.mainBodyChunk.pos.x < -248f)
                        {
                            player.SuperHardSetPosition(new Vector2(this.room.RoomRect.right + 232f, player.mainBodyChunk.pos.y));
                        }
                        if (player.mainBodyChunk.pos.x > this.room.RoomRect.right + 248f)
                        {
                            player.SuperHardSetPosition(new Vector2(-232f, player.mainBodyChunk.pos.y));
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
                                /*if(room.game.GetStorySession.saveState.deathPersistentSaveData.ghostsTalkedTo[GhostWorldPresence.GhostID.UW] == -8)
                                {
                                    WorldCoordinate newCoord = new WorldCoordinate(this.room.world.GetAbstractRoom("SB_E01GRAV").index, 175, 40, -1);
                                    MSCRoomSpecificScript.RoomWarp(player, room, "SB_E01GRAV", newCoord, false);
                                    return;
                                }*/
                                this.target_blend = Mathf.Clamp(this.target_blend + 0.1f, 0f, 1f);
                                this.room.game.cameras[0].ChangeFadePalette(this.room.game.cameras[0].paletteB, Mathf.Clamp(this.room.game.cameras[0].paletteBlend + 0.05f, 0f, 1f));
                                player.SuperHardSetPosition(new Vector2(player.mainBodyChunk.pos.x, 700f));
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
                                player.SuperHardSetPosition(new Vector2(player.mainBodyChunk.pos.x, -102f));
                                player.Stun(300);
                                this.ClearAllVoidSpawn();
                                this.StoredEffect.amount = 0f;
                            }
                            if (this.target_blend == 1f && this.fadeObj == null)
                            {
                                this.fadeObj = new FadeOut(this.room, Color.cyan, 130f, false);
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
                    /*else if (this.room.game.Players[i].realizedCreature != null && (this.room.game.Players[i].realizedCreature as Player).room != this.room)
                    {
                        for (int l = 0; l < this.room.shortcuts.Length; l++)
                        {
                            this.room.shortcuts[l].;
                        }
                    }*/
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

            public FadeOut fadeObj;

            private RoomSettings.RoomEffect StoredEffect;

            private bool clearedSpawn;

            public KarmaVectorX karmaObj;

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
                    this.fadeObj = new FadeOut(this.room, Color.cyan, 130f, true);
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
                            player.SuperHardSetPosition(new Vector2(420f, 760f));
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

            public FadeOut fadeObj;

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
                    AbstractCreature abstractCreature = new AbstractCreature(this.room.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.HunterDaddy), null, pos, this.room.game.GetNewID());
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

            public FadeOut fadeIn;

            public bool firstSummon;

            public int timeSinceDead;
        }

    }
}
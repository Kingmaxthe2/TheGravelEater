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
using static RewiredConsts.Layout;

namespace GravelSlug
{
    public class CreatureHooks
    {
        public void ApplyHooks()
        {
            On.DartMaggot.ShotUpdate += NoStick;
            On.GarbageWorm.Update += GarbageHate;
            On.EggBug.ctor += EggBug_ctor;
            On.VoidSea.VoidWorm.MainWormBehavior.Update += MainWormBehavior_Update;
            On.VoidSea.VoidSeaScene.ArtificerEndUpdate += VoidSeaScene_ArtificerEndUpdate;
            On.TubeWorm.JumpButton += TubeWorm_JumpButton;
        }

        private void NoStick(On.DartMaggot.orig_ShotUpdate orig, DartMaggot self)
        {
            orig(self);
            if (self.mode == DartMaggot.Mode.StuckInChunk && self.stuckInChunk.owner is Player player)
            {
                if (Plugin.GravelHasAbilities(player) && (player.SlugCatClass.value == "Gravelslug") && !Plugin.IsGravelFeral(player))
                {
                    self.Unstuck();
                    self.firstChunk.vel *= Custom.RNV() * Mathf.Lerp(1f, 5f, UnityEngine.Random.value);
                    Debug.Log("dart maggot failed to pierce Gravel Eater");
                }
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

        private void EggBug_ctor(On.EggBug.orig_ctor orig, EggBug self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (self.FireBug && (world.game.IsStorySession && world.game.GetStorySession.saveStateNumber.value == "Gravelslug") || (world.game.IsArenaSession && abstractCreature.Room.name == "comatose"))
            {
                self.hue = 1f;
            }
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


        private bool TubeWorm_JumpButton(On.TubeWorm.orig_JumpButton orig, TubeWorm self, Player plr)
        {
            orig(self, plr);
            if (GravelHasAbilities(plr) && plr.SlugCatClass.value == "Gravelslug" && IsGravelFeral(plr) && self.tongues[0].Attached && plr.canJump < 1 && plr.bodyMode == Player.BodyModeIndex.Default)
            {
                if (plr.bodyChunks[0].vel.y < 0 && plr.bodyChunks[1].vel.y < 0)
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
    }
}
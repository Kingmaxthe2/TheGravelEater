using UnityEngine;
using static GravelSlug.RoomScripts;

namespace GravelSlug
{
    public class RoomscriptHooks
    {
        public static void ApplyHooks()
        {
            On.MoreSlugcats.MSCRoomSpecificScript.AddRoomSpecificScript += GravelStart; //add room specific scripts
        }

        private static void GravelStart(On.MoreSlugcats.MSCRoomSpecificScript.orig_AddRoomSpecificScript orig, Room room)
        {
            orig(room);
            string name = room.abstractRoom.name;
            if (name == "CL_GRAVEL" && room.game.IsStorySession && room.abstractRoom.firstTimeRealized && room.game.GetStorySession.saveState.cycleNumber <= 0 && room.game.StoryCharacter.value == "Gravelslug" && !(ModManager.Expedition && room.game.manager.rainWorld.ExpeditionMode))
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("It's Gravel-Eatin' Time!");
                }
                room.AddObject(new CL_GRAVEL(room));
            }
            if (name == "CL_D05" && room.game.IsStorySession && room.abstractRoom.firstTimeRealized && room.game.GetStorySession.saveState.cycleNumber < 0 && room.game.StoryCharacter.value == "Gravelslug" && !(ModManager.Expedition && room.game.manager.rainWorld.ExpeditionMode))
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("Grains force encounter");
                }
                room.AddObject(new CL_D05(room));
            }
            /*if (room.game.IsStorySession && (name == "MS_COMMS" || name == "SI_A07" || name == "LF_H01" || name == "CC_H01SAINT" || name == "GW_A25" || name == "DS_RIVSTART" || name == "OE_CAVE02" || name == "SB_GOR02RIV" || name == "SU_A53") && room.game.StoryCharacter.value == "Gravelslug" && !(ModManager.Expedition && room.game.manager.rainWorld.ExpeditionMode))
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("Slugcat Ghost placed");
                }
                room.AddObject(new SlugGhostVision(room));
            }*/
            if (name == "SB_A06GRAV" && room.game.IsStorySession && room.game.StoryCharacter.value == "Gravelslug" && !(ModManager.Expedition && room.game.manager.rainWorld.ExpeditionMode))
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("Gravel end room applied");
                }
                room.AddObject(new SB_A06GRAV(room));
            }
            if (name == "HR_GRAVINTRO" && room.game.IsStorySession && room.game.GetStorySession.saveState.denPosition == "HR_GRAVINTRO" && room.game.StoryCharacter.value == "Gravelslug" && !(ModManager.Expedition && room.game.manager.rainWorld.ExpeditionMode))
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("It's nacho Time!");
                }
                room.AddObject(new HR_GravelIntro(room));
            }
        }
    }
}
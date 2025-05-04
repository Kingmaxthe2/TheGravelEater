using UnityEngine;
using Smoke;
using MoreSlugcats;
using HUD;
using RWCustom;
using Pom;
using System.Collections.Generic;

namespace StellarOutpost
{
    public class SpaceView : BackgroundScene
    {
        // Token: 0x060024BF RID: 9407 RVA: 0x002D0AE4 File Offset: 0x002CECE4
        public SpaceView(Room room) : base(room)
        {
            this.AddElement(new BackgroundScene.Simple2DBackgroundIllustration(this, "atc_space", new Vector2(683f, 384f)));
        }

        /*private Vector2 DrawPos(BackgroundScene.BackgroundSceneElement element, Vector2 camPos, RoomCamera camera)
        {
            Vector2 vector = base.RoomToWorldPos(camera.pos);
            float num = this.DrawScale(element);
            float num2 = element.pos.x - (vector.x + camPos.x + this.sceneOrigo.x);
            float num3 = element.pos.y - (vector.y + camPos.y + this.sceneOrigo.y);
            return new Vector2(num2 * num + this.perspectiveCenter.x, num3 * num + this.perspectiveCenter.y);
        }

        private float DrawScale(BackgroundScene.BackgroundSceneElement element)
        {
            return 1f / (element.depth * SpaceView.depthScale + 1f);
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            foreach (SpaceView.FallingStar fallingStar in this.fallingStars)
            {
                fallingStar.Update();
            }
        }

        public RoomSettings.RoomEffect effect;

        // Token: 0x0400221A RID: 8730
        private static Color atmosphereColor = new Color(0.25882354f, 0.2509804f, 0.2784314f);

        // Token: 0x0400221B RID: 8731
        private static Vector2 ViewOffset = new Vector2(-50000f, 6000f);

        // Token: 0x0400221C RID: 8732
        private static Vector2 ConvergenceMult = new Vector2(0.4f, 0.5f);

        // Token: 0x0400221D RID: 8733
        private static Vector2 CloudPos = new Vector2(-2000f, 600f);

        // Token: 0x0400221E RID: 8734
        private static Vector2 CloudScale = new Vector2(12f, 8f);

        // Token: 0x0400221F RID: 8735
        private static Vector2 CloudOffset = new Vector2(20000f, -300f);

        // Token: 0x04002220 RID: 8736
        private static float ViewScale = 20f;

        // Token: 0x04002221 RID: 8737
        private static float ViewDepthMultiplier = 8f;

        // Token: 0x04002222 RID: 8738
        private static int ViewSeed = 1;

        // Token: 0x04002223 RID: 8739
        private static float TurbulenceScale = 3000f;

        // Token: 0x04002224 RID: 8740
        private static Vector4 StarSpeed = new Vector4(2f, -1f, 1f, 0.1f);

        // Token: 0x04002225 RID: 8741
        private static float startDepth = 2f;

        // Token: 0x04002226 RID: 8742
        private static float fogDepth = 30f;

        // Token: 0x04002227 RID: 8743
        private static float sceneScale = 1f;

        // Token: 0x04002228 RID: 8744
        private static float depthScale = 1f;

        // Token: 0x04002229 RID: 8745
        private Vector2 perspectiveCenter;

        // Token: 0x0400222A RID: 8746
        private List<SpaceView.FallingStar> fallingStars = new List<SpaceView.FallingStar>();
        private class FallingStar : BackgroundScene.BackgroundSceneElement
        {
            // Token: 0x17000C7C RID: 3196
            // (get) Token: 0x06004DB9 RID: 19897 RVA: 0x0053ACA7 File Offset: 0x00538EA7
            private SpaceView orScene
            {
                get
                {
                    return this.scene as SpaceView;
                }
            }

            // Token: 0x06004DBA RID: 19898 RVA: 0x0053ACB4 File Offset: 0x00538EB4
            public FallingStar(SpaceView scene, float x, float y, float depth, float scale, float randomValue) : base(scene, new Vector2(x * SpaceView.sceneScale, y * SpaceView.sceneScale), depth)
            {
                this.initialX = x * SpaceView.sceneScale;
                this.initialY = y * SpaceView.sceneScale;
                this.reset();
                this.scale = scale * SpaceView.sceneScale;
                this.randomValue = randomValue;
                this.prevRotation = (this.rotation = randomValue * 360f);
                this.y = Mathf.Lerp(this.initialY, 150f, randomValue);
            }

            // Token: 0x06004DBB RID: 19899 RVA: 0x0053AD42 File Offset: 0x00538F42
            private void reset()
            {
                this.x = this.initialX;
                this.y = this.initialY;
                this.prevX = this.initialX;
                this.prevY = this.initialY;
                this.timeAlive = 0;
            }

            // Token: 0x06004DBC RID: 19900 RVA: 0x0053AD7C File Offset: 0x00538F7C
            public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
            {
                sLeaser.sprites = new FSprite[1];
                sLeaser.sprites[0] = new FSprite("otr_fallingstar", true);
                sLeaser.sprites[0].scale = this.scale;
                sLeaser.sprites[0].shader = rCam.game.rainWorld.Shaders["FallingStar"];
                sLeaser.sprites[0].color = new Color(1f, 1f, 1f, Mathf.Min(this.depth / SpaceView.fogDepth, 1f));
                this.AddToContainer(sLeaser, rCam, null);
            }

            // Token: 0x06004DBD RID: 19901 RVA: 0x0053AE24 File Offset: 0x00539024
            public void Update()
            {
                this.timeAlive++;
                this.prevX = this.x;
                this.prevY = this.y;
                this.prevRotation = this.rotation;
                float num = Mathf.PerlinNoise(this.x / SpaceView.TurbulenceScale, this.y / SpaceView.TurbulenceScale + this.depth);
                num *= Custom.MapRangeClamped(num, -1f, 1f, -0.2f, 1f);
                this.x += SpaceView.StarSpeed.x * SpaceView.sceneScale * num;
                this.y += SpaceView.StarSpeed.y * SpaceView.sceneScale;
                this.rotation += Mathf.Sin((float)this.timeAlive / 40f * SpaceView.StarSpeed.w) * SpaceView.StarSpeed.z;
                if (this.y <= 150f)
                {
                    this.reset();
                }
            }

            // Token: 0x06004DBE RID: 19902 RVA: 0x0053AF2C File Offset: 0x0053912C
            public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
            {
                this.pos.x = Mathf.Lerp(this.prevX, this.x, timeStacker);
                this.pos.y = Mathf.Lerp(this.prevY, this.y, timeStacker);
                Vector2 vector = this.orScene.DrawPos(this, new Vector2(camPos.x, camPos.y), rCam);
                sLeaser.sprites[0].x = vector.x;
                sLeaser.sprites[0].y = vector.y;
                sLeaser.sprites[0].rotation = Mathf.Lerp(this.prevRotation, this.rotation, timeStacker);
                sLeaser.sprites[0].color = new Color(1f, this.randomValue, Mathf.InverseLerp(this.initialY, 150f, this.pos.y), Mathf.Min(this.depth / SpaceView.fogDepth, 1f));
                base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            }

            // Token: 0x04004DF3 RID: 19955
            private float initialX;

            // Token: 0x04004DF4 RID: 19956
            private float initialY;

            // Token: 0x04004DF5 RID: 19957
            private float prevX;

            // Token: 0x04004DF6 RID: 19958
            private float prevY;

            // Token: 0x04004DF7 RID: 19959
            private float x;

            // Token: 0x04004DF8 RID: 19960
            private float y;

            // Token: 0x04004DF9 RID: 19961
            private float prevRotation;

            // Token: 0x04004DFA RID: 19962
            private float rotation;

            // Token: 0x04004DFB RID: 19963
            private float scale;

            // Token: 0x04004DFC RID: 19964
            private int timeAlive;

            // Token: 0x04004DFD RID: 19965
            private float randomValue;
        }*/
    }
}
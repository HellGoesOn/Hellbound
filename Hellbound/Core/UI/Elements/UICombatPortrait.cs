using Microsoft.Xna.Framework;

namespace Casull.Core.UI.Elements
{
    public class UICombatPortrait : UIElement
    {
        public UIPicture picture;
        public UIPicture eliminatedPicture;
        public UIBorderedText hpText;
        public UIBorderedText spText;
        public UIBorderedText hpValueText;
        public UIBorderedText spValueText;
        public UIProgressBar hpBar;
        public UIProgressBar spBar;
        public string portrait;
        public bool penisEnlargmentPills;

        private int hpCurrentValue;
        private int spCurrentValue;
        private int hpTargetValue;
        private int spTargetValue;

        public float lerpSpeed = 1;

        public UICombatPortrait(string portrait, int maxHp, int maxSp)
        {
            this.portrait = portrait;
            picture = new UIPicture(portrait);
            picture.scale = new Vector2(3);
            picture.frames = [new(0, 0, 32, 32)];
            picture.origin = new Vector2(16, 16);

            eliminatedPicture = new UIPicture("Eliminated");
            eliminatedPicture.scale = new Vector2(3);
            eliminatedPicture.frames = [new(0, 0, 32, 32)];
            eliminatedPicture.origin = new Vector2(16, 16);
            eliminatedPicture.Visible = false;

            picture.SetPosition(48, 32);
            eliminatedPicture.SetPosition(48, 32);

            hpText = new UIBorderedText("HP");
            hpText.SetPosition(0, 80);
            hpText.color = Color.LightSeaGreen;
            spText = new UIBorderedText("SP");
            spText.color = Color.HotPink;
            spText.SetPosition(0, 118);

            hpValueText = new UIBorderedText("");
            hpValueText.SetPosition(36, 80);
            hpValueText.color = Color.LightSeaGreen;
            spValueText = new UIBorderedText("");
            spValueText.color = Color.HotPink;
            spValueText.SetPosition(36, 118);


            hpBar = new UIProgressBar(new Vector2(100, 8), maxHp);
            hpBar.fillColor = Color.LightSeaGreen;
            spBar = new UIProgressBar(new Vector2(100, 8), maxSp);
            spBar.fillColor = Color.HotPink;
            hpBar.SetPosition(0, 100);
            spBar.SetPosition(0, 100 + spBar.size.Y * 2);

            this.Append(picture);
            this.Append(eliminatedPicture);
            this.Append(hpBar);
            this.Append(spBar);
            this.Append(spText);
            this.Append(hpText);
            this.Append(spValueText);
            this.Append(hpValueText);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (!penisEnlargmentPills) {
                picture.scale = Vector2.Lerp(picture.scale, new Vector2(3), 0.12f);
            }
            else {
                picture.scale = Vector2.Lerp(picture.scale, new Vector2(4), 0.12f);
            }

            if (hpCurrentValue != hpTargetValue) {
                hpCurrentValue = (int)Math.Clamp(hpCurrentValue + Math.Sign(hpTargetValue - hpCurrentValue) * lerpSpeed, 0, int.MaxValue);

                if (Math.Abs(hpCurrentValue - hpTargetValue) <= lerpSpeed * 0.5f)
                    hpCurrentValue = hpTargetValue;
            }
            if (spCurrentValue != spTargetValue) {
                spCurrentValue = (int)Math.Clamp(spCurrentValue + Math.Sign(spTargetValue - spCurrentValue) * lerpSpeed, 0, int.MaxValue);
                if (Math.Abs(spCurrentValue - spTargetValue) <= lerpSpeed * 0.5f)
                    spCurrentValue = spTargetValue;
            }

            //hpCurrentValue = (int)MathHelper.Lerp(hpCurrentValue, hpTargetValue, lerpSpeed);
            //spCurrentValue = (int)MathHelper.Lerp(spCurrentValue, spTargetValue, lerpSpeed);

            hpValueText.text = hpCurrentValue.ToString();
            spValueText.text = spCurrentValue.ToString();
        }

        public void SetValues(int hp, int sp, int maxHp, int maxSp)
        {
            if (hp <= 0)
                eliminatedPicture.Visible = true;
            else
                eliminatedPicture.Visible = false;

            spTargetValue = sp;
            hpTargetValue = hp;

            hpBar.value = hp;
            hpBar.maxValue = maxHp;
            spBar.value = sp;
            spBar.maxValue = maxSp;

        }
    }
}

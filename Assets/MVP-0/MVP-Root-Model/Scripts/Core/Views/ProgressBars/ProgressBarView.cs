using System;
using UnityEngine.UI;

namespace MVP_0.MVP_Root_Model.Scripts.Core.Views.ProgressBars
{
    public class ProgressBarView : BaseProgressBarView
    {
        public Image image;
        public override void Report(float value)
        {
            if (image != null)
                image.fillAmount = Math.Min(value, 1f);
        }
        public override void ReportToZero(float value)
        {
            if (image != null)
                image.fillAmount = Math.Max(value, 0f);
        }
    }
}
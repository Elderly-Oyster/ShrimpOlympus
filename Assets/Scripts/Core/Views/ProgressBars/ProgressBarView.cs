using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Views.ProgressBars
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
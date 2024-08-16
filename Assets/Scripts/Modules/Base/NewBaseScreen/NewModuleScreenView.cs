using Core.Views.UIViews;

namespace Modules.Base.NewBaseScreen
{
    public class NewModuleScreenView : BubbleFadeUIView
    {
        public void SetupEventListeners()
        {
            
        }

        private void RemoveEventListeners()
        {
            
        }

        public override void Dispose()
        {
            RemoveEventListeners();
            base.Dispose();
        }
    }
}
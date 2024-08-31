using CodeBase.Core.MVVM.View;

namespace Modules.Base.NewBaseScreen
{
    public class NewModuleScreenView : BaseScreenView
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
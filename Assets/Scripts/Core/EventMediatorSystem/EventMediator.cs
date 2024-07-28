using System;

namespace Scripts.Core.EventMediatorSystem
{
    public class EventMediator
    {
        public event Action<PopupOpenedEvent> OnPopupOpened;

        public void Publish<T>(T eventArgs) where T : class
        {
            if (eventArgs is PopupOpenedEvent popupOpenedEvent) 
                OnPopupOpened?.Invoke(popupOpenedEvent);
        }

        public void Subscribe<T>(Action<T> action) where T : class
        {
            if (typeof(T) == typeof(PopupOpenedEvent)) 
                OnPopupOpened += action as Action<PopupOpenedEvent>;
        }

        public void Unsubscribe<T>(Action<T> action) where T : class
        {
            if (typeof(T) == typeof(PopupOpenedEvent)) 
                OnPopupOpened -= action as Action<PopupOpenedEvent>;
        }
    }
}
using UniRx;
using System;

namespace Core.EventMediatorSystem
{
    public class EventMediator
    {
        private readonly Subject<PopupOpenedEvent> _popupOpenedSubject = new Subject<PopupOpenedEvent>();

        public void Publish<T>(T eventArgs) where T : class
        {
            if (eventArgs is PopupOpenedEvent popupOpenedEvent) 
                _popupOpenedSubject.OnNext(popupOpenedEvent);
        }

        public IObservable<PopupOpenedEvent> OnPopupOpenedAsObservable() => 
            _popupOpenedSubject.AsObservable();

        public void Complete() => _popupOpenedSubject.OnCompleted();
    }
}
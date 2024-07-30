using System;
using System.Collections.Generic;
using System.Threading;
using Core.EventMediatorSystem;
using Core.Popup.Popups.FirstPopup.Scripts;
using Core.Popup.Popups.SecondPopup.Scripts;
using Core.Popup.Popups.ThirdPopup.Scripts;
using Core.Views.ProgressBars;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Core.Popup.Scripts
{
    public class PopupHub
    {
        [NonSerialized] public BasePopup CurrentPopup;

        [Inject] private PopupRootCanvas _rootCanvas;

        [Inject] private IBasePopupFactory<FirstPopup> _firstPopupFactory;
        [Inject] private IBasePopupFactory<SecondPopup> _secondPopupFactory;
        [Inject] private IBasePopupFactory<ThirdPopup> _thirdPopupFactory;
        [Inject] private EventMediator _eventMediator;

        private readonly PopupsPriorityQueue _popupQueue = new(); 
        private readonly Stack<BasePopup> _popups = new();
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        private void CreateAndOpenPopup(IFactory<Transform, BasePopup> basePopupFactory)
        {
            var popup = basePopupFactory.Create(_rootCanvas.PopupParent);
            EnqueuePopup(popup);
        }

        private void CreateAndOpenPopup<T>(IFactory<Transform, BasePopup> basePopupFactory, T param)
        {
            var popup = basePopupFactory.Create(_rootCanvas.PopupParent);
            EnqueuePopup(popup, param);
        }

        private void EnqueuePopup(BasePopup popup)
        {
            _popupQueue.Enqueue(popup); 
            TryOpenNextPopup().Forget();
        }

        private void EnqueuePopup<T>(BasePopup popup, T param)
        {
            _popupQueue.Enqueue(popup);  
            TryOpenNextPopup(param).Forget();
        }

        private async UniTaskVoid TryOpenNextPopup()
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                if (CurrentPopup == null && _popupQueue.TryDequeue(out var nextPopup)) 
                {
                    CurrentPopup = nextPopup;
                    _popups.Push(CurrentPopup);
                    CurrentPopup.gameObject.SetActive(true);
                    await CurrentPopup.Open<object>(null);
                    _eventMediator.Publish(new PopupOpenedEvent(CurrentPopup.GetType().Name));
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private async UniTaskVoid TryOpenNextPopup<T>(T param)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                if (CurrentPopup == null && _popupQueue.TryDequeue(out var nextPopup)) 
                {
                    CurrentPopup = nextPopup;
                    _popups.Push(CurrentPopup);
                    CurrentPopup.gameObject.SetActive(true);
                    await CurrentPopup.Open(param);
                    _eventMediator.Publish(new PopupOpenedEvent(CurrentPopup.GetType().Name));
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async UniTask CloseCurrentPopup()
        {
            if (CurrentPopup != null)
            {
                await CurrentPopup.Close();
                CurrentPopup = null;
                TryOpenNextPopup().Forget();  
            }
        }

        public void NotifyPopupClosed()
        {
            CurrentPopup = null;
            TryOpenNextPopup().Forget();
        }

        public void OpenFirstPopup() => CreateAndOpenPopup(_firstPopupFactory);
        public void OpenSecondPopup() => CreateAndOpenPopup(_secondPopupFactory);
        public void OpenThirdPopup() => CreateAndOpenPopup(_thirdPopupFactory);
    }
}

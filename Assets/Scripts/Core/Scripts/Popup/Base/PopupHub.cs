using System;
using System.Collections.Generic;
using System.Threading;
using Core.Scripts.EventMediatorSystem;
using Core.Scripts.Popup.Types.FirstPopup.Scripts;
using Core.Scripts.Popup.Types.SecondPopup.Scripts;
using Core.Scripts.Popup.Types.ThirdPopup.Scripts;
using Core.Scripts.Views.ProgressBars;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Core.Scripts.Popup.Base
{
    public class PopupHub
    {
        [NonSerialized] public BasePopup CurrentPopup;

        [Inject] private PopupCanvas _canvas;

        [Inject] private IBasePopupFactory<FirstPopup> _firstPopupFactory;
        [Inject] private IBasePopupFactory<SecondPopup> _secondPopupFactory;
        [Inject] private IBasePopupFactory<ThirdPopup> _thirdPopupFactory;
        [Inject] private EventMediator _eventMediator;

        private readonly PopupsPriorityQueue _popupQueue = new(); 
        private readonly Stack<BasePopup> _popups = new();
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        private void CreateAndOpenPopup(IFactory<Transform, BasePopup> basePopupFactory)
        {
            var popup = basePopupFactory.Create(_canvas.PopupParent);
            EnqueuePopup(popup);
        }

        private void CreateAndOpenPopup<T>(IFactory<Transform, BasePopup> basePopupFactory, T param)
        {
            var popup = basePopupFactory.Create(_canvas.PopupParent);
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

        private async UniTask TryOpenNextPopup() => await TryOpenNextPopup<object>(null);

        private async UniTask TryOpenNextPopup<T>(T param)
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
            catch (Exception ex)
            {
                Debug.LogError($"Error while opening popup '{CurrentPopup.name}' : {ex.Message}");
            }
            finally { _semaphoreSlim.Release(); }
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

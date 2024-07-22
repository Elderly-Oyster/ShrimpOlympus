using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MVP.MVP_Root_Model.Scripts.Core.EventMediator;
using MVP.MVP_Root_Model.Scripts.Core.Popup.Popups.FirstPopup.Scripts;
using MVP.MVP_Root_Model.Scripts.Core.Views;
using UnityEngine;
using VContainer;

namespace MVP.MVP_Root_Model.Scripts.Core.Popup
{
    public class PopupHub
    {
        [NonSerialized] public BasePopup CurrentPopup;

        [Inject] private RootCanvas _rootCanvas;

        [Inject] private BasePopupFactory<FirstPopup> _firstPopupFactory;
        [Inject] private EventMediator.EventMediator _eventMediator;

        private readonly Stack<BasePopup> _popups = new();
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        private void CreateAndOpenPopup(IFactory<Transform, BasePopup> basePopupFactory)
        {
            CurrentPopup = basePopupFactory.Create(_rootCanvas.PopupParent);
            OpenCurrentPopup<object>(null);
        }

        private void CreateAndOpenPopup<T>(IFactory<Transform, BasePopup> basePopupFactory, T param)
        {
            CurrentPopup = basePopupFactory.Create(_rootCanvas.PopupParent);
            OpenCurrentPopup(param);
        }

        private void OpenCurrentPopup<T>(T param)
        {
            _popups.Push(CurrentPopup);
            CurrentPopup.gameObject.SetActive(true);
            CurrentPopup.Open<T>(param);
            _eventMediator.Publish(new PopupOpenedEvent(CurrentPopup.GetType().Name));
        }

        public async UniTask CloseCurrentPopup()
        {
            if (_popups.TryPop(out CurrentPopup) && CurrentPopup != null)
                await CurrentPopup.Close();
        }

        public void OpenFirstPopup() => CreateAndOpenPopup(_firstPopupFactory);

        private async UniTask OpenDynamicPopup<TParam>(BasePopup popup, TParam param, string id)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                CurrentPopup = popup;
                _popups.Push(CurrentPopup);

                _eventMediator.Publish(new PopupOpenedEvent(id));
                await CurrentPopup.Open(param);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}

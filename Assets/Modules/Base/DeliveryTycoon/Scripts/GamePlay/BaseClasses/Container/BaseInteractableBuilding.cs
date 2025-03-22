using System.Threading;
using System.Threading.Tasks;
using CodeBase.Core.UI.ProgressBars;
using Cysharp.Threading.Tasks;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.BaseClasses.Cars;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.BaseClasses.Parcels;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.BaseClasses.Container
{
    public abstract class BaseInteractableBuilding : MonoBehaviour
    {
        [SerializeField] protected Parcel parcel;
        [SerializeField] protected BaseProgressBarView progressBar;
        [SerializeField] protected Image icon;

        protected BaseCarController _carController;
        protected bool _taskIsInProgress;
        private float _timeToCompleteAction = 1.5f;
        private const float AnimateToZeroTime = 0.5f;
        private bool _actionCompleted;
        private Coroutine _taskCoroutine;
        private CancellationTokenSource _cts;
        
        public bool TaskIsInProgress => _taskIsInProgress;

        public Parcel Parcel => parcel;

        protected virtual void Awake()
        {
            progressBar.AnimateToZero(0, progressBar.CurrentRatio).Forget();
            progressBar.gameObject.SetActive(false);
        }

        public void UpdateTimeToCompleteAction(float timeToCompleteAction)
        {
            _timeToCompleteAction = timeToCompleteAction;
        }

        protected void  OnTriggerEnter(Collider other)
        { 
            if (_carController == null && other.TryGetComponent(out BaseCarController car))
            {
                _carController = car;
                ShowProgressBar();
                _ = ExecuteTask(_carController);
            }
        }

        protected async void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out BaseCarController carController) && carController == _carController)
            {
                _cts?.Cancel();

                _carController = null;
                
                if (!_actionCompleted)
                    await progressBar.AnimateToZero(AnimateToZeroTime, progressBar.CurrentRatio);
            }
        }
        
        protected async UniTask ExecuteTask(BaseCarController playerController = null)
        {
            _taskIsInProgress = true;
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                await progressBar.Animate(_timeToCompleteAction, token);

                if (!token.IsCancellationRequested)
                {
                    progressBar.AnimateToZero(0, progressBar.CurrentRatio).Forget();
                    HideProgressBar();
                    CompleteAction(playerController);
                }
                
            }
            catch (TaskCanceledException)
            {
                _taskIsInProgress = false;
                _actionCompleted = false;
                 
            }
            finally
            {
                _cts.Dispose();
                _cts = null;
            }
        }
        
        protected abstract void CompleteAction(BaseCarController carController);
        
        private void ShowProgressBar()
        {
            progressBar.gameObject.SetActive(true);
        }

        protected void HideProgressBar()
        {
            progressBar.gameObject.SetActive(false);
        }
    }
}
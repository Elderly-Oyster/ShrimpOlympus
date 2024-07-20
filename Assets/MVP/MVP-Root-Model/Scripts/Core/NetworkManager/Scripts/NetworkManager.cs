using System.Net.Http;
using Cysharp.Threading.Tasks;
using MVP.MVP_Root_Model.Scripts.Core.Popup;
using VContainer;

namespace MVP.MVP_Root_Model.Scripts.Core.NetworkManager.Scripts
{
    public class NetworkManager
    {
        [Inject] private PopupHub _popupHub;
        private bool _isOnline = true;
        
        private async void CheckInternetConnection()
        {
            string[] internetCheckURLs = { "https://www.google.com", "https://www.youtube.com" };
            var currentURLIndex = 0;
            while (true)
            {
                try
                {
                    using var httpClient = new HttpClient();
                    using var response = await httpClient.GetAsync(internetCheckURLs[currentURLIndex]);
                    response.EnsureSuccessStatusCode();
                    if (!_isOnline)
                        HandleInternetConnected();
                }
                catch (HttpRequestException)
                {
                    if (currentURLIndex + 1 == internetCheckURLs.Length)
                    {
                        if (_isOnline)
                            HandleInternetDisconnected();
                        currentURLIndex = 0;
                    }                    
                    else
                    {
                        currentURLIndex++;
                        continue;
                    }
                }
                await UniTask.Delay(5000);
            }

            void HandleInternetConnected()
            {
                _isOnline = true;
                //_popupHub.CloseCurrentPopup();
            }

            void HandleInternetDisconnected()
            {
                _isOnline = false;
                //_popupHub.OpenOfflinePopup();
            }
        }
    }
}
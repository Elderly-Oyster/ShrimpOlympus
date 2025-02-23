using System;
using UnityEngine;

namespace CodeBase.Core
{
	public class MonoBehaviourEventsHandler: MonoBehaviour
	{
		private static MonoBehaviourEventsHandler _instance;
		public static event Action<bool> OnApplicationFocusEvent;
		public static event Action<bool> OnApplicationPauseEvent;
		public static event Action OnApplicationQuitEvent;

		private void Awake()
		{
			if(_instance != null)
			{
				Destroy(gameObject);
				return;
			}
			
			DontDestroyOnLoad(gameObject);
			_instance = this;
		}

		private void OnApplicationFocus(bool focus)
		{
			OnApplicationFocusEvent?.Invoke(focus);
		}

		private void OnApplicationPause(bool pause)
		{
			OnApplicationPauseEvent?.Invoke(pause);
		}

		private void OnApplicationQuit()
		{
			OnApplicationQuitEvent?.Invoke();
		}
	}
}
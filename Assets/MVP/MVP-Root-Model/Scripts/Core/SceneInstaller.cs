using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace MVP.MVP_Root_Model.Scripts.Core
{
    public abstract class SceneInstaller : MonoBehaviour, ISceneInstaller
    {
        [SerializeField] private List<GameObject> objectsToDelete;

        private void Awake() => DeleteSelectedObjects();

        public abstract void RegisterSceneDependencies(IContainerBuilder builder);

        private void DeleteSelectedObjects()
        {
            foreach (var objectToDelete in objectsToDelete) 
                Destroy(objectToDelete);
        }
    }
}
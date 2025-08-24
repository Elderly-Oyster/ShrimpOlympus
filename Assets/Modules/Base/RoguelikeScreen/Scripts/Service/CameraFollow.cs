using Modules.Base.RoguelikeScreen.Scripts.Character;
using R3;
using UnityEngine;
using VContainer;

namespace Modules.Base.RoguelikeScreen.Scripts.Service
{
    public class CameraFollow :  MonoBehaviour
    {
        private readonly CompositeDisposable _disposables = new();
        
        [Inject]
        public void Construct(CharacterModel characterModel) =>
            characterModel.Position.Subscribe(pos => ChangePosition(pos)).AddTo(_disposables);
        
        private void ChangePosition(Vector2 position) =>
            gameObject.transform.position = new Vector3(position.x, position.y, transform.position.z);
        
        private void OnDestroy() =>
            _disposables.Dispose();
    }
}
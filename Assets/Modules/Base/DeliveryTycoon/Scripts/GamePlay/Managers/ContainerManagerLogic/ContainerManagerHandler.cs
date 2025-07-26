using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystem;
using UnityEngine;
using static Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers.ContainerManagerLogic.ContainerManagerOperations;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers.ContainerManagerLogic
{
    public class ContainerManagerHandler : IRequestHandler<AddNewContainerCommand>
    {
        private ReceiverManager ReceiverManager { get;  set; }
        private GameDataSystem GameDataSystem { get; set; }

        public ContainerManagerHandler
        (ReceiverManager receiverManager, 
            GameDataSystem gameDataSystem)
        {
            ReceiverManager = receiverManager;
            GameDataSystem = gameDataSystem;
        }


        public Task Handle(AddNewContainerCommand request, CancellationToken cancellationToken)
        {
            ReceiverManager.UpdateReceiversTypes(request.ContainerHolders);
            Debug.Log("ContainerManagerHolder " + request.ContainerHolders.Count);
            GameDataSystem.SetContainersData(GameDataSystem.ConvertToData(request.ContainerHolders));
            return Task.FromResult(Unit.Value);
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystemLogic;
using static Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers.ContainerManagerLogic.ContainerManagerOperations;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers.ContainerManagerLogic
{
    public class ContainerManagerHandler : IRequestHandler<ContainerManagerOperations.AddNewContainerCommand>
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


        public Task<Unit> Handle(AddNewContainerCommand request, CancellationToken cancellationToken)
        {
            ReceiverManager.UpdateReceiversTypes(request.ContainerHolders);
            GameDataSystem.SetContainersData(GameDataSystem.ConvertToData(request.ContainerHolders));
            return Task.FromResult(Unit.Value);
        }
    }
}
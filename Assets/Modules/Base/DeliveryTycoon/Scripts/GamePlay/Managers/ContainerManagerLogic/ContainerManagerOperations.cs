using System.Collections.Generic;
using MediatR;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Containers;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers.ContainerManagerLogic
{
    public static class ContainerManagerOperations
    {
        public record AddNewContainerCommand(List<ContainerHolder> ContainerHolders) : IRequest
        {
            public List<ContainerHolder> ContainerHolders { get; } = ContainerHolders;
        }
    }
}
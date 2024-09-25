namespace Core.Scripts.EventMediatorSystem
{
    public class PopupOpenedEvent
    {
        public string PopupName { get; }

        public PopupOpenedEvent(string popupName) => PopupName = popupName;
    }
}
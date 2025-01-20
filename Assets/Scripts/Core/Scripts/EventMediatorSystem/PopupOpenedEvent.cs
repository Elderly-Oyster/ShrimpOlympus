namespace Core.Scripts.EventMediatorSystem
{
    public class PopupOpenedEvent
    {
        //Represents an event that encapsulates the name of the popup that was opened.
        public string PopupName { get; }

        public PopupOpenedEvent(string popupName) => PopupName = popupName;
    }
}
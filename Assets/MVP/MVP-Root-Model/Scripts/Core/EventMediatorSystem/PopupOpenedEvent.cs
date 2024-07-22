namespace MVP.MVP_Root_Model.Scripts.Core.EventMediatorSystem
{
    public class PopupOpenedEvent
    {
        public string PopupName { get; }

        public PopupOpenedEvent(string popupName)
        {
            PopupName = popupName;
        }
    }
}
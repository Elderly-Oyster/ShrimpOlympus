using System.Collections.Generic;

namespace Core.Popup.Scripts
{
    public class PopupsPriorityQueue
    {
        private readonly SortedDictionary<int, Queue<BasePopup>> _dictionary = new();

        public void Enqueue(BasePopup popup)
        {
            int priority = (int)popup.Priority;
            if (!_dictionary.ContainsKey(priority)) 
                _dictionary[priority] = new Queue<BasePopup>();
            _dictionary[priority].Enqueue(popup);
        }

        public bool TryDequeue(out BasePopup popup)
        {
            foreach (var queue in _dictionary.Values)
            {
                if (queue.Count > 0)
                {
                    popup = queue.Dequeue();
                    return true;
                }
            }

            popup = null;
            return false;
        }
    }
    
    public enum PopupPriority
    {
        High,
        Medium,
        Low
    }
}
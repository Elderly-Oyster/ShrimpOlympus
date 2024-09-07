using System.Collections.Generic;
using System.Linq;

namespace Core.Popup.Base
{
    public class PopupsPriorityQueue
    {
        private readonly SortedDictionary<int, Queue<BasePopup>> _dictionary = new();
        private int? _minPriority = null;

        public void Enqueue(BasePopup popup)
        {
            int priority = (int)popup.Priority;
            if (!_dictionary.ContainsKey(priority)) 
                _dictionary[priority] = new Queue<BasePopup>();

            _dictionary[priority].Enqueue(popup);

            if (_minPriority == null || priority < _minPriority) 
                _minPriority = priority;
        }

        public bool TryDequeue(out BasePopup popup)
        {
            if (_minPriority == null)
            {
                popup = null;
                return false;
            }

            var queue = _dictionary[_minPriority.Value];
            popup = queue.Dequeue();

            if (queue.Count == 0)
            {
                _dictionary.Remove(_minPriority.Value);
                _minPriority = _dictionary.Count > 0 ? _dictionary.Keys.Min() : null;
            }

            return true;
        }
    }

    public enum PopupPriority
    {
        High,
        Medium,
        Low
    }
}
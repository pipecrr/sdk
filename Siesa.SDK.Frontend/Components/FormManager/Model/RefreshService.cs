using System;
using System.Collections.Generic;


namespace Siesa.SDK.Frontend.Components.FormManager.Model
{
    public class RefreshService
    {
        public event Action OnChange;

        public Dictionary<string, List<Action>> Observers { get; private set; } = new Dictionary<string, List<Action>>();

        public void AddObserver(string pKey, Action pAction)
        {
            if (!Observers.ContainsKey(pKey))
            {
                Observers.Add(pKey, new List<Action>());
            }
            Observers[pKey].Add(pAction);
        }

        public void RemoveObserver(string pKey)
        {
            Observers.Remove(pKey);
        }
        public void Transmit(string pKey)
        {
            if (Observers.ContainsKey(pKey))
            {
                foreach (Action act in Observers[pKey])
                {
                    act?.Invoke();
                }
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}

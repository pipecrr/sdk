using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siesa.SDK.Protos;

namespace Siesa.SDK.Frontend.Backend
{

    //Create a singleton class to manage the backend connection
    public class BackendManager
    {
        private static BackendManager _instance;
        private Dictionary<string, BackendRegistry> backendDict = new Dictionary<string, BackendRegistry>();
        private BackendManager()
        {
            
        }

        public static BackendManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BackendManager();
                }
                return _instance;
            }
        }

        public void RegisterBackend(string backendName, string backendUrl)
        {
            if (!backendDict.ContainsKey(backendName))
            {
                backendDict.Add(backendName, new BackendRegistry(backendName, backendUrl));
            }
        }

        public BackendRegistry GetBackend(string backendName)
        {
            if (backendDict.ContainsKey(backendName))
            {
                return backendDict[backendName];
            }
            else
            {
                return null;
            }
        }

        public Dictionary<string, BackendRegistry> GetBackendDict()
        {
            return backendDict;
        }
    }
}

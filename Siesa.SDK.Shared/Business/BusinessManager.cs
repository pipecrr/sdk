using Siesa.SDK.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Business
{
//Create a singleton class to manage the business layer
    public class BusinessManager
    {
        private static BusinessManager _instance;
        public Dictionary<string, BusinessModel> Businesses { get; set; }
        private BusinessManager()
        {
        }
        public static BusinessManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BusinessManager();
                    _instance.Businesses = new Dictionary<string, BusinessModel>();
                }
                return _instance;
            }
        }

        public void AddBusiness(BusinessModel business)
        {
            if (!Businesses.ContainsKey(business.Name))
            {
                Businesses.Add(business.Name, business);
            }
        }

        public BusinessModel GetBusiness(string name)
        {
            if (Businesses.ContainsKey(name))
            {
                return Businesses[name];
            }
            return null;
        }
    }
}

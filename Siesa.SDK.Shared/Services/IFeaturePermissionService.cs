using System.Collections.Generic;
using System.Threading.Tasks;
using Siesa.SDK.Shared.DTOS;
using Siesa.SDK.Shared.Utilities;
using System;
using System.Threading.Tasks;
using System.Linq;
namespace Siesa.SDK.Shared.Services
{
    public interface IFeaturePermissionService
    {
        public bool CheckUserActionPermission(string businessName, int actionRowid, IAuthenticationService authenticationService);
        public bool CheckUserActionPermissions(string businessName, List<int> permissions, IAuthenticationService authenticationService);
        public Task<bool> CheckUserActionPermission(string businessName, string actionName, IAuthenticationService authenticationService);
        public Task<bool> CheckUserActionPermissions(string businessName, List<string> actions, IAuthenticationService authenticationService);
        public Task<int> GetActionRowid(string actionName);
    }

    public class FeaturePermissionService : IFeaturePermissionService
    {
        Dictionary<string, int> ActionDict = new Dictionary<string, int>();
        private bool loadingAction;

        public SDKBusinessModel Backend {get { return BackendRouterServiceBase.Instance.GetSDKBusinessModel("BLSDKAction", null); } }

        public bool CheckUserActionPermission(string businessName, int actionRowid, IAuthenticationService authenticationService)
        {
            return Siesa.SDK.Shared.Utilities.Utilities.CheckUserActionPermission(businessName, actionRowid, authenticationService);
        }

        public FeaturePermissionService()
        {
            _ = SyncSDKActions();
        }

        private async Task SyncSDKActions()
        {
            loadingAction = true;
            try{
                var request = await Backend.Call("GetSDKActions");
                if(request.Success){
                    var actions = (List<SDKActionDTO>)request.Data;
                    foreach(var item in actions){
                        if(!ActionDict.ContainsKey(item.Id)){
                            ActionDict.Add(item.Id, item.Rowid);
                        }
                    }
                }
            }finally{
                loadingAction = false;
            }
        }

        public async Task<int> GetActionRowid(string actionName)
        {
            if(loadingAction){
                while(loadingAction){
                    await Task.Delay(500);
                }
            }
            var result = 0;
            if(ActionDict.ContainsKey(actionName)){
                result = ActionDict[actionName];
            }
            else{
                loadingAction = true;
                try{
                    var request = await Backend.Call("GetRowidActionByName", actionName);
                    if(request.Success){
                        result = (int)request.Data;
                        ActionDict.Add(actionName, result);
                    }
                }finally{
                    loadingAction = false;
                }
            }
            return result;
        }

        public async Task<bool> CheckUserActionPermission(string businessName, string actionName, IAuthenticationService authenticationService)
        {
            var result = false;
            var actionRowid = await GetActionRowid(actionName);
            return CheckUserActionPermission(businessName, actionRowid, authenticationService);
        }

        public bool CheckUserActionPermissions(string businessName, List<int> permissions, IAuthenticationService authenticationService){
            var result = false;
            foreach(var item in permissions){
                result = CheckUserActionPermission(businessName, item, authenticationService);
                if(!result){
                    break;
                }
            }
            return result;
        }

        public async Task<bool> CheckUserActionPermissions(string businessName, List<string> actions, IAuthenticationService authenticationService){
            var result = false;
            foreach(var item in actions){
                result = await CheckUserActionPermission(businessName, item, authenticationService);
                if(!result){
                    break;
                }
            }
            return result;
        }
    }
}
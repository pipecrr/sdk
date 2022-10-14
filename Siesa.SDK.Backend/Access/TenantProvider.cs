using System;
using System.Collections.Generic;
using System.Linq;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.DTOS;

namespace Siesa.SDK.Backend.Access
{
    public interface ITenantProvider
    {
        void SetTenant(SDKDbConnection tenant);
        void SetTenantByRowId(short rowId);
        Guid Register(Action callback);
        void Unregister(Guid id);
        SDKDbConnection GetTenant();
        string GetTenantShortName();
        bool GetUseLazyLoadingProxies();
        void SetUseLazyLoadingProxies(bool value);
        List<SDKDbConnection> GetTenants();
    }
    public class TenantProvider: ITenantProvider
    {
        private readonly IDictionary<Guid, Action> callbacks = new Dictionary<Guid, Action>();

        private List<SDKDbConnection> tenants = new List<SDKDbConnection>();

        private SDKDbConnection tenant = null;

        private readonly IAuthenticationService _authenticationService;

        private bool UseLazyLoadingProxies = false;

        public void SetTenant(SDKDbConnection tenant)
        {
            this.tenant = tenant;
            foreach (var callback in callbacks.Values)
            {
                callback();
            }
        }

        public void SetTenantByRowId(short rowId)
        {
            var tenant = tenants.FirstOrDefault(x => x.Rowid == rowId);
            if (tenant != null)
            {
                SetTenant(tenant);
            }
        }

        public TenantProvider(IAuthenticationService authenticationService, List<SDKDbConnection> tenants)
        {
            this.tenants = tenants;
            if(_authenticationService != null && _authenticationService.User != null)
            {
                if(_authenticationService.User.RowIdDBConnection != 0)
                {
                    SetTenantByRowId(_authenticationService.User.RowIdDBConnection);
                }
            }
        }

        public void Unregister(Guid id)
        {
            if (callbacks.ContainsKey(id))
            {
                callbacks.Remove(id);
            }
        }

        public Guid Register(Action callback)
        {
            var id = Guid.NewGuid();
            callbacks.Add(id, callback);
            return id;
        }

        public SDKDbConnection GetTenant() => tenant;

        public string GetTenantShortName() => tenant.Name;

        public List<SDKDbConnection> GetTenants() => tenants;

        public bool GetUseLazyLoadingProxies() => UseLazyLoadingProxies;

        public void SetUseLazyLoadingProxies(bool value) => UseLazyLoadingProxies = value;
    }
}

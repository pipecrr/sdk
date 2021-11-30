using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditAppGrpcClient
{
    public interface ISDKLogStorageService
    {
        Task Save(string json);
    }
}

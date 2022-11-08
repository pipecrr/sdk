using System;
using System.Threading.Tasks;
using Blazor.IndexedDB.Framework;
using Microsoft.JSInterop;
    
namespace Siesa.SDK.Frontend.Data
   {
       public class IndexDb : IndexedDb
       {
           public IndexDb(IJSRuntime jSRuntime, string name, int version) : base(jSRuntime, name, version) {}
           public IndexedSet<Culture> Cultures { get; set; }
           public IndexedSet<Resource> Resources { get; set; }
           public IndexedSet<ResourceDetail> ResourcesDetail { get; set; }

           public async Task ClearAll(){
               Cultures.Clear();
               Resources.Clear();
               ResourcesDetail.Clear();

               await SaveChanges();
           }
        }
   }
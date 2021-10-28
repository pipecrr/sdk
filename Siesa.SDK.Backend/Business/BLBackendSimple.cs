using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Siesa.SDK.Backend.Access;
using Siesa.SDK.Entities;

namespace Siesa.SDK.Business
{
    public class BLBackendSimple<T> : IBLBase<T> where T : BaseEntity
    {
        private IServiceProvider _provider;
        private dynamic _dbFactory;

        public string BusinessName { get; set; }
        public T BaseObj { get; set; }

        public BLBackendSimple()
        {
            BaseObj = Activator.CreateInstance<T>();
        }

        public void SetProvider(IServiceProvider provider)
        {
            _provider = provider;

            _dbFactory = _provider.GetService(typeof(IDbContextFactory<SDKContext>));
        }

        public virtual T Get(int id)
        {
            using (SDKContext context = _dbFactory.CreateDbContext())
            {
                return context.Set<T>().Find(id);
            } 
        }

        public virtual int Save()
        {
            using (SDKContext context = _dbFactory.CreateDbContext())
            {
                if (BaseObj.RowID == 0)
                {
                    context.Add<T>(BaseObj);
                }
                else
                {
                    BaseObj.LastUpdateDate = DateTime.Now;
                    context.Update<T>(BaseObj); //TODO: Validar que el ID exista al actualizar
                }
                
                context.SaveChanges(); //TODO: Capturar errores db y hacer rollback
            }
            return BaseObj.RowID;
        }

        public virtual void Update()
        {
            throw new NotImplementedException();
        }

        public virtual int Delete()
        {
            using (SDKContext context = _dbFactory.CreateDbContext())
            {
                context.Set<T>().Remove(BaseObj);
                context.SaveChanges();
            }
            return 0;
        }

        public virtual IEnumerable<T> List(int page = 0, int pageSize = 30)
        {
            using (SDKContext context = _dbFactory.CreateDbContext())
            {
                return context.Set<T>().Skip(page * pageSize).Take(pageSize).ToList();
            }
        }

        public Task<T> GetAsync(int id)
        {
            throw new NotImplementedException();
        }
    }

}

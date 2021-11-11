using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Siesa.SDK.Backend.Access;
using Siesa.SDK.Entities;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.Business;
using Siesa.SDK.Shared.Validators;
using Siesa.SDK.Shared.Exceptions;
using Siesa.SDK.Backend.Exceptions;

namespace Siesa.SDK.Business
{
    public class BLBackendSimple<T, K> : IBLBase<T> where T : BaseEntity where K : BLBaseValidator<T>
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

        public virtual ValidateAndSaveBusinessObjResponse ValidateAndSave()
        {
            ValidateAndSaveBusinessObjResponse result = new();

            try
            {

                Validate(ref result);

                if (result.Errors.Count > 0)
                {
                    return result;
                }

                using (SDKContext context = _dbFactory.CreateDbContext())
                {
                    result.Rowid = Save(context);
                }
            }
            catch(DbUpdateException exception)
            {
                AddErrorToResult(result, exception);
            }
            catch (Exception exception)
            {
                AddErrorToResult(result, exception);
            }
            
            return result;            
        }

        private void AddErrorToResult(ValidateAndSaveBusinessObjResponse result, DbUpdateException exception)
        {
            var message = BackendExceptionManager.ExceptionToString(exception);
            message += $"\nBussiness Name: {BusinessName}";
            message += $"\nObject {BaseObj}";
            result.Errors.Add(new OperationError() { Message = message });
        }
        private void AddErrorToResult(ValidateAndSaveBusinessObjResponse result, Exception exception)
        {
            var message = ExceptionManager.ExceptionToString(exception);
            message += $"\nBussiness Name: {BusinessName}";
            message += $"\nObject {BaseObj}";
            result.Errors.Add(new OperationError() { Message = message});
        }

        private void Validate(ref ValidateAndSaveBusinessObjResponse baseOperation)
        {
            ValidateBussines(ref baseOperation);
            K validator = Activator.CreateInstance<K>();
            SDKValidator.Validate<T>(BaseObj, validator, ref baseOperation);
        }

        private int Save(SDKContext context)
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

        public virtual Siesa.SDK.Shared.Business.LoadResult List(int page = 0, int pageSize = 30, string options = "")
        {
            var result = new Siesa.SDK.Shared.Business.LoadResult();
            using (SDKContext context = _dbFactory.CreateDbContext())
            {
                //total data
                result.TotalCount = context.Set<T>().Count();
                //data
                result.Data = context.Set<T>().Skip(page * pageSize).Take(pageSize).ToList();
            }
            return result;
        }

        public Task<T> GetAsync(int id)
        {
            throw new NotImplementedException();
        }
        protected virtual void ValidateBussines(ref ValidateAndSaveBusinessObjResponse operationResult)
        {
            // Do nothing
        }
    }

}

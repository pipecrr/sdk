using System;
using System.Collections.Generic;
using Siesa.SDK.Backend.Access;
using Siesa.SDK.Entities;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Validators;
using System.Linq.Dynamic.Core;
using System.Linq;
using Siesa.SDK.Shared.DataAnnotations;
using Siesa.SDK.Shared.Business;

namespace Siesa.SDK.Business
{
    /// <summary>
    /// Represents a class for managing parent-child relationships in the SDK context, inherits from BLBackendSimple.
    /// </summary>
    /// <typeparam name="TParent">The type of the parent object.</typeparam>
    /// <typeparam name="TChild">The type of the child object.</typeparam>
    public class BLBackendDocument<TParent, TChild> : BLBackendSimple<TParent,BLBaseValidator<TParent>> where TParent : class,IBaseSDK where TChild : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BLBackendDocument{TParent,TChild}"/> class.
        /// </summary>
        /// <param name="authenticationService">The authentication service.</param>
        public BLBackendDocument(IAuthenticationService authenticationService) : base(authenticationService)
        {
        }
        
        /// <summary>
        /// Gets or sets the child objects.
        /// </summary>
        public List<TChild> ChildObjs { get; set; } = new();
        /// <summary>
        /// Gets or sets the child objects to be deleted.
        /// </summary>
        public List<TChild> ChildObjsDeleted { get; set; } = new();
        /// <summary>
        /// Gets or sets the rowids of the child objects to be updated.
        /// </summary>
        public List<dynamic> ChildRowidsUpdated { get; set; } = new();
        /// <summary>
        /// Gets or sets the extra detail fields.
        /// </summary>
        public List<string> ExtraDetailFields { get; set; } = new();
        /// <summary>
        /// Method to return Type of the child object.
        /// </summary>
        /// <returns>Type of ChildObjs</returns>
        public Type ChildType() => typeof(TChild);
        /// <summary>
        /// Performs actions before deleting the business object, specifically deleting related child objects. 
        /// </summary>
        /// <param name="result">The result of the validation and save operation.</param>
        /// <param name="context">The SDK context.</param>
        public override void BeforeDelete(ref ValidateAndSaveBusinessObjResponse result, SDKContext context)
        {
            var query = context.AllSet<TChild>().AsQueryable();
            query = query.Where($"{GetRelatedChildField()} == @0", BaseObj.GetRowid());
            List<TChild> childsDelete = query.ToList();
            context.RemoveRange(childsDelete);
            context.SaveChanges();
        }

        /// <summary>
        /// Performs actions after saving the business object, specifically updating related child objects with the appropriate value.
        /// </summary>
        /// <param name="result">The result of the validation and save operation.</param>
        /// <param name="context">The SDK context.</param>
        public override void AfterSave(ref ValidateAndSaveBusinessObjResponse result, SDKContext context)
        {
            var rowidValue = result.Rowid; 
            string relatedChildField = GetRelatedChildField();
            dynamic rowidValueProperty = ConvertToPropertyType(relatedChildField, rowidValue); 
            List<TChild> childsAdded = ChildObjs.FindAll(x =>
            {
                dynamic rowid = x.GetType().GetProperty("Rowid")?.GetValue(x);
                bool isAdd = (rowid == null || rowid == 0);
                if (isAdd)
                {
                    var propertyRelatedChild = x.GetType().GetProperty(relatedChildField);
                    propertyRelatedChild?.SetValue(x, rowidValueProperty);
                }
                return isAdd;
            });

            //where x.Rowid in ChildRowidsUpdated
            List<TChild> childsUpdated = new(); 
            ChildRowidsUpdated.ForEach(x =>
            {
                var childObj = ChildObjs.Find(y =>
                {
                    bool result = false;
                    var property = y.GetType().GetProperty("Rowid");
                    if (property != null)
                    {
                        dynamic rowidY = Convert.ChangeType(property.GetValue(y), property.PropertyType);
                        dynamic rowidX = Convert.ChangeType(x, property.PropertyType);
                        result = rowidY?.Equals(rowidX);
                    }
                    return result;
                });
                if (childObj != null)
                {
                    childsUpdated.Add(childObj);   
                }
            });

            if (childsUpdated.Count > 0)
            {
                context.UpdateRange(childsUpdated);
            }
            if (ChildObjsDeleted.Count > 0)
            {
                context.RemoveRange(ChildObjsDeleted);
            }
            if (childsAdded.Count > 0){
                context.AddRange(childsAdded);
            }
            context.SaveChanges();

        }

        private static object ConvertToPropertyType(string relatedChildField, long rowidValue)
        {
            object result = rowidValue;
            var propertyRelatedChild = typeof(TChild).GetProperty(relatedChildField);
            Type type = propertyRelatedChild?.PropertyType;
            if (type != null)
            {
                result = Convert.ChangeType(rowidValue, propertyRelatedChild.PropertyType);
            }
            return result;
        }

        /// <summary>
        /// Gets the name of the field that relates the child object to the parent object.
        /// The field name is derived from the parent type's name.
        /// </summary>
        /// <returns>The name of the related field.</returns>
        public virtual string GetRelatedChildField()
        {
            string result = "";
            string nameTypeParent = typeof(TParent).Name;
            int index = nameTypeParent.IndexOf("_", StringComparison.Ordinal);
            if (index > 0 && index < nameTypeParent.Length - 1)
            {
                result = nameTypeParent.Substring(index + 1);
            }

            result = "Rowid"+result;
            
            return result;
        }
        /// <summary>
        /// Retrieves child objects based on a parent's rowid and optional extra detail fields.
        /// </summary>
        /// <param name="rowid">The parent's rowid.</param>
        /// <param name="extraDetailFields">Optional extra detail fields to retrieve.</param>
        /// <returns>The action result containing the list of child objects.</returns>
        [SDKExposedMethod]
        public ActionResult<List<TChild>> GetChilds(Int64 rowid, List<string> extraDetailFields = null)
        {
            string relatedChildField = GetRelatedChildField();
            using (SDKContext context = CreateDbContext())
            {
                var query = context.Set<TChild>().AsQueryable();
                var selectedFields = "";
                bool hasRelated = false;
                bool hasExtraFields = false;
                List<string> inlcudesAdd = new List<string>();
                if (extraDetailFields != null && extraDetailFields.Count > 0)
                {
                    hasExtraFields = true;
                    if(!extraDetailFields.Contains("RowVersion")){
                        extraDetailFields.Add("RowVersion");
                    }
                    if(!extraDetailFields.Contains("xmin")){
                        extraDetailFields.Add("xmin");
                    }
                    if(!extraDetailFields.Contains(relatedChildField)){
                        extraDetailFields.Add(relatedChildField);
                    }
                    CreateQueryExtraFields<TChild>(query, inlcudesAdd, extraDetailFields, ref selectedFields, ref hasRelated);
                }
                var rowidValueProperty = ConvertToPropertyType(relatedChildField, rowid);
                query = query.Where($"{relatedChildField} == @0", rowidValueProperty);
                if(hasExtraFields){
                    query = query.Select<TChild>($"new ({selectedFields})");
                }
                List<TChild> result = query.ToList();
                return new ActionResult<List<TChild>>{
                    Data = result,
                };
            }
        } 
    }
}

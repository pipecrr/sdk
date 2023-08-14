using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Validators;
namespace Siesa.SDK.Business
{
    /// <summary>
    /// Represents a class for managing parent-child relationships in the SDK context, inherits from BLFrontendSimple.
    /// </summary>
    /// <typeparam name="TParent">The type of the parent object.</typeparam>
    /// <typeparam name="TChild">The type of the child object.</typeparam>
    public class BLFrontendDocment<TParent, TChild> : BLFrontendSimple<TParent,BLBaseValidator<TParent>> where TParent : class,IBaseSDK
    {   
        /// <summary>
        /// Initializes a new instance of the <see cref="BLFrontendDocment{TParent, TChild}"/> class.
        /// </summary>
        /// <param name="authenticationService">The authentication service.</param>
        public BLFrontendDocment(IAuthenticationService authenticationService) : base(authenticationService)
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
        /// Initializes the ChildObjs.
        /// </summary>
        /// <returns>The task.</returns>
        public async Task InitializeChilds()
        {
            var response = await Backend.Call("GetChilds",BaseObj.GetRowid(), ExtraDetailFields).ConfigureAwait(true);
            if (response.Success)
            {
                ChildObjs = response.Data;
            }
        }

        /// <summary>
        /// This method is called when any field of the grid is change and adds the rowid of the parent object to the list of updated rowids.
        /// </summary>
        /// <param name="row">The full row that was update</param>
        public void SdkOnChangeCell(dynamic row)
        {

            if (row.Rowid > 0 && !ChildRowidsUpdated.Contains(row.Rowid))
            {
                ChildRowidsUpdated.Add(row.Rowid);
            }
        }
    }
}

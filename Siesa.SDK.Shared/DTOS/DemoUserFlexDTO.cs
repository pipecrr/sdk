using System;
using Siesa.SDK.Entities;

namespace Siesa.SDK.Shared.DTOS
{
    /// <summary>
    /// Represents a DTO for Demo User with flex component from api
    /// </summary>
    public class DemoUserFlexDTO : IBaseSDK
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Gets or sets the role of the user.
        /// </summary>
        public string role { get; set; }
        /// <summary>
        /// Gets or sets the avatar URL of the user.
        /// </summary>
        public string avatar { get; set; }
        /// <summary>
        /// Gets or sets the creation date and time of the user.
        /// </summary>
        public DateTime creationAt { get; set; }
        /// <summary>
        /// Gets or sets the last update date and time of the user.
        /// </summary>
        public DateTime updatedAt { get; set; }
        /// <inheritdoc/>
        public bool CheckRowid(long rowid)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>
        public long GetRowid()
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>
        public void SetRowid(long rowid)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>
        public object GetRowidObject()
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>
        public Type GetRowidType()
        {
            throw new NotImplementedException();
        }
    }
}
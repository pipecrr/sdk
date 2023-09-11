using System;
using Siesa.SDK.Entities;

namespace Siesa.SDK.Shared.DTOS
{
    /// <summary>
    /// Represents a DTO for Demo User with flex component from file .txt
    /// </summary>
    public class DemoUserFlexTextDTO : IBaseSDK
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// Gets or sets the age of the user.
        /// </summary>
        public int Edad { get; set; }
        /// <summary>
        /// Gets or sets the address of the user.
        /// </summary>
        public string Direccion { get; set; }
        /// <summary>
        /// Gets or sets the date of birth of the user.
        /// </summary>
        public DateTime FechaNacimiento { get; set; }
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


using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{
    public abstract class BaseCompanyTransaction<T>: BaseCompany<T>
    {
        //TODO: Agregar relación con attachment
    }
}

using Siesa.SDK.Entities;
using System.Collections.Generic;

namespace Siesa.SDK.Frontend.Components.Documentation.DTOs
{
    public class TestMultiSelectDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public List<E00220_User> Users { get; set; }
    }
}
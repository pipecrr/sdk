
using System;
using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class Entry
    {
        public int Rowid { get; set; }
        public string Name { get; set; }
        public bool IsDirectory { get; set; }
        public List<Entry> Children { get; set; }
        public string Code { get; set; }
        public Entry Parent { get; set; }

        public bool isEdit { get; set; }

        public string GetPath()
        {
            var path = Name;
            var parent = Parent;
            while (parent != null)
            {
                path = parent.GetPath() + "/" + path;
                parent = parent.Parent;
            }

            return path;
        }
    }
}

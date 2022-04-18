
using System;
using  System.Collections.Generic;


namespace Siesa.SDK.Shared.Utilities
{
    public class Utilities
    {
        public Utilities()
        {

        }

        public static string  GetDinamycWhere(Dictionary<string,object> inDictionary){

            string filter=string.Empty;

            foreach(var field in inDictionary)
            {
                if(!String.IsNullOrEmpty(filter))  filter+=" AND ";

                filter+=$"{field.Key}==\"{field.Value}\"";  

            }

            return filter;

        }

    }
}


using System;
using  System.Collections.Generic;


namespace Siesa.SDK.Shared.Utilities
{
    public class Utilities
    {
        public Utilities()
        {

        }

        public static string  GetDinamycWhere(Dictionary<string,object> inDictionary, Dictionary<string,object> PrimaryKey){

            string filter=string.Empty;

            foreach(var field in inDictionary)
            {
                if(!String.IsNullOrEmpty(filter))  filter+=" AND ";

                filter+=$"{field.Key}==\"{field.Value}\"";  

            }

            foreach(var keyPropertie in PrimaryKey)
            {
                if(keyPropertie.Value !=null){
                    
                    if(!String.IsNullOrEmpty(filter))  filter+=" AND ";
                    
                    filter+=$"{keyPropertie.Key}!=\"{keyPropertie.Value}\"";  
                }
                
            }

            return filter;

        }

    }
}

using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Shared.Json;
using Microsoft.EntityFrameworkCore;
using Siesa.Global.Enums;

namespace Siesa.SDK.Entities
{
	public partial class E00061_Menu
	{
		public virtual ICollection<E00061_Menu> SubMenus { get; set; }
		[NotMapped]
        public virtual string ResourceTag { get; set; }
        [NotMapped]
        public virtual string CurrentText { get; set; }
        [NotMapped]
        public virtual string StyleColor { get; set; }
        
        [NotMapped]
        [JsonIgnore]
        public virtual Action<E00061_Menu> CustomAction {get;set;}
        [NotMapped]
        [JsonIgnore]
        public virtual bool? Show {get; set;}

        [NotMapped]
        [JsonIgnore]
        public virtual string CurrentURL { get{
            var url = "";

            if(SubMenus != null && SubMenus.Count > 0)
            {
                url = $"/Menu/{Rowid}/";
            }else{
                if(this.Type == EnumMenuType.Feature){
                    if(this.Feature != null && !string.IsNullOrEmpty(this.Url)){
                        url = $"/{this.Feature.BusinessName}/{this.Url}/";
                    }else{
                        url = $"/{this.Feature.BusinessName}/";
                    }
                }else if (this.Type == EnumMenuType.CustomMenu)
                {
                    if (!string.IsNullOrEmpty(this.Url)) 
                    {
                        url = this.Url;
                    }             
                }
            }
            return url;
        } }


	}
}
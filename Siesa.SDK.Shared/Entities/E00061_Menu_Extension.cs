using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Shared.Json;
using Microsoft.EntityFrameworkCore;


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
        public virtual string CurrentURL { get{
            var url = "";
            if (!string.IsNullOrEmpty(this.Url)) {
                url = this.Url;
            }else{
                if(this.Feature != null){
                    url = $"/{this.Feature.BusinessName}/";
                }else{
                    url = "/";
                }
            }
            return url;
        } }


	}
}
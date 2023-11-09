export function PoblateWatermark() {
    setTimeout(function () {
        var clientWidth = document.body.clientWidth;
        Array.from(document.querySelectorAll('.sdk-test-watermark')).forEach(function(el) {

            var text = el.dataset.watermark;
            var textLength = text.length;
            var repeatTimes = Math.floor(clientWidth / textLength * 10);

            el.dataset.watermark = (el.dataset.watermark + ' ').repeat(repeatTimes);
        });
    }, 500);
}


export function checkAndRenewToken(dotnethelper) {
    dotnethelper.invokeMethodAsync("ShowLogin");
    /*var timeout_seconds = 10;
    var token = localStorage.getItem('usertoken');
    if (token) {
        var tokenData = JSON.parse(atob(token.split('.')[1]));
        var now = new Date();
        var tokenExpiration = new Date(tokenData.exp * 1000);
        var lastInteraction = localStorage.getItem('lastInteraction');
        if (now > new Date(tokenExpiration - 60000 * 5) && ((now - lastInteraction) < 60000 * 2)) {
            dotnethelper.invokeMethodAsync("RenewToken");
            timeout_seconds = 5;
        } else if (now > tokenExpiration) {
            dotnethelper.invokeMethodAsync("ShowLogin");
            timeout_seconds = 5;
        }else{
            //check if exits a div with class sdk-modal-login
            var loginModal = document.querySelector('.sdk-modal-login');
            if (loginModal) {
                dotnethelper.invokeMethodAsync("HideLogin");
            }
        }
    }
    setTimeout(function () {
        checkAndRenewToken(dotnethelper);
    }, 1000 * timeout_seconds);*/
    
} 

export async function InitSDK(dotnethelper){
    document.onmousemove = resetTimeDelay;
    document.onkeypress = resetTimeDelay;
    window.SDKDotNetHelper = dotnethelper;

    function resetTimeDelay() {
        //save the last time the user interacted with the page
        localStorage.setItem('lastInteraction', new Date().getTime());
    }

    //get openned tabs from local storage and add the current tab
    var n_tabs = localStorage.getItem('n_tabs');
    if (n_tabs == null) {
        n_tabs = 0;
    }
    n_tabs++;
    localStorage.setItem('n_tabs', n_tabs);

    // //generate a guid
    // var tab_guid = (function () {
    //     function s4() {
    //         return Math.floor((1 + Math.random()) * 0x10000)
    //             .toString(16)
    //             .substring(1);
    //     }
    //     return function () {
    //         return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
    //             s4() + '-' + s4() + s4() + s4();
    //     };
    // })();

    // //save the guid in session storage
    // sessionStorage.setItem('guid', tab_guid);
    checkAndRenewToken(dotnethelper);

    // dispatch event
    var event = new CustomEvent('sdkinit', { detail: { dotnethelper: dotnethelper } });
    document.dispatchEvent(event);
    
    var bd = localStorage.getItem('bd');
    if (bd == null || bd != 'sync'){
        var response = await getResouces();
        if(response.data.Data){
            var data = response.data.Data;
            const db = await new Dexie("IndexDb").open();
            var Cultures = data.Cultures;
            db.table("Cultures").clear();
            db.table("Cultures").bulkPut(Cultures);
            var Resources = data.Resources;
            db.table("Resources").clear();
            db.table("Resources").bulkPut(Resources);
            var ResourcesDetail = data.ResourcesDetail;
            db.table("ResourcesDetail").clear();
            db.table("ResourcesDetail").bulkPut(ResourcesDetail);

            let culture = {};
            if(!window.sdkFlexCulture){
                window.sdkFlexCulture = Cultures[0];
                culture = Cultures[0];
            }else{
                culture = window.sdkFlexCulture;
            }                       
            var rowidCulture = culture.id; 
            var resourcesDetail = ResourcesDetail.filter(x => x.CultureId == culture.Id).map(x => {return {[x.idResource]:x.description}})
            if(!window.ResourceFlex){
                window.ResourceFlex = {};
            }
            window.ResourceFlex[rowidCulture] = resourcesDetail;
            db.close();
        }
        localStorage.setItem('bd', 'sync');        
    }
}

async function getResouces() {
    
    return await fetch('/api/BLResource/GetAllResourcesForIndexedDB/', {
        method: 'GET',
        credentials: "same-origin",
        headers: {
          "Accept": "application/json",
          'X-Requested-With': 'XMLHttpRequest'
        }
      }).then(res => {
        return res.json();
      }).catch(err => {
        console.log(err);
    });
}

window.addEventListener('storage', function(event){
    if (event.key == 'usertoken') {
        if(event.newValue == null){
            window.location.href = '/login';
        }
        
        if(event.oldValue != event.newValue && window.location.pathname == '/login'){
            // reload page
            window.location.reload();
        }
    }
});

//onclose event
window.addEventListener('beforeunload', function (e) {
    //get openned tabs from local storage and remove the current tab
    var n_tabs = localStorage.getItem('n_tabs');
    if (n_tabs == null) {
        n_tabs = 0;
    }
    n_tabs--;
    localStorage.setItem('n_tabs', n_tabs);

    //if there are no more openned tabs, remove the last interaction time
    if (n_tabs == 0) {
        localStorage.removeItem('lastInteraction');
        localStorage.removeItem('n_tabs');
        //remove user token
        localStorage.removeItem('usertoken');
        
    }
});

(() => {
    preloadFlex();
})();
// $.ajaxSetup({
//     cache: false
// });

function ListViewInstance(dotnethelper, key){
    if(!window.dotnethelpersListView){
        var data = new Map();
        data.set(key, dotnethelper);          
        window.dotnethelpersListView = data;
    }else{
        window.dotnethelpersListView.set(key, dotnethelper);
    }
}

function existMountFlex(){
    return window.MountFlex != null;
}

function MountFlex(div_id, retries = 0)
{
    if(!window.ResourceFlex){
        window.ResourceFlex = {}
    }
    if(document.getElementById(div_id) == null)
    {
        console.log("Flex debug div not found, creating it", retries);
        if(retries < 10)
        {
            setTimeout(function(){MountFlex(div_id, retries + 1)}, 500);
            return;
        }else{
            console.log("FlexComponent.js: Flex not loaded");
            return;
        }
    }
    //
    document.getElementById(div_id).innerHTML = "";
    console.log("FlexComponent.js: Flex loading");
    let interval_flexdebug = setInterval(function () {
        if ("mountOReportsReact" in window) {
            clearInterval(interval_flexdebug);
            mountOReportsReact(div_id);
            console.log("FlexComponent.js: Flex loaded");
        }
    }, 100);
}
(() => {
    window.MountFlex = MountFlex;
    window.ListViewInstance = ListViewInstance;
    window.existMountFlex = existMountFlex;
/*
    window.addEventListener('locationchange', function () {
        console.log("locationchange probando");
         setTimeout(function  () { MountFlex() }, 200);
    });
    MountFlex();*/
    
})();
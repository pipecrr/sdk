// $.ajaxSetup({
//     cache: false
// });

function loadScript(url, in_head = false, callback = null) {
    var parent = in_head ? document.head : document.body;
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.src = url;

    if (callback) {
        script.onreadystatechange = callback;
        script.onload = callback;
    }
    parent.appendChild(script);
}

function loadCss(url){
    var link = document.createElement("link");
    link.type = "text/css";
    link.rel = "stylesheet";
    link.href = url;
    document.getElementsByTagName("head")[0].appendChild(link);
}

function ListViewInstance(dotnethelper, key){
    if(!window.dotnethelpersListView){
        var data = new Map();
        data.set(key, dotnethelper);          
        window.dotnethelpersListView = data;
    }else{
        window.dotnethelpersListView.set(key, dotnethelper);
    }
}

function MountFlex(div_id, retries = 0)
{
    if(!window.ResourceFlex){
        window.ResourceFlex = {}
    }
    if(document.getElementById("flexdebug") == null)
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
    document.getElementById("flexdebug").innerHTML = "";

    let interval_flexdebug = setInterval(function () {
        if ("mountOReportsReact" in window) {
            clearInterval(interval_flexdebug);
            mountOReportsReact("flexdebug");
        }
    }, 100);
}
(() => {

    loadCss('/_content/Siesa.SDK.Frontend/flex/static/css/2.css?v=20221219');
    loadCss('/_content/Siesa.SDK.Frontend/flex/static/css/main.css?v=20221219');

    loadScript("/_content/Siesa.SDK.Frontend/flex/static/js/2.chunk.js?v=20221219");
    loadScript("/_content/Siesa.SDK.Frontend/flex/static/js/main.chunk.js?v=20221219");
    loadScript("/_content/Siesa.SDK.Frontend/flex/static/js/runtime-main.js?v=20221219");

    window.MountFlex = MountFlex;
    window.ListViewInstance = ListViewInstance;
/*
    window.addEventListener('locationchange', function () {
        console.log("locationchange probando");
         setTimeout(function  () { MountFlex() }, 200);
    });
    MountFlex();*/
    
})();
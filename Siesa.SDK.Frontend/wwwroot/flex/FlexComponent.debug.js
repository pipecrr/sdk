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

function ListViewInstance(dotnethelper){
    window.dotnethelperListView = dotnethelper;
}

function MountFlex(id)
{
    console.log("porobando",id);
    if(document.getElementById(id) == null)
    {
        return;
    }
    let interval_flexdebug = setInterval(function () {
        if ("mountOReportsReact" in window) {
            clearInterval(interval_flexdebug);
            mountOReportsReact(id);
        }
    }, 100);
}

(() => {
    loadScript("http://127.0.0.1:3000/static/js/bundle.js");
    loadScript("http://127.0.0.1:3000/static/js/0.chunk.js");
    loadScript("http://127.0.0.1:3000/static/js/1.chunk.js");
    loadScript("http://127.0.0.1:3000/static/js/main.chunk.js");
    window.MountFlex = MountFlex;    
    window.ListViewInstance = ListViewInstance;

    // window.addEventListener('locationchange', function () {
    //         setTimeout(function  () { MountFlex() }, 200);
    // });
    //MountFlex();
        
})();
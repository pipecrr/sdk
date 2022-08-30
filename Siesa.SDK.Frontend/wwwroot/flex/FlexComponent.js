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

(() => {

    loadCss('/_content/Siesa.SDK.Frontend/flex/static/css/2.css?v=20220830');
    loadCss('/_content/Siesa.SDK.Frontend/flex/static/css/main.css?v=20220830');

    loadScript("/_content/Siesa.SDK.Frontend/flex/static/js/2.chunk.js?v=20220830");
    loadScript("/_content/Siesa.SDK.Frontend/flex/static/js/main.chunk.js?v=20220830");
    loadScript("/_content/Siesa.SDK.Frontend/flex/static/js/runtime-main.js?v=20220830");



    let interval_flexdebug = setInterval(function () {
        if ("mountOReportsReact" in window) {
            clearInterval(interval_flexdebug);
            mountOReportsReact("flexdebug");
        }
    }, 100);
})();
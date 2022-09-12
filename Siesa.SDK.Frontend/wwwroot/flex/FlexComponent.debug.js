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

(() => {
    loadScript("http://127.0.0.1:3000/static/js/bundle.js");
    loadScript("http://127.0.0.1:3000/static/js/0.chunk.js");
    loadScript("http://127.0.0.1:3000/static/js/1.chunk.js");
    loadScript("http://127.0.0.1:3000/static/js/main.chunk.js");

    let interval_flexdebug = setInterval(function () {
        if ("mountOReportsReact" in window) {
            clearInterval(interval_flexdebug);
            mountOReportsReact("flexdebug");
        }
    }, 100);
})();
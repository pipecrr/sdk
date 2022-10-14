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

window.loadScript = loadScript;
window.loadCss = loadCss;
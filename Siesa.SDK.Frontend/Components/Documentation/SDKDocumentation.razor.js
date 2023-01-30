(() => {
    var search = window.location.search;
    var params = search.substring(1,search.length).split('&');
    var sdk_debug = params.find(x => x == 'sdk_debug=1');
    if(sdk_debug){
        loadScript("http://127.0.0.1:3000/static/js/bundle.js");
        loadScript("http://127.0.0.1:3000/static/js/0.chunk.js");
        loadScript("http://127.0.0.1:3000/static/js/1.chunk.js");
        loadScript("http://127.0.0.1:3000/static/js/main.chunk.js");
    }else{
        loadCss('/_content/Siesa.SDK.Frontend/flex/static/css/2.css?v=20230104');
        loadCss('/_content/Siesa.SDK.Frontend/flex/static/css/main.css?v=20230104');

        loadScript("/_content/Siesa.SDK.Frontend/flex/static/js/2.chunk.js?v=20230104");
        loadScript("/_content/Siesa.SDK.Frontend/flex/static/js/main.chunk.js?v=20230104");
        loadScript("/_content/Siesa.SDK.Frontend/flex/static/js/runtime-main.js?v=20230104");
    }
    loadScript("/_content/Siesa.SDK.Frontend/vendor/dexie/dexie.js");
})();
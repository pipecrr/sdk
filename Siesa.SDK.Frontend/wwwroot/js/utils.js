const version = '20231220';

function loadScript(url, in_head = false, callback = null) {

    //check if script is already loaded
    var scripts = document.getElementsByTagName('script');
    for (var i = 0; i < scripts.length; i++) {
        if (scripts[i].src == url) {
            // if (callback) {
            //     callback();
            // }
            return;
        }
    }

    var parent = in_head ? document.head : document.body;
    var script = document.createElement('script');
    script.type = 'application/javascript';
    script.src = url;

    if (callback) {
        script.onreadystatechange = callback;
        script.onload = callback;
    }
    parent.appendChild(script);
}

function loadCss(url){
    //check if css is already loaded
    var links = document.getElementsByTagName('link');
    for (var i = 0; i < links.length; i++) {
        if (links[i].href == url) {
            return;
        }
    }
    var link = document.createElement("link");
    link.type = "text/css";
    link.rel = "stylesheet";
    link.href = url;
    document.getElementsByTagName("head")[0].appendChild(link);
}

function createCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function deleteCookie(name) {
    document.cookie = name + '=; Max-Age=-99999999;';
}

function SetFocusToElement(dataAutomationId){
    try {
        const element = document.querySelector(`[data-automation-id="${dataAutomationId}"]`);
        if (element) {
          element.focus();
        }
      } catch (error) {
        // No hacer nada en caso de que el elemento no se encuentre
      }
    //document.querySelector(`[data-automation-id="${dataAutomationId}"]`).focus();
}

function preloadFlex(){
    var search = window.location.search;
    var params = search.substring(1,search.length).split('&');
    var sdk_debug = params.find(x => x == 'sdk_debug=1');
    //sdk_debug = true;
    if(sdk_debug){
        loadScript('/_content/Siesa.SDK.Frontend/flex/FlexComponent.debug.js');
        loadScript("http://127.0.0.1:3000/static/js/bundle.js");
        loadScript("http://127.0.0.1:3000/static/js/0.chunk.js");
        loadScript("http://127.0.0.1:3000/static/js/1.chunk.js");
        loadScript("http://127.0.0.1:3000/static/js/main.chunk.js");
    }else{
        // loadCss(`/_content/Siesa.SDK.Frontend/flex/static/css/2.css?v=${version}`);
        loadCss(`/_content/Siesa.SDK.Frontend/flex/static/css/main.css?v=${version}`);

        loadScript(`/_content/Siesa.SDK.Frontend/flex/FlexComponent.js?v=${version}`);
        // loadScript(`/_content/Siesa.SDK.Frontend/flex/static/js/2.chunk.js?v=${version}`);
        loadScript(`/_content/Siesa.SDK.Frontend/flex/static/js/main.js?v=${version}`);
        // loadScript(`/_content/Siesa.SDK.Frontend/flex/static/js/runtime-main.js?v=${version}`);
    }
    loadScript("/_content/Siesa.SDK.Frontend/vendor/dexie/dexie.js");
}

function downloadFileFromStream(fileName, bytes) {
    var blob = new Blob([bytes], {type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
    var url = URL.createObjectURL(blob);
    var anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName || '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
  }

  function previewImage (inputElem, imgElem)  {
    const url = URL.createObjectURL(inputElem.files[0]);
    imgElem.addEventListener('load', () => URL.revokeObjectURL(url), { once: true });
    imgElem.src = url;
  }

  function closePreviewImage (imgElem) 
  {
    
    if (imgElem == null) {
        
        var element = document.getElementById("single_file_icon");

        if (element) {
            element.parentNode.removeChild(element);
        }
    } else 
    {
        
        imgElem.src = '';
    }
  }

  function getUrlsImage (inputElem){
    let files = inputElem.files;
    var urls = [];
    for (var i = 0; i < files.length; i++) {
        urls.push(URL.createObjectURL(files[i]));
    }
    return urls;
  }

  function clickInputFile (element) {
    element.click();
  }
  
function globalVariableExists(variableName) {
    if (typeof window[variableName] !== 'undefined') {
      return true;
    }
    return false;
}
  

window.downloadFileFromStream = downloadFileFromStream;
window.loadScript = loadScript;
window.loadCss = loadCss;
window.createCookie = createCookie;
window.readCookie = readCookie;
window.deleteCookie = deleteCookie;
window.SetFocusToElement = SetFocusToElement;
window.preloadFlex = preloadFlex;
window.previewImage = previewImage;
window.closePreviewImage = closePreviewImage;
window.getUrlsImage = getUrlsImage;
window.clickInputFile = clickInputFile;
window.globalVariableExists = globalVariableExists;
(function(){
    if(typeof(getLocalResourceRoot) === 'undefined'){
        window.getLocalResourceRoot = function(){
            return '';
        }
    }
})()

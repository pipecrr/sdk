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

function MountFlex(id)
{
    if(document.getElementById(id) == null)
    {
        return;
    }
    if(!window.ResourceFlex){
        window.ResourceFlex = {}
    }
    let interval_flexdebug = setInterval(function () {
        if ("mountOReportsReact" in window) {
            clearInterval(interval_flexdebug);
            mountOReportsReact(id);
        }
    }, 100);
}

(() => {
    
    window.MountFlex = MountFlex;    
    window.ListViewInstance = ListViewInstance;
    window.existMountFlex = existMountFlex;

    // window.addEventListener('locationchange', function () {
    //         setTimeout(function  () { MountFlex() }, 200);
    // });
    //MountFlex();
        
})();
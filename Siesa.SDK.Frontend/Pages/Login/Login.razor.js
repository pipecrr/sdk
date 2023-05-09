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

(() => {
    preloadFlex();
})();
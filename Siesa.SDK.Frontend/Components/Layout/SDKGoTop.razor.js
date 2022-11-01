window.onscroll = function(){
    if(document.documentElement.scrollTop > 90){
        document.querySelector('.sdk_go_top')
        .classList.add('sdk_show');
    }else{
        document.querySelector('.sdk_go_top')
        .classList.remove('sdk_show');
    }
}

export function goToTop(){
    window.scrollTo({
        top:0,
        behavior: 'smooth'
    });
}
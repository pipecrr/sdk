window.onscroll = function(){
    if(document.documentElement.scrollTop > 90){
        document.querySelector('.sdk_go_top')
        .classList.add('sdk_show');
    }else{
        document.querySelector('.sdk_go_top')
        .classList.remove('sdk_show');
    }
}

document.querySelector('.sdk_go_top')
.addEventListener('click', () =>{
    window.scrollTo({
        top:0,
        behavior: 'smooth'
    });
});
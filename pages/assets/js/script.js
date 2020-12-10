window.addEventListener("scroll", () => {
    if(window.pageYOffset) 
        document.querySelector("nav").classList.add("scrolled");
    else
        document.querySelector("nav").classList.remove("scrolled");

    let lowScroll = window.pageYOffset / 4;
    let header = document.querySelector("header");
    let headerContent = document.querySelector("header > .container");

    header.style.transform = "translate3d(0, " + lowScroll + "px, 0)";
    headerContent.style.opacity = (100 - lowScroll / 1.25) + "%";
});
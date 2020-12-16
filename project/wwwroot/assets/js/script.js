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

let tmdbURL = "https://api.themoviedb.org/3/";
let tmdbKey = "684e74556adcb4fe60b7c3b4399540ff";

// Recommendation Header
fetch('/api/movies/' + Math.floor(Math.random() * 15000 + 1))
    .then(response => response.json())
    .then(data => {
        fetch(tmdbURL + 'movie/' + data.tmdbId + '?api_key=' + tmdbKey + '&language=en-US')
            .then(response => response.json())
            .then(data => {
                title = document.querySelector("header > .container .info .info-main .info-main-title h1");
                year = document.querySelector("header > .container .info .info-main .info-main-title span");
                description = document.querySelector("header > .container .info .info-description");
                poster = document.querySelector("header img.poster");
                header = document.querySelector("header");

                title.innerHTML = data.title;
                year.innerHTML = "(" + data.release_date.substring(0, 4) + ")";
                description.innerHTML = data.overview;
                poster.src = poster.src.substr(0, poster.src.lastIndexOf('/')) + data.poster_path;
            });
    });

// Top Picks
fetch('/api/movies?page=' + Math.floor(Math.random() * 3999 + 1) + '&pageSize=5')
    .then(response => response.json())
    .then(data => {
        // console.log(data);

        for(let i in data) {
            fetch(tmdbURL + 'movie/' + data[i].tmdbId + '?api_key=' + tmdbKey + '&language=en-US')
                .then(response => response.json())
                .then(data => {
                    title = document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") h3");
                    title.innerHTML = data.title;

                    poster = document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") img.poster");
                    poster.src = poster.src.substr(0, poster.src.lastIndexOf('/')) + data.poster_path;
            });
        }
    });
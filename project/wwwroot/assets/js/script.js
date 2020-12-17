window.addEventListener("scroll", () => {
  if (window.pageYOffset)
    document.querySelector("nav").classList.add("scrolled");
  else document.querySelector("nav").classList.remove("scrolled");

  let lowScroll = window.pageYOffset / 4;
  let header = document.querySelector("header");
  let headerContent = document.querySelector("header > .container");

  header.style.transform = "translate3d(0, " + lowScroll + "px, 0)";
  headerContent.style.opacity = 100 - lowScroll / 1.25 + "%";
});

let tmdbURL = "https://api.themoviedb.org/3/";
let tmdbKey = "684e74556adcb4fe60b7c3b4399540ff";

function transformLocalTitle(text) {
  text = text.replace(/\s*\(.*?\)\s*/g, "");
  if (text.slice(-5) === ", The") {
    text = "The " + text.slice(0, -5);
  } else if (text.slice(-3) === ", A") {
    text = "The " + text.slice(0, -3);
  }
  return text;
}

// se sterge cand se mai rezolva la back
function fetchRetryHeader(url, options = {}, retries = 15) {
  return fetch(url, options)
    .then((res) => {
      if (res.ok) return res.json();
      if (retries > 0) {
        console.log("error header... retrying");
        return fetchRetryHeader(
          "/api/movies/" + Math.floor(Math.random() * 10000),
          options,
          retries - 1
        );
      } else throw new Error(res);
    })
    .catch(console.error);
}

// se sterge cand se mai rezolva la back
function fetchRetryTop(url, options = {}, retries = 10) {
  return fetch(url, options)
    .then((res) => {
      if (res.ok) return res.json();
      if (retries > 0) {
        console.log("error top... retrying");
        return fetchRetryTop(
          "/api/movies?page=" +
            Math.floor(Math.random() * 2000) +
            "&pageSize=5",
          options,
          retries - 1
        );
      } else throw new Error(res);
    })
    .catch(console.error);
}

// Recommendation Header
fetchRetryHeader("/api/v1/movies/" + Math.floor(Math.random() * 10000))
  //.then(response => response.json())
  .then((data) => {
    fetch(
      tmdbURL +
        "movie/" +
        data.tmdbId +
        "?api_key=" +
        tmdbKey +
        "&language=en-US"
    )
      .then((tmdbResponse) => tmdbResponse.json())
      .then((tmdbData) => {
        let title = document.querySelector(
          "header > .container .info .info-main .info-main-title h1"
        );
        let year = document.querySelector(
          "header > .container .info .info-main .info-main-title span"
        );
        let description = document.querySelector(
          "header > .container .info .info-description"
        );
        let language = document.querySelector(
          "header > .container .info .info-main .info-main-data .language"
        );
        let genres = document.querySelector(
          "header > .container .info .info-main .info-main-data .genres"
        );
        let duration = document.querySelector(
          "header > .container .info .info-main .info-main-data .duration"
        );

        tmdbData.title
          ? (title.innerHTML = tmdbData.title)
          : (title.innerHTML = transformLocalTitle(data.title));
        tmdbData.release_date
          ? (year.innerHTML = "(" + tmdbData.release_date.substring(0, 4) + ")")
          : (year.innerHTML = "(" + data.title.slice(-5).slice(0, -1) + ")");
        tmdbData.overview
          ? (description.innerHTML = tmdbData.overview)
          : description.remove();
        tmdbData.spoken_languages && tmdbData.spoken_languages.length
          ? (language.innerHTML = tmdbData.spoken_languages[0].english_name
              ? tmdbData.spoken_languages[0].english_name
              : tmdbData.spoken_languages[0].name)
          : language.remove();
        genres !== "(no genres listed)" && genres
          ? (genres.innerHTML = data.genres.replaceAll("|", ", "))
          : (genres.innerHTML = "<i>no genres listed</i>");
        tmdbData.runtime
          ? (duration.innerHTML =
              Math.floor(parseInt(tmdbData.runtime) / 60) +
              "h " +
              (parseInt(tmdbData.runtime) % 60) +
              "m")
          : duration.remove();

        let poster = document.querySelector("header img.poster");

        if (tmdbData.poster_path) {
          poster.src = "http://image.tmdb.org/t/p/w300" + tmdbData.poster_path;
        } else {
          document.querySelector("header > .container").style.padding =
            "45px 0 175px 0";
          poster.remove();
        }

        if (tmdbData.backdrop_path) {
          let header = document.querySelector("header");
          document.querySelector("nav").classList.add("invert");
          document.querySelector("header > .container").style.color = "#fff";
          header.style.backgroundImage =
            "linear-gradient(rgba(0, 0, 0, 0.7), rgba(0, 0, 0, 0.2)), linear-gradient(rgba(0, 0, 0, 0.2), rgba(0, 0, 0, 0.5)), url(https://image.tmdb.org/t/p/original" +
            tmdbData.backdrop_path +
            ")";
        }
      });

    fetch(
      tmdbURL +
        "movie/" +
        data.tmdbId +
        "/credits?api_key=" +
        tmdbKey +
        "&language=en-US"
    )
      .then((tmdbResponse) => tmdbResponse.json())
      .then((tmdbData) => {
        if (tmdbData.cast && tmdbData.cast.length) {
          let cast = document.querySelector(
            "header > .container .info .info-cast .cast"
          );
          let castList = [];
          for (let i = 0; i < tmdbData.cast.length && i < 3; i++)
            castList.push(tmdbData.cast[i].name);
          cast.innerHTML = "Cast: " + castList.join(", ");
        }
        if (tmdbData.crew && tmdbData.crew.length) {
          let director = document.querySelector(
            "header > .container .info .info-cast .director"
          );
          let directorList = [];
          for (let i in tmdbData.crew)
            if (tmdbData.crew[i].job === "Director")
              directorList.push(tmdbData.crew[i].name);
          director.innerHTML = "Director: " + directorList.join(", ");
        }
        let poster = document.querySelector("header img.poster");
        if (poster) poster.style.opacity = "1";
        document.querySelector("header > .container .info").style.display =
          "flex";
      });

    fetch("/api/v1/movies/" + data.id)
      .then((ratingResponse) => ratingResponse.json())
      .then((ratingData) => {
        let ratingStars = document.querySelectorAll(
          "header > .container .info .info-user a.rating .i"
        );
        if (ratingData.rating >= 1) {
          document.querySelector(
            "header > .container .info .info-user a.rating"
          ).style.color = "#ff9800";
          document.querySelector(
            "header > .container .info .info-user a.rating"
          ).style.opacity = 1;
          for (let i = 0; i < 5; i++) {
            if (i + 1 <= Math.floor(ratingData.rating))
              ratingStars[i].innerHTML = "star";
            else if (
              i === Math.floor(ratingData.rating) &&
              ratingData.rating - Math.floor(ratingData.rating) >= 0.5
            )
              ratingStars[i].innerHTML = "star_half";
            else ratingStars[i].innerHTML = "star_outline";
          }
        }
      });
  });

// Top Picks
fetchRetryTop(
  "/api/v1/movies?page=" + Math.floor(Math.random() * 2000) + "&pageSize=5"
)
  // .then(response => response.json())
  .then((data) => {
    for (let i in data) {
      fetch(
        tmdbURL +
          "movie/" +
          data[i].tmdbId +
          "?api_key=" +
          tmdbKey +
          "&language=en-US"
      )
        .then((tmdbResponse) => tmdbResponse.json())
        .then((tmdbData) => {
          let title = document.querySelector(
            "main section ul li:nth-child(" + (parseInt(i) + 1) + ") h3"
          );
          tmdbData.title
            ? (title.innerHTML = tmdbData.title)
            : (title.innerHTML = transformLocalTitle(data[i].title));

          if (tmdbData.poster_path) {
            let poster = document.querySelector(
              "main section ul li:nth-child(" +
                (parseInt(i) + 1) +
                ") img.poster"
            );
            poster.src =
              "https://image.tmdb.org/t/p/w220_and_h330_face" +
              tmdbData.poster_path;
          }
        });

      fetch("/api/v1/movies/" + data[i].id)
        .then((ratingResponse) => ratingResponse.json())
        .then((ratingData) => {
          let ratingStars = document.querySelectorAll(
            "main section ul li:nth-child(" + (parseInt(i) + 1) + ") .rating .i"
          );
          if (ratingData.rating >= 1) {
            document.querySelector(
              "main section ul li:nth-child(" + (parseInt(i) + 1) + ") .rating"
            ).style.color = "#ff9800";
            document.querySelector(
              "main section ul li:nth-child(" + (parseInt(i) + 1) + ") .rating"
            ).style.opacity = 1;
            for (let i = 0; i < 5; i++) {
              if (i + 1 <= Math.floor(ratingData.rating))
                ratingStars[i].innerHTML = "star";
              else if (
                i === Math.floor(ratingData.rating) &&
                ratingData.rating - Math.floor(ratingData.rating) >= 0.5
              )
                ratingStars[i].innerHTML = "star_half";
              else ratingStars[i].innerHTML = "star_outline";
            }
          }
        });
    }
  });

fetchRetryTop("/api/v1/movies/recent-releases" + "?pageSize=5")
  // .then(response => response.json())
  .then((data) => {
    for (let i in data) {
      fetch(
        tmdbURL +
          "movie/" +
          data[i].tmdbId +
          "?api_key=" +
          tmdbKey +
          "&language=en-US"
      )
        .then((tmdbResponse) => tmdbResponse.json())
        .then((tmdbData) => {
          let title = document.querySelector(
            "main section:nth-of-type(2) ul li:nth-child(" +
              (parseInt(i) + 1) +
              ") h3"
          );
          tmdbData.title
            ? (title.innerHTML = tmdbData.title)
            : (title.innerHTML = transformLocalTitle(data[i].title));

          if (tmdbData.poster_path) {
            let poster = document.querySelector(
              "main section:nth-of-type(2) ul li:nth-child(" +
                (parseInt(i) + 1) +
                ") img.poster"
            );
            poster.src =
              "https://image.tmdb.org/t/p/w220_and_h330_face" +
              tmdbData.poster_path;
          }
        });
    }
  });

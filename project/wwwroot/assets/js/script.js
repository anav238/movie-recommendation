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

let userId = 1;

function transformLocalTitle(text) {
	text = text.replace(/\s*\(.*?\)\s*/g, "");
	if (text.slice(-5) === ", The") {
		text = "The " + text.slice(0, -5);
	} else if (text.slice(-3) === ", A") {
		text = "The " + text.slice(0, -3);
	}
	return text;
}

if(window.location.pathname === "/" || window.location.pathname === "index.html" || window.location.pathname === "/index.html")
{
	// Recommendation Header
	fetch("/api/v1/Users/" + userId + "/recommendations")
		.then(response => response.json())
		.then(data => {
			fetch(tmdbURL + "movie/" + data[0].tmdbId + "?api_key=" + tmdbKey + "&language=en-US")
				.then(tmdbResponse => tmdbResponse.json())
				.then(tmdbData => {
					let title = document.querySelector("header > .container .info .info-main .info-main-title h1");
					let year = document.querySelector("header > .container .info .info-main .info-main-title span");
					let description = document.querySelector("header > .container .info .info-description");
					let language = document.querySelector("header > .container .info .info-main .info-main-data .language");
					let genres = document.querySelector("header > .container .info .info-main .info-main-data .genres");
					let duration = document.querySelector("header > .container .info .info-main .info-main-data .duration");

					tmdbData.title ? (title.innerHTML = tmdbData.title) : (title.innerHTML = transformLocalTitle(data[0].title));
					tmdbData.release_date ? (year.innerHTML = "(" + tmdbData.release_date.substring(0, 4) + ")") : (year.innerHTML = "(" + data[0].title.slice(-5).slice(0, -1) + ")");
					tmdbData.overview ? (description.innerHTML = tmdbData.overview) : description.remove();
					tmdbData.spoken_languages && tmdbData.spoken_languages.length ? (language.innerHTML = tmdbData.spoken_languages[0].english_name ? tmdbData.spoken_languages[0].english_name : tmdbData.spoken_languages[0].name) : language.remove();
					genres !== "(no genres listed)" && genres ? (genres.innerHTML = data[0].genres.replaceAll("|", ", ")) : (genres.innerHTML = "<i>no genres listed</i>");
					tmdbData.runtime ? (duration.innerHTML = Math.floor(parseInt(tmdbData.runtime) / 60) + "h " + (parseInt(tmdbData.runtime) % 60) + "m") : duration.remove();

					let poster = document.querySelector("header img.poster");

					if (tmdbData.poster_path) {
						poster.src = "http://image.tmdb.org/t/p/w300" + tmdbData.poster_path;
					} else {
						document.querySelector("header > .container").style.padding = "45px 0 175px 0";
						poster.remove();
					}

					if (tmdbData.backdrop_path) {
						let header = document.querySelector("header");
						document.querySelector("nav").classList.add("invert");
						document.querySelector("header > .container").style.color = "#fff";
						header.style.backgroundImage = "linear-gradient(rgba(0, 0, 0, 0.7), rgba(0, 0, 0, 0.2)), linear-gradient(rgba(0, 0, 0, 0.2), rgba(0, 0, 0, 0.5)), url(https://image.tmdb.org/t/p/original" + tmdbData.backdrop_path + ")";
					}
				});

			fetch(tmdbURL + "movie/" + data[0].tmdbId + "/credits?api_key=" + tmdbKey + "&language=en-US")
				.then(tmdbResponse => tmdbResponse.json())
				.then(tmdbData => {
					if (tmdbData.cast && tmdbData.cast.length) {
						let cast = document.querySelector("header > .container .info .info-cast .cast");
						let castList = [];
						for (let i = 0; i < tmdbData.cast.length && i < 3; i++)
							castList.push(tmdbData.cast[i].name);
						cast.innerHTML = "Cast: " + castList.join(", ");
					}
					if (tmdbData.crew && tmdbData.crew.length) {
						let director = document.querySelector("header > .container .info .info-cast .director");
						let directorList = [];
						for (let i in tmdbData.crew)
							if (tmdbData.crew[i].job === "Director")
								directorList.push(tmdbData.crew[i].name);
						director.innerHTML = "Director: " + directorList.join(", ");
					}
					let poster = document.querySelector("header img.poster");
					if (poster) poster.style.opacity = "1";
					document.querySelector("header > .container .info").style.display = "flex";
				});

			fetch("/api/v1/movies/" + data[0].id)
				.then(ratingResponse => ratingResponse.json())
				.then(ratingData => {
					let ratingStars = document.querySelectorAll("header > .container .info .info-user a.rating .i");
					if (ratingData.rating >= 1) {
						document.querySelector("header > .container .info .info-user a.rating").style.color = "#ff9800";
						document.querySelector("header > .container .info .info-user a.rating").style.opacity = 1;
						for (let i = 0; i < 5; i++) {
							if (i + 1 <= Math.floor(ratingData.rating))
								ratingStars[i].innerHTML = "star";
							else if (i === Math.floor(ratingData.rating) && ratingData.rating - Math.floor(ratingData.rating) >= 0.5)
								ratingStars[i].innerHTML = "star_half";
							else ratingStars[i].innerHTML = "star_outline";
						}
					}
				});
		});

	// Top Picks
	fetch("/api/v1/Users/" + userId + "/recommendations")
		.then(response => response.json())
		.then(data => {
			for (let i = 0; i < 5; i++) {
				fetch(tmdbURL + "movie/" + data[i].tmdbId + "?api_key=" + tmdbKey + "&language=en-US")
					.then(tmdbResponse => tmdbResponse.json())
					.then(tmdbData => {
						let title = document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") h3");
						tmdbData.title ? (title.innerHTML = tmdbData.title) : (title.innerHTML = transformLocalTitle(data[i].title));

						if (tmdbData.poster_path) {
							let poster = document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") img.poster");
							poster.src = "https://image.tmdb.org/t/p/w220_and_h330_face" + tmdbData.poster_path;
						}
					});

				fetch("/api/v1/movies/" + data[i].id)
					.then(ratingResponse => ratingResponse.json())
					.then(ratingData => {
						let ratingStars = document.querySelectorAll("main section ul li:nth-child(" + (parseInt(i) + 1) + ") .rating .i");
						if (ratingData.rating >= 1) {
							document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") .rating").style.color = "#ff9800";
							document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") .rating").style.opacity = 1;
							for (let i = 0; i < 5; i++) {
								if (i + 1 <= Math.floor(ratingData.rating))
									ratingStars[i].innerHTML = "star";
								else if (i === Math.floor(ratingData.rating) && ratingData.rating - Math.floor(ratingData.rating) >= 0.5)
									ratingStars[i].innerHTML = "star_half";
								else ratingStars[i].innerHTML = "star_outline";
							}
						}
					});
			}
		});

	fetch("/api/v1/movies/recent-releases?page=1&pageSize=5")
		.then(response => response.json())
		.then(data => {
			for (let i in data) {
				fetch(tmdbURL + "movie/" + data[i].tmdbId + "?api_key=" + tmdbKey + "&language=en-US")
					.then(tmdbResponse => tmdbResponse.json())
					.then(tmdbData => {
						let title = document.querySelector("main section:nth-of-type(2) ul li:nth-child(" + (parseInt(i) + 1) + ") h3");
						let year = document.querySelector("main section:nth-of-type(2) ul li:nth-child(" + (parseInt(i) + 1) + ") .date");
						tmdbData.title ? (title.innerHTML = tmdbData.title) : (title.innerHTML = transformLocalTitle(data[i].title));
						tmdbData.release_date ? (year.innerHTML = tmdbData.release_date.substring(0, 4)) : (year.innerHTML = data[0].title.slice(-5).slice(0, -1));

						if (tmdbData.poster_path) {
							let poster = document.querySelector("main section:nth-of-type(2) ul li:nth-child(" + (parseInt(i) + 1) + ") img.poster");
							poster.src = "https://image.tmdb.org/t/p/w220_and_h330_face" + tmdbData.poster_path;
						}
					});
			}
		});
} else if(window.location.pathname === "/top.html" || window.location.pathname === "/recents.html" || window.location.pathname === "/friends.html") {
	let endpoint;
	if(window.location.pathname === "/top.html") {
		endpoint = "/api/v1/Users/" + userId + "/recommendations"
	} else if(window.location.pathname === "/recents.html") {
		endpoint = "/api/v1/movies/recent-releases?page=1&pageSize=25";
	} else if(window.location.pathname === "/friends.html") {
		fetch("/api/v1/friendships/" + userId)
			.then(friendsListResponse => friendsListResponse.json())
			.then(friendsListData => {
				let friendsList = document.querySelector(".page header > .container ul.friendslist");

				for(let i in friendsListData) {
					let friendId;
					friendsListData[i].userId_1 !== userId ? friendId = friendsListData[i].userId_1 : friendId = friendsListData[i].userId_2;
					
					fetch("/api/v1/users/" + friendId)
						.then(friendDataResponse => friendDataResponse.json())
						.then(friendData => {
							listItem = document.createElement("li");
							listItem.appendChild(document.createTextNode(friendData.username));
							friendsList.appendChild(listItem);
						});
				}
			});
		endpoint = "/api/v1/Users/" + userId + "/friendswatching?page=1&pageSize=25";
	}
	fetch(endpoint)
		.then(response => response.json())
		.then(data => {
			for (let i = 0; i < 25; i++) {
				fetch(tmdbURL + "movie/" + data[i].tmdbId + "?api_key=" + tmdbKey + "&language=en-US")
					.then(tmdbResponse => tmdbResponse.json())
					.then(tmdbData => {
						let title = document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") .movie-details .movie-details-title h3");
						let year = document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") .movie-details .movie-details-title span");
						let description = document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") .movie-details .movie-details-description");
						let language = document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") .movie-details .movie-details-data .language");
						let genres = document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") .movie-details .movie-details-data .genres");
						let duration = document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") .movie-details .movie-details-data .duration");
						
						tmdbData.title ? (title.innerHTML = tmdbData.title) : (title.innerHTML = transformLocalTitle(data[i].title));
						tmdbData.release_date ? (year.innerHTML = "(" + tmdbData.release_date.substring(0, 4) + ")") : (year.innerHTML = "(" + data[i].title.slice(-5).slice(0, -1) + ")");
						tmdbData.overview ? (description.innerHTML = tmdbData.overview) : description.remove();
						tmdbData.spoken_languages && tmdbData.spoken_languages.length ? (language.innerHTML = tmdbData.spoken_languages[0].english_name ? tmdbData.spoken_languages[0].english_name : tmdbData.spoken_languages[0].name) : language.remove();
						genres !== "(no genres listed)" && genres ? (genres.innerHTML = data[i].genres.replaceAll("|", ", ")) : (genres.innerHTML = "<i>no genres listed</i>");
						tmdbData.runtime ? (duration.innerHTML = Math.floor(parseInt(tmdbData.runtime) / 60) + "h " + (parseInt(tmdbData.runtime) % 60) + "m") : duration.remove();

						if (tmdbData.poster_path) {
							let poster = document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") img.poster");
							poster.src = "https://image.tmdb.org/t/p/w220_and_h330_face" + tmdbData.poster_path;
						}
					});

				fetch(tmdbURL + "movie/" + data[i].tmdbId + "/credits?api_key=" + tmdbKey + "&language=en-US")
					.then(tmdbResponse => tmdbResponse.json())
					.then(tmdbData => {
						if (tmdbData.cast && tmdbData.cast.length) {
							let cast = document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") .movie-details .movie-details-cast .cast");
							let castList = [];
							for (let i = 0; i < tmdbData.cast.length && i < 3; i++)
								castList.push(tmdbData.cast[i].name);
							cast.innerHTML = "Cast: " + castList.join(", ");
						}
						if (tmdbData.crew && tmdbData.crew.length) {
							let director = document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") .movie-details .movie-details-cast .director");
							let directorList = [];
							for (let i in tmdbData.crew)
								if (tmdbData.crew[i].job === "Director")
									directorList.push(tmdbData.crew[i].name);
							director.innerHTML = "Director: " + directorList.join(", ");
						}
					});

				fetch("/api/v1/movies/" + data[i].id)
					.then(ratingResponse => ratingResponse.json())
					.then(ratingData => {
						let ratingStars = document.querySelectorAll("main section ul li:nth-child(" + (parseInt(i) + 1) + ") .rating .i");
						if (ratingData.rating >= 1) {
							document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") .rating").style.color = "#ff9800";
							document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") .rating").style.opacity = 1;
							for (let i = 0; i < 5; i++) {
								if (i + 1 <= Math.floor(ratingData.rating))
									ratingStars[i].innerHTML = "star";
								else if (i === Math.floor(ratingData.rating) && ratingData.rating - Math.floor(ratingData.rating) >= 0.5)
									ratingStars[i].innerHTML = "star_half";
								else ratingStars[i].innerHTML = "star_outline";
							}
						}
					});
			}
		});
}

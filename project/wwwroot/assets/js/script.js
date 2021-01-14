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

let fanartURL = "http://webservice.fanart.tv/v3/movies/";
let fanartKey = "68ec2de67dbfc2250512dc4cfa01ce18";

let cookie = getCookie("token");
let userId = cookie[0];
let token = cookie[1];

if(!userId) document.location.href = "/login.html";

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
	fetch("/api/v1/Users/" + userId + "/recommendations", {
		headers: {'Authorization': 'Bearer ' + token}
	})
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

					title.addEventListener("click", function() { showMoviePopup( data[0].id ); }, false);
					tmdbData.title ? (title.innerHTML = tmdbData.title) : (title.innerHTML = transformLocalTitle(data[0].title));
					tmdbData.release_date ? (year.innerHTML = "(" + tmdbData.release_date.substring(0, 4) + ")") : (year.innerHTML = "(" + data[0].title.slice(-5).slice(0, -1) + ")");
					tmdbData.overview ? (description.innerHTML = tmdbData.overview) : description.remove();
					tmdbData.spoken_languages && tmdbData.spoken_languages.length ? (language.innerHTML = tmdbData.spoken_languages[0].english_name ? tmdbData.spoken_languages[0].english_name : tmdbData.spoken_languages[0].name) : language.remove();
					genres !== "(no genres listed)" && genres ? (genres.innerHTML = data[0].genres.replaceAll("|", ", ")) : (genres.innerHTML = "<i>no genres listed</i>");
					tmdbData.runtime ? (duration.innerHTML = Math.floor(parseInt(tmdbData.runtime) / 60) + "h " + (parseInt(tmdbData.runtime) % 60) + "m") : duration.remove();

					let poster = document.querySelector("header img.poster");

					if (tmdbData.poster_path) {
						poster.src = "http://image.tmdb.org/t/p/w300" + tmdbData.poster_path;
						poster.addEventListener("click", function() { showMoviePopup( data[0].id ); }, false);
					} else {
						document.querySelector("header > .container").style.padding = "45px 0 175px 0";
						poster.remove();
					}

					if (tmdbData.backdrop_path) {
						let header = document.querySelector("header");
						document.querySelector("nav").classList.add("invert");
						document.querySelector("header > .container").style.color = "#ffffff";
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

			fetch("/api/v1/movies/" + data[0].id, {
				headers: {'Authorization': 'Bearer ' + token}
			})
				.then(ratingResponse => ratingResponse.json())
				.then(ratingData => {
					let ratingStars = document.querySelectorAll("header > .container .info .info-user .rating .i");
					if (ratingData.rating >= 1) {
						// document.querySelector("header > .container .info .info-user a.rating").style.color = "#ff9800";
						// document.querySelector("header > .container .info .info-user a.rating").style.opacity = 1;
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
	fetch("/api/v1/Users/" + userId + "/recommendations", {
		headers: {'Authorization': 'Bearer ' + token}
	})
		.then(response => response.json())
		.then(data => {
			for (let i = 0; i < 5; i++) {
				fetch(tmdbURL + "movie/" + data[i].tmdbId + "?api_key=" + tmdbKey + "&language=en-US")
					.then(tmdbResponse => tmdbResponse.json())
					.then(tmdbData => {
						let title = document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") h3");
						let poster = document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") img.poster");
						
						title.addEventListener("click", function() { showMoviePopup( data[i].id ); }, false);
						poster.addEventListener("click", function() { showMoviePopup( data[i].id ); }, false);

						tmdbData.title ? (title.innerHTML = tmdbData.title) : (title.innerHTML = transformLocalTitle(data[i].title));

						if (tmdbData.poster_path)	
							poster.src = "https://image.tmdb.org/t/p/w220_and_h330_face" + tmdbData.poster_path;
					});

				fetch("/api/v1/movies/" + data[i].id, {
					headers: {'Authorization': 'Bearer ' + token}
				})
					.then(ratingResponse => ratingResponse.json())
					.then(ratingData => {
						let ratingStars = document.querySelectorAll("main section ul li:nth-child(" + (parseInt(i) + 1) + ") .rating .i");
						if (ratingData.rating >= 1) {
							// document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") .rating").style.color = "#ff9800";
							// document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") .rating").style.opacity = 1;
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

	fetch("/api/v1/movies/recent-releases?page=1&pageSize=5", {
		headers: {'Authorization': 'Bearer ' + token}
	})
		.then(response => response.json())
		.then(data => {
			for (let i in data) {
				fetch(tmdbURL + "movie/" + data[i].tmdbId + "?api_key=" + tmdbKey + "&language=en-US")
					.then(tmdbResponse => tmdbResponse.json())
					.then(tmdbData => {
						let title = document.querySelector("main section:nth-of-type(2) ul li:nth-child(" + (parseInt(i) + 1) + ") h3");
						let year = document.querySelector("main section:nth-of-type(2) ul li:nth-child(" + (parseInt(i) + 1) + ") .date");
						let poster = document.querySelector("main section:nth-of-type(2) ul li:nth-child(" + (parseInt(i) + 1) + ") img.poster");
												
						title.addEventListener("click", function() { showMoviePopup( data[i].id ); }, false);
						poster.addEventListener("click", function() { showMoviePopup( data[i].id ); }, false);

						tmdbData.title ? (title.innerHTML = tmdbData.title) : (title.innerHTML = transformLocalTitle(data[i].title));
						tmdbData.release_date ? (year.innerHTML = tmdbData.release_date.substring(0, 4)) : (year.innerHTML = data[0].title.slice(-5).slice(0, -1));

						if (tmdbData.poster_path)
							poster.src = "https://image.tmdb.org/t/p/w220_and_h330_face" + tmdbData.poster_path;
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
		fetch("/api/v1/friendships/" + userId, {
			headers: {'Authorization': 'Bearer ' + token}
		})
			.then(friendsListResponse => friendsListResponse.json())
			.then(friendsListData => {
				let friendsList = document.querySelector(".page header > .container ul.friendslist");

				for(let i in friendsListData) {
					let friendId;
					friendsListData[i].userId_1 != userId ? friendId = friendsListData[i].userId_1 : friendId = friendsListData[i].userId_2;
					
					fetch("/api/v1/users/" + friendId, {
						headers: {'Authorization': 'Bearer ' + token}
					})
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
	fetch(endpoint, {
		headers: {'Authorization': 'Bearer ' + token}
	})
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
						let poster = document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") img.poster");
												
						title.addEventListener("click", function() { showMoviePopup( data[i].id ); }, false);
						poster.addEventListener("click", function() { showMoviePopup( data[i].id ); }, false);

						tmdbData.title ? (title.innerHTML = tmdbData.title) : (title.innerHTML = transformLocalTitle(data[i].title));
						tmdbData.release_date ? (year.innerHTML = "(" + tmdbData.release_date.substring(0, 4) + ")") : (year.innerHTML = "(" + data[i].title.slice(-5).slice(0, -1) + ")");
						tmdbData.overview ? (description.innerHTML = tmdbData.overview) : description.remove();
						tmdbData.spoken_languages && tmdbData.spoken_languages.length ? (language.innerHTML = tmdbData.spoken_languages[0].english_name ? tmdbData.spoken_languages[0].english_name : tmdbData.spoken_languages[0].name) : language.remove();
						genres !== "(no genres listed)" && genres ? (genres.innerHTML = data[i].genres.replaceAll("|", ", ")) : (genres.innerHTML = "<i>no genres listed</i>");
						tmdbData.runtime ? (duration.innerHTML = Math.floor(parseInt(tmdbData.runtime) / 60) + "h " + (parseInt(tmdbData.runtime) % 60) + "m") : duration.remove();

						if (tmdbData.poster_path)
							poster.src = "https://image.tmdb.org/t/p/w220_and_h330_face" + tmdbData.poster_path;
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

				fetch("/api/v1/movies/" + data[i].id, {
					headers: {'Authorization': 'Bearer ' + token}
				})
					.then(ratingResponse => ratingResponse.json())
					.then(ratingData => {
						let ratingStars = document.querySelectorAll("main section ul li:nth-child(" + (parseInt(i) + 1) + ") .rating .i");
						if (ratingData.rating >= 1) {
							// document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") .rating").style.color = "#ff9800";
							// document.querySelector("main section ul li:nth-child(" + (parseInt(i) + 1) + ") .rating").style.opacity = 1;
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

function showMoviePopup(movieId) {
	document.body.classList.add("popup-open");

	let popup = document.createElement("div");
	document.body.prepend(popup);
	popup.classList.add("popup");

	let popupContainer = document.createElement("div");
	popup.prepend(popupContainer);
	popupContainer.classList.add("container");

	let popupControls = document.createElement("div");
	popupContainer.prepend(popupControls);
	popupControls.classList.add("popup-controls");

	let popupClose = document.createElement("i");
	popupControls.prepend(popupClose);
	popupClose.classList.add("i");
	popupClose.insertAdjacentText("afterbegin", "close");

	let popupContent = document.createElement("div");
	popupContainer.append(popupContent);
	popupContent.classList.add("popup-content");
	popupContent.classList.add("loading");

	popupContent.insertAdjacentHTML("beforeend", `
		<div class="loading-icon"></div>
		<div class="popup-content-header">
			<h1 class="title"></h1>
			<div class="short-stats">
				<span class="release-date"></span>
				<span class="language"></span>
				<span class="genres"></span>
				<span class="duration"></span>
			</div>
			<div class="tagline"></div>
			<div class="description"></div>
			<div class="short-cast">
				<div class="cast"></div>
				<div class="director"></div>
			</div>
			<a class="watch" href="" rel="noopener noreferrer nofollow" target="_blank">
			</a>
		</div>
		<div class="popup-content-cast">
			<h2></h2>
			<div class="cast">
			</div>
		</div>
		<div class="popup-content-details">
			<div class="keywords">
				<span class="label"></span>
				<div class="info"></div>
			</div>
			<div class="original-title">
				<span class="label"></span>
				<div class="info"></div>
			</div>
			<div class="budget">
				<span class="label"></span>
				<div class="info"></div>
			</div>
			<div class="revenue">
				<span class="label"></span>
				<div class="info"></div>
			</div>
			<div class="links">
				<span class="label"></span>
				<div class="info"></div>
			</div>
		</div>
		<div class="popup-content-videos">
			<h2></h2>
			<div class="videos"></div>
		</div>
		<div class="popup-content-images">
			<h2></h2>
			<div class="images"></div>
		</div>
	`);

	popupClose.addEventListener("click", () => {
		popup.remove();
		document.body.classList.remove("popup-open");
	});

	fetch("/api/v1/movies/" + movieId, {
		headers: {'Authorization': 'Bearer ' + token}
	})
		.then(response => response.json())
		.then(data => {
			let title = transformLocalTitle(data.title);
			let releaseDate = data.title.slice(-5).slice(0, -1);
			let genres = data.genres.replaceAll("|", ", ").replaceAll("(", "").replaceAll(")", "");
			let rating = data.rating;
			let numberOfRatings = data.numberOfRatings;

			let imdbId;
			if(data.imdbId)
				imdbId = data.imdbId.toString().padStart(7, 0).padStart(9, "t");

			let hasLogo = false, logo;

			Promise.all([
				fetch(fanartURL + imdbId + "?api_key=" + fanartKey),
				fetch(tmdbURL + "movie/" + data.tmdbId + "?api_key=" + tmdbKey + "&language=en-US&append_to_response=credits,external_ids,keywords,watch/providers,videos,images&include_image_language=en,null")
			]).then(responses => {
				return Promise.all(responses.map(promiseResponse => promiseResponse.json()))
			}).then(promiseData => {
				let fanartData = promiseData[0];
				let tmdbData = promiseData[1];

				/* .popup-content-header */
				if(fanartData.status !== "error" && fanartData.hdmovielogo) {
					for(let i = 0; i < fanartData.hdmovielogo.length && !hasLogo; i++) {
						if(fanartData.hdmovielogo[i].lang === "en") {
							hasLogo = true;
							logo = fanartData.hdmovielogo[i].url;
						}
					}
				}

				if(!tmdbData.backdrop_path)
					popupContent.querySelector(".popup-content-header").classList.add("no-backdrop");
				else
					popupContent.querySelector(".popup-content-header").style.backgroundImage = "linear-gradient(to right, rgba(0, 0, 0, 0.85), rgba(0, 0, 0, 0.2)), linear-gradient(to left, rgba(0, 0, 0, 0.2), rgba(0, 0, 0, 0.8)), url(https://image.tmdb.org/t/p/original" + tmdbData.backdrop_path + ")";

				if(tmdbData.title) title = tmdbData.title;
				popupContent.querySelector(".popup-content-header h1.title").innerHTML = title;

				if(hasLogo)
					popupContent.querySelector(".popup-content-header").insertAdjacentHTML("afterbegin", `
						<img class="logo" src="` + logo + `" alt="` + title + `" />
					`);
				else
					popupContent.querySelector(".popup-content-header").classList.add("no-logo");

				if(tmdbData.release_date) releaseDate = Intl.DateTimeFormat('ro-RO').format(Date.parse(tmdbData.release_date));
				popupContent.querySelector(".popup-content-header .short-stats .release-date").innerHTML = releaseDate;

				if(tmdbData.spoken_languages && tmdbData.spoken_languages.length) {
					if(tmdbData.spoken_languages[0].english_name)
						popupContent.querySelector(".popup-content-header .short-stats .language").innerHTML = tmdbData.spoken_languages[0].english_name;
					else
						popupContent.querySelector(".popup-content-header .short-stats .language").innerHTML = tmdbData.spoken_languages[0].name;
				} else {
					popupContent.querySelector(".popup-content-header .short-stats .language").remove();
				}

				popupContent.querySelector(".popup-content-header .short-stats .genres").innerHTML = genres;

				if(tmdbData.runtime)
					popupContent.querySelector(".popup-content-header .short-stats .duration").innerHTML = Math.floor(parseInt(tmdbData.runtime) / 60) + "h " + (parseInt(tmdbData.runtime) % 60) + "m"; 
				else
					popupContent.querySelector(".popup-content-header .short-stats .duration").remove();
					
				if(tmdbData.tagline) popupContent.querySelector(".popup-content-header .tagline").innerHTML = tmdbData.tagline;
				else popupContent.querySelector(".popup-content-header .tagline").remove();

				if(tmdbData.overview) popupContent.querySelector(".popup-content-header .description").innerHTML = tmdbData.overview;
				else popupContent.querySelector(".popup-content-header .description").remove();

				if(!tmdbData.credits || (!tmdbData.credits.cast.length && !tmdbData.credits.crew.length))
					popupContent.querySelector(".popup-content-header .short-cast").remove();
				else {
					if (tmdbData.credits.cast && tmdbData.credits.cast.length) {
						let castList = [];
						for (let i = 0; i < tmdbData.credits.cast.length && i < 3; i++)
							castList.push(tmdbData.credits.cast[i].name);
						popupContent.querySelector(".popup-content-header .short-cast .cast").innerHTML = "Cast: " + castList.join(", ");
						if(!castList.length) popupContent.querySelector(".popup-content-header .short-cast .cast").remove();
					}
					if (tmdbData.credits.crew && tmdbData.credits.crew.length) {
						let directorList = [];
						for (let i in tmdbData.credits.crew)
							if (tmdbData.credits.crew[i].job === "Director")
								directorList.push(tmdbData.credits.crew[i].name);
						popupContent.querySelector(".popup-content-header .short-cast .director").innerHTML = "Director: " + directorList.join(", ");
						if(!directorList.length) popupContent.querySelector(".popup-content-header .short-cast .director").remove();
					}
				}
				if(tmdbData["watch/providers"] && tmdbData["watch/providers"].results.RO) {
					popupContent.querySelector(".popup-content-header a.watch").href = tmdbData["watch/providers"].results.RO.link;
					for(let i in tmdbData["watch/providers"].results.RO.flatrate) {
						popupContent.querySelector(".popup-content-header a.watch").insertAdjacentHTML("beforeend", `
							<img src="https://www.themoviedb.org/t/p/original` + tmdbData["watch/providers"].results.RO.flatrate[i].logo_path + `" alt="` + tmdbData["watch/providers"].results.RO.flatrate[i].provider_name + `" />
						`)
					}
				}
				else popupContent.querySelector(".popup-content-header a.watch").remove();

				/* .popup-content-rating */
				// se implementeaza la final

				/* .popup-content-cast */
				if(!tmdbData.credits || !tmdbData.credits.cast.length)
					popupContent.querySelector(".popup-content-cast").remove();
				else {
					popupContent.querySelector(".popup-content-cast h2").innerHTML = "Top Billed Cast";
					for (let i = 0; i < tmdbData.credits.cast.length && i < 7; i++) {
						let actorPicture = "assets/img/placeholder.svg";
						if(tmdbData.credits.cast[i].profile_path)
							actorPicture = "https://www.themoviedb.org/t/p/w138_and_h175_face/" + tmdbData.credits.cast[i].profile_path;
						popupContent.querySelector(".popup-content-cast .cast").insertAdjacentHTML("beforeend", `
							<div class="actor">
								<img src="` + actorPicture + `" alt="` + tmdbData.credits.cast[i].name + `" />
								<div class="name">` + tmdbData.credits.cast[i].name + `</div>
								<div class="character">` + tmdbData.credits.cast[i].character + `</div>
							</div>
						`);
					}
				}
				
				/* popup-content-details */
				if(!tmdbData.keywords || (!tmdbData.keywords.keywords.length && !tmdbData.original_title && !tmdbData.budget && !tmdbData.revenue && !tmdbData.homepage && !imdbId && !tmdbData.external_ids.facebook_id && !tmdbData.external_ids.instagram_id && !tmdbData.external_ids.twitter_id))
					popupContent.querySelector(".popup-content-details").remove();
				else {
					if(!tmdbData.keywords.keywords.length)
						popupContent.querySelector(".popup-content-details .keywords").remove();
					else {
						popupContent.querySelector(".popup-content-details .keywords .label").innerHTML = "Keywords";
						popupContent.querySelector(".popup-content-details .keywords .info").innerHTML = tmdbData.keywords.keywords.map(keyword => keyword.name).join(", ");
					}

					if(!tmdbData.original_title)
						popupContent.querySelector(".popup-content-details .original-title").remove();
					else {
						popupContent.querySelector(".popup-content-details .original-title .label").innerHTML = "Original Title";
						popupContent.querySelector(".popup-content-details .original-title .info").innerHTML = tmdbData.original_title;
					}

					if(!tmdbData.budget)
						popupContent.querySelector(".popup-content-details .budget").remove();
					else {
						popupContent.querySelector(".popup-content-details .budget .label").innerHTML = "Budget";
						popupContent.querySelector(".popup-content-details .budget .info").innerHTML = new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(tmdbData.budget);
					}

					if(!tmdbData.revenue)
						popupContent.querySelector(".popup-content-details .revenue").remove();
					else {
						popupContent.querySelector(".popup-content-details .revenue .label").innerHTML = "Revenue";
						popupContent.querySelector(".popup-content-details .revenue .info").innerHTML = new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(tmdbData.revenue);
					}

					if(!tmdbData.homepage && !imdbId && !tmdbData.external_ids.facebook_id && !tmdbData.external_ids.instagram_id && !tmdbData.external_ids.twitter_id)
						popupContent.querySelector(".popup-content-details .links").remove();
					else {
						popupContent.querySelector(".popup-content-details .links .label").innerHTML = "Links";
						let links = popupContent.querySelector(".popup-content-details .links .info");
						if(tmdbData.homepage)
							links.insertAdjacentHTML("beforeend", `
								<a class="link homepage" href="` + tmdbData.homepage + `" rel="noopener noreferrer nofollow" target="_blank">
									<i class="i">public</i>
								</a>
							`);
						if(imdbId)
							links.insertAdjacentHTML("beforeend", `
								<a class="link imdb" href="https://www.imdb.com/title/` + imdbId + `" rel="noopener noreferrer nofollow" target="_blank">
									<img src="assets/img/imdb.svg" alt="IMDb page" />
								</a>
							`);
						if(tmdbData.external_ids.facebook_id)
							links.insertAdjacentHTML("beforeend", `
								<a class="link facebook" href="https://www.facebook.com/` + tmdbData.external_ids.facebook_id + `" rel="noopener noreferrer nofollow" target="_blank">
									<img src="assets/img/facebook.svg" alt="Facebook page" />
								</a>
							`);
						if(tmdbData.external_ids.instagram_id)
							links.insertAdjacentHTML("beforeend", `
								<a class="link instagram" href="https://www.instagram.com/` + tmdbData.external_ids.instagram_id + `" rel="noopener noreferrer nofollow" target="_blank">
									<img src="assets/img/instagram.svg" alt="Instagram page" />
								</a>
							`);
						if(tmdbData.external_ids.twitter_id)
							links.insertAdjacentHTML("beforeend", `
								<a class="link twitter" href="https://twitter.com/` + tmdbData.external_ids.twitter_id + `" rel="noopener noreferrer nofollow" target="_blank">
									<img src="assets/img/twitter.svg" alt="Twitter page" />
								</a>
							`);
					}
				}

				/* .popup-content-videos */
				if(!tmdbData.videos || !tmdbData.videos.results.length)
					popupContent.querySelector(".popup-content-videos").remove();
				else {
					popupContent.querySelector(".popup-content-videos h2").innerHTML = "Videos";
					for(let i in tmdbData.videos.results)
						popupContent.querySelector(".popup-content-videos .videos").insertAdjacentHTML("beforeend", `
							<a style="background-image: radial-gradient(rgba(0, 0, 0, 0.85), rgba(0, 0, 0, 0.15)), url('https://i.ytimg.com/vi/` + tmdbData.videos.results[i].key + `/hqdefault.jpg');" class="video" href="https://www.youtube.com/watch?v=` + tmdbData.videos.results[i].key + `" rel="noopener noreferrer nofollow" target="_blank">
								<i class="i">play_arrow</i>
							</a>
						`);
				}

				/* .popup-content-images */
				let captures = [];
				if(tmdbData.images && tmdbData.images.backdrops.length) {
					for (let i in tmdbData.images.backdrops)
						if (!tmdbData.images.backdrops[i].iso_639_1)
							captures.push(tmdbData.images.backdrops[i].file_path);
				}

				if(!tmdbData.poster_path && !captures.length)
					popupContent.querySelector(".popup-content-images").remove();
				else {
					popupContent.querySelector(".popup-content-images h2").innerHTML = "Photos";
					if(tmdbData.poster_path)
						popupContent.querySelector(".popup-content-images .images").insertAdjacentHTML("beforeend", `
							<a class="poster-link" href="https://www.themoviedb.org/t/p/original` + tmdbData.poster_path + `" target="_blank">
								<img class="poster-image" src="https://www.themoviedb.org/t/p/w220_and_h330_face` + tmdbData.poster_path + `" alt="movie poster" />
							</a>
						`);
					for(let i in captures)
						popupContent.querySelector(".popup-content-images .images").insertAdjacentHTML("beforeend", `
							<a class="capture-link" href="https://www.themoviedb.org/t/p/original` + captures[i] + `" target="_blank">
								<img class="capture-image" src="https://www.themoviedb.org/t/p/w533_and_h300_bestv2` + captures[i] + `" alt="movie capture" />
							</a>
						`);
				}

				/* loaded */
				popupContent.querySelector(".loading-icon").remove();
				popupContent.classList.remove("loading");
			});
		});
}

function getCookie(cname) {
	var name = cname + "=";
	var ca = document.cookie.split(';');
	for (var i = 0; i < ca.length; i++) {
		var c = ca[i];
		while (c.charAt(0) == ' ') {
			c = c.substring(1);
		}
		if (c.indexOf(name) == 0) {
			spilt_cokie = c.substring(name.length, c.length).split(' ');
			return [spilt_cokie[0], spilt_cokie[1]];
		}
	}
	return "";
}

function logout() {
	document.cookie = "token= ; expires = Thu, 01 Jan 1970 00:00:00 GMT";
	document.location.href = "/login.html";
}
console.log("cokie: " + document.cookie)
cokie = getCookie("token")
console.log("id user: " + cokie[0])
console.log("token: " + cokie[1])


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

function setCookie(cookieName, cookieValue) {
    var d = new Date();
    d.setTime(d.getTime() + (24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cookieName + "=" + cookieValue + ";" + expires + ";path=/";
}

function login_user() {
    var userName = document.getElementById("username").value;
    var password = document.getElementById("password").value;
    var error_login = document.querySelector("main .error");
    result = userName + " " + password;
    let _user = {
        username: userName,
        password: password
    }

    fetch("/api/v1/Users/login", {
        method: "POST",
        body: JSON.stringify(_user), headers: { "Content-type": "application/json; charset=UTF-8" }
    })
        .then(response => response.json())
        .then(data => {
            if (data.message == "Username or password is incorrect") {
                error_login.textContent = "Incorrect username or password.";
            }
            else
            {
                var cookie_value = data.id + " " + data.token;
                setCookie("token", cookie_value);
                document.location.href = "/";
            }
        });
}


function register_user() {
    var userName = document.getElementById("username").value;
    var password = document.getElementById("password").value;

    // var error_name = document.getElementById("error_username");
    //var error_password = document.getElementById("error_password");
    var error_password = "";
    var error_signup = document.querySelector("main .error_signup");
    var passw = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20}$/;
    let _user = {
        username: userName,
        password: password
    }

    fetch("/api/v1/Users/authenticate", {
        method: "POST",
        body: JSON.stringify(_user), headers: { "Content-type": "application/json; charset=UTF-8" }
    })
        .then(response => response.json())
        .then(data => {
            if (data.message == "username exist") {
                error_signup.textContent = "Username used";
                //error_name.style.color = "red"
            }
            else if (data.message == "Failed") {
                error_signup.textContent = "Complete all fields";
                //error_name.style.color = "red"
            }
            else {
                error_signup.textContent = "";
            }

            if (!password.match(passw)) {
                error_signup.textContent = "password between 6 to 20 characters, one numeric digit, one uppercase and one lowercase letter ";
                error_password = "password between 6 to 20 characters, one numeric digit, one uppercase and one lowercase letter ";
                //error_password.style.color = "red"
            }
            else {
                error_password = "";
            }

            console.log(error_password + " " + error_signup);
            if (error_signup.textContent == "" && error_password == "") {


                fetch("/api/v1/Users", {
                    method: "POST",
                    body: JSON.stringify(_user), headers: { "Content-type": "application/json; charset=UTF-8" }
                })
                    .then(response => response.json())
                    .then(json => {
                        console.log(json);
                        var cokie_value = json.id + " " + data.token;
                        setCookie("token", cokie_value);
                        document.location.href = "/";
                    });

            }
        });
    //document.getElementById("display").innerHTML = result;
}




function showPassword() {

    var x = document.getElementById("password");
    if (x.type === "password") {
        x.type = "text";
    } else {
        x.type = "password";
    }
}
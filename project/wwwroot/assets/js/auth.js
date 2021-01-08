token = getCookie("token")
console.log("token: " + token)

//token dupa login
/*if (document.cookie != "") {
    alert("Welcome again ");
    fetch('/api/v1/Users/1', {
        headers: { 'Authorization': 'Bearer ' + token }
    })
        .then(response => response.json()) 
        .then(json => {   
            document.getElementById("display").innerHTML = json.username;
        });
} else {
    alert("Login");
}*/


function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
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
            else {
                error_login.textContent = ""
            }

            if (error_login.textContent == "") {

                var d = new Date();
                d.setTime(d.getTime() + (2 * 60 * 1000));
                var expires = "expires=" + d.toUTCString();
                document.cookie = "token" + "=" + data.token + ";" + expires + ";path=/";
                document.location.href = "/";
            }
        });
}


function register_user() {
    var userName = document.getElementById("username").value;
    var password = document.getElementById("password").value;
    var error_name = document.querySelector("main .error"); 
    var error_password = document.querySelector("main .error"); 
    var passw = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20}$/;
    result = userName + " " + password;
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
                    error_name.textContent = "This username is already taken."
                }
                else if (data.message == "Failed")
                {
                    error_name.textContent = "You must full in all the fields."
                }
                else
                {
                    error_name.textContent = ""
                }

                if (password.match(passw)) {
                    error_password.textContent = ""
                }
                else {
                    error_password.textContent = "Your password must be between 6 and 20 characters and must contain at least one numeric digit, one uppercase and one lowercase letter."
                }

                if (error_name.textContent == "" && error_password.textContent == "") {

                    let _data = {
                        username: userName,
                        password: password
                    }
                    fetch("/api/v1/Users", {
                        method: "POST",
                        body: JSON.stringify(_data), headers: { "Content-type": "application/json; charset=UTF-8" }
                    })
                        .then(response => response.json())
                        .then(json => console.log(json));

                    var d = new Date();
                    d.setTime(d.getTime() + (2 * 60 * 1000));
                    var expires = "expires=" + d.toUTCString();
                    document.cookie = "token" + "=" + data.token + ";" + expires + ";path=/";
                    document.location.href = "/";
                }
            });
}
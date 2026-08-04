const loginForm = document.getElementById("login-form");
const loginMessage = document.getElementById("login-message");
const loginButton = document.getElementById("login-button");

loginForm.addEventListener("submit", async (event) => {
    event.preventDefault();

    loginMessage.textContent = "";
    loginMessage.className = "message";
    loginButton.disabled = true;

    const username = document.getElementById("username").value.trim();
    const password = document.getElementById("password").value;

    try {
        const response = await fetch("/api/Auth/login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ username, password })
        });

        const result = await response.json();

        if (!response.ok) {
            loginMessage.textContent =
                result.message ?? "No fue posible iniciar sesión.";

            loginMessage.className = "message error";
            return;
        }

        sessionStorage.setItem("accessToken", result.accessToken);
        sessionStorage.setItem("username", result.username);

        window.location.href = "/payments.html";
    } catch {
        loginMessage.textContent =
            "No fue posible establecer conexión con el servidor.";

        loginMessage.className = "message error";
    } finally {
        loginButton.disabled = false;
    }
});
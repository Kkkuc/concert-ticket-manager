function clearErrors(form)
{
    form.querySelectorAll(".invalid").forEach(function (input) {
        input.classList.remove("invalid");
    });

    form.querySelectorAll(".field-error").forEach(function (error) {
        error.remove();
    });
}

function showError(input, message)
{
    input.classList.add("invalid");

    const error = document.createElement("small");
    error.className = "field-error";
    error.textContent = message;

    input.after(error);
}

async function registerUser(event, form)
{
    event.preventDefault();
    clearErrors(form);
    try
    {
        const response = await fetch(form.action, {
            method: "POST",
            body: new FormData(form)
        });

        if (response.status === 422)
        {
            const errors = await response.json();

            for (const [field, message] of Object.entries(errors))
            {
                const input = form.elements.namedItem(field);

                if (input) {
                    showError(input, message);
                }
            }
            return;
        }

        if (response.ok) {
            window.location.href = "http://localhost:5000";
        }

        else {
            alert("Błąd serwera: " + response.status);
        }
    }
    catch (error) {
        console.error(error);
        alert("Nie udało się połączyć z serwerem.");
    }
}

async function loginUser(event, form)
{
    event.preventDefault();
    clearErrors(form);
    try
    {
        const response = await fetch(form.action, {
            method: "POST",
            body: new FormData(form)
        });

        if (response.status === 401)
        {
            const result = await response.json();
            const input = form.elements.namedItem("password");

            if (input) {
                showError(input, result.error);
            }
            return;
        }

        if (response.ok) {
            window.location.href = "http://localhost:5000";
        }

        else {
            alert("Błąd serwera: " + response.status);
        }
    }

    catch (error) {
        console.error(error);
        alert("Nie udało się połączyć z serwerem.");
    }
}

const token = sessionStorage.getItem("accessToken");

if (!token) {
    window.location.href = "/index.html";
}

const paymentForm = document.getElementById("payment-form");
const paymentIdInput = document.getElementById("payment-id");
const sourceAccountInput = document.getElementById("source-account-id");
const destinationAccountInput =
    document.getElementById("destination-account-id");
const amountInput = document.getElementById("amount");
const currencyInput = document.getElementById("currency");
const descriptionInput = document.getElementById("description");
const paymentMessage = document.getElementById("payment-message");
const paymentsBody = document.getElementById("payments-body");
const emptyMessage = document.getElementById("empty-message");
const formTitle = document.getElementById("form-title");
const saveButton = document.getElementById("save-button");
const cancelButton = document.getElementById("cancel-button");
const refreshButton = document.getElementById("refresh-button");
const logoutButton = document.getElementById("logout-button");

let payments = [];

let messageTimer;

paymentForm.addEventListener("submit", savePayment);
cancelButton.addEventListener("click", resetForm);
//refreshButton.addEventListener("click", loadPayments);
refreshButton.addEventListener("click", () => {
    clearMessage();
    loadPayments();
});
logoutButton.addEventListener("click", logout);

async function loadPayments() {
    //clearMessage();

    try {
        const response = await fetch("/api/Payments");

        if (!response.ok) {
            throw new Error(await getErrorMessage(response));
        }

        payments = await response.json();
        renderPayments();
    } catch (error) {
        showMessage(error.message, "error");
    }
}

async function savePayment(event) {
    event.preventDefault();
    clearMessage();

    const paymentId = paymentIdInput.value;
    const isEditing = paymentId !== "";

    const request = isEditing
        ? {
            amount: Number(amountInput.value),
            currency: currencyInput.value.trim().toUpperCase(),
            description: descriptionInput.value.trim()
        }
        : {
            sourceAccountId: sourceAccountInput.value.trim(),
            destinationAccountId: destinationAccountInput.value.trim(),
            amount: Number(amountInput.value),
            currency: currencyInput.value.trim().toUpperCase(),
            description: descriptionInput.value.trim()
        };

    const url = isEditing
        ? `/api/Payments/${paymentId}`
        : "/api/Payments";

    const method = isEditing ? "PUT" : "POST";

    try {
        saveButton.disabled = true;

        const response = await fetch(url, {
            method,
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(request)
        });

        if (!response.ok) {
            throw new Error(await getErrorMessage(response));
        }

        resetForm();

        showMessage(
            isEditing
                ? "El pago fue actualizado correctamente."
                : "El pago fue registrado correctamente.",
            "success"
        );

        await loadPayments();
    } catch (error) {
        showMessage(error.message, "error");
    } finally {
        saveButton.disabled = false;
    }
}

function renderPayments() {
    paymentsBody.replaceChildren();
    emptyMessage.classList.toggle("hidden", payments.length > 0);

    for (const payment of payments) {
        const row = document.createElement("tr");
        row.dataset.paymentId = payment.id;

        appendCell(row, shortenId(payment.sourceAccountId));
        appendCell(row, shortenId(payment.destinationAccountId));
        appendCell(row, formatAmount(payment.amount));
        appendCell(row, payment.currency);
        appendCell(row, payment.description ?? "");
        appendCell(row, getStatusName(payment.status));

        const actionsCell = document.createElement("td");
        const actionsContainer = document.createElement("div");
        actionsContainer.className = "action-buttons";

        const editButton = document.createElement("button");
        editButton.type = "button";
        editButton.className = "edit-button";
        editButton.textContent = "Editar";
        editButton.dataset.testid = `edit-${payment.id}`;
        editButton.addEventListener("click", () => editPayment(payment));

        const deleteButton = document.createElement("button");
        deleteButton.type = "button";
        deleteButton.className = "delete-button";
        deleteButton.textContent = "Eliminar";
        deleteButton.dataset.testid = `delete-${payment.id}`;
        deleteButton.addEventListener(
            "click",
            () => deletePayment(payment.id)
        );

        actionsContainer.append(editButton, deleteButton);
        actionsCell.appendChild(actionsContainer);
        row.appendChild(actionsCell);
        paymentsBody.appendChild(row);
    }
}

function editPayment(payment) {
    paymentIdInput.value = payment.id;
    sourceAccountInput.value = payment.sourceAccountId;
    destinationAccountInput.value = payment.destinationAccountId;
    amountInput.value = payment.amount;
    currencyInput.value = payment.currency;
    descriptionInput.value = payment.description ?? "";

    sourceAccountInput.disabled = true;
    destinationAccountInput.disabled = true;

    formTitle.textContent = "Actualizar pago";
    saveButton.textContent = "Guardar cambios";
    cancelButton.classList.remove("hidden");

    window.scrollTo({ top: 0, behavior: "smooth" });
}

async function deletePayment(paymentId) {
    const confirmed = window.confirm(
        "¿Está seguro de que desea eliminar este pago?"
    );

    if (!confirmed) {
        return;
    }

    clearMessage();

    try {
        const response = await fetch(`/api/Payments/${paymentId}`, {
            method: "DELETE"
        });

        if (!response.ok) {
            throw new Error(await getErrorMessage(response));
        }

        showMessage("El pago fue eliminado correctamente.", "success");
        resetForm();
        await loadPayments();
    } catch (error) {
        showMessage(error.message, "error");
    }
}

function resetForm() {
    paymentForm.reset();
    paymentIdInput.value = "";
    currencyInput.value = "";

    sourceAccountInput.disabled = false;
    destinationAccountInput.disabled = false;

    formTitle.textContent = "Registrar pago";
    saveButton.textContent = "Guardar pago";
    cancelButton.classList.add("hidden");
}

function logout() {
    sessionStorage.clear();
    window.location.href = "/index.html";
}

function appendCell(row, value) {
    const cell = document.createElement("td");
    cell.textContent = value;
    row.appendChild(cell);
}

function shortenId(id) {
    return id ? `${id.substring(0, 8)}...` : "";
}

function formatAmount(amount) {
    return Number(amount).toLocaleString("es-DO", {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
}

function getStatusName(status) {
    const statuses = {
        0: "Pendiente",
        1: "Completado",
        2: "Rechazado"
    };

    return statuses[status] ?? String(status);
}

async function getErrorMessage(response) {
    try {
        const result = await response.json();

        if (result.message) {
            return result.message;
        }

        if (result.errors) {
            return Object.values(result.errors).flat().join(" ");
        }

        return result.title ?? "La operación no pudo completarse.";
    } catch {
        return "La operación no pudo completarse.";
    }
}

function showMessage(text, type) {
    clearTimeout(messageTimer);

    paymentMessage.textContent = text;
    paymentMessage.className = `message ${type}`;

    messageTimer = setTimeout(() => {
        paymentMessage.textContent = "";
        paymentMessage.className = "message";
    }, 4000);
}

function clearMessage() {
    clearTimeout(messageTimer);

    paymentMessage.textContent = "";
    paymentMessage.className = "message";
}

loadPayments();
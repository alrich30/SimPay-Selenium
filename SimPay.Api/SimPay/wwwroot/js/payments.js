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

const filterForm = document.getElementById("filter-form");
const filterStatus = document.getElementById("filter-status");
const filterCurrency = document.getElementById("filter-currency");
const filterMinAmount = document.getElementById("filter-min-amount");
const filterMaxAmount = document.getElementById("filter-max-amount");
const filterFromDate = document.getElementById("filter-from-date");
const filterToDate = document.getElementById("filter-to-date");
const filterMessage = document.getElementById("filter-message");
const clearFiltersButton = document.getElementById("clear-filters-button");

const totalPayments = document.getElementById("total-payments");
const pendingPayments = document.getElementById("pending-payments");
const completedPayments = document.getElementById("completed-payments");
const rejectedPayments = document.getElementById("rejected-payments");

const exportCsvButton = document.getElementById("export-csv-button");

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
filterForm.addEventListener("submit", applyFilters);
clearFiltersButton.addEventListener("click", clearFilters);
exportCsvButton.addEventListener("click", exportPaymentsToCsv);

async function loadPayments() {
    clearFilterMessage();

    try {
        const response = await fetch(buildPaymentsUrl());

        if (!response.ok) {
            throw new Error(await getErrorMessage(response));
        }

        payments = await response.json();
        renderPayments();
    } catch (error) {
        showFilterMessage(error.message, "error");
    }
}

function buildPaymentsUrl() {
    const parameters = new URLSearchParams();

    if (filterStatus.value) {
        parameters.set("status", filterStatus.value);
    }

    if (filterCurrency.value.trim()) {
        parameters.set(
            "currency",
            filterCurrency.value.trim().toUpperCase()
        );
    }

    if (filterMinAmount.value) {
        parameters.set("minAmount", filterMinAmount.value);
    }

    if (filterMaxAmount.value) {
        parameters.set("maxAmount", filterMaxAmount.value);
    }

    if (filterFromDate.value) {
        parameters.set("fromDate", filterFromDate.value);
    }

    if (filterToDate.value) {
        parameters.set("toDate", filterToDate.value);
    }

    const queryString = parameters.toString();

    return queryString
        ? `/api/Payments?${queryString}`
        : "/api/Payments";
}

function applyFilters(event) {
    event.preventDefault();

    const minAmount = Number(filterMinAmount.value);
    const maxAmount = Number(filterMaxAmount.value);

    if (filterMinAmount.value &&
        filterMaxAmount.value &&
        minAmount > maxAmount) {
        showFilterMessage(
            "El monto mínimo no puede superar el monto máximo.",
            "error"
        );

        return;
    }

    if (filterFromDate.value &&
        filterToDate.value &&
        filterFromDate.value > filterToDate.value) {
        showFilterMessage(
            "La fecha inicial no puede ser posterior a la fecha final.",
            "error"
        );

        return;
    }

    loadPayments();
}

function clearFilters() {
    filterForm.reset();
    clearFilterMessage();
    loadPayments();
}

function showFilterMessage(text, type) {
    filterMessage.textContent = text;
    filterMessage.className = `message ${type}`;
}

function clearFilterMessage() {
    filterMessage.textContent = "";
    filterMessage.className = "message";
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

    updateStatistics();
    exportCsvButton.disabled = payments.length === 0;

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

function exportPaymentsToCsv() {
    if (payments.length === 0) {
        showFilterMessage(
            "No existen pagos para exportar.",
            "error"
        );

        return;
    }

    const headers = [
        "Id",
        "Cuenta de origen",
        "Cuenta de destino",
        "Monto",
        "Moneda",
        "Descripción",
        "Estado",
        "Fecha de creación"
    ];

    const rows = payments.map(payment => [
        payment.id,
        payment.sourceAccountId,
        payment.destinationAccountId,
        payment.amount,
        payment.currency,
        payment.description ?? "",
        getStatusName(payment.status),
        payment.createdAtUtc
    ]);

    const csvContent = [headers, ...rows]
        .map(row => row.map(escapeCsvValue).join(","))
        .join("\r\n");

    const blob = new Blob(
        ["\uFEFF", csvContent],
        { type: "text/csv;charset=utf-8;" }
    );

    const url = URL.createObjectURL(blob);
    const downloadLink = document.createElement("a");
    const currentDate = new Date().toISOString().slice(0, 10);

    downloadLink.href = url;
    downloadLink.download = `simpay-pagos-${currentDate}.csv`;

    document.body.appendChild(downloadLink);
    downloadLink.click();
    downloadLink.remove();

    URL.revokeObjectURL(url);

    showFilterMessage(
        `Se exportaron ${payments.length} pagos correctamente.`,
        "success"
    );
}

function escapeCsvValue(value) {
    let text = String(value ?? "");

    if (/^[=+\-@]/.test(text)) {
        text = `'${text}`;
    }

    return `"${text.replaceAll('"', '""')}"`;
}

function updateStatistics() {
    totalPayments.textContent = payments.length;

    pendingPayments.textContent = payments.filter(
        payment => payment.status === 0
    ).length;

    completedPayments.textContent = payments.filter(
        payment => payment.status === 1
    ).length;

    rejectedPayments.textContent = payments.filter(
        payment => payment.status === 2
    ).length;
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
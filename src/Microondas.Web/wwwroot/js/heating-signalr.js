"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/heatingHub")
    .withAutomaticReconnect()
    .build();

const STATUS_LABELS = {
    "Running":   "▶ AQUECENDO",
    "Paused":    "⏸ PAUSADO",
    "Idle":      "◯ AGUARDANDO",
    "Completed": "✓ CONCLUÍDO",
    "Cancelled": "✕ CANCELADO"
};

connection.on("HeatingStarted", function (data) {
    updateStatus("Running");
    updateTimeDisplay(data.totalSeconds);
    updatePower(data.powerLevel);
    clearOutput();
});

connection.on("HeatingTicked", function (data) {
    updateStatus("Running");
    updateTimeDisplay(data.remainingSeconds, data.displayTime);
    updateOutput(data.currentOutput);
});

connection.on("HeatingPaused", function (data) {
    updateStatus("Paused");
    updateTimeDisplay(data.remainingSeconds);
});

connection.on("HeatingResumed", function (data) {
    updateStatus("Running");
    updateTimeDisplay(data.remainingSeconds);
});

connection.on("HeatingCancelled", function () {
    updateStatus("Idle");
    updateTimeDisplay(0);
    clearOutput();
    updatePower(null);
});

connection.on("HeatingCompleted", function (data) {
    updateStatus("Completed");
    updateTimeDisplay(0);
    updateOutput(data.finalOutput);
});

connection.on("HeatingTimeAdded", function (data) {
    updateTimeDisplay(data.newRemainingSeconds);
});

function updateStatus(status) {
    const el    = document.getElementById("status-label");
    const panel = document.getElementById("displayPanel");

    if (el) el.textContent = STATUS_LABELS[status] || status;

    if (panel) {
        // Remove all existing status-* classes
        const classes = Array.from(panel.classList);
        classes.forEach(c => { if (c.startsWith("status-")) panel.classList.remove(c); });
        panel.classList.add("status-" + status.toLowerCase());
    }
}

function updateTimeDisplay(seconds, displayTime) {
    const el = document.getElementById("time-display");
    if (!el) return;
    if (displayTime) {
        el.textContent = displayTime;
    } else if (seconds > 60 && seconds < 100) {
        const mins = Math.floor(seconds / 60);
        const secs = seconds % 60;
        el.textContent = `${mins}:${String(secs).padStart(2, "0")}`;
    } else {
        el.textContent = seconds;
    }
}

function updatePower(power) {
    const el = document.getElementById("power-display");
    if (el) el.textContent = power ? `POTÊNCIA: ${power}` : "POTÊNCIA: —";
}

function updateOutput(text) {
    const el = document.getElementById("output-display");
    if (el) el.textContent = text || "";
}

function clearOutput() {
    updateOutput("");
}

async function startConnection() {
    try {
        await connection.start();
        console.log("SignalR connected.");
    } catch (err) {
        console.error("SignalR connection error:", err);
        setTimeout(startConnection, 5000);
    }
}

startConnection();

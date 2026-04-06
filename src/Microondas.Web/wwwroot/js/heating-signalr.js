"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/heatingHub")
    .withAutomaticReconnect()
    .build();

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
    const el = document.getElementById("status-label");
    if (el) el.textContent = status;
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
    if (el) el.textContent = power ? `Potência: ${power}` : "Potência: -";
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

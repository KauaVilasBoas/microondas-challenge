"use strict";

// ── Real-time status polling ──────────────────────────────────────────────────
// Polls /Heating/Status every second. The endpoint proxies the REST API,
// so all state is authoritative from Microondas.Api.

const STATUS_LABELS = {
    "Running":   "▶ AQUECENDO",
    "Paused":    "⏸ PAUSADO",
    "Idle":      "◯ AGUARDANDO",
    "Completed": "✓ CONCLUÍDO",
    "Cancelled": "✕ CANCELADO"
};

let _isPolling = false;

async function pollHeatingStatus() {
    if (_isPolling) return;
    _isPolling = true;

    try {
        const res = await fetch("/Heating/Status", { credentials: "same-origin" });
        if (!res.ok) return;

        const data = await res.json();
        const status = data.status || "Idle";

        updateStatus(status);
        updateTimeDisplay(data.remainingSeconds || 0, data.displayTime);
        updatePower(data.powerLevel);
        updateOutput(data.currentOutput);
    } catch (_) {
        // API unreachable — keep last known state, don't crash
    } finally {
        _isPolling = false;
    }
}

function updateStatus(status) {
    const label = document.getElementById("status-label");
    const panel = document.getElementById("displayPanel");

    if (label) label.textContent = STATUS_LABELS[status] ?? status;

    if (panel) {
        Array.from(panel.classList)
            .filter(c => c.startsWith("status-"))
            .forEach(c => panel.classList.remove(c));

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
        const secs = String(seconds % 60).padStart(2, "0");
        el.textContent = `${mins}:${secs}`;
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

// Only poll when the heating display panel exists on the current page
if (document.getElementById("displayPanel")) {
    setInterval(pollHeatingStatus, 1000);
}

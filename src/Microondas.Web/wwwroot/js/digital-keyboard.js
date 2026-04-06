"use strict";

document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".key-btn").forEach(function (btn) {
        btn.addEventListener("click", function () {
            const target = btn.getAttribute("data-target");
            const input = document.getElementById(target);
            if (!input) return;

            const action = btn.getAttribute("data-action");
            const digit = btn.getAttribute("data-digit");

            if (action === "clear") {
                input.value = "";
            } else if (action === "backspace") {
                input.value = input.value.slice(0, -1);
            } else if (digit !== null) {
                input.value = (input.value || "") + digit;
            }
        });
    });
});

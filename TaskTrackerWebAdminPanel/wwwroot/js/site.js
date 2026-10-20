// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {
    const themeToggle = document.getElementById("themeToggle");
    const themeToggleIcon = document.getElementById("themeToggleIcon");
    const headerControls = document.querySelector(".hbt-header-controls");
    const navbar = document.getElementById("mainNavbar");
    const adminTag = navbar?.querySelector(".hbt-header-tag");

    // Stop if the theme button or its icon is missing.
    if (!themeToggle || !themeToggleIcon) {
        return;
    }

    // Load the previously saved theme.
    const savedTheme = localStorage.getItem("hbt-theme");
    const initialTheme = savedTheme === "light" ? "light" : "dark";

    // Apply the selected theme.
    function applyTheme(theme) {
        document.documentElement.dataset.theme = theme;
        document.documentElement.style.colorScheme = theme;

        const isLight = theme === "light";

        // Update the button icon.
        themeToggleIcon.className = isLight
            ? "bi bi-moon-stars-fill"
            : "bi bi-sun-fill";

        // Update accessibility label and tooltip.
        const buttonLabel = isLight
            ? "Switch to dark theme"
            : "Switch to light theme";

        themeToggle.setAttribute("aria-label", buttonLabel);
        themeToggle.setAttribute("title", buttonLabel);

        // Remember the selected theme.
        localStorage.setItem("hbt-theme", theme);
    }

    // Place the theme button according to screen size.
    function positionThemeToggle() {
        if (!headerControls || !navbar || !adminTag) {
            return;
        }

        if (window.matchMedia("(min-width: 768px)").matches) {
            // Desktop: place the button after Privacy
            // and before the Admin Panel label.
            navbar.insertBefore(themeToggle, adminTag);
        } else {
            // Mobile: place the button beside the hamburger.
            const hamburger =
                headerControls.querySelector(".navbar-toggler");

            if (hamburger) {
                headerControls.insertBefore(themeToggle, hamburger);
            } else {
                headerControls.appendChild(themeToggle);
            }
        }
    }

    // Toggle between dark and light themes when clicked.
    themeToggle.addEventListener("click", function () {
        const currentTheme =
            document.documentElement.dataset.theme || "dark";

        const newTheme =
            currentTheme === "dark" ? "light" : "dark";

        applyTheme(newTheme);
    });

    // Apply the saved theme and position the button.
    applyTheme(initialTheme);
    positionThemeToggle();

    // Reposition the button when the screen is resized.
    window.addEventListener("resize", positionThemeToggle);
});

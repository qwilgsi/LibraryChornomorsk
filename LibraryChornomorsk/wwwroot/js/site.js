document.addEventListener("DOMContentLoaded", function () {
    let currentLocation = window.location.pathname.toLowerCase();
    let navLinks = document.querySelectorAll(".navbar-nav .nav-link");

    navLinks.forEach(link => {
        let linkPath = link.getAttribute("asp-controller") ? `/${link.getAttribute("asp-controller").toLowerCase()}` : new URL(link.href).pathname.toLowerCase();

    if (currentLocation.startsWith(linkPath)) {
        navLinks.forEach(l => l.classList.remove("active"));
    link.classList.add("active");
        }
    });
});

console.log("JavaScript загружен");
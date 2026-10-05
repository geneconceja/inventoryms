// InventoryMS — Client Scripts

function toggleTenantDropdown() {
    const menu = document.getElementById('tenantDropdownMenu');
    if (menu) {
        menu.classList.toggle('show');
    }
}

document.addEventListener('click', function (e) {
    const switcher = document.querySelector('.sidebar-tenant-switcher');
    const menu = document.getElementById('tenantDropdownMenu');
    if (switcher && menu && !switcher.contains(e.target)) {
        menu.classList.remove('show');
    }
});

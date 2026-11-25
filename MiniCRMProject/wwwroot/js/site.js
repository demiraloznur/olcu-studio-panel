// Dar ekranda sol menüyü aç/kapat.
(function () {
    const button = document.querySelector('.menu-dugme');
    const menu = document.getElementById('yan-menu');
    if (!button || !menu) return;

    function setOpen(open) {
        menu.classList.toggle('acik', open);
        button.setAttribute('aria-expanded', String(open));
    }

    button.addEventListener('click', function () {
        setOpen(!menu.classList.contains('acik'));
    });

    document.addEventListener('keydown', function (event) {
        if (event.key === 'Escape' && menu.classList.contains('acik')) {
            setOpen(false);
            button.focus();
        }
    });
})();

document.querySelectorAll('.mui-button:not(.mui-button-disabled)').forEach(button => {
    button.addEventListener('click', function (e) {
        const ripple = document.createElement('span');
        const rect = button.getBoundingClientRect();
        const size = Math.max(rect.width, rect.height);
        const left = e.clientX - rect.left - size / 2;
        const top = e.clientY - rect.top - size / 2;

        ripple.classList.add('mui-ripple');
        ripple.style.width = ripple.style.height = size + 'px';
        ripple.style.left = left + 'px';
        ripple.style.top = top + 'px';

        button.appendChild(ripple);

        setTimeout(() => ripple.remove(), 600);
    });
});
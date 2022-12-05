/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './**/*.html',
        './**/*.razor'
    ],
    theme: {
        extend: {},
    },
    plugins: [
        require('@tailwindcss/forms'),
        require('@tailwindcss/typography'),
    ],
}

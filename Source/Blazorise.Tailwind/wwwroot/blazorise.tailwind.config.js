tailwind.config = {
    content: ["**/*.razor", "**/*.cshtml", "**/*.html"],
    theme: {
        container: {
            center: true,
        },
        screens: {
            'sm': '640px',
            'md': '768px',
            'lg': '1024px',
            'xl': '1280px',
            '2xl': '1536px',
        },
        fontFamily: {
            sans: ['Graphik', 'sans-serif'],
            serif: ['Merriweather', 'serif'],
        },
        fontSize: {
            md: ['1.0625rem', '1.25rem']
        },
        variants: {
            backgroundColor: ['even', 'odd', 'hover']
        },
        extend: {
            colors: {
                primary: {
                    DEFAULT: '#3b82f6',
                    50: '#eff6ff',
                    100: '#dbeafe',
                    200: '#bfdbfe',
                    300: '#93c5fd',
                    400: '#60a5fa',
                    500: '#3b82f6',
                    600: '#2563eb',
                    700: '#1d4ed8',
                    800: '#1e40af',
                    900: '#1e3a8a'
                },
                secondary: {
                    DEFAULT: '#6B7280',
                    50: "#F1F2F3",
                    100: "#E0E2E5",
                    200: "#C2C5CC",
                    300: "#A6ABB5",
                    400: "#888E9B",
                    500: "#6B7280",
                    600: "#565C67",
                    700: "#41454E",
                    800: "#2A2D32",
                    900: "#151619"
                },
                success: {
                    DEFAULT: '#0E9F6E',
                    50: '#F3FAF7',
                    100: '#DEF7EC',
                    200: '#BCF0DA',
                    300: '#84E1BC',
                    400: '#31C48D',
                    500: '#0E9F6E',
                    600: '#057A55',
                    700: '#046C4E',
                    800: '#03543F',
                    900: '#014737'
                },
                danger: {
                    DEFAULT: '#F05252',
                    50: '#FDF2F2',
                    100: '#FDE8E8',
                    200: '#FBD5D5',
                    300: '#F8B4B4',
                    400: '#F98080',
                    500: '#F05252',
                    600: '#E02424',
                    700: '#C81E1E',
                    800: '#9B1C1C',
                    900: '#771D1D'
                },
                warning: {
                    DEFAULT: '#C27803',
                    50: '#FDFDEA',
                    100: '#FDF6B2',
                    200: '#FCE96A',
                    300: '#FACA15',
                    400: '#E3A008',
                    500: '#C27803',
                    600: '#9F580A',
                    700: '#8E4B10',
                    800: '#723B13',
                    900: '#633112'
                },
                info: {
                    DEFAULT: '#03A9F4',
                    50: '#E1F5FE',
                    100: '#B3E5FC',
                    200: '#81D4FA',
                    300: '#4FC3F7',
                    400: '#29B6F6',
                    500: '#03A9F4',
                    600: '#039BE5',
                    700: '#0288D1',
                    800: '#0277BD',
                    900: '#01579B'
                },
                light: {
                    DEFAULT: '#F3F4F6',
                    50: '#F9FAFB',
                    100: '#F3F4F6',
                    200: '#E5E7EB',
                    300: '#D1D5DB',
                    400: '#9CA3AF',
                    500: '#6B7280',
                    600: '#4B5563',
                    700: '#374151',
                    800: '#1F2937',
                    900: '#111827'
                },
                dark: {
                    DEFAULT: '#1F2937',
                    50: '#F9FAFB',
                    100: '#F3F4F6',
                    200: '#E5E7EB',
                    300: '#D1D5DB',
                    400: '#9CA3AF',
                    500: '#6B7280',
                    600: '#4B5563',
                    700: '#374151',
                    800: '#1F2937',
                    900: '#111827'
                }
            },
            spacing: {
                '128': '32rem',
                '144': '36rem',
            },
            borderWidth: {
                DEFAULT: '1px',
                '0': '0',
                '1': '1px',
                '2': '2px',
                '3': '3px',
                '4': '4px',
                '5': '5px',
                '6': '6px',
                '7': '7px',
                '8': '8px'
            },
            borderRadius: {
                '4xl': '2rem',
            },
            flexGrow: {
                '0': 0,
                '1': 1
            }
        }
    },
    plugins: [
        ({ addVariant }) => {
            // based on: https://github.com/tailwindlabs/tailwindcss/blob/f116f9f6648af81bf22e0c28d01a8da015a53180/src/corePlugins.js#L61-L129
            [
                // Positional
                ['first', ':first-child'],
                ['last', ':last-child'],
                ['only', ':only-child'],
                ['odd', ':nth-child(odd)'],
                ['even', ':nth-child(even)'],
                'first-of-type',
                'last-of-type',
                'only-of-type',

                // State
                'visited',
                'target',
                ['open', '[open]'],

                // Forms
                'default',
                'checked',
                'indeterminate',
                'placeholder-shown',
                'autofill',
                'required',
                'valid',
                'invalid',
                'in-range',
                'out-of-range',
                'read-only',

                // Content
                'empty',

                // Interactive
                'focus-within',
                'hover',
                'focus',
                'focus-visible',
                'active',
                'disabled',
            ]
                .map((variant) =>
                    Array.isArray(variant) ? variant : [variant, `:${variant}`]
                )
                .forEach(([variantName, state]) => {
                    addVariant(`parent-${variantName}`, `:merge(.parent)${state} > &`);
                });
        },
        function ({ addBase, theme }) {
            function extractColorVars(colorObj, colorGroup = '') {
                return Object.keys(colorObj).reduce((vars, colorKey) => {
                    const value = colorObj[colorKey];
                    const cssVariable = colorKey === "DEFAULT" ? `--btw-color${colorGroup}` : `--btw-color${colorGroup}-${colorKey}`;

                    const newVars =
                        typeof value === 'string'
                            ? { [cssVariable]: value }
                            : extractColorVars(value, `-${colorKey}`);

                    return { ...vars, ...newVars };
                }, {});
            }

            addBase({
                ':root': extractColorVars(theme('colors')),
            });
        }
    ]
}

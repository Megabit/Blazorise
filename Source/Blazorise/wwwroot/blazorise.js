// ========================================
!function (e, t) { "object" == typeof exports && "undefined" != typeof module ? module.exports = t() : "function" == typeof define && define.amd ? define(t) : (e = "undefined" != typeof globalThis ? globalThis : e || self).flatpickr = t() }(this, (function () { "use strict"; var e = function () { return (e = Object.assign || function (e) { for (var t, n = 1, a = arguments.length; n < a; n++)for (var i in t = arguments[n]) Object.prototype.hasOwnProperty.call(t, i) && (e[i] = t[i]); return e }).apply(this, arguments) }; function t() { for (var e = 0, t = 0, n = arguments.length; t < n; t++)e += arguments[t].length; var a = Array(e), i = 0; for (t = 0; t < n; t++)for (var o = arguments[t], r = 0, l = o.length; r < l; r++, i++)a[i] = o[r]; return a } var n = ["onChange", "onClose", "onDayCreate", "onDestroy", "onKeyDown", "onMonthChange", "onOpen", "onParseConfig", "onReady", "onValueUpdate", "onYearChange", "onPreCalendarPosition"], a = { _disable: [], allowInput: !1, allowInvalidPreload: !1, altFormat: "F j, Y", altInput: !1, altInputClass: "form-control input", animate: "object" == typeof window && -1 === window.navigator.userAgent.indexOf("MSIE"), ariaDateFormat: "F j, Y", autoFillDefaultTime: !0, clickOpens: !0, closeOnSelect: !0, conjunction: ", ", dateFormat: "Y-m-d", defaultHour: 12, defaultMinute: 0, defaultSeconds: 0, disable: [], disableMobile: !1, enableSeconds: !1, enableTime: !1, errorHandler: function (e) { return "undefined" != typeof console && console.warn(e) }, getWeek: function (e) { var t = new Date(e.getTime()); t.setHours(0, 0, 0, 0), t.setDate(t.getDate() + 3 - (t.getDay() + 6) % 7); var n = new Date(t.getFullYear(), 0, 4); return 1 + Math.round(((t.getTime() - n.getTime()) / 864e5 - 3 + (n.getDay() + 6) % 7) / 7) }, hourIncrement: 1, ignoredFocusElements: [], inline: !1, locale: "default", minuteIncrement: 5, mode: "single", monthSelectorType: "dropdown", nextArrow: "<svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' viewBox='0 0 17 17'><g></g><path d='M13.207 8.472l-7.854 7.854-0.707-0.707 7.146-7.146-7.146-7.148 0.707-0.707 7.854 7.854z' /></svg>", noCalendar: !1, now: new Date, onChange: [], onClose: [], onDayCreate: [], onDestroy: [], onKeyDown: [], onMonthChange: [], onOpen: [], onParseConfig: [], onReady: [], onValueUpdate: [], onYearChange: [], onPreCalendarPosition: [], plugins: [], position: "auto", positionElement: void 0, prevArrow: "<svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' viewBox='0 0 17 17'><g></g><path d='M5.207 8.471l7.146 7.147-0.707 0.707-7.853-7.854 7.854-7.853 0.707 0.707-7.147 7.146z' /></svg>", shorthandCurrentMonth: !1, showMonths: 1, static: !1, time_24hr: !1, weekNumbers: !1, wrap: !1 }, i = { weekdays: { shorthand: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"], longhand: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"] }, months: { shorthand: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"], longhand: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"] }, daysInMonth: [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31], firstDayOfWeek: 0, ordinal: function (e) { var t = e % 100; if (t > 3 && t < 21) return "th"; switch (t % 10) { case 1: return "st"; case 2: return "nd"; case 3: return "rd"; default: return "th" } }, rangeSeparator: " to ", weekAbbreviation: "Wk", scrollTitle: "Scroll to increment", toggleTitle: "Click to toggle", amPM: ["AM", "PM"], yearAriaLabel: "Year", monthAriaLabel: "Month", hourAriaLabel: "Hour", minuteAriaLabel: "Minute", time_24hr: !1 }, o = function (e, t) { return void 0 === t && (t = 2), ("000" + e).slice(-1 * t) }, r = function (e) { return !0 === e ? 1 : 0 }; function l(e, t) { var n; return function () { var a = this; clearTimeout(n), n = setTimeout((function () { return e.apply(a, arguments) }), t) } } var c = function (e) { return e instanceof Array ? e : [e] }; function d(e, t, n) { if (!0 === n) return e.classList.add(t); e.classList.remove(t) } function s(e, t, n) { var a = window.document.createElement(e); return t = t || "", n = n || "", a.className = t, void 0 !== n && (a.textContent = n), a } function u(e) { for (; e.firstChild;)e.removeChild(e.firstChild) } function f(e, t) { return t(e) ? e : e.parentNode ? f(e.parentNode, t) : void 0 } function m(e, t) { var n = s("div", "numInputWrapper"), a = s("input", "numInput " + e), i = s("span", "arrowUp"), o = s("span", "arrowDown"); if (-1 === navigator.userAgent.indexOf("MSIE 9.0") ? a.type = "number" : (a.type = "text", a.pattern = "\\d*"), void 0 !== t) for (var r in t) a.setAttribute(r, t[r]); return n.appendChild(a), n.appendChild(i), n.appendChild(o), n } function g(e) { try { return "function" == typeof e.composedPath ? e.composedPath()[0] : e.target } catch (t) { return e.target } } var p = function () { }, h = function (e, t, n) { return n.months[t ? "shorthand" : "longhand"][e] }, v = { D: p, F: function (e, t, n) { e.setMonth(n.months.longhand.indexOf(t)) }, G: function (e, t) { e.setHours(parseFloat(t)) }, H: function (e, t) { e.setHours(parseFloat(t)) }, J: function (e, t) { e.setDate(parseFloat(t)) }, K: function (e, t, n) { e.setHours(e.getHours() % 12 + 12 * r(new RegExp(n.amPM[1], "i").test(t))) }, M: function (e, t, n) { e.setMonth(n.months.shorthand.indexOf(t)) }, S: function (e, t) { e.setSeconds(parseFloat(t)) }, U: function (e, t) { return new Date(1e3 * parseFloat(t)) }, W: function (e, t, n) { var a = parseInt(t), i = new Date(e.getFullYear(), 0, 2 + 7 * (a - 1), 0, 0, 0, 0); return i.setDate(i.getDate() - i.getDay() + n.firstDayOfWeek), i }, Y: function (e, t) { e.setFullYear(parseFloat(t)) }, Z: function (e, t) { return new Date(t) }, d: function (e, t) { e.setDate(parseFloat(t)) }, h: function (e, t) { e.setHours(parseFloat(t)) }, i: function (e, t) { e.setMinutes(parseFloat(t)) }, j: function (e, t) { e.setDate(parseFloat(t)) }, l: p, m: function (e, t) { e.setMonth(parseFloat(t) - 1) }, n: function (e, t) { e.setMonth(parseFloat(t) - 1) }, s: function (e, t) { e.setSeconds(parseFloat(t)) }, u: function (e, t) { return new Date(parseFloat(t)) }, w: p, y: function (e, t) { e.setFullYear(2e3 + parseFloat(t)) } }, D = { D: "(\\w+)", F: "(\\w+)", G: "(\\d\\d|\\d)", H: "(\\d\\d|\\d)", J: "(\\d\\d|\\d)\\w+", K: "", M: "(\\w+)", S: "(\\d\\d|\\d)", U: "(.+)", W: "(\\d\\d|\\d)", Y: "(\\d{4})", Z: "(.+)", d: "(\\d\\d|\\d)", h: "(\\d\\d|\\d)", i: "(\\d\\d|\\d)", j: "(\\d\\d|\\d)", l: "(\\w+)", m: "(\\d\\d|\\d)", n: "(\\d\\d|\\d)", s: "(\\d\\d|\\d)", u: "(.+)", w: "(\\d\\d|\\d)", y: "(\\d{2})" }, w = { Z: function (e) { return e.toISOString() }, D: function (e, t, n) { return t.weekdays.shorthand[w.w(e, t, n)] }, F: function (e, t, n) { return h(w.n(e, t, n) - 1, !1, t) }, G: function (e, t, n) { return o(w.h(e, t, n)) }, H: function (e) { return o(e.getHours()) }, J: function (e, t) { return void 0 !== t.ordinal ? e.getDate() + t.ordinal(e.getDate()) : e.getDate() }, K: function (e, t) { return t.amPM[r(e.getHours() > 11)] }, M: function (e, t) { return h(e.getMonth(), !0, t) }, S: function (e) { return o(e.getSeconds()) }, U: function (e) { return e.getTime() / 1e3 }, W: function (e, t, n) { return n.getWeek(e) }, Y: function (e) { return o(e.getFullYear(), 4) }, d: function (e) { return o(e.getDate()) }, h: function (e) { return e.getHours() % 12 ? e.getHours() % 12 : 12 }, i: function (e) { return o(e.getMinutes()) }, j: function (e) { return e.getDate() }, l: function (e, t) { return t.weekdays.longhand[e.getDay()] }, m: function (e) { return o(e.getMonth() + 1) }, n: function (e) { return e.getMonth() + 1 }, s: function (e) { return e.getSeconds() }, u: function (e) { return e.getTime() }, w: function (e) { return e.getDay() }, y: function (e) { return String(e.getFullYear()).substring(2) } }, b = function (e) { var t = e.config, n = void 0 === t ? a : t, o = e.l10n, r = void 0 === o ? i : o, l = e.isMobile, c = void 0 !== l && l; return function (e, t, a) { var i = a || r; return void 0 === n.formatDate || c ? t.split("").map((function (t, a, o) { return w[t] && "\\" !== o[a - 1] ? w[t](e, i, n) : "\\" !== t ? t : "" })).join("") : n.formatDate(e, t, i) } }, C = function (e) { var t = e.config, n = void 0 === t ? a : t, o = e.l10n, r = void 0 === o ? i : o; return function (e, t, i, o) { if (0 === e || e) { var l, c = o || r, d = e; if (e instanceof Date) l = new Date(e.getTime()); else if ("string" != typeof e && void 0 !== e.toFixed) l = new Date(e); else if ("string" == typeof e) { var s = t || (n || a).dateFormat, u = String(e).trim(); if ("today" === u) l = new Date, i = !0; else if (/Z$/.test(u) || /GMT$/.test(u)) l = new Date(e); else if (n && n.parseDate) l = n.parseDate(e, s); else { l = n && n.noCalendar ? new Date((new Date).setHours(0, 0, 0, 0)) : new Date((new Date).getFullYear(), 0, 1, 0, 0, 0, 0); for (var f = void 0, m = [], g = 0, p = 0, h = ""; g < s.length; g++) { var w = s[g], b = "\\" === w, C = "\\" === s[g - 1] || b; if (D[w] && !C) { h += D[w]; var M = new RegExp(h).exec(e); M && (f = !0) && m["Y" !== w ? "push" : "unshift"]({ fn: v[w], val: M[++p] }) } else b || (h += "."); m.forEach((function (e) { var t = e.fn, n = e.val; return l = t(l, n, c) || l })) } l = f ? l : void 0 } } if (l instanceof Date && !isNaN(l.getTime())) return !0 === i && l.setHours(0, 0, 0, 0), l; n.errorHandler(new Error("Invalid date provided: " + d)) } } }; function M(e, t, n) { return void 0 === n && (n = !0), !1 !== n ? new Date(e.getTime()).setHours(0, 0, 0, 0) - new Date(t.getTime()).setHours(0, 0, 0, 0) : e.getTime() - t.getTime() } var y = 864e5; function x(e) { var t = e.defaultHour, n = e.defaultMinute, a = e.defaultSeconds; if (void 0 !== e.minDate) { var i = e.minDate.getHours(), o = e.minDate.getMinutes(), r = e.minDate.getSeconds(); t < i && (t = i), t === i && n < o && (n = o), t === i && n === o && a < r && (a = e.minDate.getSeconds()) } if (void 0 !== e.maxDate) { var l = e.maxDate.getHours(), c = e.maxDate.getMinutes(); (t = Math.min(t, l)) === l && (n = Math.min(c, n)), t === l && n === c && (a = e.maxDate.getSeconds()) } return { hours: t, minutes: n, seconds: a } } "function" != typeof Object.assign && (Object.assign = function (e) { for (var t = [], n = 1; n < arguments.length; n++)t[n - 1] = arguments[n]; if (!e) throw TypeError("Cannot convert undefined or null to object"); for (var a = function (t) { t && Object.keys(t).forEach((function (n) { return e[n] = t[n] })) }, i = 0, o = t; i < o.length; i++) { var r = o[i]; a(r) } return e }); function E(p, v) { var w = { config: e(e({}, a), T.defaultConfig), l10n: i }; function E(e) { return e.bind(w) } function k() { var e = w.config; !1 === e.weekNumbers && 1 === e.showMonths || !0 !== e.noCalendar && window.requestAnimationFrame((function () { if (void 0 !== w.calendarContainer && (w.calendarContainer.style.visibility = "hidden", w.calendarContainer.style.display = "block"), void 0 !== w.daysContainer) { var t = (w.days.offsetWidth + 1) * e.showMonths; w.daysContainer.style.width = t + "px", w.calendarContainer.style.width = t + (void 0 !== w.weekWrapper ? w.weekWrapper.offsetWidth : 0) + "px", w.calendarContainer.style.removeProperty("visibility"), w.calendarContainer.style.removeProperty("display") } })) } function I(e) { if (0 === w.selectedDates.length) { var t = void 0 === w.config.minDate || M(new Date, w.config.minDate) >= 0 ? new Date : new Date(w.config.minDate.getTime()), n = x(w.config); t.setHours(n.hours, n.minutes, n.seconds, t.getMilliseconds()), w.selectedDates = [t], w.latestSelectedDateObj = t } void 0 !== e && "blur" !== e.type && function (e) { e.preventDefault(); var t = "keydown" === e.type, n = g(e), a = n; void 0 !== w.amPM && n === w.amPM && (w.amPM.textContent = w.l10n.amPM[r(w.amPM.textContent === w.l10n.amPM[0])]); var i = parseFloat(a.getAttribute("min")), l = parseFloat(a.getAttribute("max")), c = parseFloat(a.getAttribute("step")), d = parseInt(a.value, 10), s = e.delta || (t ? 38 === e.which ? 1 : -1 : 0), u = d + c * s; if (void 0 !== a.value && 2 === a.value.length) { var f = a === w.hourElement, m = a === w.minuteElement; u < i ? (u = l + u + r(!f) + (r(f) && r(!w.amPM)), m && j(void 0, -1, w.hourElement)) : u > l && (u = a === w.hourElement ? u - l - r(!w.amPM) : i, m && j(void 0, 1, w.hourElement)), w.amPM && f && (1 === c ? u + d === 23 : Math.abs(u - d) > c) && (w.amPM.textContent = w.l10n.amPM[r(w.amPM.textContent === w.l10n.amPM[0])]), a.value = o(u) } }(e); var a = w._input.value; S(), be(), w._input.value !== a && w._debouncedChange() } function S() { if (void 0 !== w.hourElement && void 0 !== w.minuteElement) { var e, t, n = (parseInt(w.hourElement.value.slice(-2), 10) || 0) % 24, a = (parseInt(w.minuteElement.value, 10) || 0) % 60, i = void 0 !== w.secondElement ? (parseInt(w.secondElement.value, 10) || 0) % 60 : 0; void 0 !== w.amPM && (e = n, t = w.amPM.textContent, n = e % 12 + 12 * r(t === w.l10n.amPM[1])); var o = void 0 !== w.config.minTime || w.config.minDate && w.minDateHasTime && w.latestSelectedDateObj && 0 === M(w.latestSelectedDateObj, w.config.minDate, !0); if (void 0 !== w.config.maxTime || w.config.maxDate && w.maxDateHasTime && w.latestSelectedDateObj && 0 === M(w.latestSelectedDateObj, w.config.maxDate, !0)) { var l = void 0 !== w.config.maxTime ? w.config.maxTime : w.config.maxDate; (n = Math.min(n, l.getHours())) === l.getHours() && (a = Math.min(a, l.getMinutes())), a === l.getMinutes() && (i = Math.min(i, l.getSeconds())) } if (o) { var c = void 0 !== w.config.minTime ? w.config.minTime : w.config.minDate; (n = Math.max(n, c.getHours())) === c.getHours() && a < c.getMinutes() && (a = c.getMinutes()), a === c.getMinutes() && (i = Math.max(i, c.getSeconds())) } O(n, a, i) } } function _(e) { var t = e || w.latestSelectedDateObj; t && O(t.getHours(), t.getMinutes(), t.getSeconds()) } function O(e, t, n) { void 0 !== w.latestSelectedDateObj && w.latestSelectedDateObj.setHours(e % 24, t, n || 0, 0), w.hourElement && w.minuteElement && !w.isMobile && (w.hourElement.value = o(w.config.time_24hr ? e : (12 + e) % 12 + 12 * r(e % 12 == 0)), w.minuteElement.value = o(t), void 0 !== w.amPM && (w.amPM.textContent = w.l10n.amPM[r(e >= 12)]), void 0 !== w.secondElement && (w.secondElement.value = o(n))) } function F(e) { var t = g(e), n = parseInt(t.value) + (e.delta || 0); (n / 1e3 > 1 || "Enter" === e.key && !/[^\d]/.test(n.toString())) && Q(n) } function A(e, t, n, a) { return t instanceof Array ? t.forEach((function (t) { return A(e, t, n, a) })) : e instanceof Array ? e.forEach((function (e) { return A(e, t, n, a) })) : (e.addEventListener(t, n, a), void w._handlers.push({ remove: function () { return e.removeEventListener(t, n) } })) } function N() { pe("onChange") } function P(e, t) { var n = void 0 !== e ? w.parseDate(e) : w.latestSelectedDateObj || (w.config.minDate && w.config.minDate > w.now ? w.config.minDate : w.config.maxDate && w.config.maxDate < w.now ? w.config.maxDate : w.now), a = w.currentYear, i = w.currentMonth; try { void 0 !== n && (w.currentYear = n.getFullYear(), w.currentMonth = n.getMonth()) } catch (e) { e.message = "Invalid date supplied: " + n, w.config.errorHandler(e) } t && w.currentYear !== a && (pe("onYearChange"), K()), !t || w.currentYear === a && w.currentMonth === i || pe("onMonthChange"), w.redraw() } function Y(e) { var t = g(e); ~t.className.indexOf("arrow") && j(e, t.classList.contains("arrowUp") ? 1 : -1) } function j(e, t, n) { var a = e && g(e), i = n || a && a.parentNode && a.parentNode.firstChild, o = he("increment"); o.delta = t, i && i.dispatchEvent(o) } function H(e, t, n, a) { var i = X(t, !0), o = s("span", "flatpickr-day " + e, t.getDate().toString()); return o.dateObj = t, o.$i = a, o.setAttribute("aria-label", w.formatDate(t, w.config.ariaDateFormat)), -1 === e.indexOf("hidden") && 0 === M(t, w.now) && (w.todayDateElem = o, o.classList.add("today"), o.setAttribute("aria-current", "date")), i ? (o.tabIndex = -1, ve(t) && (o.classList.add("selected"), w.selectedDateElem = o, "range" === w.config.mode && (d(o, "startRange", w.selectedDates[0] && 0 === M(t, w.selectedDates[0], !0)), d(o, "endRange", w.selectedDates[1] && 0 === M(t, w.selectedDates[1], !0)), "nextMonthDay" === e && o.classList.add("inRange")))) : o.classList.add("flatpickr-disabled"), "range" === w.config.mode && function (e) { return !("range" !== w.config.mode || w.selectedDates.length < 2) && (M(e, w.selectedDates[0]) >= 0 && M(e, w.selectedDates[1]) <= 0) }(t) && !ve(t) && o.classList.add("inRange"), w.weekNumbers && 1 === w.config.showMonths && "prevMonthDay" !== e && n % 7 == 1 && w.weekNumbers.insertAdjacentHTML("beforeend", "<span class='flatpickr-day'>" + w.config.getWeek(t) + "</span>"), pe("onDayCreate", o), o } function L(e) { e.focus(), "range" === w.config.mode && ae(e) } function W(e) { for (var t = e > 0 ? 0 : w.config.showMonths - 1, n = e > 0 ? w.config.showMonths : -1, a = t; a != n; a += e)for (var i = w.daysContainer.children[a], o = e > 0 ? 0 : i.children.length - 1, r = e > 0 ? i.children.length : -1, l = o; l != r; l += e) { var c = i.children[l]; if (-1 === c.className.indexOf("hidden") && X(c.dateObj)) return c } } function R(e, t) { var n = ee(document.activeElement || document.body), a = void 0 !== e ? e : n ? document.activeElement : void 0 !== w.selectedDateElem && ee(w.selectedDateElem) ? w.selectedDateElem : void 0 !== w.todayDateElem && ee(w.todayDateElem) ? w.todayDateElem : W(t > 0 ? 1 : -1); void 0 === a ? w._input.focus() : n ? function (e, t) { for (var n = -1 === e.className.indexOf("Month") ? e.dateObj.getMonth() : w.currentMonth, a = t > 0 ? w.config.showMonths : -1, i = t > 0 ? 1 : -1, o = n - w.currentMonth; o != a; o += i)for (var r = w.daysContainer.children[o], l = n - w.currentMonth === o ? e.$i + t : t < 0 ? r.children.length - 1 : 0, c = r.children.length, d = l; d >= 0 && d < c && d != (t > 0 ? c : -1); d += i) { var s = r.children[d]; if (-1 === s.className.indexOf("hidden") && X(s.dateObj) && Math.abs(e.$i - d) >= Math.abs(t)) return L(s) } w.changeMonth(i), R(W(i), 0) }(a, t) : L(a) } function B(e, t) { for (var n = (new Date(e, t, 1).getDay() - w.l10n.firstDayOfWeek + 7) % 7, a = w.utils.getDaysInMonth((t - 1 + 12) % 12, e), i = w.utils.getDaysInMonth(t, e), o = window.document.createDocumentFragment(), r = w.config.showMonths > 1, l = r ? "prevMonthDay hidden" : "prevMonthDay", c = r ? "nextMonthDay hidden" : "nextMonthDay", d = a + 1 - n, u = 0; d <= a; d++, u++)o.appendChild(H(l, new Date(e, t - 1, d), d, u)); for (d = 1; d <= i; d++, u++)o.appendChild(H("", new Date(e, t, d), d, u)); for (var f = i + 1; f <= 42 - n && (1 === w.config.showMonths || u % 7 != 0); f++, u++)o.appendChild(H(c, new Date(e, t + 1, f % i), f, u)); var m = s("div", "dayContainer"); return m.appendChild(o), m } function J() { if (void 0 !== w.daysContainer) { u(w.daysContainer), w.weekNumbers && u(w.weekNumbers); for (var e = document.createDocumentFragment(), t = 0; t < w.config.showMonths; t++) { var n = new Date(w.currentYear, w.currentMonth, 1); n.setMonth(w.currentMonth + t), e.appendChild(B(n.getFullYear(), n.getMonth())) } w.daysContainer.appendChild(e), w.days = w.daysContainer.firstChild, "range" === w.config.mode && 1 === w.selectedDates.length && ae() } } function K() { if (!(w.config.showMonths > 1 || "dropdown" !== w.config.monthSelectorType)) { var e = function (e) { return !(void 0 !== w.config.minDate && w.currentYear === w.config.minDate.getFullYear() && e < w.config.minDate.getMonth()) && !(void 0 !== w.config.maxDate && w.currentYear === w.config.maxDate.getFullYear() && e > w.config.maxDate.getMonth()) }; w.monthsDropdownContainer.tabIndex = -1, w.monthsDropdownContainer.innerHTML = ""; for (var t = 0; t < 12; t++)if (e(t)) { var n = s("option", "flatpickr-monthDropdown-month"); n.value = new Date(w.currentYear, t).getMonth().toString(), n.textContent = h(t, w.config.shorthandCurrentMonth, w.l10n), n.tabIndex = -1, w.currentMonth === t && (n.selected = !0), w.monthsDropdownContainer.appendChild(n) } } } function U() { var e, t = s("div", "flatpickr-month"), n = window.document.createDocumentFragment(); w.config.showMonths > 1 || "static" === w.config.monthSelectorType ? e = s("span", "cur-month") : (w.monthsDropdownContainer = s("select", "flatpickr-monthDropdown-months"), w.monthsDropdownContainer.setAttribute("aria-label", w.l10n.monthAriaLabel), A(w.monthsDropdownContainer, "change", (function (e) { var t = g(e), n = parseInt(t.value, 10); w.changeMonth(n - w.currentMonth), pe("onMonthChange") })), K(), e = w.monthsDropdownContainer); var a = m("cur-year", { tabindex: "-1" }), i = a.getElementsByTagName("input")[0]; i.setAttribute("aria-label", w.l10n.yearAriaLabel), w.config.minDate && i.setAttribute("min", w.config.minDate.getFullYear().toString()), w.config.maxDate && (i.setAttribute("max", w.config.maxDate.getFullYear().toString()), i.disabled = !!w.config.minDate && w.config.minDate.getFullYear() === w.config.maxDate.getFullYear()); var o = s("div", "flatpickr-current-month"); return o.appendChild(e), o.appendChild(a), n.appendChild(o), t.appendChild(n), { container: t, yearElement: i, monthElement: e } } function q() { u(w.monthNav), w.monthNav.appendChild(w.prevMonthNav), w.config.showMonths && (w.yearElements = [], w.monthElements = []); for (var e = w.config.showMonths; e--;) { var t = U(); w.yearElements.push(t.yearElement), w.monthElements.push(t.monthElement), w.monthNav.appendChild(t.container) } w.monthNav.appendChild(w.nextMonthNav) } function $() { w.weekdayContainer ? u(w.weekdayContainer) : w.weekdayContainer = s("div", "flatpickr-weekdays"); for (var e = w.config.showMonths; e--;) { var t = s("div", "flatpickr-weekdaycontainer"); w.weekdayContainer.appendChild(t) } return z(), w.weekdayContainer } function z() { if (w.weekdayContainer) { var e = w.l10n.firstDayOfWeek, n = t(w.l10n.weekdays.shorthand); e > 0 && e < n.length && (n = t(n.splice(e, n.length), n.splice(0, e))); for (var a = w.config.showMonths; a--;)w.weekdayContainer.children[a].innerHTML = "\n      <span class='flatpickr-weekday'>\n        " + n.join("</span><span class='flatpickr-weekday'>") + "\n      </span>\n      " } } function G(e, t) { void 0 === t && (t = !0); var n = t ? e : e - w.currentMonth; n < 0 && !0 === w._hidePrevMonthArrow || n > 0 && !0 === w._hideNextMonthArrow || (w.currentMonth += n, (w.currentMonth < 0 || w.currentMonth > 11) && (w.currentYear += w.currentMonth > 11 ? 1 : -1, w.currentMonth = (w.currentMonth + 12) % 12, pe("onYearChange"), K()), J(), pe("onMonthChange"), De()) } function V(e) { return !(!w.config.appendTo || !w.config.appendTo.contains(e)) || w.calendarContainer.contains(e) } function Z(e) { if (w.isOpen && !w.config.inline) { var t = g(e), n = V(t), a = t === w.input || t === w.altInput || w.element.contains(t) || e.path && e.path.indexOf && (~e.path.indexOf(w.input) || ~e.path.indexOf(w.altInput)), i = "blur" === e.type ? a && e.relatedTarget && !V(e.relatedTarget) : !a && !n && !V(e.relatedTarget), o = !w.config.ignoredFocusElements.some((function (e) { return e.contains(t) })); i && o && (void 0 !== w.timeContainer && void 0 !== w.minuteElement && void 0 !== w.hourElement && "" !== w.input.value && void 0 !== w.input.value && I(), w.close(), w.config && "range" === w.config.mode && 1 === w.selectedDates.length && (w.clear(!1), w.redraw())) } } function Q(e) { if (!(!e || w.config.minDate && e < w.config.minDate.getFullYear() || w.config.maxDate && e > w.config.maxDate.getFullYear())) { var t = e, n = w.currentYear !== t; w.currentYear = t || w.currentYear, w.config.maxDate && w.currentYear === w.config.maxDate.getFullYear() ? w.currentMonth = Math.min(w.config.maxDate.getMonth(), w.currentMonth) : w.config.minDate && w.currentYear === w.config.minDate.getFullYear() && (w.currentMonth = Math.max(w.config.minDate.getMonth(), w.currentMonth)), n && (w.redraw(), pe("onYearChange"), K()) } } function X(e, t) { var n; void 0 === t && (t = !0); var a = w.parseDate(e, void 0, t); if (w.config.minDate && a && M(a, w.config.minDate, void 0 !== t ? t : !w.minDateHasTime) < 0 || w.config.maxDate && a && M(a, w.config.maxDate, void 0 !== t ? t : !w.maxDateHasTime) > 0) return !1; if (!w.config.enable && 0 === w.config.disable.length) return !0; if (void 0 === a) return !1; for (var i = !!w.config.enable, o = null !== (n = w.config.enable) && void 0 !== n ? n : w.config.disable, r = 0, l = void 0; r < o.length; r++) { if ("function" == typeof (l = o[r]) && l(a)) return i; if (l instanceof Date && void 0 !== a && l.getTime() === a.getTime()) return i; if ("string" == typeof l) { var c = w.parseDate(l, void 0, !0); return c && c.getTime() === a.getTime() ? i : !i } if ("object" == typeof l && void 0 !== a && l.from && l.to && a.getTime() >= l.from.getTime() && a.getTime() <= l.to.getTime()) return i } return !i } function ee(e) { return void 0 !== w.daysContainer && (-1 === e.className.indexOf("hidden") && -1 === e.className.indexOf("flatpickr-disabled") && w.daysContainer.contains(e)) } function te(e) { !(e.target === w._input) || !(w.selectedDates.length > 0 || w._input.value.length > 0) || e.relatedTarget && V(e.relatedTarget) || w.setDate(w._input.value, !0, e.target === w.altInput ? w.config.altFormat : w.config.dateFormat) } function ne(e) { var t = g(e), n = w.config.wrap ? p.contains(t) : t === w._input, a = w.config.allowInput, i = w.isOpen && (!a || !n), o = w.config.inline && n && !a; if (13 === e.keyCode && n) { if (a) return w.setDate(w._input.value, !0, t === w.altInput ? w.config.altFormat : w.config.dateFormat), t.blur(); w.open() } else if (V(t) || i || o) { var r = !!w.timeContainer && w.timeContainer.contains(t); switch (e.keyCode) { case 13: r ? (e.preventDefault(), I(), se()) : ue(e); break; case 27: e.preventDefault(), se(); break; case 8: case 46: n && !w.config.allowInput && (e.preventDefault(), w.clear()); break; case 37: case 39: if (r || n) w.hourElement && w.hourElement.focus(); else if (e.preventDefault(), void 0 !== w.daysContainer && (!1 === a || document.activeElement && ee(document.activeElement))) { var l = 39 === e.keyCode ? 1 : -1; e.ctrlKey ? (e.stopPropagation(), G(l), R(W(1), 0)) : R(void 0, l) } break; case 38: case 40: e.preventDefault(); var c = 40 === e.keyCode ? 1 : -1; w.daysContainer && void 0 !== t.$i || t === w.input || t === w.altInput ? e.ctrlKey ? (e.stopPropagation(), Q(w.currentYear - c), R(W(1), 0)) : r || R(void 0, 7 * c) : t === w.currentYearElement ? Q(w.currentYear - c) : w.config.enableTime && (!r && w.hourElement && w.hourElement.focus(), I(e), w._debouncedChange()); break; case 9: if (r) { var d = [w.hourElement, w.minuteElement, w.secondElement, w.amPM].concat(w.pluginElements).filter((function (e) { return e })), s = d.indexOf(t); if (-1 !== s) { var u = d[s + (e.shiftKey ? -1 : 1)]; e.preventDefault(), (u || w._input).focus() } } else !w.config.noCalendar && w.daysContainer && w.daysContainer.contains(t) && e.shiftKey && (e.preventDefault(), w._input.focus()) } } if (void 0 !== w.amPM && t === w.amPM) switch (e.key) { case w.l10n.amPM[0].charAt(0): case w.l10n.amPM[0].charAt(0).toLowerCase(): w.amPM.textContent = w.l10n.amPM[0], S(), be(); break; case w.l10n.amPM[1].charAt(0): case w.l10n.amPM[1].charAt(0).toLowerCase(): w.amPM.textContent = w.l10n.amPM[1], S(), be() }(n || V(t)) && pe("onKeyDown", e) } function ae(e) { if (1 === w.selectedDates.length && (!e || e.classList.contains("flatpickr-day") && !e.classList.contains("flatpickr-disabled"))) { for (var t = e ? e.dateObj.getTime() : w.days.firstElementChild.dateObj.getTime(), n = w.parseDate(w.selectedDates[0], void 0, !0).getTime(), a = Math.min(t, w.selectedDates[0].getTime()), i = Math.max(t, w.selectedDates[0].getTime()), o = !1, r = 0, l = 0, c = a; c < i; c += y)X(new Date(c), !0) || (o = o || c > a && c < i, c < n && (!r || c > r) ? r = c : c > n && (!l || c < l) && (l = c)); for (var d = 0; d < w.config.showMonths; d++)for (var s = w.daysContainer.children[d], u = function (a, i) { var c, d, u, f = s.children[a], m = f.dateObj.getTime(), g = r > 0 && m < r || l > 0 && m > l; return g ? (f.classList.add("notAllowed"), ["inRange", "startRange", "endRange"].forEach((function (e) { f.classList.remove(e) })), "continue") : o && !g ? "continue" : (["startRange", "inRange", "endRange", "notAllowed"].forEach((function (e) { f.classList.remove(e) })), void (void 0 !== e && (e.classList.add(t <= w.selectedDates[0].getTime() ? "startRange" : "endRange"), n < t && m === n ? f.classList.add("startRange") : n > t && m === n && f.classList.add("endRange"), m >= r && (0 === l || m <= l) && (d = n, u = t, (c = m) > Math.min(d, u) && c < Math.max(d, u)) && f.classList.add("inRange")))) }, f = 0, m = s.children.length; f < m; f++)u(f) } } function ie() { !w.isOpen || w.config.static || w.config.inline || ce() } function oe(e) { return function (t) { var n = w.config["_" + e + "Date"] = w.parseDate(t, w.config.dateFormat), a = w.config["_" + ("min" === e ? "max" : "min") + "Date"]; void 0 !== n && (w["min" === e ? "minDateHasTime" : "maxDateHasTime"] = n.getHours() > 0 || n.getMinutes() > 0 || n.getSeconds() > 0), w.selectedDates && (w.selectedDates = w.selectedDates.filter((function (e) { return X(e) })), w.selectedDates.length || "min" !== e || _(n), be()), w.daysContainer && (de(), void 0 !== n ? w.currentYearElement[e] = n.getFullYear().toString() : w.currentYearElement.removeAttribute(e), w.currentYearElement.disabled = !!a && void 0 !== n && a.getFullYear() === n.getFullYear()) } } function re() { return w.config.wrap ? p.querySelector("[data-input]") : p } function le() { "object" != typeof w.config.locale && void 0 === T.l10ns[w.config.locale] && w.config.errorHandler(new Error("flatpickr: invalid locale " + w.config.locale)), w.l10n = e(e({}, T.l10ns.default), "object" == typeof w.config.locale ? w.config.locale : "default" !== w.config.locale ? T.l10ns[w.config.locale] : void 0), D.K = "(" + w.l10n.amPM[0] + "|" + w.l10n.amPM[1] + "|" + w.l10n.amPM[0].toLowerCase() + "|" + w.l10n.amPM[1].toLowerCase() + ")", void 0 === e(e({}, v), JSON.parse(JSON.stringify(p.dataset || {}))).time_24hr && void 0 === T.defaultConfig.time_24hr && (w.config.time_24hr = w.l10n.time_24hr), w.formatDate = b(w), w.parseDate = C({ config: w.config, l10n: w.l10n }) } function ce(e) { if ("function" != typeof w.config.position) { if (void 0 !== w.calendarContainer) { pe("onPreCalendarPosition"); var t = e || w._positionElement, n = Array.prototype.reduce.call(w.calendarContainer.children, (function (e, t) { return e + t.offsetHeight }), 0), a = w.calendarContainer.offsetWidth, i = w.config.position.split(" "), o = i[0], r = i.length > 1 ? i[1] : null, l = t.getBoundingClientRect(), c = window.innerHeight - l.bottom, s = "above" === o || "below" !== o && c < n && l.top > n, u = window.pageYOffset + l.top + (s ? -n - 2 : t.offsetHeight + 2); if (d(w.calendarContainer, "arrowTop", !s), d(w.calendarContainer, "arrowBottom", s), !w.config.inline) { var f = window.pageXOffset + l.left, m = !1, g = !1; "center" === r ? (f -= (a - l.width) / 2, m = !0) : "right" === r && (f -= a - l.width, g = !0), d(w.calendarContainer, "arrowLeft", !m && !g), d(w.calendarContainer, "arrowCenter", m), d(w.calendarContainer, "arrowRight", g); var p = window.document.body.offsetWidth - (window.pageXOffset + l.right), h = f + a > window.document.body.offsetWidth, v = p + a > window.document.body.offsetWidth; if (d(w.calendarContainer, "rightMost", h), !w.config.static) if (w.calendarContainer.style.top = u + "px", h) if (v) { var D = function () { for (var e = null, t = 0; t < document.styleSheets.length; t++) { var n = document.styleSheets[t]; try { n.cssRules } catch (e) { continue } e = n; break } return null != e ? e : (a = document.createElement("style"), document.head.appendChild(a), a.sheet); var a }(); if (void 0 === D) return; var b = window.document.body.offsetWidth, C = Math.max(0, b / 2 - a / 2), M = D.cssRules.length, y = "{left:" + l.left + "px;right:auto;}"; d(w.calendarContainer, "rightMost", !1), d(w.calendarContainer, "centerMost", !0), D.insertRule(".flatpickr-calendar.centerMost:before,.flatpickr-calendar.centerMost:after" + y, M), w.calendarContainer.style.left = C + "px", w.calendarContainer.style.right = "auto" } else w.calendarContainer.style.left = "auto", w.calendarContainer.style.right = p + "px"; else w.calendarContainer.style.left = f + "px", w.calendarContainer.style.right = "auto" } } } else w.config.position(w, e) } function de() { w.config.noCalendar || w.isMobile || (K(), De(), J()) } function se() { w._input.focus(), -1 !== window.navigator.userAgent.indexOf("MSIE") || void 0 !== navigator.msMaxTouchPoints ? setTimeout(w.close, 0) : w.close() } function ue(e) { e.preventDefault(), e.stopPropagation(); var t = f(g(e), (function (e) { return e.classList && e.classList.contains("flatpickr-day") && !e.classList.contains("flatpickr-disabled") && !e.classList.contains("notAllowed") })); if (void 0 !== t) { var n = t, a = w.latestSelectedDateObj = new Date(n.dateObj.getTime()), i = (a.getMonth() < w.currentMonth || a.getMonth() > w.currentMonth + w.config.showMonths - 1) && "range" !== w.config.mode; if (w.selectedDateElem = n, "single" === w.config.mode) w.selectedDates = [a]; else if ("multiple" === w.config.mode) { var o = ve(a); o ? w.selectedDates.splice(parseInt(o), 1) : w.selectedDates.push(a) } else "range" === w.config.mode && (2 === w.selectedDates.length && w.clear(!1, !1), w.latestSelectedDateObj = a, w.selectedDates.push(a), 0 !== M(a, w.selectedDates[0], !0) && w.selectedDates.sort((function (e, t) { return e.getTime() - t.getTime() }))); if (S(), i) { var r = w.currentYear !== a.getFullYear(); w.currentYear = a.getFullYear(), w.currentMonth = a.getMonth(), r && (pe("onYearChange"), K()), pe("onMonthChange") } if (De(), J(), be(), i || "range" === w.config.mode || 1 !== w.config.showMonths ? void 0 !== w.selectedDateElem && void 0 === w.hourElement && w.selectedDateElem && w.selectedDateElem.focus() : L(n), void 0 !== w.hourElement && void 0 !== w.hourElement && w.hourElement.focus(), w.config.closeOnSelect) { var l = "single" === w.config.mode && !w.config.enableTime, c = "range" === w.config.mode && 2 === w.selectedDates.length && !w.config.enableTime; (l || c) && se() } N() } } w.parseDate = C({ config: w.config, l10n: w.l10n }), w._handlers = [], w.pluginElements = [], w.loadedPlugins = [], w._bind = A, w._setHoursFromDate = _, w._positionCalendar = ce, w.changeMonth = G, w.changeYear = Q, w.clear = function (e, t) { void 0 === e && (e = !0); void 0 === t && (t = !0); w.input.value = "", void 0 !== w.altInput && (w.altInput.value = ""); void 0 !== w.mobileInput && (w.mobileInput.value = ""); w.selectedDates = [], w.latestSelectedDateObj = void 0, !0 === t && (w.currentYear = w._initialDate.getFullYear(), w.currentMonth = w._initialDate.getMonth()); if (!0 === w.config.enableTime) { var n = x(w.config), a = n.hours, i = n.minutes, o = n.seconds; O(a, i, o) } w.redraw(), e && pe("onChange") }, w.close = function () { w.isOpen = !1, w.isMobile || (void 0 !== w.calendarContainer && w.calendarContainer.classList.remove("open"), void 0 !== w._input && w._input.classList.remove("active")); pe("onClose") }, w._createElement = s, w.destroy = function () { void 0 !== w.config && pe("onDestroy"); for (var e = w._handlers.length; e--;)w._handlers[e].remove(); if (w._handlers = [], w.mobileInput) w.mobileInput.parentNode && w.mobileInput.parentNode.removeChild(w.mobileInput), w.mobileInput = void 0; else if (w.calendarContainer && w.calendarContainer.parentNode) if (w.config.static && w.calendarContainer.parentNode) { var t = w.calendarContainer.parentNode; if (t.lastChild && t.removeChild(t.lastChild), t.parentNode) { for (; t.firstChild;)t.parentNode.insertBefore(t.firstChild, t); t.parentNode.removeChild(t) } } else w.calendarContainer.parentNode.removeChild(w.calendarContainer); w.altInput && (w.input.type = "text", w.altInput.parentNode && w.altInput.parentNode.removeChild(w.altInput), delete w.altInput); w.input && (w.input.type = w.input._type, w.input.classList.remove("flatpickr-input"), w.input.removeAttribute("readonly"));["_showTimeInput", "latestSelectedDateObj", "_hideNextMonthArrow", "_hidePrevMonthArrow", "__hideNextMonthArrow", "__hidePrevMonthArrow", "isMobile", "isOpen", "selectedDateElem", "minDateHasTime", "maxDateHasTime", "days", "daysContainer", "_input", "_positionElement", "innerContainer", "rContainer", "monthNav", "todayDateElem", "calendarContainer", "weekdayContainer", "prevMonthNav", "nextMonthNav", "monthsDropdownContainer", "currentMonthElement", "currentYearElement", "navigationCurrentMonth", "selectedDateElem", "config"].forEach((function (e) { try { delete w[e] } catch (e) { } })) }, w.isEnabled = X, w.jumpToDate = P, w.open = function (e, t) { void 0 === t && (t = w._positionElement); if (!0 === w.isMobile) { if (e) { e.preventDefault(); var n = g(e); n && n.blur() } return void 0 !== w.mobileInput && (w.mobileInput.focus(), w.mobileInput.click()), void pe("onOpen") } if (w._input.disabled || w.config.inline) return; var a = w.isOpen; w.isOpen = !0, a || (w.calendarContainer.classList.add("open"), w._input.classList.add("active"), pe("onOpen"), ce(t)); !0 === w.config.enableTime && !0 === w.config.noCalendar && (!1 !== w.config.allowInput || void 0 !== e && w.timeContainer.contains(e.relatedTarget) || setTimeout((function () { return w.hourElement.select() }), 50)) }, w.redraw = de, w.set = function (e, t) { if (null !== e && "object" == typeof e) for (var a in Object.assign(w.config, e), e) void 0 !== fe[a] && fe[a].forEach((function (e) { return e() })); else w.config[e] = t, void 0 !== fe[e] ? fe[e].forEach((function (e) { return e() })) : n.indexOf(e) > -1 && (w.config[e] = c(t)); w.redraw(), be(!0) }, w.setDate = function (e, t, n) { void 0 === t && (t = !1); void 0 === n && (n = w.config.dateFormat); if (0 !== e && !e || e instanceof Array && 0 === e.length) return w.clear(t); me(e, n), w.latestSelectedDateObj = w.selectedDates[w.selectedDates.length - 1], w.redraw(), P(void 0, t), _(), 0 === w.selectedDates.length && w.clear(!1); be(t), t && pe("onChange") }, w.toggle = function (e) { if (!0 === w.isOpen) return w.close(); w.open(e) }; var fe = { locale: [le, z], showMonths: [q, k, $], minDate: [P], maxDate: [P], clickOpens: [function () { !0 === w.config.clickOpens ? (A(w._input, "focus", w.open), A(w._input, "click", w.open)) : (w._input.removeEventListener("focus", w.open), w._input.removeEventListener("click", w.open)) }] }; function me(e, t) { var n = []; if (e instanceof Array) n = e.map((function (e) { return w.parseDate(e, t) })); else if (e instanceof Date || "number" == typeof e) n = [w.parseDate(e, t)]; else if ("string" == typeof e) switch (w.config.mode) { case "single": case "time": n = [w.parseDate(e, t)]; break; case "multiple": n = e.split(w.config.conjunction).map((function (e) { return w.parseDate(e, t) })); break; case "range": n = e.split(w.l10n.rangeSeparator).map((function (e) { return w.parseDate(e, t) })) } else w.config.errorHandler(new Error("Invalid date supplied: " + JSON.stringify(e))); w.selectedDates = w.config.allowInvalidPreload ? n : n.filter((function (e) { return e instanceof Date && X(e, !1) })), "range" === w.config.mode && w.selectedDates.sort((function (e, t) { return e.getTime() - t.getTime() })) } function ge(e) { return e.slice().map((function (e) { return "string" == typeof e || "number" == typeof e || e instanceof Date ? w.parseDate(e, void 0, !0) : e && "object" == typeof e && e.from && e.to ? { from: w.parseDate(e.from, void 0), to: w.parseDate(e.to, void 0) } : e })).filter((function (e) { return e })) } function pe(e, t) { if (void 0 !== w.config) { var n = w.config[e]; if (void 0 !== n && n.length > 0) for (var a = 0; n[a] && a < n.length; a++)n[a](w.selectedDates, w.input.value, w, t); "onChange" === e && (w.input.dispatchEvent(he("change")), w.input.dispatchEvent(he("input"))) } } function he(e) { var t = document.createEvent("Event"); return t.initEvent(e, !0, !0), t } function ve(e) { for (var t = 0; t < w.selectedDates.length; t++)if (0 === M(w.selectedDates[t], e)) return "" + t; return !1 } function De() { w.config.noCalendar || w.isMobile || !w.monthNav || (w.yearElements.forEach((function (e, t) { var n = new Date(w.currentYear, w.currentMonth, 1); n.setMonth(w.currentMonth + t), w.config.showMonths > 1 || "static" === w.config.monthSelectorType ? w.monthElements[t].textContent = h(n.getMonth(), w.config.shorthandCurrentMonth, w.l10n) + " " : w.monthsDropdownContainer.value = n.getMonth().toString(), e.value = n.getFullYear().toString() })), w._hidePrevMonthArrow = void 0 !== w.config.minDate && (w.currentYear === w.config.minDate.getFullYear() ? w.currentMonth <= w.config.minDate.getMonth() : w.currentYear < w.config.minDate.getFullYear()), w._hideNextMonthArrow = void 0 !== w.config.maxDate && (w.currentYear === w.config.maxDate.getFullYear() ? w.currentMonth + 1 > w.config.maxDate.getMonth() : w.currentYear > w.config.maxDate.getFullYear())) } function we(e) { return w.selectedDates.map((function (t) { return w.formatDate(t, e) })).filter((function (e, t, n) { return "range" !== w.config.mode || w.config.enableTime || n.indexOf(e) === t })).join("range" !== w.config.mode ? w.config.conjunction : w.l10n.rangeSeparator) } function be(e) { void 0 === e && (e = !0), void 0 !== w.mobileInput && w.mobileFormatStr && (w.mobileInput.value = void 0 !== w.latestSelectedDateObj ? w.formatDate(w.latestSelectedDateObj, w.mobileFormatStr) : ""), w.input.value = we(w.config.dateFormat), void 0 !== w.altInput && (w.altInput.value = we(w.config.altFormat)), !1 !== e && pe("onValueUpdate") } function Ce(e) { var t = g(e), n = w.prevMonthNav.contains(t), a = w.nextMonthNav.contains(t); n || a ? G(n ? -1 : 1) : w.yearElements.indexOf(t) >= 0 ? t.select() : t.classList.contains("arrowUp") ? w.changeYear(w.currentYear + 1) : t.classList.contains("arrowDown") && w.changeYear(w.currentYear - 1) } return function () { w.element = w.input = p, w.isOpen = !1, function () { var t = ["wrap", "weekNumbers", "allowInput", "allowInvalidPreload", "clickOpens", "time_24hr", "enableTime", "noCalendar", "altInput", "shorthandCurrentMonth", "inline", "static", "enableSeconds", "disableMobile"], i = e(e({}, JSON.parse(JSON.stringify(p.dataset || {}))), v), o = {}; w.config.parseDate = i.parseDate, w.config.formatDate = i.formatDate, Object.defineProperty(w.config, "enable", { get: function () { return w.config._enable }, set: function (e) { w.config._enable = ge(e) } }), Object.defineProperty(w.config, "disable", { get: function () { return w.config._disable }, set: function (e) { w.config._disable = ge(e) } }); var r = "time" === i.mode; if (!i.dateFormat && (i.enableTime || r)) { var l = T.defaultConfig.dateFormat || a.dateFormat; o.dateFormat = i.noCalendar || r ? "H:i" + (i.enableSeconds ? ":S" : "") : l + " H:i" + (i.enableSeconds ? ":S" : "") } if (i.altInput && (i.enableTime || r) && !i.altFormat) { var d = T.defaultConfig.altFormat || a.altFormat; o.altFormat = i.noCalendar || r ? "h:i" + (i.enableSeconds ? ":S K" : " K") : d + " h:i" + (i.enableSeconds ? ":S" : "") + " K" } Object.defineProperty(w.config, "minDate", { get: function () { return w.config._minDate }, set: oe("min") }), Object.defineProperty(w.config, "maxDate", { get: function () { return w.config._maxDate }, set: oe("max") }); var s = function (e) { return function (t) { w.config["min" === e ? "_minTime" : "_maxTime"] = w.parseDate(t, "H:i:S") } }; Object.defineProperty(w.config, "minTime", { get: function () { return w.config._minTime }, set: s("min") }), Object.defineProperty(w.config, "maxTime", { get: function () { return w.config._maxTime }, set: s("max") }), "time" === i.mode && (w.config.noCalendar = !0, w.config.enableTime = !0); Object.assign(w.config, o, i); for (var u = 0; u < t.length; u++)w.config[t[u]] = !0 === w.config[t[u]] || "true" === w.config[t[u]]; n.filter((function (e) { return void 0 !== w.config[e] })).forEach((function (e) { w.config[e] = c(w.config[e] || []).map(E) })), w.isMobile = !w.config.disableMobile && !w.config.inline && "single" === w.config.mode && !w.config.disable.length && !w.config.enable && !w.config.weekNumbers && /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent); for (u = 0; u < w.config.plugins.length; u++) { var f = w.config.plugins[u](w) || {}; for (var m in f) n.indexOf(m) > -1 ? w.config[m] = c(f[m]).map(E).concat(w.config[m]) : void 0 === i[m] && (w.config[m] = f[m]) } i.altInputClass || (w.config.altInputClass = re().className + " " + w.config.altInputClass); pe("onParseConfig") }(), le(), function () { if (w.input = re(), !w.input) return void w.config.errorHandler(new Error("Invalid input element specified")); w.input._type = w.input.type, w.input.type = "text", w.input.classList.add("flatpickr-input"), w._input = w.input, w.config.altInput && (w.altInput = s(w.input.nodeName, w.config.altInputClass), w._input = w.altInput, w.altInput.placeholder = w.input.placeholder, w.altInput.disabled = w.input.disabled, w.altInput.required = w.input.required, w.altInput.tabIndex = w.input.tabIndex, w.altInput.type = "text", w.input.setAttribute("type", "hidden"), !w.config.static && w.input.parentNode && w.input.parentNode.insertBefore(w.altInput, w.input.nextSibling)); w.config.allowInput || w._input.setAttribute("readonly", "readonly"); w._positionElement = w.config.positionElement || w._input }(), function () { w.selectedDates = [], w.now = w.parseDate(w.config.now) || new Date; var e = w.config.defaultDate || ("INPUT" !== w.input.nodeName && "TEXTAREA" !== w.input.nodeName || !w.input.placeholder || w.input.value !== w.input.placeholder ? w.input.value : null); e && me(e, w.config.dateFormat); w._initialDate = w.selectedDates.length > 0 ? w.selectedDates[0] : w.config.minDate && w.config.minDate.getTime() > w.now.getTime() ? w.config.minDate : w.config.maxDate && w.config.maxDate.getTime() < w.now.getTime() ? w.config.maxDate : w.now, w.currentYear = w._initialDate.getFullYear(), w.currentMonth = w._initialDate.getMonth(), w.selectedDates.length > 0 && (w.latestSelectedDateObj = w.selectedDates[0]); void 0 !== w.config.minTime && (w.config.minTime = w.parseDate(w.config.minTime, "H:i")); void 0 !== w.config.maxTime && (w.config.maxTime = w.parseDate(w.config.maxTime, "H:i")); w.minDateHasTime = !!w.config.minDate && (w.config.minDate.getHours() > 0 || w.config.minDate.getMinutes() > 0 || w.config.minDate.getSeconds() > 0), w.maxDateHasTime = !!w.config.maxDate && (w.config.maxDate.getHours() > 0 || w.config.maxDate.getMinutes() > 0 || w.config.maxDate.getSeconds() > 0) }(), w.utils = { getDaysInMonth: function (e, t) { return void 0 === e && (e = w.currentMonth), void 0 === t && (t = w.currentYear), 1 === e && (t % 4 == 0 && t % 100 != 0 || t % 400 == 0) ? 29 : w.l10n.daysInMonth[e] } }, w.isMobile || function () { var e = window.document.createDocumentFragment(); if (w.calendarContainer = s("div", "flatpickr-calendar"), w.calendarContainer.tabIndex = -1, !w.config.noCalendar) { if (e.appendChild((w.monthNav = s("div", "flatpickr-months"), w.yearElements = [], w.monthElements = [], w.prevMonthNav = s("span", "flatpickr-prev-month"), w.prevMonthNav.innerHTML = w.config.prevArrow, w.nextMonthNav = s("span", "flatpickr-next-month"), w.nextMonthNav.innerHTML = w.config.nextArrow, q(), Object.defineProperty(w, "_hidePrevMonthArrow", { get: function () { return w.__hidePrevMonthArrow }, set: function (e) { w.__hidePrevMonthArrow !== e && (d(w.prevMonthNav, "flatpickr-disabled", e), w.__hidePrevMonthArrow = e) } }), Object.defineProperty(w, "_hideNextMonthArrow", { get: function () { return w.__hideNextMonthArrow }, set: function (e) { w.__hideNextMonthArrow !== e && (d(w.nextMonthNav, "flatpickr-disabled", e), w.__hideNextMonthArrow = e) } }), w.currentYearElement = w.yearElements[0], De(), w.monthNav)), w.innerContainer = s("div", "flatpickr-innerContainer"), w.config.weekNumbers) { var t = function () { w.calendarContainer.classList.add("hasWeeks"); var e = s("div", "flatpickr-weekwrapper"); e.appendChild(s("span", "flatpickr-weekday", w.l10n.weekAbbreviation)); var t = s("div", "flatpickr-weeks"); return e.appendChild(t), { weekWrapper: e, weekNumbers: t } }(), n = t.weekWrapper, a = t.weekNumbers; w.innerContainer.appendChild(n), w.weekNumbers = a, w.weekWrapper = n } w.rContainer = s("div", "flatpickr-rContainer"), w.rContainer.appendChild($()), w.daysContainer || (w.daysContainer = s("div", "flatpickr-days"), w.daysContainer.tabIndex = -1), J(), w.rContainer.appendChild(w.daysContainer), w.innerContainer.appendChild(w.rContainer), e.appendChild(w.innerContainer) } w.config.enableTime && e.appendChild(function () { w.calendarContainer.classList.add("hasTime"), w.config.noCalendar && w.calendarContainer.classList.add("noCalendar"); var e = x(w.config); w.timeContainer = s("div", "flatpickr-time"), w.timeContainer.tabIndex = -1; var t = s("span", "flatpickr-time-separator", ":"), n = m("flatpickr-hour", { "aria-label": w.l10n.hourAriaLabel }); w.hourElement = n.getElementsByTagName("input")[0]; var a = m("flatpickr-minute", { "aria-label": w.l10n.minuteAriaLabel }); w.minuteElement = a.getElementsByTagName("input")[0], w.hourElement.tabIndex = w.minuteElement.tabIndex = -1, w.hourElement.value = o(w.latestSelectedDateObj ? w.latestSelectedDateObj.getHours() : w.config.time_24hr ? e.hours : function (e) { switch (e % 24) { case 0: case 12: return 12; default: return e % 12 } }(e.hours)), w.minuteElement.value = o(w.latestSelectedDateObj ? w.latestSelectedDateObj.getMinutes() : e.minutes), w.hourElement.setAttribute("step", w.config.hourIncrement.toString()), w.minuteElement.setAttribute("step", w.config.minuteIncrement.toString()), w.hourElement.setAttribute("min", w.config.time_24hr ? "0" : "1"), w.hourElement.setAttribute("max", w.config.time_24hr ? "23" : "12"), w.hourElement.setAttribute("maxlength", "2"), w.minuteElement.setAttribute("min", "0"), w.minuteElement.setAttribute("max", "59"), w.minuteElement.setAttribute("maxlength", "2"), w.timeContainer.appendChild(n), w.timeContainer.appendChild(t), w.timeContainer.appendChild(a), w.config.time_24hr && w.timeContainer.classList.add("time24hr"); if (w.config.enableSeconds) { w.timeContainer.classList.add("hasSeconds"); var i = m("flatpickr-second"); w.secondElement = i.getElementsByTagName("input")[0], w.secondElement.value = o(w.latestSelectedDateObj ? w.latestSelectedDateObj.getSeconds() : e.seconds), w.secondElement.setAttribute("step", w.minuteElement.getAttribute("step")), w.secondElement.setAttribute("min", "0"), w.secondElement.setAttribute("max", "59"), w.secondElement.setAttribute("maxlength", "2"), w.timeContainer.appendChild(s("span", "flatpickr-time-separator", ":")), w.timeContainer.appendChild(i) } w.config.time_24hr || (w.amPM = s("span", "flatpickr-am-pm", w.l10n.amPM[r((w.latestSelectedDateObj ? w.hourElement.value : w.config.defaultHour) > 11)]), w.amPM.title = w.l10n.toggleTitle, w.amPM.tabIndex = -1, w.timeContainer.appendChild(w.amPM)); return w.timeContainer }()); d(w.calendarContainer, "rangeMode", "range" === w.config.mode), d(w.calendarContainer, "animate", !0 === w.config.animate), d(w.calendarContainer, "multiMonth", w.config.showMonths > 1), w.calendarContainer.appendChild(e); var i = void 0 !== w.config.appendTo && void 0 !== w.config.appendTo.nodeType; if ((w.config.inline || w.config.static) && (w.calendarContainer.classList.add(w.config.inline ? "inline" : "static"), w.config.inline && (!i && w.element.parentNode ? w.element.parentNode.insertBefore(w.calendarContainer, w._input.nextSibling) : void 0 !== w.config.appendTo && w.config.appendTo.appendChild(w.calendarContainer)), w.config.static)) { var l = s("div", "flatpickr-wrapper"); w.element.parentNode && w.element.parentNode.insertBefore(l, w.element), l.appendChild(w.element), w.altInput && l.appendChild(w.altInput), l.appendChild(w.calendarContainer) } w.config.static || w.config.inline || (void 0 !== w.config.appendTo ? w.config.appendTo : window.document.body).appendChild(w.calendarContainer) }(), function () { w.config.wrap && ["open", "close", "toggle", "clear"].forEach((function (e) { Array.prototype.forEach.call(w.element.querySelectorAll("[data-" + e + "]"), (function (t) { return A(t, "click", w[e]) })) })); if (w.isMobile) return void function () { var e = w.config.enableTime ? w.config.noCalendar ? "time" : "datetime-local" : "date"; w.mobileInput = s("input", w.input.className + " flatpickr-mobile"), w.mobileInput.tabIndex = 1, w.mobileInput.type = e, w.mobileInput.disabled = w.input.disabled, w.mobileInput.required = w.input.required, w.mobileInput.placeholder = w.input.placeholder, w.mobileFormatStr = "datetime-local" === e ? "Y-m-d\\TH:i:S" : "date" === e ? "Y-m-d" : "H:i:S", w.selectedDates.length > 0 && (w.mobileInput.defaultValue = w.mobileInput.value = w.formatDate(w.selectedDates[0], w.mobileFormatStr)); w.config.minDate && (w.mobileInput.min = w.formatDate(w.config.minDate, "Y-m-d")); w.config.maxDate && (w.mobileInput.max = w.formatDate(w.config.maxDate, "Y-m-d")); w.input.getAttribute("step") && (w.mobileInput.step = String(w.input.getAttribute("step"))); w.input.type = "hidden", void 0 !== w.altInput && (w.altInput.type = "hidden"); try { w.input.parentNode && w.input.parentNode.insertBefore(w.mobileInput, w.input.nextSibling) } catch (e) { } A(w.mobileInput, "change", (function (e) { w.setDate(g(e).value, !1, w.mobileFormatStr), pe("onChange"), pe("onClose") })) }(); var e = l(ie, 50); w._debouncedChange = l(N, 300), w.daysContainer && !/iPhone|iPad|iPod/i.test(navigator.userAgent) && A(w.daysContainer, "mouseover", (function (e) { "range" === w.config.mode && ae(g(e)) })); A(window.document.body, "keydown", ne), w.config.inline || w.config.static || A(window, "resize", e); void 0 !== window.ontouchstart ? A(window.document, "touchstart", Z) : A(window.document, "mousedown", Z); A(window.document, "focus", Z, { capture: !0 }), !0 === w.config.clickOpens && (A(w._input, "focus", w.open), A(w._input, "click", w.open)); void 0 !== w.daysContainer && (A(w.monthNav, "click", Ce), A(w.monthNav, ["keyup", "increment"], F), A(w.daysContainer, "click", ue)); if (void 0 !== w.timeContainer && void 0 !== w.minuteElement && void 0 !== w.hourElement) { var t = function (e) { return g(e).select() }; A(w.timeContainer, ["increment"], I), A(w.timeContainer, "blur", I, { capture: !0 }), A(w.timeContainer, "click", Y), A([w.hourElement, w.minuteElement], ["focus", "click"], t), void 0 !== w.secondElement && A(w.secondElement, "focus", (function () { return w.secondElement && w.secondElement.select() })), void 0 !== w.amPM && A(w.amPM, "click", (function (e) { I(e), N() })) } w.config.allowInput && A(w._input, "blur", te) }(), (w.selectedDates.length || w.config.noCalendar) && (w.config.enableTime && _(w.config.noCalendar ? w.latestSelectedDateObj : void 0), be(!1)), k(); var t = /^((?!chrome|android).)*safari/i.test(navigator.userAgent); !w.isMobile && t && ce(), pe("onReady") }(), w } function k(e, t) { for (var n = Array.prototype.slice.call(e).filter((function (e) { return e instanceof HTMLElement })), a = [], i = 0; i < n.length; i++) { var o = n[i]; try { if (null !== o.getAttribute("data-fp-omit")) continue; void 0 !== o._flatpickr && (o._flatpickr.destroy(), o._flatpickr = void 0), o._flatpickr = E(o, t || {}), a.push(o._flatpickr) } catch (e) { console.error(e) } } return 1 === a.length ? a[0] : a } "undefined" != typeof HTMLElement && "undefined" != typeof HTMLCollection && "undefined" != typeof NodeList && (HTMLCollection.prototype.flatpickr = NodeList.prototype.flatpickr = function (e) { return k(this, e) }, HTMLElement.prototype.flatpickr = function (e) { return k([this], e) }); var T = function (e, t) { return "string" == typeof e ? k(window.document.querySelectorAll(e), t) : e instanceof Node ? k([e], t) : k(e, t) }; return T.defaultConfig = {}, T.l10ns = { en: e({}, i), default: e({}, i) }, T.localize = function (t) { T.l10ns.default = e(e({}, T.l10ns.default), t) }, T.setDefaults = function (t) { T.defaultConfig = e(e({}, T.defaultConfig), t) }, T.parseDate = C({}), T.formatDate = b({}), T.compareDates = M, "undefined" != typeof jQuery && void 0 !== jQuery.fn && (jQuery.fn.flatpickr = function (e) { return k(this, e) }), Date.prototype.fp_incr = function (e) { return new Date(this.getFullYear(), this.getMonth(), this.getDate() + ("string" == typeof e ? parseInt(e, 10) : e)) }, "undefined" != typeof window && (window.flatpickr = T), T }));
!function (e, t) { "object" == typeof exports && "undefined" != typeof module ? module.exports = t() : "function" == typeof define && define.amd ? define(t) : (e = "undefined" != typeof globalThis ? globalThis : e || self).monthSelectPlugin = t() }(this, function () { "use strict"; var e = function () { return (e = Object.assign || function (e) { for (var t, n = 1, a = arguments.length; n < a; n++)for (var o in t = arguments[n]) Object.prototype.hasOwnProperty.call(t, o) && (e[o] = t[o]); return e }).apply(this, arguments) }, t = function (e, t, n) { return n.months[t ? "shorthand" : "longhand"][e] }; var n = { shorthand: !1, dateFormat: "F Y", altFormat: "F Y", theme: "light" }; return function (a) { var o = e(e({}, n), a); return function (e) { e.config.dateFormat = o.dateFormat, e.config.altFormat = o.altFormat; var n = { monthsContainer: null }; function a() { if (e.rContainer) { for (var t = e.rContainer.querySelectorAll(".flatpickr-monthSelect-month.selected"), n = 0; n < t.length; n++)t[n].classList.remove("selected"); var a = (e.selectedDates[0] || new Date).getMonth(), o = e.rContainer.querySelector(".flatpickr-monthSelect-month:nth-child(" + (a + 1) + ")"); o && o.classList.add("selected") } } function r() { var t = e.selectedDates[0]; t && ((t = new Date(t)).setFullYear(e.currentYear), e.config.minDate && t < e.config.minDate && (t = e.config.minDate), e.config.maxDate && t > e.config.maxDate && (t = e.config.maxDate), e.currentYear = t.getFullYear()), e.currentYearElement.value = String(e.currentYear), e.rContainer && e.rContainer.querySelectorAll(".flatpickr-monthSelect-month").forEach(function (t) { t.dateObj.setFullYear(e.currentYear), e.config.minDate && t.dateObj < e.config.minDate || e.config.maxDate && t.dateObj > e.config.maxDate ? t.classList.add("disabled") : t.classList.remove("disabled") }), a() } function i(t) { t.preventDefault(), t.stopPropagation(); var n = function (e) { try { return "function" == typeof e.composedPath ? e.composedPath()[0] : e.target } catch (t) { return e.target } }(t); n instanceof Element && !n.classList.contains("disabled") && (c(n.dateObj), e.close()) } function c(t) { var n = new Date(t); n.setFullYear(e.currentYear), e.setDate(n, !0), a() } var l = { 37: -1, 39: 1, 40: 3, 38: -3 }; return { onParseConfig: function () { e.config.mode = "single", e.config.enableTime = !1 }, onValueUpdate: a, onKeyDown: function (t, a, o, r) { var i = void 0 !== l[r.keyCode]; if ((i || 13 === r.keyCode) && e.rContainer && n.monthsContainer) { var s = e.rContainer.querySelector(".flatpickr-monthSelect-month.selected"), d = Array.prototype.indexOf.call(n.monthsContainer.children, document.activeElement); if (-1 === d) { var f = s || n.monthsContainer.firstElementChild; f.focus(), d = f.$i } i ? n.monthsContainer.children[(12 + d + l[r.keyCode]) % 12].focus() : 13 === r.keyCode && n.monthsContainer.contains(document.activeElement) && c(document.activeElement.dateObj) } }, onReady: [function () { e.currentMonth = 0 }, function () { if (e.rContainer && e.daysContainer && e.weekdayContainer) { e.rContainer.removeChild(e.daysContainer), e.rContainer.removeChild(e.weekdayContainer); for (var t = 0; t < e.monthElements.length; t++) { var n = e.monthElements[t]; n.parentNode && n.parentNode.removeChild(n) } } }, function () { e._bind(e.prevMonthNav, "click", function (t) { t.preventDefault(), t.stopPropagation(), e.changeYear(e.currentYear - 1), r() }), e._bind(e.nextMonthNav, "click", function (t) { t.preventDefault(), t.stopPropagation(), e.changeYear(e.currentYear + 1), r() }) }, function () { if (e.rContainer) { n.monthsContainer = e._createElement("div", "flatpickr-monthSelect-months"), n.monthsContainer.tabIndex = -1, e.calendarContainer.classList.add("flatpickr-monthSelect-theme-" + o.theme); for (var a = 0; a < 12; a++) { var r = e._createElement("span", "flatpickr-monthSelect-month"); r.dateObj = new Date(e.currentYear, a), r.$i = a, r.textContent = t(a, o.shorthand, e.l10n), r.tabIndex = -1, r.addEventListener("click", i), n.monthsContainer.appendChild(r), (e.config.minDate && r.dateObj < e.config.minDate || e.config.maxDate && r.dateObj > e.config.maxDate) && r.classList.add("disabled") } e.rContainer.appendChild(n.monthsContainer) } }, a, function () { e.loadedPlugins.push("monthSelect") }], onDestroy: function () { if (null !== n.monthsContainer) for (var e = n.monthsContainer.querySelectorAll(".flatpickr-monthSelect-month"), t = 0; t < e.length; t++)e[t].removeEventListener("click", i) } } } } });
!function (t, e) { "object" == typeof exports && "object" == typeof module ? module.exports = e() : "function" == typeof define && define.amd ? define([], e) : "object" == typeof exports ? exports.Pickr = e() : t.Pickr = e() }(self, (function () { return (() => { "use strict"; var t = { d: (e, o) => { for (var n in o) t.o(o, n) && !t.o(e, n) && Object.defineProperty(e, n, { enumerable: !0, get: o[n] }) }, o: (t, e) => Object.prototype.hasOwnProperty.call(t, e), r: t => { "undefined" != typeof Symbol && Symbol.toStringTag && Object.defineProperty(t, Symbol.toStringTag, { value: "Module" }), Object.defineProperty(t, "__esModule", { value: !0 }) } }, e = {}; t.d(e, { default: () => L }); var o = {}; function n(t, e, o, n, i = {}) { e instanceof HTMLCollection || e instanceof NodeList ? e = Array.from(e) : Array.isArray(e) || (e = [e]), Array.isArray(o) || (o = [o]); for (const s of e) for (const e of o) s[t](e, n, { capture: !1, ...i }); return Array.prototype.slice.call(arguments, 1) } t.r(o), t.d(o, { adjustableInputNumbers: () => p, createElementFromString: () => r, createFromTemplate: () => a, eventPath: () => l, off: () => s, on: () => i, resolveElement: () => c }); const i = n.bind(null, "addEventListener"), s = n.bind(null, "removeEventListener"); function r(t) { const e = document.createElement("div"); return e.innerHTML = t.trim(), e.firstElementChild } function a(t) { const e = (t, e) => { const o = t.getAttribute(e); return t.removeAttribute(e), o }, o = (t, n = {}) => { const i = e(t, ":obj"), s = e(t, ":ref"), r = i ? n[i] = {} : n; s && (n[s] = t); for (const n of Array.from(t.children)) { const t = e(n, ":arr"), i = o(n, t ? {} : r); t && (r[t] || (r[t] = [])).push(Object.keys(i).length ? i : n) } return n }; return o(r(t)) } function l(t) { let e = t.path || t.composedPath && t.composedPath(); if (e) return e; let o = t.target.parentElement; for (e = [t.target, o]; o = o.parentElement;)e.push(o); return e.push(document, window), e } function c(t) { return t instanceof Element ? t : "string" == typeof t ? t.split(/>>/g).reduce(((t, e, o, n) => (t = t.querySelector(e), o < n.length - 1 ? t.shadowRoot : t)), document) : null } function p(t, e = (t => t)) { function o(o) { const n = [.001, .01, .1][Number(o.shiftKey || 2 * o.ctrlKey)] * (o.deltaY < 0 ? 1 : -1); let i = 0, s = t.selectionStart; t.value = t.value.replace(/[\d.]+/g, ((t, o) => o <= s && o + t.length >= s ? (s = o, e(Number(t), n, i)) : (i++, t))), t.focus(), t.setSelectionRange(s, s), o.preventDefault(), t.dispatchEvent(new Event("input")) } i(t, "focus", (() => i(window, "wheel", o, { passive: !1 }))), i(t, "blur", (() => s(window, "wheel", o))) } const { min: u, max: h, floor: d, round: m } = Math; function f(t, e, o) { e /= 100, o /= 100; const n = d(t = t / 360 * 6), i = t - n, s = o * (1 - e), r = o * (1 - i * e), a = o * (1 - (1 - i) * e), l = n % 6; return [255 * [o, r, s, s, a, o][l], 255 * [a, o, o, r, s, s][l], 255 * [s, s, a, o, o, r][l]] } function v(t, e, o) { const n = (2 - (e /= 100)) * (o /= 100) / 2; return 0 !== n && (e = 1 === n ? 0 : n < .5 ? e * o / (2 * n) : e * o / (2 - 2 * n)), [t, 100 * e, 100 * n] } function b(t, e, o) { const n = u(t /= 255, e /= 255, o /= 255), i = h(t, e, o), s = i - n; let r, a; if (0 === s) r = a = 0; else { a = s / i; const n = ((i - t) / 6 + s / 2) / s, l = ((i - e) / 6 + s / 2) / s, c = ((i - o) / 6 + s / 2) / s; t === i ? r = c - l : e === i ? r = 1 / 3 + n - c : o === i && (r = 2 / 3 + l - n), r < 0 ? r += 1 : r > 1 && (r -= 1) } return [360 * r, 100 * a, 100 * i] } function y(t, e, o, n) { e /= 100, o /= 100; return [...b(255 * (1 - u(1, (t /= 100) * (1 - (n /= 100)) + n)), 255 * (1 - u(1, e * (1 - n) + n)), 255 * (1 - u(1, o * (1 - n) + n)))] } function g(t, e, o) { e /= 100; const n = 2 * (e *= (o /= 100) < .5 ? o : 1 - o) / (o + e) * 100, i = 100 * (o + e); return [t, isNaN(n) ? 0 : n, i] } function _(t) { return b(...t.match(/.{2}/g).map((t => parseInt(t, 16)))) } function w(t) { t = t.match(/^[a-zA-Z]+$/) ? function (t) { if ("black" === t.toLowerCase()) return "#000"; const e = document.createElement("canvas").getContext("2d"); return e.fillStyle = t, "#000" === e.fillStyle ? null : e.fillStyle }(t) : t; const e = { cmyk: /^cmyk[\D]+([\d.]+)[\D]+([\d.]+)[\D]+([\d.]+)[\D]+([\d.]+)/i, rgba: /^((rgba)|rgb)[\D]+([\d.]+)[\D]+([\d.]+)[\D]+([\d.]+)[\D]*?([\d.]+|$)/i, hsla: /^((hsla)|hsl)[\D]+([\d.]+)[\D]+([\d.]+)[\D]+([\d.]+)[\D]*?([\d.]+|$)/i, hsva: /^((hsva)|hsv)[\D]+([\d.]+)[\D]+([\d.]+)[\D]+([\d.]+)[\D]*?([\d.]+|$)/i, hexa: /^#?(([\dA-Fa-f]{3,4})|([\dA-Fa-f]{6})|([\dA-Fa-f]{8}))$/i }, o = t => t.map((t => /^(|\d+)\.\d+|\d+$/.test(t) ? Number(t) : void 0)); let n; t: for (const i in e) { if (!(n = e[i].exec(t))) continue; const s = t => !!n[2] == ("number" == typeof t); switch (i) { case "cmyk": { const [, t, e, s, r] = o(n); if (t > 100 || e > 100 || s > 100 || r > 100) break t; return { values: y(t, e, s, r), type: i } } case "rgba": { const [, , , t, e, r, a] = o(n); if (t > 255 || e > 255 || r > 255 || a < 0 || a > 1 || !s(a)) break t; return { values: [...b(t, e, r), a], a, type: i } } case "hexa": { let [, t] = n; 4 !== t.length && 3 !== t.length || (t = t.split("").map((t => t + t)).join("")); const e = t.substring(0, 6); let o = t.substring(6); return o = o ? parseInt(o, 16) / 255 : void 0, { values: [..._(e), o], a: o, type: i } } case "hsla": { const [, , , t, e, r, a] = o(n); if (t > 360 || e > 100 || r > 100 || a < 0 || a > 1 || !s(a)) break t; return { values: [...g(t, e, r), a], a, type: i } } case "hsva": { const [, , , t, e, r, a] = o(n); if (t > 360 || e > 100 || r > 100 || a < 0 || a > 1 || !s(a)) break t; return { values: [t, e, r, a], a, type: i } } } } return { values: null, type: null } } function A(t = 0, e = 0, o = 0, n = 1) { const i = (t, e) => (o = -1) => e(~o ? t.map((t => Number(t.toFixed(o)))) : t), s = { h: t, s: e, v: o, a: n, toHSVA() { const t = [s.h, s.s, s.v, s.a]; return t.toString = i(t, (t => `hsva(${t[0]}, ${t[1]}%, ${t[2]}%, ${s.a})`)), t }, toHSLA() { const t = [...v(s.h, s.s, s.v), s.a]; return t.toString = i(t, (t => `hsla(${t[0]}, ${t[1]}%, ${t[2]}%, ${s.a})`)), t }, toRGBA() { const t = [...f(s.h, s.s, s.v), s.a]; return t.toString = i(t, (t => `rgba(${t[0]}, ${t[1]}, ${t[2]}, ${s.a})`)), t }, toCMYK() { const t = function (t, e, o) { const n = f(t, e, o), i = n[0] / 255, s = n[1] / 255, r = n[2] / 255, a = u(1 - i, 1 - s, 1 - r); return [100 * (1 === a ? 0 : (1 - i - a) / (1 - a)), 100 * (1 === a ? 0 : (1 - s - a) / (1 - a)), 100 * (1 === a ? 0 : (1 - r - a) / (1 - a)), 100 * a] }(s.h, s.s, s.v); return t.toString = i(t, (t => `cmyk(${t[0]}%, ${t[1]}%, ${t[2]}%, ${t[3]}%)`)), t }, toHEXA() { const t = function (t, e, o) { return f(t, e, o).map((t => m(t).toString(16).padStart(2, "0"))) }(s.h, s.s, s.v), e = s.a >= 1 ? "" : Number((255 * s.a).toFixed(0)).toString(16).toUpperCase().padStart(2, "0"); return e && t.push(e), t.toString = () => `#${t.join("").toUpperCase()}`, t }, clone: () => A(s.h, s.s, s.v, s.a) }; return s } const C = t => Math.max(Math.min(t, 1), 0); function $(t) { const e = { options: Object.assign({ lock: null, onchange: () => 0, onstop: () => 0 }, t), _keyboard(t) { const { options: o } = e, { type: n, key: i } = t; if (document.activeElement === o.wrapper) { const { lock: o } = e.options, s = "ArrowUp" === i, r = "ArrowRight" === i, a = "ArrowDown" === i, l = "ArrowLeft" === i; if ("keydown" === n && (s || r || a || l)) { let n = 0, i = 0; "v" === o ? n = s || r ? 1 : -1 : "h" === o ? n = s || r ? -1 : 1 : (i = s ? -1 : a ? 1 : 0, n = l ? -1 : r ? 1 : 0), e.update(C(e.cache.x + .01 * n), C(e.cache.y + .01 * i)), t.preventDefault() } else i.startsWith("Arrow") && (e.options.onstop(), t.preventDefault()) } }, _tapstart(t) { i(document, ["mouseup", "touchend", "touchcancel"], e._tapstop), i(document, ["mousemove", "touchmove"], e._tapmove), t.cancelable && t.preventDefault(), e._tapmove(t) }, _tapmove(t) { const { options: o, cache: n } = e, { lock: i, element: s, wrapper: r } = o, a = r.getBoundingClientRect(); let l = 0, c = 0; if (t) { const e = t && t.touches && t.touches[0]; l = t ? (e || t).clientX : 0, c = t ? (e || t).clientY : 0, l < a.left ? l = a.left : l > a.left + a.width && (l = a.left + a.width), c < a.top ? c = a.top : c > a.top + a.height && (c = a.top + a.height), l -= a.left, c -= a.top } else n && (l = n.x * a.width, c = n.y * a.height); "h" !== i && (s.style.left = `calc(${l / a.width * 100}% - ${s.offsetWidth / 2}px)`), "v" !== i && (s.style.top = `calc(${c / a.height * 100}% - ${s.offsetHeight / 2}px)`), e.cache = { x: l / a.width, y: c / a.height }; const p = C(l / a.width), u = C(c / a.height); switch (i) { case "v": return o.onchange(p); case "h": return o.onchange(u); default: return o.onchange(p, u) } }, _tapstop() { e.options.onstop(), s(document, ["mouseup", "touchend", "touchcancel"], e._tapstop), s(document, ["mousemove", "touchmove"], e._tapmove) }, trigger() { e._tapmove() }, update(t = 0, o = 0) { const { left: n, top: i, width: s, height: r } = e.options.wrapper.getBoundingClientRect(); "h" === e.options.lock && (o = t), e._tapmove({ clientX: n + s * t, clientY: i + r * o }) }, destroy() { const { options: t, _tapstart: o, _keyboard: n } = e; s(document, ["keydown", "keyup"], n), s([t.wrapper, t.element], "mousedown", o), s([t.wrapper, t.element], "touchstart", o, { passive: !1 }) } }, { options: o, _tapstart: n, _keyboard: r } = e; return i([o.wrapper, o.element], "mousedown", n), i([o.wrapper, o.element], "touchstart", n, { passive: !1 }), i(document, ["keydown", "keyup"], r), e } function k(t = {}) { t = Object.assign({ onchange: () => 0, className: "", elements: [] }, t); const e = i(t.elements, "click", (e => { t.elements.forEach((o => o.classList[e.target === o ? "add" : "remove"](t.className))), t.onchange(e), e.stopPropagation() })); return { destroy: () => s(...e) } } const S = { variantFlipOrder: { start: "sme", middle: "mse", end: "ems" }, positionFlipOrder: { top: "tbrl", right: "rltb", bottom: "btrl", left: "lrbt" }, position: "bottom", margin: 8 }, O = (t, e, o) => { const { container: n, margin: i, position: s, variantFlipOrder: r, positionFlipOrder: a } = { container: document.documentElement.getBoundingClientRect(), ...S, ...o }, { left: l, top: c } = e.style; e.style.left = "0", e.style.top = "0"; const p = t.getBoundingClientRect(), u = e.getBoundingClientRect(), h = { t: p.top - u.height - i, b: p.bottom + i, r: p.right + i, l: p.left - u.width - i }, d = { vs: p.left, vm: p.left + p.width / 2 + -u.width / 2, ve: p.left + p.width - u.width, hs: p.top, hm: p.bottom - p.height / 2 - u.height / 2, he: p.bottom - u.height }, [m, f = "middle"] = s.split("-"), v = a[m], b = r[f], { top: y, left: g, bottom: _, right: w } = n; for (const t of v) { const o = "t" === t || "b" === t, n = h[t], [i, s] = o ? ["top", "left"] : ["left", "top"], [r, a] = o ? [u.height, u.width] : [u.width, u.height], [l, c] = o ? [_, w] : [w, _], [p, m] = o ? [y, g] : [g, y]; if (!(n < p || n + r > l)) for (const r of b) { const l = d[(o ? "v" : "h") + r]; if (!(l < m || l + a > c)) return e.style[s] = l - u[s] + "px", e.style[i] = n - u[i] + "px", t + r } } return e.style.left = l, e.style.top = c, null }; function E(t, e, o) { return e in t ? Object.defineProperty(t, e, { value: o, enumerable: !0, configurable: !0, writable: !0 }) : t[e] = o, t } class L { constructor(t) { E(this, "_initializingActive", !0), E(this, "_recalc", !0), E(this, "_nanopop", null), E(this, "_root", null), E(this, "_color", A()), E(this, "_lastColor", A()), E(this, "_swatchColors", []), E(this, "_setupAnimationFrame", null), E(this, "_eventListener", { init: [], save: [], hide: [], show: [], clear: [], change: [], changestop: [], cancel: [], swatchselect: [] }), this.options = t = Object.assign({ ...L.DEFAULT_OPTIONS }, t); const { swatches: e, components: o, theme: n, sliders: i, lockOpacity: s, padding: r } = t;["nano", "monolith"].includes(n) && !i && (t.sliders = "h"), o.interaction || (o.interaction = {}); const { preview: a, opacity: l, hue: c, palette: p } = o; o.opacity = !s && l, o.palette = p || a || l || c, this._preBuild(), this._buildComponents(), this._bindEvents(), this._finalBuild(), e && e.length && e.forEach((t => this.addSwatch(t))); const { button: u, app: h } = this._root; this._nanopop = ((t, e, o) => { const n = "object" != typeof t || t instanceof HTMLElement ? { reference: t, popper: e, ...o } : t; return { update(t = n) { const { reference: e, popper: o } = Object.assign(n, t); if (!o || !e) throw new Error("Popper- or reference-element missing."); return O(e, o, n) } } })(u, h, { margin: r }), u.setAttribute("role", "button"), u.setAttribute("aria-label", this._t("btn:toggle")); const d = this; this._setupAnimationFrame = requestAnimationFrame((function e() { if (!h.offsetWidth) return requestAnimationFrame(e); d.setColor(t.default), d._rePositioningPicker(), t.defaultRepresentation && (d._representation = t.defaultRepresentation, d.setColorRepresentation(d._representation)), t.showAlways && d.show(), d._initializingActive = !1, d._emit("init") })) } _preBuild() { const { options: t } = this; for (const e of ["el", "container"]) t[e] = c(t[e]); this._root = (t => { const { components: e, useAsButton: o, inline: n, appClass: i, theme: s, lockOpacity: r } = t.options, l = t => t ? "" : 'style="display:none" hidden', c = e => t._t(e), p = a(`\n      <div :ref="root" class="pickr">\n\n        ${o ? "" : '<button type="button" :ref="button" class="pcr-button"></button>'}\n\n        <div :ref="app" class="pcr-app ${i || ""}" data-theme="${s}" ${n ? 'style="position: unset"' : ""} aria-label="${c("ui:dialog")}" role="window">\n          <div class="pcr-selection" ${l(e.palette)}>\n            <div :obj="preview" class="pcr-color-preview" ${l(e.preview)}>\n              <button type="button" :ref="lastColor" class="pcr-last-color" aria-label="${c("btn:last-color")}"></button>\n              <div :ref="currentColor" class="pcr-current-color"></div>\n            </div>\n\n            <div :obj="palette" class="pcr-color-palette">\n              <div :ref="picker" class="pcr-picker"></div>\n              <div :ref="palette" class="pcr-palette" tabindex="0" aria-label="${c("aria:palette")}" role="listbox"></div>\n            </div>\n\n            <div :obj="hue" class="pcr-color-chooser" ${l(e.hue)}>\n              <div :ref="picker" class="pcr-picker"></div>\n              <div :ref="slider" class="pcr-hue pcr-slider" tabindex="0" aria-label="${c("aria:hue")}" role="slider"></div>\n            </div>\n\n            <div :obj="opacity" class="pcr-color-opacity" ${l(e.opacity)}>\n              <div :ref="picker" class="pcr-picker"></div>\n              <div :ref="slider" class="pcr-opacity pcr-slider" tabindex="0" aria-label="${c("aria:opacity")}" role="slider"></div>\n            </div>\n          </div>\n\n          <div class="pcr-swatches ${e.palette ? "" : "pcr-last"}" :ref="swatches"></div>\n\n          <div :obj="interaction" class="pcr-interaction" ${l(Object.keys(e.interaction).length)}>\n            <input :ref="result" class="pcr-result" type="text" spellcheck="false" ${l(e.interaction.input)} aria-label="${c("aria:input")}">\n\n            <input :arr="options" class="pcr-type" data-type="HEXA" value="${r ? "HEX" : "HEXA"}" type="button" ${l(e.interaction.hex)}>\n            <input :arr="options" class="pcr-type" data-type="RGBA" value="${r ? "RGB" : "RGBA"}" type="button" ${l(e.interaction.rgba)}>\n            <input :arr="options" class="pcr-type" data-type="HSLA" value="${r ? "HSL" : "HSLA"}" type="button" ${l(e.interaction.hsla)}>\n            <input :arr="options" class="pcr-type" data-type="HSVA" value="${r ? "HSV" : "HSVA"}" type="button" ${l(e.interaction.hsva)}>\n            <input :arr="options" class="pcr-type" data-type="CMYK" value="CMYK" type="button" ${l(e.interaction.cmyk)}>\n\n            <input :ref="save" class="pcr-save" value="${c("btn:save")}" type="button" ${l(e.interaction.save)} aria-label="${c("aria:btn:save")}">\n            <input :ref="cancel" class="pcr-cancel" value="${c("btn:cancel")}" type="button" ${l(e.interaction.cancel)} aria-label="${c("aria:btn:cancel")}">\n            <input :ref="clear" class="pcr-clear" value="${c("btn:clear")}" type="button" ${l(e.interaction.clear)} aria-label="${c("aria:btn:clear")}">\n          </div>\n        </div>\n      </div>\n    `), u = p.interaction; return u.options.find((t => !t.hidden && !t.classList.add("active"))), u.type = () => u.options.find((t => t.classList.contains("active"))), p })(this), t.useAsButton && (this._root.button = t.el), t.container.appendChild(this._root.root) } _finalBuild() { const t = this.options, e = this._root; if (t.container.removeChild(e.root), t.inline) { const o = t.el.parentElement; t.el.nextSibling ? o.insertBefore(e.app, t.el.nextSibling) : o.appendChild(e.app) } else t.container.appendChild(e.app); t.useAsButton ? t.inline && t.el.remove() : t.el.parentNode.replaceChild(e.root, t.el), t.disabled && this.disable(), t.comparison || (e.button.style.transition = "none", t.useAsButton || (e.preview.lastColor.style.transition = "none")), this.hide() } _buildComponents() { const t = this, e = this.options.components, o = (t.options.sliders || "v").repeat(2), [n, i] = o.match(/^[vh]+$/g) ? o : [], s = () => this._color || (this._color = this._lastColor.clone()), r = { palette: $({ element: t._root.palette.picker, wrapper: t._root.palette.palette, onstop: () => t._emit("changestop", "slider", t), onchange(o, n) { if (!e.palette) return; const i = s(), { _root: r, options: a } = t, { lastColor: l, currentColor: c } = r.preview; t._recalc && (i.s = 100 * o, i.v = 100 - 100 * n, i.v < 0 && (i.v = 0), t._updateOutput("slider")); const p = i.toRGBA().toString(0); this.element.style.background = p, this.wrapper.style.background = `\n                        linear-gradient(to top, rgba(0, 0, 0, ${i.a}), transparent),\n                        linear-gradient(to left, hsla(${i.h}, 100%, 50%, ${i.a}), rgba(255, 255, 255, ${i.a}))\n                    `, a.comparison ? a.useAsButton || t._lastColor || l.style.setProperty("--pcr-color", p) : (r.button.style.setProperty("--pcr-color", p), r.button.classList.remove("clear")); const u = i.toHEXA().toString(); for (const { el: e, color: o } of t._swatchColors) e.classList[u === o.toHEXA().toString() ? "add" : "remove"]("pcr-active"); c.style.setProperty("--pcr-color", p) } }), hue: $({ lock: "v" === i ? "h" : "v", element: t._root.hue.picker, wrapper: t._root.hue.slider, onstop: () => t._emit("changestop", "slider", t), onchange(o) { if (!e.hue || !e.palette) return; const n = s(); t._recalc && (n.h = 360 * o), this.element.style.backgroundColor = `hsl(${n.h}, 100%, 50%)`, r.palette.trigger() } }), opacity: $({ lock: "v" === n ? "h" : "v", element: t._root.opacity.picker, wrapper: t._root.opacity.slider, onstop: () => t._emit("changestop", "slider", t), onchange(o) { if (!e.opacity || !e.palette) return; const n = s(); t._recalc && (n.a = Math.round(100 * o) / 100), this.element.style.background = `rgba(0, 0, 0, ${n.a})`, r.palette.trigger() } }), selectable: k({ elements: t._root.interaction.options, className: "active", onchange(e) { t._representation = e.target.getAttribute("data-type").toUpperCase(), t._recalc && t._updateOutput("swatch") } }) }; this._components = r } _bindEvents() { const { _root: t, options: e } = this, o = [i(t.interaction.clear, "click", (() => this._clearColor())), i([t.interaction.cancel, t.preview.lastColor], "click", (() => { this.setHSVA(...(this._lastColor || this._color).toHSVA(), !0), this._emit("cancel") })), i(t.interaction.save, "click", (() => { !this.applyColor() && !e.showAlways && this.hide() })), i(t.interaction.result, ["keyup", "input"], (t => { this.setColor(t.target.value, !0) && !this._initializingActive && (this._emit("change", this._color, "input", this), this._emit("changestop", "input", this)), t.stopImmediatePropagation() })), i(t.interaction.result, ["focus", "blur"], (t => { this._recalc = "blur" === t.type, this._recalc && this._updateOutput(null) })), i([t.palette.palette, t.palette.picker, t.hue.slider, t.hue.picker, t.opacity.slider, t.opacity.picker], ["mousedown", "touchstart"], (() => this._recalc = !0), { passive: !0 })]; if (!e.showAlways) { const n = e.closeWithKey; o.push(i(t.button, "click", (() => this.isOpen() ? this.hide() : this.show())), i(document, "keyup", (t => this.isOpen() && (t.key === n || t.code === n) && this.hide())), i(document, ["touchstart", "mousedown"], (e => { this.isOpen() && !l(e).some((e => e === t.app || e === t.button)) && this.hide() }), { capture: !0 })) } if (e.adjustableNumbers) { const e = { rgba: [255, 255, 255, 1], hsva: [360, 100, 100, 1], hsla: [360, 100, 100, 1], cmyk: [100, 100, 100, 100] }; p(t.interaction.result, ((t, o, n) => { const i = e[this.getColorRepresentation().toLowerCase()]; if (i) { const e = i[n], s = t + (e >= 100 ? 1e3 * o : o); return s <= 0 ? 0 : Number((s < e ? s : e).toPrecision(3)) } return t })) } if (e.autoReposition && !e.inline) { let t = null; const n = this; o.push(i(window, ["scroll", "resize"], (() => { n.isOpen() && (e.closeOnScroll && n.hide(), null === t ? (t = setTimeout((() => t = null), 100), requestAnimationFrame((function e() { n._rePositioningPicker(), null !== t && requestAnimationFrame(e) }))) : (clearTimeout(t), t = setTimeout((() => t = null), 100))) }), { capture: !0 })) } this._eventBindings = o } _rePositioningPicker() { const { options: t } = this; if (!t.inline) { if (!this._nanopop.update({ container: document.body.getBoundingClientRect(), position: t.position })) { const t = this._root.app, e = t.getBoundingClientRect(); t.style.top = (window.innerHeight - e.height) / 2 + "px", t.style.left = (window.innerWidth - e.width) / 2 + "px" } } } _updateOutput(t) { const { _root: e, _color: o, options: n } = this; if (e.interaction.type()) { const t = `to${e.interaction.type().getAttribute("data-type")}`; e.interaction.result.value = "function" == typeof o[t] ? o[t]().toString(n.outputPrecision) : "" } !this._initializingActive && this._recalc && this._emit("change", o, t, this) } _clearColor(t = !1) { const { _root: e, options: o } = this; o.useAsButton || e.button.style.setProperty("--pcr-color", "rgba(0, 0, 0, 0.15)"), e.button.classList.add("clear"), o.showAlways || this.hide(), this._lastColor = null, this._initializingActive || t || (this._emit("save", null), this._emit("clear")) } _parseLocalColor(t) { const { values: e, type: o, a: n } = w(t), { lockOpacity: i } = this.options, s = void 0 !== n && 1 !== n; return e && 3 === e.length && (e[3] = void 0), { values: !e || i && s ? null : e, type: o } } _t(t) { return this.options.i18n[t] || L.I18N_DEFAULTS[t] } _emit(t, ...e) { this._eventListener[t].forEach((t => t(...e, this))) } on(t, e) { return this._eventListener[t].push(e), this } off(t, e) { const o = this._eventListener[t] || [], n = o.indexOf(e); return ~n && o.splice(n, 1), this } addSwatch(t) { const { values: e } = this._parseLocalColor(t); if (e) { const { _swatchColors: t, _root: o } = this, n = A(...e), s = r(`<button type="button" style="--pcr-color: ${n.toRGBA().toString(0)}" aria-label="${this._t("btn:swatch")}"/>`); return o.swatches.appendChild(s), t.push({ el: s, color: n }), this._eventBindings.push(i(s, "click", (() => { this.setHSVA(...n.toHSVA(), !0), this._emit("swatchselect", n), this._emit("change", n, "swatch", this) }))), !0 } return !1 } removeSwatch(t) { const e = this._swatchColors[t]; if (e) { const { el: o } = e; return this._root.swatches.removeChild(o), this._swatchColors.splice(t, 1), !0 } return !1 } applyColor(t = !1) { const { preview: e, button: o } = this._root, n = this._color.toRGBA().toString(0); return e.lastColor.style.setProperty("--pcr-color", n), this.options.useAsButton || o.style.setProperty("--pcr-color", n), o.classList.remove("clear"), this._lastColor = this._color.clone(), this._initializingActive || t || this._emit("save", this._color), this } destroy() { cancelAnimationFrame(this._setupAnimationFrame), this._eventBindings.forEach((t => s(...t))), Object.keys(this._components).forEach((t => this._components[t].destroy())) } destroyAndRemove() { this.destroy(); const { root: t, app: e } = this._root; t.parentElement && t.parentElement.removeChild(t), e.parentElement.removeChild(e), Object.keys(this).forEach((t => this[t] = null)) } hide() { return !!this.isOpen() && (this._root.app.classList.remove("visible"), this._emit("hide"), !0) } show() { return !this.options.disabled && !this.isOpen() && (this._root.app.classList.add("visible"), this._rePositioningPicker(), this._emit("show", this._color), this) } isOpen() { return this._root.app.classList.contains("visible") } setHSVA(t = 360, e = 0, o = 0, n = 1, i = !1) { const s = this._recalc; if (this._recalc = !1, t < 0 || t > 360 || e < 0 || e > 100 || o < 0 || o > 100 || n < 0 || n > 1) return !1; this._color = A(t, e, o, n); const { hue: r, opacity: a, palette: l } = this._components; return r.update(t / 360), a.update(n), l.update(e / 100, 1 - o / 100), i || this.applyColor(), s && this._updateOutput(), this._recalc = s, !0 } setColor(t, e = !1) { if (null === t) return this._clearColor(e), !0; const { values: o, type: n } = this._parseLocalColor(t); if (o) { const t = n.toUpperCase(), { options: i } = this._root.interaction, s = i.find((e => e.getAttribute("data-type") === t)); if (s && !s.hidden) for (const t of i) t.classList[t === s ? "add" : "remove"]("active"); return !!this.setHSVA(...o, e) && this.setColorRepresentation(t) } return !1 } setColorRepresentation(t) { return t = t.toUpperCase(), !!this._root.interaction.options.find((e => e.getAttribute("data-type").startsWith(t) && !e.click())) } getColorRepresentation() { return this._representation } getColor() { return this._color } getSelectedColor() { return this._lastColor } getRoot() { return this._root } disable() { return this.hide(), this.options.disabled = !0, this._root.button.classList.add("disabled"), this } enable() { return this.options.disabled = !1, this._root.button.classList.remove("disabled"), this } } return E(L, "utils", o), E(L, "version", "1.8.2"), E(L, "I18N_DEFAULTS", { "ui:dialog": "color picker dialog", "btn:toggle": "toggle color picker dialog", "btn:swatch": "color swatch", "btn:last-color": "use previous color", "btn:save": "Save", "btn:cancel": "Cancel", "btn:clear": "Clear", "aria:btn:save": "save and close", "aria:btn:cancel": "cancel and close", "aria:btn:clear": "clear and close", "aria:input": "color input field", "aria:palette": "color selection area", "aria:hue": "hue selection slider", "aria:opacity": "selection slider" }), E(L, "DEFAULT_OPTIONS", { appClass: null, theme: "classic", useAsButton: !1, padding: 8, disabled: !1, comparison: !0, closeOnScroll: !1, outputPrecision: 0, lockOpacity: !1, autoReposition: !0, container: "body", components: { interaction: {} }, i18n: {}, swatches: null, inline: !1, sliders: null, default: "#42445a", defaultRepresentation: null, position: "bottom-middle", adjustableNumbers: !0, showAlways: !1, closeWithKey: "Escape" }), E(L, "create", (t => new L(t))), e = e.default })() }));

Pickr.prototype.getSwatches = function () {
    return this._swatchColors.reduce((arr, swatch) => {
        arr.push(swatch.color.toRGBA().toString(0));
        return arr;
    }, []);
}

Pickr.prototype.setSwatches = function (swatches) {
    swatches = swatches || [];

    for (let i = this._swatchColors.length - 1; i > -1; i--) {
        this.removeSwatch(i);
    }

    swatches.forEach(swatch => this.addSwatch(swatch));
}
// ========================================

if (!window.blazorise) {
    window.blazorise = {};
}

window.blazorise = {
    lastClickedDocumentElement: null,

    utils: {
        getRequiredElement: (element, elementId) => {
            if (element)
                return element;

            return document.getElementById(elementId);
        }
    },

    // adds a classname to the specified element
    addClass: (element, classname) => {
        element.classList.add(classname);
    },

    // removes a classname from the specified element
    removeClass: (element, classname) => {
        if (element.classList.contains(classname)) {
            element.classList.remove(classname);
        }
    },

    // toggles a classname on the given element id
    toggleClass: (element, classname) => {
        if (element) {
            if (element.classList.contains(classname)) {
                element.classList.remove(classname);
            } else {
                element.classList.add(classname);
            }
        }
    },

    // adds a classname to the body element
    addClassToBody: (classname) => {
        blazorise.addClass(document.body, classname);
    },

    // removes a classname from the body element
    removeClassFromBody: (classname) => {
        blazorise.removeClass(document.body, classname);
    },

    // indicates if parent node has a specified classname
    parentHasClass: (element, classname) => {
        if (element && element.parentElement) {
            return element.parentElement.classList.contains(classname);
        }
        return false;
    },

    // sets the value to the element property
    setProperty: (element, property, value) => {
        if (element && property) {
            element[property] = value;
        }
    },

    getElementInfo: (element, elementId) => {
        if (!element) {
            element = document.getElementById(elementId);
        }

        if (element) {
            const position = element.getBoundingClientRect();

            return {
                boundingClientRect: {
                    x: position.x,
                    y: position.y,
                    top: position.top,
                    bottom: position.bottom,
                    left: position.left,
                    right: position.right,
                    width: position.width,
                    height: position.height
                },
                offsetTop: element.offsetTop,
                offsetLeft: element.offsetLeft,
                offsetWidth: element.offsetWidth,
                offsetHeight: element.offsetHeight,
                scrollTop: element.scrollTop,
                scrollLeft: element.scrollLeft,
                scrollWidth: element.scrollWidth,
                scrollHeight: element.scrollHeight,
                clientTop: element.clientTop,
                clientLeft: element.clientLeft,
                clientWidth: element.clientWidth,
                clientHeight: element.clientHeight
            };
        }

        return {};
    },

    setTextValue(element, value) {
        element.value = value;
    },

    hasSelectionCapabilities: (element) => {
        const nodeName = element && element.nodeName && element.nodeName.toLowerCase();

        return (
            nodeName &&
            ((nodeName === 'input' &&
                (element.type === 'text' ||
                    element.type === 'search' ||
                    element.type === 'tel' ||
                    element.type === 'url' ||
                    element.type === 'password')) ||
                nodeName === 'textarea' ||
                element.contentEditable === 'true')
        );
    },

    setCaret: (element, caret) => {
        if (window.blazorise.hasSelectionCapabilities(element)) {
            window.requestAnimationFrame(() => {
                element.selectionStart = caret;
                element.selectionEnd = caret;
            });
        }
    },

    getCaret: (element) => {
        return window.blazorise.hasSelectionCapabilities(element)
            ? element.selectionStart :
            -1;
    },

    getSelectedOptions: (elementId) => {
        const element = document.getElementById(elementId);
        const len = element.options.length;
        var opts = [], opt;

        for (var i = 0; i < len; i++) {
            opt = element.options[i];

            if (opt.selected) {
                opts.push(opt.value);
            }
        }

        return opts;
    },

    setSelectedOptions: (elementId, values) => {
        const element = document.getElementById(elementId);

        if (element && element.options) {
            const len = element.options.length;

            for (var i = 0; i < len; i++) {
                const opt = element.options[i];

                if (values && values.find(x => x !== null && x.toString() === opt.value)) {
                    opt.selected = true;
                } else {
                    opt.selected = false;
                }
            }
        }
    },

    // holds the list of components that are triggers to close other components
    closableComponents: [],

    addClosableComponent: (elementId, dotnetAdapter) => {
        window.blazorise.closableComponents.push({ elementId: elementId, dotnetAdapter: dotnetAdapter });
    },

    findClosableComponent: (elementId) => {
        for (index = 0; index < window.blazorise.closableComponents.length; ++index) {
            if (window.blazorise.closableComponents[index].elementId === elementId)
                return window.blazorise.closableComponents[index];
        }
        return null;
    },

    findClosableComponentIndex: (elementId) => {
        for (index = 0; index < window.blazorise.closableComponents.length; ++index) {
            if (window.blazorise.closableComponents[index].elementId === elementId)
                return index;
        }
        return -1;
    },

    isClosableComponent: (elementId) => {
        for (index = 0; index < window.blazorise.closableComponents.length; ++index) {
            if (window.blazorise.closableComponents[index].elementId === elementId)
                return true;
        }
        return false;
    },

    registerClosableComponent: (element, dotnetAdapter) => {
        if (element) {
            if (window.blazorise.isClosableComponent(element.id) !== true) {
                window.blazorise.addClosableComponent(element.id, dotnetAdapter);
            }
        }
    },

    unregisterClosableComponent: (element) => {
        if (element) {
            const index = window.blazorise.findClosableComponentIndex(element.id);
            if (index !== -1) {
                window.blazorise.closableComponents.splice(index, 1);
            }
        }
    },

    tryClose: (closable, targetElementId, isEscapeKey, isChildClicked) => {
        let request = new Promise((resolve, reject) => {
            closable.dotnetAdapter.invokeMethodAsync('SafeToClose', targetElementId, isEscapeKey ? 'escape' : 'leave', isChildClicked)
                .then((result) => resolve({ elementId: closable.elementId, dotnetAdapter: closable.dotnetAdapter, status: result === true ? 'ok' : 'cancelled' }))
                .catch(() => resolve({ elementId: closable.elementId, status: 'error' }));
        });

        if (request) {
            request
                .then((response) => {
                    if (response.status === 'ok') {
                        response.dotnetAdapter.invokeMethodAsync('Close', isEscapeKey ? 'escape' : 'leave')
                            // If the user navigates to another page then it will raise exception because the reference to the component cannot be found.
                            // In that case just remove the elementId from the list.
                            .catch(() => window.blazorise.unregisterClosableComponent(response.elementId));
                    }
                });
        }
    },
    focus: (element, elementId, scrollToElement) => {
        element = window.blazorise.utils.getRequiredElement(element, elementId);

        if (element) {
            element.focus({
                preventScroll: !scrollToElement
            });
        }
    },
    select: (element, elementId, focus) => {
        if (focus) {
            window.blazorise.focus(element, elementId, true);
        }

        element = window.blazorise.utils.getRequiredElement(element, elementId);

        if (element) {
            element.select();
        }
    },
    theme: {
        addVariable: (name, value) => {
            const themeVariablesElement = document.getElementById("b-theme-variables");

            // make sure that themeVariables element exists and that we don't have the variable already defined
            if (themeVariablesElement && themeVariablesElement.innerHTML) {
                const newVariable = "\n" + name + ": " + value + ";";

                const variableStartIndex = themeVariablesElement.innerHTML.indexOf(name + ":");

                if (variableStartIndex >= 0) {
                    const variableEndIndex = themeVariablesElement.innerHTML.indexOf(";", variableStartIndex);
                    const existingVariable = themeVariablesElement.innerHTML.substr(variableStartIndex, variableEndIndex);

                    const result = themeVariablesElement.innerHTML.replace(existingVariable, newVariable);

                    themeVariablesElement.innerHTML = result;
                }
                else {
                    const innerHTML = themeVariablesElement.innerHTML;
                    const position = innerHTML.lastIndexOf(';');

                    if (position >= 0) {
                        const result = [innerHTML.slice(0, position + 1), newVariable, innerHTML.slice(position + 1)].join('');

                        themeVariablesElement.innerHTML = result;
                    }
                }

                return;
            }

            // The fallback mechanism for custom CSS variables where we don't use theme provider
            // is to apply them to the body element
            document.body.style.setProperty(name, value);
        }
    },
    tooltip: {
        _instances: [],

        initialize: (element, elementId, options) => {
            if (!element) {
                return;
            }

            const defaultOptions = {
                theme: 'blazorise',
                content: options.text,
                placement: options.placement,
                maxWidth: options.maxWidth ? options.maxWidth : options.multiline ? "15rem" : null,
                duration: options.fade ? [options.fadeDuration, options.fadeDuration] : [0, 0],
                arrow: options.showArrow,
                allowHTML: true,
                trigger: options.trigger
            };

            const alwaysActiveOptions = options.alwaysActive
                ? {
                    showOnCreate: true,
                    hideOnClick: false,
                    trigger: "manual"
                } : {};

            const instance = tippy(element, {
                ...defaultOptions,
                ...alwaysActiveOptions
            });

            if (options.text) {
                instance.enable();
            }
            else {
                instance.disable();
            }

            window.blazorise.tooltip._instances[elementId] = instance;
        },
        destroy: (element, elementId) => {
            var instances = window.blazorise.tooltip._instances || {};

            const instance = instances[elementId];

            if (instance) {
                instance.hide();

                delete instances[elementId];
            }
        },
        updateContent: (element, elementId, content) => {
            const instance = window.blazorise.tooltip._instances[elementId];

            if (instance) {
                instance.setContent(content);

                if (content) {
                    instance.enable();
                }
                else {
                    instance.disable();
                }
            }
        }
    },
    textEdit: {
        _instances: [],

        initialize: (element, elementId, maskType, editMask) => {
            var instances = window.blazorise.textEdit._instances = window.blazorise.textEdit._instances || {};

            if (maskType === "numeric") {
                instances[elementId] = new window.blazorise.NumericMaskValidator(null, element, elementId);
            }
            else if (maskType === "datetime") {
                instances[elementId] = new window.blazorise.DateTimeMaskValidator(element, elementId);
            }
            else if (maskType === "regex") {
                instances[elementId] = new window.blazorise.RegExMaskValidator(element, elementId, editMask);
            }
            else {
                instances[elementId] = new window.blazorise.NoValidator();
            }

            element.addEventListener("keypress", (e) => {
                window.blazorise.textEdit.keyPress(instances[elementId], e);
            });

            element.addEventListener("paste", (e) => {
                window.blazorise.textEdit.paste(instances[elementId], e);
            });
        },
        destroy: (element, elementId) => {
            var instances = window.blazorise.textEdit._instances || {};
            delete instances[elementId];
        },
        keyPress: (validator, e) => {
            var currentValue = String.fromCharCode(e.which);

            return validator.isValid(currentValue) || e.preventDefault();
        },
        paste: (validator, e) => {
            return validator.isValid(e.clipboardData.getData("text/plain")) || e.preventDefault();
        }
    },
    numericEdit: {
        _instances: [],

        initialize: (dotnetAdapter, element, elementId, options) => {
            const instance = new window.blazorise.NumericMaskValidator(dotnetAdapter, element, elementId, options);

            window.blazorise.numericEdit._instances[elementId] = instance;

            element.addEventListener("keypress", (e) => {
                window.blazorise.numericEdit.keyPress(window.blazorise.numericEdit._instances[elementId], e);
            });

            element.addEventListener("paste", (e) => {
                window.blazorise.numericEdit.paste(window.blazorise.numericEdit._instances[elementId], e);
            });

            if (instance.decimals && instance.decimals !== 2) {
                instance.truncate();
            }
        },
        update: (element, elementId, options) => {
            const instance = window.blazorise.numericEdit._instances[elementId];

            if (instance) {
                instance.update(options);
            }
        },
        destroy: (element, elementId) => {
            var instances = window.blazorise.numericEdit._instances || {};
            delete instances[elementId];
        },
        keyPress: (validator, e) => {
            var currentValue = String.fromCharCode(e.which);

            return e.which === 13 // still need to allow ENTER key so that we don't preventDefault on form submit
                || validator.isValid(currentValue)
                || e.preventDefault();
        },
        paste: (validator, e) => {
            return validator.isValid(e.clipboardData.getData("text/plain")) || e.preventDefault();
        }
    },
    datePicker: {
        _pickers: [],

        initialize: (element, elementId, options) => {
            function mutationObserverCallback(mutationsList, observer) {
                mutationsList.forEach(mutation => {
                    if (mutation.attributeName === 'class') {
                        const picker = window.blazorise.datePicker._pickers[mutation.target.id];

                        if (picker && picker.altInput) {
                            const altInputClassListToRemove = [...picker.altInput.classList].filter(cn => !["input", "active"].includes(cn));
                            const inputClassListToAdd = [...picker.input.classList].filter(cn => !["flatpickr-input"].includes(cn));

                            altInputClassListToRemove.forEach(name => {
                                picker.altInput.classList.remove(name);
                            });

                            inputClassListToAdd.forEach(name => {
                                picker.altInput.classList.add(name);
                            });
                        }
                    }
                });
            }

            // When flatpickr is defined with altInput=true, it will create a second input
            // element while the original input element will be hidden. With MutationObserver
            // we can copy classnames from hidden to the visible element.
            const mutationObserver = new MutationObserver(mutationObserverCallback);
            mutationObserver.observe(document.getElementById(elementId), { attributes: true });

            const defaultOptions = {
                enableTime: options.inputMode === 1,
                dateFormat: options.inputMode === 1 ? 'Y-m-d H:i' : 'Y-m-d',
                allowInput: true,
                altInput: true,
                altFormat: options.displayFormat ? options.displayFormat : (options.inputMode === 1 ? 'Y-m-d H:i' : 'Y-m-d'),
                defaultValue: options.default,
                minDate: options.min,
                maxDate: options.max,
                locale: {
                    firstDayOfWeek: options.firstDayOfWeek
                },
                time_24hr: options.timeAs24hr ? options.timeAs24hr : false,
                clickOpens: !(options.readOnly || false),
                disable: options.disabledDates || []
            };

            const pluginOptions = options.inputMode === 2 ? {
                plugins: [new monthSelectPlugin({
                    shorthand: false,
                    dateFormat: "Y-m-d",
                    altFormat: "M Y"
                })]
            } : {};

            const picker = flatpickr(element, {
                ...defaultOptions,
                ...pluginOptions
            });

            if (options) {
                picker.altInput.disabled = options.disabled || false;
                picker.altInput.readOnly = options.readOnly || false;
            }

            window.blazorise.datePicker._pickers[elementId] = picker;
        },

        destroy: (element, elementId) => {
            const instances = window.blazorise.datePicker._pickers || {};
            delete instances[elementId];
        },

        updateValue: (element, elementId, value) => {
            const picker = window.blazorise.datePicker._pickers[elementId];

            if (picker) {
                picker.setDate(value);
            }
        },

        updateOptions: (element, elementId, options) => {
            const picker = window.blazorise.datePicker._pickers[elementId];

            if (picker) {
                if (options.firstDayOfWeek.changed) {
                    picker.set("firstDayOfWeek", options.firstDayOfWeek.value);
                }

                if (options.displayFormat.changed) {
                    picker.set("altFormat", options.displayFormat.value);
                }

                if (options.timeAs24hr.changed) {
                    picker.set("time_24hr", options.timeAs24hr.value);
                }

                if (options.min.changed) {
                    picker.set("minDate", options.min.value);
                }

                if (options.max.changed) {
                    picker.set("maxDate", options.max.value);
                }

                if (options.disabled.changed) {
                    picker.altInput.disabled = options.disabled.value;
                }

                if (options.readOnly.changed) {
                    picker.altInput.readOnly = options.readOnly.value;
                    picker.set("clickOpens", !options.readOnly.value);
                }

                if (options.disabledDates.changed) {
                    picker.set("disable", options.disabledDates.value || []);
                }
            }
        },

        open: (element, elementId) => {
            const picker = window.blazorise.datePicker._pickers[elementId];

            if (picker) {
                picker.open();
            }
        },

        close: (element, elementId) => {
            const picker = window.blazorise.datePicker._pickers[elementId];

            if (picker) {
                picker.close();
            }
        },

        toggle: (element, elementId) => {
            const picker = window.blazorise.datePicker._pickers[elementId];

            if (picker) {
                picker.toggle();
            }
        },

        focus: (element, elementId, scrollToElement) => {
            const picker = window.blazorise.datePicker._pickers[elementId];

            if (picker && picker.altInput) {
                window.blazorise.focus(picker.altInput, null, scrollToElement);
            }
        },

        select: (element, elementId, focus) => {
            const picker = window.blazorise.datePicker._pickers[elementId];

            if (picker && picker.altInput) {
                window.blazorise.select(picker.altInput, null, focus);
            }
        }
    },

    timePicker: {
        _pickers: [],

        initialize: (element, elementId, options) => {
            function mutationObserverCallback(mutationsList, observer) {
                mutationsList.forEach(mutation => {
                    if (mutation.attributeName === 'class') {
                        const picker = window.blazorise.timePicker._pickers[mutation.target.id];

                        if (picker && picker.altInput) {
                            const altInputClassListToRemove = [...picker.altInput.classList].filter(cn => !["input", "active"].includes(cn));
                            const inputClassListToAdd = [...picker.input.classList].filter(cn => !["flatpickr-input"].includes(cn));

                            altInputClassListToRemove.forEach(name => {
                                picker.altInput.classList.remove(name);
                            });

                            inputClassListToAdd.forEach(name => {
                                picker.altInput.classList.add(name);
                            });
                        }
                    }
                });
            }

            // When flatpickr is defined with altInput=true, it will create a second input
            // element while the original input element will be hidden. With MutationObserver
            // we can copy classnames from hidden to the visible element.
            const mutationObserver = new MutationObserver(mutationObserverCallback);
            mutationObserver.observe(document.getElementById(elementId), { attributes: true });

            const picker = flatpickr(element, {
                enableTime: true,
                noCalendar: true,
                dateFormat: "H:i",
                allowInput: true,
                altInput: true,
                altFormat: options.displayFormat ? options.displayFormat : "H:i",
                defaultValue: options.default,
                minTime: options.min,
                maxTime: options.max,
                time_24hr: options.timeAs24hr ? options.timeAs24hr : false,
                clickOpens: !(options.readOnly || false)
            });

            if (options) {
                picker.altInput.disabled = options.disabled || false;
                picker.altInput.readOnly = options.readOnly || false;
            }

            window.blazorise.timePicker._pickers[elementId] = picker;
        },

        destroy: (element, elementId) => {
            const instances = window.blazorise.timePicker._pickers || {};
            delete instances[elementId];
        },

        updateValue: (element, elementId, value) => {
            const picker = window.blazorise.timePicker._pickers[elementId];

            if (picker) {
                picker.setDate(value);
            }
        },

        updateOptions: (element, elementId, options) => {
            const picker = window.blazorise.timePicker._pickers[elementId];

            if (picker) {
                if (options.displayFormat.changed) {
                    picker.set("altFormat", options.displayFormat.value);
                }

                if (options.timeAs24hr.changed) {
                    picker.set("time_24hr", options.timeAs24hr.value);
                }

                if (options.min.changed) {
                    picker.set("minTime", options.min.value);
                }

                if (options.max.changed) {
                    picker.set("maxTime", options.max.value);
                }

                if (options.disabled.changed) {
                    picker.altInput.disabled = options.disabled.value;
                }

                if (options.readOnly.changed) {
                    picker.altInput.readOnly = options.readOnly.value;
                    picker.set("clickOpens", !options.readOnly.value);
                }
            }
        },

        open: (element, elementId) => {
            const picker = window.blazorise.timePicker._pickers[elementId];

            if (picker) {
                picker.open();
            }
        },

        close: (element, elementId) => {
            const picker = window.blazorise.timePicker._pickers[elementId];

            if (picker) {
                picker.close();
            }
        },

        toggle: (element, elementId) => {
            const picker = window.blazorise.timePicker._pickers[elementId];

            if (picker) {
                picker.toggle();
            }
        },

        focus: (element, elementId, scrollToElement) => {
            const picker = window.blazorise.timePicker._pickers[elementId];

            if (picker && picker.altInput) {
                window.blazorise.focus(picker.altInput, null, scrollToElement);
            }
        },

        select: (element, elementId, focus) => {
            const picker = window.blazorise.timePicker._pickers[elementId];

            if (picker && picker.altInput) {
                window.blazorise.select(picker.altInput, null, focus);
            }
        }
    },

    colorPicker: {
        _instancesInfos: [],

        initialize: (dotnetAdapter, element, elementId, options) => {
            const picker = Pickr.create({
                el: element,
                theme: 'monolith', // or 'monolith', or 'nano'

                useAsButton: element,

                comparison: false,
                default: options.default || "#000000",
                position: 'bottom-start',
                silent: true,

                swatches: options.showPalette ? options.palette : null,
                components: {
                    //palette: false,

                    // Main components
                    preview: true,
                    opacity: true,
                    hue: false,

                    // Input / output Options
                    interaction: {
                        hex: true,
                        rgba: true,
                        hsla: false,
                        hsva: false,
                        cmyk: false,
                        input: true,
                        save: false,
                        clear: options.showClearButton || true,
                        cancel: options.showCancelButton || true
                    }
                },

                // Translations, these are the default values.
                i18n: options.localization || {
                    // Strings visible in the UI
                    'ui:dialog': 'color picker dialog',
                    'btn:toggle': 'toggle color picker dialog',
                    'btn:swatch': 'color swatch',
                    'btn:last-color': 'use previous color',
                    'btn:save': 'Save',
                    'btn:cancel': 'Cancel',
                    'btn:clear': 'Clear',

                    // Strings used for aria-labels
                    'aria:btn:save': 'save and close',
                    'aria:btn:cancel': 'cancel and close',
                    'aria:btn:clear': 'clear and close',
                    'aria:input': 'color input field',
                    'aria:palette': 'color selection area',
                    'aria:hue': 'hue selection slider',
                    'aria:opacity': 'selection slider'
                }
            });

            const hexColor = options.default ? options.default : "#000000";

            const previewElement = element.querySelector(":scope > .b-input-color-picker-preview > .b-input-color-picker-curent-color");

            const instanceInfo = {
                picker: picker,
                dotnetAdapter: dotnetAdapter,
                element: element,
                elementId: elementId,
                previewElement: previewElement,
                hexColor: hexColor,
                palette: options.palette || [],
                showPalette: options.showPalette || true,
                hideAfterPaletteSelect: options.hideAfterPaletteSelect || true,
                showButtons: options.showButtons || true
            };

            window.blazorise.colorPicker.applyHexColor(instanceInfo, hexColor, true);

            let hexColorShow = picker.getColor() ? picker.getColor().toHEXA().toString() : null;

            if (options.disabled) {
                picker.disable();
            }

            picker
                .on('show', (color, instance) => {
                    hexColorShow = color ? color.toHEXA().toString() : null;
                })
                .on("cancel", (instance) => {
                    window.blazorise.colorPicker.applyHexColor(instanceInfo, hexColorShow);
                    instanceInfo.picker.setColor(hexColorShow, true);
                    instanceInfo.picker.hide()
                })
                .on("clear", (instance) => {
                    hexColorShow = null;
                    window.blazorise.colorPicker.applyHexColor(instanceInfo, null);
                })
                .on("changestop", (source, instance) => {
                    const hexColor = instance.getColor() ? instance.getColor().toHEXA().toString() : null;
                    window.blazorise.colorPicker.applyHexColor(instanceInfo, hexColor);
                })
                .on("swatchselect", (color, instance) => {
                    const hexColor = color ? color.toHEXA().toString() : null;
                    window.blazorise.colorPicker.applyHexColor(instanceInfo, hexColor);

                    if (instanceInfo.hideAfterPaletteSelect) {
                        instanceInfo.picker.hide();
                    }
                });

            window.blazorise.colorPicker._instancesInfos[elementId] = instanceInfo;
        },

        destroy: (element, elementId) => {
            const instanceInfo = window.blazorise.colorPicker._instancesInfos || {};
            delete instanceInfo[elementId];
        },

        updateValue: (element, elementId, hexColor) => {
            const instanceInfo = window.blazorise.colorPicker._instancesInfos[elementId];

            if (instanceInfo) {
                window.blazorise.colorPicker.applyHexColor(instanceInfo, hexColor);
            }
        },

        updateOptions: (element, elementId, options) => {
            const instanceInfo = window.blazorise.colorPicker._instancesInfos[elementId];

            if (instanceInfo) {
                if (options.palette.changed) {
                    instanceInfo.palette = options.palette.value || [];
                    instanceInfo.picker.setSwatches(instanceInfo.palette);
                }

                if (options.showPalette.changed) {
                    if (options.showPalette.value) {
                        instanceInfo.picker.setSwatches(instanceInfo.palette);
                    } else {
                        instanceInfo.picker.setSwatches([]);
                    }
                }

                if (options.hideAfterPaletteSelect.changed) {
                    instanceInfo.hideAfterPaletteSelect = options.hideAfterPaletteSelect.value;
                }

                if (options.disabled.changed || options.readOnly.changed) {
                    if (options.disabled.value || options.readOnly.value) {
                        instanceInfo.picker.disable();
                    } else {
                        instanceInfo.picker.enable();
                    }
                }
            }
        },

        updateLocalization: (element, elementId, localization) => {
            const instanceInfo = window.blazorise.colorPicker._instancesInfos[elementId];

            if (instanceInfo) {
                instanceInfo.picker.options.i18n = localization;

                instanceInfo.picker._root.interaction.save.value = localization["btn:save"];
                instanceInfo.picker._root.interaction.cancel.value = localization["btn:cancel"];
                instanceInfo.picker._root.interaction.clear.value = localization["btn:clear"];
            }
        },

        focus: (element, elementId, scrollToElement) => {
            const instanceInfo = window.blazorise.colorPicker._instancesInfos[elementId];

            if (instanceInfo) {
                window.blazorise.focus(picker.element, null, scrollToElement);
            }
        },

        select: (element, elementId, focus) => {
            const instanceInfo = window.blazorise.colorPicker._instancesInfos[elementId];

            if (instanceInfo) {
                window.blazorise.select(picker.element, null, focus);
            }
        },

        applyHexColor: (instanceInfo, hexColor, force = false) => {
            if (instanceInfo.hexColor !== hexColor || force) {
                instanceInfo.hexColor = hexColor;

                if (instanceInfo.previewElement) {
                    instanceInfo.previewElement.style.backgroundColor = hexColor;
                }

                if (instanceInfo.element) {
                    instanceInfo.element.setAttribute('data-color', hexColor);
                }

                if (instanceInfo.dotnetAdapter) {
                    instanceInfo.dotnetAdapter.invokeMethodAsync('SetValue', hexColor);
                }
            }
        }
    },

    NoValidator: function () {
        this.isValid = function (currentValue) {
            return true;
        };
    },
    NumericMaskValidator: function (dotnetAdapter, element, elementId, options) {
        options = options || {};

        this.dotnetAdapter = dotnetAdapter;
        this.elementId = elementId;
        this.element = element;
        this.decimals = options.decimals === null || options.decimals === undefined ? 2 : options.decimals;
        this.separator = options.separator || ".";
        this.step = options.step || 1;
        this.min = options.min;
        this.max = options.max;
        this.typeMin = options.typeMin;
        this.typeMax = options.typeMax;
        this.changeTextOnKeyPress = options.changeTextOnKeyPress || true;

        this.regex = function () {
            var sep = "\\" + this.separator,
                dec = this.decimals,
                reg = "{0," + dec + "}";

            return dec ? new RegExp("^(-)?(((\\d+(" + sep + "\\d" + reg + ")?)|(" + sep + "\\d" + reg + ")))?$") : /^(-)?(\d*)$/;
        };
        this.carret = function () {
            return [this.element.selectionStart, this.element.selectionEnd];
        };
        this.isValid = function (currentValue) {
            let value = this.element.value,
                selection = this.carret();

            if (value = value.substring(0, selection[0]) + currentValue + value.substring(selection[1]), !!this.regex().test(value)) {

                value = (value || "").replace(this.separator, ".");

                // Now that we know the number is valid we also need to make sure it can fit in the min-max range ot the TValue type.
                let number = Number(value);
                let numberOverflow = false;

                if (number > this.typeMax) {
                    number = Number(this.typeMax);

                    numberOverflow = true;
                }
                else if (number < this.typeMin) {
                    number = Number(this.typeMin);

                    numberOverflow = true;
                }

                if (numberOverflow) {
                    value = this.fromExponential(number);

                    // Update input with new value and also make sure that Blazor knows it is changed.
                    this.element.value = value;

                    // Trigger event so that Blazor can pick it up.
                    let eventName = this.changeTextOnKeyPress ? 'input' : 'change';

                    if ("createEvent" in document) {
                        let event = document.createEvent("HTMLEvents");
                        event.initEvent(eventName, false, true);
                        this.element.dispatchEvent(event);
                    }
                    else {
                        this.element.fireEvent("on" + eventName);
                    }

                    return false; // This will make it fail the validation and do the preventDefault().
                }

                return value;
            }

            return false;
        };
        this.update = function (options) {
            if (options.decimals && options.decimals.changed) {
                this.decimals = options.decimals.value;

                this.truncate();
            }
        };
        this.getExponentialParts = function (num) {
            return Array.isArray(num) ? num : String(num).split(/[eE]/);
        };
        this.isExponential = function (num) {
            const eParts = this.getExponentialParts(num);
            return !Number.isNaN(Number(eParts[1]));
        };
        this.fromExponential = function (num) {
            const eParts = this.getExponentialParts(num);
            if (!this.isExponential(eParts)) {
                return eParts[0];
            }

            const sign = eParts[0][0] === '-' ? '-' : '';
            const digits = eParts[0].replace(/^-/, '');
            const digitsParts = digits.split('.');
            const wholeDigits = digitsParts[0];
            const fractionDigits = digitsParts[1] || '';
            let e = Number(eParts[1]);

            if (e === 0) {
                return `${sign + wholeDigits}.${fractionDigits}`;
            } else if (e < 0) {
                // move dot to the left
                const countWholeAfterTransform = wholeDigits.length + e;
                if (countWholeAfterTransform > 0) {
                    // transform whole to fraction
                    const wholeDigitsAfterTransform = wholeDigits.substr(0, countWholeAfterTransform);
                    const wholeDigitsTransformedToFraction = wholeDigits.substr(countWholeAfterTransform);
                    return `${sign + wholeDigitsAfterTransform}.${wholeDigitsTransformedToFraction}${fractionDigits}`;
                } else {
                    // not enough whole digits: prepend with fractional zeros

                    // first e goes to dotted zero
                    let zeros = '0.';
                    e = countWholeAfterTransform;
                    while (e) {
                        zeros += '0';
                        e += 1;
                    }
                    return sign + zeros + wholeDigits + fractionDigits;
                }
            } else {
                // move dot to the right
                const countFractionAfterTransform = fractionDigits.length - e;
                if (countFractionAfterTransform > 0) {
                    // transform fraction to whole
                    // countTransformedFractionToWhole = e
                    const fractionDigitsAfterTransform = fractionDigits.substr(e);
                    const fractionDigitsTransformedToWhole = fractionDigits.substr(0, e);
                    return `${sign + wholeDigits + fractionDigitsTransformedToWhole}.${fractionDigitsAfterTransform}`;
                } else {
                    // not enough fractions: append whole zeros
                    let zerosCount = -countFractionAfterTransform;
                    let zeros = '';
                    while (zerosCount) {
                        zeros += '0';
                        zerosCount -= 1;
                    }
                    return sign + wholeDigits + fractionDigits + zeros;
                }
            }
        };
        this.truncate = function () {
            let value = (this.element.value || "").replace(this.separator, ".");

            if (value) {
                let number = Number(value);

                number = Math.trunc(number * Math.pow(10, this.decimals)) / Math.pow(10, this.decimals);

                let newValue = number.toString().replace(".", this.separator);

                this.element.value = newValue;

                if (this.dotnetAdapter) {
                    this.dotnetAdapter.invokeMethodAsync('SetValue', newValue);
                }
            }
        };
    },
    DateTimeMaskValidator: function (element, elementId) {
        this.elementId = elementId;
        this.element = element;
        this.regex = function () {
            return /^\d{0,4}$|^\d{4}-0?$|^\d{4}-(?:0?[1-9]|1[012])(?:-(?:0?[1-9]?|[12]\d|3[01])?)?$/;
        };
        this.carret = function () {
            return [this.element.selectionStart, this.element.selectionEnd];
        };
        this.isValid = function (currentValue) {
            var value = this.element.value,
                selection = this.carret();

            return value = value.substring(0, selection[0]) + currentValue + value.substring(selection[1]), !!this.regex().test(value);
        };
    },
    RegExMaskValidator: function (element, elementId, editMask) {
        this.elementId = elementId;
        this.element = element;
        this.editMask = editMask;
        this.regex = function () {
            return new RegExp(this.editMask);
        };
        this.carret = function () {
            return [this.element.selectionStart, this.element.selectionEnd];
        };
        this.isValid = function (currentValue) {
            var value = this.element.value,
                selection = this.carret();

            return value = value.substring(0, selection[0]) + currentValue + value.substring(selection[1]), !!this.regex().test(value);
        };
    },
    link: {
        scrollIntoView: (elementId) => {
            var element = document.getElementById(elementId);

            if (element) {
                element.scrollIntoView();
                window.location.hash = elementId;
            }
        }
    },
    fileEdit: {
        _instances: [],

        initialize: (adapter, element, elementId) => {
            var nextFileId = 0;

            // save an instance of adapter
            window.blazorise.fileEdit._instances[elementId] = new window.blazorise.FileEditInfo(adapter, element, elementId);

            element.addEventListener('change', function handleInputFileChange(event) {
                // Reduce to purely serializable data, plus build an index by ID
                element._blazorFilesById = {};
                var fileList = Array.prototype.map.call(element.files, function (file) {
                    var result = {
                        id: ++nextFileId,
                        lastModified: new Date(file.lastModified).toISOString(),
                        name: file.name,
                        size: file.size,
                        type: file.type
                    };
                    element._blazorFilesById[result.id] = result;

                    // Attach the blob data itself as a non-enumerable property so it doesn't appear in the JSON
                    Object.defineProperty(result, 'blob', { value: file });

                    return result;
                });

                adapter.invokeMethodAsync('NotifyChange', fileList).then(null, function (err) {
                    throw new Error(err);
                });
            });
        },
        destroy: (element, elementId) => {
            var instances = window.blazorise.fileEdit._instances || {};
            delete instances[elementId];
        },

        reset: (element, elementId) => {
            if (element) {
                element.value = '';

                var fileEditInfo = window.blazorise.fileEdit._instances[elementId];

                if (fileEditInfo) {
                    fileEditInfo.adapter.invokeMethodAsync('NotifyChange', []).then(null, function (err) {
                        throw new Error(err);
                    });
                }
            }
        },

        readFileData: function readFileData(element, fileEntryId, position, length) {
            var readPromise = getArrayBufferFromFileAsync(element, fileEntryId);

            return readPromise.then(function (arrayBuffer) {
                var uint8Array = new Uint8Array(arrayBuffer, position, length);
                var base64 = uint8ToBase64(uint8Array);
                return base64;
            });
        },

        ensureArrayBufferReadyForSharedMemoryInterop: function ensureArrayBufferReadyForSharedMemoryInterop(elem, fileId) {
            return getArrayBufferFromFileAsync(elem, fileId).then(function (arrayBuffer) {
                getFileById(elem, fileId).arrayBuffer = arrayBuffer;
            });
        },

        readFileDataSharedMemory: function readFileDataSharedMemory(readRequest) {
            // This uses various unsupported internal APIs. Beware that if you also use them,
            // your code could become broken by any update.
            var inputFileElementReferenceId = Blazor.platform.readStringField(readRequest, 0);
            var inputFileElement = document.querySelector('[_bl_' + inputFileElementReferenceId + ']');
            var fileId = Blazor.platform.readInt32Field(readRequest, 4);
            var sourceOffset = Blazor.platform.readUint64Field(readRequest, 8);
            var destination = Blazor.platform.readInt32Field(readRequest, 16);
            var destinationOffset = Blazor.platform.readInt32Field(readRequest, 20);
            var maxBytes = Blazor.platform.readInt32Field(readRequest, 24);

            var sourceArrayBuffer = getFileById(inputFileElement, fileId).arrayBuffer;
            var bytesToRead = Math.min(maxBytes, sourceArrayBuffer.byteLength - sourceOffset);
            var sourceUint8Array = new Uint8Array(sourceArrayBuffer, sourceOffset, bytesToRead);

            var destinationUint8Array = Blazor.platform.toUint8Array(destination);
            destinationUint8Array.set(sourceUint8Array, destinationOffset);

            return bytesToRead;
        },
        open: (element, elementId) => {
            if (!element && elementId) {
                element = document.getElementById(elementId);
            }

            if (element) {
                element.click();
            }
        }
    },

    FileEditInfo: function (adapter, element, elementId) {
        this.adapter = adapter;
        this.element = element;
        this.elementId = elementId;
    },

    breakpoint: {
        // Get the current breakpoint
        getBreakpoint: function () {
            return window.getComputedStyle(document.body, ':before').content.replace(/\"/g, '');
        },

        // holds the list of components that are triggers to breakpoint
        breakpointComponents: [],

        lastBreakpoint: null,

        addBreakpointComponent: (elementId, dotnetAdapter) => {
            window.blazorise.breakpoint.breakpointComponents.push({ elementId: elementId, dotnetAdapter: dotnetAdapter });
        },

        findBreakpointComponentIndex: (elementId) => {
            for (index = 0; index < window.blazorise.breakpoint.breakpointComponents.length; ++index) {
                if (window.blazorise.breakpoint.breakpointComponents[index].elementId === elementId)
                    return index;
            }
            return -1;
        },

        isBreakpointComponent: (elementId) => {
            for (index = 0; index < window.blazorise.breakpoint.breakpointComponents.length; ++index) {
                if (window.blazorise.breakpoint.breakpointComponents[index].elementId === elementId)
                    return true;
            }
            return false;
        },

        registerBreakpointComponent: (elementId, dotnetAdapter) => {
            if (window.blazorise.breakpoint.isBreakpointComponent(elementId) !== true) {
                window.blazorise.breakpoint.addBreakpointComponent(elementId, dotnetAdapter);
            }
        },

        unregisterBreakpointComponent: (elementId) => {
            const index = window.blazorise.breakpoint.findBreakpointComponentIndex(elementId);
            if (index !== -1) {
                window.blazorise.breakpoint.breakpointComponents.splice(index, 1);
            }
        },

        onBreakpoint: (dotnetAdapter, currentBreakpoint) => {
            dotnetAdapter.invokeMethodAsync('OnBreakpoint', currentBreakpoint);
        }
    },
    table: {
        initializeTableFixedHeader: function (element, elementId) {
            let resizeTimeout = null
            this.resizeThottler = function () {
                if (!resizeTimeout) {
                    resizeTimeout = setTimeout(function () {
                        resizeTimeout = null;
                        resizeHandler(element);
                    }.bind(this), 66);
                }
            }
            function resizeHandler(element) {
                const tableRows = element.querySelectorAll("thead tr");
                if (tableRows !== null && tableRows.length > 1) {
                    let previousRowCellHeight = 0;
                    for (let i = 0; i < tableRows.length; i++) {
                        let currentTh = tableRows[i].querySelectorAll("th");
                        currentTh.forEach(x => x.style.top = `${previousRowCellHeight}px`);
                        previousRowCellHeight += currentTh[0].offsetHeight;
                    }
                }
            }
            resizeHandler(element);
            window.addEventListener("resize", this.resizeThottler, false);
        },
        destroyTableFixedHeader: function (element, elementId) {
            if (typeof this.resizeThottler === "function") {
                window.removeEventListener("resize", this.resizeThottler);
            }
            const tableRows = element.querySelectorAll("thead tr");

            if (tableRows !== null && tableRows.length > 1) {
                for (let i = 0; i < tableRows.length; i++) {
                    let currentTh = tableRows[i].querySelectorAll("th");
                    currentTh.forEach(x => x.style.top = `${0}px`);
                }
            }
        },
        fixedHeaderScrollTableToPixels: function (element, elementId, pixels) {
            if (element !== null && element.parentElement !== null) {
                element.parentElement.scrollTop = pixels;
            }
        },
        fixedHeaderScrollTableToRow: function (element, elementId, row) {
            if (element !== null) {
                let rows = element.querySelectorAll("tr");
                let rowsLength = rows.length;

                if (rowsLength > 0 && row >= 0 && row < rowsLength) {
                    rows[row].scrollIntoView({
                        behavior: "smooth",
                        block: "start"
                    });
                }
            }
        },
        initializeResizable: function (element, elementId, mode) {
            const resizerClass = "b-table-resizer";
            const resizingClass = "b-table-resizing";
            const resizerHeaderMode = 0;
            let cols = null;

            if (element !== null) {
                cols = element.querySelectorAll('thead tr:first-child > th');
            }

            if (cols !== null) {

                const calculateTableActualHeight = function () {
                    let height = 0;
                    if (element !== null) {
                        const tableRows = element.querySelectorAll('tr');

                        tableRows.forEach(x => {
                            let firstCol = x.querySelector('th:first-child,td:first-child');
                            if (firstCol !== null) {
                                height += firstCol.offsetHeight;
                            }
                        });
                    }
                    return height;
                };

                const calculateModeHeight = () => {
                    return mode === resizerHeaderMode
                        ? element !== null
                            ? element.querySelector('tr:first-child > th:first-child').offsetHeight
                            : 0
                        : calculateTableActualHeight();
                };

                let actualHeight = calculateModeHeight();

                const createResizableColumn = function (col) {
                    if (col.querySelector(`.${resizerClass}`) !== null)
                        return;
                    // Add a resizer element to the column
                    const resizer = document.createElement('div');
                    resizer.classList.add(resizerClass);

                    // Set the height
                    resizer.style.height = `${actualHeight}px`;

                    resizer.addEventListener("click", function (e) {
                        e.preventDefault();
                        e.stopPropagation();
                    });

                    let mouseDownDate;
                    let mouseUpDate;

                    col.addEventListener('click', function (e) {
                        let resized = (mouseDownDate !== null && mouseUpDate !== null);
                        if (resized) {
                            let currentDate = new Date();

                            // Checks if mouse down was some ms ago, which means click from resizing
                            let elapsedFromMouseDown = currentDate - mouseDownDate;
                            let clickFromResize = elapsedFromMouseDown > 100;

                            // Checks if mouse up was some ms ago, which either means: 
                            // we clicked from resizing just now or 
                            // did not click from resizing and should handle click normally.
                            let elapsedFromMouseUp = currentDate - mouseUpDate;
                            let clickFromResizeJustNow = elapsedFromMouseUp < 100;

                            if (resized && clickFromResize && clickFromResizeJustNow) {
                                e.preventDefault();
                                e.stopPropagation();
                            }
                            mouseDownDate = null;
                            mouseUpDate = null;
                        }
                    });
                    col.appendChild(resizer);

                    // Track the current position of mouse
                    let x = 0;
                    let w = 0;

                    const mouseDownHandler = function (e) {
                        mouseDownDate = new Date();

                        // Get the current mouse position
                        x = e.clientX;

                        // Calculate the current width of column
                        const styles = window.getComputedStyle(col);
                        w = parseInt(styles.width, 10);

                        // Attach listeners for document's events
                        document.addEventListener('pointermove', mouseMoveHandler);
                        document.addEventListener('pointerup', mouseUpHandler);

                        resizer.classList.add(resizingClass);
                    };

                    const mouseMoveHandler = function (e) {
                        // Determine how far the mouse has been moved
                        const dx = e.clientX - x;

                        resizer.style.height = `${calculateTableActualHeight()}px`;

                        // Update the width of column
                        col.style.width = `${w + dx}px`;
                    };

                    // When user releases the mouse, remove the existing event listeners
                    const mouseUpHandler = function () {
                        mouseUpDate = new Date();

                        resizer.classList.remove(resizingClass);

                        element.querySelectorAll(`.${resizerClass}`).forEach(x => x.style.height = `${calculateModeHeight()}px`);

                        document.removeEventListener('pointermove', mouseMoveHandler);
                        document.removeEventListener('pointerup', mouseUpHandler);
                    };

                    resizer.addEventListener('pointerdown', mouseDownHandler);
                };


                [].forEach.call(cols, function (col) {
                    createResizableColumn(col);
                });
            }
        },
        destroyResizable: function (element, elementId) {
            if (element !== null) {
                element.querySelectorAll('.b-table-resizer').forEach(x => x.remove());
            }
        }
    }
};



document.addEventListener('mousedown', function handler(evt) {
    window.blazorise.lastClickedDocumentElement = evt.target;
});

document.addEventListener('mouseup', function handler(evt) {
    if (evt.button === 0 && evt.target === window.blazorise.lastClickedDocumentElement && window.blazorise.closableComponents && window.blazorise.closableComponents.length > 0) {
        const lastClosable = window.blazorise.closableComponents[window.blazorise.closableComponents.length - 1];
        if (lastClosable) {
            window.blazorise.tryClose(lastClosable, evt.target.id, false, hasParentInTree(evt.target, lastClosable.elementId));
        }
    }
});

document.addEventListener('keyup', function handler(evt) {
    if (evt.keyCode === 27 && window.blazorise.closableComponents && window.blazorise.closableComponents.length > 0) {
        const lastClosable = window.blazorise.closableComponents[window.blazorise.closableComponents.length - 1];

        if (lastClosable) {
            window.blazorise.tryClose(lastClosable, lastClosable.elementId, true, hasParentInTree(evt.target, lastClosable.elementId));
        }
    }
});

// Recalculate breakpoint on resize
window.addEventListener('resize', function () {
    if (window.blazorise.breakpoint.breakpointComponents && window.blazorise.breakpoint.breakpointComponents.length > 0) {
        var currentBreakpoint = window.blazorise.breakpoint.getBreakpoint();

        if (window.blazorise.breakpoint.lastBreakpoint !== currentBreakpoint) {
            window.blazorise.breakpoint.lastBreakpoint = currentBreakpoint;

            for (index = 0; index < window.blazorise.breakpoint.breakpointComponents.length; ++index) {
                window.blazorise.breakpoint.onBreakpoint(window.blazorise.breakpoint.breakpointComponents[index].dotnetAdapter, currentBreakpoint);
            }
        }
    }
});

// Set initial breakpoint
window.blazorise.breakpoint.lastBreakpoint = window.blazorise.breakpoint.getBreakpoint();

function getFileById(elem, fileId) {
    var file = elem._blazorFilesById[fileId];
    if (!file) {
        throw new Error('There is no file with ID ' + fileId + '. The file list may have changed');
    }

    return file;
}

function getArrayBufferFromFileAsync(elem, fileId) {
    var file = getFileById(elem, fileId);

    // On the first read, convert the FileReader into a Promise<ArrayBuffer>
    if (!file.readPromise) {
        file.readPromise = new Promise(function (resolve, reject) {
            var reader = new FileReader();
            reader.onload = function () { resolve(reader.result); };
            reader.onerror = function (err) { reject(err); };
            reader.readAsArrayBuffer(file.blob);
        });
    }

    return file.readPromise;
}

function hasParentInTree(element, parentElementId) {
    if (!element.parentElement) return false;
    if (element.parentElement.id === parentElementId) return true;
    return hasParentInTree(element.parentElement, parentElementId);
}

var uint8ToBase64 = (function () {
    // Code from https://github.com/beatgammit/base64-js/
    // License: MIT
    var lookup = [];

    var code = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';
    for (var i = 0, len = code.length; i < len; ++i) {
        lookup[i] = code[i];
    }

    function tripletToBase64(num) {
        return lookup[num >> 18 & 0x3F] +
            lookup[num >> 12 & 0x3F] +
            lookup[num >> 6 & 0x3F] +
            lookup[num & 0x3F];
    }

    function encodeChunk(uint8, start, end) {
        var tmp;
        var output = [];
        for (var i = start; i < end; i += 3) {
            tmp =
                ((uint8[i] << 16) & 0xFF0000) +
                ((uint8[i + 1] << 8) & 0xFF00) +
                (uint8[i + 2] & 0xFF);
            output.push(tripletToBase64(tmp));
        }
        return output.join('');
    }

    return function fromByteArray(uint8) {
        var tmp;
        var len = uint8.length;
        var extraBytes = len % 3; // if we have 1 byte left, pad 2 bytes
        var parts = [];
        var maxChunkLength = 16383; // must be multiple of 3

        // go through the array every three bytes, we'll deal with trailing stuff later
        for (var i = 0, len2 = len - extraBytes; i < len2; i += maxChunkLength) {
            parts.push(encodeChunk(
                uint8, i, (i + maxChunkLength) > len2 ? len2 : (i + maxChunkLength)
            ));
        }

        // pad the end with zeros, but make sure to not forget the extra bytes
        if (extraBytes === 1) {
            tmp = uint8[len - 1];
            parts.push(
                lookup[tmp >> 2] +
                lookup[(tmp << 4) & 0x3F] +
                '=='
            );
        } else if (extraBytes === 2) {
            tmp = (uint8[len - 2] << 8) + uint8[len - 1];
            parts.push(
                lookup[tmp >> 10] +
                lookup[(tmp >> 4) & 0x3F] +
                lookup[(tmp << 2) & 0x3F] +
                '='
            );
        }

        return parts.join('');
    };
})();
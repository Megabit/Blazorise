// ========================================
!function (e, t) { "object" == typeof exports && "undefined" != typeof module ? t(exports) : "function" == typeof define && define.amd ? define(["exports"], t) : t((e = "undefined" != typeof globalThis ? globalThis : e || self).Popper = {}) }(this, (function (e) { function t(e) { return { width: (e = e.getBoundingClientRect()).width, height: e.height, top: e.top, right: e.right, bottom: e.bottom, left: e.left, x: e.left, y: e.top } } function n(e) { return null == e ? window : "[object Window]" !== e.toString() ? (e = e.ownerDocument) && e.defaultView || window : e } function o(e) { return { scrollLeft: (e = n(e)).pageXOffset, scrollTop: e.pageYOffset } } function r(e) { return e instanceof n(e).Element || e instanceof Element } function i(e) { return e instanceof n(e).HTMLElement || e instanceof HTMLElement } function a(e) { return "undefined" != typeof ShadowRoot && (e instanceof n(e).ShadowRoot || e instanceof ShadowRoot) } function s(e) { return e ? (e.nodeName || "").toLowerCase() : null } function f(e) { return ((r(e) ? e.ownerDocument : e.document) || window.document).documentElement } function p(e) { return t(f(e)).left + o(e).scrollLeft } function c(e) { return n(e).getComputedStyle(e) } function l(e) { return e = c(e), /auto|scroll|overlay|hidden/.test(e.overflow + e.overflowY + e.overflowX) } function u(e, r, a) { void 0 === a && (a = !1); var c = f(r); e = t(e); var u = i(r), d = { scrollLeft: 0, scrollTop: 0 }, m = { x: 0, y: 0 }; return (u || !u && !a) && (("body" !== s(r) || l(c)) && (d = r !== n(r) && i(r) ? { scrollLeft: r.scrollLeft, scrollTop: r.scrollTop } : o(r)), i(r) ? ((m = t(r)).x += r.clientLeft, m.y += r.clientTop) : c && (m.x = p(c))), { x: e.left + d.scrollLeft - m.x, y: e.top + d.scrollTop - m.y, width: e.width, height: e.height } } function d(e) { var n = t(e), o = e.offsetWidth, r = e.offsetHeight; return 1 >= Math.abs(n.width - o) && (o = n.width), 1 >= Math.abs(n.height - r) && (r = n.height), { x: e.offsetLeft, y: e.offsetTop, width: o, height: r } } function m(e) { return "html" === s(e) ? e : e.assignedSlot || e.parentNode || (a(e) ? e.host : null) || f(e) } function h(e) { return 0 <= ["html", "body", "#document"].indexOf(s(e)) ? e.ownerDocument.body : i(e) && l(e) ? e : h(m(e)) } function v(e, t) { var o; void 0 === t && (t = []); var r = h(e); return e = r === (null == (o = e.ownerDocument) ? void 0 : o.body), o = n(r), r = e ? [o].concat(o.visualViewport || [], l(r) ? r : []) : r, t = t.concat(r), e ? t : t.concat(v(m(r))) } function g(e) { return i(e) && "fixed" !== c(e).position ? e.offsetParent : null } function y(e) { for (var t = n(e), o = g(e); o && 0 <= ["table", "td", "th"].indexOf(s(o)) && "static" === c(o).position;)o = g(o); if (o && ("html" === s(o) || "body" === s(o) && "static" === c(o).position)) return t; if (!o) e: { for (o = -1 !== navigator.userAgent.toLowerCase().indexOf("firefox"), e = m(e); i(e) && 0 > ["html", "body"].indexOf(s(e));) { var r = c(e); if ("none" !== r.transform || "none" !== r.perspective || "paint" === r.contain || -1 !== ["transform", "perspective"].indexOf(r.willChange) || o && "filter" === r.willChange || o && r.filter && "none" !== r.filter) { o = e; break e } e = e.parentNode } o = null } return o || t } function b(e) { function t(e) { o.add(e.name), [].concat(e.requires || [], e.requiresIfExists || []).forEach((function (e) { o.has(e) || (e = n.get(e)) && t(e) })), r.push(e) } var n = new Map, o = new Set, r = []; return e.forEach((function (e) { n.set(e.name, e) })), e.forEach((function (e) { o.has(e.name) || t(e) })), r } function w(e) { var t; return function () { return t || (t = new Promise((function (n) { Promise.resolve().then((function () { t = void 0, n(e()) })) }))), t } } function x(e) { return e.split("-")[0] } function O(e, t) { var n = t.getRootNode && t.getRootNode(); if (e.contains(t)) return !0; if (n && a(n)) do { if (t && e.isSameNode(t)) return !0; t = t.parentNode || t.host } while (t); return !1 } function j(e) { return Object.assign({}, e, { left: e.x, top: e.y, right: e.x + e.width, bottom: e.y + e.height }) } function E(e, r) { if ("viewport" === r) { r = n(e); var a = f(e); r = r.visualViewport; var s = a.clientWidth; a = a.clientHeight; var l = 0, u = 0; r && (s = r.width, a = r.height, /^((?!chrome|android).)*safari/i.test(navigator.userAgent) || (l = r.offsetLeft, u = r.offsetTop)), e = j(e = { width: s, height: a, x: l + p(e), y: u }) } else i(r) ? ((e = t(r)).top += r.clientTop, e.left += r.clientLeft, e.bottom = e.top + r.clientHeight, e.right = e.left + r.clientWidth, e.width = r.clientWidth, e.height = r.clientHeight, e.x = e.left, e.y = e.top) : (u = f(e), e = f(u), s = o(u), r = null == (a = u.ownerDocument) ? void 0 : a.body, a = _(e.scrollWidth, e.clientWidth, r ? r.scrollWidth : 0, r ? r.clientWidth : 0), l = _(e.scrollHeight, e.clientHeight, r ? r.scrollHeight : 0, r ? r.clientHeight : 0), u = -s.scrollLeft + p(u), s = -s.scrollTop, "rtl" === c(r || e).direction && (u += _(e.clientWidth, r ? r.clientWidth : 0) - a), e = j({ width: a, height: l, x: u, y: s })); return e } function D(e, t, n) { return t = "clippingParents" === t ? function (e) { var t = v(m(e)), n = 0 <= ["absolute", "fixed"].indexOf(c(e).position) && i(e) ? y(e) : e; return r(n) ? t.filter((function (e) { return r(e) && O(e, n) && "body" !== s(e) })) : [] }(e) : [].concat(t), (n = (n = [].concat(t, [n])).reduce((function (t, n) { return n = E(e, n), t.top = _(n.top, t.top), t.right = U(n.right, t.right), t.bottom = U(n.bottom, t.bottom), t.left = _(n.left, t.left), t }), E(e, n[0]))).width = n.right - n.left, n.height = n.bottom - n.top, n.x = n.left, n.y = n.top, n } function L(e) { return 0 <= ["top", "bottom"].indexOf(e) ? "x" : "y" } function P(e) { var t = e.reference, n = e.element, o = (e = e.placement) ? x(e) : null; e = e ? e.split("-")[1] : null; var r = t.x + t.width / 2 - n.width / 2, i = t.y + t.height / 2 - n.height / 2; switch (o) { case "top": r = { x: r, y: t.y - n.height }; break; case "bottom": r = { x: r, y: t.y + t.height }; break; case "right": r = { x: t.x + t.width, y: i }; break; case "left": r = { x: t.x - n.width, y: i }; break; default: r = { x: t.x, y: t.y } }if (null != (o = o ? L(o) : null)) switch (i = "y" === o ? "height" : "width", e) { case "start": r[o] -= t[i] / 2 - n[i] / 2; break; case "end": r[o] += t[i] / 2 - n[i] / 2 }return r } function M(e) { return Object.assign({}, { top: 0, right: 0, bottom: 0, left: 0 }, e) } function k(e, t) { return t.reduce((function (t, n) { return t[n] = e, t }), {}) } function W(e, n) { void 0 === n && (n = {}); var o = n; n = void 0 === (n = o.placement) ? e.placement : n; var i = o.boundary, a = void 0 === i ? "clippingParents" : i, s = void 0 === (i = o.rootBoundary) ? "viewport" : i; i = void 0 === (i = o.elementContext) ? "popper" : i; var p = o.altBoundary, c = void 0 !== p && p; o = M("number" != typeof (o = void 0 === (o = o.padding) ? 0 : o) ? o : k(o, C)); var l = e.elements.reference; p = e.rects.popper, a = D(r(c = e.elements[c ? "popper" === i ? "reference" : "popper" : i]) ? c : c.contextElement || f(e.elements.popper), a, s), c = P({ reference: s = t(l), element: p, strategy: "absolute", placement: n }), p = j(Object.assign({}, p, c)), s = "popper" === i ? p : s; var u = { top: a.top - s.top + o.top, bottom: s.bottom - a.bottom + o.bottom, left: a.left - s.left + o.left, right: s.right - a.right + o.right }; if (e = e.modifiersData.offset, "popper" === i && e) { var d = e[n]; Object.keys(u).forEach((function (e) { var t = 0 <= ["right", "bottom"].indexOf(e) ? 1 : -1, n = 0 <= ["top", "bottom"].indexOf(e) ? "y" : "x"; u[e] += d[n] * t })) } return u } function A() { for (var e = arguments.length, t = Array(e), n = 0; n < e; n++)t[n] = arguments[n]; return !t.some((function (e) { return !(e && "function" == typeof e.getBoundingClientRect) })) } function B(e) { void 0 === e && (e = {}); var t = e.defaultModifiers, n = void 0 === t ? [] : t, o = void 0 === (e = e.defaultOptions) ? F : e; return function (e, t, i) { function a() { f.forEach((function (e) { return e() })), f = [] } void 0 === i && (i = o); var s = { placement: "bottom", orderedModifiers: [], options: Object.assign({}, F, o), modifiersData: {}, elements: { reference: e, popper: t }, attributes: {}, styles: {} }, f = [], p = !1, c = { state: s, setOptions: function (i) { return a(), s.options = Object.assign({}, o, s.options, i), s.scrollParents = { reference: r(e) ? v(e) : e.contextElement ? v(e.contextElement) : [], popper: v(t) }, i = function (e) { var t = b(e); return I.reduce((function (e, n) { return e.concat(t.filter((function (e) { return e.phase === n }))) }), []) }(function (e) { var t = e.reduce((function (e, t) { var n = e[t.name]; return e[t.name] = n ? Object.assign({}, n, t, { options: Object.assign({}, n.options, t.options), data: Object.assign({}, n.data, t.data) }) : t, e }), {}); return Object.keys(t).map((function (e) { return t[e] })) }([].concat(n, s.options.modifiers))), s.orderedModifiers = i.filter((function (e) { return e.enabled })), s.orderedModifiers.forEach((function (e) { var t = e.name, n = e.options; n = void 0 === n ? {} : n, "function" == typeof (e = e.effect) && (t = e({ state: s, name: t, instance: c, options: n }), f.push(t || function () { })) })), c.update() }, forceUpdate: function () { if (!p) { var e = s.elements, t = e.reference; if (A(t, e = e.popper)) for (s.rects = { reference: u(t, y(e), "fixed" === s.options.strategy), popper: d(e) }, s.reset = !1, s.placement = s.options.placement, s.orderedModifiers.forEach((function (e) { return s.modifiersData[e.name] = Object.assign({}, e.data) })), t = 0; t < s.orderedModifiers.length; t++)if (!0 === s.reset) s.reset = !1, t = -1; else { var n = s.orderedModifiers[t]; e = n.fn; var o = n.options; o = void 0 === o ? {} : o, n = n.name, "function" == typeof e && (s = e({ state: s, options: o, name: n, instance: c }) || s) } } }, update: w((function () { return new Promise((function (e) { c.forceUpdate(), e(s) })) })), destroy: function () { a(), p = !0 } }; return A(e, t) ? (c.setOptions(i).then((function (e) { !p && i.onFirstUpdate && i.onFirstUpdate(e) })), c) : c } } function T(e) { var t, o = e.popper, r = e.popperRect, i = e.placement, a = e.offsets, s = e.position, p = e.gpuAcceleration, l = e.adaptive; if (!0 === (e = e.roundOffsets)) { e = a.y; var u = window.devicePixelRatio || 1; e = { x: z(z(a.x * u) / u) || 0, y: z(z(e * u) / u) || 0 } } else e = "function" == typeof e ? e(a) : a; e = void 0 === (e = (u = e).x) ? 0 : e, u = void 0 === (u = u.y) ? 0 : u; var d = a.hasOwnProperty("x"); a = a.hasOwnProperty("y"); var m, h = "left", v = "top", g = window; if (l) { var b = y(o), w = "clientHeight", x = "clientWidth"; b === n(o) && ("static" !== c(b = f(o)).position && (w = "scrollHeight", x = "scrollWidth")), "top" === i && (v = "bottom", u -= b[w] - r.height, u *= p ? 1 : -1), "left" === i && (h = "right", e -= b[x] - r.width, e *= p ? 1 : -1) } return o = Object.assign({ position: s }, l && J), p ? Object.assign({}, o, ((m = {})[v] = a ? "0" : "", m[h] = d ? "0" : "", m.transform = 2 > (g.devicePixelRatio || 1) ? "translate(" + e + "px, " + u + "px)" : "translate3d(" + e + "px, " + u + "px, 0)", m)) : Object.assign({}, o, ((t = {})[v] = a ? u + "px" : "", t[h] = d ? e + "px" : "", t.transform = "", t)) } function H(e) { return e.replace(/left|right|bottom|top/g, (function (e) { return $[e] })) } function R(e) { return e.replace(/start|end/g, (function (e) { return ee[e] })) } function S(e, t, n) { return void 0 === n && (n = { x: 0, y: 0 }), { top: e.top - t.height - n.y, right: e.right - t.width + n.x, bottom: e.bottom - t.height + n.y, left: e.left - t.width - n.x } } function q(e) { return ["top", "right", "bottom", "left"].some((function (t) { return 0 <= e[t] })) } var C = ["top", "bottom", "right", "left"], N = C.reduce((function (e, t) { return e.concat([t + "-start", t + "-end"]) }), []), V = [].concat(C, ["auto"]).reduce((function (e, t) { return e.concat([t, t + "-start", t + "-end"]) }), []), I = "beforeRead read afterRead beforeMain main afterMain beforeWrite write afterWrite".split(" "), _ = Math.max, U = Math.min, z = Math.round, F = { placement: "bottom", modifiers: [], strategy: "absolute" }, X = { passive: !0 }, Y = { name: "eventListeners", enabled: !0, phase: "write", fn: function () { }, effect: function (e) { var t = e.state, o = e.instance, r = (e = e.options).scroll, i = void 0 === r || r, a = void 0 === (e = e.resize) || e, s = n(t.elements.popper), f = [].concat(t.scrollParents.reference, t.scrollParents.popper); return i && f.forEach((function (e) { e.addEventListener("scroll", o.update, X) })), a && s.addEventListener("resize", o.update, X), function () { i && f.forEach((function (e) { e.removeEventListener("scroll", o.update, X) })), a && s.removeEventListener("resize", o.update, X) } }, data: {} }, G = { name: "popperOffsets", enabled: !0, phase: "read", fn: function (e) { var t = e.state; t.modifiersData[e.name] = P({ reference: t.rects.reference, element: t.rects.popper, strategy: "absolute", placement: t.placement }) }, data: {} }, J = { top: "auto", right: "auto", bottom: "auto", left: "auto" }, K = { name: "computeStyles", enabled: !0, phase: "beforeWrite", fn: function (e) { var t = e.state, n = e.options; e = void 0 === (e = n.gpuAcceleration) || e; var o = n.adaptive; o = void 0 === o || o, n = void 0 === (n = n.roundOffsets) || n, e = { placement: x(t.placement), popper: t.elements.popper, popperRect: t.rects.popper, gpuAcceleration: e }, null != t.modifiersData.popperOffsets && (t.styles.popper = Object.assign({}, t.styles.popper, T(Object.assign({}, e, { offsets: t.modifiersData.popperOffsets, position: t.options.strategy, adaptive: o, roundOffsets: n })))), null != t.modifiersData.arrow && (t.styles.arrow = Object.assign({}, t.styles.arrow, T(Object.assign({}, e, { offsets: t.modifiersData.arrow, position: "absolute", adaptive: !1, roundOffsets: n })))), t.attributes.popper = Object.assign({}, t.attributes.popper, { "data-popper-placement": t.placement }) }, data: {} }, Q = { name: "applyStyles", enabled: !0, phase: "write", fn: function (e) { var t = e.state; Object.keys(t.elements).forEach((function (e) { var n = t.styles[e] || {}, o = t.attributes[e] || {}, r = t.elements[e]; i(r) && s(r) && (Object.assign(r.style, n), Object.keys(o).forEach((function (e) { var t = o[e]; !1 === t ? r.removeAttribute(e) : r.setAttribute(e, !0 === t ? "" : t) }))) })) }, effect: function (e) { var t = e.state, n = { popper: { position: t.options.strategy, left: "0", top: "0", margin: "0" }, arrow: { position: "absolute" }, reference: {} }; return Object.assign(t.elements.popper.style, n.popper), t.styles = n, t.elements.arrow && Object.assign(t.elements.arrow.style, n.arrow), function () { Object.keys(t.elements).forEach((function (e) { var o = t.elements[e], r = t.attributes[e] || {}; e = Object.keys(t.styles.hasOwnProperty(e) ? t.styles[e] : n[e]).reduce((function (e, t) { return e[t] = "", e }), {}), i(o) && s(o) && (Object.assign(o.style, e), Object.keys(r).forEach((function (e) { o.removeAttribute(e) }))) })) } }, requires: ["computeStyles"] }, Z = { name: "offset", enabled: !0, phase: "main", requires: ["popperOffsets"], fn: function (e) { var t = e.state, n = e.name, o = void 0 === (e = e.options.offset) ? [0, 0] : e, r = (e = V.reduce((function (e, n) { var r = t.rects, i = x(n), a = 0 <= ["left", "top"].indexOf(i) ? -1 : 1, s = "function" == typeof o ? o(Object.assign({}, r, { placement: n })) : o; return r = (r = s[0]) || 0, s = ((s = s[1]) || 0) * a, i = 0 <= ["left", "right"].indexOf(i) ? { x: s, y: r } : { x: r, y: s }, e[n] = i, e }), {}))[t.placement], i = r.x; r = r.y, null != t.modifiersData.popperOffsets && (t.modifiersData.popperOffsets.x += i, t.modifiersData.popperOffsets.y += r), t.modifiersData[n] = e } }, $ = { left: "right", right: "left", bottom: "top", top: "bottom" }, ee = { start: "end", end: "start" }, te = { name: "flip", enabled: !0, phase: "main", fn: function (e) { var t = e.state, n = e.options; if (e = e.name, !t.modifiersData[e]._skip) { var o = n.mainAxis; o = void 0 === o || o; var r = n.altAxis; r = void 0 === r || r; var i = n.fallbackPlacements, a = n.padding, s = n.boundary, f = n.rootBoundary, p = n.altBoundary, c = n.flipVariations, l = void 0 === c || c, u = n.allowedAutoPlacements; c = x(n = t.options.placement), i = i || (c !== n && l ? function (e) { if ("auto" === x(e)) return []; var t = H(e); return [R(e), t, R(t)] }(n) : [H(n)]); var d = [n].concat(i).reduce((function (e, n) { return e.concat("auto" === x(n) ? function (e, t) { void 0 === t && (t = {}); var n = t.boundary, o = t.rootBoundary, r = t.padding, i = t.flipVariations, a = t.allowedAutoPlacements, s = void 0 === a ? V : a, f = t.placement.split("-")[1]; 0 === (i = (t = f ? i ? N : N.filter((function (e) { return e.split("-")[1] === f })) : C).filter((function (e) { return 0 <= s.indexOf(e) }))).length && (i = t); var p = i.reduce((function (t, i) { return t[i] = W(e, { placement: i, boundary: n, rootBoundary: o, padding: r })[x(i)], t }), {}); return Object.keys(p).sort((function (e, t) { return p[e] - p[t] })) }(t, { placement: n, boundary: s, rootBoundary: f, padding: a, flipVariations: l, allowedAutoPlacements: u }) : n) }), []); n = t.rects.reference, i = t.rects.popper; var m = new Map; c = !0; for (var h = d[0], v = 0; v < d.length; v++) { var g = d[v], y = x(g), b = "start" === g.split("-")[1], w = 0 <= ["top", "bottom"].indexOf(y), O = w ? "width" : "height", j = W(t, { placement: g, boundary: s, rootBoundary: f, altBoundary: p, padding: a }); if (b = w ? b ? "right" : "left" : b ? "bottom" : "top", n[O] > i[O] && (b = H(b)), O = H(b), w = [], o && w.push(0 >= j[y]), r && w.push(0 >= j[b], 0 >= j[O]), w.every((function (e) { return e }))) { h = g, c = !1; break } m.set(g, w) } if (c) for (o = function (e) { var t = d.find((function (t) { if (t = m.get(t)) return t.slice(0, e).every((function (e) { return e })) })); if (t) return h = t, "break" }, r = l ? 3 : 1; 0 < r && "break" !== o(r); r--); t.placement !== h && (t.modifiersData[e]._skip = !0, t.placement = h, t.reset = !0) } }, requiresIfExists: ["offset"], data: { _skip: !1 } }, ne = { name: "preventOverflow", enabled: !0, phase: "main", fn: function (e) { var t = e.state, n = e.options; e = e.name; var o = n.mainAxis, r = void 0 === o || o, i = void 0 !== (o = n.altAxis) && o; o = void 0 === (o = n.tether) || o; var a = n.tetherOffset, s = void 0 === a ? 0 : a, f = W(t, { boundary: n.boundary, rootBoundary: n.rootBoundary, padding: n.padding, altBoundary: n.altBoundary }); n = x(t.placement); var p = t.placement.split("-")[1], c = !p, l = L(n); n = "x" === l ? "y" : "x", a = t.modifiersData.popperOffsets; var u = t.rects.reference, m = t.rects.popper, h = "function" == typeof s ? s(Object.assign({}, t.rects, { placement: t.placement })) : s; if (s = { x: 0, y: 0 }, a) { if (r || i) { var v = "y" === l ? "top" : "left", g = "y" === l ? "bottom" : "right", b = "y" === l ? "height" : "width", w = a[l], O = a[l] + f[v], j = a[l] - f[g], E = o ? -m[b] / 2 : 0, D = "start" === p ? u[b] : m[b]; p = "start" === p ? -m[b] : -u[b], m = t.elements.arrow, m = o && m ? d(m) : { width: 0, height: 0 }; var P = t.modifiersData["arrow#persistent"] ? t.modifiersData["arrow#persistent"].padding : { top: 0, right: 0, bottom: 0, left: 0 }; v = P[v], g = P[g], m = _(0, U(u[b], m[b])), D = c ? u[b] / 2 - E - m - v - h : D - m - v - h, u = c ? -u[b] / 2 + E + m + g + h : p + m + g + h, c = t.elements.arrow && y(t.elements.arrow), h = t.modifiersData.offset ? t.modifiersData.offset[t.placement][l] : 0, c = a[l] + D - h - (c ? "y" === l ? c.clientTop || 0 : c.clientLeft || 0 : 0), u = a[l] + u - h, r && (r = o ? U(O, c) : O, j = o ? _(j, u) : j, r = _(r, U(w, j)), a[l] = r, s[l] = r - w), i && (r = (i = a[n]) + f["x" === l ? "top" : "left"], f = i - f["x" === l ? "bottom" : "right"], r = o ? U(r, c) : r, o = o ? _(f, u) : f, o = _(r, U(i, o)), a[n] = o, s[n] = o - i) } t.modifiersData[e] = s } }, requiresIfExists: ["offset"] }, oe = { name: "arrow", enabled: !0, phase: "main", fn: function (e) { var t, n = e.state, o = e.name, r = e.options, i = n.elements.arrow, a = n.modifiersData.popperOffsets, s = x(n.placement); if (e = L(s), s = 0 <= ["left", "right"].indexOf(s) ? "height" : "width", i && a) { r = M("number" != typeof (r = "function" == typeof (r = r.padding) ? r(Object.assign({}, n.rects, { placement: n.placement })) : r) ? r : k(r, C)); var f = d(i), p = "y" === e ? "top" : "left", c = "y" === e ? "bottom" : "right", l = n.rects.reference[s] + n.rects.reference[e] - a[e] - n.rects.popper[s]; a = a[e] - n.rects.reference[e], a = (i = (i = y(i)) ? "y" === e ? i.clientHeight || 0 : i.clientWidth || 0 : 0) / 2 - f[s] / 2 + (l / 2 - a / 2), s = _(r[p], U(a, i - f[s] - r[c])), n.modifiersData[o] = ((t = {})[e] = s, t.centerOffset = s - a, t) } }, effect: function (e) { var t = e.state; if (null != (e = void 0 === (e = e.options.element) ? "[data-popper-arrow]" : e)) { if ("string" == typeof e && !(e = t.elements.popper.querySelector(e))) return; O(t.elements.popper, e) && (t.elements.arrow = e) } }, requires: ["popperOffsets"], requiresIfExists: ["preventOverflow"] }, re = { name: "hide", enabled: !0, phase: "main", requiresIfExists: ["preventOverflow"], fn: function (e) { var t = e.state; e = e.name; var n = t.rects.reference, o = t.rects.popper, r = t.modifiersData.preventOverflow, i = W(t, { elementContext: "reference" }), a = W(t, { altBoundary: !0 }); n = S(i, n), o = S(a, o, r), r = q(n), a = q(o), t.modifiersData[e] = { referenceClippingOffsets: n, popperEscapeOffsets: o, isReferenceHidden: r, hasPopperEscaped: a }, t.attributes.popper = Object.assign({}, t.attributes.popper, { "data-popper-reference-hidden": r, "data-popper-escaped": a }) } }, ie = B({ defaultModifiers: [Y, G, K, Q] }), ae = [Y, G, K, Q, Z, te, ne, oe, re], se = B({ defaultModifiers: ae }); e.applyStyles = Q, e.arrow = oe, e.computeStyles = K, e.createPopper = se, e.createPopperLite = ie, e.defaultModifiers = ae, e.detectOverflow = W, e.eventListeners = Y, e.flip = te, e.hide = re, e.offset = Z, e.popperGenerator = B, e.popperOffsets = G, e.preventOverflow = ne, Object.defineProperty(e, "__esModule", { value: !0 }) }));
!function (t, e) { "object" == typeof exports && "undefined" != typeof module ? module.exports = e(require("@popperjs/core")) : "function" == typeof define && define.amd ? define(["@popperjs/core"], e) : (t = t || self).tippy = e(t.Popper) }(this, (function (t) { "use strict"; var e = "undefined" != typeof window && "undefined" != typeof document, n = e ? navigator.userAgent : "", r = /MSIE |Trident\//.test(n), i = { passive: !0, capture: !0 }; function o(t, e, n) { if (Array.isArray(t)) { var r = t[e]; return null == r ? Array.isArray(n) ? n[e] : n : r } return t } function a(t, e) { var n = {}.toString.call(t); return 0 === n.indexOf("[object") && n.indexOf(e + "]") > -1 } function s(t, e) { return "function" == typeof t ? t.apply(void 0, e) : t } function p(t, e) { return 0 === e ? t : function (r) { clearTimeout(n), n = setTimeout((function () { t(r) }), e) }; var n } function u(t, e) { var n = Object.assign({}, t); return e.forEach((function (t) { delete n[t] })), n } function c(t) { return [].concat(t) } function f(t, e) { -1 === t.indexOf(e) && t.push(e) } function l(t) { return t.split("-")[0] } function d(t) { return [].slice.call(t) } function v() { return document.createElement("div") } function m(t) { return ["Element", "Fragment"].some((function (e) { return a(t, e) })) } function g(t) { return a(t, "MouseEvent") } function h(t) { return !(!t || !t._tippy || t._tippy.reference !== t) } function b(t) { return m(t) ? [t] : function (t) { return a(t, "NodeList") }(t) ? d(t) : Array.isArray(t) ? t : d(document.querySelectorAll(t)) } function y(t, e) { t.forEach((function (t) { t && (t.style.transitionDuration = e + "ms") })) } function w(t, e) { t.forEach((function (t) { t && t.setAttribute("data-state", e) })) } function x(t) { var e, n = c(t)[0]; return (null == n || null == (e = n.ownerDocument) ? void 0 : e.body) ? n.ownerDocument : document } function E(t, e, n) { var r = e + "EventListener";["transitionend", "webkitTransitionEnd"].forEach((function (e) { t[r](e, n) })) } var O = { isTouch: !1 }, C = 0; function T() { O.isTouch || (O.isTouch = !0, window.performance && document.addEventListener("mousemove", A)) } function A() { var t = performance.now(); t - C < 20 && (O.isTouch = !1, document.removeEventListener("mousemove", A)), C = t } function L() { var t = document.activeElement; if (h(t)) { var e = t._tippy; t.blur && !e.state.isVisible && t.blur() } } var D = Object.assign({ appendTo: function () { return document.body }, aria: { content: "auto", expanded: "auto" }, delay: 0, duration: [300, 250], getReferenceClientRect: null, hideOnClick: !0, ignoreAttributes: !1, interactive: !1, interactiveBorder: 2, interactiveDebounce: 0, moveTransition: "", offset: [0, 10], onAfterUpdate: function () { }, onBeforeUpdate: function () { }, onCreate: function () { }, onDestroy: function () { }, onHidden: function () { }, onHide: function () { }, onMount: function () { }, onShow: function () { }, onShown: function () { }, onTrigger: function () { }, onUntrigger: function () { }, onClickOutside: function () { }, placement: "top", plugins: [], popperOptions: {}, render: null, showOnCreate: !1, touch: !0, trigger: "mouseenter focus", triggerTarget: null }, { animateFill: !1, followCursor: !1, inlinePositioning: !1, sticky: !1 }, {}, { allowHTML: !1, animation: "fade", arrow: !0, content: "", inertia: !1, maxWidth: 350, role: "tooltip", theme: "", zIndex: 9999 }), k = Object.keys(D); function R(t) { var e = (t.plugins || []).reduce((function (e, n) { var r = n.name, i = n.defaultValue; return r && (e[r] = void 0 !== t[r] ? t[r] : i), e }), {}); return Object.assign({}, t, {}, e) } function j(t, e) { var n = Object.assign({}, e, { content: s(e.content, [t]) }, e.ignoreAttributes ? {} : function (t, e) { return (e ? Object.keys(R(Object.assign({}, D, { plugins: e }))) : k).reduce((function (e, n) { var r = (t.getAttribute("data-tippy-" + n) || "").trim(); if (!r) return e; if ("content" === n) e[n] = r; else try { e[n] = JSON.parse(r) } catch (t) { e[n] = r } return e }), {}) }(t, e.plugins)); return n.aria = Object.assign({}, D.aria, {}, n.aria), n.aria = { expanded: "auto" === n.aria.expanded ? e.interactive : n.aria.expanded, content: "auto" === n.aria.content ? e.interactive ? null : "describedby" : n.aria.content }, n } function M(t, e) { t.innerHTML = e } function P(t) { var e = v(); return !0 === t ? e.className = "tippy-arrow" : (e.className = "tippy-svg-arrow", m(t) ? e.appendChild(t) : M(e, t)), e } function V(t, e) { m(e.content) ? (M(t, ""), t.appendChild(e.content)) : "function" != typeof e.content && (e.allowHTML ? M(t, e.content) : t.textContent = e.content) } function I(t) { var e = t.firstElementChild, n = d(e.children); return { box: e, content: n.find((function (t) { return t.classList.contains("tippy-content") })), arrow: n.find((function (t) { return t.classList.contains("tippy-arrow") || t.classList.contains("tippy-svg-arrow") })), backdrop: n.find((function (t) { return t.classList.contains("tippy-backdrop") })) } } function S(t) { var e = v(), n = v(); n.className = "tippy-box", n.setAttribute("data-state", "hidden"), n.setAttribute("tabindex", "-1"); var r = v(); function i(n, r) { var i = I(e), o = i.box, a = i.content, s = i.arrow; r.theme ? o.setAttribute("data-theme", r.theme) : o.removeAttribute("data-theme"), "string" == typeof r.animation ? o.setAttribute("data-animation", r.animation) : o.removeAttribute("data-animation"), r.inertia ? o.setAttribute("data-inertia", "") : o.removeAttribute("data-inertia"), o.style.maxWidth = "number" == typeof r.maxWidth ? r.maxWidth + "px" : r.maxWidth, r.role ? o.setAttribute("role", r.role) : o.removeAttribute("role"), n.content === r.content && n.allowHTML === r.allowHTML || V(a, t.props), r.arrow ? s ? n.arrow !== r.arrow && (o.removeChild(s), o.appendChild(P(r.arrow))) : o.appendChild(P(r.arrow)) : s && o.removeChild(s) } return r.className = "tippy-content", r.setAttribute("data-state", "hidden"), V(r, t.props), e.appendChild(n), n.appendChild(r), i(t.props, t.props), { popper: e, onUpdate: i } } S.$$tippy = !0; var B = 1, H = [], N = []; function U(e, n) { var a, u, m, h, b, C, T, A, L, k = j(e, Object.assign({}, D, {}, R((a = n, Object.keys(a).reduce((function (t, e) { return void 0 !== a[e] && (t[e] = a[e]), t }), {}))))), M = !1, P = !1, V = !1, S = !1, U = [], _ = p(bt, k.interactiveDebounce), z = B++, F = (L = k.plugins).filter((function (t, e) { return L.indexOf(t) === e })), W = { id: z, reference: e, popper: v(), popperInstance: null, props: k, state: { isEnabled: !0, isVisible: !1, isDestroyed: !1, isMounted: !1, isShown: !1 }, plugins: F, clearDelayTimeouts: function () { clearTimeout(u), clearTimeout(m), cancelAnimationFrame(h) }, setProps: function (t) { if (W.state.isDestroyed) return; it("onBeforeUpdate", [W, t]), gt(); var n = W.props, r = j(e, Object.assign({}, W.props, {}, t, { ignoreAttributes: !0 })); W.props = r, mt(), n.interactiveDebounce !== r.interactiveDebounce && (st(), _ = p(bt, r.interactiveDebounce)); n.triggerTarget && !r.triggerTarget ? c(n.triggerTarget).forEach((function (t) { t.removeAttribute("aria-expanded") })) : r.triggerTarget && e.removeAttribute("aria-expanded"); at(), rt(), q && q(n, r); W.popperInstance && (Et(), Ct().forEach((function (t) { requestAnimationFrame(t._tippy.popperInstance.forceUpdate) }))); it("onAfterUpdate", [W, t]) }, setContent: function (t) { W.setProps({ content: t }) }, show: function () { var t = W.state.isVisible, e = W.state.isDestroyed, n = !W.state.isEnabled, r = O.isTouch && !W.props.touch, i = o(W.props.duration, 0, D.duration); if (t || e || n || r) return; if (Z().hasAttribute("disabled")) return; if (it("onShow", [W], !1), !1 === W.props.onShow(W)) return; W.state.isVisible = !0, Q() && (Y.style.visibility = "visible"); rt(), ft(), W.state.isMounted || (Y.style.transition = "none"); if (Q()) { var a = et(), p = a.box, u = a.content; y([p, u], 0) } T = function () { var t; if (W.state.isVisible && !S) { if (S = !0, Y.offsetHeight, Y.style.transition = W.props.moveTransition, Q() && W.props.animation) { var e = et(), n = e.box, r = e.content; y([n, r], i), w([n, r], "visible") } ot(), at(), f(N, W), null == (t = W.popperInstance) || t.forceUpdate(), W.state.isMounted = !0, it("onMount", [W]), W.props.animation && Q() && function (t, e) { dt(t, e) }(i, (function () { W.state.isShown = !0, it("onShown", [W]) })) } }, function () { var t, e = W.props.appendTo, n = Z(); t = W.props.interactive && e === D.appendTo || "parent" === e ? n.parentNode : s(e, [n]); t.contains(Y) || t.appendChild(Y); Et() }() }, hide: function () { var t = !W.state.isVisible, e = W.state.isDestroyed, n = !W.state.isEnabled, r = o(W.props.duration, 1, D.duration); if (t || e || n) return; if (it("onHide", [W], !1), !1 === W.props.onHide(W)) return; W.state.isVisible = !1, W.state.isShown = !1, S = !1, M = !1, Q() && (Y.style.visibility = "hidden"); if (st(), lt(), rt(), Q()) { var i = et(), a = i.box, s = i.content; W.props.animation && (y([a, s], r), w([a, s], "hidden")) } ot(), at(), W.props.animation ? Q() && function (t, e) { dt(t, (function () { !W.state.isVisible && Y.parentNode && Y.parentNode.contains(Y) && e() })) }(r, W.unmount) : W.unmount() }, hideWithInteractivity: function (t) { tt().addEventListener("mousemove", _), f(H, _), _(t) }, enable: function () { W.state.isEnabled = !0 }, disable: function () { W.hide(), W.state.isEnabled = !1 }, unmount: function () { W.state.isVisible && W.hide(); if (!W.state.isMounted) return; Ot(), Ct().forEach((function (t) { t._tippy.unmount() })), Y.parentNode && Y.parentNode.removeChild(Y); N = N.filter((function (t) { return t !== W })), W.state.isMounted = !1, it("onHidden", [W]) }, destroy: function () { if (W.state.isDestroyed) return; W.clearDelayTimeouts(), W.unmount(), gt(), delete e._tippy, W.state.isDestroyed = !0, it("onDestroy", [W]) } }; if (!k.render) return W; var X = k.render(W), Y = X.popper, q = X.onUpdate; Y.setAttribute("data-tippy-root", ""), Y.id = "tippy-" + W.id, W.popper = Y, e._tippy = W, Y._tippy = W; var $ = F.map((function (t) { return t.fn(W) })), J = e.hasAttribute("aria-expanded"); return mt(), at(), rt(), it("onCreate", [W]), k.showOnCreate && Tt(), Y.addEventListener("mouseenter", (function () { W.props.interactive && W.state.isVisible && W.clearDelayTimeouts() })), Y.addEventListener("mouseleave", (function (t) { W.props.interactive && W.props.trigger.indexOf("mouseenter") >= 0 && (tt().addEventListener("mousemove", _), _(t)) })), W; function G() { var t = W.props.touch; return Array.isArray(t) ? t : [t, 0] } function K() { return "hold" === G()[0] } function Q() { var t; return !!(null == (t = W.props.render) ? void 0 : t.$$tippy) } function Z() { return A || e } function tt() { var t = Z().parentNode; return t ? x(t) : document } function et() { return I(Y) } function nt(t) { return W.state.isMounted && !W.state.isVisible || O.isTouch || b && "focus" === b.type ? 0 : o(W.props.delay, t ? 0 : 1, D.delay) } function rt() { Y.style.pointerEvents = W.props.interactive && W.state.isVisible ? "" : "none", Y.style.zIndex = "" + W.props.zIndex } function it(t, e, n) { var r; (void 0 === n && (n = !0), $.forEach((function (n) { n[t] && n[t].apply(void 0, e) })), n) && (r = W.props)[t].apply(r, e) } function ot() { var t = W.props.aria; if (t.content) { var n = "aria-" + t.content, r = Y.id; c(W.props.triggerTarget || e).forEach((function (t) { var e = t.getAttribute(n); if (W.state.isVisible) t.setAttribute(n, e ? e + " " + r : r); else { var i = e && e.replace(r, "").trim(); i ? t.setAttribute(n, i) : t.removeAttribute(n) } })) } } function at() { !J && W.props.aria.expanded && c(W.props.triggerTarget || e).forEach((function (t) { W.props.interactive ? t.setAttribute("aria-expanded", W.state.isVisible && t === Z() ? "true" : "false") : t.removeAttribute("aria-expanded") })) } function st() { tt().removeEventListener("mousemove", _), H = H.filter((function (t) { return t !== _ })) } function pt(t) { if (!(O.isTouch && (V || "mousedown" === t.type) || W.props.interactive && Y.contains(t.target))) { if (Z().contains(t.target)) { if (O.isTouch) return; if (W.state.isVisible && W.props.trigger.indexOf("click") >= 0) return } else it("onClickOutside", [W, t]); !0 === W.props.hideOnClick && (W.clearDelayTimeouts(), W.hide(), P = !0, setTimeout((function () { P = !1 })), W.state.isMounted || lt()) } } function ut() { V = !0 } function ct() { V = !1 } function ft() { var t = tt(); t.addEventListener("mousedown", pt, !0), t.addEventListener("touchend", pt, i), t.addEventListener("touchstart", ct, i), t.addEventListener("touchmove", ut, i) } function lt() { var t = tt(); t.removeEventListener("mousedown", pt, !0), t.removeEventListener("touchend", pt, i), t.removeEventListener("touchstart", ct, i), t.removeEventListener("touchmove", ut, i) } function dt(t, e) { var n = et().box; function r(t) { t.target === n && (E(n, "remove", r), e()) } if (0 === t) return e(); E(n, "remove", C), E(n, "add", r), C = r } function vt(t, n, r) { void 0 === r && (r = !1), c(W.props.triggerTarget || e).forEach((function (e) { e.addEventListener(t, n, r), U.push({ node: e, eventType: t, handler: n, options: r }) })) } function mt() { var t; K() && (vt("touchstart", ht, { passive: !0 }), vt("touchend", yt, { passive: !0 })), (t = W.props.trigger, t.split(/\s+/).filter(Boolean)).forEach((function (t) { if ("manual" !== t) switch (vt(t, ht), t) { case "mouseenter": vt("mouseleave", yt); break; case "focus": vt(r ? "focusout" : "blur", wt); break; case "focusin": vt("focusout", wt) } })) } function gt() { U.forEach((function (t) { var e = t.node, n = t.eventType, r = t.handler, i = t.options; e.removeEventListener(n, r, i) })), U = [] } function ht(t) { var e, n = !1; if (W.state.isEnabled && !xt(t) && !P) { var r = "focus" === (null == (e = b) ? void 0 : e.type); b = t, A = t.currentTarget, at(), !W.state.isVisible && g(t) && H.forEach((function (e) { return e(t) })), "click" === t.type && (W.props.trigger.indexOf("mouseenter") < 0 || M) && !1 !== W.props.hideOnClick && W.state.isVisible ? n = !0 : Tt(t), "click" === t.type && (M = !n), n && !r && At(t) } } function bt(t) { var e = t.target, n = Z().contains(e) || Y.contains(e); "mousemove" === t.type && n || function (t, e) { var n = e.clientX, r = e.clientY; return t.every((function (t) { var e = t.popperRect, i = t.popperState, o = t.props.interactiveBorder, a = l(i.placement), s = i.modifiersData.offset; if (!s) return !0; var p = "bottom" === a ? s.top.y : 0, u = "top" === a ? s.bottom.y : 0, c = "right" === a ? s.left.x : 0, f = "left" === a ? s.right.x : 0, d = e.top - r + p > o, v = r - e.bottom - u > o, m = e.left - n + c > o, g = n - e.right - f > o; return d || v || m || g })) }(Ct().concat(Y).map((function (t) { var e, n = null == (e = t._tippy.popperInstance) ? void 0 : e.state; return n ? { popperRect: t.getBoundingClientRect(), popperState: n, props: k } : null })).filter(Boolean), t) && (st(), At(t)) } function yt(t) { xt(t) || W.props.trigger.indexOf("click") >= 0 && M || (W.props.interactive ? W.hideWithInteractivity(t) : At(t)) } function wt(t) { W.props.trigger.indexOf("focusin") < 0 && t.target !== Z() || W.props.interactive && t.relatedTarget && Y.contains(t.relatedTarget) || At(t) } function xt(t) { return !!O.isTouch && K() !== t.type.indexOf("touch") >= 0 } function Et() { Ot(); var n = W.props, r = n.popperOptions, i = n.placement, o = n.offset, a = n.getReferenceClientRect, s = n.moveTransition, p = Q() ? I(Y).arrow : null, u = a ? { getBoundingClientRect: a, contextElement: a.contextElement || Z() } : e, c = [{ name: "offset", options: { offset: o } }, { name: "preventOverflow", options: { padding: { top: 2, bottom: 2, left: 5, right: 5 } } }, { name: "flip", options: { padding: 5 } }, { name: "computeStyles", options: { adaptive: !s } }, { name: "$$tippy", enabled: !0, phase: "beforeWrite", requires: ["computeStyles"], fn: function (t) { var e = t.state; if (Q()) { var n = et().box;["placement", "reference-hidden", "escaped"].forEach((function (t) { "placement" === t ? n.setAttribute("data-placement", e.placement) : e.attributes.popper["data-popper-" + t] ? n.setAttribute("data-" + t, "") : n.removeAttribute("data-" + t) })), e.attributes.popper = {} } } }]; Q() && p && c.push({ name: "arrow", options: { element: p, padding: 3 } }), c.push.apply(c, (null == r ? void 0 : r.modifiers) || []), W.popperInstance = t.createPopper(u, Y, Object.assign({}, r, { placement: i, onFirstUpdate: T, modifiers: c })) } function Ot() { W.popperInstance && (W.popperInstance.destroy(), W.popperInstance = null) } function Ct() { return d(Y.querySelectorAll("[data-tippy-root]")) } function Tt(t) { W.clearDelayTimeouts(), t && it("onTrigger", [W, t]), ft(); var e = nt(!0), n = G(), r = n[0], i = n[1]; O.isTouch && "hold" === r && i && (e = i), e ? u = setTimeout((function () { W.show() }), e) : W.show() } function At(t) { if (W.clearDelayTimeouts(), it("onUntrigger", [W, t]), W.state.isVisible) { if (!(W.props.trigger.indexOf("mouseenter") >= 0 && W.props.trigger.indexOf("click") >= 0 && ["mouseleave", "mousemove"].indexOf(t.type) >= 0 && M)) { var e = nt(!1); e ? m = setTimeout((function () { W.state.isVisible && W.hide() }), e) : h = requestAnimationFrame((function () { W.hide() })) } } else lt() } } function _(t, e) { void 0 === e && (e = {}); var n = D.plugins.concat(e.plugins || []); document.addEventListener("touchstart", T, i), window.addEventListener("blur", L); var r = Object.assign({}, e, { plugins: n }), o = b(t).reduce((function (t, e) { var n = e && U(e, r); return n && t.push(n), t }), []); return m(t) ? o[0] : o } _.defaultProps = D, _.setDefaultProps = function (t) { Object.keys(t).forEach((function (e) { D[e] = t[e] })) }, _.currentInput = O; var z = Object.assign({}, t.applyStyles, { effect: function (t) { var e = t.state, n = { popper: { position: e.options.strategy, left: "0", top: "0", margin: "0" }, arrow: { position: "absolute" }, reference: {} }; Object.assign(e.elements.popper.style, n.popper), e.styles = n, e.elements.arrow && Object.assign(e.elements.arrow.style, n.arrow) } }), F = { mouseover: "mouseenter", focusin: "focus", click: "click" }; var W = { name: "animateFill", defaultValue: !1, fn: function (t) { var e; if (!(null == (e = t.props.render) ? void 0 : e.$$tippy)) return {}; var n = I(t.popper), r = n.box, i = n.content, o = t.props.animateFill ? function () { var t = v(); return t.className = "tippy-backdrop", w([t], "hidden"), t }() : null; return { onCreate: function () { o && (r.insertBefore(o, r.firstElementChild), r.setAttribute("data-animatefill", ""), r.style.overflow = "hidden", t.setProps({ arrow: !1, animation: "shift-away" })) }, onMount: function () { if (o) { var t = r.style.transitionDuration, e = Number(t.replace("ms", "")); i.style.transitionDelay = Math.round(e / 10) + "ms", o.style.transitionDuration = t, w([o], "visible") } }, onShow: function () { o && (o.style.transitionDuration = "0ms") }, onHide: function () { o && w([o], "hidden") } } } }; var X = { clientX: 0, clientY: 0 }, Y = []; function q(t) { var e = t.clientX, n = t.clientY; X = { clientX: e, clientY: n } } var $ = { name: "followCursor", defaultValue: !1, fn: function (t) { var e = t.reference, n = x(t.props.triggerTarget || e), r = !1, i = !1, o = !0, a = t.props; function s() { return "initial" === t.props.followCursor && t.state.isVisible } function p() { n.addEventListener("mousemove", f) } function u() { n.removeEventListener("mousemove", f) } function c() { r = !0, t.setProps({ getReferenceClientRect: null }), r = !1 } function f(n) { var r = !n.target || e.contains(n.target), i = t.props.followCursor, o = n.clientX, a = n.clientY, s = e.getBoundingClientRect(), p = o - s.left, u = a - s.top; !r && t.props.interactive || t.setProps({ getReferenceClientRect: function () { var t = e.getBoundingClientRect(), n = o, r = a; "initial" === i && (n = t.left + p, r = t.top + u); var s = "horizontal" === i ? t.top : r, c = "vertical" === i ? t.right : n, f = "horizontal" === i ? t.bottom : r, l = "vertical" === i ? t.left : n; return { width: c - l, height: f - s, top: s, right: c, bottom: f, left: l } } }) } function l() { t.props.followCursor && (Y.push({ instance: t, doc: n }), function (t) { t.addEventListener("mousemove", q) }(n)) } function d() { 0 === (Y = Y.filter((function (e) { return e.instance !== t }))).filter((function (t) { return t.doc === n })).length && function (t) { t.removeEventListener("mousemove", q) }(n) } return { onCreate: l, onDestroy: d, onBeforeUpdate: function () { a = t.props }, onAfterUpdate: function (e, n) { var o = n.followCursor; r || void 0 !== o && a.followCursor !== o && (d(), o ? (l(), !t.state.isMounted || i || s() || p()) : (u(), c())) }, onMount: function () { t.props.followCursor && !i && (o && (f(X), o = !1), s() || p()) }, onTrigger: function (t, e) { g(e) && (X = { clientX: e.clientX, clientY: e.clientY }), i = "focus" === e.type }, onHidden: function () { t.props.followCursor && (c(), u(), o = !0) } } } }; var J = { name: "inlinePositioning", defaultValue: !1, fn: function (t) { var e, n = t.reference; var r = -1, i = !1, o = { name: "tippyInlinePositioning", enabled: !0, phase: "afterWrite", fn: function (i) { var o = i.state; t.props.inlinePositioning && (e !== o.placement && t.setProps({ getReferenceClientRect: function () { return function (t) { return function (t, e, n, r) { if (n.length < 2 || null === t) return e; if (2 === n.length && r >= 0 && n[0].left > n[1].right) return n[r] || e; switch (t) { case "top": case "bottom": var i = n[0], o = n[n.length - 1], a = "top" === t, s = i.top, p = o.bottom, u = a ? i.left : o.left, c = a ? i.right : o.right; return { top: s, bottom: p, left: u, right: c, width: c - u, height: p - s }; case "left": case "right": var f = Math.min.apply(Math, n.map((function (t) { return t.left }))), l = Math.max.apply(Math, n.map((function (t) { return t.right }))), d = n.filter((function (e) { return "left" === t ? e.left === f : e.right === l })), v = d[0].top, m = d[d.length - 1].bottom; return { top: v, bottom: m, left: f, right: l, width: l - f, height: m - v }; default: return e } }(l(t), n.getBoundingClientRect(), d(n.getClientRects()), r) }(o.placement) } }), e = o.placement) } }; function a() { var e; i || (e = function (t, e) { var n; return { popperOptions: Object.assign({}, t.popperOptions, { modifiers: [].concat(((null == (n = t.popperOptions) ? void 0 : n.modifiers) || []).filter((function (t) { return t.name !== e.name })), [e]) }) } }(t.props, o), i = !0, t.setProps(e), i = !1) } return { onCreate: a, onAfterUpdate: a, onTrigger: function (e, n) { if (g(n)) { var i = d(t.reference.getClientRects()), o = i.find((function (t) { return t.left - 2 <= n.clientX && t.right + 2 >= n.clientX && t.top - 2 <= n.clientY && t.bottom + 2 >= n.clientY })); r = i.indexOf(o) } }, onUntrigger: function () { r = -1 } } } }; var G = { name: "sticky", defaultValue: !1, fn: function (t) { var e = t.reference, n = t.popper; function r(e) { return !0 === t.props.sticky || t.props.sticky === e } var i = null, o = null; function a() { var s = r("reference") ? (t.popperInstance ? t.popperInstance.state.elements.reference : e).getBoundingClientRect() : null, p = r("popper") ? n.getBoundingClientRect() : null; (s && K(i, s) || p && K(o, p)) && t.popperInstance && t.popperInstance.update(), i = s, o = p, t.state.isMounted && requestAnimationFrame(a) } return { onMount: function () { t.props.sticky && a() } } } }; function K(t, e) { return !t || !e || (t.top !== e.top || t.right !== e.right || t.bottom !== e.bottom || t.left !== e.left) } return e && function (t) { var e = document.createElement("style"); e.textContent = t, e.setAttribute("data-tippy-stylesheet", ""); var n = document.head, r = document.querySelector("head>style,head>link"); r ? n.insertBefore(e, r) : n.appendChild(e) }('.tippy-box[data-animation=fade][data-state=hidden]{opacity:0}[data-tippy-root]{max-width:calc(100vw - 10px)}.tippy-box{position:relative;background-color:#333;color:#fff;border-radius:4px;font-size:14px;line-height:1.4;outline:0;transition-property:transform,visibility,opacity}.tippy-box[data-placement^=top]>.tippy-arrow{bottom:0}.tippy-box[data-placement^=top]>.tippy-arrow:before{bottom:-7px;left:0;border-width:8px 8px 0;border-top-color:initial;transform-origin:center top}.tippy-box[data-placement^=bottom]>.tippy-arrow{top:0}.tippy-box[data-placement^=bottom]>.tippy-arrow:before{top:-7px;left:0;border-width:0 8px 8px;border-bottom-color:initial;transform-origin:center bottom}.tippy-box[data-placement^=left]>.tippy-arrow{right:0}.tippy-box[data-placement^=left]>.tippy-arrow:before{border-width:8px 0 8px 8px;border-left-color:initial;right:-7px;transform-origin:center left}.tippy-box[data-placement^=right]>.tippy-arrow{left:0}.tippy-box[data-placement^=right]>.tippy-arrow:before{left:-7px;border-width:8px 8px 8px 0;border-right-color:initial;transform-origin:center right}.tippy-box[data-inertia][data-state=visible]{transition-timing-function:cubic-bezier(.54,1.5,.38,1.11)}.tippy-arrow{width:16px;height:16px;color:#333}.tippy-arrow:before{content:"";position:absolute;border-color:transparent;border-style:solid}.tippy-content{position:relative;padding:5px 9px;z-index:1}'), _.setDefaultProps({ plugins: [W, $, J, G], render: S }), _.createSingleton = function (t, e) { var n; void 0 === e && (e = {}); var r, i = t, o = [], a = e.overrides, s = [], p = !1; function c() { o = i.map((function (t) { return t.reference })) } function f(t) { i.forEach((function (e) { t ? e.enable() : e.disable() })) } function l(t) { return i.map((function (e) { var n = e.setProps; return e.setProps = function (i) { n(i), e.reference === r && t.setProps(i) }, function () { e.setProps = n } })) } function d(t, e) { var n = o.indexOf(e); if (e !== r) { r = e; var s = (a || []).concat("content").reduce((function (t, e) { return t[e] = i[n].props[e], t }), {}); t.setProps(Object.assign({}, s, { getReferenceClientRect: "function" == typeof s.getReferenceClientRect ? s.getReferenceClientRect : function () { return e.getBoundingClientRect() } })) } } f(!1), c(); var m = { fn: function () { return { onDestroy: function () { f(!0) }, onHidden: function () { r = null }, onClickOutside: function (t) { t.props.showOnCreate && !p && (p = !0, r = null) }, onShow: function (t) { t.props.showOnCreate && !p && (p = !0, d(t, o[0])) }, onTrigger: function (t, e) { d(t, e.currentTarget) } } } }, g = _(v(), Object.assign({}, u(e, ["overrides"]), { plugins: [m].concat(e.plugins || []), triggerTarget: o, popperOptions: Object.assign({}, e.popperOptions, { modifiers: [].concat((null == (n = e.popperOptions) ? void 0 : n.modifiers) || [], [z]) }) })), h = g.show; g.show = function (t) { if (h(), !r && null == t) return d(g, o[0]); if (!r || null != t) { if ("number" == typeof t) return o[t] && d(g, o[t]); if (i.includes(t)) { var e = t.reference; return d(g, e) } return o.includes(t) ? d(g, t) : void 0 } }, g.showNext = function () { var t = o[0]; if (!r) return g.show(0); var e = o.indexOf(r); g.show(o[e + 1] || t) }, g.showPrevious = function () { var t = o[o.length - 1]; if (!r) return g.show(t); var e = o.indexOf(r), n = o[e - 1] || t; g.show(n) }; var b = g.setProps; return g.setProps = function (t) { a = t.overrides || a, b(t) }, g.setInstances = function (t) { f(!0), s.forEach((function (t) { return t() })), i = t, f(!1), c(), l(g), g.setProps({ triggerTarget: o }) }, s = l(g), g }, _.delegate = function (t, e) { var n = [], r = [], o = !1, a = e.target, s = u(e, ["target"]), p = Object.assign({}, s, { trigger: "manual", touch: !1 }), f = Object.assign({}, s, { showOnCreate: !0 }), l = _(t, p); function d(t) { if (t.target && !o) { var n = t.target.closest(a); if (n) { var i = n.getAttribute("data-tippy-trigger") || e.trigger || D.trigger; if (!n._tippy && !("touchstart" === t.type && "boolean" == typeof f.touch || "touchstart" !== t.type && i.indexOf(F[t.type]) < 0)) { var s = _(n, f); s && (r = r.concat(s)) } } } } function v(t, e, r, i) { void 0 === i && (i = !1), t.addEventListener(e, r, i), n.push({ node: t, eventType: e, handler: r, options: i }) } return c(l).forEach((function (t) { var e = t.destroy, a = t.enable, s = t.disable; t.destroy = function (t) { void 0 === t && (t = !0), t && r.forEach((function (t) { t.destroy() })), r = [], n.forEach((function (t) { var e = t.node, n = t.eventType, r = t.handler, i = t.options; e.removeEventListener(n, r, i) })), n = [], e() }, t.enable = function () { a(), r.forEach((function (t) { return t.enable() })), o = !1 }, t.disable = function () { s(), r.forEach((function (t) { return t.disable() })), o = !0 }, function (t) { var e = t.reference; v(e, "touchstart", d, i), v(e, "mouseover", d), v(e, "focusin", d), v(e, "click", d) }(t) })), l }, _.hideAll = function (t) { var e = void 0 === t ? {} : t, n = e.exclude, r = e.duration; N.forEach((function (t) { var e = !1; if (n && (e = h(n) ? t.reference === n : t.popper === n.popper), !e) { var i = t.props.duration; t.setProps({ duration: r }), t.hide(), t.state.isDestroyed || t.setProps({ duration: i }) } })) }, _.roundArrow = '<svg width="16" height="6" xmlns="http://www.w3.org/2000/svg"><path d="M0 6s1.796-.013 4.67-3.615C5.851.9 6.93.006 8 0c1.07-.006 2.148.887 3.343 2.385C14.233 6.005 16 6 16 6H0z"></svg>', _ }));
!function (e, t) { "object" == typeof exports && "undefined" != typeof module ? module.exports = t() : "function" == typeof define && define.amd ? define(t) : (e = "undefined" != typeof globalThis ? globalThis : e || self).flatpickr = t() }(this, (function () { "use strict"; var e = function () { return (e = Object.assign || function (e) { for (var t, n = 1, a = arguments.length; n < a; n++)for (var i in t = arguments[n]) Object.prototype.hasOwnProperty.call(t, i) && (e[i] = t[i]); return e }).apply(this, arguments) }; function t() { for (var e = 0, t = 0, n = arguments.length; t < n; t++)e += arguments[t].length; var a = Array(e), i = 0; for (t = 0; t < n; t++)for (var o = arguments[t], r = 0, l = o.length; r < l; r++, i++)a[i] = o[r]; return a } var n = ["onChange", "onClose", "onDayCreate", "onDestroy", "onKeyDown", "onMonthChange", "onOpen", "onParseConfig", "onReady", "onValueUpdate", "onYearChange", "onPreCalendarPosition"], a = { _disable: [], allowInput: !1, allowInvalidPreload: !1, altFormat: "F j, Y", altInput: !1, altInputClass: "form-control input", animate: "object" == typeof window && -1 === window.navigator.userAgent.indexOf("MSIE"), ariaDateFormat: "F j, Y", autoFillDefaultTime: !0, clickOpens: !0, closeOnSelect: !0, conjunction: ", ", dateFormat: "Y-m-d", defaultHour: 12, defaultMinute: 0, defaultSeconds: 0, disable: [], disableMobile: !1, enableSeconds: !1, enableTime: !1, errorHandler: function (e) { return "undefined" != typeof console && console.warn(e) }, getWeek: function (e) { var t = new Date(e.getTime()); t.setHours(0, 0, 0, 0), t.setDate(t.getDate() + 3 - (t.getDay() + 6) % 7); var n = new Date(t.getFullYear(), 0, 4); return 1 + Math.round(((t.getTime() - n.getTime()) / 864e5 - 3 + (n.getDay() + 6) % 7) / 7) }, hourIncrement: 1, ignoredFocusElements: [], inline: !1, locale: "default", minuteIncrement: 5, mode: "single", monthSelectorType: "dropdown", nextArrow: "<svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' viewBox='0 0 17 17'><g></g><path d='M13.207 8.472l-7.854 7.854-0.707-0.707 7.146-7.146-7.146-7.148 0.707-0.707 7.854 7.854z' /></svg>", noCalendar: !1, now: new Date, onChange: [], onClose: [], onDayCreate: [], onDestroy: [], onKeyDown: [], onMonthChange: [], onOpen: [], onParseConfig: [], onReady: [], onValueUpdate: [], onYearChange: [], onPreCalendarPosition: [], plugins: [], position: "auto", positionElement: void 0, prevArrow: "<svg version='1.1' xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' viewBox='0 0 17 17'><g></g><path d='M5.207 8.471l7.146 7.147-0.707 0.707-7.853-7.854 7.854-7.853 0.707 0.707-7.147 7.146z' /></svg>", shorthandCurrentMonth: !1, showMonths: 1, static: !1, time_24hr: !1, weekNumbers: !1, wrap: !1 }, i = { weekdays: { shorthand: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"], longhand: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"] }, months: { shorthand: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"], longhand: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"] }, daysInMonth: [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31], firstDayOfWeek: 0, ordinal: function (e) { var t = e % 100; if (t > 3 && t < 21) return "th"; switch (t % 10) { case 1: return "st"; case 2: return "nd"; case 3: return "rd"; default: return "th" } }, rangeSeparator: " to ", weekAbbreviation: "Wk", scrollTitle: "Scroll to increment", toggleTitle: "Click to toggle", amPM: ["AM", "PM"], yearAriaLabel: "Year", monthAriaLabel: "Month", hourAriaLabel: "Hour", minuteAriaLabel: "Minute", time_24hr: !1 }, o = function (e, t) { return void 0 === t && (t = 2), ("000" + e).slice(-1 * t) }, r = function (e) { return !0 === e ? 1 : 0 }; function l(e, t) { var n; return function () { var a = this; clearTimeout(n), n = setTimeout((function () { return e.apply(a, arguments) }), t) } } var c = function (e) { return e instanceof Array ? e : [e] }; function d(e, t, n) { if (!0 === n) return e.classList.add(t); e.classList.remove(t) } function s(e, t, n) { var a = window.document.createElement(e); return t = t || "", n = n || "", a.className = t, void 0 !== n && (a.textContent = n), a } function u(e) { for (; e.firstChild;)e.removeChild(e.firstChild) } function f(e, t) { return t(e) ? e : e.parentNode ? f(e.parentNode, t) : void 0 } function m(e, t) { var n = s("div", "numInputWrapper"), a = s("input", "numInput " + e), i = s("span", "arrowUp"), o = s("span", "arrowDown"); if (-1 === navigator.userAgent.indexOf("MSIE 9.0") ? a.type = "number" : (a.type = "text", a.pattern = "\\d*"), void 0 !== t) for (var r in t) a.setAttribute(r, t[r]); return n.appendChild(a), n.appendChild(i), n.appendChild(o), n } function g(e) { try { return "function" == typeof e.composedPath ? e.composedPath()[0] : e.target } catch (t) { return e.target } } var p = function () { }, h = function (e, t, n) { return n.months[t ? "shorthand" : "longhand"][e] }, v = { D: p, F: function (e, t, n) { e.setMonth(n.months.longhand.indexOf(t)) }, G: function (e, t) { e.setHours(parseFloat(t)) }, H: function (e, t) { e.setHours(parseFloat(t)) }, J: function (e, t) { e.setDate(parseFloat(t)) }, K: function (e, t, n) { e.setHours(e.getHours() % 12 + 12 * r(new RegExp(n.amPM[1], "i").test(t))) }, M: function (e, t, n) { e.setMonth(n.months.shorthand.indexOf(t)) }, S: function (e, t) { e.setSeconds(parseFloat(t)) }, U: function (e, t) { return new Date(1e3 * parseFloat(t)) }, W: function (e, t, n) { var a = parseInt(t), i = new Date(e.getFullYear(), 0, 2 + 7 * (a - 1), 0, 0, 0, 0); return i.setDate(i.getDate() - i.getDay() + n.firstDayOfWeek), i }, Y: function (e, t) { e.setFullYear(parseFloat(t)) }, Z: function (e, t) { return new Date(t) }, d: function (e, t) { e.setDate(parseFloat(t)) }, h: function (e, t) { e.setHours(parseFloat(t)) }, i: function (e, t) { e.setMinutes(parseFloat(t)) }, j: function (e, t) { e.setDate(parseFloat(t)) }, l: p, m: function (e, t) { e.setMonth(parseFloat(t) - 1) }, n: function (e, t) { e.setMonth(parseFloat(t) - 1) }, s: function (e, t) { e.setSeconds(parseFloat(t)) }, u: function (e, t) { return new Date(parseFloat(t)) }, w: p, y: function (e, t) { e.setFullYear(2e3 + parseFloat(t)) } }, D = { D: "(\\w+)", F: "(\\w+)", G: "(\\d\\d|\\d)", H: "(\\d\\d|\\d)", J: "(\\d\\d|\\d)\\w+", K: "", M: "(\\w+)", S: "(\\d\\d|\\d)", U: "(.+)", W: "(\\d\\d|\\d)", Y: "(\\d{4})", Z: "(.+)", d: "(\\d\\d|\\d)", h: "(\\d\\d|\\d)", i: "(\\d\\d|\\d)", j: "(\\d\\d|\\d)", l: "(\\w+)", m: "(\\d\\d|\\d)", n: "(\\d\\d|\\d)", s: "(\\d\\d|\\d)", u: "(.+)", w: "(\\d\\d|\\d)", y: "(\\d{2})" }, w = { Z: function (e) { return e.toISOString() }, D: function (e, t, n) { return t.weekdays.shorthand[w.w(e, t, n)] }, F: function (e, t, n) { return h(w.n(e, t, n) - 1, !1, t) }, G: function (e, t, n) { return o(w.h(e, t, n)) }, H: function (e) { return o(e.getHours()) }, J: function (e, t) { return void 0 !== t.ordinal ? e.getDate() + t.ordinal(e.getDate()) : e.getDate() }, K: function (e, t) { return t.amPM[r(e.getHours() > 11)] }, M: function (e, t) { return h(e.getMonth(), !0, t) }, S: function (e) { return o(e.getSeconds()) }, U: function (e) { return e.getTime() / 1e3 }, W: function (e, t, n) { return n.getWeek(e) }, Y: function (e) { return o(e.getFullYear(), 4) }, d: function (e) { return o(e.getDate()) }, h: function (e) { return e.getHours() % 12 ? e.getHours() % 12 : 12 }, i: function (e) { return o(e.getMinutes()) }, j: function (e) { return e.getDate() }, l: function (e, t) { return t.weekdays.longhand[e.getDay()] }, m: function (e) { return o(e.getMonth() + 1) }, n: function (e) { return e.getMonth() + 1 }, s: function (e) { return e.getSeconds() }, u: function (e) { return e.getTime() }, w: function (e) { return e.getDay() }, y: function (e) { return String(e.getFullYear()).substring(2) } }, b = function (e) { var t = e.config, n = void 0 === t ? a : t, o = e.l10n, r = void 0 === o ? i : o, l = e.isMobile, c = void 0 !== l && l; return function (e, t, a) { var i = a || r; return void 0 === n.formatDate || c ? t.split("").map((function (t, a, o) { return w[t] && "\\" !== o[a - 1] ? w[t](e, i, n) : "\\" !== t ? t : "" })).join("") : n.formatDate(e, t, i) } }, C = function (e) { var t = e.config, n = void 0 === t ? a : t, o = e.l10n, r = void 0 === o ? i : o; return function (e, t, i, o) { if (0 === e || e) { var l, c = o || r, d = e; if (e instanceof Date) l = new Date(e.getTime()); else if ("string" != typeof e && void 0 !== e.toFixed) l = new Date(e); else if ("string" == typeof e) { var s = t || (n || a).dateFormat, u = String(e).trim(); if ("today" === u) l = new Date, i = !0; else if (/Z$/.test(u) || /GMT$/.test(u)) l = new Date(e); else if (n && n.parseDate) l = n.parseDate(e, s); else { l = n && n.noCalendar ? new Date((new Date).setHours(0, 0, 0, 0)) : new Date((new Date).getFullYear(), 0, 1, 0, 0, 0, 0); for (var f = void 0, m = [], g = 0, p = 0, h = ""; g < s.length; g++) { var w = s[g], b = "\\" === w, C = "\\" === s[g - 1] || b; if (D[w] && !C) { h += D[w]; var M = new RegExp(h).exec(e); M && (f = !0) && m["Y" !== w ? "push" : "unshift"]({ fn: v[w], val: M[++p] }) } else b || (h += "."); m.forEach((function (e) { var t = e.fn, n = e.val; return l = t(l, n, c) || l })) } l = f ? l : void 0 } } if (l instanceof Date && !isNaN(l.getTime())) return !0 === i && l.setHours(0, 0, 0, 0), l; n.errorHandler(new Error("Invalid date provided: " + d)) } } }; function M(e, t, n) { return void 0 === n && (n = !0), !1 !== n ? new Date(e.getTime()).setHours(0, 0, 0, 0) - new Date(t.getTime()).setHours(0, 0, 0, 0) : e.getTime() - t.getTime() } var y = 864e5; function x(e) { var t = e.defaultHour, n = e.defaultMinute, a = e.defaultSeconds; if (void 0 !== e.minDate) { var i = e.minDate.getHours(), o = e.minDate.getMinutes(), r = e.minDate.getSeconds(); t < i && (t = i), t === i && n < o && (n = o), t === i && n === o && a < r && (a = e.minDate.getSeconds()) } if (void 0 !== e.maxDate) { var l = e.maxDate.getHours(), c = e.maxDate.getMinutes(); (t = Math.min(t, l)) === l && (n = Math.min(c, n)), t === l && n === c && (a = e.maxDate.getSeconds()) } return { hours: t, minutes: n, seconds: a } } "function" != typeof Object.assign && (Object.assign = function (e) { for (var t = [], n = 1; n < arguments.length; n++)t[n - 1] = arguments[n]; if (!e) throw TypeError("Cannot convert undefined or null to object"); for (var a = function (t) { t && Object.keys(t).forEach((function (n) { return e[n] = t[n] })) }, i = 0, o = t; i < o.length; i++) { var r = o[i]; a(r) } return e }); function E(p, v) { var w = { config: e(e({}, a), T.defaultConfig), l10n: i }; function E(e) { return e.bind(w) } function k() { var e = w.config; !1 === e.weekNumbers && 1 === e.showMonths || !0 !== e.noCalendar && window.requestAnimationFrame((function () { if (void 0 !== w.calendarContainer && (w.calendarContainer.style.visibility = "hidden", w.calendarContainer.style.display = "block"), void 0 !== w.daysContainer) { var t = (w.days.offsetWidth + 1) * e.showMonths; w.daysContainer.style.width = t + "px", w.calendarContainer.style.width = t + (void 0 !== w.weekWrapper ? w.weekWrapper.offsetWidth : 0) + "px", w.calendarContainer.style.removeProperty("visibility"), w.calendarContainer.style.removeProperty("display") } })) } function I(e) { if (0 === w.selectedDates.length) { var t = void 0 === w.config.minDate || M(new Date, w.config.minDate) >= 0 ? new Date : new Date(w.config.minDate.getTime()), n = x(w.config); t.setHours(n.hours, n.minutes, n.seconds, t.getMilliseconds()), w.selectedDates = [t], w.latestSelectedDateObj = t } void 0 !== e && "blur" !== e.type && function (e) { e.preventDefault(); var t = "keydown" === e.type, n = g(e), a = n; void 0 !== w.amPM && n === w.amPM && (w.amPM.textContent = w.l10n.amPM[r(w.amPM.textContent === w.l10n.amPM[0])]); var i = parseFloat(a.getAttribute("min")), l = parseFloat(a.getAttribute("max")), c = parseFloat(a.getAttribute("step")), d = parseInt(a.value, 10), s = e.delta || (t ? 38 === e.which ? 1 : -1 : 0), u = d + c * s; if (void 0 !== a.value && 2 === a.value.length) { var f = a === w.hourElement, m = a === w.minuteElement; u < i ? (u = l + u + r(!f) + (r(f) && r(!w.amPM)), m && j(void 0, -1, w.hourElement)) : u > l && (u = a === w.hourElement ? u - l - r(!w.amPM) : i, m && j(void 0, 1, w.hourElement)), w.amPM && f && (1 === c ? u + d === 23 : Math.abs(u - d) > c) && (w.amPM.textContent = w.l10n.amPM[r(w.amPM.textContent === w.l10n.amPM[0])]), a.value = o(u) } }(e); var a = w._input.value; S(), be(), w._input.value !== a && w._debouncedChange() } function S() { if (void 0 !== w.hourElement && void 0 !== w.minuteElement) { var e, t, n = (parseInt(w.hourElement.value.slice(-2), 10) || 0) % 24, a = (parseInt(w.minuteElement.value, 10) || 0) % 60, i = void 0 !== w.secondElement ? (parseInt(w.secondElement.value, 10) || 0) % 60 : 0; void 0 !== w.amPM && (e = n, t = w.amPM.textContent, n = e % 12 + 12 * r(t === w.l10n.amPM[1])); var o = void 0 !== w.config.minTime || w.config.minDate && w.minDateHasTime && w.latestSelectedDateObj && 0 === M(w.latestSelectedDateObj, w.config.minDate, !0); if (void 0 !== w.config.maxTime || w.config.maxDate && w.maxDateHasTime && w.latestSelectedDateObj && 0 === M(w.latestSelectedDateObj, w.config.maxDate, !0)) { var l = void 0 !== w.config.maxTime ? w.config.maxTime : w.config.maxDate; (n = Math.min(n, l.getHours())) === l.getHours() && (a = Math.min(a, l.getMinutes())), a === l.getMinutes() && (i = Math.min(i, l.getSeconds())) } if (o) { var c = void 0 !== w.config.minTime ? w.config.minTime : w.config.minDate; (n = Math.max(n, c.getHours())) === c.getHours() && a < c.getMinutes() && (a = c.getMinutes()), a === c.getMinutes() && (i = Math.max(i, c.getSeconds())) } O(n, a, i) } } function _(e) { var t = e || w.latestSelectedDateObj; t && O(t.getHours(), t.getMinutes(), t.getSeconds()) } function O(e, t, n) { void 0 !== w.latestSelectedDateObj && w.latestSelectedDateObj.setHours(e % 24, t, n || 0, 0), w.hourElement && w.minuteElement && !w.isMobile && (w.hourElement.value = o(w.config.time_24hr ? e : (12 + e) % 12 + 12 * r(e % 12 == 0)), w.minuteElement.value = o(t), void 0 !== w.amPM && (w.amPM.textContent = w.l10n.amPM[r(e >= 12)]), void 0 !== w.secondElement && (w.secondElement.value = o(n))) } function F(e) { var t = g(e), n = parseInt(t.value) + (e.delta || 0); (n / 1e3 > 1 || "Enter" === e.key && !/[^\d]/.test(n.toString())) && Q(n) } function A(e, t, n, a) { return t instanceof Array ? t.forEach((function (t) { return A(e, t, n, a) })) : e instanceof Array ? e.forEach((function (e) { return A(e, t, n, a) })) : (e.addEventListener(t, n, a), void w._handlers.push({ remove: function () { return e.removeEventListener(t, n) } })) } function N() { pe("onChange") } function P(e, t) { var n = void 0 !== e ? w.parseDate(e) : w.latestSelectedDateObj || (w.config.minDate && w.config.minDate > w.now ? w.config.minDate : w.config.maxDate && w.config.maxDate < w.now ? w.config.maxDate : w.now), a = w.currentYear, i = w.currentMonth; try { void 0 !== n && (w.currentYear = n.getFullYear(), w.currentMonth = n.getMonth()) } catch (e) { e.message = "Invalid date supplied: " + n, w.config.errorHandler(e) } t && w.currentYear !== a && (pe("onYearChange"), K()), !t || w.currentYear === a && w.currentMonth === i || pe("onMonthChange"), w.redraw() } function Y(e) { var t = g(e); ~t.className.indexOf("arrow") && j(e, t.classList.contains("arrowUp") ? 1 : -1) } function j(e, t, n) { var a = e && g(e), i = n || a && a.parentNode && a.parentNode.firstChild, o = he("increment"); o.delta = t, i && i.dispatchEvent(o) } function H(e, t, n, a) { var i = X(t, !0), o = s("span", "flatpickr-day " + e, t.getDate().toString()); return o.dateObj = t, o.$i = a, o.setAttribute("aria-label", w.formatDate(t, w.config.ariaDateFormat)), -1 === e.indexOf("hidden") && 0 === M(t, w.now) && (w.todayDateElem = o, o.classList.add("today"), o.setAttribute("aria-current", "date")), i ? (o.tabIndex = -1, ve(t) && (o.classList.add("selected"), w.selectedDateElem = o, "range" === w.config.mode && (d(o, "startRange", w.selectedDates[0] && 0 === M(t, w.selectedDates[0], !0)), d(o, "endRange", w.selectedDates[1] && 0 === M(t, w.selectedDates[1], !0)), "nextMonthDay" === e && o.classList.add("inRange")))) : o.classList.add("flatpickr-disabled"), "range" === w.config.mode && function (e) { return !("range" !== w.config.mode || w.selectedDates.length < 2) && (M(e, w.selectedDates[0]) >= 0 && M(e, w.selectedDates[1]) <= 0) }(t) && !ve(t) && o.classList.add("inRange"), w.weekNumbers && 1 === w.config.showMonths && "prevMonthDay" !== e && n % 7 == 1 && w.weekNumbers.insertAdjacentHTML("beforeend", "<span class='flatpickr-day'>" + w.config.getWeek(t) + "</span>"), pe("onDayCreate", o), o } function L(e) { e.focus(), "range" === w.config.mode && ae(e) } function W(e) { for (var t = e > 0 ? 0 : w.config.showMonths - 1, n = e > 0 ? w.config.showMonths : -1, a = t; a != n; a += e)for (var i = w.daysContainer.children[a], o = e > 0 ? 0 : i.children.length - 1, r = e > 0 ? i.children.length : -1, l = o; l != r; l += e) { var c = i.children[l]; if (-1 === c.className.indexOf("hidden") && X(c.dateObj)) return c } } function R(e, t) { var n = ee(document.activeElement || document.body), a = void 0 !== e ? e : n ? document.activeElement : void 0 !== w.selectedDateElem && ee(w.selectedDateElem) ? w.selectedDateElem : void 0 !== w.todayDateElem && ee(w.todayDateElem) ? w.todayDateElem : W(t > 0 ? 1 : -1); void 0 === a ? w._input.focus() : n ? function (e, t) { for (var n = -1 === e.className.indexOf("Month") ? e.dateObj.getMonth() : w.currentMonth, a = t > 0 ? w.config.showMonths : -1, i = t > 0 ? 1 : -1, o = n - w.currentMonth; o != a; o += i)for (var r = w.daysContainer.children[o], l = n - w.currentMonth === o ? e.$i + t : t < 0 ? r.children.length - 1 : 0, c = r.children.length, d = l; d >= 0 && d < c && d != (t > 0 ? c : -1); d += i) { var s = r.children[d]; if (-1 === s.className.indexOf("hidden") && X(s.dateObj) && Math.abs(e.$i - d) >= Math.abs(t)) return L(s) } w.changeMonth(i), R(W(i), 0) }(a, t) : L(a) } function B(e, t) { for (var n = (new Date(e, t, 1).getDay() - w.l10n.firstDayOfWeek + 7) % 7, a = w.utils.getDaysInMonth((t - 1 + 12) % 12, e), i = w.utils.getDaysInMonth(t, e), o = window.document.createDocumentFragment(), r = w.config.showMonths > 1, l = r ? "prevMonthDay hidden" : "prevMonthDay", c = r ? "nextMonthDay hidden" : "nextMonthDay", d = a + 1 - n, u = 0; d <= a; d++, u++)o.appendChild(H(l, new Date(e, t - 1, d), d, u)); for (d = 1; d <= i; d++, u++)o.appendChild(H("", new Date(e, t, d), d, u)); for (var f = i + 1; f <= 42 - n && (1 === w.config.showMonths || u % 7 != 0); f++, u++)o.appendChild(H(c, new Date(e, t + 1, f % i), f, u)); var m = s("div", "dayContainer"); return m.appendChild(o), m } function J() { if (void 0 !== w.daysContainer) { u(w.daysContainer), w.weekNumbers && u(w.weekNumbers); for (var e = document.createDocumentFragment(), t = 0; t < w.config.showMonths; t++) { var n = new Date(w.currentYear, w.currentMonth, 1); n.setMonth(w.currentMonth + t), e.appendChild(B(n.getFullYear(), n.getMonth())) } w.daysContainer.appendChild(e), w.days = w.daysContainer.firstChild, "range" === w.config.mode && 1 === w.selectedDates.length && ae() } } function K() { if (!(w.config.showMonths > 1 || "dropdown" !== w.config.monthSelectorType)) { var e = function (e) { return !(void 0 !== w.config.minDate && w.currentYear === w.config.minDate.getFullYear() && e < w.config.minDate.getMonth()) && !(void 0 !== w.config.maxDate && w.currentYear === w.config.maxDate.getFullYear() && e > w.config.maxDate.getMonth()) }; w.monthsDropdownContainer.tabIndex = -1, w.monthsDropdownContainer.innerHTML = ""; for (var t = 0; t < 12; t++)if (e(t)) { var n = s("option", "flatpickr-monthDropdown-month"); n.value = new Date(w.currentYear, t).getMonth().toString(), n.textContent = h(t, w.config.shorthandCurrentMonth, w.l10n), n.tabIndex = -1, w.currentMonth === t && (n.selected = !0), w.monthsDropdownContainer.appendChild(n) } } } function U() { var e, t = s("div", "flatpickr-month"), n = window.document.createDocumentFragment(); w.config.showMonths > 1 || "static" === w.config.monthSelectorType ? e = s("span", "cur-month") : (w.monthsDropdownContainer = s("select", "flatpickr-monthDropdown-months"), w.monthsDropdownContainer.setAttribute("aria-label", w.l10n.monthAriaLabel), A(w.monthsDropdownContainer, "change", (function (e) { var t = g(e), n = parseInt(t.value, 10); w.changeMonth(n - w.currentMonth), pe("onMonthChange") })), K(), e = w.monthsDropdownContainer); var a = m("cur-year", { tabindex: "-1" }), i = a.getElementsByTagName("input")[0]; i.setAttribute("aria-label", w.l10n.yearAriaLabel), w.config.minDate && i.setAttribute("min", w.config.minDate.getFullYear().toString()), w.config.maxDate && (i.setAttribute("max", w.config.maxDate.getFullYear().toString()), i.disabled = !!w.config.minDate && w.config.minDate.getFullYear() === w.config.maxDate.getFullYear()); var o = s("div", "flatpickr-current-month"); return o.appendChild(e), o.appendChild(a), n.appendChild(o), t.appendChild(n), { container: t, yearElement: i, monthElement: e } } function q() { u(w.monthNav), w.monthNav.appendChild(w.prevMonthNav), w.config.showMonths && (w.yearElements = [], w.monthElements = []); for (var e = w.config.showMonths; e--;) { var t = U(); w.yearElements.push(t.yearElement), w.monthElements.push(t.monthElement), w.monthNav.appendChild(t.container) } w.monthNav.appendChild(w.nextMonthNav) } function $() { w.weekdayContainer ? u(w.weekdayContainer) : w.weekdayContainer = s("div", "flatpickr-weekdays"); for (var e = w.config.showMonths; e--;) { var t = s("div", "flatpickr-weekdaycontainer"); w.weekdayContainer.appendChild(t) } return z(), w.weekdayContainer } function z() { if (w.weekdayContainer) { var e = w.l10n.firstDayOfWeek, n = t(w.l10n.weekdays.shorthand); e > 0 && e < n.length && (n = t(n.splice(e, n.length), n.splice(0, e))); for (var a = w.config.showMonths; a--;)w.weekdayContainer.children[a].innerHTML = "\n      <span class='flatpickr-weekday'>\n        " + n.join("</span><span class='flatpickr-weekday'>") + "\n      </span>\n      " } } function G(e, t) { void 0 === t && (t = !0); var n = t ? e : e - w.currentMonth; n < 0 && !0 === w._hidePrevMonthArrow || n > 0 && !0 === w._hideNextMonthArrow || (w.currentMonth += n, (w.currentMonth < 0 || w.currentMonth > 11) && (w.currentYear += w.currentMonth > 11 ? 1 : -1, w.currentMonth = (w.currentMonth + 12) % 12, pe("onYearChange"), K()), J(), pe("onMonthChange"), De()) } function V(e) { return !(!w.config.appendTo || !w.config.appendTo.contains(e)) || w.calendarContainer.contains(e) } function Z(e) { if (w.isOpen && !w.config.inline) { var t = g(e), n = V(t), a = t === w.input || t === w.altInput || w.element.contains(t) || e.path && e.path.indexOf && (~e.path.indexOf(w.input) || ~e.path.indexOf(w.altInput)), i = "blur" === e.type ? a && e.relatedTarget && !V(e.relatedTarget) : !a && !n && !V(e.relatedTarget), o = !w.config.ignoredFocusElements.some((function (e) { return e.contains(t) })); i && o && (void 0 !== w.timeContainer && void 0 !== w.minuteElement && void 0 !== w.hourElement && "" !== w.input.value && void 0 !== w.input.value && I(), w.close(), w.config && "range" === w.config.mode && 1 === w.selectedDates.length && (w.clear(!1), w.redraw())) } } function Q(e) { if (!(!e || w.config.minDate && e < w.config.minDate.getFullYear() || w.config.maxDate && e > w.config.maxDate.getFullYear())) { var t = e, n = w.currentYear !== t; w.currentYear = t || w.currentYear, w.config.maxDate && w.currentYear === w.config.maxDate.getFullYear() ? w.currentMonth = Math.min(w.config.maxDate.getMonth(), w.currentMonth) : w.config.minDate && w.currentYear === w.config.minDate.getFullYear() && (w.currentMonth = Math.max(w.config.minDate.getMonth(), w.currentMonth)), n && (w.redraw(), pe("onYearChange"), K()) } } function X(e, t) { var n; void 0 === t && (t = !0); var a = w.parseDate(e, void 0, t); if (w.config.minDate && a && M(a, w.config.minDate, void 0 !== t ? t : !w.minDateHasTime) < 0 || w.config.maxDate && a && M(a, w.config.maxDate, void 0 !== t ? t : !w.maxDateHasTime) > 0) return !1; if (!w.config.enable && 0 === w.config.disable.length) return !0; if (void 0 === a) return !1; for (var i = !!w.config.enable, o = null !== (n = w.config.enable) && void 0 !== n ? n : w.config.disable, r = 0, l = void 0; r < o.length; r++) { if ("function" == typeof (l = o[r]) && l(a)) return i; if (l instanceof Date && void 0 !== a && l.getTime() === a.getTime()) return i; if ("string" == typeof l) { var c = w.parseDate(l, void 0, !0); return c && c.getTime() === a.getTime() ? i : !i } if ("object" == typeof l && void 0 !== a && l.from && l.to && a.getTime() >= l.from.getTime() && a.getTime() <= l.to.getTime()) return i } return !i } function ee(e) { return void 0 !== w.daysContainer && (-1 === e.className.indexOf("hidden") && -1 === e.className.indexOf("flatpickr-disabled") && w.daysContainer.contains(e)) } function te(e) { !(e.target === w._input) || !(w.selectedDates.length > 0 || w._input.value.length > 0) || e.relatedTarget && V(e.relatedTarget) || w.setDate(w._input.value, !0, e.target === w.altInput ? w.config.altFormat : w.config.dateFormat) } function ne(e) { var t = g(e), n = w.config.wrap ? p.contains(t) : t === w._input, a = w.config.allowInput, i = w.isOpen && (!a || !n), o = w.config.inline && n && !a; if (13 === e.keyCode && n) { if (a) return w.setDate(w._input.value, !0, t === w.altInput ? w.config.altFormat : w.config.dateFormat), t.blur(); w.open() } else if (V(t) || i || o) { var r = !!w.timeContainer && w.timeContainer.contains(t); switch (e.keyCode) { case 13: r ? (e.preventDefault(), I(), se()) : ue(e); break; case 27: e.preventDefault(), se(); break; case 8: case 46: n && !w.config.allowInput && (e.preventDefault(), w.clear()); break; case 37: case 39: if (r || n) w.hourElement && w.hourElement.focus(); else if (e.preventDefault(), void 0 !== w.daysContainer && (!1 === a || document.activeElement && ee(document.activeElement))) { var l = 39 === e.keyCode ? 1 : -1; e.ctrlKey ? (e.stopPropagation(), G(l), R(W(1), 0)) : R(void 0, l) } break; case 38: case 40: e.preventDefault(); var c = 40 === e.keyCode ? 1 : -1; w.daysContainer && void 0 !== t.$i || t === w.input || t === w.altInput ? e.ctrlKey ? (e.stopPropagation(), Q(w.currentYear - c), R(W(1), 0)) : r || R(void 0, 7 * c) : t === w.currentYearElement ? Q(w.currentYear - c) : w.config.enableTime && (!r && w.hourElement && w.hourElement.focus(), I(e), w._debouncedChange()); break; case 9: if (r) { var d = [w.hourElement, w.minuteElement, w.secondElement, w.amPM].concat(w.pluginElements).filter((function (e) { return e })), s = d.indexOf(t); if (-1 !== s) { var u = d[s + (e.shiftKey ? -1 : 1)]; e.preventDefault(), (u || w._input).focus() } } else !w.config.noCalendar && w.daysContainer && w.daysContainer.contains(t) && e.shiftKey && (e.preventDefault(), w._input.focus()) } } if (void 0 !== w.amPM && t === w.amPM) switch (e.key) { case w.l10n.amPM[0].charAt(0): case w.l10n.amPM[0].charAt(0).toLowerCase(): w.amPM.textContent = w.l10n.amPM[0], S(), be(); break; case w.l10n.amPM[1].charAt(0): case w.l10n.amPM[1].charAt(0).toLowerCase(): w.amPM.textContent = w.l10n.amPM[1], S(), be() }(n || V(t)) && pe("onKeyDown", e) } function ae(e) { if (1 === w.selectedDates.length && (!e || e.classList.contains("flatpickr-day") && !e.classList.contains("flatpickr-disabled"))) { for (var t = e ? e.dateObj.getTime() : w.days.firstElementChild.dateObj.getTime(), n = w.parseDate(w.selectedDates[0], void 0, !0).getTime(), a = Math.min(t, w.selectedDates[0].getTime()), i = Math.max(t, w.selectedDates[0].getTime()), o = !1, r = 0, l = 0, c = a; c < i; c += y)X(new Date(c), !0) || (o = o || c > a && c < i, c < n && (!r || c > r) ? r = c : c > n && (!l || c < l) && (l = c)); for (var d = 0; d < w.config.showMonths; d++)for (var s = w.daysContainer.children[d], u = function (a, i) { var c, d, u, f = s.children[a], m = f.dateObj.getTime(), g = r > 0 && m < r || l > 0 && m > l; return g ? (f.classList.add("notAllowed"), ["inRange", "startRange", "endRange"].forEach((function (e) { f.classList.remove(e) })), "continue") : o && !g ? "continue" : (["startRange", "inRange", "endRange", "notAllowed"].forEach((function (e) { f.classList.remove(e) })), void (void 0 !== e && (e.classList.add(t <= w.selectedDates[0].getTime() ? "startRange" : "endRange"), n < t && m === n ? f.classList.add("startRange") : n > t && m === n && f.classList.add("endRange"), m >= r && (0 === l || m <= l) && (d = n, u = t, (c = m) > Math.min(d, u) && c < Math.max(d, u)) && f.classList.add("inRange")))) }, f = 0, m = s.children.length; f < m; f++)u(f) } } function ie() { !w.isOpen || w.config.static || w.config.inline || ce() } function oe(e) { return function (t) { var n = w.config["_" + e + "Date"] = w.parseDate(t, w.config.dateFormat), a = w.config["_" + ("min" === e ? "max" : "min") + "Date"]; void 0 !== n && (w["min" === e ? "minDateHasTime" : "maxDateHasTime"] = n.getHours() > 0 || n.getMinutes() > 0 || n.getSeconds() > 0), w.selectedDates && (w.selectedDates = w.selectedDates.filter((function (e) { return X(e) })), w.selectedDates.length || "min" !== e || _(n), be()), w.daysContainer && (de(), void 0 !== n ? w.currentYearElement[e] = n.getFullYear().toString() : w.currentYearElement.removeAttribute(e), w.currentYearElement.disabled = !!a && void 0 !== n && a.getFullYear() === n.getFullYear()) } } function re() { return w.config.wrap ? p.querySelector("[data-input]") : p } function le() { "object" != typeof w.config.locale && void 0 === T.l10ns[w.config.locale] && w.config.errorHandler(new Error("flatpickr: invalid locale " + w.config.locale)), w.l10n = e(e({}, T.l10ns.default), "object" == typeof w.config.locale ? w.config.locale : "default" !== w.config.locale ? T.l10ns[w.config.locale] : void 0), D.K = "(" + w.l10n.amPM[0] + "|" + w.l10n.amPM[1] + "|" + w.l10n.amPM[0].toLowerCase() + "|" + w.l10n.amPM[1].toLowerCase() + ")", void 0 === e(e({}, v), JSON.parse(JSON.stringify(p.dataset || {}))).time_24hr && void 0 === T.defaultConfig.time_24hr && (w.config.time_24hr = w.l10n.time_24hr), w.formatDate = b(w), w.parseDate = C({ config: w.config, l10n: w.l10n }) } function ce(e) { if ("function" != typeof w.config.position) { if (void 0 !== w.calendarContainer) { pe("onPreCalendarPosition"); var t = e || w._positionElement, n = Array.prototype.reduce.call(w.calendarContainer.children, (function (e, t) { return e + t.offsetHeight }), 0), a = w.calendarContainer.offsetWidth, i = w.config.position.split(" "), o = i[0], r = i.length > 1 ? i[1] : null, l = t.getBoundingClientRect(), c = window.innerHeight - l.bottom, s = "above" === o || "below" !== o && c < n && l.top > n, u = window.pageYOffset + l.top + (s ? -n - 2 : t.offsetHeight + 2); if (d(w.calendarContainer, "arrowTop", !s), d(w.calendarContainer, "arrowBottom", s), !w.config.inline) { var f = window.pageXOffset + l.left, m = !1, g = !1; "center" === r ? (f -= (a - l.width) / 2, m = !0) : "right" === r && (f -= a - l.width, g = !0), d(w.calendarContainer, "arrowLeft", !m && !g), d(w.calendarContainer, "arrowCenter", m), d(w.calendarContainer, "arrowRight", g); var p = window.document.body.offsetWidth - (window.pageXOffset + l.right), h = f + a > window.document.body.offsetWidth, v = p + a > window.document.body.offsetWidth; if (d(w.calendarContainer, "rightMost", h), !w.config.static) if (w.calendarContainer.style.top = u + "px", h) if (v) { var D = function () { for (var e = null, t = 0; t < document.styleSheets.length; t++) { var n = document.styleSheets[t]; try { n.cssRules } catch (e) { continue } e = n; break } return null != e ? e : (a = document.createElement("style"), document.head.appendChild(a), a.sheet); var a }(); if (void 0 === D) return; var b = window.document.body.offsetWidth, C = Math.max(0, b / 2 - a / 2), M = D.cssRules.length, y = "{left:" + l.left + "px;right:auto;}"; d(w.calendarContainer, "rightMost", !1), d(w.calendarContainer, "centerMost", !0), D.insertRule(".flatpickr-calendar.centerMost:before,.flatpickr-calendar.centerMost:after" + y, M), w.calendarContainer.style.left = C + "px", w.calendarContainer.style.right = "auto" } else w.calendarContainer.style.left = "auto", w.calendarContainer.style.right = p + "px"; else w.calendarContainer.style.left = f + "px", w.calendarContainer.style.right = "auto" } } } else w.config.position(w, e) } function de() { w.config.noCalendar || w.isMobile || (K(), De(), J()) } function se() { w._input.focus(), -1 !== window.navigator.userAgent.indexOf("MSIE") || void 0 !== navigator.msMaxTouchPoints ? setTimeout(w.close, 0) : w.close() } function ue(e) { e.preventDefault(), e.stopPropagation(); var t = f(g(e), (function (e) { return e.classList && e.classList.contains("flatpickr-day") && !e.classList.contains("flatpickr-disabled") && !e.classList.contains("notAllowed") })); if (void 0 !== t) { var n = t, a = w.latestSelectedDateObj = new Date(n.dateObj.getTime()), i = (a.getMonth() < w.currentMonth || a.getMonth() > w.currentMonth + w.config.showMonths - 1) && "range" !== w.config.mode; if (w.selectedDateElem = n, "single" === w.config.mode) w.selectedDates = [a]; else if ("multiple" === w.config.mode) { var o = ve(a); o ? w.selectedDates.splice(parseInt(o), 1) : w.selectedDates.push(a) } else "range" === w.config.mode && (2 === w.selectedDates.length && w.clear(!1, !1), w.latestSelectedDateObj = a, w.selectedDates.push(a), 0 !== M(a, w.selectedDates[0], !0) && w.selectedDates.sort((function (e, t) { return e.getTime() - t.getTime() }))); if (S(), i) { var r = w.currentYear !== a.getFullYear(); w.currentYear = a.getFullYear(), w.currentMonth = a.getMonth(), r && (pe("onYearChange"), K()), pe("onMonthChange") } if (De(), J(), be(), i || "range" === w.config.mode || 1 !== w.config.showMonths ? void 0 !== w.selectedDateElem && void 0 === w.hourElement && w.selectedDateElem && w.selectedDateElem.focus() : L(n), void 0 !== w.hourElement && void 0 !== w.hourElement && w.hourElement.focus(), w.config.closeOnSelect) { var l = "single" === w.config.mode && !w.config.enableTime, c = "range" === w.config.mode && 2 === w.selectedDates.length && !w.config.enableTime; (l || c) && se() } N() } } w.parseDate = C({ config: w.config, l10n: w.l10n }), w._handlers = [], w.pluginElements = [], w.loadedPlugins = [], w._bind = A, w._setHoursFromDate = _, w._positionCalendar = ce, w.changeMonth = G, w.changeYear = Q, w.clear = function (e, t) { void 0 === e && (e = !0); void 0 === t && (t = !0); w.input.value = "", void 0 !== w.altInput && (w.altInput.value = ""); void 0 !== w.mobileInput && (w.mobileInput.value = ""); w.selectedDates = [], w.latestSelectedDateObj = void 0, !0 === t && (w.currentYear = w._initialDate.getFullYear(), w.currentMonth = w._initialDate.getMonth()); if (!0 === w.config.enableTime) { var n = x(w.config), a = n.hours, i = n.minutes, o = n.seconds; O(a, i, o) } w.redraw(), e && pe("onChange") }, w.close = function () { w.isOpen = !1, w.isMobile || (void 0 !== w.calendarContainer && w.calendarContainer.classList.remove("open"), void 0 !== w._input && w._input.classList.remove("active")); pe("onClose") }, w._createElement = s, w.destroy = function () { void 0 !== w.config && pe("onDestroy"); for (var e = w._handlers.length; e--;)w._handlers[e].remove(); if (w._handlers = [], w.mobileInput) w.mobileInput.parentNode && w.mobileInput.parentNode.removeChild(w.mobileInput), w.mobileInput = void 0; else if (w.calendarContainer && w.calendarContainer.parentNode) if (w.config.static && w.calendarContainer.parentNode) { var t = w.calendarContainer.parentNode; if (t.lastChild && t.removeChild(t.lastChild), t.parentNode) { for (; t.firstChild;)t.parentNode.insertBefore(t.firstChild, t); t.parentNode.removeChild(t) } } else w.calendarContainer.parentNode.removeChild(w.calendarContainer); w.altInput && (w.input.type = "text", w.altInput.parentNode && w.altInput.parentNode.removeChild(w.altInput), delete w.altInput); w.input && (w.input.type = w.input._type, w.input.classList.remove("flatpickr-input"), w.input.removeAttribute("readonly"));["_showTimeInput", "latestSelectedDateObj", "_hideNextMonthArrow", "_hidePrevMonthArrow", "__hideNextMonthArrow", "__hidePrevMonthArrow", "isMobile", "isOpen", "selectedDateElem", "minDateHasTime", "maxDateHasTime", "days", "daysContainer", "_input", "_positionElement", "innerContainer", "rContainer", "monthNav", "todayDateElem", "calendarContainer", "weekdayContainer", "prevMonthNav", "nextMonthNav", "monthsDropdownContainer", "currentMonthElement", "currentYearElement", "navigationCurrentMonth", "selectedDateElem", "config"].forEach((function (e) { try { delete w[e] } catch (e) { } })) }, w.isEnabled = X, w.jumpToDate = P, w.open = function (e, t) { void 0 === t && (t = w._positionElement); if (!0 === w.isMobile) { if (e) { e.preventDefault(); var n = g(e); n && n.blur() } return void 0 !== w.mobileInput && (w.mobileInput.focus(), w.mobileInput.click()), void pe("onOpen") } if (w._input.disabled || w.config.inline) return; var a = w.isOpen; w.isOpen = !0, a || (w.calendarContainer.classList.add("open"), w._input.classList.add("active"), pe("onOpen"), ce(t)); !0 === w.config.enableTime && !0 === w.config.noCalendar && (!1 !== w.config.allowInput || void 0 !== e && w.timeContainer.contains(e.relatedTarget) || setTimeout((function () { return w.hourElement.select() }), 50)) }, w.redraw = de, w.set = function (e, t) { if (null !== e && "object" == typeof e) for (var a in Object.assign(w.config, e), e) void 0 !== fe[a] && fe[a].forEach((function (e) { return e() })); else w.config[e] = t, void 0 !== fe[e] ? fe[e].forEach((function (e) { return e() })) : n.indexOf(e) > -1 && (w.config[e] = c(t)); w.redraw(), be(!0) }, w.setDate = function (e, t, n) { void 0 === t && (t = !1); void 0 === n && (n = w.config.dateFormat); if (0 !== e && !e || e instanceof Array && 0 === e.length) return w.clear(t); me(e, n), w.latestSelectedDateObj = w.selectedDates[w.selectedDates.length - 1], w.redraw(), P(void 0, t), _(), 0 === w.selectedDates.length && w.clear(!1); be(t), t && pe("onChange") }, w.toggle = function (e) { if (!0 === w.isOpen) return w.close(); w.open(e) }; var fe = { locale: [le, z], showMonths: [q, k, $], minDate: [P], maxDate: [P], clickOpens: [function () { !0 === w.config.clickOpens ? (A(w._input, "focus", w.open), A(w._input, "click", w.open)) : (w._input.removeEventListener("focus", w.open), w._input.removeEventListener("click", w.open)) }] }; function me(e, t) { var n = []; if (e instanceof Array) n = e.map((function (e) { return w.parseDate(e, t) })); else if (e instanceof Date || "number" == typeof e) n = [w.parseDate(e, t)]; else if ("string" == typeof e) switch (w.config.mode) { case "single": case "time": n = [w.parseDate(e, t)]; break; case "multiple": n = e.split(w.config.conjunction).map((function (e) { return w.parseDate(e, t) })); break; case "range": n = e.split(w.l10n.rangeSeparator).map((function (e) { return w.parseDate(e, t) })) } else w.config.errorHandler(new Error("Invalid date supplied: " + JSON.stringify(e))); w.selectedDates = w.config.allowInvalidPreload ? n : n.filter((function (e) { return e instanceof Date && X(e, !1) })), "range" === w.config.mode && w.selectedDates.sort((function (e, t) { return e.getTime() - t.getTime() })) } function ge(e) { return e.slice().map((function (e) { return "string" == typeof e || "number" == typeof e || e instanceof Date ? w.parseDate(e, void 0, !0) : e && "object" == typeof e && e.from && e.to ? { from: w.parseDate(e.from, void 0), to: w.parseDate(e.to, void 0) } : e })).filter((function (e) { return e })) } function pe(e, t) { if (void 0 !== w.config) { var n = w.config[e]; if (void 0 !== n && n.length > 0) for (var a = 0; n[a] && a < n.length; a++)n[a](w.selectedDates, w.input.value, w, t); "onChange" === e && (w.input.dispatchEvent(he("change")), w.input.dispatchEvent(he("input"))) } } function he(e) { var t = document.createEvent("Event"); return t.initEvent(e, !0, !0), t } function ve(e) { for (var t = 0; t < w.selectedDates.length; t++)if (0 === M(w.selectedDates[t], e)) return "" + t; return !1 } function De() { w.config.noCalendar || w.isMobile || !w.monthNav || (w.yearElements.forEach((function (e, t) { var n = new Date(w.currentYear, w.currentMonth, 1); n.setMonth(w.currentMonth + t), w.config.showMonths > 1 || "static" === w.config.monthSelectorType ? w.monthElements[t].textContent = h(n.getMonth(), w.config.shorthandCurrentMonth, w.l10n) + " " : w.monthsDropdownContainer.value = n.getMonth().toString(), e.value = n.getFullYear().toString() })), w._hidePrevMonthArrow = void 0 !== w.config.minDate && (w.currentYear === w.config.minDate.getFullYear() ? w.currentMonth <= w.config.minDate.getMonth() : w.currentYear < w.config.minDate.getFullYear()), w._hideNextMonthArrow = void 0 !== w.config.maxDate && (w.currentYear === w.config.maxDate.getFullYear() ? w.currentMonth + 1 > w.config.maxDate.getMonth() : w.currentYear > w.config.maxDate.getFullYear())) } function we(e) { return w.selectedDates.map((function (t) { return w.formatDate(t, e) })).filter((function (e, t, n) { return "range" !== w.config.mode || w.config.enableTime || n.indexOf(e) === t })).join("range" !== w.config.mode ? w.config.conjunction : w.l10n.rangeSeparator) } function be(e) { void 0 === e && (e = !0), void 0 !== w.mobileInput && w.mobileFormatStr && (w.mobileInput.value = void 0 !== w.latestSelectedDateObj ? w.formatDate(w.latestSelectedDateObj, w.mobileFormatStr) : ""), w.input.value = we(w.config.dateFormat), void 0 !== w.altInput && (w.altInput.value = we(w.config.altFormat)), !1 !== e && pe("onValueUpdate") } function Ce(e) { var t = g(e), n = w.prevMonthNav.contains(t), a = w.nextMonthNav.contains(t); n || a ? G(n ? -1 : 1) : w.yearElements.indexOf(t) >= 0 ? t.select() : t.classList.contains("arrowUp") ? w.changeYear(w.currentYear + 1) : t.classList.contains("arrowDown") && w.changeYear(w.currentYear - 1) } return function () { w.element = w.input = p, w.isOpen = !1, function () { var t = ["wrap", "weekNumbers", "allowInput", "allowInvalidPreload", "clickOpens", "time_24hr", "enableTime", "noCalendar", "altInput", "shorthandCurrentMonth", "inline", "static", "enableSeconds", "disableMobile"], i = e(e({}, JSON.parse(JSON.stringify(p.dataset || {}))), v), o = {}; w.config.parseDate = i.parseDate, w.config.formatDate = i.formatDate, Object.defineProperty(w.config, "enable", { get: function () { return w.config._enable }, set: function (e) { w.config._enable = ge(e) } }), Object.defineProperty(w.config, "disable", { get: function () { return w.config._disable }, set: function (e) { w.config._disable = ge(e) } }); var r = "time" === i.mode; if (!i.dateFormat && (i.enableTime || r)) { var l = T.defaultConfig.dateFormat || a.dateFormat; o.dateFormat = i.noCalendar || r ? "H:i" + (i.enableSeconds ? ":S" : "") : l + " H:i" + (i.enableSeconds ? ":S" : "") } if (i.altInput && (i.enableTime || r) && !i.altFormat) { var d = T.defaultConfig.altFormat || a.altFormat; o.altFormat = i.noCalendar || r ? "h:i" + (i.enableSeconds ? ":S K" : " K") : d + " h:i" + (i.enableSeconds ? ":S" : "") + " K" } Object.defineProperty(w.config, "minDate", { get: function () { return w.config._minDate }, set: oe("min") }), Object.defineProperty(w.config, "maxDate", { get: function () { return w.config._maxDate }, set: oe("max") }); var s = function (e) { return function (t) { w.config["min" === e ? "_minTime" : "_maxTime"] = w.parseDate(t, "H:i:S") } }; Object.defineProperty(w.config, "minTime", { get: function () { return w.config._minTime }, set: s("min") }), Object.defineProperty(w.config, "maxTime", { get: function () { return w.config._maxTime }, set: s("max") }), "time" === i.mode && (w.config.noCalendar = !0, w.config.enableTime = !0); Object.assign(w.config, o, i); for (var u = 0; u < t.length; u++)w.config[t[u]] = !0 === w.config[t[u]] || "true" === w.config[t[u]]; n.filter((function (e) { return void 0 !== w.config[e] })).forEach((function (e) { w.config[e] = c(w.config[e] || []).map(E) })), w.isMobile = !w.config.disableMobile && !w.config.inline && "single" === w.config.mode && !w.config.disable.length && !w.config.enable && !w.config.weekNumbers && /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent); for (u = 0; u < w.config.plugins.length; u++) { var f = w.config.plugins[u](w) || {}; for (var m in f) n.indexOf(m) > -1 ? w.config[m] = c(f[m]).map(E).concat(w.config[m]) : void 0 === i[m] && (w.config[m] = f[m]) } i.altInputClass || (w.config.altInputClass = re().className + " " + w.config.altInputClass); pe("onParseConfig") }(), le(), function () { if (w.input = re(), !w.input) return void w.config.errorHandler(new Error("Invalid input element specified")); w.input._type = w.input.type, w.input.type = "text", w.input.classList.add("flatpickr-input"), w._input = w.input, w.config.altInput && (w.altInput = s(w.input.nodeName, w.config.altInputClass), w._input = w.altInput, w.altInput.placeholder = w.input.placeholder, w.altInput.disabled = w.input.disabled, w.altInput.required = w.input.required, w.altInput.tabIndex = w.input.tabIndex, w.altInput.type = "text", w.input.setAttribute("type", "hidden"), !w.config.static && w.input.parentNode && w.input.parentNode.insertBefore(w.altInput, w.input.nextSibling)); w.config.allowInput || w._input.setAttribute("readonly", "readonly"); w._positionElement = w.config.positionElement || w._input }(), function () { w.selectedDates = [], w.now = w.parseDate(w.config.now) || new Date; var e = w.config.defaultDate || ("INPUT" !== w.input.nodeName && "TEXTAREA" !== w.input.nodeName || !w.input.placeholder || w.input.value !== w.input.placeholder ? w.input.value : null); e && me(e, w.config.dateFormat); w._initialDate = w.selectedDates.length > 0 ? w.selectedDates[0] : w.config.minDate && w.config.minDate.getTime() > w.now.getTime() ? w.config.minDate : w.config.maxDate && w.config.maxDate.getTime() < w.now.getTime() ? w.config.maxDate : w.now, w.currentYear = w._initialDate.getFullYear(), w.currentMonth = w._initialDate.getMonth(), w.selectedDates.length > 0 && (w.latestSelectedDateObj = w.selectedDates[0]); void 0 !== w.config.minTime && (w.config.minTime = w.parseDate(w.config.minTime, "H:i")); void 0 !== w.config.maxTime && (w.config.maxTime = w.parseDate(w.config.maxTime, "H:i")); w.minDateHasTime = !!w.config.minDate && (w.config.minDate.getHours() > 0 || w.config.minDate.getMinutes() > 0 || w.config.minDate.getSeconds() > 0), w.maxDateHasTime = !!w.config.maxDate && (w.config.maxDate.getHours() > 0 || w.config.maxDate.getMinutes() > 0 || w.config.maxDate.getSeconds() > 0) }(), w.utils = { getDaysInMonth: function (e, t) { return void 0 === e && (e = w.currentMonth), void 0 === t && (t = w.currentYear), 1 === e && (t % 4 == 0 && t % 100 != 0 || t % 400 == 0) ? 29 : w.l10n.daysInMonth[e] } }, w.isMobile || function () { var e = window.document.createDocumentFragment(); if (w.calendarContainer = s("div", "flatpickr-calendar"), w.calendarContainer.tabIndex = -1, !w.config.noCalendar) { if (e.appendChild((w.monthNav = s("div", "flatpickr-months"), w.yearElements = [], w.monthElements = [], w.prevMonthNav = s("span", "flatpickr-prev-month"), w.prevMonthNav.innerHTML = w.config.prevArrow, w.nextMonthNav = s("span", "flatpickr-next-month"), w.nextMonthNav.innerHTML = w.config.nextArrow, q(), Object.defineProperty(w, "_hidePrevMonthArrow", { get: function () { return w.__hidePrevMonthArrow }, set: function (e) { w.__hidePrevMonthArrow !== e && (d(w.prevMonthNav, "flatpickr-disabled", e), w.__hidePrevMonthArrow = e) } }), Object.defineProperty(w, "_hideNextMonthArrow", { get: function () { return w.__hideNextMonthArrow }, set: function (e) { w.__hideNextMonthArrow !== e && (d(w.nextMonthNav, "flatpickr-disabled", e), w.__hideNextMonthArrow = e) } }), w.currentYearElement = w.yearElements[0], De(), w.monthNav)), w.innerContainer = s("div", "flatpickr-innerContainer"), w.config.weekNumbers) { var t = function () { w.calendarContainer.classList.add("hasWeeks"); var e = s("div", "flatpickr-weekwrapper"); e.appendChild(s("span", "flatpickr-weekday", w.l10n.weekAbbreviation)); var t = s("div", "flatpickr-weeks"); return e.appendChild(t), { weekWrapper: e, weekNumbers: t } }(), n = t.weekWrapper, a = t.weekNumbers; w.innerContainer.appendChild(n), w.weekNumbers = a, w.weekWrapper = n } w.rContainer = s("div", "flatpickr-rContainer"), w.rContainer.appendChild($()), w.daysContainer || (w.daysContainer = s("div", "flatpickr-days"), w.daysContainer.tabIndex = -1), J(), w.rContainer.appendChild(w.daysContainer), w.innerContainer.appendChild(w.rContainer), e.appendChild(w.innerContainer) } w.config.enableTime && e.appendChild(function () { w.calendarContainer.classList.add("hasTime"), w.config.noCalendar && w.calendarContainer.classList.add("noCalendar"); var e = x(w.config); w.timeContainer = s("div", "flatpickr-time"), w.timeContainer.tabIndex = -1; var t = s("span", "flatpickr-time-separator", ":"), n = m("flatpickr-hour", { "aria-label": w.l10n.hourAriaLabel }); w.hourElement = n.getElementsByTagName("input")[0]; var a = m("flatpickr-minute", { "aria-label": w.l10n.minuteAriaLabel }); w.minuteElement = a.getElementsByTagName("input")[0], w.hourElement.tabIndex = w.minuteElement.tabIndex = -1, w.hourElement.value = o(w.latestSelectedDateObj ? w.latestSelectedDateObj.getHours() : w.config.time_24hr ? e.hours : function (e) { switch (e % 24) { case 0: case 12: return 12; default: return e % 12 } }(e.hours)), w.minuteElement.value = o(w.latestSelectedDateObj ? w.latestSelectedDateObj.getMinutes() : e.minutes), w.hourElement.setAttribute("step", w.config.hourIncrement.toString()), w.minuteElement.setAttribute("step", w.config.minuteIncrement.toString()), w.hourElement.setAttribute("min", w.config.time_24hr ? "0" : "1"), w.hourElement.setAttribute("max", w.config.time_24hr ? "23" : "12"), w.hourElement.setAttribute("maxlength", "2"), w.minuteElement.setAttribute("min", "0"), w.minuteElement.setAttribute("max", "59"), w.minuteElement.setAttribute("maxlength", "2"), w.timeContainer.appendChild(n), w.timeContainer.appendChild(t), w.timeContainer.appendChild(a), w.config.time_24hr && w.timeContainer.classList.add("time24hr"); if (w.config.enableSeconds) { w.timeContainer.classList.add("hasSeconds"); var i = m("flatpickr-second"); w.secondElement = i.getElementsByTagName("input")[0], w.secondElement.value = o(w.latestSelectedDateObj ? w.latestSelectedDateObj.getSeconds() : e.seconds), w.secondElement.setAttribute("step", w.minuteElement.getAttribute("step")), w.secondElement.setAttribute("min", "0"), w.secondElement.setAttribute("max", "59"), w.secondElement.setAttribute("maxlength", "2"), w.timeContainer.appendChild(s("span", "flatpickr-time-separator", ":")), w.timeContainer.appendChild(i) } w.config.time_24hr || (w.amPM = s("span", "flatpickr-am-pm", w.l10n.amPM[r((w.latestSelectedDateObj ? w.hourElement.value : w.config.defaultHour) > 11)]), w.amPM.title = w.l10n.toggleTitle, w.amPM.tabIndex = -1, w.timeContainer.appendChild(w.amPM)); return w.timeContainer }()); d(w.calendarContainer, "rangeMode", "range" === w.config.mode), d(w.calendarContainer, "animate", !0 === w.config.animate), d(w.calendarContainer, "multiMonth", w.config.showMonths > 1), w.calendarContainer.appendChild(e); var i = void 0 !== w.config.appendTo && void 0 !== w.config.appendTo.nodeType; if ((w.config.inline || w.config.static) && (w.calendarContainer.classList.add(w.config.inline ? "inline" : "static"), w.config.inline && (!i && w.element.parentNode ? w.element.parentNode.insertBefore(w.calendarContainer, w._input.nextSibling) : void 0 !== w.config.appendTo && w.config.appendTo.appendChild(w.calendarContainer)), w.config.static)) { var l = s("div", "flatpickr-wrapper"); w.element.parentNode && w.element.parentNode.insertBefore(l, w.element), l.appendChild(w.element), w.altInput && l.appendChild(w.altInput), l.appendChild(w.calendarContainer) } w.config.static || w.config.inline || (void 0 !== w.config.appendTo ? w.config.appendTo : window.document.body).appendChild(w.calendarContainer) }(), function () { w.config.wrap && ["open", "close", "toggle", "clear"].forEach((function (e) { Array.prototype.forEach.call(w.element.querySelectorAll("[data-" + e + "]"), (function (t) { return A(t, "click", w[e]) })) })); if (w.isMobile) return void function () { var e = w.config.enableTime ? w.config.noCalendar ? "time" : "datetime-local" : "date"; w.mobileInput = s("input", w.input.className + " flatpickr-mobile"), w.mobileInput.tabIndex = 1, w.mobileInput.type = e, w.mobileInput.disabled = w.input.disabled, w.mobileInput.required = w.input.required, w.mobileInput.placeholder = w.input.placeholder, w.mobileFormatStr = "datetime-local" === e ? "Y-m-d\\TH:i:S" : "date" === e ? "Y-m-d" : "H:i:S", w.selectedDates.length > 0 && (w.mobileInput.defaultValue = w.mobileInput.value = w.formatDate(w.selectedDates[0], w.mobileFormatStr)); w.config.minDate && (w.mobileInput.min = w.formatDate(w.config.minDate, "Y-m-d")); w.config.maxDate && (w.mobileInput.max = w.formatDate(w.config.maxDate, "Y-m-d")); w.input.getAttribute("step") && (w.mobileInput.step = String(w.input.getAttribute("step"))); w.input.type = "hidden", void 0 !== w.altInput && (w.altInput.type = "hidden"); try { w.input.parentNode && w.input.parentNode.insertBefore(w.mobileInput, w.input.nextSibling) } catch (e) { } A(w.mobileInput, "change", (function (e) { w.setDate(g(e).value, !1, w.mobileFormatStr), pe("onChange"), pe("onClose") })) }(); var e = l(ie, 50); w._debouncedChange = l(N, 300), w.daysContainer && !/iPhone|iPad|iPod/i.test(navigator.userAgent) && A(w.daysContainer, "mouseover", (function (e) { "range" === w.config.mode && ae(g(e)) })); A(window.document.body, "keydown", ne), w.config.inline || w.config.static || A(window, "resize", e); void 0 !== window.ontouchstart ? A(window.document, "touchstart", Z) : A(window.document, "mousedown", Z); A(window.document, "focus", Z, { capture: !0 }), !0 === w.config.clickOpens && (A(w._input, "focus", w.open), A(w._input, "click", w.open)); void 0 !== w.daysContainer && (A(w.monthNav, "click", Ce), A(w.monthNav, ["keyup", "increment"], F), A(w.daysContainer, "click", ue)); if (void 0 !== w.timeContainer && void 0 !== w.minuteElement && void 0 !== w.hourElement) { var t = function (e) { return g(e).select() }; A(w.timeContainer, ["increment"], I), A(w.timeContainer, "blur", I, { capture: !0 }), A(w.timeContainer, "click", Y), A([w.hourElement, w.minuteElement], ["focus", "click"], t), void 0 !== w.secondElement && A(w.secondElement, "focus", (function () { return w.secondElement && w.secondElement.select() })), void 0 !== w.amPM && A(w.amPM, "click", (function (e) { I(e), N() })) } w.config.allowInput && A(w._input, "blur", te) }(), (w.selectedDates.length || w.config.noCalendar) && (w.config.enableTime && _(w.config.noCalendar ? w.latestSelectedDateObj : void 0), be(!1)), k(); var t = /^((?!chrome|android).)*safari/i.test(navigator.userAgent); !w.isMobile && t && ce(), pe("onReady") }(), w } function k(e, t) { for (var n = Array.prototype.slice.call(e).filter((function (e) { return e instanceof HTMLElement })), a = [], i = 0; i < n.length; i++) { var o = n[i]; try { if (null !== o.getAttribute("data-fp-omit")) continue; void 0 !== o._flatpickr && (o._flatpickr.destroy(), o._flatpickr = void 0), o._flatpickr = E(o, t || {}), a.push(o._flatpickr) } catch (e) { console.error(e) } } return 1 === a.length ? a[0] : a } "undefined" != typeof HTMLElement && "undefined" != typeof HTMLCollection && "undefined" != typeof NodeList && (HTMLCollection.prototype.flatpickr = NodeList.prototype.flatpickr = function (e) { return k(this, e) }, HTMLElement.prototype.flatpickr = function (e) { return k([this], e) }); var T = function (e, t) { return "string" == typeof e ? k(window.document.querySelectorAll(e), t) : e instanceof Node ? k([e], t) : k(e, t) }; return T.defaultConfig = {}, T.l10ns = { en: e({}, i), default: e({}, i) }, T.localize = function (t) { T.l10ns.default = e(e({}, T.l10ns.default), t) }, T.setDefaults = function (t) { T.defaultConfig = e(e({}, T.defaultConfig), t) }, T.parseDate = C({}), T.formatDate = b({}), T.compareDates = M, "undefined" != typeof jQuery && void 0 !== jQuery.fn && (jQuery.fn.flatpickr = function (e) { return k(this, e) }), Date.prototype.fp_incr = function (e) { return new Date(this.getFullYear(), this.getMonth(), this.getDate() + ("string" == typeof e ? parseInt(e, 10) : e)) }, "undefined" != typeof window && (window.flatpickr = T), T }));
!function (e, t) { "object" == typeof exports && "undefined" != typeof module ? module.exports = t() : "function" == typeof define && define.amd ? define(t) : (e = "undefined" != typeof globalThis ? globalThis : e || self).monthSelectPlugin = t() }(this, function () { "use strict"; var e = function () { return (e = Object.assign || function (e) { for (var t, n = 1, a = arguments.length; n < a; n++)for (var o in t = arguments[n]) Object.prototype.hasOwnProperty.call(t, o) && (e[o] = t[o]); return e }).apply(this, arguments) }, t = function (e, t, n) { return n.months[t ? "shorthand" : "longhand"][e] }; var n = { shorthand: !1, dateFormat: "F Y", altFormat: "F Y", theme: "light" }; return function (a) { var o = e(e({}, n), a); return function (e) { e.config.dateFormat = o.dateFormat, e.config.altFormat = o.altFormat; var n = { monthsContainer: null }; function a() { if (e.rContainer) { for (var t = e.rContainer.querySelectorAll(".flatpickr-monthSelect-month.selected"), n = 0; n < t.length; n++)t[n].classList.remove("selected"); var a = (e.selectedDates[0] || new Date).getMonth(), o = e.rContainer.querySelector(".flatpickr-monthSelect-month:nth-child(" + (a + 1) + ")"); o && o.classList.add("selected") } } function r() { var t = e.selectedDates[0]; t && ((t = new Date(t)).setFullYear(e.currentYear), e.config.minDate && t < e.config.minDate && (t = e.config.minDate), e.config.maxDate && t > e.config.maxDate && (t = e.config.maxDate), e.currentYear = t.getFullYear()), e.currentYearElement.value = String(e.currentYear), e.rContainer && e.rContainer.querySelectorAll(".flatpickr-monthSelect-month").forEach(function (t) { t.dateObj.setFullYear(e.currentYear), e.config.minDate && t.dateObj < e.config.minDate || e.config.maxDate && t.dateObj > e.config.maxDate ? t.classList.add("disabled") : t.classList.remove("disabled") }), a() } function i(t) { t.preventDefault(), t.stopPropagation(); var n = function (e) { try { return "function" == typeof e.composedPath ? e.composedPath()[0] : e.target } catch (t) { return e.target } }(t); n instanceof Element && !n.classList.contains("disabled") && (c(n.dateObj), e.close()) } function c(t) { var n = new Date(t); n.setFullYear(e.currentYear), e.setDate(n, !0), a() } var l = { 37: -1, 39: 1, 40: 3, 38: -3 }; return { onParseConfig: function () { e.config.mode = "single", e.config.enableTime = !1 }, onValueUpdate: a, onKeyDown: function (t, a, o, r) { var i = void 0 !== l[r.keyCode]; if ((i || 13 === r.keyCode) && e.rContainer && n.monthsContainer) { var s = e.rContainer.querySelector(".flatpickr-monthSelect-month.selected"), d = Array.prototype.indexOf.call(n.monthsContainer.children, document.activeElement); if (-1 === d) { var f = s || n.monthsContainer.firstElementChild; f.focus(), d = f.$i } i ? n.monthsContainer.children[(12 + d + l[r.keyCode]) % 12].focus() : 13 === r.keyCode && n.monthsContainer.contains(document.activeElement) && c(document.activeElement.dateObj) } }, onReady: [function () { e.currentMonth = 0 }, function () { if (e.rContainer && e.daysContainer && e.weekdayContainer) { e.rContainer.removeChild(e.daysContainer), e.rContainer.removeChild(e.weekdayContainer); for (var t = 0; t < e.monthElements.length; t++) { var n = e.monthElements[t]; n.parentNode && n.parentNode.removeChild(n) } } }, function () { e._bind(e.prevMonthNav, "click", function (t) { t.preventDefault(), t.stopPropagation(), e.changeYear(e.currentYear - 1), r() }), e._bind(e.nextMonthNav, "click", function (t) { t.preventDefault(), t.stopPropagation(), e.changeYear(e.currentYear + 1), r() }) }, function () { if (e.rContainer) { n.monthsContainer = e._createElement("div", "flatpickr-monthSelect-months"), n.monthsContainer.tabIndex = -1, e.calendarContainer.classList.add("flatpickr-monthSelect-theme-" + o.theme); for (var a = 0; a < 12; a++) { var r = e._createElement("span", "flatpickr-monthSelect-month"); r.dateObj = new Date(e.currentYear, a), r.$i = a, r.textContent = t(a, o.shorthand, e.l10n), r.tabIndex = -1, r.addEventListener("click", i), n.monthsContainer.appendChild(r), (e.config.minDate && r.dateObj < e.config.minDate || e.config.maxDate && r.dateObj > e.config.maxDate) && r.classList.add("disabled") } e.rContainer.appendChild(n.monthsContainer) } }, a, function () { e.loadedPlugins.push("monthSelect") }], onDestroy: function () { if (null !== n.monthsContainer) for (var e = n.monthsContainer.querySelectorAll(".flatpickr-monthSelect-month"), t = 0; t < e.length; t++)e[t].removeEventListener("click", i) } } } } });
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
            }
        }
    },
    textEdit: {
        _instances: [],

        initialize: (element, elementId, maskType, editMask) => {
            var instances = window.blazorise.textEdit._instances = window.blazorise.textEdit._instances || {};

            if (maskType === "numeric") {
                instances[elementId] = new window.blazorise.NumericMaskValidator(element, elementId);
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
                clickOpens: !(options.readOnly || false)
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
        }
    },

    NoValidator: function () {
        this.isValid = function (currentValue) {
            return true;
        };
    },
    NumericMaskValidator: function (dotnetAdapter, element, elementId, options) {
        this.dotnetAdapter = dotnetAdapter;
        this.elementId = elementId;
        this.element = element;
        this.decimals = options.decimals === null || options.decimals === undefined ? 2 : options.decimals;
        this.separator = options.separator || ".";
        this.step = options.step || 1;
        this.min = options.min;
        this.max = options.max;

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
            var value = this.element.value,
                selection = this.carret();

            if (value = value.substring(0, selection[0]) + currentValue + value.substring(selection[1]), !!this.regex().test(value)) {
                return value = (value || "").replace(this.separator, ".");
            }

            return false;
        };
        this.update = function (options) {
            if (options.decimals && options.decimals.changed) {
                this.decimals = options.decimals.value;

                this.truncate();
            }
        };
        this.truncate = function () {
            let value = (this.element.value || "").replace(this.separator, ".");
            let number = Number(value);

            number = Math.trunc(number * Math.pow(10, this.decimals)) / Math.pow(10, this.decimals);

            let newValue = number.toString().replace(".", this.separator);

            this.element.value = newValue;
            this.dotnetAdapter.invokeMethodAsync('SetValue', newValue);
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
    button: {
        _instances: [],

        initialize: (element, elementId, preventDefaultOnSubmit) => {
            window.blazorise.button._instances[elementId] = new window.blazorise.ButtonInfo(element, elementId, preventDefaultOnSubmit);

            if (element.type === "submit") {
                element.addEventListener("click", (e) => {
                    window.blazorise.button.click(window.blazorise.button._instances[elementId], e);
                });
            }
        },
        destroy: (elementId) => {
            var instances = window.blazorise.button._instances || {};
            delete instances[elementId];
        },
        click: (buttonInfo, e) => {
            if (buttonInfo.preventDefaultOnSubmit) {
                return e.preventDefault();
            }
        }
    },
    ButtonInfo: function (element, elementId, preventDefaultOnSubmit) {
        this.elementId = elementId;
        this.element = element;
        this.preventDefaultOnSubmit = preventDefaultOnSubmit;
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
            window.blazorise.tryClose(lastClosable, lastClosable.elementId, true, false);
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
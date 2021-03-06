﻿//! moment.js
//! version : 2.9.0
//! authors : Tim Wood, Iskren Chernev, Moment.js contributors
//! license : MIT
//! momentjs.com
//(function (n) { function it(n, t, i) { switch (arguments.length) { case 2: return n != null ? n : t; case 3: return n != null ? n : t != null ? t : i; default: throw new Error("Implement me"); } } function g(n, t) { return dr.call(n, t) } function vt() { return { empty: !1, unusedTokens: [], unusedInput: [], overflow: -2, charsLeftOver: 0, nullInput: !1, invalidMonth: null, invalidFormat: !1, userInvalidated: !1, iso: !1 } } function pi(n) { t.suppressDeprecationWarnings === !1 && typeof console != "undefined" && console.warn && console.warn("Deprecation warning: " + n) } function o(n, t) { var i = !0; return nt(function () { return i && (pi(n), i = !1), t.apply(this, arguments) }, t) } function bu(n, t) { vi[n] || (pi(t), vi[n] = !0) } function wi(n, t) { return function (i) { return r(n.call(this, i), t) } } function ku(n, t) { return function (i) { return this.localeData().ordinal(n.call(this, i), t) } } function du(n, t) { var r = (t.year() - n.year()) * 12 + (t.month() - n.month()), i = n.clone().add(r, "months"), u, f; return t - i < 0 ? (u = n.clone().add(r - 1, "months"), f = (t - i) / (i - u)) : (u = n.clone().add(r + 1, "months"), f = (t - i) / (u - i)), -(r + f) } function gu(n, t, i) { var r; return i == null ? t : n.meridiemHour != null ? n.meridiemHour(t, i) : n.isPM != null ? (r = n.isPM(i), r && t < 12 && (t += 12), r || t !== 12 || (t = 0), t) : t } function bi() { } function et(n, i) { i !== !1 && er(n); ki(this, n); this._d = new Date(+n._d); at === !1 && (at = !0, t.updateOffset(this), at = !1) } function yt(n) { var i = ir(n), r = i.year || 0, u = i.quarter || 0, f = i.month || 0, e = i.week || 0, o = i.day || 0, s = i.hour || 0, h = i.minute || 0, c = i.second || 0, l = i.millisecond || 0; this._milliseconds = +l + c * 1e3 + h * 6e4 + s * 36e5; this._days = +o + e * 7; this._months = +f + u * 3 + r * 12; this._data = {}; this._locale = t.localeData(); this._bubble() } function nt(n, t) { for (var i in t) g(t, i) && (n[i] = t[i]); return g(t, "toString") && (n.toString = t.toString), g(t, "valueOf") && (n.valueOf = t.valueOf), n } function ki(n, t) { var u, i, r; if (typeof t._isAMomentObject != "undefined" && (n._isAMomentObject = t._isAMomentObject), typeof t._i != "undefined" && (n._i = t._i), typeof t._f != "undefined" && (n._f = t._f), typeof t._l != "undefined" && (n._l = t._l), typeof t._strict != "undefined" && (n._strict = t._strict), typeof t._tzm != "undefined" && (n._tzm = t._tzm), typeof t._isUTC != "undefined" && (n._isUTC = t._isUTC), typeof t._offset != "undefined" && (n._offset = t._offset), typeof t._pf != "undefined" && (n._pf = t._pf), typeof t._locale != "undefined" && (n._locale = t._locale), ut.length > 0) for (u in ut) i = ut[u], r = t[i], typeof r != "undefined" && (n[i] = r); return n } function h(n) { return n < 0 ? Math.ceil(n) : Math.floor(n) } function r(n, t, i) { for (var r = "" + Math.abs(n), u = n >= 0; r.length < t;) r = "0" + r; return (u ? i ? "+" : "" : "-") + r } function di(n, t) { var i = { milliseconds: 0, months: 0 }; return i.months = t.month() - n.month() + (t.year() - n.year()) * 12, n.clone().add(i.months, "M").isAfter(t) && --i.months, i.milliseconds = +t - +n.clone().add(i.months, "M"), i } function nf(n, t) { var i; return t = bt(t, n), n.isBefore(t) ? i = di(n, t) : (i = di(t, n), i.milliseconds = -i.milliseconds, i.months = -i.months), i } function gi(n, i) { return function (r, u) { var f, e; return u === null || isNaN(+u) || (bu(i, "moment()." + i + "(period, number) is deprecated. Please use moment()." + i + "(number, period)."), e = r, r = u, u = e), r = typeof r == "string" ? +r : r, f = t.duration(r, u), nr(this, f, n), this } } function nr(n, i, r, u) { var o = i._milliseconds, f = i._days, e = i._months; u = u == null ? !0 : u; o && n._d.setTime(+n._d + o * r); f && pr(n, "Date", ii(n, "Date") + f * r); e && yr(n, ii(n, "Month") + e * r); u && t.updateOffset(n, f || e) } function ot(n) { return Object.prototype.toString.call(n) === "[object Array]" } function pt(n) { return Object.prototype.toString.call(n) === "[object Date]" || n instanceof Date } function tr(n, t, r) { for (var e = Math.min(n.length, t.length), o = Math.abs(n.length - t.length), f = 0, u = 0; u < e; u++) (r && n[u] !== t[u] || !r && i(n[u]) !== i(t[u])) && f++; return f + o } function e(n) { if (n) { var t = n.toLowerCase().replace(/(.)s$/, "$1"); n = pu[n] || wu[t] || t } return n } function ir(n) { var r = {}, t; for (var i in n) g(n, i) && (t = e(i), t && (r[t] = n[i])); return r } function tf(i) { var r, u; if (i.indexOf("week") === 0) r = 7, u = "day"; else if (i.indexOf("month") === 0) r = 12, u = "month"; else return; t[i] = function (f, e) { var o, s, c = t._locale[i], h = []; if (typeof f == "number" && (e = f, f = n), s = function (n) { var i = t().utc().set(u, n); return c.call(t._locale, i, f || "") }, e != null) return s(e); for (o = 0; o < r; o++) h.push(s(o)); return h } } function i(n) { var t = +n, i = 0; return t !== 0 && isFinite(t) && (i = t >= 0 ? Math.floor(t) : Math.ceil(t)), i } function wt(n, t) { return new Date(Date.UTC(n, t + 1, 0)).getUTCDate() } function rr(n, i, r) { return tt(t([n, 11, 31 + i - r]), i, r).week } function ur(n) { return fr(n) ? 366 : 365 } function fr(n) { return n % 4 == 0 && n % 100 != 0 || n % 400 == 0 } function er(n) { var t; n._a && n._pf.overflow === -2 && (t = n._a[l] < 0 || n._a[l] > 11 ? l : n._a[s] < 1 || n._a[s] > wt(n._a[c], n._a[l]) ? s : n._a[f] < 0 || n._a[f] > 24 || n._a[f] === 24 && (n._a[w] !== 0 || n._a[b] !== 0 || n._a[k] !== 0) ? f : n._a[w] < 0 || n._a[w] > 59 ? w : n._a[b] < 0 || n._a[b] > 59 ? b : n._a[k] < 0 || n._a[k] > 999 ? k : -1, n._pf._overflowDayOfYear && (t < c || t > s) && (t = s), n._pf.overflow = t) } function or(t) { return t._isValid == null && (t._isValid = !isNaN(t._d.getTime()) && t._pf.overflow < 0 && !t._pf.empty && !t._pf.invalidMonth && !t._pf.nullInput && !t._pf.invalidFormat && !t._pf.userInvalidated, t._strict && (t._isValid = t._isValid && t._pf.charsLeftOver === 0 && t._pf.unusedTokens.length === 0 && t._pf.bigHour === n)), t._isValid } function sr(n) { return n ? n.toLowerCase().replace("_", "-") : n } function rf(n) { for (var r = 0, i, t, f, u; r < n.length;) { for (u = sr(n[r]).split("-"), i = u.length, t = sr(n[r + 1]), t = t ? t.split("-") : null; i > 0;) { if (f = hr(u.slice(0, i).join("-")), f) return f; if (t && t.length >= i && tr(u, t, !0) >= i - 1) break; i-- } r++ } return null } function hr(n) { var i = null; if (!d[n] && ui) try { i = t.locale(); require("./locale/" + n); t.locale(i) } catch (r) { } return d[n] } function bt(n, i) { var r, u; return i._isUTC ? (r = i.clone(), u = (t.isMoment(n) || pt(n) ? +n : +t(n)) - +r, r._d.setTime(+r._d + u), t.updateOffset(r, !1), r) : t(n).local() } function uf(n) { return n.match(/\[[\s\S]/) ? n.replace(/^\[|\]$/g, "") : n.replace(/\\/g, "") } function ff(n) { for (var i = n.match(fi), t = 0, r = i.length; t < r; t++) i[t] = a[i[t]] ? a[i[t]] : uf(i[t]); return function (u) { var f = ""; for (t = 0; t < r; t++) f += i[t] instanceof Function ? i[t].call(u, n) : i[t]; return f } } function kt(n, t) { return n.isValid() ? (t = cr(t, n.localeData()), lt[t] || (lt[t] = ff(t)), lt[t](n)) : n.localeData().invalidDate() } function cr(n, t) { function r(n) { return t.longDateFormat(n) || n } var i = 5; for (ft.lastIndex = 0; i >= 0 && ft.test(n) ;) n = n.replace(ft, r), ft.lastIndex = 0, i -= 1; return n } function ef(n, t) { var i = t._strict; switch (n) { case "Q": return oi; case "DDDD": return hi; case "YYYY": case "GGGG": case "gggg": return i ? cu : ru; case "Y": case "G": case "g": return au; case "YYYYYY": case "YYYYY": case "GGGGG": case "ggggg": return i ? lu : uu; case "S": if (i) return oi; case "SS": if (i) return si; case "SSS": if (i) return hi; case "DDD": return iu; case "MMM": case "MMMM": case "dd": case "ddd": case "dddd": return eu; case "a": case "A": return t._locale._meridiemParse; case "x": return su; case "X": return hu; case "Z": case "ZZ": return st; case "T": return ou; case "SSSS": return fu; case "MM": case "DD": case "YY": case "GG": case "gg": case "HH": case "hh": case "mm": case "ss": case "ww": case "WW": return i ? si : ei; case "M": case "D": case "d": case "H": case "h": case "m": case "s": case "w": case "W": case "e": case "E": return ei; case "Do": return i ? t._locale._ordinalParse : t._locale._ordinalParseLenient; default: return new RegExp(af(lf(n.replace("\\", "")), "i")) } } function dt(n) { n = n || ""; var r = n.match(st) || [], f = r[r.length - 1] || [], t = (f + "").match(yu) || ["-", 0, 0], u = +(t[1] * 60) + i(t[2]); return t[0] === "+" ? u : -u } function of(n, r, u) { var o, e = u._a; switch (n) { case "Q": r != null && (e[l] = (i(r) - 1) * 3); break; case "M": case "MM": r != null && (e[l] = i(r) - 1); break; case "MMM": case "MMMM": o = u._locale.monthsParse(r, n, u._strict); o != null ? e[l] = o : u._pf.invalidMonth = r; break; case "D": case "DD": r != null && (e[s] = i(r)); break; case "Do": r != null && (e[s] = i(parseInt(r.match(/\d{1,2}/)[0], 10))); break; case "DDD": case "DDDD": r != null && (u._dayOfYear = i(r)); break; case "YY": e[c] = t.parseTwoDigitYear(r); break; case "YYYY": case "YYYYY": case "YYYYYY": e[c] = i(r); break; case "a": case "A": u._meridiem = r; break; case "h": case "hh": u._pf.bigHour = !0; case "H": case "HH": e[f] = i(r); break; case "m": case "mm": e[w] = i(r); break; case "s": case "ss": e[b] = i(r); break; case "S": case "SS": case "SSS": case "SSSS": e[k] = i(("0." + r) * 1e3); break; case "x": u._d = new Date(i(r)); break; case "X": u._d = new Date(parseFloat(r) * 1e3); break; case "Z": case "ZZ": u._useUTC = !0; u._tzm = dt(r); break; case "dd": case "ddd": case "dddd": o = u._locale.weekdaysParse(r); o != null ? (u._w = u._w || {}, u._w.d = o) : u._pf.invalidWeekday = r; break; case "w": case "ww": case "W": case "WW": case "d": case "e": case "E": n = n.substr(0, 1); case "gggg": case "GGGG": case "GGGGG": n = n.substr(0, 2); r && (u._w = u._w || {}, u._w[n] = i(r)); break; case "gg": case "GG": u._w = u._w || {}; u._w[n] = t.parseTwoDigitYear(r) } } function sf(n) { var i, o, f, u, r, e, s; i = n._w; i.GG != null || i.W != null || i.E != null ? (r = 1, e = 4, o = it(i.GG, n._a[c], tt(t(), 1, 4).year), f = it(i.W, 1), u = it(i.E, 1)) : (r = n._locale._week.dow, e = n._locale._week.doy, o = it(i.gg, n._a[c], tt(t(), r, e).year), f = it(i.w, 1), i.d != null ? (u = i.d, u < r && ++f) : u = i.e != null ? i.e + r : r); s = ne(o, f, u, e, r); n._a[c] = s.year; n._dayOfYear = s.dayOfYear } function gt(n) { var t, i, r = [], u, e; if (!n._d) { for (u = cf(n), n._w && n._a[s] == null && n._a[l] == null && sf(n), n._dayOfYear && (e = it(n._a[c], u[c]), n._dayOfYear > ur(e) && (n._pf._overflowDayOfYear = !0), i = ti(e, 0, n._dayOfYear), n._a[l] = i.getUTCMonth(), n._a[s] = i.getUTCDate()), t = 0; t < 3 && n._a[t] == null; ++t) n._a[t] = r[t] = u[t]; for (; t < 7; t++) n._a[t] = r[t] = n._a[t] == null ? t === 2 ? 1 : 0 : n._a[t]; n._a[f] === 24 && n._a[w] === 0 && n._a[b] === 0 && n._a[k] === 0 && (n._nextDay = !0, n._a[f] = 0); n._d = (n._useUTC ? ti : bf).apply(null, r); n._tzm != null && n._d.setUTCMinutes(n._d.getUTCMinutes() - n._tzm); n._nextDay && (n._a[f] = 24) } } function hf(n) { var t; n._d || (t = ir(n._i), n._a = [t.year, t.month, t.day || t.date, t.hour, t.minute, t.second, t.millisecond], gt(n)) } function cf(n) { var t = new Date; return n._useUTC ? [t.getUTCFullYear(), t.getUTCMonth(), t.getUTCDate()] : [t.getFullYear(), t.getMonth(), t.getDate()] } function ni(i) { if (i._f === t.ISO_8601) { lr(i); return } i._a = []; i._pf.empty = !0; for (var r = "" + i._i, u, e, h, l = r.length, c = 0, s = cr(i._f, i._locale).match(fi) || [], o = 0; o < s.length; o++) e = s[o], u = (r.match(ef(e, i)) || [])[0], u && (h = r.substr(0, r.indexOf(u)), h.length > 0 && i._pf.unusedInput.push(h), r = r.slice(r.indexOf(u) + u.length), c += u.length), a[e] ? (u ? i._pf.empty = !1 : i._pf.unusedTokens.push(e), of(e, u, i)) : i._strict && !u && i._pf.unusedTokens.push(e); i._pf.charsLeftOver = l - c; r.length > 0 && i._pf.unusedInput.push(r); i._pf.bigHour === !0 && i._a[f] <= 12 && (i._pf.bigHour = n); i._a[f] = gu(i._locale, i._a[f], i._meridiem); gt(i); er(i) } function lf(n) { return n.replace(/\\(\[)|\\(\])|\[([^\]\[]*)\]|\\(.)/g, function (n, t, i, r, u) { return t || i || r || u }) } function af(n) { return n.replace(/[-\/\\^$*+?.()|[\]{}]/g, "\\$&") } function vf(n) { var t, f, u, r, i; if (n._f.length === 0) { n._pf.invalidFormat = !0; n._d = new Date(NaN); return } for (r = 0; r < n._f.length; r++) (i = 0, t = ki({}, n), n._useUTC != null && (t._useUTC = n._useUTC), t._pf = vt(), t._f = n._f[r], ni(t), or(t)) && (i += t._pf.charsLeftOver, i += t._pf.unusedTokens.length * 10, t._pf.score = i, (u == null || i < u) && (u = i, f = t)); nt(n, f || t) } function lr(n) { var t, i, r = n._i, u = vu.exec(r); if (u) { for (n._pf.iso = !0, t = 0, i = ht.length; t < i; t++) if (ht[t][1].exec(r)) { n._f = ht[t][0] + (u[6] || " "); break } for (t = 0, i = ct.length; t < i; t++) if (ct[t][1].exec(r)) { n._f += ct[t][0]; break } r.match(st) && (n._f += "Z"); ni(n) } else n._isValid = !1 } function yf(n) { lr(n); n._isValid === !1 && (delete n._isValid, t.createFromInputFallback(n)) } function pf(n, t) { for (var r = [], i = 0; i < n.length; ++i) r.push(t(n[i], i)); return r } function wf(i) { var r = i._i, u; r === n ? i._d = new Date : pt(r) ? i._d = new Date(+r) : (u = gr.exec(r)) !== null ? i._d = new Date(+u[1]) : typeof r == "string" ? yf(i) : ot(r) ? (i._a = pf(r.slice(0), function (n) { return parseInt(n, 10) }), gt(i)) : typeof r == "object" ? hf(i) : typeof r == "number" ? i._d = new Date(r) : t.createFromInputFallback(i) } function bf(n, t, i, r, u, f, e) { var o = new Date(n, t, i, r, u, f, e); return n < 1970 && o.setFullYear(n), o } function ti(n) { var t = new Date(Date.UTC.apply(null, arguments)); return n < 1970 && t.setUTCFullYear(n), t } function kf(n, t) { if (typeof n == "string") if (isNaN(n)) { if (n = t.weekdaysParse(n), typeof n != "number") return null } else n = parseInt(n, 10); return n } function df(n, t, i, r, u) { return u.relativeTime(t || 1, !!i, n, r) } function gf(n, i, r) { var u = t.duration(n).abs(), c = p(u.as("s")), e = p(u.as("m")), o = p(u.as("h")), s = p(u.as("d")), h = p(u.as("M")), l = p(u.as("y")), f = c < y.s && ["s", c] || e === 1 && ["m"] || e < y.m && ["mm", e] || o === 1 && ["h"] || o < y.h && ["hh", o] || s === 1 && ["d"] || s < y.d && ["dd", s] || h === 1 && ["M"] || h < y.M && ["MM", h] || l === 1 && ["y"] || ["yy", l]; return f[2] = i, f[3] = +n > 0, f[4] = r, df.apply({}, f) } function tt(n, i, r) { var e = r - i, u = r - n.day(), f; return u > e && (u -= 7), u < e - 7 && (u += 7), f = t(n).add(u, "d"), { week: Math.ceil(f.dayOfYear() / 7), year: f.year() } } function ne(n, t, i, r, u) { var f = ti(n, 0, 1).getUTCDay(), o, e; return f = f === 0 ? 7 : f, i = i != null ? i : u, o = u - f + (f > r ? 7 : 0) - (f < u ? 7 : 0), e = 7 * (t - 1) + (i - u) + o + 1, { year: e > 0 ? n : n - 1, dayOfYear: e > 0 ? e : ur(n - 1) + e } } function ar(i) { var r = i._i, f = i._f, u; return (i._locale = i._locale || t.localeData(i._l), r === null || f === n && r === "") ? t.invalid({ nullInput: !0 }) : (typeof r == "string" && (i._i = r = i._locale.preparse(r)), t.isMoment(r)) ? new et(r, !0) : (f ? ot(f) ? vf(i) : ni(i) : wf(i), u = new et(i), u._nextDay && (u.add(1, "d"), u._nextDay = n), u) } function vr(n, i) { var u, r; if (i.length === 1 && ot(i[0]) && (i = i[0]), !i.length) return t(); for (u = i[0], r = 1; r < i.length; ++r) i[r][n](u) && (u = i[r]); return u } function yr(n, t) { var i; return typeof t == "string" && (t = n.localeData().monthsParse(t), typeof t != "number") ? n : (i = Math.min(n.date(), wt(n.year(), t)), n._d["set" + (n._isUTC ? "UTC" : "") + "Month"](t, i), n) } function ii(n, t) { return n._d["get" + (n._isUTC ? "UTC" : "") + t]() } function pr(n, t, i) { return t === "Month" ? yr(n, i) : n._d["set" + (n._isUTC ? "UTC" : "") + t](i) } function v(n, i) { return function (r) { return r != null ? (pr(this, n, r), t.updateOffset(this, i), this) : ii(this, n) } } function wr(n) { return n * 400 / 146097 } function br(n) { return n * 146097 / 400 } function te(n) { t.duration.fn[n] = function () { return this._data[n] } } function kr(n) { typeof ender == "undefined" && (ri = rt.moment, rt.moment = n ? o("Accessing Moment through the global scope is deprecated, and will be removed in an upcoming release.", t) : t) } for (var t, rt = typeof global != "undefined" && (typeof window == "undefined" || window === global.window) ? global : this, ri, p = Math.round, dr = Object.prototype.hasOwnProperty, u, c = 0, l = 1, s = 2, f = 3, w = 4, b = 5, k = 6, d = {}, ut = [], ui = typeof module != "undefined" && module && module.exports, gr = /^\/?Date\((\-?\d+)/i, nu = /(\-)?(?:(\d*)\.)?(\d+)\:(\d+)(?:\:(\d+)\.?(\d{3})?)?/, tu = /^(-)?P(?:(?:([0-9,.]*)Y)?(?:([0-9,.]*)M)?(?:([0-9,.]*)D)?(?:T(?:([0-9,.]*)H)?(?:([0-9,.]*)M)?(?:([0-9,.]*)S)?)?|([0-9,.]*)W)$/, fi = /(\[[^\[]*\])|(\\)?(Mo|MM?M?M?|Do|DDDo|DD?D?D?|ddd?d?|do?|w[o|w]?|W[o|W]?|Q|YYYYYY|YYYYY|YYYY|YY|gg(ggg?)?|GG(GGG?)?|e|E|a|A|hh?|HH?|mm?|ss?|S{1,4}|x|X|zz?|ZZ?|.)/g, ft = /(\[[^\[]*\])|(\\)?(LTS|LT|LL?L?L?|l{1,4})/g, ei = /\d\d?/, iu = /\d{1,3}/, ru = /\d{1,4}/, uu = /[+\-]?\d{1,6}/, fu = /\d+/, eu = /[0-9]*['a-z\u00A0-\u05FF\u0700-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+|[\u0600-\u06FF\/]+(\s*?[\u0600-\u06FF]+){1,2}/i, st = /Z|[\+\-]\d\d:?\d\d/gi, ou = /T/i, su = /[\+\-]?\d+/, hu = /[\+\-]?\d+(\.\d{1,3})?/, oi = /\d/, si = /\d\d/, hi = /\d{3}/, cu = /\d{4}/, lu = /[+-]?\d{6}/, au = /[+-]?\d+/, vu = /^\s*(?:[+-]\d{6}|\d{4})-(?:(\d\d-\d\d)|(W\d\d$)|(W\d\d-\d)|(\d\d\d))((T| )(\d\d(:\d\d(:\d\d(\.\d+)?)?)?)?([\+\-]\d\d(?::?\d\d)?|\s*Z)?)?$/, ht = [["YYYYYY-MM-DD", /[+-]\d{6}-\d{2}-\d{2}/], ["YYYY-MM-DD", /\d{4}-\d{2}-\d{2}/], ["GGGG-[W]WW-E", /\d{4}-W\d{2}-\d/], ["GGGG-[W]WW", /\d{4}-W\d{2}/], ["YYYY-DDD", /\d{4}-\d{3}/]], ct = [["HH:mm:ss.SSSS", /(T| )\d\d:\d\d:\d\d\.\d+/], ["HH:mm:ss", /(T| )\d\d:\d\d:\d\d/], ["HH:mm", /(T| )\d\d:\d\d/], ["HH", /(T| )\d\d/]], yu = /([\+\-]|\d\d)/gi, ie = "Date|Hours|Minutes|Seconds|Milliseconds".split("|"), ci = { Milliseconds: 1, Seconds: 1e3, Minutes: 6e4, Hours: 36e5, Days: 864e5, Months: 2592e6, Years: 31536e6 }, pu = { ms: "millisecond", s: "second", m: "minute", h: "hour", d: "day", D: "date", w: "week", W: "isoWeek", M: "month", Q: "quarter", y: "year", DDD: "dayOfYear", e: "weekday", E: "isoWeekday", gg: "weekYear", GG: "isoWeekYear" }, wu = { dayofyear: "dayOfYear", isoweekday: "isoWeekday", isoweek: "isoWeek", weekyear: "weekYear", isoweekyear: "isoWeekYear" }, lt = {}, y = { s: 45, m: 45, h: 22, d: 26, M: 11 }, li = "DDD w W M D d".split(" "), ai = "M D H h m s w W".split(" "), a = { M: function () { return this.month() + 1 }, MMM: function (n) { return this.localeData().monthsShort(this, n) }, MMMM: function (n) { return this.localeData().months(this, n) }, D: function () { return this.date() }, DDD: function () { return this.dayOfYear() }, d: function () { return this.day() }, dd: function (n) { return this.localeData().weekdaysMin(this, n) }, ddd: function (n) { return this.localeData().weekdaysShort(this, n) }, dddd: function (n) { return this.localeData().weekdays(this, n) }, w: function () { return this.week() }, W: function () { return this.isoWeek() }, YY: function () { return r(this.year() % 100, 2) }, YYYY: function () { return r(this.year(), 4) }, YYYYY: function () { return r(this.year(), 5) }, YYYYYY: function () { var n = this.year(), t = n >= 0 ? "+" : "-"; return t + r(Math.abs(n), 6) }, gg: function () { return r(this.weekYear() % 100, 2) }, gggg: function () { return r(this.weekYear(), 4) }, ggggg: function () { return r(this.weekYear(), 5) }, GG: function () { return r(this.isoWeekYear() % 100, 2) }, GGGG: function () { return r(this.isoWeekYear(), 4) }, GGGGG: function () { return r(this.isoWeekYear(), 5) }, e: function () { return this.weekday() }, E: function () { return this.isoWeekday() }, a: function () { return this.localeData().meridiem(this.hours(), this.minutes(), !0) }, A: function () { return this.localeData().meridiem(this.hours(), this.minutes(), !1) }, H: function () { return this.hours() }, h: function () { return this.hours() % 12 || 12 }, m: function () { return this.minutes() }, s: function () { return this.seconds() }, S: function () { return i(this.milliseconds() / 100) }, SS: function () { return r(i(this.milliseconds() / 10), 2) }, SSS: function () { return r(this.milliseconds(), 3) }, SSSS: function () { return r(this.milliseconds(), 3) }, Z: function () { var n = this.utcOffset(), t = "+"; return n < 0 && (n = -n, t = "-"), t + r(i(n / 60), 2) + ":" + r(i(n) % 60, 2) }, ZZ: function () { var n = this.utcOffset(), t = "+"; return n < 0 && (n = -n, t = "-"), t + r(i(n / 60), 2) + r(i(n) % 60, 2) }, z: function () { return this.zoneAbbr() }, zz: function () { return this.zoneName() }, x: function () { return this.valueOf() }, X: function () { return this.unix() }, Q: function () { return this.quarter() } }, vi = {}, yi = ["months", "monthsShort", "weekdays", "weekdaysShort", "weekdaysMin"], at = !1; li.length;) u = li.pop(), a[u + "o"] = ku(a[u], u); while (ai.length) u = ai.pop(), a[u + u] = wi(a[u], 2); for (a.DDDD = wi(a.DDD, 3), nt(bi.prototype, { set: function (n) { var t; for (var i in n) t = n[i], typeof t == "function" ? this[i] = t : this["_" + i] = t; this._ordinalParseLenient = new RegExp(this._ordinalParse.source + "|" + /\d{1,2}/.source) }, _months: "January_February_March_April_May_June_July_August_September_October_November_December".split("_"), months: function (n) { return this._months[n.month()] }, _monthsShort: "Jan_Feb_Mar_Apr_May_Jun_Jul_Aug_Sep_Oct_Nov_Dec".split("_"), monthsShort: function (n) { return this._monthsShort[n.month()] }, monthsParse: function (n, i, r) { var u, f, e; for (this._monthsParse || (this._monthsParse = [], this._longMonthsParse = [], this._shortMonthsParse = []), u = 0; u < 12; u++) if ((f = t.utc([2e3, u]), r && !this._longMonthsParse[u] && (this._longMonthsParse[u] = new RegExp("^" + this.months(f, "").replace(".", "") + "$", "i"), this._shortMonthsParse[u] = new RegExp("^" + this.monthsShort(f, "").replace(".", "") + "$", "i")), r || this._monthsParse[u] || (e = "^" + this.months(f, "") + "|^" + this.monthsShort(f, ""), this._monthsParse[u] = new RegExp(e.replace(".", ""), "i")), r && i === "MMMM" && this._longMonthsParse[u].test(n)) || r && i === "MMM" && this._shortMonthsParse[u].test(n) || !r && this._monthsParse[u].test(n)) return u }, _weekdays: "Sunday_Monday_Tuesday_Wednesday_Thursday_Friday_Saturday".split("_"), weekdays: function (n) { return this._weekdays[n.day()] }, _weekdaysShort: "Sun_Mon_Tue_Wed_Thu_Fri_Sat".split("_"), weekdaysShort: function (n) { return this._weekdaysShort[n.day()] }, _weekdaysMin: "Su_Mo_Tu_We_Th_Fr_Sa".split("_"), weekdaysMin: function (n) { return this._weekdaysMin[n.day()] }, weekdaysParse: function (n) { var i, r, u; for (this._weekdaysParse || (this._weekdaysParse = []), i = 0; i < 7; i++) if (this._weekdaysParse[i] || (r = t([2e3, 1]).day(i), u = "^" + this.weekdays(r, "") + "|^" + this.weekdaysShort(r, "") + "|^" + this.weekdaysMin(r, ""), this._weekdaysParse[i] = new RegExp(u.replace(".", ""), "i")), this._weekdaysParse[i].test(n)) return i }, _longDateFormat: { LTS: "h:mm:ss A", LT: "h:mm A", L: "MM/DD/YYYY", LL: "MMMM D, YYYY", LLL: "MMMM D, YYYY LT", LLLL: "dddd, MMMM D, YYYY LT" }, longDateFormat: function (n) { var t = this._longDateFormat[n]; return !t && this._longDateFormat[n.toUpperCase()] && (t = this._longDateFormat[n.toUpperCase()].replace(/MMMM|MM|DD|dddd/g, function (n) { return n.slice(1) }), this._longDateFormat[n] = t), t }, isPM: function (n) { return (n + "").toLowerCase().charAt(0) === "p" }, _meridiemParse: /[ap]\.?m?\.?/i, meridiem: function (n, t, i) { return n > 11 ? i ? "pm" : "PM" : i ? "am" : "AM" }, _calendar: { sameDay: "[Today at] LT", nextDay: "[Tomorrow at] LT", nextWeek: "dddd [at] LT", lastDay: "[Yesterday at] LT", lastWeek: "[Last] dddd [at] LT", sameElse: "L" }, calendar: function (n, t, i) { var r = this._calendar[n]; return typeof r == "function" ? r.apply(t, [i]) : r }, _relativeTime: { future: "in %s", past: "%s ago", s: "a few seconds", m: "a minute", mm: "%d minutes", h: "an hour", hh: "%d hours", d: "a day", dd: "%d days", M: "a month", MM: "%d months", y: "a year", yy: "%d years" }, relativeTime: function (n, t, i, r) { var u = this._relativeTime[i]; return typeof u == "function" ? u(n, t, i, r) : u.replace(/%d/i, n) }, pastFuture: function (n, t) { var i = this._relativeTime[n > 0 ? "future" : "past"]; return typeof i == "function" ? i(t) : i.replace(/%s/i, t) }, ordinal: function (n) { return this._ordinal.replace("%d", n) }, _ordinal: "%d", _ordinalParse: /\d{1,2}/, preparse: function (n) { return n }, postformat: function (n) { return n }, week: function (n) { return tt(n, this._week.dow, this._week.doy).week }, _week: { dow: 0, doy: 6 }, firstDayOfWeek: function () { return this._week.dow }, firstDayOfYear: function () { return this._week.doy }, _invalidDate: "Invalid date", invalidDate: function () { return this._invalidDate } }), t = function (t, i, r, u) { var f; return typeof r == "boolean" && (u = r, r = n), f = {}, f._isAMomentObject = !0, f._i = t, f._f = i, f._l = r, f._strict = u, f._isUTC = !1, f._pf = vt(), ar(f) }, t.suppressDeprecationWarnings = !1, t.createFromInputFallback = o("moment construction falls back to js Date. This is discouraged and will be removed in upcoming major release. Please refer to https://github.com/moment/moment/issues/1407 for more info.", function (n) { n._d = new Date(n._i + (n._useUTC ? " UTC" : "")) }), t.min = function () { var n = [].slice.call(arguments, 0); return vr("isBefore", n) }, t.max = function () { var n = [].slice.call(arguments, 0); return vr("isAfter", n) }, t.utc = function (t, i, r, u) { var f; return typeof r == "boolean" && (u = r, r = n), f = {}, f._isAMomentObject = !0, f._useUTC = !0, f._isUTC = !0, f._l = r, f._i = t, f._f = i, f._strict = u, f._pf = vt(), ar(f).utc() }, t.unix = function (n) { return t(n * 1e3) }, t.duration = function (n, r) { var u = n, e = null, o, c, h, l; return t.isDuration(n) ? u = { ms: n._milliseconds, d: n._days, M: n._months } : typeof n == "number" ? (u = {}, r ? u[r] = n : u.milliseconds = n) : (e = nu.exec(n)) ? (o = e[1] === "-" ? -1 : 1, u = { y: 0, d: i(e[s]) * o, h: i(e[f]) * o, m: i(e[w]) * o, s: i(e[b]) * o, ms: i(e[k]) * o }) : (e = tu.exec(n)) ? (o = e[1] === "-" ? -1 : 1, h = function (n) { var t = n && parseFloat(n.replace(",", ".")); return (isNaN(t) ? 0 : t) * o }, u = { y: h(e[2]), M: h(e[3]), d: h(e[4]), h: h(e[5]), m: h(e[6]), s: h(e[7]), w: h(e[8]) }) : u == null ? u = {} : typeof u == "object" && ("from" in u || "to" in u) && (l = nf(t(u.from), t(u.to)), u = {}, u.ms = l.milliseconds, u.M = l.months), c = new yt(u), t.isDuration(n) && g(n, "_locale") && (c._locale = n._locale), c }, t.version = "2.9.0", t.defaultFormat = "YYYY-MM-DDTHH:mm:ssZ", t.ISO_8601 = function () { }, t.momentProperties = ut, t.updateOffset = function () { }, t.relativeTimeThreshold = function (t, i) { return y[t] === n ? !1 : i === n ? y[t] : (y[t] = i, !0) }, t.lang = o("moment.lang is deprecated. Use moment.locale instead.", function (n, i) { return t.locale(n, i) }), t.locale = function (n, i) { var r; return n && (r = typeof i != "undefined" ? t.defineLocale(n, i) : t.localeData(n), r && (t.duration._locale = t._locale = r)), t._locale._abbr }, t.defineLocale = function (n, i) { return i !== null ? (i.abbr = n, d[n] || (d[n] = new bi), d[n].set(i), t.locale(n), d[n]) : (delete d[n], null) }, t.langData = o("moment.langData is deprecated. Use moment.localeData instead.", function (n) { return t.localeData(n) }), t.localeData = function (n) { var i; if (n && n._locale && n._locale._abbr && (n = n._locale._abbr), !n) return t._locale; if (!ot(n)) { if (i = hr(n), i) return i; n = [n] } return rf(n) }, t.isMoment = function (n) { return n instanceof et || n != null && g(n, "_isAMomentObject") }, t.isDuration = function (n) { return n instanceof yt }, u = yi.length - 1; u >= 0; --u) tf(yi[u]); t.normalizeUnits = function (n) { return e(n) }; t.invalid = function (n) { var i = t.utc(NaN); return n != null ? nt(i._pf, n) : i._pf.userInvalidated = !0, i }; t.parseZone = function () { return t.apply(null, arguments).parseZone() }; t.parseTwoDigitYear = function (n) { return i(n) + (i(n) > 68 ? 1900 : 2e3) }; t.isDate = pt; nt(t.fn = et.prototype, { clone: function () { return t(this) }, valueOf: function () { return +this._d - (this._offset || 0) * 6e4 }, unix: function () { return Math.floor(+this / 1e3) }, toString: function () { return this.clone().locale("en").format("ddd MMM DD YYYY HH:mm:ss [GMT]ZZ") }, toDate: function () { return this._offset ? new Date(+this) : this._d }, toISOString: function () { var n = t(this).utc(); return 0 < n.year() && n.year() <= 9999 ? "function" == typeof Date.prototype.toISOString ? this.toDate().toISOString() : kt(n, "YYYY-MM-DD[T]HH:mm:ss.SSS[Z]") : kt(n, "YYYYYY-MM-DD[T]HH:mm:ss.SSS[Z]") }, toArray: function () { var n = this; return [n.year(), n.month(), n.date(), n.hours(), n.minutes(), n.seconds(), n.milliseconds()] }, isValid: function () { return or(this) }, isDSTShifted: function () { return this._a ? this.isValid() && tr(this._a, (this._isUTC ? t.utc(this._a) : t(this._a)).toArray()) > 0 : !1 }, parsingFlags: function () { return nt({}, this._pf) }, invalidAt: function () { return this._pf.overflow }, utc: function (n) { return this.utcOffset(0, n) }, local: function (n) { return this._isUTC && (this.utcOffset(0, n), this._isUTC = !1, n && this.subtract(this._dateUtcOffset(), "m")), this }, format: function (n) { var i = kt(this, n || t.defaultFormat); return this.localeData().postformat(i) }, add: gi(1, "add"), subtract: gi(-1, "subtract"), diff: function (n, t, i) { var f = bt(n, this), o = (f.utcOffset() - this.utcOffset()) * 6e4, u, r; return t = e(t), t === "year" || t === "month" || t === "quarter" ? (r = du(this, f), t === "quarter" ? r = r / 3 : t === "year" && (r = r / 12)) : (u = this - f, r = t === "second" ? u / 1e3 : t === "minute" ? u / 6e4 : t === "hour" ? u / 36e5 : t === "day" ? (u - o) / 864e5 : t === "week" ? (u - o) / 6048e5 : u), i ? r : h(r) }, from: function (n, i) { return t.duration({ to: this, from: n }).locale(this.locale()).humanize(!i) }, fromNow: function (n) { return this.from(t(), n) }, calendar: function (n) { var r = n || t(), u = bt(r, this).startOf("day"), i = this.diff(u, "days", !0), f = i < -6 ? "sameElse" : i < -1 ? "lastWeek" : i < 0 ? "lastDay" : i < 1 ? "sameDay" : i < 2 ? "nextDay" : i < 7 ? "nextWeek" : "sameElse"; return this.format(this.localeData().calendar(f, this, t(r))) }, isLeapYear: function () { return fr(this.year()) }, isDST: function () { return this.utcOffset() > this.clone().month(0).utcOffset() || this.utcOffset() > this.clone().month(5).utcOffset() }, day: function (n) { var t = this._isUTC ? this._d.getUTCDay() : this._d.getDay(); return n != null ? (n = kf(n, this.localeData()), this.add(n - t, "d")) : t }, month: v("Month", !0), startOf: function (n) { n = e(n); switch (n) { case "year": this.month(0); case "quarter": case "month": this.date(1); case "week": case "isoWeek": case "day": this.hours(0); case "hour": this.minutes(0); case "minute": this.seconds(0); case "second": this.milliseconds(0) } return n === "week" ? this.weekday(0) : n === "isoWeek" && this.isoWeekday(1), n === "quarter" && this.month(Math.floor(this.month() / 3) * 3), this }, endOf: function (t) { return (t = e(t), t === n || t === "millisecond") ? this : this.startOf(t).add(1, t === "isoWeek" ? "week" : t).subtract(1, "ms") }, isAfter: function (n, i) { var r; return i = e(typeof i != "undefined" ? i : "millisecond"), i === "millisecond" ? (n = t.isMoment(n) ? n : t(n), +this > +n) : (r = t.isMoment(n) ? +n : +t(n), r < +this.clone().startOf(i)) }, isBefore: function (n, i) { var r; return i = e(typeof i != "undefined" ? i : "millisecond"), i === "millisecond" ? (n = t.isMoment(n) ? n : t(n), +this < +n) : (r = t.isMoment(n) ? +n : +t(n), +this.clone().endOf(i) < r) }, isBetween: function (n, t, i) { return this.isAfter(n, i) && this.isBefore(t, i) }, isSame: function (n, i) { var r; return i = e(i || "millisecond"), i === "millisecond" ? (n = t.isMoment(n) ? n : t(n), +this == +n) : (r = +t(n), +this.clone().startOf(i) <= r && r <= +this.clone().endOf(i)) }, min: o("moment().min is deprecated, use moment.min instead. https://github.com/moment/moment/issues/1548", function (n) { return n = t.apply(null, arguments), n < this ? this : n }), max: o("moment().max is deprecated, use moment.max instead. https://github.com/moment/moment/issues/1548", function (n) { return n = t.apply(null, arguments), n > this ? this : n }), zone: o("moment().zone is deprecated, use moment().utcOffset instead. https://github.com/moment/moment/issues/1779", function (n, t) { return n != null ? (typeof n != "string" && (n = -n), this.utcOffset(n, t), this) : -this.utcOffset() }), utcOffset: function (n, i) { var r = this._offset || 0, u; return n != null ? (typeof n == "string" && (n = dt(n)), Math.abs(n) < 16 && (n = n * 60), !this._isUTC && i && (u = this._dateUtcOffset()), this._offset = n, this._isUTC = !0, u != null && this.add(u, "m"), r !== n && (!i || this._changeInProgress ? nr(this, t.duration(n - r, "m"), 1, !1) : this._changeInProgress || (this._changeInProgress = !0, t.updateOffset(this, !0), this._changeInProgress = null)), this) : this._isUTC ? r : this._dateUtcOffset() }, isLocal: function () { return !this._isUTC }, isUtcOffset: function () { return this._isUTC }, isUtc: function () { return this._isUTC && this._offset === 0 }, zoneAbbr: function () { return this._isUTC ? "UTC" : "" }, zoneName: function () { return this._isUTC ? "Coordinated Universal Time" : "" }, parseZone: function () { return this._tzm ? this.utcOffset(this._tzm) : typeof this._i == "string" && this.utcOffset(dt(this._i)), this }, hasAlignedHourOffset: function (n) { return n = n ? t(n).utcOffset() : 0, (this.utcOffset() - n) % 60 == 0 }, daysInMonth: function () { return wt(this.year(), this.month()) }, dayOfYear: function (n) { var i = p((t(this).startOf("day") - t(this).startOf("year")) / 864e5) + 1; return n == null ? i : this.add(n - i, "d") }, quarter: function (n) { return n == null ? Math.ceil((this.month() + 1) / 3) : this.month((n - 1) * 3 + this.month() % 3) }, weekYear: function (n) { var t = tt(this, this.localeData()._week.dow, this.localeData()._week.doy).year; return n == null ? t : this.add(n - t, "y") }, isoWeekYear: function (n) { var t = tt(this, 1, 4).year; return n == null ? t : this.add(n - t, "y") }, week: function (n) { var t = this.localeData().week(this); return n == null ? t : this.add((n - t) * 7, "d") }, isoWeek: function (n) { var t = tt(this, 1, 4).week; return n == null ? t : this.add((n - t) * 7, "d") }, weekday: function (n) { var t = (this.day() + 7 - this.localeData()._week.dow) % 7; return n == null ? t : this.add(n - t, "d") }, isoWeekday: function (n) { return n == null ? this.day() || 7 : this.day(this.day() % 7 ? n : n - 7) }, isoWeeksInYear: function () { return rr(this.year(), 1, 4) }, weeksInYear: function () { var n = this.localeData()._week; return rr(this.year(), n.dow, n.doy) }, get: function (n) { return n = e(n), this[n]() }, set: function (n, t) { var i; if (typeof n == "object") for (i in n) this.set(i, n[i]); else n = e(n), typeof this[n] == "function" && this[n](t); return this }, locale: function (i) { var r; return i === n ? this._locale._abbr : (r = t.localeData(i), r != null && (this._locale = r), this) }, lang: o("moment().lang() is deprecated. Instead, use moment().localeData() to get the language configuration. Use moment().locale() to change languages.", function (t) { return t === n ? this.localeData() : this.locale(t) }), localeData: function () { return this._locale }, _dateUtcOffset: function () { return -Math.round(this._d.getTimezoneOffset() / 15) * 15 } }); t.fn.millisecond = t.fn.milliseconds = v("Milliseconds", !1); t.fn.second = t.fn.seconds = v("Seconds", !1); t.fn.minute = t.fn.minutes = v("Minutes", !1); t.fn.hour = t.fn.hours = v("Hours", !0); t.fn.date = v("Date", !0); t.fn.dates = o("dates accessor is deprecated. Use date instead.", v("Date", !0)); t.fn.year = v("FullYear", !0); t.fn.years = o("years accessor is deprecated. Use year instead.", v("FullYear", !0)); t.fn.days = t.fn.day; t.fn.months = t.fn.month; t.fn.weeks = t.fn.week; t.fn.isoWeeks = t.fn.isoWeek; t.fn.quarters = t.fn.quarter; t.fn.toJSON = t.fn.toISOString; t.fn.isUTC = t.fn.isUtc; nt(t.duration.fn = yt.prototype, { _bubble: function () { var o = this._milliseconds, t = this._days, i = this._months, n = this._data, u, f, e, r = 0; n.milliseconds = o % 1e3; u = h(o / 1e3); n.seconds = u % 60; f = h(u / 60); n.minutes = f % 60; e = h(f / 60); n.hours = e % 24; t += h(e / 24); r = h(wr(t)); t -= h(br(r)); i += h(t / 30); t %= 30; r += h(i / 12); i %= 12; n.days = t; n.months = i; n.years = r }, abs: function () { return this._milliseconds = Math.abs(this._milliseconds), this._days = Math.abs(this._days), this._months = Math.abs(this._months), this._data.milliseconds = Math.abs(this._data.milliseconds), this._data.seconds = Math.abs(this._data.seconds), this._data.minutes = Math.abs(this._data.minutes), this._data.hours = Math.abs(this._data.hours), this._data.months = Math.abs(this._data.months), this._data.years = Math.abs(this._data.years), this }, weeks: function () { return h(this.days() / 7) }, valueOf: function () { return this._milliseconds + this._days * 864e5 + this._months % 12 * 2592e6 + i(this._months / 12) * 31536e6 }, humanize: function (n) { var t = gf(this, !n, this.localeData()); return n && (t = this.localeData().pastFuture(+this, t)), this.localeData().postformat(t) }, add: function (n, i) { var r = t.duration(n, i); return this._milliseconds += r._milliseconds, this._days += r._days, this._months += r._months, this._bubble(), this }, subtract: function (n, i) { var r = t.duration(n, i); return this._milliseconds -= r._milliseconds, this._days -= r._days, this._months -= r._months, this._bubble(), this }, get: function (n) { return n = e(n), this[n.toLowerCase() + "s"]() }, as: function (n) { var t, i; if (n = e(n), n === "month" || n === "year") return t = this._days + this._milliseconds / 864e5, i = this._months + wr(t) * 12, n === "month" ? i : i / 12; t = this._days + Math.round(br(this._months / 12)); switch (n) { case "week": return t / 7 + this._milliseconds / 6048e5; case "day": return t + this._milliseconds / 864e5; case "hour": return t * 24 + this._milliseconds / 36e5; case "minute": return t * 1440 + this._milliseconds / 6e4; case "second": return t * 86400 + this._milliseconds / 1e3; case "millisecond": return Math.floor(t * 864e5) + this._milliseconds; default: throw new Error("Unknown unit " + n); } }, lang: t.fn.lang, locale: t.fn.locale, toIsoString: o("toIsoString() is deprecated. Please use toISOString() instead (notice the capitals)", function () { return this.toISOString() }), toISOString: function () { var r = Math.abs(this.years()), u = Math.abs(this.months()), f = Math.abs(this.days()), n = Math.abs(this.hours()), t = Math.abs(this.minutes()), i = Math.abs(this.seconds() + this.milliseconds() / 1e3); return this.asSeconds() ? (this.asSeconds() < 0 ? "-" : "") + "P" + (r ? r + "Y" : "") + (u ? u + "M" : "") + (f ? f + "D" : "") + (n || t || i ? "T" : "") + (n ? n + "H" : "") + (t ? t + "M" : "") + (i ? i + "S" : "") : "P0D" }, localeData: function () { return this._locale }, toJSON: function () { return this.toISOString() } }); t.duration.fn.toString = t.duration.fn.toISOString; for (u in ci) g(ci, u) && te(u.toLowerCase()); t.duration.fn.asMilliseconds = function () { return this.as("ms") }; t.duration.fn.asSeconds = function () { return this.as("s") }; t.duration.fn.asMinutes = function () { return this.as("m") }; t.duration.fn.asHours = function () { return this.as("h") }; t.duration.fn.asDays = function () { return this.as("d") }; t.duration.fn.asWeeks = function () { return this.as("weeks") }; t.duration.fn.asMonths = function () { return this.as("M") }; t.duration.fn.asYears = function () { return this.as("y") }; t.locale("en", { ordinalParse: /\d{1,2}(th|st|nd|rd)/, ordinal: function (n) { var t = n % 10, r = i(n % 100 / 10) === 1 ? "th" : t === 1 ? "st" : t === 2 ? "nd" : t === 3 ? "rd" : "th"; return n + r } }); ui ? module.exports = t : typeof define == "function" && define.amd ? (define(function (n, i, r) { return r.config && r.config() && r.config().noGlobal === !0 && (rt.moment = ri), t }), kr(!0)) : kr() }).call(this);




// Currently this has to be called on every page to work
var StateManager = function (container, callback) {
	this.container = $(container);
	if (!window.btoa) return false;
	this.storedName = btoa(location.pathname);
	this.callback = callback;
	this.checkFormState();
	this.retrieveFormState();
};

StateManager.prototype = {
	inputType: function (input) {
		var tagname = input[0].tagName.toLowerCase();
		return tagname === 'input' ? input.prop('type').toLowerCase() : tagname;
	},
	saveFormState: function () {
		this.container.find('input,select,textarea')
		.each(function (e) {
			var input = $(this);
			if (input.is('select')) {
				input.find('option').each(function (e) {
					$(this).attr('selected', this.value == input.val());
				});
			}
			else if (input.is('textarea')) {
				input.text(input.val());
			}
			else if (input.is(':checkbox') || input.is(':radio')) {
				input.attr('checked', input[0].checked);
			}
			else {
				input.attr('value', input.val());
			}
		});
		var content = this.container[0].innerHTML;
		sessionStorage.setItem(this.storedName, JSON.stringify(content));
	},
	retrieveFormState: function () {
		if (sessionStorage.getItem(this.storedName)) {
			// Use JSON to retrieve the stored data and convert it 
			var storedData = sessionStorage[this.storedName];
			this.container.html(JSON.parse(storedData));
			if (this.callback) this.callback(this.container);
		}
	},
	checkFormState: function () {
		if (sessionStorage.currentUrl) {
			if (sessionStorage.currentUrl == location.href) {
				if (sessionStorage[this.storedName]) {
					sessionStorage.removeItem(this.storedName);
				}
			}
		}
		sessionStorage.setItem('currentUrl', location.href);
	},
	destroy: function () {
		sessionStorage.clear();
	}
};







var isMobile = {
	Android: function () { return navigator.userAgent.match(/Android/i) != null; },
	BlackBerry: function () { return navigator.userAgent.match(/BlackBerry/i) != null; },
	iOS: function () { return navigator.userAgent.match(/iPhone|iPad|iPod/i) != null; },
	Opera: function () { return navigator.userAgent.match(/Opera Mini/i) != null; },
	Windows: function () { return navigator.userAgent.match(/IEMobile/i) != null; },
	Webkit: function () { return (isMobile.Android() || isMobile.iOS()); },
	any: function () { return (isMobile.Android() || isMobile.BlackBerry() || isMobile.iOS() || isMobile.Opera() || isMobile.Windows()); }
};





function isInIframe() {
	try {
		return window.self !== window.top;
	} catch (e) {
		return true;
	}
}




function AddressByZip(_zip, _city, _state, _readonly) {
	// when the zip code changes…
	function SetReadOnly(input) {
		//input.nextElementSibling.textContent = '';
		input.prop('readonly', true);
	}

	$(_zip).on('keyup', function () {
		var googleUrl = 'http://maps.googleapis.com/maps/api/geocode/json?';
		var getParams = { address: '', sensor: true };
		if (this.value.length === 5) {
			getParams.address = this.value;
			var finalUrl = googleUrl + $.param(getParams);
			//this.nextElementSibling.textContent = '';
			this.blur();
			$.get(finalUrl, function (data) {
				if (data.status === 'OK') {
					var city, state, comp = data.results[0].address_components;
					for (var i = 0; i < comp.length; i++) {
						if (comp[i].types[0] === 'locality') {
							city = $(_city).val(comp[i].long_name);
							if (_readonly) SetReadOnly(city);
						}
						else if (comp[i].types[0] === 'administrative_area_level_1') {
							state = $(_state).val(comp[i].long_name);
							if (_readonly) SetReadOnly(state);
						}
					}
				}
			});
		}
	})
}// AddressByZip




function PopupCenter(url, title, w, h) {
	// Fixes dual-screen position                         Most browsers      Firefox
	var dualScreenLeft = window.screenLeft != undefined ? window.screenLeft : screen.left;
	var dualScreenTop = window.screenTop != undefined ? window.screenTop : screen.top;

	var width = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
	var height = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

	var left = ((width / 2) - (w / 2)) + dualScreenLeft;
	var top = ((height / 2) - (h / 2)) + dualScreenTop;

	var params = 'directories=no,titlebar=no,toolbar=no,location=no,status=no,menubar=no,scrollbars=no,resizable=no';
	var newWindow = window.open(url, title, params + ',width=' + w + ',height=' + h + ',top=' + top + ',left=' + left);

	// Puts focus on the newWindow
	if (window.focus) {
		newWindow.focus();
	}
}




function isNationwide() {
	var productId = parseInt($('#product_id').val());
	return (productId == 65 || productId == 66 || productId == 67);
}



var dateFormat = isMobile.Webkit() ? 'YYYY-MM-DD' : 'MM/DD/YYYY';


// Bind all Events related to Datepicker
function bindDatepickerEvents() {
	// Datepicker defaults
	$.fn.datepicker.defaults.disableTouchKeyboard = true;
	$.fn.datepicker.defaults.enableOnReadonly = false;
	$.fn.datepicker.defaults.showOnFocus = !isMobile.Webkit() ? true : false;
	$.fn.datepicker.defaults.assumeNearbyYear = true;
	$.fn.datepicker.defaults.autoclose = true;
	$.fn.datepicker.defaults.format = !isMobile.Webkit() ? 'mm/dd/yyyy' : 'YYYY-MM-DD';
	$.fn.datepicker.defaults.zIndexOffset = 1000;
	$.fn.datepicker.defaults.templates = {
		leftArrow: '<i class="fa fa-caret-left"></i>',
		rightArrow: '<i class="fa fa-caret-right"></i>'
	};

	//$('body').append('<div>x:'+isMobile.Webkit()+'</div>');

	var dobs = $('.dob, .travel-age, .date-prior');
	dobs.each(createPriorPicker);

	var exps = $('.expiration-date');
	exps.each(createExpirationPicker);

	var eff = $('#effDate'), term = $('#termDate');
	if (eff.length) createRangePickers(eff, term);

	var purch = $('#purchaseDate');
	purch.each(createPurchaseDatePicker);


	if (isMobile.Webkit()) {

		$('.input-group.date:not(.expiration-date)').each(function (e) {
			var picker = $(this);
			var min = moment(picker.datepicker('getStartDate')).format('YYYY-MM-DD');
			var max = moment(picker.datepicker('getEndDate')).format('YYYY-MM-DD');

			//console.log('min: ', moment(picker.datepicker('getStartDate')).format('YYYY-MM-DD'));
			//console.log('max: ', moment(picker.datepicker('getEndDate')).format('YYYY-MM-DD'));

			picker.find('input')
			.attr({
				'type': 'date',
				'min': min == 'Invalid date' ? null : min,
				'max': max == 'Invalid date' ? null : max
			})
			.on('change', function () {
				this.blur();
			});

			picker.find('.input-group-addon').off()
			.on('click', function () {
				//picker.find('input').focus();
				picker.find('input').trigger('click');
			})
		});

		//$('.expiration-date').find('input').attr('type', 'month');
	}

}



function createPurchaseDatePicker(e) {
	// Deposit date
	var picker = $(this);
	picker.datepicker({
		endDate: moment().subtract(1, 'd').format(dateFormat)
	})
	.on('hide', function (e) {
		picker.find('input').trigger('blur');
	});
}




// Create datepicker for birth/prior date
function createPriorPicker(e) {
	var picker = $(this), min = -Infinity, max = '+1d';
	var input = picker.is(':input') ? picker : picker.find('input');

	var memberAge = picker.data('memberAge') || input.data('memberAge');
	var effDate = picker.data('effDate') || input.data('effDate');

	var dateObj = null;

	if (effDate && memberAge != undefined) {
		//console.log('passed');
		memberAge = parseInt(memberAge);
		effDate = moment(effDate, dateFormat);
		var date = moment(effDate).subtract(memberAge, 'y');// clone effDate
		dateObj = { 'year': date.year(), 'month': date.month(), 'day': date.day() };
		max = Infinity;
	}

	picker.datepicker({
		defaultViewDate: dateObj,
		startView: picker.is('.dob') ? 'decade' : 0,
		startDate: min,
		endDate: max
	})
	.on('hide', function (e) {
		if (e.date) {
			// Age at time of travel.
			var travelAge = getTravelDate().diff(moment(e.date), 'years');
			picker.closest('.row').find('[name$="travelerAge"]').val(travelAge);
			picker.closest('.row').find('label>.badge').text(travelAge);
			if (effDate && memberAge >= 0) {
				// Age at time of travel.
				var isBorn = effDate.diff(moment(e.date), 'days', false) > 0;
				if (!isBorn) {
					AgeBands.notBorn(input[0]);
				}
				else {
					var selectedAge = effDate.diff(moment(e.date), 'years', false);
					AgeBands.checkAge(input[0], selectedAge);
				}
			}
		}
		input.trigger('blur');
	});
	// This can later be applied to all datepickers if it works
	picker.on('changeDate', function (e) {
		if (input.val().length > 10) {
			// format year to only 4 characters
			var year = e.date.getFullYear().toString();
			e.date.setFullYear(year.substr(0, 4));
			picker.datepicker('update', e.date);
		}
	});

}



function fixDateYear(picker) {
	var input = picker.is(':input') ? picker : picker.find('input');
	picker.on('changeDate', function (e) {
		if (input.val().length > 10) {
			// format year to only 4 characters
			var year = e.date.getFullYear().toString();
			e.date.setFullYear(year.substr(0, 4));
			picker.datepicker('update', e.date);
		}
	});
}






//function getEffDatepicker() {
//	var effDate = $('#effDate');
//	//if (!effDate.length) effDate = $('#eff_date');
//	if (effDate.data('datepicker')) {
//		return effDate.datepicker();
//		//return effDate.data('datepicker').element;
//	}
//	return null;
//}



function getTravelDate() {
	var date = $('#effDate').datepicker('getDate');
	return date ? moment(date, dateFormat) : moment();
}



function updateAllAgeBadges() {
	$('[name$="travelerDOB"]').each(function () {
		var input = $(this), dob = input.val();
		if (dob) {
			var age = getTravelDate().diff(moment(dob, dateFormat), 'years');
			input.closest('.row').find('[name$="travelerAge"]').val(age);
			input.closest('.row').find('label>.badge').text(age);
		}
	});
}






// Create datepickers for date ranges
function createRangePickers(start, end) {

	start.datepicker({
		startDate: moment().add(1, 'd').format(dateFormat)
	})
	.on('hide', function (e) {
		if (e.date) {
			updateRangeRestrictions(start, end, start);
			updateAllAgeBadges();// Exact Care
		}
		start.find('input').trigger('blur');
		//getTimeSpan(start, end)
	});

	end.datepicker({
		startDate: moment().add(2, 'd').format(dateFormat)
	})
	.on('hide', function (e) {
		if (e.date) {
			updateRangeRestrictions(start, end, end);
		}
		end.find('input').trigger('blur');
		//getTimeSpan(start, end)
	});


	fixDateYear(start);
	fixDateYear(end);

	updateRangeRestrictions(start, end);

}



function updateRangeRestrictions(start, end, picker) {

	if (!start.length || !end.length) {
		// only for date ranges
		return false;
	}

	//var productId = parseInt($('#product_id').val());
	//var isNW = (productId == 65 || productId == 66 || productId == 67);


	var restrict = true;

	var min = start.datepicker('getDate');
	var max = end.datepicker('getDate');

	if (picker) {
		if (restrict) {
			if (picker == start) {
				_setMinDate();
			}
			else if (picker == end) {
				_setMaxDate();
			}
		}
	}


	if (min && max) {

		if (restrict && !picker) {
			_setMinDate();
			_setMaxDate();
		}

		if (!restrict) {
			var overlap = moment(max).diff(moment(min), 'days');
			var errorMsg = '';
			start.closest('.form-group').find('.field-validation-valid').html(errorMsg);
			end.closest('.form-group').find('.field-validation-valid').html(errorMsg);

			if (overlap < 0) {
				if (picker == start) {
					errorMsg = 'return date must be later than ' + moment(min).format(dateFormat);
					//console.log(errorMsg);
					_throwError(end, true);
					end.closest('.form-group').find('.field-validation-valid').html(errorMsg);
				}
				else if (picker == end) {
					errorMsg = 'depart date must be earlier than ' + moment(max).format(dateFormat);
					//console.log(errorMsg);
					_throwError(start, true);
					start.closest('.form-group').find('.field-validation-valid').html(errorMsg);
				}
			}
			else if (overlap == 0) {
				errorMsg = 'dates can\'t be the same day';
				//console.log(errorMsg);
				_throwError(start);
				_throwError(end);
				start.closest('.form-group').find('.field-validation-valid').html(errorMsg);
				end.closest('.form-group').find('.field-validation-valid').html(errorMsg);
			}
			else {
				//console.log('dates are good');
			}
		}

	}

	function _setMinDate() {
		var minDate = moment(min).add(1, 'day').format(dateFormat);
		end.datepicker('setStartDate', minDate);

		if (isNationwide()) {
			var maxDate = moment(min).add(89, 'day').format(dateFormat);
			//console.log(maxDate);
			end.datepicker('setEndDate', maxDate);
		}
	}

	function _setMaxDate() {
		var maxDate = moment(max).subtract(1, 'day').format(dateFormat);
		start.datepicker('setEndDate', maxDate);
	}

	function _throwError(element, reset) {
		element.closest('.form-group').removeClass('has-success').addClass('has-error');
		if (reset) {
			element.datepicker('update', '');// reset
			element.find('input').focus();
		}
	}

}




function getTimeSpan(start, end, callback) {
	var min = start.datepicker('getDate');
	var max = end.datepicker('getDate');
	//console.log('getTimeSpan: ', start.datepicker(), end.datepicker());
	if (min && max) {
		var days = moment(max).diff(moment(min), 'days') + 1;
		if (callback) callback(days);
		else return days;
	} else {
		return false;
	}
}



// Create datepicker for CC expiration date
function createExpirationPicker(e) {
	var picker = $(this);
	picker.datepicker({
		format: 'mm/yy',//'mm/yyyy'
		startDate: '0d',
		startView: 'year',
		minViewMode: 'months',
		showOnFocus: true
	});
}

function validExpiry() {
	var s = $('#expirationDate').val();
	if (/^\d{6}$/.test(s)) {
		return true;
	} else {
		// show error
		return false;
	}
}




// Bind all Events related to Travelers
function bindTravelerEvents() {
	var numberOfTravelers = $('#numberOfTravelers');
	var addTravelers = $('#addTravelers');

	numberOfTravelers
	.on('focus click', function (e) {
		this.select();
	})
	.on('input', function (e) {
		var add = numberOfTravelers[0].value;
		// Make sure at least 1 remains
		add = Math.max(add, 1);
		numberOfTravelers.val(add);
		var total = $('.traveler').length;
		if (add && add != total) {
			//var icon = add > total ? 'plus' : 'minus';
			//addTravelers.prop('disabled', false)
			//.html('Travelers <i class="fa fa-' + icon + '-circle"></i>');
			var icon = add > total ? 'Add' : 'Remove';
			addTravelers.prop('disabled', false)
			.html(icon + ' Travelers').removeClass('hidden');
		} else {
			//addTravelers.prop('disabled', true)
			//.html('Travelers <i class="fa fa-check-circle"></i>');
			addTravelers.prop('disabled', true)
			.html('Travelers').addClass('hidden');
		}
	});

	addTravelers.on('click', function (e) {
		var add = numberOfTravelers.val();
		var total = $('.traveler').length;
		updateBHTravelerList(total, add);
		//$(this).prop('disabled', true)
		//.html('Travelers <i class="fa fa-check-circle"></i>');
		$(this).prop('disabled', true)
		.html('Travelers').addClass('hidden');

		addTravelers.trigger('postclick');
	});


	var destination = $('#destination');
	var stateSelect = $('#stateSelect');

	destination.on('change', function (e) {
		if (this.value === 'US') {
			stateSelect.collapse('show');
		}
		else {
			stateSelect.collapse('hide');
			stateSelect.find('.form-group')
			.removeClass('has-success has-error')
			.find('.form-control').val('');
		}
	});

	//$('#bh-travelers .dob').each(createPriorPicker);

	if (typeof Travelers === 'function') {
		var kidsallowed = [14, 17, 38, 39];
		var pId = parseInt($('#product_id').val());
		if (kidsallowed.indexOf(pId) >= 0) {
			var children = new Travelers('#children');
		}
		else {
			var travelers = new Travelers('#travelers');
		}
	}
}


// Traveler info inputs
function updateBHTravelerList(total, add) {
	//var template = $('#primary').clone().removeAttr('id').html();
	//template = template.replace(/\_(\d+)\__/g, '_{x}__').replace(/\[(\d+)\]./g, '[{x}].');
	var template = $('#primary').clone();
	template.find(':input').removeAttr('aria-invalid');
	template = template.html().replace(/\_(\d+)\__/g, '_{x}__').replace(/\[(\d+)\]./g, '[{x}].');
	//console.log(template);
	if (add > total) {
		var fragment = $(document.createDocumentFragment());
		for (var i = total; i < add; i++) {
			var row = $('<div/>').attr('data-traveler', i).addClass('traveler');
			row.append(template.replace(/{x}/g, i));
			fragment.append(row);
		}
		fragment.find('.has-success').removeClass('has-success');
		fragment.find('.has-error').removeClass('has-error');
		//fragment.find(':input').val('');
		fragment.find(':input').each(function (e) {
			var input = $(this);
			input.val(input[0].defaultValue);
			if (input.is('[type=number]')) {
				input.attr('value', 0);
				input.val(0);
			}
		});
		fragment.find('.badge').text('');
		fragment.find('.dob').each(createPriorPicker);
		fragment.appendTo($('#bh-travelers'));
	}
	if (add < total) {
		var rows = $('.traveler')
		rows.each(function (e) {
			var row = $(this);
			if (row.data('traveler') >= add) {
				row.remove();
			}
		});
	}
}





function bindEvents(baseForm) {
	//alert('bindEvents');
	bindDatepickerEvents();
	bindTravelerEvents();
}




$(function () {
	
	//console.log(navigator.userAgent);
	//alert(navigator.userAgent);
	// Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; .NET4.0C; .NET4.0E; .NET CLR 2.0.50727; .NET CLR 3.0.30729; .NET CLR 3.5.30729; rv:11.0) like Gecko
	// Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 10.0; WOW64; Trident/7.0; .NET4.0C; .NET4.0E; .NET CLR 2.0.50727; .NET CLR 3.0.30729; .NET CLR 3.5.30729)


	$("body").iealert({ closeBtn: false });


	$('#modal').on('show.bs.modal', function (event) {
		var button = $(event.relatedTarget); // Button that triggered the modal
		var recipient = button.data('msg'); // Extract info from data-* attributes
		// If necessary, you could initiate an AJAX request here (and then do the updating in a callback).
		// Update the modal's content. We'll use jQuery here, but you could use a data binding library or other methods instead.
		var modal = $(this)
		modal.find('.modal-title').text('New message to ' + recipient)
		modal.find('.modal-body input').val(recipient)
	});



	// Default form controls
	$('body').on('change', '[data-toggle="checkbox"]', function (e) {
		$(this.dataset.target).toggleClass('collapse', !this.checked);
	});

	$('body').on('change', 'select.form-control', function (e) {
		$(this).trigger('blur');
	});

	$('body').on('click', '.widget-footer .btn-prev', function (e) {
		window.history.back();
	});

	//$('body').on('show hide', '.input-group.date', function (e) {
	//	$(this).find('input').toggleClass('focus', e.type == 'show');
	//});



	// Visitor Matrix table
	$('#matrix a').on('click', function (e) {
		var button = $(this),
		planId = button.data('plan'),
		deductible = button.data('deductible'),
		idx = button.closest('td').index();

		//$('#SelectedPrice').attr("value", button.text());

		$('#matrix a').removeClass('btn-success');

		$('#matrix th:eq(' + idx + ')').addClass('selected')
		.siblings().removeClass('selected');

		button.addClass('btn-success')
		.closest('tr').addClass('selected')
		.siblings().removeClass('selected');

		$('select[name="plan"]').val(planId);

		//$('select[name="deductible"]').val(deductible);
	});



	// File Uploads
	$(document).on('change', ':file', function () {
		var input = $(this),
				numFiles = input.get(0).files ? input.get(0).files.length : 1,
				label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
		input.trigger('fileselect', [numFiles, label]);
	});

	$(':file').on('fileselect', function (event, numFiles, label) {
		var input = $(this).parents('.input-group').find(':text'),
				log = numFiles > 1 ? numFiles + ' files selected' : label;
		if (input.length) input.val(log);
		else if (log) alert(log);

		//console.log(numFiles, label);
	});


	// http://stackoverflow.com/questions/20044554/how-to-clear-browsing-history-using-javascript
	//$('.jumper').on('click', function (e) {
	//	var jumper = $(this);
	//	if (e.preventDefault) {
	//		e.preventDefault();
	//	} else {
	//		e.returnValue = true;
	//	}
	//	if (jumper.is('[type=submit]')) {
			
	//		location.replace(this.href);
	//	} else {
	//		location.replace(this.href);
	//	}
	//	//jumper = null;
	//});



	





	var baseForm = $('#baseform');

	var state = new StateManager(baseForm, bindEvents);
	bindEvents(baseForm);



	//var formAny = $('#baseform');
	var formAny = $('#baseform, #optionsform, #memberform, #ccform');
	if (formAny.length) {
		//if (baseForm.length) {
		//console.log(formAny);

		// Add validation to form elements
		var baseValidator = formAny.validate({
			//var baseValidator = baseForm.validate({

			debug: false,
			errorClass: 'has-error',
			validClass: 'has-success',

			highlight: function (element, errorClass, validClass) {
				$(element).closest('.form-group')
				.addClass(errorClass).removeClass(validClass);
			},
			unhighlight: function (element, errorClass, validClass) {
				$(element).closest('.form-group')
				.addClass(validClass).removeClass(errorClass);
			},
			showErrors: function (errorMap, errorList) {
				// Clean up any tooltips for valid elements
				$.each(this.validElements(), function (index, element) {
					//var $element = $(element).not('[readonly]');
					var $element = $(element);
					$element.tooltip('destroy')
					.data('title', '')
					.closest('.form-group')
					.addClass('has-success')
					.removeClass('has-error');
				});
				// Create new tooltips for invalid elements
				$.each(errorList, function (index, error) {
					var $element = $(error.element);
					$element.tooltip('destroy')
					.data('title', error.message)
					.tooltip({ placement: 'bottom' })
					.closest('.form-group')
					.addClass('has-error')
					.removeClass('has-success');
				});
			},
			rules: {
				'eff_date': {
					required: true, dateLessThan: "#term_date"
				},
				'term_date': {
					required: true, dateGreaterThan: "#eff_date"
				},
				'CCPartial.school_name': {
					required: true
				},
				'CCPartial.spouseAge': {
					required: {
						depends: function () {
							return ($('#CCPartial_includeSpouse').is(':checked'));
						}
					},
					min: 1
				},
				'TravelerAges[0].travelerAge': {
					required: true,
					min: 1
				},
				'country': {
					required: true,
					notEqualTo: '#destination'
				},
				'destination': {
					required: true,
					notEqualTo: '#country'
				}
			},
			submitHandler: function (form) {
				if (baseValidator.form()) {
					state.saveFormState();
					form.submit();
				}
			}
		});


		//if (baseValidator.form()) {}

	}

});



function FormChanges(form) {
	// get form
	if (typeof form == "string") form = document.getElementById(form);
	if (!form || !form.nodeName || form.nodeName.toLowerCase() != "form") return null;

	// find changed elements
	var changed = [], n, c, def, o, ol, opt;
	for (var e = 0, el = form.elements.length; e < el; e++) {
		n = form.elements[e];
		c = false;

		switch (n.nodeName.toLowerCase()) {
			// select boxes
			case "select":
				def = 0;
				for (o = 0, ol = n.options.length; o < ol; o++) {
					opt = n.options[o];
					c = c || (opt.selected != opt.defaultSelected);
					if (opt.defaultSelected) def = o;
				}
				if (c && !n.multiple) c = (def != n.selectedIndex);
				break;
				// input / textarea
			case "textarea":
			case "input":
				switch (n.type.toLowerCase()) {
					case "checkbox":
					case "radio":
						// checkbox / radio
						c = (n.checked != n.defaultChecked);
						break;
					default:
						// standard values
						c = (n.value != n.defaultValue);
						break;
				}
				break;
		}
		if (c) changed.push(n);
	}
	return changed;
}

//$.fn.equalizeHeights = function () {
//	var maxHeight = this.map(function (i, e) {
//		return $(e).height();
//	}).get();
//	return this.height(Math.max.apply(this, maxHeight));
//};
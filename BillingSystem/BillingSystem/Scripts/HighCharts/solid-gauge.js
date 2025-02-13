﻿/*
  Highcharts JS v4.1.3 (2015-02-27)
 Solid angular gauge module

 (c) 2010-2014 Torstein Honsi

 License: www.highcharts.com/license
*/
(function (a) {
    var q = a.getOptions().plotOptions, r = a.pInt, s = a.pick, j = a.each, o; q.solidgauge = a.merge(q.gauge, { colorByPoint: !0 }); o = {
        initDataClasses: function (b) { var c = this, f = this.chart, d, n = 0, i = this.options; this.dataClasses = d = []; j(b.dataClasses, function (e, g) { var p, e = a.merge(e); d.push(e); if (!e.color) i.dataClassColor === "category" ? (p = f.options.colors, e.color = p[n++], n === p.length && (n = 0)) : e.color = c.tweenColors(a.Color(i.minColor), a.Color(i.maxColor), g / (b.dataClasses.length - 1)) }) }, initStops: function (b) {
            this.stops =
            b.stops || [[0, this.options.minColor], [1, this.options.maxColor]]; j(this.stops, function (b) { b.color = a.Color(b[1]) })
        }, toColor: function (b, c) {
            var f, d = this.stops, a, i = this.dataClasses, e, g; if (i) for (g = i.length; g--;) { if (e = i[g], a = e.from, d = e.to, (a === void 0 || b >= a) && (d === void 0 || b <= d)) { f = e.color; if (c) c.dataClass = g; break } } else {
                this.isLog && (b = this.val2lin(b)); f = 1 - (this.max - b) / (this.max - this.min); for (g = d.length; g--;) if (f > d[g][0]) break; a = d[g] || d[g + 1]; d = d[g + 1] || a; f = 1 - (d[0] - f) / (d[0] - a[0] || 1); f = this.tweenColors(a.color,
                d.color, f)
            } return f
        }, tweenColors: function (b, c, a) { var d = c.rgba[3] !== 1 || b.rgba[3] !== 1; return b.rgba.length === 0 || c.rgba.length === 0 ? "none" : (d ? "rgba(" : "rgb(") + Math.round(c.rgba[0] + (b.rgba[0] - c.rgba[0]) * (1 - a)) + "," + Math.round(c.rgba[1] + (b.rgba[1] - c.rgba[1]) * (1 - a)) + "," + Math.round(c.rgba[2] + (b.rgba[2] - c.rgba[2]) * (1 - a)) + (d ? "," + (c.rgba[3] + (b.rgba[3] - c.rgba[3]) * (1 - a)) : "") + ")" }
    }; j(["fill", "stroke"], function (b) {
        HighchartsAdapter.addAnimSetter(b, function (c) {
            c.elem.attr(b, o.tweenColors(a.Color(c.start), a.Color(c.end),
            c.pos))
        })
    }); a.seriesTypes.solidgauge = a.extendClass(a.seriesTypes.gauge, {
        type: "solidgauge", bindAxes: function () { var b; a.seriesTypes.gauge.prototype.bindAxes.call(this); b = this.yAxis; a.extend(b, o); b.options.dataClasses && b.initDataClasses(b.options); b.initStops(b.options) }, drawPoints: function () {
            var b = this, c = b.yAxis, f = c.center, d = b.options, n = b.radius = r(s(d.radius, 100)) * f[2] / 200, i = b.chart.renderer, e = d.overshoot, g = e && typeof e === "number" ? e / 180 * Math.PI : 0; a.each(b.points, function (a) {
                var e = a.graphic, h = c.startAngleRad +
                c.translate(a.y, null, null, null, !0), k = r(s(d.innerRadius, 60)) * f[2] / 200, l = c.toColor(a.y, a); l === "none" && (l = a.color || b.color || "none"); if (l !== "none") a.color = l; h = Math.max(c.startAngleRad - g, Math.min(c.endAngleRad + g, h)); d.wrap === !1 && (h = Math.max(c.startAngleRad, Math.min(c.endAngleRad, h))); var h = h * 180 / Math.PI, m = h / (180 / Math.PI), j = c.startAngleRad, h = Math.min(m, j), m = Math.max(m, j); m - h > 2 * Math.PI && (m = h + 2 * Math.PI); a.shapeArgs = k = { x: f[0], y: f[1], r: n, innerR: k, start: h, end: m, fill: l }; e ? (a = k.d, e.animate(k), k.d = a) : a.graphic =
                i.arc(k).attr({ stroke: d.borderColor || "none", "stroke-width": d.borderWidth || 0, fill: l, "sweep-flag": 0 }).add(b.group)
            })
        }, animate: function (b) { this.center = this.yAxis.center; this.center[3] = 2 * this.radius; this.startAngleRad = this.yAxis.startAngleRad; a.seriesTypes.pie.prototype.animate.call(this, b) }
    })
})(Highcharts);
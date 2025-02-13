/*
dhtmlxScheduler v.4.2.0 Stardard

This software is covered by GPL license. You also can obtain Commercial or Enterprise license to use it in non-GPL project - please contact sales@dhtmlx.com. Usage without proper license is prohibited.

(c) Dinamenta, UAB.
*/
scheduler._temp_matrix_scope = function () {



    scheduler.matrix = {};
    scheduler._merge = function (a, b) {
        for (var c in b)
            if (typeof a[c] == "undefined")
                a[c] = b[c];
    };
    scheduler.createTimelineView = function (obj) {
        scheduler._skin_init();

        scheduler._merge(obj, {
            section_autoheight: false,
            name: "matrix",
            x: "time",
            y: "time",
            x_step: 1,
            x_unit: "hour",
            y_unit: "day",
            y_step: 1,
            x_start: 0,
            x_size: 24,
            y_start: 0,
            y_size: 7,
            render: "cell",
            dx: 185,
            dy: 40,
            event_dy: scheduler.xy.bar_height - 5,
            event_min_dy: scheduler.xy.bar_height - 5,
            resize_events: true,
            fit_events: true,
            show_unassigned: false,
            second_scale: false,
            round_position: false,
            _logic: function (render_name, y_unit, timeline) {
                var res = {};
                if (scheduler.checkEvent("onBeforeSectionRender")) {
                    res = scheduler.callEvent("onBeforeSectionRender", [render_name, y_unit, timeline]);
                }
                return res;
            }
        });
        obj._original_x_start = obj.x_start;

        //first and last hours are applied only to day based timeline
        if (obj.x_unit != "day") obj.first_hour = obj.last_hour = 0;
        //correction for first and last hour
        obj._start_correction = obj.first_hour ? obj.first_hour * 60 * 60 * 1000 : 0;
        obj._end_correction = obj.last_hour ? (24 - obj.last_hour) * 60 * 60 * 1000 : 0;

        if (scheduler.checkEvent("onTimelineCreated")) {
            scheduler.callEvent("onTimelineCreated", [obj]);
        }

        var old = scheduler.render_data;
        scheduler.render_data = function (evs, mode) {
            if (this._mode == obj.name) {
                //repaint single event, precision is not necessary
                if (mode && !obj.show_unassigned && obj.render != "cell") {
                    for (var i = 0; i < evs.length; i++) {
                        this.clear_event(evs[i]);
                        this.render_timeline_event.call(this.matrix[this._mode], evs[i], true);
                    }
                } else {
                    scheduler._renderMatrix.call(obj, true, true);
                }
            } else
                return old.apply(this, arguments);
        };

        scheduler.matrix[obj.name] = obj;
        scheduler.templates[obj.name + "_cell_value"] = function (ar) { return ar ? ar.length : ""; };
        scheduler.templates[obj.name + "_cell_class"] = function (arr) { return ""; };
        scheduler.templates[obj.name + "_scalex_class"] = function (date) { return ""; };
        scheduler.templates[obj.name + "_second_scalex_class"] = function (date) { return ""; };

        scheduler.templates[obj.name + "_scaley_class"] = function (section_id, section_label, section_options) { return ""; };
        scheduler.templates[obj.name + "_scale_label"] = function (section_id, section_label, section_options) { return section_label; };

        scheduler.templates[obj.name + "_tooltip"] = function (a, b, e) { return e.text; };
        scheduler.templates[obj.name + "_date"] = function (datea, dateb) {
            if ((datea.getDay() == dateb.getDay() && dateb - datea < (24 * 60 * 60 * 1000)) ||
                +datea == +scheduler.date.date_part(new Date(dateb)) ||
                (+scheduler.date.add(datea, 1, "day") == +dateb && dateb.getHours() === 0 && dateb.getMinutes() === 0))
                return scheduler.templates.day_date(datea);
            if ((datea.getDay() != dateb.getDay() && dateb - datea < (24 * 60 * 60 * 1000))) {
                return scheduler.templates.day_date(datea) + " &ndash; " + scheduler.templates.day_date(dateb);
            }
            return scheduler.templates.week_date(datea, dateb);
        };

        scheduler.templates[obj.name + "_scale_date"] = scheduler.date.date_to_str(obj.x_date || scheduler.config.hour_date);
        scheduler.templates[obj.name + "_second_scale_date"] = scheduler.date.date_to_str((obj.second_scale && obj.second_scale.x_date) ? obj.second_scale.x_date : scheduler.config.hour_date);

        scheduler.date["add_" + obj.name] = function (date, step, c) {
            var resulting_date = scheduler.date.add(date, (obj.x_length || obj.x_size) * step * obj.x_step, obj.x_unit);
            if (obj.x_unit == "minute" || obj.x_unit == "hour") {
                var size = (obj.x_length || obj.x_size);
                var converted_step = (obj.x_unit == "hour") ? obj.x_step * 60 : obj.x_step;
                if ((converted_step * size) % (24 * 60)) {
                    //if scroll hours within the day
                    if (+scheduler.date.date_part(new Date(date)) == +scheduler.date.date_part(new Date(resulting_date))) {
                        obj.x_start += step * size;
                    } else {
                        // total steps starting from 0
                        var total_steps = ((24 * 60) / (size * converted_step)) - 1;
                        var steps_offset = Math.round(total_steps * size);

                        if (step > 0) {
                            obj.x_start = obj.x_start - steps_offset;
                        } else {
                            obj.x_start = steps_offset + obj.x_start;
                        }
                    }
                }
            }
            return resulting_date;
        };
        scheduler.date[obj.name + "_start"] = function (date) {
            var func = scheduler.date[obj.x_unit + "_start"] || scheduler.date.day_start;
            var start_date = func.call(scheduler.date, date);
            start_date = scheduler.date.add(start_date, obj.x_step * obj.x_start, obj.x_unit);
            return start_date;
        };


        scheduler.callEvent("onOptionsLoad", [obj]);

        //init custom wrappers
        scheduler[obj.name + "_view"] = function (enable) {
            if (enable) {
                scheduler._table_view = true;
                //_renderMatrix will be called by render_data immediately after
            } else {
                scheduler._renderMatrix.apply(obj, arguments);
            }
        };

        //enable drag for non-cell modes
        var temp_date = new Date();
        var step_diff = (scheduler.date.add(temp_date, obj.x_step, obj.x_unit).valueOf() - temp_date.valueOf()); // "minute" + step in ms
        scheduler["mouse_" + obj.name] = function (pos) { //mouse_coord handler
            //get event object
            var ev = this._drag_event;
            //Himanshu 10Aug2015: add condition to check if this._drag_event also not undefined to prevent undefined error
            if (this._drag_id && this._drag_event) {
                ev = this.getEvent(this._drag_id);
                this._drag_event._dhx_changed = true;
            }

            pos.x -= obj.dx;
            var summ = 0, xind = 0;
            var ratio,
                column_width;
            for (xind; xind <= this._cols.length - 1; xind++) {

                column_width = this._cols[xind];
                summ += column_width;
                if (summ > pos.x) { //index of section
                    ratio = (pos.x - (summ - column_width)) / column_width;
                    ratio = (ratio < 0) ? 0 : ratio;
                    break;
                }
            }

            if (obj.round_position) {
                // in case of click, mouse position will be always rounded to start date of the cell
                // when dragging - position can be rounded to the start date of the next column, in order to improve the usability
                // edge = 1 - always return start date of current cell
                // 0.5 - round to next cell if mouse in the right half of cell
                var edge = 1;
                if (scheduler.getState().drag_mode && scheduler.getState().drag_mode != "move") {
                    edge = 0.5;//rounding for resize
                }
                if (ratio >= edge) {
                    xind++;
                }
                ratio = 0;
            }

            //border cases
            if (xind === 0 && this._ignores[0]) {
                xind = 1; ratio = 0;
                while (this._ignores[xind]) xind++;
            } else if (xind == this._cols.length && this._ignores[xind - 1]) {
                xind = this._cols.length - 1; ratio = 0;
                while (this._ignores[xind]) xind--;
                xind++;
            }


            pos.x = 0;
            pos.force_redraw = true;
            pos.custom = true;

            var end_date;
            // if our event is at the end of the view
            if (xind >= obj._trace_x.length) {
                end_date = scheduler.date.add(obj._trace_x[obj._trace_x.length - 1], obj.x_step, obj.x_unit);
                if (obj._end_correction)
                    end_date = new Date(end_date - obj._end_correction);
            } else {
                var timestamp_diff = ratio * column_width * obj._step + obj._start_correction;
                end_date = new Date(+obj._trace_x[xind] + timestamp_diff);
            }

            // as we can simply be calling _locate_cell_timeline
            if (this._drag_mode == "move" && this._drag_id && this._drag_event) {
                var ev = this.getEvent(this._drag_id);
                var drag_event = this._drag_event;

                pos._ignores = (this._ignores_detected || obj._start_correction || obj._end_correction);
                if (!drag_event._move_delta) {
                    drag_event._move_delta = (ev.start_date - end_date) / 60000;
                    if (this.config.preserve_length && pos._ignores) {
                        drag_event._move_delta = this._get_real_event_length(ev.start_date, end_date, obj);
                        drag_event._event_length = this._get_real_event_length(ev.start_date, ev.end_date, obj);
                    }
                }




                //preserve visible size of event
                if (this.config.preserve_length && pos._ignores) {
                    var ev_length = drag_event._event_length;//this._get_real_event_length(ev.start_date, ev.end_date, obj);
                    var current_back_shift = this._get_fictional_event_length(end_date, drag_event._move_delta, obj, true);
                    end_date = new Date(end_date - current_back_shift);
                } else {
                    // converting basically to start_date
                    end_date = scheduler.date.add(end_date, drag_event._move_delta, "minute");
                }
            }

            if (this._drag_mode == "resize" && ev) {
                if (this._drag_from_start && +end_date > +ev.end_date) {
                    this._drag_from_start = false;
                } else if (!this._drag_from_start && +end_date < +ev.start_date) {
                    this._drag_from_start = true;
                }
                pos.resize_from_start = this._drag_from_start;
            }

            if (obj.round_position) {
                switch (this._drag_mode) {
                    case "move":
                        if (!this.config.preserve_length) {
                            end_date = get_rounded_date.call(obj, end_date, false);
                            // to preserve original start and end dates
                            if (obj.x_unit == "day")//only make sense for whole-day cells
                                pos.custom = false;
                        }
                        break;
                    case "resize":
                        if (this._drag_event) {
                            // will save and use resize position only once
                            if (this._drag_event._resize_from_start === null || this._drag_event._resize_from_start === undefined) {
                                this._drag_event._resize_from_start = pos.resize_from_start;
                            }
                            pos.resize_from_start = this._drag_event._resize_from_start;
                            end_date = get_rounded_date.call(obj, end_date, !this._drag_event._resize_from_start);
                        }
                        break;
                }
            }

            this._resolve_timeline_section(obj, pos);
            if (pos.section) {
                // update draggable event with current section
                this._update_timeline_section({ pos: pos, event: this.getEvent(this._drag_id), view: obj });
            }
            pos.y = Math.round((end_date - this._min_date) / (1000 * 60 * this.config.time_step));
            pos.shift = this.config.time_step; //step_diff;


            return pos;
        };
    };

    scheduler._get_timeline_event_height = function (ev, config) {
        var section = ev[config.y_property]; // section id
        var event_height = config.event_dy;
        if (config.event_dy == "full") {
            if (config.section_autoheight) {
                event_height = config._section_height[section] - 6;
            } else {
                event_height = config.dy - 3;
            }
        }

        if (config.resize_events) {
            event_height = Math.max(Math.floor(event_height / ev._count), config.event_min_dy);
        }
        return event_height;
    };
    scheduler._get_timeline_event_y = function (order, event_height) {
        var sorder = order;
        var y = 2 + sorder * event_height + (sorder ? (sorder * 2) : 0); // original top + number_of_events * event_dy + default event top/bottom borders
        if (scheduler.config.cascade_event_display) {
            y = 2 + sorder * scheduler.config.cascade_event_margin + (sorder ? (sorder * 2) : 0);
        }
        return y;
    };

    scheduler.render_timeline_event = function (ev, attach) {
        var section = ev[this.y_property]; // section id
        if (!section)
            return ""; // as we may await html

        var sorder = ev._sorder;

        var x_start = _getX(ev, false, this);
        var x_end = _getX(ev, true, this);

        var event_height = scheduler._get_timeline_event_height(ev, this);

        var hb = event_height - 2;// takes into account css sizes (border/padding)
        if (!ev._inner && this.event_dy == "full") {
            hb = (hb + 2) * (ev._count - sorder) - 2;
        }

        var y = scheduler._get_timeline_event_y(ev._sorder, event_height);

        var section_height = event_height + y + 2;
        if (!this._events_height[section] || (this._events_height[section] < section_height)) {
            this._events_height[section] = section_height;
        }

        var cs = scheduler.templates.event_class(ev.start_date, ev.end_date, ev);
        cs = "dhx_cal_event_line " + (cs || "");

        var bg_color = (ev.color ? ("background:" + ev.color + ";") : "");
        var color = (ev.textColor ? ("color:" + ev.textColor + ";") : "");
        var text = scheduler.templates.event_bar_text(ev.start_date, ev.end_date, ev);

        var content = text.split('^');
        content1 = '';
        content2 = '';
        if (content[0]) {
            content1 = content[0];
        }
        if (content[1]) {
            content2 = content[1];
        }
        if (text.indexOf('^') > 0) {
            text = "<div class='productname-text'>" + content1 + "</div><div class='productname-text'>" + content2 + "</div>";
        }

        var html = '<div event_id="' + ev.id + '" class="' + cs + '" style="' + bg_color + '' + color + 'position:absolute; top:' + y + 'px; height: ' + hb + 'px; left:' + x_start + 'px; width:' + Math.max(0, x_end - x_start) + 'px;' + (ev._text_style || "") + '">';
        if (scheduler.config.drag_resize && !scheduler.config.readonly) {
            var dhx_event_resize = 'dhx_event_resize';
            html += ("<div class='" + dhx_event_resize + " " + dhx_event_resize + "_start' style='height: " + hb + "px;'></div><div class='" + dhx_event_resize + " " + dhx_event_resize + "_end' style='height: " + hb + "px;'></div>");
        }
        html += (text + '</div>');

        if (!attach)
            return html;
        else {
            var d = document.createElement("DIV");
            d.innerHTML = html;
            var ind = this.order[section];
            var parent = scheduler._els["dhx_cal_data"][0].firstChild.rows[ind].cells[1].firstChild;

            scheduler._rendered.push(d.firstChild);
            parent.appendChild(d.firstChild);
        }
    };
    function trace_events() {
        //minimize event set
        var evs = scheduler.get_visible_events();
        var matrix = [];
        for (var i = 0; i < this.y_unit.length; i++)
            matrix[i] = [];

        //next code defines row for undefined key
        //most possible it is an artifact of incorrect configuration
        if (!matrix[y])
            matrix[y] = [];

        for (var i = 0; i < evs.length; i++) {
            var y = this.order[evs[i][this.y_property]];
            var x = 0;
            while (this._trace_x[x + 1] && evs[i].start_date >= this._trace_x[x + 1]) x++;
            while (this._trace_x[x] && evs[i].end_date > this._trace_x[x]) {
                if (!matrix[y][x]) matrix[y][x] = [];
                matrix[y][x].push(evs[i]);
                x++;
            }
        }
        return matrix;
    }
    // function used to get X (both start and end) coordinates for timeline bar view
    function _getX(ev, isEndPoint, config) {
        var x = 0;
        var step = config._step;
        var round_position = config.round_position;

        var column_offset = 0;
        var date = (isEndPoint) ? ev.end_date : ev.start_date;

        if (date.valueOf() > scheduler._max_date.valueOf())
            date = scheduler._max_date;
        var delta = date - scheduler._min_date_timeline;

        if (delta > 0) {
            var index = scheduler._get_date_index(config, date);
            if (scheduler._ignores[index])
                round_position = true;

            for (var i = 0; i < index; i++) {
                x += scheduler._cols[i];
            }

            var column_date = scheduler.date.add(scheduler._min_date_timeline, scheduler.matrix[scheduler._mode].x_step * index, scheduler.matrix[scheduler._mode].x_unit);
            if (!round_position) {
                delta = date - column_date;
                if (config.first_hour || config.last_hour) {
                    delta = delta - config._start_correction;
                    if (delta < 0) delta = 0;
                    column_offset = Math.round(delta / step);
                    if (column_offset > scheduler._cols[index])
                        column_offset = scheduler._cols[index];
                } else {
                    column_offset = Math.round(delta / step);
                }
            } else {
                if (+date > +column_date && isEndPoint) {
                    column_offset = scheduler._cols[index];
                }
            }
        }
        if (isEndPoint) {
            // special handling for "round" dates which match columns and usual ones
            if (delta !== 0 && !round_position) {
                x += column_offset - 12;
            } else {
                x += column_offset - 14;
            }
        } else {
            x += column_offset + 1;
        }
        return x;
    }
    function get_rounded_date(date, isEndDate) {
        var index = scheduler._get_date_index(this, date);
        var rounded_date = this._trace_x[index];
        if (isEndDate && (+date != +this._trace_x[index])) {
            rounded_date = (this._trace_x[index + 1]) ? this._trace_x[index + 1] : scheduler.date.add(this._trace_x[index], this.x_step, this.x_unit);
        }
        return new Date(rounded_date);
    }
    function get_events_html(evs) {
        var html = "";
        if (evs && this.render != "cell") {
            evs.sort(this.sort || function (a, b) {
                if (a.start_date.valueOf() == b.start_date.valueOf())
                    return a.id > b.id ? 1 : -1;
                return a.start_date > b.start_date ? 1 : -1;
            });
            var stack = [];
            var evs_length = evs.length;
            // prepare events for render
            for (var j = 0; j < evs_length; j++) {
                var ev = evs[j];
                ev._inner = false;

                var ev_start_date = (this.round_position) ? get_rounded_date.apply(this, [ev.start_date, false]) : ev.start_date;
                var ev_end_date = (this.round_position) ? get_rounded_date.apply(this, [ev.end_date, true]) : ev.end_date;

                // cutting stack from the last -> first event side
                while (stack.length) {
                    var stack_ev = stack[stack.length - 1];
                    if (stack_ev.end_date.valueOf() <= ev_start_date.valueOf()) {
                        stack.splice(stack.length - 1, 1);
                    } else {
                        break;
                    }
                }

                // cutting stack from the first -> last event side
                var sorderSet = false;
                for (var p = 0; p < stack.length; p++) {
                    var t_ev = stack[p];
                    if (t_ev.end_date.valueOf() <= ev_start_date.valueOf()) {
                        sorderSet = true;
                        ev._sorder = t_ev._sorder;
                        stack.splice(p, 1);
                        ev._inner = true;
                        break;
                    }
                }


                if (stack.length)
                    stack[stack.length - 1]._inner = true;


                if (!sorderSet) {
                    if (stack.length) {
                        if (stack.length <= stack[stack.length - 1]._sorder) {
                            if (!stack[stack.length - 1]._sorder)
                                ev._sorder = 0;
                            else
                                for (var h = 0; h < stack.length; h++) {
                                    var _is_sorder = false;
                                    for (var t = 0; t < stack.length; t++) {
                                        if (stack[t]._sorder == h) {
                                            _is_sorder = true;
                                            break;
                                        }
                                    }
                                    if (!_is_sorder) {
                                        ev._sorder = h;
                                        break;
                                    }
                                }
                            ev._inner = true;
                        }
                        else {
                            var _max_sorder = stack[0]._sorder;
                            for (var w = 1; w < stack.length; w++)
                                if (stack[w]._sorder > _max_sorder)
                                    _max_sorder = stack[w]._sorder;
                            ev._sorder = _max_sorder + 1;
                            ev._inner = false;
                        }
                    }
                    else
                        ev._sorder = 0;
                }

                stack.push(ev);

                if (stack.length > (stack.max_count || 0)) {
                    stack.max_count = stack.length;
                    ev._count = stack.length;
                }
                else {
                    ev._count = (ev._count) ? ev._count : 1;
                }
            }
            // fix _count for every event
            for (var m = 0; m < evs.length; m++) {
                evs[m]._count = stack.max_count;
            }
            // render events
            for (var v = 0; v < evs_length; v++) {
                html += scheduler.render_timeline_event.call(this, evs[v], false);
            }
        }
        return html;
    }


    function y_scale(d) {
        var html = "<table style='table-layout:fixed;' cellspacing='0' cellpadding='0'>";
        var evs = [];
        if (scheduler._load_mode)
            scheduler._load();
        if (this.render == "cell")
            evs = trace_events.call(this);
        else {
            var tevs = scheduler.get_visible_events();
            var order = this.order;

            for (var j = 0; j < tevs.length; j++) {
                var tev = tevs[j];
                var tev_section = tev[this.y_property];
                var index = this.order[tev_section];

                if (this.show_unassigned && !tev_section) {
                    for (var key in order) {
                        if (order.hasOwnProperty(key)) {
                            index = order[key];
                            if (!evs[index]) evs[index] = [];
                            var clone = scheduler._lame_copy({}, tev);
                            clone[this.y_property] = key;
                            evs[index].push(clone);
                        }
                    }
                } else {
                    // required as we could have index of not displayed section or "undefined"
                    if (!evs[index]) evs[index] = [];
                    evs[index].push(tev);
                }
            }
        }

        var summ = 0;
        for (var i = 0; i < scheduler._cols.length; i++)
            summ += scheduler._cols[i];

        var step = new Date();
        var realcount = scheduler._cols.length - scheduler._ignores_detected;
        step = ((scheduler.date.add(step, this.x_step * realcount, this.x_unit) - step) - (this._start_correction + this._end_correction) * realcount) / summ;
        this._step = step;
        this._summ = summ;

        var heights = scheduler._colsS.heights = [];
        var render_stats = [];

        this._events_height = {};
        this._section_height = {};
        for (var i = 0; i < this.y_unit.length; i++) {

            var stats = this._logic(this.render, this.y_unit[i], this); // obj with custom style

            scheduler._merge(stats, {
                height: this.dy
            });

            //autosize height, if we have a free space
            if (this.section_autoheight) {
                if (this.y_unit.length * stats.height < d.offsetHeight) {
                    stats.height = Math.max(stats.height, Math.floor((d.offsetHeight - 1) / this.y_unit.length));
                }
                this._section_height[this.y_unit[i].key] = stats.height;
            }


            if (!stats.td_className) {
                stats.td_className = "dhx_matrix_scell" + ((scheduler.templates[this.name + "_scaley_class"](this.y_unit[i].key, this.y_unit[i].label, this.y_unit[i])) ? " " + scheduler.templates[this.name + "_scaley_class"](this.y_unit[i].key, this.y_unit[i].label, this.y_unit[i]) : '');
            }
            if (!stats.td_content) {
                stats.td_content = scheduler.templates[this.name + '_scale_label'](this.y_unit[i].key, this.y_unit[i].label, this.y_unit[i]);
            }

            scheduler._merge(stats, {
                //section 1
                tr_className: "",
                style_height: "height:" + stats.height + "px;",
                style_width: "width:" + (this.dx - 1) + "px;",
                //section 2
                summ_width: "width:" + summ + "px;",
                //section 3
                table_className: ''
            });

            // generating events html in a temporary file, calculating their height
            var events_html = get_events_html.call(this, evs[i]);

            if (this.fit_events) {
                var rendered_height = this._events_height[this.y_unit[i].key] || 0;
                stats.height = (rendered_height > stats.height) ? rendered_height : stats.height;
                stats.style_height = "height:" + stats.height + "px;";
                this._section_height[this.y_unit[i].key] = stats.height;
            }

            // section 1
            html += "<tr class='" + stats.tr_className + "' style='" + stats.style_height + "'><td class='" + stats.td_className + "' style='" + stats.style_width + " height:" + (stats.height - 1) + "px;'><img class='employee-imgDP' src='" + this.y_unit[i].userProfileImagePath + "'><span onclick='bindWeekMode(" + '"' + this.y_unit[i].userId + '"' + ");' class='employee_NameDP pointer'>" + stats.td_content + "</span></td>";

            if (this.render == "cell") {
                for (var j = 0; j < scheduler._cols.length; j++) {
                    if (scheduler._ignores[j])
                        html += "<td></td>";
                    else
                        html += "<td class='dhx_matrix_cell " + scheduler.templates[this.name + "_cell_class"](evs[i][j], this._trace_x[j], this.y_unit[i]) + "' style='width:" + (scheduler._cols[j] - 1) + "px'><div style='width:" + (scheduler._cols[j] - 1) + "px'>" + scheduler.templates[this.name + "_cell_value"](evs[i][j], this._trace_x[j], this.y_unit[i]) + "</div></td>";
                }
            } else {
                //section 2
                html += "<td><div style='" + stats.summ_width + " " + stats.style_height + " position:relative;' class='dhx_matrix_line'>";

                // adding events
                html += events_html;

                //section 3
                html += "<table class='" + stats.table_className + "' cellpadding='0' cellspacing='0' style='" + stats.summ_width + " " + stats.style_height + "' >";
                for (var j = 0; j < scheduler._cols.length; j++) {
                    if (scheduler._ignores[j])
                        html += "<td></td>";
                    else
                        html += "<td class='dhx_matrix_cell " + scheduler.templates[this.name + "_cell_class"](evs[i], this._trace_x[j], this.y_unit[i]) + "' style='width:" + (scheduler._cols[j] - 1) + "px'><div style='width:" + (scheduler._cols[j] - 1) + "px'></div></td>";
                }
                html += "</table>";
                html += "</div></td>";
            }
            html += "</tr>";
            render_stats.push(stats);
        }
        html += "</table>";
        this._matrix = evs;

        //d.scrollTop = 0; //fix flickering in FF;  disabled as it was impossible to create dnd event if scroll was used (window jumped to the top)
        d.innerHTML = html;

        scheduler._rendered = [];
        var divs = scheduler._obj.getElementsByTagName("DIV");
        for (var i = 0; i < divs.length; i++)
            if (divs[i].getAttribute("event_id"))
                scheduler._rendered.push(divs[i]);

        this._scales = {};
        var rows = d.firstChild.rows;
        var unit = null;
        for (var i = 0, len = render_stats.length; i < len; i++) {
            unit = this.y_unit[i];
            heights.push(render_stats[i].height);

            //heights.push(rows[i].offsetHeight);
            var unit_key = unit.key;
            var scale = this._scales[unit_key] = (scheduler._isRender('cell')) ? rows[i] : rows[i].childNodes[1].getElementsByTagName('div')[0];
            scheduler.callEvent("onScaleAdd", [scale, unit_key]);
        }
    }
    function x_scale(h) {
        var current_sh = scheduler.xy.scale_height;
        var original_sh = this._header_resized || scheduler.xy.scale_height;
        scheduler._cols = [];	//store for data section, each column width
        scheduler._colsS = { height: 0 }; // heights of the y sections
        this._trace_x = []; // list of dates per cells
        var summ = scheduler._x - this.dx - scheduler.xy.scroll_width; //border delta, whole width
        var left = [this.dx]; // left margins, initial left margin
        var header = scheduler._els['dhx_cal_header'][0];
        header.style.width = (left[0] + summ) + 'px';

        scheduler._min_date_timeline = scheduler._min_date;

        var preserve = scheduler.config.preserve_scale_length;
        var start = scheduler._min_date;
        scheduler._process_ignores(start, this.x_size, this.x_unit, this.x_step, preserve);

        var size = this.x_size + (preserve ? scheduler._ignores_detected : 0);
        if (size != this.x_size)
            scheduler._max_date = scheduler.date.add(scheduler._min_date, size * this.x_step, this.x_unit);

        var realcount = size - scheduler._ignores_detected;
        for (var k = 0; k < size; k++) {
            // dates calculation
            this._trace_x[k] = new Date(start);
            start = scheduler.date.add(start, this.x_step, this.x_unit);

            // position calculation
            if (scheduler._ignores[k]) {
                scheduler._cols[k] = 0;
                realcount++;
            } else {
                scheduler._cols[k] = Math.floor(summ / (realcount - k));
            }

            summ -= scheduler._cols[k];
            left[k + 1] = left[k] + scheduler._cols[k];
        }
        h.innerHTML = "<div></div>";

        if (this.second_scale) {
            // additional calculations
            var mode = this.second_scale.x_unit;
            var control_dates = [this._trace_x[0]]; // first control date
            var second_cols = []; // each column width of the secondary row
            var second_left = [this.dx, this.dx]; // left margins of the secondary row
            var t_index = 0; // temp index
            for (var l = 0; l < this._trace_x.length; l++) {
                var date = this._trace_x[l];
                var res = is_new_interval(mode, date, control_dates[t_index]);

                if (res) { // new interval
                    ++t_index; // starting new interval
                    control_dates[t_index] = date; // updating control date as we moved to the new interval
                    second_left[t_index + 1] = second_left[t_index];
                }
                var t = t_index + 1;
                second_cols[t_index] = scheduler._cols[l] + (second_cols[t_index] || 0);
                second_left[t] += scheduler._cols[l];
            }

            h.innerHTML = "<div></div><div></div>";
            var top = h.firstChild;
            top.style.height = (original_sh) + 'px'; // actually bottom header takes 21px
            var bottom = h.lastChild;
            bottom.style.position = "relative";

            for (var m = 0; m < control_dates.length; m++) {
                var tdate = control_dates[m];
                var scs = scheduler.templates[this.name + "_second_scalex_class"](tdate);
                var head = document.createElement("DIV"); head.className = "dhx_scale_bar dhx_second_scale_bar" + ((scs) ? (" " + scs) : "");
                scheduler.set_xy(head, second_cols[m] - 1, original_sh - 3, second_left[m], 0); //-1 for border, -3 = -2 padding -1 border bottom
                head.innerHTML = scheduler.templates[this.name + "_second_scale_date"](tdate);
                top.appendChild(head);
            }
        }

        scheduler.xy.scale_height = original_sh; // fix for _render_x_header which uses current scale_height value
        h = h.lastChild; // h - original scale
        for (var i = 0; i < this._trace_x.length; i++) {
            if (scheduler._ignores[i])
                continue;

            start = this._trace_x[i];
            scheduler._render_x_header(i, left[i], start, h);
            var cs = scheduler.templates[this.name + "_scalex_class"](start);
            if (cs)
                h.lastChild.className += " " + cs;
        }
        scheduler.xy.scale_height = current_sh; // restoring current value

        var trace = this._trace_x;
        h.onclick = function (e) {
            var pos = locate_hcell(e);
            if (pos)
                scheduler.callEvent("onXScaleClick", [pos.x, trace[pos.x], e || event]);
        };
        h.ondblclick = function (e) {
            var pos = locate_hcell(e);
            if (pos)
                scheduler.callEvent("onXScaleDblClick", [pos.x, trace[pos.x], e || event]);
        };
    }
    function is_new_interval(mode, date, control_date) { // mode, date to check, control_date for which period should be checked
        switch (mode) {
            case "hour":
                return ((date.getHours() != control_date.getHours()) || is_new_interval("day", date, control_date));
            case "day":
                return !(date.getDate() == control_date.getDate() && date.getMonth() == control_date.getMonth() && date.getFullYear() == control_date.getFullYear());
            case "week":
                return !(scheduler.date.getISOWeek(date) == scheduler.date.getISOWeek(control_date) && date.getFullYear() == control_date.getFullYear());
            case "month":
                return !(date.getMonth() == control_date.getMonth() && date.getFullYear() == control_date.getFullYear());
            case "year":
                return !(date.getFullYear() == control_date.getFullYear());
            default:
                return false; // same interval
        }
    }
    function set_full_view(mode) {
        if (mode) {
            scheduler.set_sizes();
            scheduler._init_matrix_tooltip();

            //we need to have day-rounded scales for navigation
            //in same time, during rendering scales may be shifted
            var temp = scheduler._min_date;
            x_scale.call(this, scheduler._els["dhx_cal_header"][0]);
            y_scale.call(this, scheduler._els["dhx_cal_data"][0]);
            scheduler._min_date = temp;
            scheduler._els["dhx_cal_date"][0].innerHTML = '<input type="text" class="customDate pointer" readonly="readonly" onfocus="this.blur();" id="datepicker"><span>' + scheduler.templates[this.name + "_date"](scheduler._min_date, scheduler._max_date) + '</span>';
            if (scheduler._mark_now) {
                scheduler._mark_now();
            }
        }

        // hide tooltip if it is displayed
        hideToolTip();
    }


    function hideToolTip() {
        if (scheduler._tooltip) {
            scheduler._tooltip.style.display = "none";
            scheduler._tooltip.date = "";
        }
    }
    function showToolTip(obj, pos, offset) {
        if (obj.render != "cell") return;
        var mark = pos.x + "_" + pos.y;
        var evs = obj._matrix[pos.y][pos.x];

        if (!evs) return hideToolTip();

        evs.sort(function (a, b) { return a.start_date > b.start_date ? 1 : -1; });

        if (scheduler._tooltip) {
            if (scheduler._tooltip.date == mark) return;
            scheduler._tooltip.innerHTML = "";
        } else {
            var t = scheduler._tooltip = document.createElement("DIV");
            t.className = "dhx_year_tooltip";
            document.body.appendChild(t);
            t.onclick = scheduler._click.dhx_cal_data;
        }

        var html = "";

        for (var i = 0; i < evs.length; i++) {
            var bg_color = (evs[i].color ? ("background-color:" + evs[i].color + ";") : "");
            var color = (evs[i].textColor ? ("color:" + evs[i].textColor + ";") : "");
            html += "<div class='dhx_tooltip_line' event_id='" + evs[i].id + "' style='" + bg_color + "" + color + "'>";
            html += "<div class='dhx_tooltip_date'>" + (evs[i]._timed ? scheduler.templates.event_date(evs[i].start_date) : "") + "</div>";
            html += "<div class='dhx_event_icon icon_details'>&nbsp;</div>";
            html += scheduler.templates[obj.name + "_tooltip"](evs[i].start_date, evs[i].end_date, evs[i]) + "</div>";
        }

        scheduler._tooltip.style.display = "";
        scheduler._tooltip.style.top = "0px";

        if (document.body.offsetWidth - offset.left - scheduler._tooltip.offsetWidth < 0)
            scheduler._tooltip.style.left = offset.left - scheduler._tooltip.offsetWidth + "px";
        else
            scheduler._tooltip.style.left = offset.left + pos.src.offsetWidth + "px";

        scheduler._tooltip.date = mark;
        scheduler._tooltip.innerHTML = html;

        if (document.body.offsetHeight - offset.top - scheduler._tooltip.offsetHeight < 0)
            scheduler._tooltip.style.top = offset.top - scheduler._tooltip.offsetHeight + pos.src.offsetHeight + "px";
        else
            scheduler._tooltip.style.top = offset.top + "px";
    }

    scheduler._matrix_tooltip_handler = function (e) {
        var obj = scheduler.matrix[scheduler._mode];
        if (!obj || obj.render != "cell")
            return;
        if (obj) {
            var pos = scheduler._locate_cell_timeline(e);
            var e = e || event;
            var src = e.target || e.srcElement;
            if (pos)
                return showToolTip(obj, pos, getOffset(pos.src));
        }
        hideToolTip();
    };
    scheduler._init_matrix_tooltip = function () {
        scheduler._detachDomEvent(scheduler._els["dhx_cal_data"][0], "mouseover", scheduler._matrix_tooltip_handler);
        dhtmlxEvent(scheduler._els["dhx_cal_data"][0], "mouseover", scheduler._matrix_tooltip_handler);
    };

    scheduler._renderMatrix = function (mode, refresh) {
        if (!refresh)
            scheduler._els['dhx_cal_data'][0].scrollTop = 0;

        scheduler._min_date = scheduler.date[this.name + "_start"](scheduler._date);
        scheduler._max_date = scheduler.date.add(scheduler._min_date, this.x_size * this.x_step, this.x_unit);
        scheduler._table_view = true;
        if (this.second_scale) {
            if (mode && !this._header_resized) {
                this._header_resized = scheduler.xy.scale_height;
                scheduler.xy.scale_height *= 2;
                scheduler._els['dhx_cal_header'][0].className += " dhx_second_cal_header";
            }
            if (!mode && this._header_resized) {
                scheduler.xy.scale_height /= 2;
                this._header_resized = false;
                var header = scheduler._els['dhx_cal_header'][0];
                header.className = header.className.replace(/ dhx_second_cal_header/gi, "");
            }
        }
        set_full_view.call(this, mode);
    };

    function html_index(el) {
        var p = el.parentNode.childNodes;
        for (var i = 0; i < p.length; i++)
            if (p[i] == el) return i;
        return -1;
    }
    function locate_hcell(e) {
        e = e || event;
        var trg = e.target ? e.target : e.srcElement;
        while (trg && trg.tagName != "DIV")
            trg = trg.parentNode;
        if (trg && trg.tagName == "DIV") {
            var cs = trg.className.split(" ")[0];
            if (cs == "dhx_scale_bar")
                return { x: html_index(trg), y: -1, src: trg, scale: true };
        }
    }
    scheduler._locate_cell_timeline = function (e) {
        e = e || event;
        var trg = e.target ? e.target : e.srcElement;

        var res = {};
        var view = scheduler.matrix[scheduler._mode];
        var pos = scheduler.getActionData(e);

        for (var xind = 0; xind < view._trace_x.length - 1; xind++) {
            // | 8:00, 8:30 | 8:15 should be checked against 8:30
            // clicking at the most left part of the cell, say 8:30 should create event in that cell, not previous one
            if (+pos.date < view._trace_x[xind + 1])
                break;
        }

        res.x = xind;
        res.y = view.order[pos.section];
        var diff = scheduler._isRender('cell') ? 1 : 0;
        res.src = view._scales[pos.section] ? view._scales[pos.section].getElementsByTagName('td')[xind + diff] : null;

        var isScale = false;
        while (res.x === 0 && trg.className != "dhx_cal_data" && trg.parentNode) {
            if (trg.className.split(" ")[0] == "dhx_matrix_scell") {
                isScale = true;
                break;
            } else {
                trg = trg.parentNode;
            }
        }
        if (isScale) { // Y scale
            res.x = -1;
            res.src = trg;
            res.scale = true;
        }

        return res;
    };

    var old_click = scheduler._click.dhx_cal_data;
    scheduler._click.dhx_marked_timespan = scheduler._click.dhx_cal_data = function (e) {
        var ret = old_click.apply(this, arguments);
        var obj = scheduler.matrix[scheduler._mode];
        if (obj) {
            var pos = scheduler._locate_cell_timeline(e);
            if (pos) {
                if (pos.scale)
                    scheduler.callEvent("onYScaleClick", [pos.y, obj.y_unit[pos.y], e || event]);
                else
                    scheduler.callEvent("onCellClick", [pos.x, pos.y, obj._trace_x[pos.x], (((obj._matrix[pos.y] || {})[pos.x]) || []), e || event]);
            }
        }
        return ret;
    };

    scheduler.dblclick_dhx_matrix_cell = function (e) {
        var obj = scheduler.matrix[scheduler._mode];
        if (obj) {
            var pos = scheduler._locate_cell_timeline(e);
            if (pos) {
                if (pos.scale)
                    scheduler.callEvent("onYScaleDblClick", [pos.y, obj.y_unit[pos.y], e || event]);
                else
                    scheduler.callEvent("onCellDblClick", [pos.x, pos.y, obj._trace_x[pos.x], (((obj._matrix[pos.y] || {})[pos.x]) || []), e || event]);
            }
        }
    };

    var old_dblclick_marked_timespan = scheduler.dblclick_dhx_marked_timespan || function () { };
    scheduler.dblclick_dhx_marked_timespan = function (e) {
        var obj = scheduler.matrix[scheduler._mode];
        if (obj)
            return scheduler.dblclick_dhx_matrix_cell(e);
        else
            return old_dblclick_marked_timespan.apply(this, arguments);
    };

    scheduler.dblclick_dhx_matrix_scell = function (e) {
        return scheduler.dblclick_dhx_matrix_cell(e);
    };

    scheduler._isRender = function (mode) {
        return (scheduler.matrix[scheduler._mode] && scheduler.matrix[scheduler._mode].render == mode);
    };

    scheduler.attachEvent("onCellDblClick", function (x, y, a, b, event) {
        if (this.config.readonly || (event.type == "dblclick" && !this.config.dblclick_create)) return;

        var obj = scheduler.matrix[scheduler._mode];
        var event_options = {};
        event_options.start_date = obj._trace_x[x];
        event_options.end_date = (obj._trace_x[x + 1]) ? obj._trace_x[x + 1] : scheduler.date.add(obj._trace_x[x], obj.x_step, obj.x_unit);

        if (obj._start_correction)
            event_options.start_date = new Date(event_options.start_date * 1 + obj._start_correction);
        if (obj._end_correction)
            event_options.end_date = new Date(event_options.end_date - obj._end_correction);

        event_options[obj.y_property] = obj.y_unit[y].key;
        scheduler.addEventNow(event_options, null, event);
    });

    scheduler.attachEvent("onBeforeDrag", function (event_id, mode, native_event_object) {
        return !scheduler._isRender("cell");
    });
    scheduler.attachEvent("onEventChanged", function (id, ev) {
        ev._timed = this.isOneDayEvent(ev);
    });


    scheduler._is_column_visible = function (date) {
        var mode = scheduler.matrix[scheduler._mode];
        var start_ind = scheduler._get_date_index(mode, date);
        return !scheduler._ignores[start_ind];
    };
    var old_render_marked_timespan = scheduler._render_marked_timespan;
    scheduler._render_marked_timespan = function (options, area, unit_id, min_date, max_date) {
        if (!scheduler.config.display_marked_timespans)
            return [];

        if (scheduler.matrix && scheduler.matrix[scheduler._mode]) {
            if (scheduler._isRender('cell'))
                return;

            var view_opts = scheduler._lame_copy({}, scheduler.matrix[scheduler._mode]);
            //timespans must always use actual position, not rounded
            view_opts.round_position = false;
            var blocks = [];

            var units = [];
            var areas = [];
            var section = options.sections ? (options.sections.units || options.sections.timeline) : null;
            if (!unit_id) {  // should draw for every unit...
                var order = view_opts.order;
                if (section) { // ...or for only section if mentioned in configuration of timespan
                    if (order.hasOwnProperty(section)) {
                        units.push(section);
                        areas.push(view_opts._scales[section]);
                    }
                } else {
                    for (var key in order) {
                        if (order.hasOwnProperty(key)) {
                            units.push(key);
                            areas.push(view_opts._scales[key]);
                        }
                    }
                }
            } else {
                areas = [area];
                units = [unit_id];
            }

            var min_date = min_date ? new Date(min_date) : scheduler._min_date;
            var max_date = max_date ? new Date(max_date) : scheduler._max_date;

            if (min_date.valueOf() < scheduler._min_date.valueOf())
                min_date = new Date(scheduler._min_date);
            if (max_date.valueOf() > scheduler._max_date.valueOf())
                max_date = new Date(scheduler._max_date);

            if (!scheduler._is_column_visible(min_date))
                return;

            var dates = [];

            if (options.days > 6) {
                var specific_date = new Date(options.days);
                if (scheduler.date.date_part(new Date(min_date)) <= +specific_date && +max_date >= +specific_date)
                    dates.push(specific_date);
            } else {
                dates.push.apply(dates, scheduler._get_dates_by_index(options.days));
            }

            var zones = options.zones;
            var css_classes = scheduler._get_css_classes_by_config(options);

            for (var j = 0; j < units.length; j++) {
                area = areas[j];
                unit_id = units[j];

                for (var i = 0; i < dates.length; i++) {
                    var date = dates[i];
                    for (var k = 0; k < zones.length; k += 2) {
                        var zone_start = zones[k];
                        var zone_end = zones[k + 1];
                        var start_date = new Date(+date + zone_start * 60 * 1000);
                        var end_date = new Date(+date + zone_end * 60 * 1000);

                        if (!(min_date < end_date && max_date > start_date))
                            continue;

                        var block = scheduler._get_block_by_config(options);
                        block.className = css_classes;

                        var start_pos = _getX({ start_date: start_date }, false, view_opts) - 1;
                        var end_pos = _getX({ start_date: end_date }, false, view_opts) - 1;
                        var width = Math.max(1, end_pos - start_pos - 1);
                        var height = view_opts._section_height[unit_id] - 1;

                        block.style.cssText = "height: " + height + "px; left: " + start_pos + "px; width: " + width + "px; top: 0;";

                        if (area) {
                            area.insertBefore(block, area.firstChild);
                        }
                        blocks.push(block);
                    }
                }
            }

            return blocks;

        } else {
            return old_render_marked_timespan.apply(scheduler, [options, area, unit_id]);
        }
    };

    var old_append_mark_now = scheduler._append_mark_now;
    scheduler._append_mark_now = function (day_index, now) {
        if (scheduler.matrix && scheduler.matrix[scheduler._mode]) {
            var n_date = scheduler._currentDate();
            var zone_start = scheduler._get_zone_minutes(n_date);
            var options = {
                days: +scheduler.date.date_part(n_date),
                zones: [zone_start, zone_start + 1],
                css: "dhx_matrix_now_time",
                type: "dhx_now_time"
            };
            return scheduler._render_marked_timespan(options);
        } else {
            return old_append_mark_now.apply(scheduler, [day_index, now]);
        }
    };

    scheduler.attachEvent("onScaleAdd", function (scale, unit_key) {
        var timespans = scheduler._marked_timespans;

        if (timespans && scheduler.matrix && scheduler.matrix[scheduler._mode]) {
            var mode = scheduler._mode;

            var min_date = scheduler._min_date;
            var max_date = scheduler._max_date;
            var global_data = timespans["global"];

            for (var t_date = scheduler.date.date_part(new Date(min_date)) ; t_date < max_date; t_date = scheduler.date.add(t_date, 1, "day")) {
                var day_value = +t_date;
                var day_index = t_date.getDay();
                var r_configs = [];

                var day_types = global_data[day_value] || global_data[day_index];
                r_configs.push.apply(r_configs, scheduler._get_configs_to_render(day_types));

                if (timespans[mode] && timespans[mode][unit_key]) {
                    var z_config = [];
                    var unit_types = scheduler._get_types_to_render(timespans[mode][unit_key][day_index], timespans[mode][unit_key][day_value]);
                    z_config.push.apply(z_config, scheduler._get_configs_to_render(unit_types));
                    if (z_config.length)
                        r_configs = z_config;
                }

                for (var i = 0; i < r_configs.length; i++) {
                    var config = r_configs[i];
                    var day = config.days;
                    if (day < 7) {
                        day = day_value;
                        //specify min/max timespan dates, otherwise it can be rendered multiple times in some configurations
                        scheduler._render_marked_timespan(config, scale, unit_key, t_date, scheduler.date.add(t_date, 1, "day"));
                        day = day_index;
                    } else {
                        scheduler._render_marked_timespan(config, scale, unit_key, t_date, scheduler.date.add(t_date, 1, "day"));
                    }
                }
            }
        }
    });

    scheduler._resolve_timeline_section = function (view, pos) {
        var yind = 0;
        var summ = 0;
        for (yind; yind < this._colsS.heights.length; yind++) {
            summ += this._colsS.heights[yind];
            if (summ > pos.y)
                break;
        }

        if (!view.y_unit[yind]) {
            yind = view.y_unit.length - 1;
        }
        if (this._drag_event && !this._drag_event._orig_section) {
            this._drag_event._orig_section = view.y_unit[yind].key;
        }

        pos.fields = {};
        if (yind >= 0 && view.y_unit[yind]) {
            pos.section = pos.fields[view.y_property] = view.y_unit[yind].key;
        }
    };
    scheduler._update_timeline_section = function (action) {
        var view = action.view,
            event = action.event,
            pos = action.pos;

        if (event) {
            if (event[view.y_property] != pos.section) {
                var line_height = this._get_timeline_event_height(event, view);
                event._sorder = this._get_dnd_order(event._sorder, line_height, view._section_height[pos.section]);
            }
            event[view.y_property] = pos.section;
        }
    };
    scheduler._get_date_index = function (config, date) {
        var index = 0;
        var trace_x = config._trace_x;
        while (index < trace_x.length - 1 && +date >= +trace_x[index + 1]) {
            index++;
        }
        return index;
    };

    scheduler.attachEvent("onBeforeTodayDisplayed", function () {
        for (var i in scheduler.matrix) {
            var obj = scheduler.matrix[i];
            obj.x_start = obj._original_x_start;
        }
        return true;
    });

    scheduler.attachEvent("onOptionsLoad", function () {
        for (var i in scheduler.matrix) {
            var obj = scheduler.matrix[i];

            obj.order = {};
            scheduler.callEvent('onOptionsLoadStart', []);
            for (var i = 0; i < obj.y_unit.length; i++)
                obj.order[obj.y_unit[i].key] = i;
            scheduler.callEvent('onOptionsLoadFinal', []);
            if (scheduler._date && obj.name == scheduler._mode)
                scheduler.setCurrentView(scheduler._date, scheduler._mode);
        }
    });

    scheduler.attachEvent("onSchedulerResize", function () {
        if (scheduler.matrix[this._mode]) {
            var obj = scheduler.matrix[this._mode];
            scheduler._renderMatrix.call(obj, true, true);
            return false;
        }
        return true;
    });

    scheduler.attachEvent("onBeforeDrag", function (id, drag_mode, e) {
        if (drag_mode == 'resize') {
            var trg = e.target || e.srcElement;
            if ((trg.className || "").indexOf("dhx_event_resize_end") < 0) {
                scheduler._drag_from_start = true;
            } else {
                scheduler._drag_from_start = false;
            }
        }

        return true;
    });

};
scheduler._temp_matrix_scope();

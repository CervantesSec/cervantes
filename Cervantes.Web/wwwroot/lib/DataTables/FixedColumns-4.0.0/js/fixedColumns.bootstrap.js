/*! FixedColumns 4.0.0
 * 2019-2021 SpryMedia Ltd - datatables.net/license
 */
(function () {
    'use strict';

    (function (factory) {
        if (typeof define === 'function' && define.amd) {
            // AMD
            define(['jquery', 'datatables.net-bs', 'datatables.net-fixedcolumns'], function ($) {
                return factory($);
            });
        }
        else if (typeof exports === 'object') {
            // CommonJS
            module.exports = function (root, $) {
                if (!root) {
                    root = window;
                }
                if (!$ || !$.fn.dataTable) {
                    // eslint-disable-next-line @typescript-eslint/no-var-requires
                    $ = require('datatables.net-bs')(root, $).$;
                }
                if (!$.fn.dataTable.SearchPanes) {
                    // eslint-disable-next-line @typescript-eslint/no-var-requires
                    require('datatables.net-fixedcolumns')(root, $);
                }
                return factory($);
            };
        }
        else {
            // Browser
            factory(jQuery);
        }
    }(function ($) {
        var dataTable = $.fn.dataTable;
        return dataTable.fixedColumns;
    }));

}());

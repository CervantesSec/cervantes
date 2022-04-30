/*! SearchBuilder 1.2.2
 * ©SpryMedia Ltd - datatables.net/license/mit
 */
(function () {
    'use strict';

    /*! semantic ui integration for DataTables' SearchBuilder
     * ©2016 SpryMedia Ltd - datatables.net/license
     */
    (function (factory) {
        if (typeof define === 'function' && define.amd) {
            // AMD
            define(['jquery', 'datatables.net-se', 'datatables.net-searchbuilder'], function ($) {
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
                    $ = require('datatables.net-se')(root, $).$;
                }
                if (!$.fn.dataTable.searchBuilder) {
                    // eslint-disable-next-line @typescript-eslint/no-var-requires
                    require('datatables.net-searchbuilder')(root, $);
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
        $.extend(true, dataTable.SearchBuilder.classes, {
            clearAll: 'ui button dtsb-clearAll'
        });
        $.extend(true, dataTable.Group.classes, {
            add: 'ui button dtsb-add',
            clearGroup: 'ui button dtsb-clearGroup',
            logic: 'ui button dtsb-logic'
        });
        $.extend(true, dataTable.Criteria.classes, {
            condition: 'ui selection dropdown dtsb-condition',
            data: 'ui selection dropdown dtsb-data',
            "delete": 'ui button dtsb-delete',
            left: 'ui button dtsb-left',
            right: 'ui button dtsb-right',
            value: 'ui selection dropdown dtsb-value'
        });
        dataTable.ext.buttons.searchBuilder.action = function (e, dt, node, config) {
            e.stopPropagation();
            this.popover(config._searchBuilder.getNode(), {
                align: 'dt-container'
            });
            // Need to redraw the contents to calculate the correct positions for the elements
            if (config._searchBuilder.s.topGroup !== undefined) {
                config._searchBuilder.s.topGroup.dom.container.trigger('dtsb-redrawContents');
            }
            $('div.dtsb-searchBuilder').removeClass('ui basic vertical buttons');
        };
        return dataTable.searchPanes;
    }));

}());

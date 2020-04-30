/**
 * @version 1.0
 * @namespace Modules
 * @class AngularModule
 */
define(
    ['ui.router',
     'angular-animate',
     'angular-material',
     'angular-messages',
     'angular-aria',
     'angular-cookies',
     'mdPickers',
     'ui.bootstrap',
     'angular-material-data-table',
     'angular-material-fixed-table-header',
     'lodash',
     'Singleton',
     'Configuration',
     'UserService',
     'FetchService',
     'MainController',
     'AngularProvider',
     'SiteHttpInterceptor',
     'PingService',
     'UtcToLocalFilter',
     'uicalendar'
    ],
    function (router, ngAnimate, ngMaterial, ngMessages, ngAria, ngCookies, mdPickers, uibootstrap, ngDataTable, ngDataTableHeader, lodash, singleton, configuration, userService, fetchService, mainController, angularProvider, siteHttpInterceptor, pingService, utcToLocalFilter, uicalendar) {
        'use strict';

        //Configure the main angular module to establish routes
        angularProvider.config('PROD', ['$controllerProvider', '$provide', '$compileProvider', '$filterProvider', '$stateProvider', '$urlRouterProvider', '$locationProvider', '$mdThemingProvider', '$httpProvider',
        function ($controllerProvider, $provide, $compileProvider, $filterProvider, $stateProvider, $urlRouteProvider, $locationProvider, $mdThemingProvider, $httpProvider) {

            angularProvider.setProviders($controllerProvider, $provide, $compileProvider, $filterProvider);
            angularProvider.registerService('Singleton', singleton);
            angularProvider.registerService('Configuration', configuration);
            angularProvider.registerService('UserService', userService);
            angularProvider.registerService('FetchService', fetchService);

            angularProvider.registerService('siteHttpInterceptor', siteHttpInterceptor);
            angularProvider.registerService('PingService', pingService);
            angularProvider.registerController('MainController', mainController);
            angularProvider.registerFilter('utcToLocalFilter', utcToLocalFilter);

            $mdThemingProvider.theme('default').primaryPalette('blue-grey').accentPalette('teal');
            $httpProvider.interceptors.push('siteHttpInterceptor');

            $urlRouteProvider.otherwise('/');
            $stateProvider.state('home', {
                url: '/'
            })
            .state('dashboard', controllerState('DashBoard', 'DashBoard', 'DashBoardController'))
            .state('orderRelease', controllerState('OrderRelease', 'OrderRelease', 'OrderReleaseController'))
            .state('orderRelease.search', controllerState('Search', 'OrderRelease/ReleaseSearch', 'OrderReleaseSearchController'))
            //Remove after work for scheduled requests is complete
            .state('orderRelease.detail', controllerState('Detail/:status/:orderId', 'OrderRelease/ReleaseDetail', 'OrderReleaseDetailController'))
            .state('orderRelease.jobErrorsSearch', controllerState('JobErrorsSearch', 'OrderRelease/JobErrorsSearch', 'JobErrorsSearchController'))
            .state('orderRelease.scheduleRequests', controllerState('ScheduleRequests', 'ScheduleRequests', 'ScheduleRequestsController'))
            .state('orderRelease.requestPrioritization', controllerState('RequestPrioritization', 'RequestPrioritization', 'RequestPrioritizationController'))
            .state('orderRelease.requestHistory', controllerState('History', 'RequestPrioritization/History', 'RequestPrioritizationController'))
            .state('approval', controllerState('Approval', 'Approval', 'ApprovalController'))
            .state('approval.job', controllerState('Job', 'Approval/JobApproval', 'JobApprovalController'))
            .state('approval.jobv2', controllerState('JobV2', 'Approval/JobApprovalV2', 'JobApprovalControllerV2'))

            .state('accountreceivable', controllerState('AccountReceivable', 'AccountReceivable', 'AccountReceivableController'))
            .state('accountreceivable.generateattinvoice', controllerState('GenerateATTInvoice', 'AccountReceivable/ARGenerateATTInvoice', 'ARGenerateATTInvoiceController'))

            .state('accountreceivable.invoicesreadyforsubmission', controllerState('InvoicesReadyForSubmission', 'AccountReceivable/ARInvoicesReadyForSubmission', 'ARInvoicesReadyForSubmissionController'))
            .state('accountreceivable.invoicesreadyfortransmission', controllerState('InvoicesReadyForTransmission', 'AccountReceivable/ARInvoicesReadyForTransmission', 'ARInvoicesReadyForTransmissionController'))

            .state('accountreceivable.completedinvoices', controllerState('CompletedInvoices', 'AccountReceivable/ARCompletedInvoices', 'ARCompletedInvoicesController'))
            .state('accountreceivable.specialinvoicing', controllerState('SpecialInvoicing', 'AccountReceivable/ARSpecialInvoicing', 'ARSpecialInvoicingController'))
            .state('accountreceivable.reports', controllerState('Reports', 'AccountReceivable/ARReports', 'ARReportsController'))

            .state('ap', controllerState('AccountPayable', 'AccountPayable', 'ReceiveVendorInvoiceController'))
            .state('ap.rvi', controllerState('ReceiveVendorInvoice/:edit/:poNumber/:invoiceId/:vendorCode', 'AccountPayable/ReceiveVendorInvoice', 'ReceiveVendorInvoiceController'))
            .state('ap.taf', controllerState('TransmitAPFile', 'AccountPayable/TransmitAPFile', 'TransmitOrResetAPController'))
            .state('invoice', controllerState('Invoice', 'Invoice', 'MaterialInvoiceController'))
            .state('invoice.matInvoices', controllerState('Invoice', 'Invoice/MaterialInvoice','MaterialInvoiceController'))
            .state('invoice.labInvoices', controllerState('Invoice', 'Invoice/LaborInvoice', 'LaborInvoiceController'))
            .state('ad', controllerState('Adjustments', 'Adjustments', 'AdjustmentsController'))
            .state('ad.rep', controllerState('AdjustmentsReport', 'Adjustments/AdjustmentsReport', 'AdjustmentsController'))
            .state('ad.rtn', controllerState('Returns', 'Adjustments/Returns', 'AdjustmentsController'))
            .state('mr', controllerState('MaterialRequest', 'MaterialRequest', 'MRFReturnsController'))
            .state('mr.mrf', controllerState('MaterialRequestForm', 'MaterialRequest/MaterialRequestForm', 'MaterialRequestController'))
            .state('mr.mra', controllerState('MRFApproval', 'MaterialRequest/MRFApproval', 'MRFApprovalController'))
            .state('mr.mrr', controllerState('MRFReturns', 'MaterialRequest/MRFReturns', 'MRFReturnsController'))
            .state('erf', controllerState('Erf', 'Erf', 'ErfController'))
            .state('rma', controllerState('Rma', 'Rma', 'RmaController'))
            .state('error', controllerState('Error', 'Error', 'ErrorController'))
            .state('purchaseOrder', controllerState('PurchaseOrder', 'PurchaseOrder', 'PurchaseOrderController'))
            .state('purchaseOrder.nonScopedItems', controllerState('NonScopedItems', 'PurchaseOrder/NonScopedItems', 'NonScopedItemsController'))
            .state('purchaseOrder.vendorAssignment', controllerState('VendorAssignment', 'PurchaseOrder/VendorAssignment', 'VendorAssignmentController'))
            .state('purchaseOrder.list', controllerState('PurchaseOrders', 'PurchaseOrder/PurchaseOrders', 'PurchaseOrdersController'))
            .state('purchaseOrder.cancelledPOHistory', controllerState('CancelledPOHistory', 'PurchaseOrder/CancelledPOHistory', 'CancelledPOHistoryController'))
            .state('purchaseOrder.rejectedPORevision', controllerState('RejectedPORevision', 'PurchaseOrder/RejectedPORevision', 'RejectedPORevisionController'))
            .state('approval.poRevisionApproval', controllerState('PORevisionApproval', 'Approval/PORevisionApproval', 'PORevisionApprovalController'))
            .state('approval.nonScopedApproval', controllerState('NonScopedApproval', 'Approval/NonScopedApproval', 'NonScopedApprovalController'))
            .state('tran', controllerState('TransferWarehouse', 'TransferWarehouse', 'TransferWarehouseController'))
            .state('tran.new', controllerState('TransferNew', 'TransferWarehouse/TransferNew', 'TransferWarehouseController'))
            .state('tran.his', controllerState('TransferHistory', 'TransferWarehouse/TransferHistory', 'TransferWarehouseController'))
            .state('administration', controllerState('Administration', 'Administration', 'AdministrationController'))
            .state('administration.configuration', controllerState('Configuration', 'Administration/Configuration', 'ConfigurationController'))
            .state('administration.useradministration', controllerState('UserAdministration', 'Administration/UserAdministration', 'AdministrationController'))
            .state('administration.webservicechecker', controllerState('WebServiceChecker', 'Administration/WebServiceChecker', 'WebServiceCheckerController'))
        }]);

        var controllerState = function (url, template, controller) {
            return {
                url: '/' + url,
                templateUrl: template,
                controller: controller,
                resolve: ['$q', function ($q) {
                    var deferred = $q.defer();
                    require([controller], function () {
                        deferred.resolve();
                    });
                    return deferred.promise;
                }]
            }
        }

        //bootstrap the angular framework
        if (document.readyState === 'interactive' || document.readyState === 'complete') {
            angular.bootstrap(document.documentElement, [angularProvider.getModule().name]);
        } else {
            document.onreadystatechange = function () {
                if (document.readyState === 'interactive') {
                    angular.bootstrap(document.documentElement, ['AngularModule']);
                }
            };
        }
    });
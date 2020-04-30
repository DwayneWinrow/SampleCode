define(['AngularProvider', 'Constants', 'DialogService', 'TabService', 'AccountPayableService'],
    function (angularProvider, Constants) {
        /**
         * @version 1.0
         * @namespace Controllers
         * @class ApprovalController
         */
        angularProvider.registerController('TransmitOrResetAPController', ['$scope', 'orderByFilter', '$state', '$http', '$mdMedia', 'DialogService', '$q', 'TabService', 'AccountPayableService', '$timeout',
            function ($scope, orderBy, $state, $http, $mdMedia, DialogService, $q, tabService, AccountPayableService, $timeout) {
                $scope.TabService = tabService;
                $scope.setIcon(Constants.Icons.Financial)
                $scope.selectedReady2Transmit = [];
                $scope.selectedTransmitted = [];
                $scope.transmittedAP = false;
                $scope.PACEJobTasksResults = new Object();
         
                $scope.transmittedResults = new Object();
                $scope.ready2TransmittedResults = new Object();
                $scope.VendorsResults = new Object();
                $scope.POResults = new Object();
                $scope.POsByVendorResults = new Object();
                $scope.polineItemResults = new Object();
                $scope.PublishInvoice = new Object();
                $scope.transmitInvoices = new Object();
                $scope.PublishInvoice.poNumber = null;
                $scope.TransmittedReport = new Object();
                $scope.poLineItemTotal = null;
                $scope.invoiceNumberSearch0 = "";
                $scope.invoiceNumberSearch1 = "";
                $scope.APUserResults = null;
                $scope.TaskId = null;
                var errorCount = 0; //Counter for the server errors
                var loadPromise;
                var loadTime = 10000;
                $scope.APuserIdT = null;
                $scope.APuserIdR = null;

                $scope.resetScreen = function () {
                    $scope.selectedReady2Transmit = [];
                    $scope.selectedTransmitted = [];
                    $scope.invoiceNumberSearch0 = null;
                    $scope.invoiceNumberSearch1 = null;
                }
                //sort columns
                $scope.sortColumnR = 'invoiceNumber';
                $scope.reverseR = true;
                $scope.ready2TransmittedResults = null;
                $scope.sortColumnT = 'invoiceNumber';
                $scope.reverseT = true;
                $scope.transmittedResults = null;
                //sort columns
                $scope.ready2TransmittedAPSearch = function (transmitted) {
                    $scope.displaySpinner();
                    $scope.resetScreen();
                    var promise = AccountPayableService.ready2TransmittedAP(transmitted);
                    promise.then(function (data) {
                        $scope.ready2TransmittedResults = null;
                        $scope.ready2TransmittedResults = data;
                        $scope.ready2TransmittedResults = orderBy($scope.ready2TransmittedResults, $scope.sortColumnR, $scope.reverseR);
                        $scope.hideSpinner();
                    });
                }
                $scope.transmittedAPSearch = function (transmitted) {
                    $scope.displaySpinner();
                    $scope.resetScreen();
                    var promise = AccountPayableService.transmittedAP(transmitted);
                    promise.then(function (data) {
                        $scope.transmittedResults = null;
                        $scope.transmittedResults = data;
                        $scope.transmittedResults = orderBy($scope.transmittedResults, $scope.sortColumnT, $scope.reverseT);
                        $scope.hideSpinner();
                    });
                }
                $scope.getAPUsers = function () {
                    $scope.displaySpinner();
                    $scope.APuserId = null;
                    var promise = AccountPayableService.getAPUsers();
                    promise.then(function (data) {
                        $scope.APUserResults = null;
                        $scope.APUserResults = data;
                        $scope.hideSpinner();
                    });
                }
                $scope.Ready2TransmittedByInvoiceNumberAP = function () {
                    $scope.displaySpinner();
                    $scope.selectedReady2Transmit = [];
                    $scope.selectedTransmitted = [];
                    var APObject = {
                        invoiceNumber: $scope.invoiceNumberSearch0,
                        userId: $scope.APuserIdR,
                        transmitted: false
                    }
                    var promise = AccountPayableService.transmittedByInvoiceNumberAP(APObject);
                    promise.then(function (data) {
                        $scope.ready2TransmittedResults = data;
                        $scope.hideSpinner();
                    });
                    $scope.hideSpinner();
                }
                $scope.TransmittedByInvoiceNumberAP = function () {
                    $scope.displaySpinner();
                    $scope.selectedReady2Transmit = [];
                    $scope.selectedTransmitted = [];
                    var APObject = {
                        invoiceNumber: $scope.invoiceNumberSearch1,
                        userId: $scope.APuserIdT,
                        transmitted: true
                    }
                    var promise = AccountPayableService.transmittedByInvoiceNumberAP(APObject);
                    promise.then(function (data) {
                        $scope.transmittedResults = data;
                        $scope.hideSpinner();
                    });
                    $scope.hideSpinner();
                }
                
                $scope.launchTransReport = function () {
                    $scope.TransmittedReport.vendorId = null;
                    $scope.TransmittedReport.poNumber = null;
                    $scope.TransmittedReport.beginDate = null;
                    $scope.TransmittedReport.endDate = null;
                    $scope.PDFAPReport = null;
                    $scope.displaySpinner();
                    var promiseVendor = AccountPayableService.getVendorsWithPOs();
                    var promisePO = AccountPayableService.getPOwithInvoices();
                    var opsList = [promiseVendor, promisePO];
                    $q.all(opsList).then(function (data) {
                        $scope.VendorsResults = data[0];
                        $scope.POResults = data[1];
                        $scope.hideSpinner();
                        DialogService.displayDialog($scope, null, Constants.Routes.GetInvoiceTransFindDialog);
                    });
                }
                $scope.runTransReport = function () {
                    $scope.PDFAPReport = null;
                    //if($scope.TransmittedReport.beginDate ==null ||$scope.TransmittedReport.endDate == null)
                    //{
                    //    displayActionToastReport(' Dates cannot be empty. Please select a date. ');
                     //   return;
                    //}
                    var promise = AccountPayableService.runTransmittedAPReport($scope.TransmittedReport);
                    promise.then(function (data) {
                        $scope.PDFAPReport = null;
                        file = new Blob([data], {
                            type: 'application/pdf'
                        });
                        fileURL = URL.createObjectURL(file);
                        $scope.PDFAPReport = fileURL;
                    });
                }
                $scope.PreviewAPReport = function () {
                    var file = null;
                    var fileURL = null;
                    $scope.PDFreportContent = null;
                    $scope.transmitInvoices.transmitted = true;
                    $scope.transmitInvoices.Invoices2Transmit = $scope.selectedReady2Transmit;
                    var promise = $scope.IsReceiptReady($scope.selectedReady2Transmit);
                    promise.then(function(result)
                    {
                        if (result == false)
                            return;
                        if (!$scope.hasDuplicates($scope.selectedReady2Transmit)) {
                            $scope.displaySpinner();
                            var promise = AccountPayableService.createAPReport($scope.transmitInvoices);
                            promise.then(function (data) {
                                $scope.PDFreportContent = null;
                                file = new Blob([data], { type: 'application/pdf' });
                                fileURL = URL.createObjectURL(file);
                                $scope.PDFreportContent = fileURL;
                                DialogService.displayDialog($scope, null, Constants.Routes.GetAPReportDialog);
                                $scope.hideSpinner();
                            });
                        }
                    });
                 }
                $scope.GoTransmittedAP = function () {
                    _.forEach($scope.selectedReady2Transmit, function (selectedItem) {
                        _.remove($scope.ready2TransmittedResults, function (readyTwoTransmittedResults) {
                            if (selectedItem.invoiceNumber === readyTwoTransmittedResults.invoiceNumber)
                            {
                                return true;
                            }
                            return false;
                        });
                    });
                    $scope.transmitInvoices.transmitted = true;
                    $scope.transmitInvoices.Invoices2Transmit = $scope.selectedReady2Transmit;
                    //$scope.selectedTransmitted.push(selectedReady2Transmit);
                    var promise = AccountPayableService.resetOrTransmittedAP($scope.transmitInvoices);
                    var transmittedCount = $scope.selectedReady2Transmit.length;
                    //$scope.hideSpinner();
                    displayActionToastTransmit(transmittedCount + ' Invoices successfully transmitted ');
                    $scope.selectedReady2Transmit = [];
                    //$scope.selectedTransmitted.push($scope.selectedReady2Transmit);
                }

                $scope.resetTransmittedAP = function () {
                    //$scope.displaySpinner();
                    _.forEach($scope.selectedTransmitted, function (selectedItem) {
                        _.remove($scope.transmittedResults, function (readyTwoTransmittedResults) {
                            if (selectedItem.invoiceNumber === readyTwoTransmittedResults.invoiceNumber) {
                              
                                return true;
                            }
                            return false;
                        });
                    });

                    $scope.transmitInvoices.transmitted = false;
                    $scope.transmitInvoices.Invoices2Transmit = $scope.selectedTransmitted;
                    //$scope.selectedReady2Transmit.push(selectedTransmitted);
                    var promise = AccountPayableService.resetOrTransmittedAP($scope.transmitInvoices);
                    var transmittedCount = $scope.selectedTransmitted.length;
                    //$scope.hideSpinner();
                    displayActionToastReset(transmittedCount + ' Invoices successfully reset ');
                    $scope.selectedTransmitted = [];
                }
                $scope.deleteInvoice = function (invoicesAP) {
                    var promiseCheck = DialogService.displayConfirmationDialog('Delete Invoice', 'Are you sure you want to delete: ' + invoicesAP.invoiceNumber + ' ?');
                    promiseCheck.then(function (data) {
                        if (data == true) {
                            var promise = AccountPayableService.deleteInvoiceAP(invoicesAP);
                            promise.then(function (data) {
                                _.remove($scope.ready2TransmittedResults, function (readyTwoTransmittedResult) {
                                    if (invoicesAP.invoiceNumber === readyTwoTransmittedResult.invoiceNumber &&
                                        invoicesAP.vendorId === readyTwoTransmittedResult.vendorId) {
                                        return true;
                                    }
                                    return false;
                                });
                                $scope.hideSpinner();
                            });
                         }
                    });                  
                    $scope.hideSpinner();
                }

                $scope.IsReceiptReady = function (select2Go) {
                   return $q(function (resolve, reject) {
                       $scope.displaySpinner();
                       var POHeaderDTOs = [];
                        for (i = 0; i < select2Go.length; i++) {
                            POHeaderDTOs[i] = select2Go[i].poNumber
                        }
                        
                        var promise = AccountPayableService.doesPoLineItemHaveReceipt(POHeaderDTOs);
                        promise.then(function (data) {
                            $scope.TaskId = data;
                            getData();
                        });
                        $scope.hideSpinner();
                    });
                }

                var getData = function () {

                    var url = 'api/GetStatus/' + $scope.TaskId;

                    $http.get(url)
                        .then(function (res) {
                            $scope.data = res;

                        switch (res.data) {
                            case "Processing":
                                errorCount = 0;
                                nextLoad(10000);
                                break;

                            case "Complete":
                                errorCount = 0;
                                getResults();
                                break;
                        }

                    })

                    .catch(function (res) {
                        $scope.data = 'Server error';
                        nextLoad(++errorCount * 2 * loadTime);
                    });
                };

                var cancelNextLoad = function () {
                    $timeout.cancel(loadPromise);
                };

                var nextLoad = function (mill) {
                    mill = mill || loadTime;

                    //Always make sure the last timeout is cleared before starting a new one
                    cancelNextLoad();
                    loadPromise = $timeout(getData, mill);
                };

                var getResults = function () {
                    cancelNextLoad();
                    var url = 'api/GetDynamicsResult/' + $scope.TaskId;
                    $http.get(url)
                        .then(function (res) {
                            if (res.data === 'TRUE') {
                                if (!$scope.hasDuplicates($scope.selectedReady2Transmit)) {
                                    $scope.displaySpinner();
                                    var promise = AccountPayableService.createAPReport($scope.transmitInvoices);
                                    promise.then(function (data) {
                                        $scope.PDFreportContent = null;
                                        file = new Blob([data], { type: 'application/pdf' });
                                        fileURL = URL.createObjectURL(file);
                                        $scope.PDFreportContent = fileURL;
                                        DialogService.displayDialog($scope, null, Constants.Routes.GetAPReportDialog);
                                        $scope.hideSpinner();
                                    });
                                }
                            } else {
                                $scope.hideSpinner
                                DialogService.displayAlertDialog('Receipt(s) is (are) not Ready', 'Please remove PONumber(s) ' + res.data + '. It is (they are) NOT ready to be processed.');
                                return $q.reject(false);
                            }
                        });
                    $scope.hideSpinner();
                };

                $scope.hasDuplicates = function (select2Go) {
                    var valuesPoNumSoFar = [];
                    for (i = 0; i < select2Go.length; i++) {
                        var poNum = select2Go[i].poNumber;
                        if (valuesPoNumSoFar.indexOf(poNum) !== -1) {
                            DialogService.displayAlertDialog('PoNumber ' + poNum + ' is already invoiced in this batch.');
                            //displayActionToastTransmit('PoNumber ' + poNum + ' is already invoiced in this batch.');
                            return true;
                        }
                        valuesPoNumSoFar.push(poNum);
                    }
                    return false;
                }
                $scope.searchKeyPress0 = function () {
                    if (event.keyCode == 13) {
                        $scope.Ready2TransmittedByInvoiceNumberAP($scope.invoiceNumberSearch0);
                        return false;
                    }
                    return true;
                }
                $scope.reorderItemsR = function (sortColumn) {
                    $scope.reverseR = (sortColumn !== null && $scope.sortColumnR === sortColumn)
                        ? !$scope.reverseR : false;
                    $scope.sortColumnR = sortColumn;
                    $scope.ready2TransmittedResults = orderBy($scope.ready2TransmittedResults, $scope.sortColumnR, $scope.reverseR);
                    //$scope.ready2TransmittedResults.sort();
                }
                $scope.reorderItemsT = function (sortColumn) {
                    $scope.reverseT = (sortColumn !== null && $scope.sortColumnT === sortColumn)
                        ? !$scope.reverseT : false;
                    $scope.sortColumnT = sortColumn;
                    $scope.transmittedResults = orderBy($scope.transmittedResults, $scope.sortColumnT, $scope.reverseT);
                    //$scope.transmittedResults.sort();
                }
                var displayActionToastReport = function (message) {
                    if ($mdMedia('xs') == true) {
                        $scope.displayToast(message, '#Vendors');
                    }
                    else {
                        $scope.displayToast(message, '#Vendors');
                    }
                }
                $scope.searchKeyPressR = function () {
                    if (event.keyCode == 13) {
                        $scope.Ready2TransmittedByInvoiceNumberAP();
                        return false;
                    }
                    return true;
                }
                $scope.searchKeyPressT = function () {
                    if (event.keyCode == 13) {
                        $scope.TransmittedByInvoiceNumberAP();
                        return false;
                    }
                    return true;
                }
                $scope.editInvoice = function (invoiceResult) {
                    var detailUrl = 'ap.rvi';

                    $state.go(detailUrl, { edit: true, poNumber: invoiceResult.poNumber, invoiceId: invoiceResult.id, vendorCode: invoiceResult.vendorCode});
                }
                var displayActionToastTransmit = function (message) {
                    if ($mdMedia('xs') == true) {
                        $scope.displayToast(message, '#apTransmit');
                    }
                    else {
                        $scope.displayToast(message, '#apTransmit');
                    }
                }
                var displayActionToastReset = function (message) {
                    if ($mdMedia('xs') == true) {
                        $scope.displayToast(message, '#apResetTransmit');
                    }
                    else {
                        $scope.displayToast(message, '#apResetTransmit');
                    }
                }
                
                $scope.closeDetailDialog = function () {
                    DialogService.closeDialog();
                }

                /**
                 * Closes the vendor lookup dialog
                 */
                $scope.cancelLookup = function () {
                    DialogService.closeDialog();
                }

                //Call VendorSearch
                //$scope.vendorSearch();

            }]);
    });
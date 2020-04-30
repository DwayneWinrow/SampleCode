define(['AngularProvider', 'Constants', 'DialogService', 'TabService', 'AccountPayableService'],
    function (angularProvider, Constants) {
        /**
         * @version 1.0
         * @namespace Controllers
         * @class ApprovalController
         */
        angularProvider.registerController('ReceiveVendorInvoiceController', ['$scope', '$state', '$http', '$mdMedia', 'DialogService', '$q', 'TabService', '$stateParams', 'AccountPayableService',
            function ($scope, $state, $http, $mdMedia, DialogService,$q, tabService,$stateParams, AccountPayableService) {
                $scope.TabService = tabService;
                $scope.setIcon(Constants.Icons.Financial)
                $scope.selectedReady2Transmit = [];
                $scope.selectedTransmitted = [];
                $scope.selectedLineItems = [];
                $scope.transmittedAP = false;
                $scope.PACEJobTasksResults = new Object();

                $scope.transmittedResults = new Object();
                $scope.ready2TransmittedResults = new Object();
                $scope.VendorsResults = new Object();

                $scope.POsByVendorResults = new Object();
                $scope.polineItemResults = new Object();
                $scope.PublishInvoice = new Object();
                $scope.transmitInvoices = new Object();
                $scope.PublishInvoice.poNumber = null;
                $scope.TransmittedReport = new Object();
                $scope.poLineItemTotal = null;
                $scope.POReceiptsResults = null;
                $scope.POHeaderResults = null;
                $scope.PublishInvoice.freightAmount = null;
                $scope.PublishInvoice.taxAmount = null;
                $scope.PublishInvoice.subTotal = null;
                //Upload File Section
                $scope.callouts = null;
                $scope.selectedFile = null;
                $scope.selectedFileName = null;
                $scope.inputFileSelected = false;
                $scope.disableLabel = { 'visibility': 'hidden' };
                $scope.btnVendorInvoice = false;
                $scope.InvoiceByVendorPONumber = new Object();
                $scope.subTotalDisplay = '';
                $scope.poAmountDisplay = '';
                $scope.editLabel = { 'visibility': 'hidden' };
                $scope.PublishInvoice.resetReceiptInvoiceData = false;
                $scope.loadPOHeaders = function () {
                    $scope.maxDate = new Date();
                    $scope.maxDate = new Date(
                        $scope.maxDate.getFullYear(),
                        $scope.maxDate.getMonth() + 1,
                        $scope.maxDate.getDate());
                    if ($stateParams.edit) {
                        $scope.editInvoices();
                        return;
                    }                    
                }
                $scope.editInvoices = function () {
                    $scope.editLabel = { 'visibility': 'visible', 'background-color': 'yellowgreen', 'font-size': '20px' };
                    setTimeout(function () { angular.element("#ApVendorInvoiceTabs md-tab-item")[0].click() });
                    $scope.PublishInvoice.vendorCode = $stateParams.vendorCode;
                    $scope.PublishInvoice.invoiceId = $stateParams.invoiceId;
                    var promise = AccountPayableService.getInvoiceById($stateParams.invoiceId);
                    var promiseLines = AccountPayableService.getLineItemByInvoiceId($stateParams.invoiceId, $stateParams.edit);
                    var opsList = [promise, promiseLines];
                    $q.all(opsList).then(function (data) {
                        $scope.poSearchText = $stateParams.poNumber;
                        $scope.PublishInvoice.poNumber = $stateParams.poNumber;
                        $scope.PublishInvoice.taxAmount = data[0].taxAmount;
                        $scope.PublishInvoice.freightAmount = data[0].freightAmount;
                        $scope.PublishInvoice.vendorName = data[0].vendorName;
                        if (isNaN(data[0].invoiceAmount)) {
                            $scope.subTotalDisplay = '$0.00';
                        } else {
                            $scope.subTotalDisplay = '$' + (data[0].invoiceAmount.toFixed(2) - $scope.PublishInvoice.taxAmount.toFixed(2) - $scope.PublishInvoice.freightAmount.toFixed(2));
                        }
                        if (isNaN(data[0].invoiceAmount)) {
                            $scope.poAmountDisplay = '$0.00';
                        } else {
                            $scope.poAmountDisplay = '$' + data[0].invoiceAmount.toFixed(2);
                        }
                        $scope.PublishInvoice.poAmount = data[0].invoiceAmount;
                        $scope.PublishInvoice.vendorCode = data[0].vendorId;
                        $scope.PublishInvoice.invoiceNumber = data[0].invoiceNumber;
                        var iDate = new Date(data[0].invoiceDate);
                        $scope.PublishInvoice.invoiceDate = iDate;
                        $scope.PublishInvoice.poLineItemReceiptsResults = data[1];
                        $scope.PublishInvoice.resetReceiptInvoiceData = true;
                    });
                }
                $scope.getVendorByVendorCode = function (vendorCode) {
                    $scope.displaySpinner();
                    var promise = AccountPayableService.getVendorByVendorCode(vendorCode);
                    promise.then(function (data) {
                        $scope.PublishInvoice.vendorName = data;
                        $scope.hideSpinner();
                    });
                }
                $scope.getPOHeadersFilter = function (poHeader) {  // PO matching for typeahed
                    //$scope.PublishInvoice.vendorCode =null;
                    var filter = [];
                    if (poHeader.length >= 2) {
                        var re = new RegExp(poHeader, 'gi');
                        _.forEach($scope.POHeaderResults, function (jn) {
                            if (re.test(jn.poNumber)) {
                                filter.push(jn.poNumber);
                            }
                        });
                    }
                    return filter;
                }
                $scope.POHeaderSearch = function (POsearch) {
                    if (isNaN(POsearch))
                        return;
                    var deffered = $q.defer();
                    var promise = AccountPayableService.poHeaderSearch(POsearch);
                    promise.then(function (results) {
                        deffered.resolve(results)
                        if (deffered.promise == null) {
                            //$scope.PublishInvoice.vendorCode = $scope.POHeaderResults[i].vendorCode;
                            displayActionToastMissingPONum('PONumber Not Found');
                            return;
                        }
                    });
                    return deffered.promise;
                }
                $scope.setVendorCode = function (poNumber) {  // PO matching for typeahead
                    if (!$stateParams.edit) {
                        for (i = 0; i < $scope.POHeaderResults.length; i++) {
                            if ($scope.POHeaderResults[i].poNumber == poNumber) {
                                $scope.PublishInvoice.vendorCode = $scope.POHeaderResults[i].vendorCode;
                                break;
                            }
                        }
                    }
                    var promise = AccountPayableService.getInvoicesByPoNumberVendorId($scope.PublishInvoice.vendorCode, poNumber);
                    promise.then(function (data) {
                        $scope.InvoiceByVendorPONumber = data;
                    });
                }
                $scope.getVendorCode = function (poNumber) {  // PO matching for typeahead
                    var promiseVendor = AccountPayableService.getVendorNameByPONumber(poNumber);
                    promiseVendor.then(function (data) {
                        if (data != null) {
                            $scope.PublishInvoice.vendorCode = data.vendorId;
                            $scope.PublishInvoice.vendorName = data.vendorName;
                            var promise = AccountPayableService.getInvoicesByPoNumberVendorId(data.vendorId, poNumber);
                            promise.then(function (data) {
                                $scope.InvoiceByVendorPONumber = data;
                            });
                        } else {
                            displayActionToastMissingPONum(' Missing Vendor/Invalid PO. Please select & try again.');
                            return false;
                        }
                    });
                    var promiseInvoice = AccountPayableService.getInvoicesByPoNumberVendorId($scope.PublishInvoice.vendorCode, poNumber);
                    promiseInvoice.then(function (data) {
                        $scope.InvoiceByVendorPONumber = data;
                    });
                    return true;
                }
                
                $scope.PODetailsByPONumberSearch = function (POId) {
                    var promise = AccountPayableService.getPODetailsByPOId(POId);
                    promise.then(function (data) {
                        $scope.poLineItemTotal = null;
                        $scope.PublishInvoice.poLineItemReceiptsResults = null;
                        $scope.PublishInvoice.poLineItemReceiptsResults = data;
                        $scope.PublishInvoice.poNumber = POId;
                    });
                }
                $scope.LineItemByPOId = function (POId) {
                    if (POId === null || POId === '' || POId.length === 0 || isNaN(POId)) {
                        displayActionToastMissingPONum(' Missing PO Number. Please select & try again.')
                        return;
                    } else {
                        $scope.refresh(false);
                        if ($scope.getVendorCode(POId)) {
                            $scope.displaySpinner();
                            var promiseLines = AccountPayableService.getLineItemByPOId(POId);
                            var promiseVendor = AccountPayableService.getVendorByVendorCode($scope.PublishInvoice.vendorCode);
                            var opsList = [promiseLines, promiseVendor];
                            $q.all(opsList).then(function (data) {
                                $scope.poLineItemTotal = null;
                                $scope.PublishInvoice.poNumber = POId;
                                $scope.PublishInvoice.poLineItemReceiptsResults = null;
                                $scope.PublishInvoice.poLineItemReceiptsResults = data[0];
                                if ($scope.PublishInvoice.poLineItemReceiptsResults == null) {
                                    displayActionToastMissingPONum(' PO Details not found for ' + POId);
                                } else {
                                    //$scope.PublishInvoice.vendorName = data[1].vendorName;
                                }
                                $scope.hideSpinner();
                            });
                        }
                    }
                }
                $scope.PreviewAPReport = function () {
                    var file = null;
                    var fileURL = null;
                    $scope.PDFreportContent = null;
                    $scope.transmitInvoices.transmitted = true;
                    $scope.transmitInvoices.Invoices2Transmit = $scope.selectedReady2Transmit;
                    var promise = AccountPayableService.createAPReport($scope.transmitInvoices);
                    promise.then(function (data) {
                        $scope.PDFreportContent = null;
                        file = new Blob([data], { type: 'application/pdf' });
                        fileURL = URL.createObjectURL(file);
                        $scope.PDFreportContent = fileURL;
                        DialogService.displayDialog($scope, null, Constants.Routes.GetAPReportDialog);
                    });
                }
                $scope.applyInvoice = function (PublishInvoice) {
                    if (PublishInvoice.freightAmount == null) {
                        PublishInvoice.freightAmount = 0;
                    }
                    if (PublishInvoice.taxAmount == null) {
                        PublishInvoice.taxAmount = 0;
                    }
                    if ($scope.applyInvoiceValidation($scope.selectedLineItems) && $scope.TestFile() && !$scope.InvoiceNumberExist()) {
                        var publishObject = {
                            poNumber: PublishInvoice.poNumber,
                            taxAmount: PublishInvoice.taxAmount,
                            freightAmount: PublishInvoice.freightAmount,
                            poAmount: PublishInvoice.poAmount,
                            subTotal: PublishInvoice.subTotal,
                            invoiceDate: PublishInvoice.invoiceDate,
                            invoiceNumber: PublishInvoice.invoiceNumber,
                            vendorName: PublishInvoice.vendorName,
                            vendorCode: PublishInvoice.vendorCode,
                            resetReceiptInvoiceData: PublishInvoice.resetReceiptInvoiceData,
                            invoiceId: PublishInvoice.invoiceId,
                            poLineItemReceiptsResults: $scope.selectedLineItems
                        };
                        $scope.uploadInvoiceFile(publishObject);
                        var promise = AccountPayableService.postApplyInvoice(publishObject);
                        promise.then(function (data) {
                        });
                        $scope.hideSpinner();
                        displayActionToastApplyInvoice(' Vendor invoice applied ');
                        $scope.refresh(true);
                    }
                }
                $scope.poLineItemReceiptSelected = function (poLineItemReceiptsResult) {
                    $scope.calculateTotal(poLineItemReceiptsResult);
                }
                $scope.poLineItemReceiptDeselected = function (poLineItemReceiptsResult) {
                    $scope.calculateTotal(poLineItemReceiptsResult);
                }
                $scope.calculateTotal = function () {
                    $scope.PublishInvoice.poAmount = 0;
                    $scope.PublishInvoice.subTotal = 0;
                    //$scope.PublishInvoice.taxAmount = $scope.PublishInvoice.taxAmount.replace('$', '');
                    //$scope.PublishInvoice.freightAmount = $scope.PublishInvoice.freightAmount.replace('$', '');
                    var temp = 0;
                     _.forEach($scope.selectedLineItems, function (selectedItem) {
                         //selectedItem.totalPOAmount = (Math.round(selectedItem.qty2BeInvoiced * 100) / 100) * (Math.round(selectedItem.unitCost * 100) / 100);
                         selectedItem.totalPOAmount = (Math.round(selectedItem.qty2BeInvoiced * 100) / 100) * selectedItem.unitCost;
                         temp += (Math.round(selectedItem.totalPOAmount * 100) / 100)
                     });
                     $scope.PublishInvoice.subTotal = (Math.round(temp * 100) / 100);
                     $scope.PublishInvoice.poAmount = (Math.round(temp * 100) / 100) + (Math.round($scope.PublishInvoice.taxAmount * 100) / 100) + (Math.round($scope.PublishInvoice.freightAmount * 100) / 100);
                     $scope.PublishInvoice.poAmount = (Math.round($scope.PublishInvoice.poAmount * 100) / 100)
                     if (isNaN($scope.PublishInvoice.subTotal)) {
                         $scope.subTotalDisplay = '$0.00';
                     } else {
                         $scope.subTotalDisplay = '$' + $scope.PublishInvoice.subTotal.toFixed(2);
                     }
                     if (isNaN($scope.PublishInvoice.poAmount)) {
                         $scope.poAmountDisplay = '$0.00';
                     } else {
                         $scope.poAmountDisplay = '$' + $scope.PublishInvoice.poAmount.toFixed(2);
                     }
                }
                $scope.applyInvoiceValidation = function (poLineItemReceiptsResult) {
                    var blnRtn = true;
                    if ($scope.PublishInvoice.poAmount <= 0 || $scope.PublishInvoice.subTotal <= 0)
                    {
                        displayActionToastApplyInvoice(' The invoice subtotal or total can not be zero or negative. ');
                        blnRtn = false;
                    }
                    if (blnRtn) {
                        for (i = 0; i < poLineItemReceiptsResult.length; i++) {
                            if (poLineItemReceiptsResult[i].qty2BeInvoiced > 0
                                && (poLineItemReceiptsResult[i].qty2BeInvoiced > poLineItemReceiptsResult[i].qtyReceiptReceived
                                || poLineItemReceiptsResult[i].qty2BeInvoiced <= 0
                                || isNaN(poLineItemReceiptsResult[i].qty2BeInvoiced))) {
                                blnRtn = false;
                                displayActionToastApplyInvoice(' You cannot invoice more than received. See ' + poLineItemReceiptsResult[i].partNumber);
                                break;
                            }
                            if (poLineItemReceiptsResult[i].qty2BeInvoiced <= 0) {
                                blnRtn = false;
                                displayActionToastApplyInvoice(' You cannot invoice zero or negative amounts. See ' + poLineItemReceiptsResult[i].partNumber);
                                break;
                            }
                            if (poLineItemReceiptsResult[i].qty2BeInvoiced > poLineItemReceiptsResult[i].qtyInvoiceLeft) {
                                blnRtn = false;
                                displayActionToastApplyInvoice(' You cannot invoice more than ' + poLineItemReceiptsResult[i].qtyInvoiceLeft + '. See ' + poLineItemReceiptsResult[i].partNumber);
                                break;
                            }
                            if (poLineItemReceiptsResult[i].qty2BeInvoiced % 1 != 0) {
                                blnRtn = false;
                                displayActionToastApplyInvoice(' You cannot use decimal value for quantites. See ' + poLineItemReceiptsResult[i].partNumber);
                                break;
                            }
                        }
                    }
                    return blnRtn;
                }
                $scope.showFile = function () {
                    var x = $('#invoiceFile');
                    x.click();
                }
                $("#invoiceWrapper").on("change", "#invoiceFile", function () {
                    var x = $('#invoiceFile');
                    var fileName = x[0].value;
                    var result = _.split(fileName, '\\');
                    var name = result[result.length - 1];
                    //alert(name);               
                    $scope.selectedFileName = name;
                    $scope.disableLabel = { 'visibility': 'visible' };
                    $scope.$apply();
                });
                $scope.uploadInvoiceFile = function (PublishInvoice) {
                    var file = document.getElementById('invoiceFile').files[0];
                    $scope.readFile(file, function (e) {
                        //    // use result in callback...
                        var file = e.target.result
                        var data = {
                            invoiceNumber: PublishInvoice.invoiceNumber,
                            invoiceName: $scope.selectedFileName,
                            invoiceDate: PublishInvoice.invoiceDate,
                            vendorCode: PublishInvoice.vendorCode,
                            content: file
                        };
                        var promise = AccountPayableService.uploadInvoicePDFFile(data);
                        promise.then(function (data) {
                            $scope.hideSpinner();
                        });
                    }); 
                }
                $scope.readFile = function(file, onLoadCallback) {
                    var reader = new FileReader();
                    reader.onloadend = onLoadCallback;
                    reader.readAsDataURL(file);
                }
                $scope.TestFile = function TestFileType() {
                    var btnReturn = true;
                    var fileName = document.getElementById('invoiceFile').files[0].name;
                    var dots = fileName.split(".")
                    var fileType = "." + dots[dots.length-1];
                    if (fileType.toLocaleUpperCase()!='.PDF')
                    {
                        btnReturn = false;
                        displayActionToastApplyInvoice(' Invalid File Type. Please upload PDF file. ');
                    }
                    return btnReturn;
                }
                $scope.InvoiceNumberExist = function CheckInvoiceNumber()
                {
                    var btnReturn = false;
                    for (i = 0; i < $scope.InvoiceByVendorPONumber.length; i++) {
                        if($scope.InvoiceByVendorPONumber[i].invoiceNumber ==$scope.PublishInvoice.invoiceNumber)
                        {
                            btnReturn = true;
                            displayActionToastApplyInvoice(' The invoice number ' + $scope.PublishInvoice.invoiceNumber + ' already exists for ' + $scope.PublishInvoice.vendorName);
                            break;
                        }
                    }
                    return btnReturn;
                }
                $scope.refresh = function (NoRefreshPOHeader) {
                    if (NoRefreshPOHeader) {
                        $scope.poSearchText = null;
                    } 
                    $scope.disableLabel = { 'visibility': 'hidden' };
                    $scope.editLabel = { 'visibility': 'hidden' };
                    $scope.PublishInvoice.vendorName = null;
                    $scope.selectedLineItems = [];
                    $scope.selectedFileName = null;
                    $scope.PublishInvoice.invoiceDate = null;
                    $scope.PublishInvoice.invoiceNumber = null;
                    $scope.PublishInvoice.poAmount = null;
                    $scope.PublishInvoice.freightAmount = null;
                    $scope.PublishInvoice.taxAmount = null;
                    $scope.PublishInvoice.subTotal = null;
                    $scope.subTotalDisplay = '';
                    $scope.poAmountDisplay = '';
                    $scope.poLineItemTotal = null;
                    $scope.POReceiptsResults = null;
                   // $scope.readFile = null;
                    $scope.PublishInvoice.poLineItemReceiptsResults = null;
                    $scope.PublishInvoice.resetReceiptInvoiceData = false;
                }
               
                $scope.disableActionFileUpload = function (target) {
                    if (target === 'fileUpload') {
                        return $scope.inputFileSelected == false ? true : false;
                    }
                }
                $scope.searchKeyPress = function ()  {
                    if (event.keyCode == 13) {
                        $scope.LineItemByPOId($scope.poSearchText);
                        return false;
                    }
                    return true;
                }
                $scope.disableAction = function () {
                    var blnRtn = true;
                    if ($scope.selectedFileName !=null & $scope.poSearchText != null && $scope.PublishInvoice.invoiceNumber != null &&
                        $scope.PublishInvoice.invoiceDate != null && $scope.PublishInvoice.poAmount != 0 &&
                        $scope.PublishInvoice.vendorCode != null && $scope.selectedLineItems.length != 0) {
                        //$scope.selectedLineItems.length != 0) {
                        //$scope.PublishInvoice.vendorCode != null) {
                        blnRtn = false;
                    }
                    return blnRtn;
                }
                /*
                Toast Messages 
                */
                var displayActionToastMissingPONum = function (message) {
                    if ($mdMedia('xs') == true) {
                        $scope.displayToast(message, '#invoiceWrapper');
                    }
                    else {
                        $scope.displayToast(message, '#invoiceWrapper');
                    }
                }
                var displayActionToastApplyInvoice = function (message) {
                    if ($mdMedia('xs') == true) {
                        $scope.displayToast(message, '#invoiceWrapper');
                    }
                    else {
                        $scope.displayToast(message, '#invoiceWrapper');
                    }
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

                $scope.cancelLookup = function () {
                    DialogService.closeDialog();
                }

            }]);
    });
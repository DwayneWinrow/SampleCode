define(['Constants','AngularProvider'], function (constants,angularProvider) {
    /**
     * @version 1.0
     * @namespace Services
     * @class AccountPayableService
     */
    angularProvider.registerService('AccountPayableService', ['$http', '$q', function ($http, $q) {

        var bulkpoService = {
            /**
             * Returns a Job
             */
            ready2TransmittedAP: function (transmitted) {
                return $q(function (resolve, reject) {
                    $http.get(constants.Routes.ReadyOrTransmittedAP(transmitted)).success(function (ready2TransmittedResults) {
                        resolve(ready2TransmittedResults);
                    });
                });
            },
            transmittedAP: function (transmitted) {
                return $q(function (resolve, reject) {
                    $http.get(constants.Routes.ReadyOrTransmittedAP(transmitted)).success(function (transmittedAPResults) {
                        resolve(transmittedAPResults);
                    });
                });
            },
            
            getPOsByVendor: function (vendorId) {
                return $q(function (resolve, reject) {
                    $http.get(constants.Routes.GetPOsByVendor(vendorId)).success(function (POsByVendorResults) {
                        resolve(POsByVendorResults);
                    });
                });
            },            

            poHeaderSearch: function (POsearch) {
                return $q(function (resolve, reject) {
                    $http.get(constants.Routes.POHeaderSearch(POsearch)).success(function (POHeaderSearchResults) {
                        resolve(POHeaderSearchResults);
                    });
                });
            },
            getPOReceiptByPOId: function (POId) {
                return $q(function (resolve, reject) {
                    $http.get(constants.Routes.GetPOReceiptsByPOId(POId)).success(function (POReceiptsResults) {
                        resolve(POReceiptsResults);
                    });
                });
            },
            getLineItemByReceiptNumber: function (receiptNumber) {
                return $q(function (resolve, reject) {
                    $http.get(constants.Routes.GetLineItemsByReceipt(receiptNumber)).success(function (LineItemsByResults) {
                        resolve(LineItemsByResults);
                    });
                });
            },
            getLineItemByPOId: function (POId) {
                return $q(function (resolve, reject) {
                    $http.get(constants.Routes.GetLineItemsByPOId(POId)).success(function (LineItemsByResults) {
                        resolve(LineItemsByResults);
                    });
                });
            },
            //getLineItemByPOId: function (POId) {
            //    return $q(function (resolve, reject) {
            //        $http.get(constants.Routes.GetLineItemsByPOId(POId)).success(function (LineItemsByResults) {
            //            resolve(LineItemsByResults);
            //        });
            //    });
            //},
            getLineItemByInvoiceId: function (invoiceId, editMode) {
                return $q(function (resolve, reject) {
                    $http.get(constants.Routes.GetLineItemsByInvoiceId(invoiceId, editMode)).success(function (LineItemsByResults) {
                        resolve(LineItemsByResults);
                    });
                });
            },
            getVendorByVendorCode: function (vendorCode) {
                return $q(function (resolve, reject) {
                    $http.get(constants.Routes.GetVendorByVendorCode(vendorCode)).success(function (VendorsResults) {
                        resolve(VendorsResults);
                    });
                });
            },
            getVendorNameByPONumber: function (poNumber) {
                return $q(function (resolve, reject) {
                    $http.get(constants.Routes.GetVendorNameByPONumber(poNumber)).success(function (VendorsResults) {
                        resolve(VendorsResults);
                    });
                });
            },
            getInvoicesByPoNumberVendorId: function (vendorId, PONumber) {
                return $q(function (resolve, reject) {
                    $http.get(constants.Routes.GetInvoicesByPoNumberVendorId(vendorId, PONumber)).success(function (InvoiceResults) {
                        resolve(InvoiceResults);
                    });
                });
            },
            getVendors: function () {
                return $q(function (resolve, reject) {
                    $http.get(constants.Routes.GetVendors()).success(function (VendorsResults) {
                        resolve(VendorsResults);
                    });
                });
            },
            getPOwithInvoices: function () {
                return $q(function (resolve, reject) {
                    $http.get(constants.Routes.GetPOwithInvoices()).success(function (POInvoiceResults) {
                        resolve(POInvoiceResults);
                    });
                });
            },
            getInvoiceById: function (Id) {
                return $q(function (resolve, reject) {
                    $http.get(constants.Routes.GetInvoiceById(Id)).success(function (InvoiceResult) {
                        resolve(InvoiceResult);
                    });
                });
            },
            getVendorsWithPOs: function () {
                return $q(function (resolve, reject) {
                    $http.get(constants.Routes.GetVendorsWithPOs()).success(function (VendorPOResults) {
                        resolve(VendorPOResults);
                    });
                });
            },
            checkVendorInvoiceNumber: function (invoiceNumber,vendorId) {
                return $q(function (resolve, reject) {
                    $http.get(constants.Routes.CheckVendorInvoiceNumber(invoiceNumber, vendorId)).success(function (VendorInvoiceResults) {
                        resolve(VendorInvoiceResults);
                    });
                });
            },
            getAPUsers: function () {
                return $q(function (resolve, reject) {
                    $http.get(constants.Routes.GetAPUsers()).success(function (APUsers) {
                        resolve(APUsers);
                    });
                });
            },
            //HTTP POST
            doesPoLineItemHaveReceipt: function (PONumbers) {
                return $q(function (resolve, reject) {
                    $http.post(constants.Routes.DoesPoLineItemHaveReceipt, PONumbers).success(function (TrueFalseData) {
                        resolve(TrueFalseData);
                    });
                });
            },
            getStatus: function(id) {
                return $q(function(resolve, reject) {
                    $http.get(constants.Routes.GetStatus(id)).success(function (status) {
                        resolve(status);
                    });
                });
            },
            getDynamicsResult: function(id) {
                return $q(function(resolve, reject) {
                    $http.get(constants.Routes.getDynamicsResult(id)).success(function (status) {
                        resolve(status);
                    });
                });
            },
            transmittedByInvoiceNumberAP: function (APObject) {
                return $q(function (resolve, reject) {
                  
                    $http.post(constants.Routes.ReadyOrTransmittedByInvoiceNumberAP,APObject).success(function (transmittedAPResults) {
                        resolve(transmittedAPResults);
                    });
                });
            },
            uploadInvoicePDFFile: function (data) {
                return $q(function (resolve, reject) {
                    $http.post(constants.Routes.UploadInvoicePDFFile, data).success(function (uploadResults) {
                        resolve(uploadResults);
                    });
                });
            },
            resetOrTransmittedAP: function (transmitInvoices) {
                return $q(function (resolve, reject) {
                    $http.post(constants.Routes.ResetOrTransmittedAP, transmitInvoices).success(function (PACEJobTasksResults) {
                        resolve(PACEJobTasksResults);
                    });
                });
            },
            createAPReport: function (transmitInvoices) {
                return $q(function (resolve, reject) {
                    $http.post(constants.Routes.CreateAPReport, transmitInvoices, { responseType: 'arraybuffer' }).success(function (PDFAPtransmit) {
                        resolve(PDFAPtransmit);
                    }).error(function (error) {
                        reject(error);
                    });
                });
            },
            runTransmittedAPReport: function (transmittedInvoices) {
                return $q(function (resolve, reject) {
                    $http.post(constants.Routes.RunTransmittedAPReport, transmittedInvoices, { responseType: 'arraybuffer' }).success(function (PDFAPtransmit) {
                        resolve(PDFAPtransmit);
                    }).error(function (error) {
                        reject(error);
                    });
                });
            },
            deleteInvoiceAP: function (invoicesAP) {
                return $q(function (resolve, reject) {
                    $http.post(constants.Routes.DeleteInvoiceAP, invoicesAP).success(function (nothingResults) {
                        resolve(nothingResults);
                    });
                });
            },
            postApplyInvoice: function (PublishInvoice) {
                return $q(function (resolve, reject) {
                    $http.post(constants.Routes.ApplyInvoice, PublishInvoice).success(function (nothingResults) {
                        resolve(nothingResults);
                    });
                });
            },
        };

        return bulkpoService;
    }]);
});

/// <reference path="ConstantsService.js" />

/**
 * Maintains a list of constants
 *
 */
define([], function() {
    var c = {
        App: 'OPApp',
        Icons: {
            Dashboard: 'dashboard',
            Approval: 'widgets',
            OrderRelease: 'assignment',
            Erf: 'description',
            Rma: 'assignment_return',
            Financial: 'monetization_on',
            Invoice: 'attach_money',
            MRF: 'view_list',
            Adjustments: 'swap_vertical_circle',
            PurchaseOrder: 'shopping_cart',
            VendorAssignment: 'assignment_turned_in',
            Transfer: 'compare_arrows'
        },
        Calendar: {
            Action: {
                AddBlockSlot: 'Calendar_Action_AddBlockSlot',
                RemoveBlockSlot: 'Calendar_Action_RemoveBlockSlot',
                UpdateBlockSlot: 'Calendar_Action_UpdateBlockSlot'
            },
            Failed: {
                AddingBlockSlot: 'Calendar_Failed_AddingBlockSlot',
                RemovingBlockSlot: 'Calendar_Failed_RemovingBlockSlot',
                UpdatingBlockSlot: 'Calendar_Failed_UpdatingBlockSlot'
            },
            Event: {
                BlockSlotChanged: 'Calendar_Event_BlockSlotChanged'
            }
        },
        CalloutStatus: {
            Assigned: 1, //Currently assigned to a job
            Reassigned: 2, //Reassigned to a job
            PendingApproval: 3, //Pending Approval
            NotAvailable: 4, //Currently assigned to another job
            Available: 5 //Available to be assigned to job scope
        },
        Routes: {
            ApprovalJobSearch: function(searchCriteria) {
                return 'api/approval/search/' + searchCriteria;
            },
            ApprovalGetVendors: function() {
                return 'api/approval/getVendorsJobApprovals/';
            },

            SiteIdSearch: function(siteNumber) {
                return 'api/jobs/siteIdSearch/' + siteNumber;
            },
            FetchJobNumbers: 'api/jobs/fetchJobNumbers',
            FetchPartNumbers: 'api/items/fetchPartNumbers',
            FetchMaterialRequests: function(jobNumber) {
                return 'api/materials/fetchMaterialRequests/' + jobNumber;
            },
            FetchMaterialRequestsByScopingLead: 'api/materials/fetchMaterialRequestsByScopingLead',
            FetchMaterialRequestDashboard: 'api/materials/fetchMaterialRequestDashboard',
            FetchSubcontractors: 'api/orders/FetchSubcontractors',
            SubContractorSearch: function(subcontractor) {
                return 'api/orders/subcontractorSearch/' + subcontractor;
            },
            GetCalloutDetailDialog: 'OrderRelease/GetCalloutDetailDialog',
            GetPaceNumberLookupDialog: 'OrderRelease/GetPaceNumberLookupDialog',
            GetCalloutLookupDialog: 'OrderRelease/GetCalloutLookupDialog',
            GetPartLookupDialog: 'MaterialRequest/GetPartLookupDialog',
            GetMaterialRequestApprovalDialog: 'MaterialRequest/GetMaterialRequestApprovalDialog',
            GetSubContractorLookupDialog: 'OrderRelease/GetSubContractorLookupDialog',
            GetDashboardDetail: 'DashBoard/GetDashboardDetail',

            GetJobLineDetailDialog: 'Approval/GetJobLineDetailDialog',
            GetVendorLookupDialog: 'Approval/GetVendorLookupDialog',
            GetErfCalloutDetailDialog: 'Erf/GetCalloutDetailDialog',
            FetchOrderReleaseDetail: function(orderId) {
                return 'api/orders/fetchOrderReleaseDetail/' + orderId;
            },
            FetchMaterialRequestsByConstructionManager: function(userId) {
                return 'api/materials/fetchMaterialRequestsByConstructionManager/' + userId;
            },
            FetchJobNumbersByScopingLead: function(userId) {
                return 'api/materials/fetchJobNumbersByScopingLead/' + userId;
            },
            FetchPaceNumbersBySiteId: function(siteId) {
                return 'api/orders/fetchPaceNumbersBySiteId/' + siteId;
            },
            FetchOrderReleaseJob: function(jobNumber, releaseRequestType) {
                return 'api/orders/fetchOrderReleaseJob/' + jobNumber + '/' + releaseRequestType;
            },
            FetchJob: function(jobNumber) {
                return 'api/jobs/fetchJob/' + jobNumber;
            },
            FetchJobNumbersBySiteId: function(siteId) {
                return 'api/jobs/fetchJobNumbersBySiteId/' + siteId;
            },
            FetchJobImportErrors: function(jobNumber) {
                return 'api/jobs/fetchJobImportErrors/' + jobNumber;
            },
            SyncJobs: 'api/jobs/syncJobs/',
            FetchPart: function(partNumber) {
                return 'api/items/fetchPart/' + partNumber;
            },
            FetchCallout: function(calloutId) {
                return 'api/orders/fetchCallout/' + calloutId;
            },
            FetchCallouts: 'api/orders/fetchCallouts',
            RequestMaterial: 'api/materials/requestMaterial',
            ReleaseOrder: 'api/orders/releaseOrder',
            ReleaseCrossShipOrder: 'api/orders/releaseCrossShipOrder',
            UploadErfFile: 'api/erf/uploadErfFile',
            FetchUser: 'api/user/fetchUser',
            FetchDashBoardInventoryItems: 'api/dashboard/fetchDashBoardInventoryItems',

            fetchDashBoardInventoryItemByPartNumber: function(partNumber) {
                return 'api/dashboard/fetchDashBoardInventoryItem/' + partNumber;
            },

            fetchApplicationConfigurationItems: 'api/administration/FetchApplicationConfigurationItems',

            putApplicationConfigurationItem: 'api/administration/putApplicationConfigurationItem',
            postApplicationConfigurationItem: 'api/administration/postApplicationConfigurationItem',


            GetNORADConfiguration: 'api/administration/GetNORADConfiguration',
            TestNORADConfiguration: function (NoradServiceUrl, NoradVendorId, NoradAuthenticationId) {
                return 'api/administration/TestNORADConfiguration?NoradServiceUrl=' + NoradServiceUrl + '&NoradVendorId=' + NoradVendorId + '&NoradAuthenticationId=' + NoradAuthenticationId;
            },

            ReportOOMTaskProjectNumberListModified: 'api/administration/ReportOOMTaskProjectNumberListModified',

            //AR
            GetPACECodeSummary: function() {
                return 'api/GetPACECodeSummary';
            },
            BillableMilestones: function(jobCode, siteCode) {
                return 'api/BillableMilestones?jobCode=' + jobCode + '&siteCode=' + siteCode;
            },
            LoadMilestoneInvoices: function(casprJobCode) {
                return 'api/LoadMilestoneInvoices/' + casprJobCode
            },
            BillablePACEJobTasks: function(jobCode, siteCode) {
                //    return 'api/BillablePACEJobTasks?jobCode=' + jobCode + '&siteCode=' + siteCode;
                return 'api/BillablePACEJobTasks?jobCode=' + jobCode + '&siteCode=' + siteCode;
            },
            LoadPACEJobTaskInvoices: function(casprJobCode) {
                return 'api/LoadPACEJobTaskInvoices/' + casprJobCode;
            },
            CompletedInvoiceSearch: function (criteria, value, userForQuery) {
                return 'api/AccountReceivable/ARCompletedInvoiceSearch?criteria=' + criteria + '&value=' + value + '&selectedUser=' + userForQuery;
            },
            GetInvoiceJobs: function(transmitted) {
                return 'api/GetInvoiceJobs/' + transmitted;
            },
            GetInvoiceCancel: function(casprJobCode, viewCreditMemo, transmitted) {
                return 'api/GetInvoiceCancel?casprJobCode=' + casprJobCode + '&viewCreditMemo=' + viewCreditMemo + '&transmitted=' + transmitted;
            },
            GetInvoiceLineItems: function(casprJobCode) {
                return 'api/GetInvoiceLineItems/' + casprJobCode;
            },
            GetInvoice: function(recordId) {
                return 'api/GetInvoice/' + recordId;
            },

            LineItemsByJob: function(casprJobCode) {
                return 'api/LineItemsByJob/' + casprJobCode;
            },

            ReadyOrTransmittedAR: function(transmitted) {
                return 'api/ReadyOrTransmittedAR/' + transmitted;
            },

            ReadyOrTransmittedByInvoiceNumberAR: function(invoiceNumber, transmitted) {
                return 'api/ReadyOrTransmittedByInvoiceNumberAR?invoiceNumber=' + invoiceNumber + '&transmitted=' + transmitted;
            },

            GenerateInvoiceNumber: function(casprJobCode) {
                return 'api/GenerateInvoiceNumber/' + casprJobCode;
            },
            CI006InvoicesCreated: 'api/AccountReceivable/CI006InvoicesCreated',

            ARCI006InvoicesCreatedCSV: 'api/AccountReceivable/ARCI006InvoicesCreatedCSV',
            // AR POST
            BuildReportPublishInvoice: 'api/BuildReportPublishInvoice/',

            GetARInvoicePDF: 'api/GetARInvoicePDF/',

            ResetOrTransmittedAR: 'api/ResetOrTransmittedAR',

            CreateARReport: 'api/CreateARReport',

            ApplyCreditMemo: 'api/ApplyCreditMemo/',

            ARGetAccountsReceivableUsers: 'api/AccountReceivable/ARGetAccountsReceivableUsers',

            ARGetInvoicesReadyForSubmission: function (taskCode,poNumber, userId) {
                return 'api/AccountReceivable/ARGetInvoicesReadyForSubmission?taskCode=' + taskCode + '&poNumber=' + poNumber + '&userId=' + userId;
            },

            ARGetInvoicesReadyForTransmission: function (taskCode, invoiceNumber, userId) {
                return 'api/AccountReceivable/ARGetInvoicesReadyForTransmission?taskCode=' + taskCode + '&invoiceNumber=' + invoiceNumber + '&userId=' + userId;
            },

            ARTransmitDateNotInvoiceDate: 'api/AccountReceivable/ARTransmitDateNotInvoiceDate',

            iSupplierClosedStatus: 'api/AccountReceivable/iSupplierClosedStatus',

            GetCompletedInvoicePDFFileFromS3: 'api/AccountReceivable/GetCompletedInvoicePDFFileFromS3',

            ARNoradMissingPONumber: 'api/AccountReceivable/ARNoradMissingPONumber',

            DisapprovedNoradLineStatusData: 'api/AccountReceivable/DisapprovedNoradLineStatusData',

            DisapprovedNoradLineStatusDataCSV: 'api/AccountReceivable/DisapprovedNoradLineStatusDataCSV',

            NoradMissingPONumberCSV: 'api/AccountReceivable/NoradMissingPONumberCSV',

            ARiSupplierClosedStatusCSV: 'api/AccountReceivable/ARiSupplierClosedStatusCSV',

            ARTransmitDateNotInvoiceDateCSV: 'api/AccountReceivable/ARTransmitDateNotInvoiceDateCSV',

            DeleteInvoiceReadyForTransmission: 'api/AccountReceivable/DeleteInvoiceReadyForTransmission',

            TransmitInvoiceARDynamics: 'api/AccountReceivable/TransmitInvoiceARDynamics',

            TransmissionPreview:  'api/AccountReceivable/TransmissionPreview',

            ARInvoiceCreatedIniSupplier: 'api/AccountReceivable/ARInvoiceCreatedIniSupplier',

            DeleteInvoiceAR: 'api/AccountReceivable/DeleteInvoiceAR',

            GetInvoiceARCSV: 'api/AccountReceivable/GetInvoiceARCSV',

            GetInvoicesReadyForTransmissionARCSV: 'api/AccountReceivable/GetInvoicesReadyForTransmissionARCSV',

            GetCompletedInvoiceARCSV: 'api/AccountReceivable/GetCompletedInvoiceARCSV',

            GetAllMarkets: 'api/AccountReceivable/GetAllMarkets',

            GetPOLineDollarAmountsReadyToBill: 'api/AccountReceivable/GetPOLineDollarAmountsReadyToBill',

            GetPOLineDollarAmountsNotReadyToBill: 'api/AccountReceivable/GetPOLineDollarAmountsNotReadyToBill',

            GenerateInvoiceAR: 'api/AccountReceivable/GenerateInvoiceAR',

            GenerateCL100InvoiceAR: 'api/AccountReceivable/GenerateCL100InvoiceAR',

            ResolveInvalidInvoices: 'api/AccountReceivable/ResolveInvalidInvoices',

            InvoiceARFinalValidation: 'api/AccountReceivable/InvoiceARFinalValidation',

            GetAllPOLineDollarAmountNoteCategories: 'api/AccountReceivable/GetAllPOLineDollarAmountNoteCategories',

            GetiSupplierPODataNotes: function (iSupplierPODataId) {
                return 'api/AccountReceivable/GetiSupplierPODataNotes?iSupplierPODataId=' + iSupplierPODataId;
            },

            iSupplierPODataNoteCreate: 'api/AccountReceivable/iSupplierPODataNoteCreate',

            DownloadiSupplierPOLineDollarReport: 'api/AccountReceivable/DownloadiSupplierPOLineDollarReport',

            GetInvoiceFileDialog: 'Invoice/GetInvoiceFileDialog',

            // JobApproval POST

            ApproveBulkPO: 'api/approval/ApproveBulkPO',
            RejectLineItems: 'api/approval/RejectLineItems',

            //AP
            CheckVendorInvoiceNumber: function(invoiceNumber, vendorId) {
                return 'api/CheckVendorInvoiceNumber/' + invoiceNumber + "/" + vendorId;
            },
            GetPOwithInvoices: function() {
                return 'api/GetPOwithInvoices';
            },
            GetInvoiceById: function(Id) {
                return 'api/GetInvoiceById/' + Id;
            },
            POHeaderSearch: function(POSearch) {
                return 'api/POHeaderSearch/' + POSearch;
            },
            ReadyOrTransmittedAP: function(transmitted) {
                return 'api/ReadyOrTransmittedAP/' + transmitted;
            },

            ResetOrTransmittedAP: 'api/ResetOrTransmittedAP',

            GetPOList2Invoice: function(relatedVendorPo) {
                return 'api/GetPOList2Invoice/' + relatedVendorPo;
            },
            GetVendorsWithPOs: function() {
                return 'api/GetVendorsWithPOs/';
            },

            GetPOsByVendor: function(vendorId) {
                return 'api/GetPOsByVendor/' + vendorId;
            },
            GetVendors: function() {
                return 'api/GetVendors/';
            },
            GetVendorByVendorCode: function(vendorCode) {
                return 'api/GetVendorByVendorCode/' + vendorCode;
            },
            GetVendorNameByPONumber: function(poNumber) {
                return 'api/GetVendorNameByPONumber/' + poNumber;
            },
            GetInvoicesByPoNumberVendorId: function(vendorId, PONumber) {
                return 'api/GetInvoicesByPoNumberVendorId/' + vendorId + "/" + PONumber;
            },
            GetPODetailsByPOId: function(POId) {
                return 'api/GetPODetailsByPOId/' + POId;
            },
            GetPOReceipts: function(POId) {
                return 'api/GetPOReceipts/' + POId;
            },
            GetPOReceiptsByPOId: function(POId) {
                return 'api/GetPOReceiptsByPOId/' + POId;
            },
            GetLineItemsByInvoiceId: function(invoiceId, editMode) {
                return 'api/GetLineItemsByInvoiceId/' + invoiceId + "/" + editMode;
            },
            GetLineItemsByReceipt: function(receiptNumber) {
                return 'api/GetLineItemsByReceipt/' + receiptNumber;
            },
            GetLineItemsByPOId: function(POId) {
                return 'api/GetLineItemsByPOId/' + POId;
            },
            GetAPUsers: function() {
                return 'api/GetAPUsers';
            },
            CreateAPReport: 'api/CreateAPReport',
            //TransmitInvoiceARDynamics: function (invoices) {
            //    return 'api/AccountReceivable/TransmitInvoiceARDynamics/' + invoices;
            //},
            TransmitInvoiceARDynamics: 'api/AccountReceivable/TransmitInvoiceARDynamics',
            GetStatus: function(id) {
                return 'api/GetStatus/' + id;
            },
            GetDynamicsResult: function(id) {
                return 'api/GetDynamicsResult/' + id;
            },

            // AP POST
            ReadyOrTransmittedByInvoiceNumberAP: 'api/ReadyOrTransmittedByInvoiceNumberAP/',
            DeleteInvoiceAP: 'api/DeleteInvoiceAP/',
            ApplyInvoice: 'api/ApplyInvoice/',
            RunTransmittedAPReport: 'api/RunTransmittedAPReport/',
            UploadInvoicePDFFile: 'api/UploadInvoicePDFFile/',
            DoesPoLineItemHaveReceipt: 'api/doesPoLineItemHaveReceipt/',
            // Material Invoice GET
            GetMaterialInvoices: function(searchCriteria) {
                return 'api/GetMaterialInvoices/' + searchCriteria;
            },
            GetOrdersByInvoiceId: function(invoiceId) {
                return 'api/GetOrdersByInvoiceId/' + invoiceId;
            },
            // Material Invoice POST
            GetInvoicePDF: 'api/GetInvoicePDF/',
            DownloadZipFile: 'api/DownloadZipFile/',
            // Material Invoice Dialogs
            GetInvoiceFileDialog: 'Invoice/GetInvoiceFileDialog',
            GetInvoiceOrderDetailsDialog: 'Invoice/GetInvoiceOrderDetailsDialog',
            //TransferWarehouse
            GetWarehouses: function() {
                return 'api/GetWarehouses';
            },
            GetTransferHistory: function() {
                return 'api/GetTransferHistory';
            },
            GetItemsByWarehouseCode: 'api/GetItemsByWarehouseCode/',
            CreateTransferOrder: 'api/CreateTransferOrder/',

            //Financial Dialogs
            GetInvoiceDetailsDialog: 'AccountReceivable/GetInvoiceDetailsDialog',
            GetInvoiceDetailsJobItemDialog: 'AccountReceivable/GetInvoiceDetailsJobItemDialog',
            GetInvoiceDetailsPDFDialog: 'AccountReceivable/GetInvoiceDetailsPDFDialog',
            GetARReportDialog: 'AccountReceivable/GetARReportDialog',
            GetInvoiceFileDialogAR: 'AccountReceivable/ARGetInvoiceFileDialog',
            GetInvoiceTransFindDialog: 'AccountPayable/GetInvoiceTransFindDialog',
            GetAPReportDialog: 'AccountPayable/GetAPReportDialog',

            //PurchaseOrders

            FetchWareHouses: 'api/purchaseorder/fetchWareHouses',
            FetchVendorPrices: 'api/purchaseorder/fetchVendorPrices',
            SendForApproval: 'api/purchaseorder/sendForApproval',
            FetchScopedJobLineItems: 'api/purchaseorder/fetchScopedJobLineItems',
            FetchOrderItemsPendingReview: function(orderId) {
                return 'api/orders/fetchOrderItemsPendingReview/' + orderId;
            },
            ReleasePendingOrder: 'api/orders/releasePendingOrder',
            UpdatePendingSCMOrderDetails: 'api/orders/updatePendingSCMOrderDetails',
            FetchActiveProjectNames: 'api/purchaseorder/fetchActiveProjects',
            FetchtVendortNames: 'api/purchaseorder/FetchtVendortNames',
            FetchScopeStatuses: 'api/purchaseorder/FetchScopeStatuses',
            GetActiveProjectLookupDialog: 'PurchaseOrder/GetActiveProjectLookupDialog',
            GetVendorNameLookupDialog: 'PurchaseOrder/GetVendorNameLookupDialog',
            GetScopeStatusLookupDialog: 'PurchaseOrder/GetScopeStatusLookupDialog',
            GetWarehouseLookupDialog: 'PurchaseOrder/GetWarehouseLookupDialog',
            PingRoute: 'api/ping/keepalive',
            CancelReleaseRequest: function(orderNumber) {
                return 'api/orders/cancelOrder/' + orderNumber;
            },
            LogOut: 'Account/LogOut',
            FetchBomByScope: 'api/orders/fetchBomByScope',
            FetchScheduledRequestsByPaceNumbers: 'api/schedulerequests/fetchScheduleRequestsByPaceNumbers',
            SaveScheduleRequest: 'api/schedulerequests/saveScheduleRequest',
            SaveScheduleRequests: 'api/schedulerequests/saveScheduleRequests',
            RequestPriortizationSearch: 'api/requestprioritization/search',
            SendOrdersToLogFire: 'api/requestprioritization/sendOrdersToLogFire',
            FetchCalendarData: 'api/requestprioritization/fetchCalendarData',
            SaveBlockedPickupDateTimeSlot: 'api/requestprioritization/saveBlockedPickupDateTimeSlot',
            SaveBlockedPickupDateTimeSlots: 'api/requestprioritization/saveBlockedPickupDateTimeSlots',
            GetPendingSCMDialog: 'RequestPrioritization/GetPendingSCMDialog',
            GetCalendar: 'ScheduleRequests/Calendar',
            GetDownloadOptionsPopUp: 'DashBoard/DownloadOptions',
            GetRevisedPurchaseOrders: 'api/purchaseorder/getRevisedPurchaseOrders',
            RejectRevisedPurchaseOrders: 'api/purchaseorder/rejectRevisedPurchaseOrders',
            ApproveRevisedPurchaseOrders: 'api/purchaseorder/approveRevisedPurchaseOrders',
            SavePoRevision: 'api/purchaseorder/savePoRevision',
            GetCurrentPORevisionLines: function(poNumber) {
                return 'api/purchaseorder/getCurrentPORevisionLines/' + poNumber;
            },
            CancelPos: 'api/purchaseorder/cancelPurchaseOrders',
            FetchJobNumbersByPOLineId: 'api/purchaseorder/fetchJobNumbersByPOLineId',
            FetchJobNumbersByCancelledPO: 'api/purchaseorder/fetchJobNumbersByCancelledPO',
            GetEffectedJobNumbers: 'PurchaseOrder/GetEffectedJobNumbers',
            GetEffectedJobNumbersForCancelledPOs: 'PurchaseOrder/GetEffectedJobNumbersForCancelledPOs',
            GetCancelPoErrorsDialog: 'PurchaseOrder/GetCancelPoErrorsDialog',
            GetPoLineItemSiteIds: 'api/purchaseorder/getPoLineItemSiteIds',
            GetPurchaseOrderDetails: 'api/purchaseorder/getPurchaseOrderDetails',
        },
        ScopedLineItemStatus: {
            Open: 1,
            Approved: 3,
            Rejected: 4,
            PendingApproval: 5
        },
        OrderTypes: {
            Standard: 'STANDARD',
            CrossShip: 'CROSSSHIP'
        },
        OrderStatus: {
            Received: 'RECEIVED',
            Cancelled: 'Cancelled',
            PendingSCM: 'PendingSCM'
        },
        PingInterval: {
            Amount: 60000, //Delay 1 Minute
            Max: 300000, //5 Minute logout
            IdleTimeOut: 120 //2 Hours (In Minutes)
        },
        Warehouses: {
            Clarkston_Georgia: "520"
        }
    }
    return c;
})
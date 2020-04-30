using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CertusView.OP.WebClient.Common
{
    public class Constants
    {
        public static  class Api
        {
            public const string Warehouses = "api/v2/warehouses";
        }
        public class Routes
        {

            #region Accounts Receivable Constants

            public const string AccountReceivablePrefix = "api/AccountReceivable";
            public const string ARGetAccountsReceivableUsersPrefix = "aRGetAccountsReceivableUsers";
            public const string ARTransmitDateNotInvoiceDate = "ARTransmitDateNotInvoiceDate";
            public const string ARISupplierClosedStatus = "iSupplierClosedStatus";
            public const string ARNoradMissingPONumber = "aRnoradMissingPONumber";
            public const string DisapprovedNoradLineStatusDataPrefix = "disapprovedNoradLineStatusData";
            public const string DisapprovedNoradLineStatusDataCSVPrefix = "disapprovedNoradLineStatusDataCSV";
            public const string NoradMissingPONumberCSVPrefix = "NoradMissingPONumberCSV";
            public const string ARiSupplierClosedStatusCSVPrefix = "ARiSupplierClosedStatusCSV";
            public const string ARTransmitDateNotInvoiceDateCSVPrefix = "ARTransmitDateNotInvoiceDateCSV";
            public const string ARGetInvoicesReadyForSubmissionPrefix = "aRGetInvoicesReadyForSubmission";
            public const string ARGetInvoicesReadyForTransmissionPrefix = "aRGetInvoicesReadyForTransmission";
            public const string DeleteInvoiceReadyForTransmission = "deleteInvoiceReadyForTransmission";
            public const string ARCompletedInvoiceSearchPrefix = "aRCompletedInvoiceSearch";
            public const string GetCompletedInvoicePDFFileFromS3Prefix = "GetCompletedInvoicePDFFileFromS3";
            public const string ARInvoiceCreatedIniSupplier = "aRInvoiceCreatedIniSupplier";
            public const string DeleteInvoiceAR = "deleteInvoiceAR";
            public const string GetInvoiceARCSV = "getInvoiceARCSV";
            public const string GetCompletedInvoiceARCSV = "GetCompletedInvoiceARCSV";
            public const string GetInvoicesReadyForTransmissionARCSV = "getInvoicesReadyForTransmissionARCSV";

            public const string GetAllMarkets = "getAllMarkets";
            public const string GetPOLineDollarAmountsReadyToBill = "getPOLineDollarAmountsReadyToBill";
            public const string GetPOLineDollarAmountsNotReadyToBill = "getPOLineDollarAmountsNotReadyToBill";
            public const string GenerateInvoiceAR = "generateInvoiceAR";
            public const string GenerateCL100InvoiceAR = "generateCL100InvoiceAR";
            public const string ResolveInvalidInvoices = "resolveInvalidInvoices";
            public const string TransmitInvoiceARDynamics = "TransmitInvoiceARDynamics";
            public const string TransmissionPreview = "TransmissionPreview";
            public const string InvoiceARFinalValidation = "invoiceARFinalValidation";

            public const string GetAllPOLineDollarAmountNoteCategories = "getAllPOLineDollarAmountNoteCategories";
            public const string GetPOLineDollarAmountNotes = "getPOLineDollarAmountNotes";
            public const string POLineDollarAmountNoteCreate = "poLineDollarAmountNoteCreate";

            public const string GetiSupplierPODataNotes = "getiSupplierPODataNotes";
            public const string iSupplierPODataNoteCreate = "iSupplierPODataNoteCreate";


            #endregion


            public const string OrderRoutePrefix = "api/orders";
            public const string MaterialRoutePrefix = "api/materials";
            public const string JobRoutePrefix = "api/jobs";
            public const string ItemRoutePrefix = "api/items";
            public const string ApprovalPrefix = "api/approval";
            public const string UserPrefix = "api/user";
            public const string ErfPrefix = "api/erf";
            public const string PurchaseOrderPrefix = "api/purchaseorder";
            public const string DashBoardPrefix = "api/dashboard";
            public const string PingPrefix = "api/ping";
            public const string FetchSubcontractors = "fetchSubcontractors";
            public const string SubcontractorSearch = "SubcontractorSearch/{query}";
            public const string FetchOrderReleaseJob = "fetchOrderReleaseJob/{paceNumber}/{releaseRequestType}";
            public const string ReleaseOrder = "releaseOrder";
            public const string MaterialRequest = "materialRequest";
            public const string FetchCallout = "fetchCallout/{calloutId}";
            public const string FetchCallouts = "fetchCallouts";
            public const string ApprovalSearch = "search/{searchCriteria}";
            public const string ApprovalSearchV2 = "searchV2";
            public const string GetVendorsJobApprovals = "getVendorsJobApprovals/";
            public const string UploadErfFile = "uploadErfFile";
            public const string FetchUser = "fetchUser";
            public const string RefreshApplicationSettings = "refreshApplicationSettings";
            public const string FetchOrderReleasesV2 = "fetchOrderReleasesv2";
            public const string FetchOrderReleaseDetail = "fetchOrderReleaseDetail/{orderId}";
            public const string FetchPaceNumbersBySiteId = "fetchPaceNumbersBySiteId/{siteId}";
            public const string FetchDashBoardInventoryItems = "fetchDashBoardInventoryItems";
            public const string FetchDashBoardInventoryItemByPartNumber = "fetchDashBoardInventoryItem/{partNumber}";
            public const string GenerateAllocatedInventoryItemReport = "generateAllocatedInventoryItemReport";
            public const string GenerateInTransitInventoryItemReport = "generateInTransitInventoryItemReport";
            public const string GenerateOpenPOInventoryItemReport = "generateOpenPOInventoryItemReport";
            public const string GenerateScopedInventoryItemReport = "generateScopedInventoryItemReport";
            public const string GenerateStagedInventoryItemReport = "generateStagedInventoryItemReport";
            public const string FetchOrderItemsPendingReview = "fetchOrderItemsPendingReview/{orderId}";
            public const string ReleasePendingOrder = "releasePendingOrder";
            public const string UpdatePendingSCMOrderDetails = "updatePendingSCMOrderDetails";


            // QB API GET/POST  DCW Code 06/24/2016
            // AP GET Methods
            public const string GetAPUsers = "api/GetAPUsers";

            public const string ReadyOrTransmittedAP = "api/ReadyOrTransmittedAP/{transmitted}";
            public const string ResetOrTransmittedAP = "api/ResetOrTransmittedAP";
            public const string GetPOList2Invoice = "api/GetPOList2Invoice/{relatedVendorPo}";
            public const string GetVendors = "api/GetVendors";
            public const string GetVendorsWithPOs = "api/GetVendorsWithPOs";
            public const string GetVendorByVendorCode = "api/GetVendorByVendorCode/{vendorCode}";
            public const string GetVendorNameByPONumber = "api/GetVendorNameByPONumber/{PONumber}";
            public const string GetInvoicesByPoNumberVendorId = "api/GetInvoicesByPoNumberVendorId/{vendorId}/{PONumber}";
            public const string POHeaderSearch = "api/POHeaderSearch/{POSearch}";
            public const string GetPODetailsByPOId = "api/GetPODetailsByPOId/{POId}";
            public const string GetVendorPOItems = "api/GetVendorPOItems/";
            public const string GetPOReceipts = "api/GetPOReceipts/{POId}";
            public const string GetPOReceiptsByPOId = "api/GetPOReceiptsByPOId/{POId}";
            public const string GetLineItemsByReceipt = "api/GetLineItemsByReceipt/{receiptNumber}";
            public const string GetLineItemsByPOId = "api/GetLineItemsByPOId/{POId}";
            public const string GetLineItemsByInvoiceId = "api/GetLineItemsByInvoiceId/{invoiceId}/{editMode}";
            public const string CheckVendorInvoiceNumber = "api/CheckVendorInvoiceNumber/{invoiceNumber}/{vendorId}";
            public const string GetPOwithInvoices = "api/GetPOwithInvoices/";
            public const string GetInvoiceById = "api/GetInvoiceById/{Id}/";
            // AP POST Methods
            public const string DoesPoLineItemHaveReceipt = "api/DoesPoLineItemHaveReceipt";
            public const string GetStatus = "api/GetStatus/{id}/";
            public const string GetDynamicsResult = "api/GetDynamicsResult/{id}/";
            public const string ReadyOrTransmittedByInvoiceNumberAP = "api/ReadyOrTransmittedByInvoiceNumberAP/";
            public const string ApplyInvoice = "api/ApplyInvoice";
            public const string CreateAPReport = "api/CreateAPReport";
            public const string RunTransmittedAPReport = "api/RunTransmittedAPReport";
            public const string UploadInvoicePDFFile = "api/UploadInvoicePDFFile";
            public const string DeleteInvoiceAP = "api/DeleteInvoiceAP";
            // Job Approval
            public const string ApproveBulkPO = "ApproveBulkPO";
            public const string RejectLineItems = "RejectLineItems";
            public const string RejectLineItemsV2 = "RejectLineItemsV2";
            public const string ApproveBulkPOV2 = "ApproveBulkPOV2";


            // AR GET Methods
            public const string LoadJobs = "api/LoadJobs";
            public const string GetPACECodeSummary = "api/GetPACECodeSummary";
            public const string BillableMilestones = "api/BillableMilestones";
            public const string LoadMilestoneInvoices = "api/LoadMilestoneInvoices/{casprJobCode}";

            public const string BillablePACEJobTasks = "api/BillablePACEJobTasks";
            public const string LoadPACEJobTaskInvoices = "api/LoadPACEJobTaskInvoices/{casprJobCode}";

            public const string CreateARReport = "api/CreateARReport";
            public const string ReadyOrTransmittedAR = "api/ReadyOrTransmittedAR/{transmitted}";

            public const string ReadyOrTransmittedByInvoiceNumberAR = "api/ReadyOrTransmittedByInvoiceNumberAR";

            public const string ResetOrTransmittedAR = "api/ResetOrTransmittedAR";
            public const string GetInvoiceJobs = "api/GetInvoiceJobs/{transmitted}";
            public const string GetInvoices = "api/GetInvoices"; // jobs,bool creditmemo,bool transmitted
            public const string GetInvoiceLineItems = "api/GetInvoiceLineItems/{casprJobCode}";
            public const string GetInvoice = "api/GetInvoice/{recordId}";
            public const string GetInvoiceCancel = "api/GetInvoiceCancel";
            public const string LineItemsByJob = "api/LineItemsByJob/{casprJobCode}";
            // AR POST Methods
            public const string ApplyCreditMemo = "api/ApplyCreditMemo";
            public const string GenerateInvoiceNumber = "api/GenerateInvoiceNumber/{casprJobCode}";
            public const string BuildReportPublishInvoicePACE = "api/BuildReportPublishInvoicePACE";
            public const string BuildReportPublishInvoice = "api/BuildReportPublishInvoice";

            public const string LoadPriceLevel = "api/LoadPriceLevel";
            public const string LoadVendorPOLineItemsToInvoice = "api/LoadVendorPOLineItemsToInvoice";
            public const string LoadVendorPOListToInvoice = "api/LoadVendorPOListToInvoice";
            public const string Jobs = "api/Jobs";
            public const string CI006InvoicesCreatedCSV = "ARCI006InvoicesCreatedCSV";
            public const string CI006InvoicesCreated = "CI006InvoicesCreated";
            // AR GET Methods
            public const string GetARInvoicePDF = "api/GetARInvoicePDF";
            public const string LoadVendors = "api/LoadVendors";
            public const string GetPOsByVendor = "api/GetPOsByVendor/{vendorId}";
            public const string GetPODetailsByPONumber = "api/GetPODetailsByPONumber/{relatedVendorPo}";

            // Material Invoices GET Methods
            public const string GetMaterialInvoices = "api/GetMaterialInvoices/{searchCriteria}";
            public const string GetOrdersByInvoiceId = "api/GetOrdersByInvoiceId/{invoiceId}";

            // Material Invoices POST Methods
            public const string GetInvoicePDF = "api/GetInvoicePDF";
            public const string DownloadZipFile = "api/DownloadZipFile";


            // Job GET Methods
            public const string FetchJobNumbers = "fetchJobNumbers";
            public const string FetchJobNumbersBySiteId = "fetchJobNumbersBySiteId/{siteId}";
            public const string FetchJob = "fetchJob/{jobNumber}";
            public const string FetchJobImportErrors = "fetchJobImportErrors/{jobNumber}";
            public const string SyncJobs = "syncJobs";

            // Site GET Methods
            public const string FetchSiteNumbers = "fetchSiteNumbers";
            public const string SiteIdSearch = "siteIdSearch/{query}";

            // Part GET Methods
            public const string FetchPartNumbers = "fetchPartNumbers";
            public const string FetchPart = "fetchPart/{partNumber}";

            // MRF GET Methods
            public const string RequestMaterial = "requestMaterial";
            public const string FetchMaterialRequests = "fetchMaterialRequests/{jobNumber}";
            public const string FetchMaterialRequestsByConstructionManager = "fetchMaterialRequestsByConstructionManager/{userId}";
            public const string FetchJobNumbersByScopingLead = "fetchJobNumbersByScopingLead/{userId}";

            // MRF POST Methods
            public const string FetchMaterialRequestsByScopingLead = "fetchMaterialRequestsByScopingLead";

            // Purchase Order Methods
            public const string FetchVendorPrices = "fetchVendorPrices";
            public const string FetchWareHouses = "fetchWareHouses";
            public const string FetchScopedLineItems = "fetchScopedJobLineItems";
            public const string SendForApproval = "sendForApproval";
            public const string ItemSearch = "itemSearch";
            public const string GetVendorPricesByItemId = "getVendorPricesByItemId";
            public const string SavePo = "savePo";
 	        public const string SaveRequisition = "saveRequisition";
            public const string ResendPO = "resendPO";

            public const string GetPagedRequisitions = "getPagedRequisitions";
            public const string DeleteRequisition = "deleteRequisition";
            public const string ApproveRequisition = "approveRequisition";
            // Transfer Warehouse GET
            public const string GetWarehouses = "api/GetWarehouses";

            public const string GetTransferHistory = "api/GetTransferHistory";
            // Transfer Warehouse POST
            public const string GetItemsByWarehouseCode = "api/GetItemsByWarehouseCode";
            public const string FetchActiveProjects = "fetchActiveProjects";
            public const string FetchtVendortNames = "fetchtVendortNames";
            public const string FetchScopeStatuses = "fetchScopeStatuses";
            public const string CreateTransferOrder = "api/CreateTransferOrder";

            public const string KeepAlive = "keepalive";
            public const string CancelOrder = "cancelOrder/{orderNumber}";
            public const string FetchBomByScope = "FetchBomByScope";

            public const string GetPoReport = "getPoReport";
            public const string DownloadPoReport = "downloadPoReport";
            public const string DownloadiSupplierPOLineDollarReport = "downloadiSupplierPOLineDollarReport";

            public const string ScheduleRequestsPrefix = "api/schedulerequests";
            public const string FetchScheduleRequestsByPaceNumbers = "fetchScheduleRequestsByPaceNumbers";
            public const string SaveScheduleRequest = "saveScheduleRequest";
            public const string SaveScheduleRequests = "saveScheduleRequests";
            public const string RequestPrioritizationPrefix = "api/requestprioritization";
            public const string RequestPrioritizationSearch = "search";
            public const string SaveBlockedPickupDateTimeSlot = "saveBlockedPickupDateTimeSlot";
            public const string SaveBlockedPickupDateTimeSlots = "saveBlockedPickupDateTimeSlots";
            public const string GetBlockedPickupTimeSlots = "getBlockedPickupTimeSlots";
            public const string FetchCalendarData = "fetchCalendarData";
            public const string FetchCalendarDataV2 = "v2/fetchCalendarData";
            public const string FetchCalendarAvailableDateTimeSlot =  "fetchCalendarAvailableDateTimeSlot";
            public const string SendOrdersToLogFire = "sendOrdersToLogFire";

            //ApplicationConfiguration
            public const string Administration = "api/administration/FetchApplicationConfigurationItems";
            public const string SaveConfiguration = "PostConfiguration";
            public const string UpdateConfiguration = "PutConfiguration";
            public const string FetchApplicationConfigurationItems = "api/fetchApplicationConfigurationItems";
            public const string FetchApplicationConfigurationItem = "api/fetchApplicationConfigurationItems/{paramaterName}";
            public const string putApplicationConfigurationItem = "api/administration/putApplicationConfigurationItem";
            public const string postApplicationConfigurationItem = "api/administration/postApplicationConfigurationItem";
            public const string GetNORADConfiguration = "api/administration/getNORADConfiguration";
            public const string TestNORADConfiguration = "api/administration/testNORADConfiguration";
            public const string ReportOOMTaskProjectNumberListModified = "api/administration/reportOOMTaskProjectNumberListModified";

            public const string GetRevisedPurchaseOrders = "getRevisedPurchaseOrders";
            public const string ApproveRevisedPurchaseOrders = "approveRevisedPurchaseOrders";
            public const string RejectRevisedPurchaseOrders = "rejectRevisedPurchaseOrders";
            public const string SavePoRevision = "savePoRevision";
            public const string GetCurrentPORevisionLines = "getCurrentPORevisionLines/{poNumber}";
            public const string FetchJobNumbersByPOLineId = "fetchJobNumbersByPOLineId";
            public const string FetchJobNumbersByCancelledPO = "fetchJobNumbersByCancelledPO";
            public const string CancelPurchaseOrders = "cancelPurchaseOrders";
            public const string GetPoLineItemSiteIds = "getPoLineItemSiteIds";
            public const string GetPurchaseOrderDetails = "getPurchaseOrderDetails";
            public const string RollbackApprovedPurchaseOrderRevision = "rollbackApprovedPurchaseOrderRevision";

            public const string InsertNORADPOLineDollarAmount = "insertNORADPOLineDollarAmount";
        }
    }
}
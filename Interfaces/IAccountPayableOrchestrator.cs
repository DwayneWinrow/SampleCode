using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CertusView.OP.WebClient.Models;
using CertusView.IMEX.Contract.DTO;

namespace CertusView.OP.WebClient.Interfaces
{
    public interface IAccountPayableOrchestrator
    {
        string DoesPoLineItemHaveReceipt(List<string> poHeaders);
        string GetDynamicsProxyStatus(string id);
        string GetDynamicProxyResults(string id);
        bool DeleteInvoiceAP(InvoiceAPDTO transmitInvoices);
        bool DeleteReceiptInvoiceAP(InvoiceAPDTO transmitInvoices);
        List<InvoiceAPDTO> GetAPUsers();
        InvoiceAPDTO CheckVendorInvoiceNumber(InvoiceAPDTO invoice);
        bool SavePOReceiptInvoiceAP(POReceiptInvoiceAP poReceiptInvoiceAP);
        List<InvoiceAPDTO> GetInvoicesByPoNumberVendorId(string vendorId, string PONumber);
        bool ResetOrTransmittedInvoiceAP(TransmitInvoiceAPDTO transmitInvoices, string ADName, string fromEmailAddress);
        bool SaveInvoiceAP(InvoiceAPDTO invoice);
        bool UploadInvoiceFile(InvoiceFileDTO invoiceFile);
        byte[] generateAPTransReportPDF(List<InvoiceAPDTO> invoices, string ADName, bool Adhoc, out string strFileName, out string dateRun);
        void ApplyInvoice(PublishInvoiceAPDTO oPublishInvoice, string ADName);
        List<InvoiceAPDTO> GetReadyOrTransmittedAP(TransAPDTO transmitAP);
        List<InvoiceAPDTO> GetReadyOrTransmittedAP(bool transmitted, string ADName);
        InvoiceAPDTO GetInvoiceAP(int recordId);
        List<POHeaderDTO> GetPOsByVendor(string vendorId);
        POHeaderDTO GetPONumberByPOId(string POId);
        List<POHeaderDTO> POHeaderSearch(string PoHeader);
        List<POLineItemDTO> GetPODetailsByPOId(string POId); 
        //List<PoHeaderPoLineItemReceiptsViewDTO> GetPOReceipts(string POId);
        List<POReceiptDTO> GetPOReceiptsByPOId(string POId);
        List<PoLineItemReceiptsViewDTO> GetLineItemsByReceipt(string POId);
        List<PoLineItemReceiptsViewDTO> GetLineItemsByPONumber(string poNumber);
        List<PoLineItemReceiptsViewDTO> GetLineItemsByInvoiceId(string invoiceId, bool editMode);
        List<PoLineItemReceiptsViewDTO> GetLineItemsByPOId(string POId); 
        List<VendorDTO> GetVendors();
        List<VendorDTO> GetVendorsWithPOs();
        List<InvoiceAPDTO> GetPOwithInvoices();
        VendorDTO GetVendorNameByPONumber(string poNumber);
        InvoiceAPDTO GetInvoiceById(string Id);
        VendorDTO GetVendorByVendorCode(string vendorCode);
        VendorDTO GetVendor(string vendorId);
        POHeaderDTO GetPOByPONumber(string poNumber);
      //  List<POLineItemDTO> GetLineItemByVendorId(string vendorId);
        //QbAddRecord QbAddRecord(QuickBase.IMEX.Contract.Enums.TableName tableName, List<QbEditFieldValues> fieldList);
        //List<QbPriceLevel> QbGetPriceLevel(string strRelatedVendor, string strRelatedItem);
        //void QbEditFields(QuickBase.IMEX.Contract.Enums.TableName tableName, long recordId, List<QbEditFieldValues> fieldList);
    }
}
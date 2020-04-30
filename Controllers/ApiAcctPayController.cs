using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CertusView.OP.WebClient.Common;
using CertusView.OP.WebClient.Interfaces;
using CertusView.OP.WebClient.Models;
using System.Net.Http.Headers;

namespace CertusView.OP.WebClient.Controllers.AccountPayable
{
    public class ApiAcctPayController : BaseApiController
    {
        public IAccountPayableOrchestrator _accountPayableOrchestrator { get; private set; }
        public ApiAcctPayController(IAccountPayableOrchestrator accountPayableOrchestrator)
        {
           // Debug.Assert(null != _accountPayableOrchestrator);
            _accountPayableOrchestrator = accountPayableOrchestrator;
        }
        [Route(Constants.Routes.ApplyInvoice)]
        [HttpPost]
        public void ApplyInvoice(PublishInvoiceAPDTO oPublishInvoice)
        {
            string ADName = GetAdName();
            _accountPayableOrchestrator.ApplyInvoice(oPublishInvoice, ADName);
        }
        [Route(Constants.Routes.DoesPoLineItemHaveReceipt)]
        [HttpPost]
        public string DoesPoLineItemHaveReceipt(List<string> POHeaderDTOs)
        {
            return _accountPayableOrchestrator.DoesPoLineItemHaveReceipt(POHeaderDTOs);
        }

        [Route(Constants.Routes.GetStatus)]
        [HttpGet]
        public string GetTaskStatus(string id)
        {
            return _accountPayableOrchestrator.GetDynamicsProxyStatus(id);
        }

        [Route(Constants.Routes.GetDynamicsResult)]
        [HttpGet]
        public string GetTaskResults(string id)
        {
            return _accountPayableOrchestrator.GetDynamicProxyResults(id);
        }

        [Route(Constants.Routes.ReadyOrTransmittedAP)]
        [HttpGet]
        public List<InvoiceAPDTO> ReadyOrTransmittedAP(bool transmitted)
        {
            string ADName = "ALL";
            if (!transmitted)
            {
                ADName = GetAdName();
            }
            return _accountPayableOrchestrator.GetReadyOrTransmittedAP(transmitted, ADName);  
        }
        [Route(Constants.Routes.ReadyOrTransmittedByInvoiceNumberAP)]
        [HttpPost]
        public List<InvoiceAPDTO> ReadyOrTransmittedByInvoiceNumberAP(TransAPDTO transAPDTO)
        {
            string ADName = GetAdName();
            if (transAPDTO.Transmitted)
            {
                transAPDTO.UserId = string.IsNullOrEmpty(transAPDTO.UserId) ? "ALL" : transAPDTO.UserId;
            }
            else
            {
                transAPDTO.UserId = string.IsNullOrEmpty(transAPDTO.UserId) ? ADName : transAPDTO.UserId;
            }
            return _accountPayableOrchestrator.GetReadyOrTransmittedAP(transAPDTO);
        }
        [Route(Constants.Routes.UploadInvoicePDFFile)]
        [HttpPost]
        public void UploadInvoicePDFFile(InvoiceFileDTO invoiceFile)
        {
            _accountPayableOrchestrator.UploadInvoiceFile(invoiceFile);
        }
        [Route(Constants.Routes.RunTransmittedAPReport)]
        [HttpPost]
        public HttpResponseMessage RunTransmittedAPReport(TransmittedReportDTO transmittedReport)
        {
            //string ADName = GetAdName();
            string ADName = "ALL";
            HttpResponseMessage result = null;
            string strFileName = string.Empty;
            string dateRun = string.Empty;
            List<InvoiceAPDTO> invoiceToBeQueried = new List<InvoiceAPDTO>();
            DateTime tempTest = new DateTime();
            //if(transmittedReport.beginDate=="1/1/0001")
            try
            {
                List<InvoiceAPDTO> invoices = _accountPayableOrchestrator.GetReadyOrTransmittedAP(true, ADName);
                if (!string.IsNullOrEmpty(transmittedReport.vendorId) && !string.IsNullOrEmpty(transmittedReport.poNumber))
                {
                    if (transmittedReport.beginDate != tempTest && transmittedReport.endDate != tempTest)
                    {
                        invoiceToBeQueried = (from InvoiceAPDTO invoice in invoices
                                              where invoice.vendorId == transmittedReport.vendorId
                                              && invoice.poNumber == transmittedReport.poNumber
                                              && transmittedReport.beginDate <= invoice.TransmitDate
                                              && invoice.TransmitDate <= transmittedReport.endDate
                                              select invoice).ToList();
                    }
                    else if (transmittedReport.beginDate != tempTest && transmittedReport.endDate == tempTest)
                    {
                        invoiceToBeQueried = (from InvoiceAPDTO invoice in invoices
                                              where invoice.vendorId == transmittedReport.vendorId
                                              && invoice.poNumber == transmittedReport.poNumber
                                              // && transmittedReport.beginDate <= invoice.TransmitDate
                                              && transmittedReport.beginDate <= invoice.TransmitDate
                                              //&& invoice.TransmitDate <= transmittedReport.endDate
                                              select invoice).ToList();
                    }
                    else if (transmittedReport.beginDate == tempTest && transmittedReport.endDate != tempTest)
                    {
                        invoiceToBeQueried = (from InvoiceAPDTO invoice in invoices
                                              where invoice.vendorId == transmittedReport.vendorId
                                              && invoice.poNumber == transmittedReport.poNumber
                                             // && transmittedReport.beginDate <= invoice.TransmitDate
                                              && invoice.TransmitDate <= transmittedReport.endDate
                                              select invoice).ToList();
                    }
                    else if (transmittedReport.beginDate == tempTest && transmittedReport.endDate == tempTest)
                    {
                        invoiceToBeQueried = (from InvoiceAPDTO invoice in invoices
                                              where invoice.vendorId == transmittedReport.vendorId
                                               && invoice.poNumber == transmittedReport.poNumber
                                              //&& invoice.poNumber == transmittedReport.poNumber
                                              // && transmittedReport.beginDate <= invoice.TransmitDate
                                              // && invoice.TransmitDate <= transmittedReport.endDate
                                              select invoice).ToList();
                    }
                }
                else if (!string.IsNullOrEmpty(transmittedReport.vendorId) && string.IsNullOrEmpty(transmittedReport.poNumber))
                {
                    if (transmittedReport.beginDate != tempTest && transmittedReport.endDate != tempTest)
                    {
                        invoiceToBeQueried = (from InvoiceAPDTO invoice in invoices
                                              where invoice.vendorId == transmittedReport.vendorId
                                              && transmittedReport.beginDate <= invoice.TransmitDate
                                              && invoice.TransmitDate <= transmittedReport.endDate
                                              select invoice).ToList();
                    }
                    else if (transmittedReport.beginDate == tempTest && transmittedReport.endDate != tempTest)
                    {
                        invoiceToBeQueried = (from InvoiceAPDTO invoice in invoices
                                              where invoice.vendorId == transmittedReport.vendorId
                                             // && transmittedReport.beginDate <= invoice.TransmitDate
                                              && invoice.TransmitDate <= transmittedReport.endDate
                                              select invoice).ToList();
                    }
                    else if (transmittedReport.beginDate != tempTest && transmittedReport.endDate == tempTest)
                    {
                        invoiceToBeQueried = (from InvoiceAPDTO invoice in invoices
                                              where invoice.vendorId == transmittedReport.vendorId
                                              && transmittedReport.beginDate <= invoice.TransmitDate
                                              //&& invoice.TransmitDate <= transmittedReport.endDate
                                              select invoice).ToList();
                     }
                    else if (transmittedReport.beginDate == tempTest && transmittedReport.endDate == tempTest)
                    {
                        invoiceToBeQueried = (from InvoiceAPDTO invoice in invoices
                                              where invoice.vendorId == transmittedReport.vendorId
                                             // && transmittedReport.beginDate <= invoice.TransmitDate
                                             // && invoice.TransmitDate <= transmittedReport.endDate
                                              select invoice).ToList();
                    }
                }
                else if (string.IsNullOrEmpty(transmittedReport.vendorId) && !string.IsNullOrEmpty(transmittedReport.poNumber))
                {
                    if (transmittedReport.beginDate != tempTest && transmittedReport.endDate != tempTest)
                    {
                        invoiceToBeQueried = (from InvoiceAPDTO invoice in invoices
                                              where invoice.poNumber == transmittedReport.poNumber
                                              && transmittedReport.beginDate <= invoice.TransmitDate
                                              && invoice.TransmitDate <= transmittedReport.endDate
                                              select invoice).ToList();
                    }
                    else if (transmittedReport.beginDate == tempTest && transmittedReport.endDate != tempTest)
                    {
                        invoiceToBeQueried = (from InvoiceAPDTO invoice in invoices
                                              where invoice.poNumber == transmittedReport.poNumber
                                             // && transmittedReport.beginDate <= invoice.TransmitDate
                                               && invoice.TransmitDate <= transmittedReport.endDate
                                              select invoice).ToList();
                    }
                    else if (transmittedReport.beginDate != tempTest && transmittedReport.endDate == tempTest)
                    {
                        invoiceToBeQueried = (from InvoiceAPDTO invoice in invoices
                                              where invoice.poNumber == transmittedReport.poNumber
                                              && transmittedReport.beginDate <= invoice.TransmitDate
                                              //&& invoice.TransmitDate <= transmittedReport.endDate
                                              select invoice).ToList();
                    }
                    else if (transmittedReport.beginDate == tempTest && transmittedReport.endDate == tempTest)
                    {
                        invoiceToBeQueried = (from InvoiceAPDTO invoice in invoices
                                              where invoice.poNumber == transmittedReport.poNumber
                                             // && transmittedReport.beginDate <= invoice.TransmitDate
                                             // && invoice.TransmitDate <= transmittedReport.endDate
                                              select invoice).ToList();
                    }
                }
                else if (string.IsNullOrEmpty(transmittedReport.vendorId) && string.IsNullOrEmpty(transmittedReport.poNumber))
                {
                   
                    if (transmittedReport.beginDate != tempTest && transmittedReport.endDate != tempTest)
                    {
                        invoiceToBeQueried = (from InvoiceAPDTO invoice in invoices
                                              where /*invoice.SiteId == transmittedReport.siteId
                                          && invoice.vendorId == transmittedReport.vendorId 
                                          && */ transmittedReport.beginDate <= invoice.TransmitDate
                                              && invoice.TransmitDate <= transmittedReport.endDate
                                              select invoice).ToList();
                    }
                    else if (transmittedReport.beginDate == tempTest && transmittedReport.endDate != tempTest)
                    {
                        invoiceToBeQueried = (from InvoiceAPDTO invoice in invoices
                                              where /*invoice.SiteId == transmittedReport.siteId
                                          && invoice.vendorId == transmittedReport.vendorId */
                                             // transmittedReport.beginDate <= invoice.TransmitDate
                                               invoice.InvoiceDate <= transmittedReport.endDate
                                              select invoice).ToList();
                    }
                    else if (transmittedReport.beginDate != tempTest && transmittedReport.endDate == tempTest)
                    {
                        invoiceToBeQueried = (from InvoiceAPDTO invoice in invoices
                                              where /*invoice.SiteId == transmittedReport.siteId
                                          && invoice.vendorId == transmittedReport.vendorId 
                                          && */ 
                                              transmittedReport.beginDate <= invoice.InvoiceDate 
                                           //   && 
                                          //  invoice.InvoiceDate <= transmittedReport.endDate
                                              select invoice).ToList();
                    }
                    else if (transmittedReport.beginDate == tempTest && transmittedReport.endDate == tempTest)
                    {
                        invoiceToBeQueried = invoices;
                    }
                }
                byte[] bytesReport = _accountPayableOrchestrator.generateAPTransReportPDF(invoiceToBeQueried, ADName, true, 
                    out strFileName, out dateRun);
                if (bytesReport != null && !string.IsNullOrEmpty(strFileName))
                {
                    result = Request.CreateResponse(HttpStatusCode.OK);
                    result.Content = new ByteArrayContent(bytesReport);
                    result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                    result.Content.Headers.ContentDisposition.FileName = strFileName;
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                return result;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
        [Route(Constants.Routes.CreateAPReport)]
        [HttpPost]
        public HttpResponseMessage CreateAPReport(TransmitInvoiceAPDTO transmitInvoices)
        {
            string ADName = GetAdName();
            HttpResponseMessage result = null;
            string strFileName = string.Empty;
            string dateRun = string.Empty;
            try
            {
                byte[] bytesReport = _accountPayableOrchestrator.generateAPTransReportPDF(transmitInvoices.Invoices2Transmit, ADName, false, out strFileName, out dateRun);
                if (bytesReport != null && !string.IsNullOrEmpty(strFileName))
                {
                    result = Request.CreateResponse(HttpStatusCode.OK);
                    result.Content = new ByteArrayContent(bytesReport);
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    result.Content.Headers.ContentDisposition.FileName = strFileName;
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                return result;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
        [Route(Constants.Routes.GetPOwithInvoices)]
        [HttpGet]
        public List<InvoiceAPDTO> GetPOwithInvoices()
        {
            return _accountPayableOrchestrator.GetPOwithInvoices();
        }
        [Route(Constants.Routes.GetVendorNameByPONumber)]
        [HttpGet]
        public VendorDTO GetVendorNameByPONumber(string poNumber)
        {
            return _accountPayableOrchestrator.GetVendorNameByPONumber(poNumber);
        }
        [Route(Constants.Routes.GetInvoiceById)]
        [HttpGet]
        public InvoiceAPDTO GetInvoiceById(string Id)
        {
            return _accountPayableOrchestrator.GetInvoiceById(Id);
        }        
        [Route(Constants.Routes.POHeaderSearch)]
        [HttpGet]
        public List<POHeaderDTO> POHeaderSearch(string POSearch)
        {
            return _accountPayableOrchestrator.POHeaderSearch(POSearch);
        }
        [Route(Constants.Routes.GetVendorsWithPOs)]
        [HttpGet]
        public List<VendorDTO> GetVendorsWithPOs()
        {
            return _accountPayableOrchestrator.GetVendorsWithPOs();
           // var rtnVendors = lstVendors.OrderBy(x => x.VendorName).ToList();
            //return lstVendors;
        }
        [Route(Constants.Routes.GetAPUsers)]
        [HttpGet]
        public List<InvoiceAPDTO> GetAPUsers()
        {
            return _accountPayableOrchestrator.GetAPUsers(); ;
        }
        [Route(Constants.Routes.GetInvoicesByPoNumberVendorId)]
        [HttpGet]
        public List<InvoiceAPDTO> GetInvoicesByPoNumberVendorId(string vendorId,string PONumber)
        {
            return _accountPayableOrchestrator.GetInvoicesByPoNumberVendorId(vendorId, PONumber);
        }
        [Route(Constants.Routes.GetVendorByVendorCode)]
        [HttpGet]
        public VendorDTO GetVendorByVendorCode(string vendorCode)
        {
            return _accountPayableOrchestrator.GetVendorByVendorCode(vendorCode);
        }
        [Route(Constants.Routes.GetVendors)]
        [HttpGet]
        public List<VendorDTO> GetVendors(string vendorCode)
        {
            return _accountPayableOrchestrator.GetVendors();
        }
        [Route(Constants.Routes.GetPOsByVendor)]
        [HttpGet]
        public List<POHeaderDTO> GetPOsByVendor(string vendorId)
        {
            var lstPOHeader = _accountPayableOrchestrator.GetPOsByVendor(vendorId);
            return lstPOHeader;
        }
        [Route(Constants.Routes.GetPODetailsByPOId)]
        [HttpGet]
        public List<POLineItemDTO> GetPODetailsByPOId(string POId)
        {
            decimal decQuantity = 0;
            var lstPolineItems = _accountPayableOrchestrator.GetPODetailsByPOId(POId);
            foreach (var lstPolineItem in lstPolineItems)
            {
                decQuantity = 0;
                // decimal.TryParse(lstJoblineItem.Quantity, out decQuantity);
                lstPolineItem.TotalPOAmount = lstPolineItem.UnitCost * lstPolineItem.QtyReceived;
                lstPolineItem.NewOrderQty = lstPolineItem.QtyOrdered;
                lstPolineItem.NewUnitCost = lstPolineItem.UnitCost;
                lstPolineItem.Type = "Material";
            }
            // add total price
            //rtnPOLineItems = lstPOLineItems;
            return lstPolineItems;
        }
        //[Route(Constants.Routes.GetPOReceiptsByPOId)]
        //[HttpGet]
        //public List<POReceiptDTO> GetPOReceiptsByPOId(string POId)
        //{
        //    //return _accountPayableOrchestrator.GetPOReceipts(POId);
        //    return _accountPayableOrchestrator.GetPOReceiptsByPOId(POId);
        //    //return lstPolineItems;
        //}
        [Route(Constants.Routes.GetLineItemsByReceipt)]
        [HttpGet]
        public List<PoLineItemReceiptsViewDTO> GetLineItemsByReceipt(string receiptNumber)
        {
            var lstPolineItems = _accountPayableOrchestrator.GetLineItemsByReceipt(receiptNumber);
            foreach (var lstPolineItem in lstPolineItems)
            {
                lstPolineItem.TotalPOAmount = lstPolineItem.UnitCost * lstPolineItem.QtyReceiptReceived;
                lstPolineItem.Type = "Material";
            }
            // add total price
            //rtnPOLineItems = lstPOLineItems;
            return lstPolineItems;
        }
        [Route(Constants.Routes.GetLineItemsByPOId)]
        [HttpGet]
        public List<PoLineItemReceiptsViewDTO> GetLineItemsByPOId(string POId)
        {
            //var lstPolineItems = _accountPayableOrchestrator.GetLineItemsByPOId(POId);
            var lstPolineItems = _accountPayableOrchestrator.GetLineItemsByPONumber(POId);
            return lstPolineItems;
        }
        [Route(Constants.Routes.GetLineItemsByInvoiceId)]
        [HttpGet]
        public List<PoLineItemReceiptsViewDTO> GetLineItemsByInvoiceId(string invoiceId, bool editMode)
        {
            var lstPolineItems = _accountPayableOrchestrator.GetLineItemsByInvoiceId(invoiceId, editMode);
            return lstPolineItems;
        }
        [Route(Constants.Routes.CheckVendorInvoiceNumber)]
        [HttpGet]
        public bool CheckVendorInvoiceNumber(string invoiceNumber, string vendorId)
        {
            bool booReturn = false;
            InvoiceAPDTO invoice = new InvoiceAPDTO();
            invoice.InvoiceNumber = invoiceNumber;
            invoice.vendorId = vendorId;
            var invoiceTest =_accountPayableOrchestrator.CheckVendorInvoiceNumber(invoice);
            if(invoiceTest != null)
            {
                booReturn = true;
            }
            return booReturn;
        }
        [Route(Constants.Routes.DeleteInvoiceAP)]
        [HttpPost]
        public void DeleteInvoiceAP(InvoiceAPDTO transmitInvoices)
        {
            //_accountPayableOrchestrator.DeleteInvoiceAP(transmitInvoices);
            transmitInvoices.lastUpdateUserId = GetAdName();
            _accountPayableOrchestrator.DeleteReceiptInvoiceAP(transmitInvoices);
        }
        [Route(Constants.Routes.ResetOrTransmittedAP)]
        [HttpPost]
        public void ResetOrTransmittedAP(TransmitInvoiceAPDTO transmitInvoices)
        {
            string ADName = GetAdName();
            var user = (ShibbolethIdentity)User.Identity;
            string emailAddress = user.EmailAddress;
            //if (string.IsNullOrEmpty(emailAddress) && ADName == "HH0026")
            //{
            //    emailAddress = "henry.hanley@anscollc.com";
            //}
            //else if (string.IsNullOrEmpty(emailAddress) && ADName == "CR0069")
            //{
            //    emailAddress = "christine.riddell@anscollc.com";
            //}
            //else
            //{
            //    emailAddress = "christine.riddell@anscollc.com";
            //}
            _accountPayableOrchestrator.ResetOrTransmittedInvoiceAP(transmitInvoices, ADName, emailAddress);
        }
    }
}

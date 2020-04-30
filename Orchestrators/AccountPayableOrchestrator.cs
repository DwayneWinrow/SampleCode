using CertusView.OP.WebClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CertusView.IMEX.Contract.DTO;
using CertusView.Dynamics.IMEX.Contract.Interfaces;
using CertusView.Operations.IMEX.Contract.Interfaces;
using System.Diagnostics;
using AutoMapper;
using CertusView.OP.WebClient.Models;
using System.Configuration;
using System.Threading.Tasks;
using System.IO;
using CertusView.IMEX.Core.Interface;
using Microsoft.Reporting.WebForms;
using System.Text;
using System.Collections;
using CertusView.Dynamics.Common.Library;

namespace CertusView.OP.WebClient.Orchestrators.AccountPayable
{
    /// <summary>
    /// Handles OrderRelease API business logic
    /// </summary>
    public class AccountPayableOrchestrator : IAccountPayableOrchestrator
    {
        #region Member Variables

        public IOperationsManager _operationsManager { get; private set; }
       
        private IMapper _mapper;
        //public IQuickBaseManager _qbManager { get; private set; }
        public ILogger _logger { get; private set; }
        private readonly string _appName = ConfigurationManager.AppSettings.Get("QB-APPNAME");
        private string TaxCode = ConfigurationManager.AppSettings.Get("TaxCode");
        private string FreightCode = ConfigurationManager.AppSettings.Get("FreightCode");
    

        #endregion//Member Variables

        #region Methods

        /// <summary>
        /// Default dependency constructor
        /// </summary>
        /// <param name="operationsManager"></param>
        public AccountPayableOrchestrator(IOperationsManager operationsManager, ILogger logger)
        {
            Debug.Assert(null != operationsManager);
            this._operationsManager = operationsManager;
            this._logger = logger;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Operations.IMEX.Contract.DTO.InvoiceAPDTO, Models.InvoiceAPDTO>().ReverseMap();
                cfg.CreateMap<POLineItem, POLineItemDTO>();
                cfg.CreateMap<Vendor, VendorDTO>();
                cfg.CreateMap<POHeader, POHeaderDTO>();
                //cfg.CreateMap<JobLineItem, JobLineItemDTO>();
                //cfg.CreateMap<PoHeaderPoLineItemReceiptsView, PoHeaderPoLineItemReceiptsViewDTO>();
                cfg.CreateMap<PoLineItemReceiptsView, PoLineItemReceiptsViewDTO>();
                cfg.CreateMap<POReceipt, POReceiptDTO>()
                 .ForMember(dest => dest.Invoiced, opt => opt.MapFrom(src => src.Invoiced))
                 .ForMember(dest => dest.ReceiptNumber, opt => opt.MapFrom(src => src.ReceiptNumber))
                 .ForMember(dest => dest.PoLineItemId, opt => opt.MapFrom(src => src.PoLineItemId))
                 .ForMember(dest => dest.QtyInvoiced, opt => opt.MapFrom(src => src.QtyInvoiced))
                 .ForMember(dest => dest.QtyReceived, opt => opt.MapFrom(src => src.QtyReceived)).ReverseMap();
            });
            _mapper = config.CreateMapper();
        }
        public void ApplyInvoice(PublishInvoiceAPDTO oPublishInvoice, string ADName)
        {
            int starttime = Environment.TickCount;
            _logger.Debug("Apply Invoice started at " + starttime);
            List<PoLineItemReceiptsViewDTO> lstPolineItems = new List<PoLineItemReceiptsViewDTO>();
            List<PoLineItemReceiptsViewDTO> tmpPoLineItems = new List<PoLineItemReceiptsViewDTO>();
            lstPolineItems = oPublishInvoice.PoLineItemReceiptsResults;

            Models.InvoiceAPDTO invoiceAP = new Models.InvoiceAPDTO();
            Models.InvoiceAPDTO invoiceAPSaved = new Models.InvoiceAPDTO();
            POReceiptInvoiceAP poReceiptInvoiceAP = null;
            try
            {
                invoiceAP.createUserId = ADName;
                invoiceAP.lastUpdateUserId = ADName;
                invoiceAP.InvoiceNumber = oPublishInvoice.InvoiceNumber;
                invoiceAP.InvoiceAmount = oPublishInvoice.PoAmount;
                invoiceAP.vendorId = oPublishInvoice.VendorCode;
                invoiceAP.VendorName = oPublishInvoice.VendorName;
                invoiceAP.CustomerInvoiceDate = oPublishInvoice.InvoiceDate;
                //POHeaderDTO oPOHeader = GetPONumberByPOId(oPublishInvoice.PoNumber);
                invoiceAP.poNumber = oPublishInvoice.PoNumber;
                invoiceAP.InvoiceDate = oPublishInvoice.InvoiceDate;
                invoiceAP.FreightAmount = (decimal)oPublishInvoice.FreightAmount;
                invoiceAP.TaxAmount = (decimal)oPublishInvoice.TaxAmount;
                if(oPublishInvoice.resetReceiptInvoiceData)
                {
                    invoiceAP.Id = oPublishInvoice.InvoiceId; // removing the old invoice
                    _logger.Debug("DeleteReceiptInvoiceAP started at" + Environment.TickCount);
                    DeleteReceiptInvoiceAP(invoiceAP);

                }
                invoiceAP.Id = null; //forcing a new invoice id
                if (SaveInvoiceAP(invoiceAP))
                {
                    _logger.Debug("CheckVendorInvoiceNumber started");
                    invoiceAPSaved = CheckVendorInvoiceNumber(invoiceAP); // minimize calls
                    _logger.Debug("CheckVendorInvoiceNumber completed");
                    Dictionary<string, decimal> dPartNumbers = new Dictionary<string, decimal>();
                    Dictionary<string, decimal> dPartNumbersQtyInvoiced = new Dictionary<string, decimal>();
                    foreach (var lstPolineItem in oPublishInvoice.PoLineItemReceiptsResults)
                    {
                        if (lstPolineItem.Qty2BeInvoiced > 0)
                        {
                            dPartNumbers.Add(lstPolineItem.partNumber, lstPolineItem.Qty2BeInvoiced);
                        }
                    }
                    foreach (var dPartNumber in dPartNumbers)
                    {
                        var poReceipts = _operationsManager.GetPOReceiptsByPartNumber(dPartNumber.Key, oPublishInvoice.PoNumber);
                        decimal decToBeInvoicedCnt = dPartNumber.Value;
                        bool saveReceipt = false;
                        foreach (var poReceipt in poReceipts)
                        {
                            poReceiptInvoiceAP = new POReceiptInvoiceAP();
                            saveReceipt = false;
                            while (poReceipt.QtyReceived > poReceipt.QtyInvoiced && decToBeInvoicedCnt > 0)
                            {
                                poReceipt.QtyInvoiced = poReceipt.QtyInvoiced + 1;
                                poReceiptInvoiceAP.QtyInvoiced = poReceiptInvoiceAP.QtyInvoiced + 1;
                                decToBeInvoicedCnt = decToBeInvoicedCnt - 1;
                                saveReceipt = true;
                            }
                            if (saveReceipt)
                            {
                                poReceiptInvoiceAP.InvoiceId = invoiceAPSaved.Id;
                                poReceiptInvoiceAP.ReceiptId = poReceipt.Id;
                                poReceiptInvoiceAP.LastUpdateUserId = ADName;
                                poReceiptInvoiceAP.CreateUserId = ADName;
                                _logger.Debug("SavePOReceiptInvoiceAP started");
                                SavePOReceiptInvoiceAP(poReceiptInvoiceAP);
                                _logger.Debug("SavePOReceiptInvoiceAP ended");
                                // This need to be removed and we need to join the POReceiptInvoiceAP table to the view.
                                poReceipt.LastUpdateUserId = ADName;
                                poReceipt.CreateUserId = ADName;
                                _logger.Debug("SaveReceipt started");
                                SaveReceipt(poReceipt);
                                _logger.Debug("SaveReceipt ended");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error with SavePOReceiptInvoiceAP/SaveInvoiceAP or SaveReceipt " + ex.ToString());
            }
            #region TaxInput
            //if (decTax > 0)
            //{
            //    //Get the related PriceLevel Item
            //    List<QbPriceLevel> priceLevels = QbGetPriceLevel(oPublishInvoice.VendorCode, TaxCode);
            //    string strRelatedPriceLevel = string.Empty;
            //    if (priceLevels.Any())
            //    {
            //        strRelatedPriceLevel = priceLevels[0].RecordID.ToString();
            //    }
            //    _logger.Debug("Debug Inserting Tax Record " + decTax.ToString());
            //    List<QbEditFieldValues> objValues = new List<QbEditFieldValues>();
            //    QbEditFieldValues objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.OverrideWorkComplete,
            //        StrValue = "Work Complete",
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.CustomerInvoiceDate,
            //        StrValue = oPublishInvoice.InvoiceDate.ToShortDateString(),
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.AccrualOverrideDate,
            //        StrValue = oPublishInvoice.InvoiceDate.ToShortDateString(),
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.ManuallyTransferRevenuetoErp,
            //        StrValue = "1",
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.VendorInvoice,
            //        StrValue = oPublishInvoice.InvoiceNumber,// txtInvoiceNumber.Text,
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.RelatedPriceLevel,
            //        StrValue = strRelatedPriceLevel,
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.RelatedVendor,
            //        StrValue = oPublishInvoice.VendorCode,// ddlVendor.SelectedValue,
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.RelatedVendorPo,
            //        StrValue = oPublishInvoice.PoAmount.ToString(),
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.OomQuantity,
            //        StrValue = decTax.ToString(),
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.QtyPaid,
            //        StrValue = decTax.ToString(),
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    //objValue = new QbEditFieldValues
            //    //{
            //    //    FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.CasprJobCode,
            //    //    StrValue = oPublishInvoice.casprJobCode,// gvLineItems.DataKeys[0]["CASPRJobCode"].ToString(),
            //    //    IsFile = false
            //    //};
            //    //objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.RelatedItem,
            //        StrValue = TaxCode,// "6925", //TAX
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.RelatedItemType,
            //        StrValue = "2", //Driver
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    QbAddRecord(QuickBase.IMEX.Contract.Enums.TableName.LineItemTracker, objValues);
            //}
            #endregion

            #region Freight Input
            //if (decFreight > 0)
            //{
            //    _logger.Debug("Debug Inserting Freight Record " + decFreight.ToString());
            //    List<QbPriceLevel> priceLevels = QbGetPriceLevel(oPublishInvoice.VendorCode, FreightCode);
            //    //String strRelatedPriceLevel = priceLevels[0].RecordID.ToString(); 
            //    string strRelatedPriceLevel = string.Empty;
            //    if (priceLevels.Any())
            //    {
            //       strRelatedPriceLevel = priceLevels[0].RecordID.ToString();
            //    }

            //    List<QbEditFieldValues> objValues = new List<QbEditFieldValues>();
            //    QbEditFieldValues objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.OverrideWorkComplete,
            //        StrValue = "Work Complete",
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.AccrualOverrideDate,
            //        StrValue = oPublishInvoice.InvoiceDate.ToShortDateString(),// dtInvoiceDate.ToShortDateString(),
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.CustomerInvoiceDate,
            //        StrValue = oPublishInvoice.InvoiceDate.ToShortDateString(),
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.ManuallyTransferExpensetoErp,
            //        StrValue = "1",
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.VendorInvoice,
            //        StrValue = oPublishInvoice.InvoiceNumber,
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.RelatedPriceLevel,
            //        StrValue = strRelatedPriceLevel,
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.RelatedVendor,
            //        StrValue = oPublishInvoice.VendorCode,// ddlVendor.SelectedValue,
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.RelatedVendorPo,
            //        StrValue = oPublishInvoice.PoNumber,//  ddlPO.SelectedValue,
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.OomQuantity,
            //        StrValue = oPublishInvoice.FreightAmount.ToString(),// txtFreight.Text,
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.QtyPaid,
            //        StrValue = oPublishInvoice.FreightAmount.ToString(),// txtFreight.Text,
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    //objValue = new QbEditFieldValues
            //    //{
            //    //    FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.CasprJobCode,
            //    //    StrValue = oPublishInvoice.casprJobCode,
            //    //    IsFile = false
            //    //};
            //    //objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.RelatedItem,
            //        StrValue = FreightCode,// "6926", //FREIGHT
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    objValue = new QbEditFieldValues
            //    {
            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.RelatedItemType,
            //        StrValue = "2", //Driver
            //        IsFile = false
            //    };
            //    objValues.Add(objValue);
            //    QbAddRecord(QuickBase.IMEX.Contract.Enums.TableName.LineItemTracker, objValues);
            //}
            #endregion

            #region  POLineItem
            //_logger.Debug("Debug looping thru oPublishInvoice.poLineItemResults" + oPublishInvoice.PoLineItemReceiptsResults.Count.ToString());
            //foreach (var poLineItem in oPublishInvoice.PoLineItemReceiptsResults)
            //{
            //    try
            //    {
            //        var totDif = poLineItem.QtyReceiptReceived - poLineItem.Qty2BeInvoiced;
            //        if (totDif > 0 && poLineItem.Qty2BeInvoiced > 0)
            //        {
            //            List<QbEditFieldValues> objValues = null;
            //            QbEditFieldValues objValue = null;

            //            objValues = new List<QbEditFieldValues>();
            //            objValue = new QbEditFieldValues
            //            {
            //                FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.QtyPaid,
            //                StrValue = decPOAmount.ToString(),
            //                IsFile = false
            //            };
            //            objValues.Add(objValue);

            //            objValue = new QbEditFieldValues
            //            {
            //                FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.AccrualOverrideDate,
            //                StrValue = oPublishInvoice.InvoiceDate.ToShortDateString(),// dtInvoiceDate.ToShortDateString(),
            //                IsFile = false
            //            };
            //            objValues.Add(objValue);
            //            objValue = new QbEditFieldValues
            //            {
            //                FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.CustomerInvoiceDate,
            //                StrValue = oPublishInvoice.InvoiceDate.ToShortDateString(), //dtInvoiceDate.ToShortDateString(),
            //                IsFile = false
            //            };
            //            objValues.Add(objValue);
            //            objValue = new QbEditFieldValues
            //            {
            //                FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.OverrideWorkComplete,
            //                StrValue = "Work Complete",
            //                IsFile = false
            //            };
            //            objValues.Add(objValue);
            //            objValue = new QbEditFieldValues
            //            {
            //                FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.ManuallyTransferExpensetoErp,
            //                StrValue = "1",
            //                IsFile = false
            //            };
            //            objValues.Add(objValue);
            //            objValue = new QbEditFieldValues
            //            {
            //                FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.VendorInvoice,
            //                StrValue = oPublishInvoice.InvoiceNumber,// txtInvoiceNumber.Text,
            //                IsFile = false
            //            };
            //            objValues.Add(objValue);
            //            objValue = new QbEditFieldValues
            //            {
            //                FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.QtyOrdered,
            //                StrValue = poLineItem.QtyOrdered.ToString(),// txtInvoiceNumber.Text,
            //                IsFile = false
            //            };
            //            objValues.Add(objValue);
            //            objValue = new QbEditFieldValues
            //            {
            //                FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.QtyReceived,
            //                StrValue = poLineItem.Qty2BeInvoiced.ToString(),// txtInvoiceNumber.Text,
            //                IsFile = false
            //            };
            //            objValues.Add(objValue);
            //            objValue = new QbEditFieldValues
            //            {
            //                FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.UnitCost,
            //                StrValue = poLineItem.UnitCost.ToString(),// txtInvoiceNumber.Text,
            //                IsFile = false
            //            };
            //            objValues.Add(objValue);
            //            objValue = new QbEditFieldValues
            //            {
            //                FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.VendorId,
            //                StrValue = poLineItem.vendorCode.ToString(),// txtInvoiceNumber.Text,
            //                IsFile = false
            //            };
            //            objValues.Add(objValue);
            //            objValue = new QbEditFieldValues
            //            {
            //                FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.RelatedVendorPo,
            //                StrValue = poLineItem.poNumber.ToString(),// txtInvoiceNumber.Text,
            //                IsFile = false
            //            };
            //            objValues.Add(objValue);
            //            objValue = new QbEditFieldValues
            //            {
            //                FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.TotalRequestedValue,
            //                StrValue = poLineItem.TotalPOAmount.ToString(),
            //                IsFile = false
            //            };
            //            objValues.Add(objValue);
            //            objValue = new QbEditFieldValues
            //            {
            //                FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.OpsPONumber,
            //                StrValue = poLineItem.poNumber.ToString(),
            //                IsFile = false
            //            };
            //            objValues.Add(objValue);

            //            // if (poLineItem.RecordId == 0)
            //            // {
            //            QbAddRecord(QuickBase.IMEX.Contract.Enums.TableName.LineItemTracker, objValues);
            //            //}
            //            //else
            //            //{
            //            //QbEditFields(QuickBase.IMEX.Contract.Enums.TableName.LineItemTracker, poLineItem.RecordId, objValues);
            //            //}
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.Error("Error writing Invoice Data back to QB " + ex.ToString());
            //    }
            //}
            #endregion
           
            int endtime = Environment.TickCount - starttime;
            _logger.Debug("Apply Invoice ended at " + endtime);
        }     
       public List<Models.InvoiceAPDTO> GetReadyOrTransmittedAP(TransAPDTO transAPDTO)
       {
            //transAPDTO.UserId = transAPDTO.UserId.Replace("@DYNUTIL.COM", "");
            var rtnInvoices = _operationsManager.GetReadyOrTransmittedAp(transAPDTO.Transmitted, transAPDTO.UserId);
            //if (transAPDTO.UserId.ToUpper() != "ALL")
            //{
            //    rtnInvoices = rtnInvoices.FindAll(x => x.LastUpdateUserId.ToUpper().Contains(transAPDTO.UserId.ToUpper()));
            //}
            if (!string.IsNullOrEmpty(transAPDTO.InvoiceNumber))
            {
                rtnInvoices = rtnInvoices.FindAll(x => x.InvoiceNumber.ToUpper().Contains(transAPDTO.InvoiceNumber.ToUpper()) ||
                    x.poNumber.ToUpper().Contains(transAPDTO.InvoiceNumber.ToUpper())).ToList();
            }
            return _mapper.Map<List<Models.InvoiceAPDTO>>(rtnInvoices);
        }
        public List<Models.InvoiceAPDTO> GetReadyOrTransmittedAP(bool transmitted, string userId)
        {
            //userId = userId.Replace("@DYNUTIL.COM", "");
            var rtnInvoices = _operationsManager.GetReadyOrTransmittedAp(transmitted, userId);
            //rtnInvoices = rtnInvoices.FindAll(x => x.LastUpdateUserId.ToUpper().Contains(userId.ToUpper()));
            return _mapper.Map<List<Models.InvoiceAPDTO>>(rtnInvoices);
        }
        
        public Models.InvoiceAPDTO GetInvoiceById(string Id)
        {
            var invoiceAP = _operationsManager.GetInvoiceById(Id);
            return _mapper.Map<Models.InvoiceAPDTO>(invoiceAP);
        }
        public List<Models.InvoiceAPDTO> GetPOwithInvoices()
        {
            var invoiceAPs = _operationsManager.GetPOwithInvoices();
            return _mapper.Map<List<Models.InvoiceAPDTO>>(invoiceAPs);
        }
        public VendorDTO GetVendorNameByPONumber(string poNumber)
        {
            var vendor = _operationsManager.GetVendorNameByPONumber(poNumber);
            return _mapper.Map<VendorDTO>(vendor);
        }
        public List<Models.InvoiceAPDTO> GetInvoicesByPoNumberVendorId(string vendorId, string PONumber)
        {
            var invoiceAPs = _operationsManager.GetInvoicesByPoNumberVendorId(vendorId, PONumber);
            return _mapper.Map<List<Models.InvoiceAPDTO>>(invoiceAPs);
        }
        public Models.InvoiceAPDTO CheckVendorInvoiceNumber(Models.InvoiceAPDTO invoiceAP)
        {
            var invoice = _mapper.Map<Operations.IMEX.Contract.DTO.InvoiceAPDTO>(invoiceAP);
            var invoiceRtn = _operationsManager.CheckVendorInvoiceNumber(invoice);
            return _mapper.Map<Models.InvoiceAPDTO>(invoiceRtn);
        }
        public bool DeleteInvoiceAP(Models.InvoiceAPDTO invoiceAP)
        {
            var invoice = _mapper.Map<Operations.IMEX.Contract.DTO.InvoiceAPDTO>(invoiceAP);
            invoice.IsDeleted = true;
            return _operationsManager.DeleteInvoiceAP(invoice);
        }
        public bool DeleteReceiptInvoiceAP(Models.InvoiceAPDTO invoiceAP)
        {
            var invoice = _mapper.Map<Operations.IMEX.Contract.DTO.InvoiceAPDTO>(invoiceAP);
            invoice.IsDeleted = true;
            return _operationsManager.DeleteReceiptInvoiceAP(invoice);
        }
        public string DoesPoLineItemHaveReceipt(List<string> poHeaders)
        {
            string rtnData = "";
            string PONumberTest = string.Empty;
            List<POHeader> POHeaders = new List<POHeader>();
            POHeader POHeader = null;
            foreach (var poHeader in poHeaders)
            {
                POHeader = new POHeader();
                POHeader.PoNumber = poHeader;
                POHeaders.Add(POHeader);
            }
            var receiptPartNumbers = _operationsManager.GetReceiptPartNumbers(POHeaders);
            if (receiptPartNumbers != null)
            {
                return DynamicsProxyTaskManager.CreateNewTask(receiptPartNumbers, _operationsManager);
            }
            return rtnData;
        }

        public string GetDynamicsProxyStatus(string id)
        {
            return DynamicsProxyTaskManager.CheckStatus(id);
        }

        public string GetDynamicProxyResults(string id)
        {
            return DynamicsProxyTaskManager.GetTaskResults(id);
        }

        public bool SaveInvoiceAP(Models.InvoiceAPDTO invoiceAP)
        {
            var invoice = _mapper.Map<Operations.IMEX.Contract.DTO.InvoiceAPDTO>(invoiceAP);
            return _operationsManager.SaveInvoiceAP(invoice);
        }
        public bool SaveReceipt(POReceipt poReceipt)
        {
            //var poReceipt = _mapper.Map<POReceipt>(poReceipt);
            return _operationsManager.SaveReceipt(poReceipt);
        }
        public bool SavePOReceiptInvoiceAP(POReceiptInvoiceAP poReceiptInvoiceAP)
        {
            return _operationsManager.SavePOReceiptInvoiceAP(poReceiptInvoiceAP);
        }
        //public bool SaveReceipts(POReceiptDTO poReceiptDTO)
        //{
        //    var poReceipt = _mapper.Map<POReceipt>(poReceiptDTO);
        //    return _operationsManager.SaveReceipts(poReceipt);
        //}
        public bool ResetOrTransmittedInvoiceAP(TransmitInvoiceAPDTO transmitInvoices, string ADName, string fromEmailAddress)
        {
            //string strFilePath = ConfigurationManager.AppSettings.Get("APImportPath");
            //string toEmailInvoices = ConfigurationManager.AppSettings.Get("ToEmailInvoices");
            //string fromEmailInvoices = ConfigurationManager.AppSettings.Get("FromEmailInvoices");
            bool blnReturn = false;
            foreach (var invoice in transmitInvoices.Invoices2Transmit)
            {
                invoice.Transmitted = transmitInvoices.transmitted;
                invoice.TransmitDate = DateTime.Now;
            }
            var invoices = _mapper.Map<List<Operations.IMEX.Contract.DTO.InvoiceAPDTO>>(transmitInvoices.Invoices2Transmit);
            // blnReturn = _operationsManager.SaveInvoicesAP(invoices);
            blnReturn = _operationsManager.TransmitInvoiceAP(invoices, ADName,fromEmailAddress,false,true);
            //if (transmitInvoices.transmitted)
            //{
            //    string fileName = string.Empty;
            //    string dateRun = string.Empty;
            //    //generate batch PDF File return byte array
            //   byte[] bytes = generateAPTransReportPDF(transmitInvoices.Invoices2Transmit, ADName, false, out fileName, out dateRun);
            //    //generate Save PDF File to Disk Import Folder
            //    saveInvoiceBatchData(fileName, strFilePath, bytes);
            //    //push to Dynamics 
            //    generatePushFile(transmitInvoices.Invoices2Transmit, ADName);
            //    //email Batch to corporate
            //    emailInvoiceAndBatch(invoices, fileName, strFilePath, toEmailInvoices, fromEmailInvoices, dateRun);
            //}
            //else
            //{
            //    reverseQBInvoice(transmitInvoices.Invoices2Transmit);
            //}
            return blnReturn;
        }
        public bool UploadInvoiceFile(InvoiceFileDTO invoiceFile)
        {
            int starttime = Environment.TickCount;
            _logger.Debug("UploadInvoiceFile starttime "+ starttime);
            bool booReturn = false;
            try
            {
                string[] contentComponents = invoiceFile.content.Split(',');
                string fileName = string.Empty;
                byte[] sPDFDecoded = Convert.FromBase64String(contentComponents[1]);
                fileName = invoiceFile.vendorCode + "-" + invoiceFile.invoiceNumber + "-" + invoiceFile.invoiceDate.ToString("MMddyyhhmm") + ".pdf";
                //_operationsManager.saveInvoiceBatchData(fileName, sPDFDecoded);
                Operations.IMEX.Contract.DTO.InvoiceAPDTO invoiceAP = new Operations.IMEX.Contract.DTO.InvoiceAPDTO();
                invoiceAP.vendorId = invoiceFile.vendorCode;
                invoiceAP.InvoiceNumber = invoiceFile.invoiceNumber;
                invoiceAP.InvoiceDate = invoiceFile.invoiceDate;
                _operationsManager.uploadInvoiceFile(invoiceAP, invoiceFile.content);
                booReturn = true;
                int endtime = Environment.TickCount - starttime;
                _logger.Debug("UploadInvoiceFile endtime " + endtime);
            } catch (Exception ex)
            {
                _logger.Debug("Error UploadInvoiceFile" + ex.ToString());
            }
            return booReturn;
        }
        public List<Models.InvoiceAPDTO> AdHocInvoiceReport(TransmittedReportDTO transmittedReport, string ADName)
        {
            //string ADName = GetAdName();
            // string ADName = "ALL";
            // HttpResponseMessage result = null;
            string strFileName = string.Empty;
            string dateRun = string.Empty;
            List<Operations.IMEX.Contract.DTO.InvoiceAPDTO> invoiceToBeQueried = new List<Operations.IMEX.Contract.DTO.InvoiceAPDTO>();
            DateTime tempTest = new DateTime();
            //if(transmittedReport.beginDate=="1/1/0001")

            var invoices = this.GetReadyOrTransmittedAP(true, ADName);
            if (!string.IsNullOrEmpty(transmittedReport.vendorId) && !string.IsNullOrEmpty(transmittedReport.poNumber))
            {
                if (transmittedReport.beginDate != tempTest && transmittedReport.endDate != tempTest)
                {
                    invoiceToBeQueried = (from Operations.IMEX.Contract.DTO.InvoiceAPDTO invoice in invoices
                                            where invoice.vendorId == transmittedReport.vendorId
                                            && invoice.poNumber == transmittedReport.poNumber
                                            && transmittedReport.beginDate <= invoice.TransmitDate
                                            && invoice.TransmitDate <= transmittedReport.endDate
                                            select invoice).ToList();
                }
                else if (transmittedReport.beginDate != tempTest && transmittedReport.endDate == tempTest)
                {
                    invoiceToBeQueried = (from Operations.IMEX.Contract.DTO.InvoiceAPDTO invoice in invoices
                                            where invoice.vendorId == transmittedReport.vendorId
                                            && invoice.poNumber == transmittedReport.poNumber
                                            // && transmittedReport.beginDate <= invoice.TransmitDate
                                            && transmittedReport.beginDate <= invoice.TransmitDate
                                            //&& invoice.TransmitDate <= transmittedReport.endDate
                                            select invoice).ToList();
                }
                else if (transmittedReport.beginDate == tempTest && transmittedReport.endDate != tempTest)
                {
                    invoiceToBeQueried = (from Operations.IMEX.Contract.DTO.InvoiceAPDTO invoice in invoices
                                            where invoice.vendorId == transmittedReport.vendorId
                                            && invoice.poNumber == transmittedReport.poNumber
                                            // && transmittedReport.beginDate <= invoice.TransmitDate
                                            && invoice.TransmitDate <= transmittedReport.endDate
                                            select invoice).ToList();
                }
                else if (transmittedReport.beginDate == tempTest && transmittedReport.endDate == tempTest)
                {
                    invoiceToBeQueried = (from Operations.IMEX.Contract.DTO.InvoiceAPDTO invoice in invoices
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
                    invoiceToBeQueried = (from Operations.IMEX.Contract.DTO.InvoiceAPDTO invoice in invoices
                                            where invoice.vendorId == transmittedReport.vendorId
                                            && transmittedReport.beginDate <= invoice.TransmitDate
                                            && invoice.TransmitDate <= transmittedReport.endDate
                                            select invoice).ToList();
                }
                else if (transmittedReport.beginDate == tempTest && transmittedReport.endDate != tempTest)
                {
                    invoiceToBeQueried = (from Operations.IMEX.Contract.DTO.InvoiceAPDTO invoice in invoices
                                            where invoice.vendorId == transmittedReport.vendorId
                                            // && transmittedReport.beginDate <= invoice.TransmitDate
                                            && invoice.TransmitDate <= transmittedReport.endDate
                                            select invoice).ToList();
                }
                else if (transmittedReport.beginDate != tempTest && transmittedReport.endDate == tempTest)
                {
                    invoiceToBeQueried = (from Operations.IMEX.Contract.DTO.InvoiceAPDTO invoice in invoices
                                            where invoice.vendorId == transmittedReport.vendorId
                                            && transmittedReport.beginDate <= invoice.TransmitDate
                                            //&& invoice.TransmitDate <= transmittedReport.endDate
                                            select invoice).ToList();
                }
                else if (transmittedReport.beginDate == tempTest && transmittedReport.endDate == tempTest)
                {
                    invoiceToBeQueried = (from Operations.IMEX.Contract.DTO.InvoiceAPDTO invoice in invoices
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
                    invoiceToBeQueried = (from Operations.IMEX.Contract.DTO.InvoiceAPDTO invoice in invoices
                                            where invoice.poNumber == transmittedReport.poNumber
                                            && transmittedReport.beginDate <= invoice.TransmitDate
                                            && invoice.TransmitDate <= transmittedReport.endDate
                                            select invoice).ToList();
                }
                else if (transmittedReport.beginDate == tempTest && transmittedReport.endDate != tempTest)
                {
                    invoiceToBeQueried = (from Operations.IMEX.Contract.DTO.InvoiceAPDTO invoice in invoices
                                            where invoice.poNumber == transmittedReport.poNumber
                                            // && transmittedReport.beginDate <= invoice.TransmitDate
                                            && invoice.TransmitDate <= transmittedReport.endDate
                                            select invoice).ToList();
                }
                else if (transmittedReport.beginDate != tempTest && transmittedReport.endDate == tempTest)
                {
                    invoiceToBeQueried = (from Operations.IMEX.Contract.DTO.InvoiceAPDTO invoice in invoices
                                            where invoice.poNumber == transmittedReport.poNumber
                                            && transmittedReport.beginDate <= invoice.TransmitDate
                                            //&& invoice.TransmitDate <= transmittedReport.endDate
                                            select invoice).ToList();
                }
                else if (transmittedReport.beginDate == tempTest && transmittedReport.endDate == tempTest)
                {
                    invoiceToBeQueried = (from Operations.IMEX.Contract.DTO.InvoiceAPDTO invoice in invoices
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
                    invoiceToBeQueried = (from Operations.IMEX.Contract.DTO.InvoiceAPDTO invoice in invoices
                                            where /*invoice.SiteId == transmittedReport.siteId
                                        && invoice.vendorId == transmittedReport.vendorId 
                                        && */ transmittedReport.beginDate <= invoice.TransmitDate
                                            && invoice.TransmitDate <= transmittedReport.endDate
                                            select invoice).ToList();
                }
                else if (transmittedReport.beginDate == tempTest && transmittedReport.endDate != tempTest)
                {
                    invoiceToBeQueried = (from Operations.IMEX.Contract.DTO.InvoiceAPDTO invoice in invoices
                                            where /*invoice.SiteId == transmittedReport.siteId
                                        && invoice.vendorId == transmittedReport.vendorId */
                                                // transmittedReport.beginDate <= invoice.TransmitDate
                                            invoice.InvoiceDate <= transmittedReport.endDate
                                            select invoice).ToList();
                }
                else if (transmittedReport.beginDate != tempTest && transmittedReport.endDate == tempTest)
                {
                    invoiceToBeQueried = (from Operations.IMEX.Contract.DTO.InvoiceAPDTO invoice in invoices
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
                    //invoiceToBeQueried = invoices;
                }
            }
            return null;
        }
        public byte[] generateAPTransReportPDF(List<Models.InvoiceAPDTO> invoices, string ADName, bool Adhoc, out string strFileName, out string dateRun)
        {
            byte[] bytesReport = null;
            string strBranchNumber = "520";
            dateRun = DateTime.Now.ToString("MMddyyhhmm");
            strFileName = "AP-" + ADName.ToUpper() + "-" + dateRun + "-" + strBranchNumber + ".pdf";
            strFileName = strFileName.Replace("@DYNUTIL.COM", "");
            string strRawFileName = "AP" + dateRun + ".520";
            char chFile = 'A';
            decimal invoiceAmtTemp = 0;
            foreach (var invoice in invoices)
            {
                invoice.poNumber = string.IsNullOrEmpty(invoice.poNumber) ? "Missing PO Number" : invoice.poNumber;
                invoice.vendorId = string.IsNullOrEmpty(invoice.vendorId) ? "Missing Vendor Id" : invoice.vendorId;
                invoice.VendorName = string.IsNullOrEmpty(invoice.VendorName) ? "Missing Vendor Name" : invoice.VendorName;
                invoice.InvoiceNumber = string.IsNullOrEmpty(invoice.InvoiceNumber) ? "Missing Invoice Number " : invoice.InvoiceNumber;
                invoiceAmtTemp = 0;
                Decimal.TryParse(invoice.InvoiceAmount.ToString(), out invoiceAmtTemp);
                invoice.InvoiceAmount = invoiceAmtTemp;
            }
            //while (File.Exists(strFileName))
            //{
            //    strFileName = strFilePath + "AP-" + GetAdName().ToUpper() + "-" + dateRun + chFile + "." + strBranchNumber;
            //    strRawFileName = "AP-" + GetAdName().ToUpper() + "-" + dateRun + chFile + "." + strBranchNumber;
            //    chFile = (char)(((int)chFile) + 1);
            //}
            ReportParameterCollection parms = new ReportParameterCollection();
            ReportDataSource ds = new ReportDataSource("Invoices", invoices);
            ReportParameter parm = null;
            ReportViewer invoiceReport = new ReportViewer();
            if (Adhoc)
            {
                invoiceReport.LocalReport.DisplayName = "Adhoc Invoice Report";
                invoiceReport.LocalReport.ReportPath = "Reports/APTxInvoices.rdlc";
            }
            else
            {
                parm = new ReportParameter("BranchNumber", strBranchNumber);
                parms.Add(parm);
                parm = new ReportParameter("FileName", strFileName);
                parms.Add(parm);
                invoiceReport.LocalReport.DisplayName = "Vendor Invoice Transmit Report";
                invoiceReport.LocalReport.ReportPath = "Reports/APTransmit.rdlc";
                invoiceReport.LocalReport.SetParameters(parms);
            }
            invoiceReport.LocalReport.DataSources.Clear();
            invoiceReport.LocalReport.DataSources.Add(ds);
            invoiceReport.LocalReport.Refresh();
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;
            bytesReport = invoiceReport.LocalReport.Render(
                 "PDF", null, out mimeType, out encoding, out filenameExtension,
                 out streamids, out warnings);
            //saveInvoiceBatchData(strFilePath + strFileName, bytesReport);
            return bytesReport;
        }
        public List<VendorDTO> GetVendors()
        {
            var vendors = _operationsManager.GetVendors();
            return _mapper.Map<List<VendorDTO>>(vendors);
        }        
        public List<POHeaderDTO> POHeaderSearch(string POSearch)
        {
            var poHeaders = _operationsManager.POHeaderSearch(POSearch);
            return _mapper.Map<List<POHeaderDTO>>(poHeaders);
        }
        public List<VendorDTO> GetVendorsWithPOs()
        {
            var vendors = _operationsManager.GetVendorsWithPOs();
            return _mapper.Map<List<VendorDTO>>(vendors);
        }
        public VendorDTO GetVendor(string vendorId)
        {
            var vendor = _operationsManager.GetVendor(vendorId);
            return _mapper.Map<VendorDTO>(vendor);
        }
        public VendorDTO GetVendorByVendorCode(string vendorCode)
        {
            var vendor = _operationsManager.GetVendorByVendorCode(vendorCode);
            return _mapper.Map<VendorDTO>(vendor);
        }
        //public List<POLineItemDTO> GetLineItemByVendorId(string vendorId)
        //{
        //    var jobLineItems = _operationsManager.GetLineItemByVendorId(vendorId);
        //    return _mapper.Map<List<POLineItemDTO>>(jobLineItems);
        //}
        public List<POHeaderDTO> GetPOsByVendor(string vendorId)
        {
            var poheaders = _operationsManager.GetPOsByVendor(vendorId);
            return _mapper.Map<List<POHeaderDTO>>(poheaders);
        }
        public POHeaderDTO GetPONumberByPOId(string POId)
        {
            var poHeader = _operationsManager.GetPONumberByPOId(POId);
            return _mapper.Map<POHeaderDTO>(poHeader);
        }
        public POHeaderDTO GetPOByPONumber(string poNumber)
        {
            var poHeader = _operationsManager.GetPOByPONumber(poNumber);
            return _mapper.Map<POHeaderDTO>(poHeader);
        }
        public List<POLineItemDTO> GetPODetailsByPOId(string POId)
        {
            var poLineItems = _operationsManager.GetPODetailsByPOId(POId);
            return _mapper.Map<List<POLineItemDTO>>(poLineItems);
        }
        //public List<PoHeaderPoLineItemReceiptsViewDTO> GetPOReceipts(string POId)
        //{
        //    var poHeaderLineItems = _operationsManager.GetPOReceipts(POId);
        //    return _mapper.Map<List<PoHeaderPoLineItemReceiptsViewDTO>>(poHeaderLineItems);
        //}
        public List<POReceiptDTO> GetPOReceiptsByPOId(string POId)
        {
            var poHeaderLineItems = _operationsManager.GetPOReceiptsByPOId(POId);
            return _mapper.Map<List<POReceiptDTO>>(poHeaderLineItems);
        }
        public List<PoLineItemReceiptsViewDTO> GetLineItemsByReceipt(string receiptNumber)
        {
            var poLineItems = _operationsManager.GetLineItemsByReceiptNumber(receiptNumber);
            foreach (var poLineItem in poLineItems)
            {
                if (string.IsNullOrEmpty(poLineItem.ShortDescription) && !string.IsNullOrEmpty(poLineItem.Description))
                {
                    poLineItem.ItemDescription = poLineItem.Description;
                }
                if (string.IsNullOrEmpty(poLineItem.ShortDescription) && string.IsNullOrEmpty(poLineItem.Description))
                {
                    poLineItem.ItemDescription = poLineItem.ItemDescription;
                }
                else
                {
                    poLineItem.ItemDescription = poLineItem.ShortDescription;
                }
            }
            return _mapper.Map<List<PoLineItemReceiptsViewDTO>>(poLineItems);
        }
        public List<PoLineItemReceiptsViewDTO> GetLineItemsByInvoiceId(string invoiceId, bool editMode)
        {
            var poLineItems = _operationsManager.GetLineItemsByInvoiceId(invoiceId);
            foreach (var poLineItem in poLineItems)
            {
                if (string.IsNullOrEmpty(poLineItem.ShortDescription) && !string.IsNullOrEmpty(poLineItem.Description))
                {
                    poLineItem.ItemDescription = poLineItem.Description;
                }
                if (string.IsNullOrEmpty(poLineItem.ShortDescription) && string.IsNullOrEmpty(poLineItem.Description))
                {
                    poLineItem.ItemDescription = poLineItem.ItemDescription;
                }
                else
                {
                    poLineItem.ItemDescription = poLineItem.ShortDescription;
                }
                if (string.IsNullOrEmpty(poLineItem.Uom))
                {
                    poLineItem.Uom = "N/A";
                }
                decimal temp = 0;
                //if(editMode)
                //{
                //    poLineItem.QtyInvoiced = temp;
                //}
                poLineItem.QtyInvoiceLeft = poLineItem.QtyReceiptReceived - poLineItem.QtyInvoiced;
                poLineItem.qty2BeInvoiced = poLineItem.QtyInvoiceLeft;
                poLineItem.TotalPOAmount = poLineItem.UnitCost * poLineItem.QtyReceiptReceived;
                poLineItem.Type = "Material";
            }
            return _mapper.Map<List<PoLineItemReceiptsViewDTO>>(poLineItems);
        }
        public List<PoLineItemReceiptsViewDTO> GetLineItemsByPOId(string POId)
        {
            var poLineItems = _operationsManager.GetLineItemsByPOId(POId);
            foreach (var poLineItem in poLineItems)
            {
                if (string.IsNullOrEmpty(poLineItem.ShortDescription) && !string.IsNullOrEmpty(poLineItem.Description))
                {
                    poLineItem.ItemDescription = poLineItem.Description;
                }
                if (string.IsNullOrEmpty(poLineItem.ShortDescription) && string.IsNullOrEmpty(poLineItem.Description))
                {
                    poLineItem.ItemDescription = poLineItem.ItemDescription;
                }
                else
                {
                    poLineItem.ItemDescription = poLineItem.ShortDescription;
                }
                if (string.IsNullOrEmpty(poLineItem.Uom))
                {
                    poLineItem.Uom = "N/A";
                }
                poLineItem.QtyInvoiceLeft = poLineItem.QtyReceiptReceived - poLineItem.QtyInvoiced;
                poLineItem.qty2BeInvoiced = poLineItem.QtyInvoiceLeft;
                poLineItem.TotalPOAmount = poLineItem.UnitCost * poLineItem.QtyReceiptReceived;
                poLineItem.Type = "Material";
            }
            return _mapper.Map<List<PoLineItemReceiptsViewDTO>>(poLineItems);
        }
        public List<PoLineItemReceiptsViewDTO> GetLineItemsByPONumber(string poNumber)
        {
            //POHeader POHeader = _operationsManager.GetPOByPONumber(poNumber);
            //if (POHeader == null)
            //    return null;
            //var poLineItems = _operationsManager.GetLineItemsByPOId(POHeader.Id);
            var poLineItems = _operationsManager.GetLineItemsByPOId(poNumber);
            foreach (var poLineItem in poLineItems)
            {
                if (string.IsNullOrEmpty(poLineItem.ShortDescription) && !string.IsNullOrEmpty(poLineItem.Description))
                {
                    poLineItem.ItemDescription = poLineItem.Description;
                }
                if (string.IsNullOrEmpty(poLineItem.ShortDescription) && string.IsNullOrEmpty(poLineItem.Description))
                {
                    poLineItem.ItemDescription = poLineItem.ItemDescription;
                }
                else
                {
                    poLineItem.ItemDescription = poLineItem.ShortDescription;
                }
                if (string.IsNullOrEmpty(poLineItem.Uom))
                {
                    poLineItem.Uom = "N/A";
                }
                // qty2BeInvoiced
                poLineItem.QtyInvoiceLeft = poLineItem.QtyReceiptReceived - poLineItem.QtyInvoiced;
                poLineItem.qty2BeInvoiced = poLineItem.QtyInvoiceLeft;
                poLineItem.TotalPOAmount = poLineItem.UnitCost * poLineItem.QtyReceiptReceived;
                poLineItem.Type = "Material";
            }
            return _mapper.Map<List<PoLineItemReceiptsViewDTO>>(poLineItems);
        }
        public Models.InvoiceAPDTO GetInvoiceAP(int recordId)
        {
            Operations.IMEX.Contract.DTO.InvoiceAPDTO invoice = new Operations.IMEX.Contract.DTO.InvoiceAPDTO();
            invoice = _operationsManager.GetInvoiceAP(recordId);
            return _mapper.Map<Models.InvoiceAPDTO>(invoice);
        }
        public string GetJobCodeByPoNumber(string poNumber)
        {
            return _operationsManager.FetchJobCodeByPONumber(poNumber);
        }
        //public List<QbPriceLevel> QbGetPriceLevel(string strRelatedVendor, string strRelatedItem)
        //{
        //    return _qbManager.GetPriceLevel(strRelatedVendor, strRelatedItem, _appName);
        //}
        //public QbAddRecord QbAddRecord(QuickBase.IMEX.Contract.Enums.TableName tableName, List<QbEditFieldValues> fieldList)
        //{
        //    return _qbManager.AddRecord(tableName, fieldList, _appName);
        //}
        //public void QbEditFields(QuickBase.IMEX.Contract.Enums.TableName tableName, long recordId, List<QbEditFieldValues> fieldList)
        //{
        //    _qbManager.EditFields(tableName, recordId, fieldList, _appName);
        //}
        public List<Models.InvoiceAPDTO> GetAPUsers()
        {
            var invoiceAPs = _operationsManager.GetAPUsers();
            Operations.IMEX.Contract.DTO.InvoiceAPDTO invoiceALL = new Operations.IMEX.Contract.DTO.InvoiceAPDTO();
            invoiceALL.LastUpdateUserId = "ALL";
            invoiceAPs.Add(invoiceALL);
            foreach (var invoiceAP in invoiceAPs)
            {
                invoiceAP.LastUpdateUserId = invoiceAP.LastUpdateUserId.ToUpper();
            }
            return _mapper.Map<List<Models.InvoiceAPDTO>>(invoiceAPs.OrderBy(x => x.LastUpdateUserId));
        }
        private void emailInvoiceAndBatch(List<Operations.IMEX.Contract.DTO.InvoiceAPDTO> invoices, string strFileName,
            string strFilePath, string toEmailAddress, string fromEmailAddress, string dateRun)
        {
            _operationsManager.emailInvoiceAndBatch(invoices, strFileName, strFilePath, toEmailAddress, fromEmailAddress, dateRun);
        }
        //private void uploadInvoiceFile(InvoiceFileDTO invoiceFile, string ADName)
        //{
        //    Operations.IMEX.Contract.DTO.InvoiceAPDTO invoiceItems = new Operations.IMEX.Contract.DTO.InvoiceAPDTO();
        //    invoiceItems.InvoiceNumber = invoiceFile.invoiceNumber;
        //    invoiceItems.vendorId = invoiceFile.vendorCode;
        //    invoiceItems.InvoiceDate = invoiceFile.invoiceDate;
        //    string content = invoiceFile.content;
        //    string invoiceName = invoiceFile.invoiceName;
        //    //_operationsManager.uploadInvoiceFile(invoiceItems, content, ADName);
        //}
        private bool saveInvoiceBatchData(string FileName, string FilePath, byte[] Data)
        {
            if (FileName == string.Empty || FilePath == string.Empty)
            {
                _logger.Error("Error persisting file. Missing FileName or FilePath.");
                return false;
            }
            try
            {
                // Create a new stream to write to the file
                using (var Writer = new BinaryWriter(File.OpenWrite(FilePath + FileName)))
                { 
                // Writer raw data                
                    Writer.Write(Data);
                    Writer.Flush();
                    Writer.Close(); // for the BinaryWriter the Close() and Dispose are interchangeable  
                   //Writer.Dispose();
                    _logger.Debug("Writing File " + FileName);
                    _operationsManager.saveInvoiceBatchData(FileName,Data);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error persisting file" + ex.ToString());
                return false;
            }
            return true;
        }
        private void generatePushFile(List<Models.InvoiceAPDTO> invoices, string ADName)
        {
            ApVoucherRequest apVoucherRequests = new ApVoucherRequest();
            ApVoucherRequestItem apVoucherRequestItem = null;
            ApVoucherHeader apVoucherHeader = null;
            ApVoucherLineItem apVoucherLineItem = null;
            POHeaderDTO poHeader = null;
            try
            {
                foreach (var invoice in invoices)
                {
                    apVoucherRequestItem = new ApVoucherRequestItem();
                    apVoucherHeader = new ApVoucherHeader();
                    //Build New Header for apVoucherHeader
                    poHeader = GetPOByPONumber(invoice.poNumber);
                    //poReceipts = GetReceipts(invoice.receiptNumber);
                    apVoucherHeader.PONumber = invoice.poNumber;
                    //apVoucherHeader.ReceiptNumber = invoice.receiptNumber;
                    apVoucherHeader.CompanyCode = "ANSCO";
                    apVoucherHeader.WarehouseCode = poHeader.WarehouseCode;
                    apVoucherHeader.VendorId = invoice.vendorId;
                    apVoucherHeader.RecordDate = DateTime.Today;
                    apVoucherHeader.InvoiceDate = invoice.InvoiceDate;
                    apVoucherHeader.InvoiceNumber = invoice.InvoiceNumber;
                    apVoucherHeader.FreightAmount = invoice.FreightAmount;
                    apVoucherHeader.TaxAmount = invoice.TaxAmount;
                    if (invoice.TaxAmount > 0)
                    {
                        apVoucherHeader.HasTax = IMEX.Contract.Enums.TaxType.Tax;
                    }
                    else
                    {
                        apVoucherHeader.HasTax = IMEX.Contract.Enums.TaxType.NoTax;
                    }
                    apVoucherHeader.InvoiceBalance = invoice.InvoiceAmount;
                    //apVoucherHeader.DocumentType = invoice.InvoiceAmount >= 0 ? IMEX.Contract.Enums.DocumentType.Voucher : IMEX.Contract.Enums.DocumentType.CreditAdjust;
                    apVoucherHeader.DocumentType = invoice.InvoiceAmount >= 0 ? IMEX.Contract.Enums.DocumentType.Voucher : IMEX.Contract.Enums.DocumentType.CreditAdjust;
                    int i = 0;
                    List<ApVoucherLineItem> lstApVoucherLineItems = new List<ApVoucherLineItem>();
                    //Build New Detail for apVoucherLineItem
                    List<PoLineItemReceiptsViewDTO> poLineItems = GetLineItemsByInvoiceId(invoice.Id,false);
                    //List<JobLineItemDTO> jobLineItems = GetJobLineItemsByPONumber(invoice.poNumber);
                    foreach (var lineItem in poLineItems)
                    {
                        i++;
                        apVoucherLineItem = new ApVoucherLineItem();
                        apVoucherLineItem.LineItemNumber = i;
                        apVoucherLineItem.PartNumber = lineItem.partNumber;
                        apVoucherLineItem.ReceiptNumber = lineItem.receiptNumber;
                        if (!string.IsNullOrWhiteSpace(lineItem.ItemDescription))
                        {
                            apVoucherLineItem.Description = lineItem.ItemDescription.Length > 30 ? lineItem.ItemDescription.Substring(0, 30) : lineItem.ItemDescription;
                        }
                        else
                        {
                            apVoucherLineItem.Description = "";
                        }
                        apVoucherLineItem.Qty = lineItem.QtyInvoiced;
                        apVoucherLineItem.UnitCost = lineItem.UnitCost;
                        apVoucherLineItem.LineType = IMEX.Contract.Enums.LineType.Invoice;
                        lstApVoucherLineItems.Add(apVoucherLineItem);
                    }
                    apVoucherRequestItem.Header = apVoucherHeader;
                    apVoucherRequestItem.Detail = lstApVoucherLineItems;
                    apVoucherRequests.Request.Add(apVoucherRequestItem);
                    #region QB LineItems
                    try
                    {
                        //if (jobLineItems != null)
                       // {
                            //foreach (var lineItem in jobLineItems)
                            //{
                            //    //update Quickbase
                            //    //Set the Transmitted flag in the Invoice record to "1"
                            //    List<QbEditFieldValues> objValues = new List<QbEditFieldValues>();
                            //    QbEditFieldValues objValue = new QbEditFieldValues
                            //    {
                            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.Transmitted,
                            //        StrValue = "1"
                            //    };
                            //    objValues.Add(objValue);
                            //    objValue = new QbEditFieldValues
                            //    {
                            //        FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.TransmitDate,
                            //        StrValue = DateTime.Today.ToShortDateString()
                            //    };
                            //    objValues.Add(objValue);
                            //    QbEditFields(QuickBase.IMEX.Contract.Enums.TableName.LineItemTracker, lineItem.RecordId, objValues);
                            //}
                        //} else
                       // {
                       //     _logger.Debug("No JoblineItems found for " + invoice.InvoiceNumber);
                       // }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Error with AP Voucher Sending Data to QB for " + invoice.InvoiceNumber + " " + ex.ToString());
                    }
                    #endregion
                }
                // send to dynamics queue 
                publishInvoiceVouchers(apVoucherRequests);
            }
            catch (Exception ex)
            {
                _logger.Error("Error with AP Voucher Transmission process " + ex.ToString());
            }
        }
        private void publishInvoiceVouchers(ApVoucherRequest apVoucherRequests)
        {
            _operationsManager.PublishInvoiceVoucher(apVoucherRequests);
        }
        private void reverseQBInvoice(List<Models.InvoiceAPDTO> invoices)
        {
            foreach (var invoice in invoices)
            {
                try
                {

                    //List<JobLineItemDTO> jobLineItems = GetJobLineItemsByPONumber(invoice.poNumber);
                    //if (jobLineItems != null)
                    //{
                    //    foreach (var jobLineItem in jobLineItems)
                    //    {
                    //        List<QbEditFieldValues> objValues = new List<QbEditFieldValues>();
                    //        QbEditFieldValues objValue = new QbEditFieldValues
                    //        {
                    //            FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.Transmitted,
                    //            StrValue = "0"
                    //        };
                    //        objValues.Add(objValue);
                    //        objValue = new QbEditFieldValues
                    //        {
                    //            FieldId = (int)QuickBase.IMEX.Contract.Enums.LineItemTrackerTable.TransmitDate,
                    //            StrValue = ""
                    //        };
                    //        QbEditFields(QuickBase.IMEX.Contract.Enums.TableName.LineItemTracker, jobLineItem.RecordId, objValues);
                    //    }
                    //}
                    //else
                    //{
                    //    _logger.Debug("No JoblineItems found for " + invoice.InvoiceNumber);
                    //}
                } catch (Exception ex)
                {
                    _logger.Error("Error with Reversing the QB transmistion for " + invoice.InvoiceNumber + " " + ex.ToString());
                }
            }
        }
        
        /// <summary>
        /// Default Destructor
        /// </summary>
        ~AccountPayableOrchestrator()
        {
            this._operationsManager.Dispose();
            //this._dynamicsManager.Dispose();
        }
        #endregion
    }
   
}
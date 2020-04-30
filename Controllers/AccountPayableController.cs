using CertusView.OP.WebClient.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CertusView.OP.WebClient.Controllers.AccountPayable
{
    public class AccountPayableController : Controller
    {
        [LogisticsMvcAuthorize(Object = "AP", Permission = "view")]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ReceiveVendorInvoice()
        {
            return View("ReceiveVendorInvoice");
        }
        public ActionResult TransmitAPFile()
        {
            return View("TransmitAPFile");
        }
        public ActionResult APCorrections()
        {
            return View("APCorrections");
        }
        public ActionResult GetAPReportDialog()
        {
            return View("InvoiceReport");
        }
        public ActionResult GetInvoiceTransFindDialog()
        {
            return View("InvoiceTransmissionFind");
        }
    }
}
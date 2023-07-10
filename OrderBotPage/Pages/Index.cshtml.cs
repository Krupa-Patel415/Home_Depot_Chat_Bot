using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Twilio;
using Twilio.TwiML;
using OrderBot;


namespace wireless.Pages
{


 [IgnoreAntiforgeryToken(Order = 1001)]
    public class IndexModel : PageModel
    {
        private static Dictionary<string, Order> aOrders = null;

        public ActionResult OnPost()
        {
            string sFrom = Request.Form["From"];
            string sBody = Request.Form["Body"];
            var oMessage = new Twilio.TwiML.MessagingResponse();
            if(aOrders == null){
                aOrders = new Dictionary<string, Order>();
            }
            if(!aOrders.ContainsKey(sFrom)){
                aOrders[sFrom] = new Order();
            }
            oMessage.Message(aOrders[sFrom].OnMessage(sBody));
            return Content(oMessage.ToString(), "application/xml");
        }
    }
}

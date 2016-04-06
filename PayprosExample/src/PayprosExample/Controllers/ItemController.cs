using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using PayprosExample.Models;
using System.Threading.Tasks;

namespace PayprosExample.Controllers
{
    [Route("api/[controller]")]
    public class ItemController : Controller
    {
        // GET: api/item
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/item/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/item/5/payment
        [HttpPost("{id}/payment")]
        [RequireHttps]
        public async Task<PayPros> Post(int id, [FromBody]PayPros payment)
        {
            //Get item to be paid from id
            var item = new { id = id, balance = 1.00 };

            #region PayPros
            //Process Payment
            EFContext _dbContext = new EFContext();

            //Be secure, remove full CC from persistent storage record
            string FullCCNum = payment.CreditCardNumber;
            payment = PaymentProcessing.EncryptCreditCardNumber(payment);
            //Save payment attempt to persistent storage
            _dbContext.PayPros.Add(payment);
            await _dbContext.SaveChangesAsync();
            //Process with paypros
            PaymentProcessing payProcessing = new PaymentProcessing(PaymentAccounts.Judicial, PaymentMethods.Web);
            payment = payProcessing.Initiate(payment, FullCCNum);
            //Save updated payment attempt to persistent storage
            _dbContext.PayPros.Update(payment);
            await _dbContext.SaveChangesAsync();
            #endregion

            //Update item if payment is successful
            if (payment.responseCode == 1)
                item.balance = 0.00;

            return payment;
        }
    }
}

using System;
using System.Collections.Generic;
using Paygateway;
using PayprosExample.Models;

namespace PayprosExample
{
    public enum PaymentAccounts
    {
        UBO,
        Judicial
    }

    public enum PaymentMethods
    {
        Web,
        Phone
    }

    public class PaymentProcessing
    {
        private PaymentAccounts _account { get; set; }
        private PaymentMethods _method { get; set; }
        private Dictionary<PaymentAccounts, string> accountTokens = new Dictionary<PaymentAccounts, string>()
        {
            { PaymentAccounts.UBO, "<UtilityBilling_Account_Token>" },
            { PaymentAccounts.Judicial, "<Judicial_Account_Token>" }
        };
        private Dictionary<PaymentAccounts, string> testTokens = new Dictionary<PaymentAccounts, string>()
        {
            { PaymentAccounts.UBO, "<Test_Account_Token>" },
            { PaymentAccounts.Judicial, "<Test_Account_Token>" }
        };

        public PaymentProcessing(PaymentAccounts account, PaymentMethods method)
        {
            _account = account;
            _method = method;
        }

        public static PayPros EncryptCreditCardNumber(PayPros payment)
        {
            /**
             * Strip the full CC number out immediately.
             * TODO - Encrypt it and store it in CreditCardNumber field?
             * **/
            payment.CCLast4 = payment.CCLast4 ?? payment.CreditCardNumber.Substring(12, 4);
            payment.CreditCardNumber = null;
            return payment;
        }

        public PayPros Initiate(PayPros payment, string cardNumber)
        {
            #region CREATE REQUEST
            CreditCardRequest request = new CreditCardRequest();
            try
            {
                request.setCreditCardNumber(cardNumber);
                if (!string.IsNullOrEmpty(payment.CCV)) request.setCreditCardVerificationNumber(payment.CCV);
                request.setExpireMonth(payment.CCExpireMonth);
                request.setExpireYear(payment.CCExpireYear);
                request.setChargeType(CreditCardRequest.SALE);
                request.setPurchaseOrderNumber(payment.PaymentAccount);
                request.setChargeTotal((double)payment.PaymentAmount);

                request.setPartialApprovalFlag(false);

                if (!string.IsNullOrEmpty(payment.RequestingOrigin)) request.setCustomerIpAddress(payment.RequestingOrigin);

                switch (_account)
                {
                    case PaymentAccounts.UBO:
                        request.setIndustry("DIRECT_MARKETING");
                        break;
                    case PaymentAccounts.Judicial:
                        break;
                    default:
                        break;
                }

                switch (_method)
                {
                    case PaymentMethods.Web:
                        request.setTransactionConditionCode(5);
                        break;
                    case PaymentMethods.Phone:
                        request.setTransactionConditionCode(2);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                throw new ArgumentException("Unable to create request");
            }
            #endregion

            #region PROCESS REQUEST
            string payprosToken = (payment.IsTest ? testTokens[_account] : accountTokens[_account]);
            CreditCardResponse response = (CreditCardResponse)TransactionClient.doTransaction(request, payprosToken);
            payment.responseCode = (payment.IsTest ? (-1 * response.getResponseCode()) : response.getResponseCode());//flip sign of response code for test payments (keeps it from being written for processing)
            payment.responseText = response.getResponseCodeText();
            payment.retryRecommended = response.getRetryRecommended();
            payment.timestamp = response.getTimeStamp();
            #endregion

            #region RECORD RESPONSE
            double authorizedAmount = 0;
            if (double.TryParse(response.getAuthorizedAmount(), out authorizedAmount)) payment.PaymentAmount = (decimal)authorizedAmount;

            long orderID;
            if (long.TryParse(response.getOrderId(), out orderID)) payment.orderID = orderID;

            long batchID;
            if (long.TryParse(response.getBatchId(), out batchID)) payment.batchID = batchID;

            long bankApprovalCode;
            if (long.TryParse(response.getBankApprovalCode(), out bankApprovalCode)) payment.bankApprovalCode = bankApprovalCode;

            long bankTransactionId;
            if (long.TryParse(response.getBankTransactionId(), out bankTransactionId)) payment.bankTransactionId = bankTransactionId;

            int creditCardVerification;
            if (int.TryParse(response.getCreditCardVerificationResponse(), out creditCardVerification)) payment.creditCardVerificationResponse = creditCardVerification;
            #endregion

            return payment;
        }
    }
}

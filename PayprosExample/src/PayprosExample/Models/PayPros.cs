using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayprosExample.Models
{
    public class PayPros
    {
        public int ID { get; set; }
        public long? bankApprovalCode { get; set; }
        public long? bankTransactionId { get; set; }
        public long? batchID { get; set; }
        [Required]
        [MaxLength(2)]
        public string CCExpireMonth { get; set; }
        [Required]
        [MaxLength(4)]
        public string CCExpireYear { get; set; }
        [Required]
        [MaxLength(4)]
        public string CCLast4 { get; set; }
        [MaxLength(4)]
        public string CCV { get; set; }
        public int? creditCardVerificationResponse { get; set; }
        [MaxLength(255)]
        public string NameOnCard { get; set; }
        public long? orderID { get; set; }
        [Required]
        [MaxLength(50)]
        public string PaymentAccount { get; set; }
        public decimal PaymentAmount { get; set; }
        [Required]
        [MaxLength(255)]
        public string RequestingApplication { get; set; }
        [MaxLength(50)]
        public string RequestingOrigin { get; set; }
        public int? responseCode { get; set; }
        [MaxLength(250)]
        public string responseText { get; set; }
        public bool retryRecommended { get; set; }
        public DateTime? timestamp { get; set; }
        
        [NotMapped]
        public string CreditCardNumber { get; set; }
        [NotMapped]
        public bool IsTest { get; set; }
    }
}
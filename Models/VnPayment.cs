using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebApp.Models;

[Table("VnPayment")]
public class VnPayment
{
    [Key]
    public int Id { get; set; } 

    [BindProperty(Name = "vnp_Amount")] 
    public long Amount { get; set; }
    [BindProperty( Name = "vnp_BankCode")]
    public string BankCode { get; set; } = null!;
    [BindProperty(Name = "vnp_BankTranNo")]
    public string BankTranNo { get; set; } = null!;
    [BindProperty(Name = "vnp_CardType")]
    public string CardType { get; set; } = null!;
    [BindProperty(Name = "vnp_OrderInfo")]
    public string OrderInfo { get; set; } = null!;
    [BindProperty(Name = "vnp_PayDate")]
    public string PayDate { get; set; } = null!;
    [BindProperty(Name = "vnp_ResponseCode")]
    public string ResponseCode { get; set; } = null!;
    [BindProperty(Name = "vnp_TmnCode")]
    public string TmnCode { get; set; } = null!;
    [BindProperty(Name = "vnp_TransactionNo")]
    public string TransactionNo { get; set; } = null!;
    [BindProperty(Name = "vnp_TransactionStatus")]
    public string TransactionStatus { get; set; } = null!;
    [BindProperty(Name = "vnp_TxnRef")]
    public string TxnRef { get; set; } = null!;
    [BindProperty(Name = "vnp_SecureHash")]
    public string SecureHash { get; set; } = null!;
}
//vnp_Amount=22000000&vnp_BankCode=NCB&vnp_BankTranNo=VNP14634229&vnp_CardType=ATM&vnp_OrderInfo=Payment+for+27672543+with+amount+220000.00&vnp_PayDate=20241027004403&vnp_ResponseCode=00&vnp_TmnCode=G0NOWU5F&vnp_TransactionNo=14634229&vnp_TransactionStatus=00&vnp_TxnRef=27672543&vnp_SecureHash=797ae84c0486839b2b44d073984f676d96c4a3748078a1680db5409cf5ac87fbc745a4496ebc4dd02e72088e25393503f8337bede1836fedbee302d688827f25

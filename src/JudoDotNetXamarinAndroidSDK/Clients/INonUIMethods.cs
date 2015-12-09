using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using JudoPayDotNet.Models;

namespace JudoDotNetXamarinSDK.Clients
{
    /// <summary>
    /// Provides a set of operations that receive all the information gathered by the developer app and directly interact with JudoPay backend server
    /// </summary>
    public interface INonUIMethods
    {
        /// <summary>
        /// Does a payment
        /// </summary>
        /// <param name="context">The Android context.</param>
        /// <param name="judoId">The judoId.</param>
        /// <param name="currency">The currency.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="paymentReference">The payment reference (YourPaymentReference).</param>
        /// <param name="consumerReference">The consumer reference (YourConsumerReference).</param>
        /// <param name="metaData">The metadata.</param>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="postCode">The post code.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="expiryDate">The expiry date.</param>
        /// <param name="cv2">The CV2.</param>
        /// <returns>The transaction result of the operation.</returns>
        Task<IResult<ITransactionResult>> Payment(Context context, string judoId,
            string currency, decimal amount, string paymentReference,
            string consumerReference, IDictionary<string, string> metaData, string cardNumber,
            string postCode, string startDate, string expiryDate, string cv2);

        /// <summary>
        /// Does a payment using a saved card
        /// </summary>
        /// <param name="context">The Android context.</param>
        /// <param name="judoId">The judoId.</param>
        /// <param name="currency">The currency.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="paymentReference">The payment reference (YourPaymentReference).</param>
        /// <param name="consumerToken">The consumer token.</param>
        /// <param name="consumerReference">The consumer reference (YourConsumerReference).</param>
        /// <param name="metaData">The meta data.</param>
        /// <param name="cardToken">The card token.</param>
        /// <param name="cv2">The CV2.</param>
        /// <returns>The transaction result of the operation.</returns>
        Task<IResult<ITransactionResult>> TokenPayment(Context context, string judoId,
            string currency, decimal amount, string paymentReference,
            string consumerToken, string consumerReference, IDictionary<string, string> metaData,
            string cardToken, string cv2);

        /// <summary>
        /// Does a pre authorization of funds
        /// </summary>
        /// <param name="context">The Android context.</param>
        /// <param name="judoId">The judoId.</param>
        /// <param name="currency">The currency.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="paymentReference">The payment reference (YourPaymentReference).</param>
        /// <param name="consumerReference">The consumer reference (YourConsumerReference).</param>
        /// <param name="metaData">The metadata.</param>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="postCode">The post code.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="expiryDate">The expiry date.</param>
        /// <param name="cv2">The CV2.</param>
        /// <returns>The transaction result of the operation.</returns>
        Task<IResult<ITransactionResult>> PreAuth(Context context, string judoId,
            string currency, decimal amount, string paymentReference,
            string consumerReference, IDictionary<string, string> metaData, string cardNumber,
            string postCode, string startDate, string expiryDate, string cv2);

        /// <summary>
        /// Does a pre authorization of funds using a saved card
        /// </summary>
        /// <param name="context">The Android context.</param>
        /// <param name="judoId">The judoId.</param>
        /// <param name="currency">The currency.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="paymentReference">The payment reference (YourPaymentReference).</param>
        /// <param name="consumerToken">The consumer token.</param>
        /// <param name="consumerReference">The consumer reference (YourConsumerReference).</param>
        /// <param name="metaData">The meta data.</param>
        /// <param name="cardToken">The card token.</param>
        /// <param name="cv2">The CV2.</param>
        /// <returns>The transaction result of the operation.</returns>
        Task<IResult<ITransactionResult>> TokenPreAuth(Context context, string judoId,
            string currency, decimal amount, string paymentReference,
            string consumerToken, string consumerReference, IDictionary<string, string> metaData,
            string cardToken, string cv2);

        /// <summary>
        /// Registers the card for a consumer.
        /// </summary>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="cv2">The CV2.</param>
        /// <param name="expiryDate">The expiry date.</param>
        /// <param name="consumerReference">The consumer reference (YourConsumerReference).</param>
        /// <param name="postCode">The post code.</param>
        /// <returns>The transaction result of the operation.</returns>
        Task<IResult<ITransactionResult>> RegisterCard(string cardNumber, string cv2,
            string expiryDate, string consumerReference, string postCode);
    }
}
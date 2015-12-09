using System.Collections.Generic;
using Android.Content;
using JudoDotNetXamarinSDK.Models;

namespace JudoDotNetXamarinSDK.Clients
{
    /// <summary>
    /// Provides a set of operations that will receive some parameters and prompt a UI enclosed in this SDK to request the consumer the rest of parameters needed to complete the operations
    /// </summary>
    public interface IUIMethods
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
        /// <returns>The transaction result of the operation.</returns>
        Intent Payment(Context context, string judoId, string currency, string amount,
            string paymentReference, string consumerReference, Dictionary<string, string> metaData);

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
        /// <returns>The transaction result of the operation.</returns>
        Intent PreAuth(Context context, string judoId, string currency, string amount,
            string paymentReference, string consumerReference, Dictionary<string, string> metaData);

        /// <summary>
        /// Payment with saved card.
        /// </summary>
        /// <param name="context">The Android context.</param>
        /// <param name="judoId">The judoId.</param>
        /// <param name="currency">The currency.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="paymentReference">The payment reference (YourPaymentReference).</param>
        /// <param name="consumerReference">The consumer reference (YourConsumerReference).</param>
        /// <param name="cardToken">The card token.</param>
        /// <param name="metaData">The metadata.</param>
        /// <param name="consumerToken">The consumer token.</param>
        /// <returns>The transaction result of the operation.</returns>
        Intent TokenPayment(Context context, string judoId, string currency, string amount,
            string paymentReference, string consumerReference, CardToken cardToken, Dictionary<string, string> metaData,
            string consumerToken = null);

        /// <summary>
        /// Pre authorization of funds with saved card.
        /// </summary>
        /// <param name="context">The Android context.</param>
        /// <param name="judoId">The judoId.</param>
        /// <param name="currency">The currency.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="paymentReference">The payment reference (YourPaymentReference).</param>
        /// <param name="consumerReference">The consumer reference (YourConsumerReference).</param>
        /// <param name="cardToken">The card token.</param>
        /// <param name="metaData">The metadata.</param>
        /// <param name="consumerToken">The consumer token.</param>
        /// <returns>The transaction result of the operation.</returns>
        Intent TokenPreAuth(Context context, string judoId, string currency, string amount,
            string paymentReference, string consumerReference, CardToken cardToken, Dictionary<string, string> metaData,
            string consumerToken = null);

        /// <summary>
        /// Registers the card of a consumer.
        /// </summary>
        /// <param name="context">The Android context.</param>
        /// <param name="consumerReference">The consumer reference (YourConsumerReference).</param>
        /// <returns></returns>
        Intent RegisterCard(Context context, string consumerReference);
    }
}
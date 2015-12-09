using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using JudoDotNetXamarinSDK.Models;
using JudoDotNetXamarinSDK.Ui;
using JudoDotNetXamarinSDK.Utils;
using JudoPayDotNet.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JudoDotNetXamarinSDK.Activies
{
    public static class BuildConfig
    {
#if DEBUG
        public const bool DEBUG = true;
#else
        public const bool DEBUG = false;
#endif
    }

    public abstract class BaseActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.IceCreamSandwich)
            {
                RequestWindowFeature(WindowFeatures.NoTitle);
            }
            else
            {
                SetTheme(Resource.Style.Theme_Judo_payments);

                if (BuildConfig.DEBUG)
                {
                    Window.SetFlags(WindowManagerFlags.Secure, WindowManagerFlags.Secure);
                }
            }
        }

        public void SetHelpText(int titleId, int messageId)
        {
            SetHelpText(titleId, messageId, Resource.Id.infoButtonID);
        }

        public void SetHelpText(int titleId, int messageId, int helpButtonResId)
        {
            String title = GetString(titleId);
            String message = GetString(messageId);
            SetHelpText(title, message, helpButtonResId);
        }

        public void SetHelpText(string title, string message)
        {
            SetHelpText(title, message, Resource.Id.infoButtonID);
        }

        public void SetHelpText(string title, string message, int helpButtonResId)
        {
            HelpButton infoButton = FindViewById<HelpButton>(helpButtonResId);

            if (infoButton == null)
            {
                return;
            }

            infoButton.HelpClickListener += help =>
            {
                ShowMessage(title, message);
            };
        }

        protected void ShowMessage(string message)
        {
            ShowMessage(null, message);
        }

        protected void ShowMessage(string title, string message)
        {
            RunOnUiThread(() => ShowConfirmation(title, message));
        }

        public void ShowConfirmation(string title, string message)
        {
            ShowConfirmation(title, message, true, null, null);
        }

        public void ShowConfirmation(string title, string message, bool cancelable, string buttonLabel, Action buttonAction)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            if (!String.IsNullOrWhiteSpace(title))
            {
                builder.SetTitle(title);
            }

            builder.SetMessage(message);
            builder.SetCancelable(cancelable);

            var dialog = builder.Create();

            if (buttonLabel == null)
            {
                builder.SetPositiveButton(Resource.String.ok, (sender, args) => dialog.Dismiss());
            }
            else
            {
                builder.SetPositiveButton(buttonLabel,
                    (sender, args) =>
                    {
                        if (buttonAction != null)
                        {
                            Task.Factory.StartNew(buttonAction);
                        }
                        dialog.Dismiss();
                    });

                if (cancelable)
                {
                    builder.SetNegativeButton(Resource.String.cancel, (sender, args) => dialog.Dismiss());
                }

            }

            builder.Show();
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);

            View cancelButton = FindViewById<View>(Resource.Id.cancelButton);

            if (cancelButton != null)
            {
                cancelButton.Click += (sender, args) =>
                {
                    SetResult(JudoSDKManager.JUDO_CANCELLED);
                    Finish();
                };
            }
        }

        protected abstract void ShowLoadingSpinner(bool show);

        protected void TransactClickHandler(Action DoTransaction)
        {
            try
            {
                DoTransaction();
            }
            catch (Exception e)
            {
                Log.Error(JudoSDKManager.DEBUG_TAG, "Exception", e);
                SetResult(JudoSDKManager.JUDO_ERROR,
                    JudoSDKManager.CreateErrorIntent(e.Message, e, null));
                Finish();
            }
        }

        protected void HandleServerResponse(Task<IResult<ITransactionResult>> t)
        {
            try
            {
                ShowLoadingSpinner(false);

                if (t.IsFaulted || t.Result == null || t.Result.HasError)
                {
                    var errorMessage = !t.IsFaulted && t.Result != null
                        ? t.Result.Error.ErrorMessage
                        : t.Exception.ToString();
                    Log.Error("com.judopay.android", "ERROR: " + errorMessage);
                    SetResult(JudoSDKManager.JUDO_ERROR,
                        JudoSDKManager.CreateErrorIntent(errorMessage, t.Exception,
                            !t.IsFaulted && t.Result != null ? t.Result.Error : null));
                    Finish();
                    return;
                }

                var receipt = t.Result.Response;

                Intent intent = new Intent();
                intent.PutExtra(JudoSDKManager.JUDO_RECEIPT, new Receipt(receipt));
                SetResult(JudoSDKManager.JUDO_SUCCESS, intent);
                Log.Debug("com.judopay.android", "SUCCESS: " + receipt.Result);
                Finish();
            }
            //Prevent being locked in a payment screen without being notified of an error
            catch (Exception e)
            {
                var errorMessage = e.ToString();
                Log.Error("com.judopay.android", "ERROR: " + errorMessage);
                SetResult(JudoSDKManager.JUDO_ERROR,
                    JudoSDKManager.CreateErrorIntent(errorMessage, e, null));
                Finish();
                return;
            }
        }
    }
}
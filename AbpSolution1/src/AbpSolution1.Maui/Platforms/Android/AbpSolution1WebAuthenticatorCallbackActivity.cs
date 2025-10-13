using Android.App;
using Android.Content;
using Android.Content.PM;

namespace AbpSolution1.Maui.Platforms.Android;

[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
[IntentFilter(new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataScheme = CALLBACK_SCHEME)]
public class AbpSolution1WebAuthenticatorCallbackActivity : WebAuthenticatorCallbackActivity
{
   public const string CALLBACK_SCHEME = "abpsolution1";
}
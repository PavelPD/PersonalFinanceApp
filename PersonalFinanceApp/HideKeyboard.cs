using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceApp
{
    public static class HideKeyboard
    {
        public static void Hide()
        {
#if ANDROID
        var imm =
            (Android.Views.InputMethods.InputMethodManager)MauiApplication.Current.GetSystemService(Android.Content.Context.InputMethodService);
        if(imm != null)
        {
            var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            Android.OS.IBinder wToken = activity.CurrentFocus?.WindowToken;
            imm.HideSoftInputFromWindow(wToken, 0);
        }
#endif
        }
    }
}

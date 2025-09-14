using CmrGameFramework.Procedure;
using CmrUnityGameFramework.Runtime;
using System.Collections;
using UnityEngine;

namespace CmrGame
{
    public static class UIExtension
    {
        public static IEnumerator FadeToAlpha(this CanvasGroup canvasGroup, float alpha, float duration)
        {
            float time = 0f;
            float originalAlpha = canvasGroup.alpha;
            while (time < duration)
            {
                time += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(originalAlpha, alpha, time / duration);
                yield return new WaitForEndOfFrame();
            }

            canvasGroup.alpha = alpha;
        }
        public static int OpenUIForm(this UIComponent uiComponent, EUIForm formEnum,object userdata =null)
        {
            DRUIForm uiform = GFEntry.DataTable.GetDataTable<DRUIForm>().GetDataRow((int)formEnum);
            return GFEntry.UI.OpenUIForm(GFEntry.GlobalSettings.Asset.GetUIFormAsset(uiform.AssetName),
                uiform.UIGroupName, Constant.AssetPriority.UIFormAsset, userData: userdata);
        }
        public static void OpenDialog(this UIComponent uiComponent, DialogParams dialogParams)
        {
            uiComponent.OpenUIForm(EUIForm.DialogForm, dialogParams);
        }
    }
}

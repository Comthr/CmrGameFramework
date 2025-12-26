using CmrGameFramework.Procedure;
using CmrUnityFramework.Runtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
        public static IEnumerator SmoothColor(this MaskableGraphic graphic,Color color,float duration)
        {
            float time = 0f;
            Color originalColor = graphic.color;
            while (time < duration)
            {
                time += Time.deltaTime;
                graphic.color = Color.Lerp(originalColor, color, time / duration);
                yield return new WaitForEndOfFrame();
            }
            graphic.color = color;
            yield return null;
        }
        public static IEnumerator SmoothColor(this Text text,float alpha,float duration)
        {
            float time = 0f;
            Color color = text.color;
            float originalAlpha = color.a;
            while (time < duration)
            {
                time += Time.deltaTime;
                color.a = Mathf.Lerp(originalAlpha, alpha, time / duration);
                text.color = color;
                yield return new WaitForEndOfFrame();
            }
            color.a = alpha;
            text.color = color;
            yield return null;
        }
        public static IEnumerator SmoothTextSize(this Text text,int size,float duration)
        {
            float time = 0f;
            float originalAlpha = text.fontSize;
            while (time < duration)
            {
                time += Time.deltaTime;
                text.fontSize = (int)Mathf.Lerp(originalAlpha, size, time / duration);
                yield return new WaitForEndOfFrame();
            }
            text.fontSize = size;
            yield return null;
        }
        public static int OpenUIForm(this UIComponent uiComponent, EUIForm formEnum,object userdata =null)
        {
            DRUIForm uiform = GameEntry.DataTable.GetDataTable<DRUIForm>().GetDataRow((int)formEnum);
            return GameEntry.UI.OpenUIForm(Constant.Asset.GetGlobalUIFormAsset(uiform.AssetName),
                uiform.UIGroupName, userData: userdata);
        }
        public static void OpenDialog(this UIComponent uiComponent, DialogParams dialogParams)
        {
            uiComponent.OpenUIForm(EUIForm.DialogForm, dialogParams);
        }
    }
}

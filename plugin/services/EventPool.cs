using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Warborn;

namespace JapaneseMod.services
{
    /**
     * 元々のLanguageChangedHandlerとは別に作る、パッチ独自のイベント管理クラス。
     * ConditionWeakTableなのでキー破棄時の処理は必要になるまで書かない(というか、Viewに直接OnDestroyをパッチできないのでやりづらい)
     */
    internal class EventPool
    {
        private ConditionalWeakTable<View, LocalizationManager.LanguageChangedHandler> languageChangedHandlers = new();
        private List<WeakReference<View>> registeredViews = new();

        internal void AddLanguageChangedHandler(View view, LocalizationManager.LanguageChangedHandler handler)
        {
            languageChangedHandlers.Add(view, handler);
            registeredViews.Add(new WeakReference<View>(view));
        }

        internal void RemoveLanguageChangedHandler(View view)
        {
            languageChangedHandlers.Remove(view);
            registeredViews.RemoveAll(weakRef => !weakRef.TryGetTarget(out var target) || target == view);
        }

        internal void LanguageChanged()
        {
            // 生存中のView
            var liveViews = new List<WeakReference<View>>();
            // 呼び出しをシンプルにするためにハンドラをリスト化
            var handlersToInvoke = new List<LocalizationManager.LanguageChangedHandler>();

            foreach (var weakRef in registeredViews)
            {
                if (weakRef.TryGetTarget(out var view) && view != null && languageChangedHandlers.TryGetValue(view, out var handler))
                {
                    handlersToInvoke.Add(handler);
                    liveViews.Add(weakRef);
                }
            }
            registeredViews = liveViews;

            foreach (var handler in handlersToInvoke)
            {
                handler?.Invoke();
            }
        }

    }
}

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

        internal void LanguageChanged()
        {
            // ハンドラを走査し、生きているビューのみを残す
            var liveViews = new List<WeakReference<View>>();
            foreach (var weakRef in registeredViews)
            {
                if (weakRef.TryGetTarget(out var view) && view != null && languageChangedHandlers.TryGetValue(view, out var handler))
                {
                    handler?.Invoke();
                    liveViews.Add(weakRef);
                }
            }
            registeredViews = liveViews;
        }

    }
}

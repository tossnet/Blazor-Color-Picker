using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorColorPicker
{
    // This class provides an example of how JavaScript functionality can be wrapped
    // in a .NET class for easy consumption. The associated JavaScript module is
    // loaded on demand when first needed.
    //
    // This class can be registered as scoped DI service and then injected into Blazor
    // components for use.

    public sealed class JsInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public JsInterop(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/BlazorColorPicker/colorpicker.js").AsTask());
        }

        public async ValueTask<string> SetFocusTo(ElementReference element)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<string>("SetFocusTo2", element);
        }

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}

using ImGuiNET;
using System.Runtime.InteropServices;

namespace Celeste.ImGuiNET;

public static class ImGuiUtils
{
    public static bool SetDragDropPayload<T>(ReadOnlySpan<char> type, T obj)
        where T : unmanaged
    {
        var handle = GCHandle.Alloc(obj, GCHandleType.Pinned);
        var result = ImGui.SetDragDropPayload(type, handle.AddrOfPinnedObject(), sizeof(int));
        handle.Free();

        return result;
    }

    public static unsafe T? AcceptDragDropPayload<T>(ReadOnlySpan<char> type)
        where T : unmanaged
    {
        var payload = ImGui.AcceptDragDropPayload(type);

        if (payload.NativePtr != null)
        {
            var data = (T*)payload.Data;
            return *data;
        }

        return null;
    }
}
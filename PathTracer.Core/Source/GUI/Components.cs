using System;
using System.Collections.Generic;
using System.Text;
using ImGuiNET;

namespace PathTracer.Core.Source.GUI; 
internal class Components {
	/* Helper to display a little (?) mark which shows a tooltip when hovered.
	 * Credit: ImGui Manual - https://pthom.github.io/imgui_manual_online/manual/imgui_manual.html
	 */
	public static void HelpMarker(string desc, bool isInline) {
        if (isInline) ImGui.SameLine();
	    ImGui.TextDisabled("(?)");
        if (ImGui.BeginItemTooltip()) {
            ImGui.PushTextWrapPos(ImGui.GetFontSize()* 35.0f);
            ImGui.TextUnformatted(desc);
            ImGui.PopTextWrapPos();
            ImGui.EndTooltip();
        }
    }
}

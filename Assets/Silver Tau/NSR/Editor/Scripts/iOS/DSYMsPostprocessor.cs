#if UNITY_IOS
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace SilverTau.SafeArea
{
    public static class DSYMsPostprocessor
    {
        private const string PluginSymbolsPath = "Assets/Plugins/iOS/Symbols";

        private static readonly FrameworkSpec[] Frameworks =
        {
            new FrameworkSpec("NSR"),
            new FrameworkSpec("StcCorder"),
        };

        [PostProcessBuild(9999)]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {
            if (target != BuildTarget.iOS) return;

            var src = Path.GetFullPath(PluginSymbolsPath);
            var dst = Path.Combine(path, "Symbols");
            if (Directory.Exists(src))
            {
                if (Directory.Exists(dst)) Directory.Delete(dst, true);
                CopyDir(src, dst);
                UnityEngine.Debug.Log($"[dSYM] Copied vendor dSYM directory {src} -> {dst}");
            }
            else
            {
                UnityEngine.Debug.Log($"[dSYM] Symbols directory not found at {src}. Proceeding without local dSYMs.");
            }

            var pbxPath = PBXProject.GetPBXProjectPath(path);
            var pbx = new PBXProject();
            pbx.ReadFromFile(pbxPath);

#if UNITY_2020_3_OR_NEWER
            var mainTarget = pbx.GetUnityMainTargetGuid();
            var frameworkTarget = pbx.GetUnityFrameworkTargetGuid();
#else
            var mainTarget = pbx.TargetGuidByName("Unity-iPhone");
            var frameworkTarget = mainTarget;
#endif

            // Add ReplayKit framework to UnityFramework target
            pbx.AddFrameworkToProject(frameworkTarget, "ReplayKit.framework", false);

            foreach (var fw in Frameworks)
            {
                var fwName = fw.FrameworkName;
                var execName = string.IsNullOrEmpty(fw.ExecutableName) ? fw.FrameworkName : fw.ExecutableName;
                var cdn = fw.OptionalCdnBaseUrl ?? "";

                var expectedLocal = Path.Combine(src ?? "", $"{fwName}.framework.dSYM");
                if (!string.IsNullOrEmpty(src) && !Directory.Exists(expectedLocal))
                {
                    UnityEngine.Debug.Log($"[dSYM] Local dSYM for {fwName} not found at: {expectedLocal} (will try dsymutil/CDN at archive time).");
                }

                var bash = MakeBash(fwName, execName, cdn);

                pbx.AddShellScriptBuildPhase(
                    mainTarget,
                    $"Ensure {fwName} dSYM (match/generate/fetch)",
                    "/bin/sh",
                    bash);

                if (!string.IsNullOrEmpty(frameworkTarget) && frameworkTarget != mainTarget)
                {
                    pbx.AddShellScriptBuildPhase(
                        frameworkTarget,
                        $"Ensure {fwName} dSYM (match/generate/fetch)",
                        "/bin/sh",
                        bash);
                }
            }

            pbx.WriteToFile(pbxPath);
        }

        private static string MakeBash(string fwName, string execName, string optionalCdnBaseUrl)
        {
            return $@"
set -euo pipefail
echo ""[Vendor dSYM] Ensure {fwName} dSYM in archive...""

APP_WRAPPER=""${{TARGET_BUILD_DIR}}/${{WRAPPER_NAME}}""
FW_BIN=""${{APP_WRAPPER}}/Frameworks/{fwName}.framework/{execName}""
SYMS_SRC=""${{PROJECT_DIR}}/Symbols""
SYMS_DST=""${{DWARF_DSYM_FOLDER_PATH:-}}""

if [ -z ""${{SYMS_DST}}"" ]; then
  echo ""[Vendor dSYM] Non-Archive build. Skip ({fwName}).""; exit 0; fi

if [ ! -f ""${{FW_BIN}}"" ]; then
  echo ""[Vendor dSYM] Framework binary not found for {fwName}: $FW_BIN""; exit 0; fi

uuid_for_arm64() {{ dwarfdump --uuid ""$1"" 2>/dev/null | awk '/arm64/ {{print $2; exit}}'; }}

FW_UUID=""$(uuid_for_arm64 ""$FW_BIN"" || true)""
if [ -z ""$FW_UUID"" ]; then
  echo ""[Vendor dSYM] No arm64 UUID in {fwName} framework (sim-only?).""; exit 0; fi
echo ""[Vendor dSYM] {fwName} arm64 UUID: $FW_UUID""

ensure_name_and_copy() {{
  local dsym_bundle=""$1""
  local dwarf_dir=""${{dsym_bundle}}/Contents/Resources/DWARF""
  local dwarf_file=""${{dwarf_dir}}/{execName}""

  if [ ! -f ""$dwarf_file"" ]; then
    local first_dwarf=""$(/bin/ls ""$dwarf_dir"" 2>/dev/null | head -n1 || true)""
    if [ -n ""$first_dwarf"" ] && [ -f ""$dwarf_dir/$first_dwarf"" ]; then
      dwarf_file=""$dwarf_dir/$first_dwarf""
    else
      echo ""[Vendor dSYM] No DWARF inside dSYM: $dsym_bundle""; return 1
    fi
  fi

  local dsym_uuid=""$(uuid_for_arm64 ""$dwarf_file"" || true)""
  if [ -z ""$dsym_uuid"" ]; then
    echo ""[Vendor dSYM] dSYM has no arm64 slice: $dsym_bundle""; return 1; fi

  echo ""[Vendor dSYM] dSYM arm64 UUID: $dsym_uuid""
  if [ ""$FW_UUID"" != ""$dsym_uuid"" ]; then
    echo ""[Vendor dSYM] UUID mismatch for {fwName} (FW $FW_UUID vs dSYM $dsym_uuid).""; return 1
  fi

  mkdir -p ""$SYMS_DST""
  local out=""${{SYMS_DST}}/{fwName}.framework.dSYM""
  rm -rf ""$out""
  ditto ""$dsym_bundle"" ""$out""

  if [ ""$(basename ""$dwarf_file"")"" != ""{execName}"" ]; then
    rm -f ""$out/Contents/Resources/DWARF/{execName}"" || true
    cp ""$dwarf_file"" ""$out/Contents/Resources/DWARF/{execName}""
  fi

  echo ""[Vendor dSYM] Copied dSYM for {fwName} -> $out""
  return 0
}}

if [ -d ""$SYMS_SRC/{fwName}.framework.dSYM"" ]; then
  echo ""[Vendor dSYM] Trying local dSYM for {fwName}...""
  if ensure_name_and_copy ""$SYMS_SRC/{fwName}.framework.dSYM""; then
    echo ""[Vendor dSYM] Done (local) for {fwName}.""; exit 0
  fi
fi

echo ""[Vendor dSYM] Trying to generate dSYM via dsymutil for {fwName}...""
TMP_OUT=""${{TEMP_DIR}}/{fwName}.framework.dSYM""
rm -rf ""$TMP_OUT""
if dsymutil ""$FW_BIN"" -o ""$TMP_OUT"" >/dev/null 2>&1; then
  if ensure_name_and_copy ""$TMP_OUT""; then
    echo ""[Vendor dSYM] Done (generated) for {fwName}.""
    rm -rf ""$TMP_OUT""
    exit 0
  fi
fi
rm -rf ""$TMP_OUT"" || true

BASE_URL=""{optionalCdnBaseUrl}""
if [ -n ""$BASE_URL"" ]; then
  echo ""[Vendor dSYM] Trying CDN fetch by UUID for {fwName}...""
  ZIP_URL=""$BASE_URL/$FW_UUID.zip""
  TMP_ZIP=""${{TEMP_DIR}}/$FW_UUID.zip""
  if curl -fL ""$ZIP_URL"" -o ""$TMP_ZIP"" >/dev/null 2>&1; then
    unzip -o ""$TMP_ZIP"" -d ""$SYMS_DST"" >/dev/null 2>&1 || true
    rm -f ""$TMP_ZIP""
    if [ -d ""$SYMS_DST/{fwName}.framework.dSYM"" ]; then
      if ensure_name_and_copy ""$SYMS_DST/{fwName}.framework.dSYM""; then
        echo ""[Vendor dSYM] Done (CDN) for {fwName}.""; exit 0
      fi
    fi
  else
    echo ""[Vendor dSYM] CDN has no dSYM for UUID $FW_UUID ({fwName}).""
  fi
fi

echo ""[Vendor dSYM] WARNING: Failed to provide matching dSYM for {fwName}. Archive may warn.""
exit 0
";
        }

        private static void CopyDir(string src, string dst)
        {
            Directory.CreateDirectory(dst);
            foreach (var f in Directory.GetFiles(src))
                File.Copy(f, Path.Combine(dst, Path.GetFileName(f)), true);
            foreach (var d in Directory.GetDirectories(src))
                CopyDir(d, Path.Combine(dst, Path.GetFileName(d)));
        }

        private sealed class FrameworkSpec
        {
            public string FrameworkName { get; }
            public string ExecutableName { get; }
            public string OptionalCdnBaseUrl { get; }

            public FrameworkSpec(string frameworkName, string executableName = null, string optionalCdnBaseUrl = null)
            {
                FrameworkName = frameworkName;
                ExecutableName = executableName;
                OptionalCdnBaseUrl = optionalCdnBaseUrl;
            }
        }
    }
}
#endif

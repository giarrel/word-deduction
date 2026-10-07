using System;
using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using WordDeduction.UI;

namespace WordDeduction.Tests
{
    public class ReleaseScreenTests
    {
        GameObject host; PanelSettings panel; string directory;
        [TearDown] public void Cleanup()
        {
            if (host != null) UnityEngine.Object.Destroy(host);
            if (panel != null) UnityEngine.Object.Destroy(panel);
            if (directory != null && Directory.Exists(directory)) Directory.Delete(directory,true);
        }
        [UnityTest] public IEnumerator InformationIsOptionalReadableAndReturnsToTheUnchangedGroup()
        {
            foreach (var language in new[] { Language.German, Language.English })
            {
                directory = Path.Combine(Application.temporaryCachePath,"release-ui-" + Guid.NewGuid().ToString("N"));
                var session = Session.Open(directory,language); session.AddPlayer("Alex"); session.AddPlayer("Bea"); session.AddPlayer("Cora");
                host = new GameObject("Release information test"); host.SetActive(false);
                panel = UnityEngine.Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/WordDeduction/UI/Panel.asset"));
                panel.scaleMode = PanelScaleMode.ConstantPixelSize; panel.scale = 1;
                var document = host.AddComponent<UIDocument>(); document.panelSettings = panel;
                host.AddComponent<GroupScreen>().Initialize(session); host.SetActive(true);
                var root = document.rootVisualElement; root.style.width = 360; root.style.height = 640;
                yield return null; yield return null;
                root.Q<TextField>("nameInput").value = "Unsaved draft";
                var entry = root.Q<Button>("appInfo");
                Assert.That(entry,Is.Not.Null,"Privacy and licenses must be reachable before starting a match.");
                Assert.That(entry.worldBound.height,Is.GreaterThanOrEqualTo(48));
                Submit(entry); yield return null; yield return null;
                Assert.That(root.Q<Label>("privacyText").text,Does.Contain(language == Language.German ? "Handy" : "phone"));
                root.AddToClassList("large-type");
                foreach (var label in root.Query<TextElement>().ToList()) label.style.fontSize = label.resolvedStyle.fontSize * 1.5f;
                yield return null; yield return null;
                var back = root.Q<Button>("closeInfo");
                Assert.That(back.worldBound.xMin,Is.GreaterThanOrEqualTo(0)); Assert.That(back.worldBound.xMax,Is.LessThanOrEqualTo(360));
                Assert.That(back.worldBound.yMax,Is.LessThanOrEqualTo(640));
                Submit(root.Q<Button>("showLicenses")); yield return null; yield return null;
                var notices = root.Q<Label>("licenseText").text;
                Assert.That(notices,Does.Contain("SIL OPEN FONT LICENSE")); Assert.That(notices,Does.Contain("Unicode")); Assert.That(notices,Does.Contain("Newtonsoft"));
                Submit(root.Q<Button>("closeInfo")); yield return null;
                Assert.That(root.Q<TextField>("nameInput").value,Is.EqualTo("Unsaved draft"));
                Assert.That(Session.Open(directory,Language.English).View.Players.Count,Is.EqualTo(3));
                Assert.That(root.Q<Button>("playButton").enabledInHierarchy,Is.True);
                UnityEngine.Object.Destroy(host); UnityEngine.Object.Destroy(panel); Directory.Delete(directory,true);
                host = null; panel = null; directory = null; yield return null;
            }
        }
        static void Submit(VisualElement target)
        {
            Assert.That(target,Is.Not.Null);
            using (var e = NavigationSubmitEvent.GetPooled()) { e.target = target; target.SendEvent(e); }
        }
    }
}

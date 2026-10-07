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
    public class ContentScreenTests
    {
        GameObject host;
        PanelSettings panel;
        string directory;
        [TearDown] public void Cleanup()
        {
            if(host!=null) UnityEngine.Object.Destroy(host);
            if(panel!=null) UnityEngine.Object.Destroy(panel);
            if(directory!=null && Directory.Exists(directory)) Directory.Delete(directory,true);
        }
        [UnityTest]
        public IEnumerator LongBilingualWordsFitTheRealSmallCardAndDisappearOnRelease()
        {
            foreach(var text in new[]{"Sprachnachricht","Nuss-Nougat-Creme","Chocolate hazelnut spread","Pedestrian traffic light","Blood pressure monitor","Rollkragenpullover","Blutdruckmessgerät","Schlittschuhlaufen","Schraubenschlüssel"})
            {
                var language=text.Contains(" ") ? Language.English : Language.German;
                directory=Path.Combine(Application.temporaryCachePath,"content-ui-"+Guid.NewGuid().ToString("N"));
                var session=Session.Open(directory,language, maximum=>maximum-1);
                foreach(var name in new[]{"Alexandra Maximiliane","Bea","Chris"}) session.AddPlayer(name);
                bool found=false;
                for(int draw=0;draw<520 && !found;draw++)
                {
                    session.StartMatch();
                    while(session.Match.Phase==MatchPhase.Handoff)
                    {
                        string owner=session.Match.Owner.Id;
                        found=session.RevealWord(owner)==text; session.HideWord();
                        if(found) break;
                        session.AdvanceHandoff(owner);
                    }
                    if(!found) session.AbandonMatch(session.Match.Id);
                }
                Assert.That(found,Is.True,"Long word must come from an actual Session deal: "+text);
                host=new GameObject("Bilingual content card test"); host.SetActive(false);
                var document=host.AddComponent<UIDocument>();
#if UNITY_EDITOR
                panel=UnityEngine.Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/WordDeduction/UI/Panel.asset"));
#else
                Assert.Ignore("This fixture clones the Editor's production panel; Android validation uses the built scene.");
#endif
                panel.scaleMode=PanelScaleMode.ConstantPixelSize; panel.scale=1;
                document.panelSettings=panel;
                host.AddComponent<GroupScreen>().Initialize(session); host.SetActive(true);
                var root=document.rootVisualElement; root.style.width=360; root.style.height=640;
                yield return null;
                Submit(root.Q<Button>("resumeMatch")); yield return null;
                foreach (bool large in new[]{false,true})
                {
                    if (large)
                    {
                        root.AddToClassList("large-type");
                        foreach(var element in root.Query<TextElement>().ToList())
                            if(element.name!="secretWord" && element.name!="markQuestion" && element.GetFirstAncestorOfType<TextField>()==null) element.style.fontSize=element.resolvedStyle.fontSize*1.5f;
                        root.Q<VisualElement>("safeRoot").style.paddingTop=40; root.Q<VisualElement>("safeRoot").style.paddingBottom=24;
                        yield return null; yield return null;
                    }
                    var hold=root.Q<VisualElement>("holdReveal");
                    using(var e=PointerDownEvent.GetPooled(new Touch{fingerId=0,position=hold.worldBound.center,phase=TouchPhase.Began})) { e.target=hold; hold.SendEvent(e); }
                    yield return null; yield return null;
                    var word=root.Q<Label>("secretWord"); var face=word.parent;
                    Assert.That(word.text,Is.EqualTo(text));
                    Assert.That(word.MeasureTextSize(text,0,VisualElement.MeasureMode.Undefined,0,VisualElement.MeasureMode.Undefined).x,Is.GreaterThan(50),"Production font produces measurable glyphs");
                    Assert.That(word.resolvedStyle.fontSize,Is.GreaterThanOrEqualTo(22),"A fitted secret remains comfortably readable.");
                    foreach(var token in text.Split(' '))
                        Assert.That(word.MeasureTextSize(token,0,VisualElement.MeasureMode.Undefined,0,VisualElement.MeasureMode.Undefined).x,Is.LessThanOrEqualTo(word.contentRect.width+0.5f),token+" remains whole rather than creating an orphan suffix");
                    if(!text.Contains(" "))
                        Assert.That(word.contentRect.height,Is.LessThanOrEqualTo(word.MeasureTextSize(text,0,VisualElement.MeasureMode.Undefined,0,VisualElement.MeasureMode.Undefined).y+1),"A single catalog term fits one complete line.");
                    Assert.That(word.worldBound.xMin,Is.GreaterThanOrEqualTo(face.worldBound.xMin+face.resolvedStyle.paddingLeft-1),text+" left edge");
                    Assert.That(word.worldBound.xMax,Is.LessThanOrEqualTo(face.worldBound.xMax-face.resolvedStyle.paddingRight+1),text+" right edge");
                    Assert.That(word.worldBound.yMax,Is.LessThanOrEqualTo(face.worldBound.yMax-face.resolvedStyle.paddingBottom+1),text+" bottom edge");
                    Assert.That(root.Q<Button>("nextOwner").worldBound.yMax,Is.LessThanOrEqualTo(640),"Next action stays visible");
                    using(var e=PointerUpEvent.GetPooled(new Touch{fingerId=0,position=hold.worldBound.center,phase=TouchPhase.Ended})) { e.target=hold; hold.SendEvent(e); }
                    Assert.That(word.text,Is.Empty,"Release clears the complete long word immediately");
                }
                Cleanup(); host=null; panel=null; directory=null; yield return null;
            }
        }
        static void Submit(VisualElement target)
        {
            Assert.That(target,Is.Not.Null);
            using(var e=NavigationSubmitEvent.GetPooled()) { e.target=target; target.SendEvent(e); }
        }
    }
}

using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace WordDeduction.UI
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class GroupScreen : MonoBehaviour
    {
        Session session;
        VisualElement root;
        TextField nameInput;
        string editingId;
        bool savedPeopleOpen;
        readonly System.Collections.Generic.List<GroupReorder> reorders = new System.Collections.Generic.List<GroupReorder>();
        string renameDraft;
        string noticeCode;
        Rect lastSafeArea;
        float keyboardHeight;
        float nextViewportCheck;
        TouchScreenKeyboard editKeyboard;
        MatchSurface matchSurface;
        MobileBack mobileBack;
        RuntimeTypography typography;
        AccessibleMenu accessibility;
        TextPreferences textPreferences;
        GroupInformation information;
        public UnityEngine.Accessibility.AccessibilityHierarchy Accessibility => accessibility?.Hierarchy;
        public void Initialize(Session value)
        {
            matchSurface?.Dispose(); matchSurface = null;
            information?.Close();
            if (root != null) CloseEditKeyboard();
            editingId = null; renameDraft = null; noticeCode = null; savedPeopleOpen = false;
            nameInput?.SetValueWithoutNotify("");
            session = value;
            if (root != null) { CreateMatchSurface(); Render(); }
        }
        void OnEnable()
        {
            typography = new RuntimeTypography(GetComponent<UIDocument>());
            mobileBack = new MobileBack();
            if (session == null) session = Session.Open(StorageDirectory(), Application.systemLanguage == SystemLanguage.German ? Language.German : Language.English);
            root = GetComponent<UIDocument>().rootVisualElement;
            root.Clear();
            textPreferences = new TextPreferences(root);
            accessibility = new AccessibleMenu(root,() => session.Match?.Language ?? session.View.Language);
            Resources.Load<VisualTreeAsset>("Group").CloneTree(root);
            nameInput = root.Q<TextField>("nameInput");
            information = new GroupInformation(root,() => session.View.Language,RefreshPresentation);
            root.Q<Button>("appInfo").clicked += () => {
                var keyboard = nameInput.textEdition.touchScreenKeyboard; if (keyboard != null) keyboard.active = false;
                nameInput.Blur(); CloseEditKeyboard(); information.Open();
            };
            nameInput.RegisterValueChangedCallback(e => typography?.IncludeNames(new[] { e.newValue }));
            root.Q<Button>("addPlayer").clicked += Add;
            nameInput.RegisterCallback<KeyDownEvent>(e => { if (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter) { Add(); e.StopPropagation(); } });
            root.Q<Button>("german").clicked += () => Apply(session.SetLanguage(Language.German));
            root.Q<Button>("english").clicked += () => Apply(session.SetLanguage(Language.English));
            root.Q<Button>("quickMode").clicked += () => Apply(session.SetMode(GameMode.Quick));
            root.Q<Button>("classicMode").clicked += () => Apply(session.SetMode(GameMode.Classic));
            root.Q<Button>("kingsMode").clicked += () => Apply(session.SetMode(GameMode.Kings));
            root.Q<Button>("lessUndercover").clicked += () => Apply(session.SetKingsUndercoverPreference(session.View.UndercoverCount - 1));
            root.Q<Button>("moreUndercover").clicked += () => Apply(session.SetKingsUndercoverPreference(session.View.UndercoverCount + 1));
            root.Q<Button>("automaticUndercover").clicked += () => Apply(session.SetKingsUndercoverPreference(null));
            root.Q<Button>("whitePreference").clicked += () => Apply(session.SetWhitePreference(!session.View.WhitePreferred));
            root.Q<Button>("undo").clicked += () => Apply(session.UndoRemove());
            root.Q<Button>("resetDamaged").clicked += () => Apply(session.StartFreshAfterDamage());
            root.Q<Button>("playButton").clicked += () => { var keyboard = nameInput.textEdition.touchScreenKeyboard; if (keyboard != null) keyboard.active = false; nameInput.Blur(); Apply(session.StartMatch()); };
            CreateMatchSurface();
            var players = root.Q<ScrollView>("players");
            players.contentViewport.RegisterCallback<GeometryChangedEvent>(_ =>
            {
                if (root == null || editingId == null) return;
                var row = root.Q<VisualElement>("player-" + editingId);
                row?.schedule.Execute(() => { if (row.panel != null) players.ScrollTo(row); });
            });
            Render(); UpdateSafeArea();
            root.RegisterCallback<GeometryChangedEvent>(_ => UpdateSafeArea());
        }
        void CreateMatchSurface() { matchSurface = new MatchSurface(session,root.Q<VisualElement>("safeRoot"),Render,RefreshPresentation); }
        void OnDisable() { CancelReorders(); information = null; textPreferences?.Dispose(); textPreferences = null; accessibility?.Dispose(); accessibility = null; mobileBack?.Dispose(); mobileBack = null; matchSurface?.Dispose(); matchSurface = null; root = null; typography?.Dispose(); typography = null; }
        void OnApplicationFocus(bool focus) { if (!focus) { CancelReorders(); matchSurface?.Pause(true); } else { MobilePrivacy.RefreshMotion(); MobileViewport.ConfigureFrameRate(); } }
        void OnApplicationPause(bool paused) { if (paused) { CancelReorders(); matchSurface?.Pause(true); } else { MobilePrivacy.RefreshMotion(); MobileViewport.ConfigureFrameRate(); } }
        void Update()
        {
            if (root == null) return;
            matchSurface?.Tick(); accessibility?.Tick();
            var keyboard = root.Q<TextField>("renameInput")?.textEdition.touchScreenKeyboard;
            if (keyboard != null && keyboard.status == TouchScreenKeyboard.Status.Visible) editKeyboard = keyboard;
            // Android reports Status.Done for both IME Done and Back. Only an explicit
            // visible action or real Return event confirms a name; hiding the IME never does.
            if (Time.unscaledTime >= nextViewportCheck)
            {
                nextViewportCheck = Time.unscaledTime + 0.1f;
                var height = MobileViewport.KeyboardHeight();
                if (!Mathf.Approximately(height,keyboardHeight)) { keyboardHeight = height; UpdateSafeArea(); }
            }
            if (Screen.safeArea != lastSafeArea) UpdateSafeArea();
            if ((mobileBack?.Consume() ?? false) || (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame))
            {
                CancelReorders();
                if (information != null && information.IsOpen) information.Close();
                else if (editingId != null) CancelEdit();
                else if (session.Match != null) matchSurface.Back();
                else { var nameKeyboard = nameInput.textEdition.touchScreenKeyboard; if (nameKeyboard != null) nameKeyboard.active = false; nameInput.Blur(); }
            }
        }
        void UpdateSafeArea()
        {
            if (Screen.width == 0 || Screen.height == 0) return;
            lastSafeArea = Screen.safeArea;
            var safe = root.Q<VisualElement>("safeRoot");
            // Runtime insets depend on the device, unlike static visual styling in USS.
            float scale = root.resolvedStyle.width / Screen.width;
            if (float.IsNaN(scale) || scale <= 0) return;
            safe.style.paddingTop = (Screen.height - lastSafeArea.yMax) * scale;
            safe.style.paddingBottom = Mathf.Max(lastSafeArea.yMin,keyboardHeight) * scale;
            safe.style.paddingLeft = lastSafeArea.xMin * scale;
            safe.style.paddingRight = (Screen.width - lastSafeArea.xMax) * scale;
            safe.EnableInClassList("compact", root.resolvedStyle.height < 740);
            safe.EnableInClassList("typing", keyboardHeight > 0);
        }
        static string StorageDirectory()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var files = activity.Call<AndroidJavaObject>("getFilesDir"))
                return Path.Combine(files.Call<string>("getAbsolutePath"), "word-deduction");
#else
            return Path.Combine(Application.persistentDataPath, "word-deduction");
#endif
        }
        string T(string key, params object[] args) => Copy.Get(session.View.Language, key, args);
        void Add()
        {
            var result = session.AddPlayer(nameInput.value);
            if (result.Success)
            {
                nameInput.SetValueWithoutNotify("");
                var keyboard = nameInput.textEdition.touchScreenKeyboard;
                if (keyboard != null) keyboard.text = "";
            }
            Apply(result);
            nameInput.Focus();
            if (result.Success) RevealLastPlayer();
        }
        void RevealLastPlayer()
        {
            var scroll = root.Q<ScrollView>("players");
            if (scroll.childCount == 0) return;
            RevealAfterLayout(scroll,root.Q<VisualElement>("player-" + session.View.Players.Last(p => p.Active).Id));
        }
        static void RevealAfterLayout(ScrollView scroll, VisualElement row)
        {
            if (row == null) return;
            EventCallback<GeometryChangedEvent> laidOut = null;
            laidOut = e =>
            {
                if (e.newRect.height <= 0) return;
                row.UnregisterCallback(laidOut);
                // The row and scroller need their new geometry before ScrollTo can
                // calculate the offset, especially on the first overflowing addition.
                row.schedule.Execute(() => scroll.ScrollTo(row));
            };
            row.RegisterCallback(laidOut);
        }
        void Apply(CommandResult result, string successNotice = null)
        {
            noticeCode = result.Success ? result.Notice ?? successNotice : result.Error;
            if (result.Success) { if (editingId != null) CloseEditKeyboard(); editingId = null; renameDraft = null; }
            Render();
        }
        void CloseEditKeyboard()
        {
            var field = root.Q<TextField>("renameInput");
            // The field may already have lost focus by the time Button.clicked runs.
            // Retain the visible native handle only to close it, never to infer a submit.
            var keyboard = editKeyboard ?? field?.textEdition.touchScreenKeyboard;
            if (keyboard != null) keyboard.active = false;
            field?.Blur();
            editKeyboard = null;
        }
        void CancelEdit() { CloseEditKeyboard(); editingId = null; renameDraft = null; Render(); }
        void Render()
        {
            if (root == null) return;
            CancelReorders();
            var view = session.View;
            root.Q<VisualElement>("safeRoot").EnableInClassList("ready",view.ReadyToStart && !view.StorageBlocked);
            root.Q<VisualElement>("safeRoot").EnableInClassList("recovery",view.StorageBlocked);
            typography?.IncludeNames(view.Players.Select(p => p.Name));
            root.Q<VisualElement>("screen").EnableInClassList("hidden",session.Match != null);
            matchSurface?.Render();
            root.Q<Label>("title").text = T("title"); root.Q<Label>("subtitle").text = T("subtitle");
            root.Q<Label>("groupTitle").text = T("group"); root.Q<Label>("count").text = T(view.ActiveCount == 1 ? "countOne" : "count", view.ActiveCount);
            root.Q<VisualElement>("safeRoot").EnableInClassList("has-players",view.Players.Count > 0);
            root.Q<VisualElement>("safeRoot").EnableInClassList("editing",editingId != null);
            root.Q<Label>("emptyTitle").text = T("emptyTitle"); root.Q<Label>("emptyHint").text = T("emptyHint");
            root.Q<Label>("editHint").text = T(view.ActiveCount >= 20 ? "activeCapacity" : view.Players.Count >= 40 ? "groupCapacity" : "editHint");
            root.Q<VisualElement>("safeRoot").EnableInClassList("at-capacity",view.ActiveCount >= 20 || view.Players.Count >= 40);
            nameInput.textEdition.placeholder = T("name"); nameInput.tooltip = T("name");
            root.Q<Button>("addPlayer").tooltip = T("add");
            root.Q<Button>("appInfo").tooltip = T("appInfo");
            root.Q<Button>("quickMode").text = T("quick"); root.Q<Button>("classicMode").text = T("classic");
            root.Q<Button>("kingsMode").text = T("kings");
            root.Q<Button>("quickMode").EnableInClassList("selected",view.Mode == GameMode.Quick);
            root.Q<Button>("classicMode").EnableInClassList("selected",view.Mode == GameMode.Classic);
            root.Q<Button>("kingsMode").EnableInClassList("selected",view.Mode == GameMode.Kings);
            root.Q<Button>("german").EnableInClassList("selected",view.Language == Language.German);
            root.Q<Button>("english").EnableInClassList("selected",view.Language == Language.English);
            root.Q<Button>("german").tooltip = T("german"); root.Q<Button>("english").tooltip = T("english");
            root.Q<Label>("modeDescription").text = view.Mode == GameMode.Kings ? T(view.ReadyToStart ? "kingsRoleMix" : "kingsDescription",view.CivilianCount,view.UndercoverCount) : view.Mode == GameMode.Classic && view.ReadyToStart ? T("roleMix",view.CivilianCount,view.UndercoverCount,view.WhiteCount) : T(view.Mode == GameMode.Quick ? "quickDescription" : "classicDescription");
            root.Q<VisualElement>("kingsSettings").EnableInClassList("hidden",view.Mode != GameMode.Kings);
            root.Q<Label>("kingsUndercoverCount").text = T("kingsUndercoverCount",view.UndercoverCount);
            root.Q<Button>("lessUndercover").tooltip = T("lessUndercover");
            root.Q<Button>("moreUndercover").tooltip = T("moreUndercover");
            root.Q<Button>("lessUndercover").SetEnabled(view.ReadyToStart && view.UndercoverCount > 1 && !view.StorageBlocked);
            root.Q<Button>("moreUndercover").SetEnabled(view.ReadyToStart && view.UndercoverCount < view.KingsUndercoverLimit && !view.StorageBlocked);
            var automatic = root.Q<Button>("automaticUndercover"); automatic.text = T("automaticUndercover"); automatic.tooltip = T("automaticUndercoverHint"); automatic.SetEnabled(view.KingsUndercoverPreference.HasValue && !view.StorageBlocked);
            var adjustment = root.Q<Label>("kingsCountAdjustment"); adjustment.text = T("kingsCountAdjustment",view.UndercoverCount,view.KingsUndercoverPreference); adjustment.EnableInClassList("hidden",!view.KingsCountAdjusted);
            var white = root.Q<Button>("whitePreference");
            white.EnableInClassList("hidden",view.Mode != GameMode.Classic);
            white.text = T(view.ActiveCount < 5 ? (view.WhitePreferred ? "whiteSavedUnavailable" : "whiteUnavailable") : view.WhitePreferred ? "whiteOn" : "whiteOff");
            white.SetEnabled(view.ActiveCount >= 5 && !view.StorageBlocked);
            root.Q<Button>("playButton").text = T("play");
            root.Q<Button>("playButton").SetEnabled(view.ReadyToStart && !view.StorageBlocked);
            root.Q<Label>("startHint").text = T(view.ReadyToStart ? "ready" : view.NeededPlayers == 1 ? "neededOne" : "needed",view.NeededPlayers);
            var list = root.Q<ScrollView>("players"); var oldOffset = list.scrollOffset; list.Clear(); reorders.Clear();
            foreach (var player in view.Players.Where(p => p.Active)) list.Add(PlayerRow(player));
            var saved = view.Players.Where(p => !p.Active).ToArray();
            if (saved.Length > 0)
            {
                var toggle = ActionButton("savedPeople",T(savedPeopleOpen ? "savedPeopleHide" : "savedPeopleShow",saved.Length),"saved-people-toggle", () => { savedPeopleOpen = !savedPeopleOpen; CancelEdit(); });
                list.Add(toggle);
                if (savedPeopleOpen)
                {
                    var hint = new Label(T("savedPeopleHint")) { name = "savedPeopleHint" }; hint.AddToClassList("hint"); list.Add(hint);
                    foreach (var player in saved) list.Add(PlayerRow(player));
                }
            }
            list.scrollOffset = oldOffset;
            if (editingId != null) RevealAfterLayout(list,root.Q<VisualElement>("player-" + editingId));
            list.EnableInClassList("hidden", view.Players.Count == 0);
            root.Q<VisualElement>("emptyState").EnableInClassList("hidden",view.Players.Count > 0);
            var message = noticeCode ?? view.StorageNotice;
            root.Q<VisualElement>("noticePanel").EnableInClassList("hidden", message == null && !view.CanUndo);
            root.Q<Label>("notice").text = message == null ? "" : T(message);
            var undo = root.Q<Button>("undo"); undo.text = T("undo"); undo.EnableInClassList("hidden",!view.CanUndo);
            var reset = root.Q<Button>("resetDamaged"); reset.text = T("startFresh"); reset.EnableInClassList("hidden",view.StorageNotice != "DamagedData");
            nameInput.SetEnabled(!view.StorageBlocked && view.ActiveCount < 20 && view.Players.Count < 40);
            root.Q<Button>("addPlayer").SetEnabled(!view.StorageBlocked && view.ActiveCount < 20 && view.Players.Count < 40);
            RefreshPresentation();
        }
        void RefreshPresentation() { textPreferences?.Refresh(); accessibility?.Refresh(); }
        VisualElement PlayerRow(PlayerView player)
        {
            var row = new VisualElement { name = "player-" + player.Id, usageHints = UsageHints.DynamicTransform };
            row.AddToClassList("player-row"); row.EnableInClassList("paused",!player.Active);
            if (editingId == player.Id)
            {
                row.AddToClassList("editing-row");
                var edit = new TextField { name = "renameInput", value = renameDraft ?? player.Name, tooltip = T("edit",player.DisplayName) }; edit.AddToClassList("name-input"); row.Add(edit);
                edit.RegisterValueChangedCallback(e => { renameDraft = e.newValue; typography?.IncludeNames(new[] { e.newValue }); });
                if (player.Active)
                {
                    var active = session.View.Players.Where(p => p.Active).Select(p => p.Id).ToArray();
                    int index = Array.IndexOf(active,player.Id);
                    var moves = new VisualElement(); moves.AddToClassList("move-actions"); row.Add(moves);
                    var up = ActionButton("movePlayerUp",T("movePlayerUp"),"move-player", () => MovePlayer(player.Id,index - 1));
                    var down = ActionButton("movePlayerDown",T("movePlayerDown"),"move-player", () => MovePlayer(player.Id,index + 1));
                    up.SetEnabled(index > 0 && !session.View.StorageBlocked); down.SetEnabled(index < active.Length - 1 && !session.View.StorageBlocked);
                    moves.Add(up); moves.Add(down);
                }
                if (!player.Active)
                {
                    var restore = ActionButton("restorePlayer",T("restorePlayer"),"save-button", () => Apply(session.SetParticipation(player.Id,true)));
                    restore.SetEnabled(session.View.ActiveCount < 20 && !session.View.StorageBlocked); row.Add(restore);
                    if (session.View.ActiveCount >= 20) { var capacity = new Label(T("activeCapacity")); capacity.AddToClassList("hint"); row.Add(capacity); }
                }
                var actions = new VisualElement(); actions.AddToClassList("edit-actions"); row.Add(actions);
                actions.Add(ActionButton("removePlayer",T("remove"),"remove-button", () => Apply(session.RemovePlayer(player.Id), "Removed")));
                actions.Add(ActionButton("cancelRename",T("cancel"),"text-button", CancelEdit));
                actions.Add(ActionButton("saveRename",T("save"),"save-button", () => Apply(session.RenamePlayer(player.Id, edit.value))));
                edit.RegisterCallback<KeyDownEvent>(e => { if (e.keyCode == KeyCode.Return) { Apply(session.RenamePlayer(player.Id, edit.value)); e.StopPropagation(); } });
                edit.schedule.Execute(() => { edit.Focus(); edit.SelectAll(); });
            }
            else
            {
                if (player.Active)
                {
                    var handle = new Label("≡") { name = "reorder-" + player.Id, tooltip = T("reorderPlayer",player.DisplayName) };
                    handle.AddToClassList("reorder-handle"); handle.SetEnabled(!session.View.StorageBlocked); row.Add(handle);
                    var snapshot = session.View.Players;
                    var active = snapshot.Where(p => p.Active).Select(p => p.Id).ToArray();
                    var expected = snapshot.Select(p => p.Id).ToArray();
                    var reorder = new GroupReorder(root.Q<ScrollView>("players"),row,active,index => MovePlayer(player.Id,index,expected),pointer => matchSurface?.EndCapturedContact(pointer));
                    reorders.Add(reorder); handle.AddManipulator(reorder);
                }
                var initial = new Label(NameText.FirstElement(player.Name).ToUpperInvariant()); initial.AddToClassList("player-initial"); row.Add(initial);
                var name = new Label(player.DisplayName) { name = "name-" + player.Id }; name.AddToClassList("player-name"); row.Add(name);
                var edit = ActionButton("edit-" + player.Id,T("editLabel"),"edit-player", () => { editingId = player.Id; renameDraft = player.Name; Render(); });
                edit.tooltip = T("edit",player.DisplayName); row.Add(edit);
            }
            return row;
        }
        void CancelReorders() { foreach (var reorder in reorders) reorder.Cancel(); }
        void MovePlayer(string id, int destination, string[] expected = null)
        {
            var players = session.View.Players;
            var active = players.Where(p => p.Active).Select(p => p.Id).ToList();
            if (!active.Remove(id) || destination < 0 || destination > active.Count) return;
            active.Insert(destination,id);
            int index = 0;
            var order = players.Select(p => p.Active ? active[index++] : p.Id).ToArray();
            var result = session.ReorderPlayers(expected ?? players.Select(p => p.Id).ToArray(),order);
            noticeCode = result.Success ? null : result.Error;
            Render();
        }
        static Button ActionButton(string name, string text, string css, Action action)
        {
            var button = new Button(action) { name = name, text = text }; button.AddToClassList(css); return button;
        }
    }
}

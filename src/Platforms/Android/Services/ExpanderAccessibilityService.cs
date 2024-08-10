using Android;
using Android.AccessibilityServices;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Accessibility;
using Android.Widget;
using CommunityToolkit.Mvvm.Messaging;
using Expandroid.Models;
using System.Security.Cryptography;
using System.Text.Json;

[Service(Exported = false, Label = "Expandroid", Permission = Manifest.Permission.BindAccessibilityService)]
[IntentFilter(["android.accessibilityservice.AccessibilityService"])]
[MetaData("android.accessibilityservice", Resource = "@xml/accessibility_service")]
public class ExpanderAccessibilityservice : AccessibilityService, Android.Views.View.IOnTouchListener
{
    private Dictionary<string, Match> dict;
    private List<Var> globals;
    private readonly Bundle CursorArgs = new();
    private readonly Bundle TextArgs = new();
    private const string CursorStr = "$|$";
    private WindowManagerLayoutParams layoutParams;
    private Android.Views.View floatView;
    private IWindowManager windowManager;
    private static readonly string[] _separators = [" ", "\n", "\r\n", " ,"];
    private static readonly string[] _formSeparators = [" ", "|", "\r\n", "\n"];
    private static readonly string[] _lineSeparators = ["\r\n", "\n"];
    private float xDown, yDown;
    private LinearLayout rowContainer;
    private string previousOg = "";
    private string previousExpansion = "";
    private string formExpansion = "";
    private string formKey = "";
    private bool skipNextFormEvent = false;
    private int skipCount = 0;

    public override void OnCreate()
    {
        base.OnCreate();
        dict = new();
        WeakReferenceMessenger.Default.Register<AcServiceMessage>(this, (r, m) =>
        {
            var cmd = m.Value.Item1;
            var item = m.Value.Item2;
            if (cmd == "Add")
            {
                if (!string.IsNullOrEmpty(item.Form) || !(string.IsNullOrEmpty(item.Trigger) || string.IsNullOrEmpty(item.Replace)))
                {
                    //dict.AddOrUpdate(item.Trigger, item,
                    //(key, oldValue) => item);
                    dict[item.Trigger] = item;
                }
            }
            else if (cmd == "Quit")
            {
                DisableSelf();
            }
            else if (cmd is not "_")
            {
                dict.Remove(item.Trigger, out var _);
            }
        });
        WeakReferenceMessenger.Default.Register<AcGlobalsMessage>(this, (r, m) =>
        {
            globals = m.Value;
        });
        try
        {
            if (File.Exists(AppSettings.DictPath))
            {
                using var stream = File.OpenRead(AppSettings.DictPath);
                dict = JsonSerializer.Deserialize<Dictionary<string, Match>>(stream);
            }
            else
                dict = new();
            if (File.Exists(AppSettings.GlobalVarsPath))
            {
                using var stream = File.OpenRead(AppSettings.GlobalVarsPath);
                globals = JsonSerializer.Deserialize<List<Var>>(stream);
            }
        }
        catch (Exception)
        {
        }
    }

    public override async void OnAccessibilityEvent(AccessibilityEvent e)
    {
        try
        {
            if (e.Source == null)
                return;
            if (e.Source.ClassName.Equals("android.widget.EditText"))
            {
                var Text = e.Text;
                if (Text != null)
                {
                    string expansionStr = Text[0].ToString();
                    string og = expansionStr; //not modified
                    CheckAndUpdateCursorArgs(expansionStr, sendIfCursorFound: true, e);
                    var arr = expansionStr.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
                    bool send = false;
                    bool storeOg = true;
                    var text = arr[^1];
                    if (previousOg == og)
                    {
                        return;
                    }
                    else if (formExpansion != "")
                    {
                        expansionStr = og.Replace(formKey, formExpansion);
                        storeOg = true;
                        send = true;
                        formExpansion = "";
                    }
                    else if (previousExpansion != "" && previousExpansion[..^1] == og)
                    {
                        expansionStr = previousOg;
                        storeOg = false;
                        send = true;
                    }
                    else if (skipNextFormEvent)
                    {
                        if (skipCount > 1)
                        {
                            skipNextFormEvent = false;
                            skipCount = 0;
                        }
                        skipCount++;
                        return;
                    }
                    else if (dict.TryGetValue(text, out var match))
                    {
                        string replace = match.Replace;
                        var triggerIndex = expansionStr.IndexOf(text);
                        // echo, random, clipboard and date only supported
                        if (match.Word)
                        {
                            //check the start
                            if (triggerIndex == 0)
                            {
                                if (!_separators.Contains(expansionStr[triggerIndex + text.Length].ToString()))
                                {
                                    return;
                                }
                            }
                            //check the start and end
                            else if (!_separators.Contains(expansionStr[triggerIndex - 1].ToString()) || !_separators.Contains(expansionStr[triggerIndex + text.Length].ToString()))
                            {
                                return;
                            }
                        }
                        if (!string.IsNullOrEmpty(match.Form))
                        {
                            string[] formLines = match.Form.Split(_lineSeparators, StringSplitOptions.RemoveEmptyEntries);
                            var replaceDict = new Dictionary<string, string>();
                            foreach (string line in formLines)
                            {
                                LinearLayout row = new(BaseContext)
                                {
                                    Orientation = Orientation.Horizontal
                                };
                                if (line.Contains("[["))
                                {
                                    string[] words = line.Split(_formSeparators, StringSplitOptions.RemoveEmptyEntries);
                                    if (words.Length > 0)
                                    {
                                        foreach (string word in words)
                                        {
                                            if (word.StartsWith("[["))
                                            {
                                                // we need to add a control after checking form_fields
                                                var endIndex = word.IndexOf(']');
                                                var placeholderStr = word[2..endIndex];
                                                FormOption formOption = null;
                                                if (match.Form_Fields is not null && match.Form_Fields.ContainsKey(placeholderStr))
                                                {
                                                    formOption = match.Form_Fields[placeholderStr];
                                                }
                                                if (formOption?.Type == "choice")
                                                {
                                                    var spinner = new Spinner(BaseContext);
                                                    var adapter = new ArrayAdapter<string>(BaseContext, Android.Resource.Layout.SimpleSpinnerDropDownItem, formOption.Values);
                                                    spinner.Adapter = adapter;
                                                    spinner.ItemSelected += (sender, e) =>
                                                    {
                                                        replaceDict[placeholderStr] = formOption.Values[e.Position];
                                                    };
                                                    row.Post(() => row.AddView(spinner));
                                                }
                                                else if (formOption?.Type == "list")
                                                {
                                                    var listView = new Android.Widget.ListView(BaseContext);
                                                    var adapter = new ArrayAdapter<string>(BaseContext, Android.Resource.Layout.SimpleListItem1, formOption.Values);
                                                    listView.Adapter = adapter;
                                                    listView.ItemClick += (sender, e) =>
                                                    {
                                                        replaceDict[placeholderStr] = formOption.Values[e.Position];
                                                    };
                                                    row.Post(() => row.AddView(listView));
                                                }
                                                else
                                                {
                                                    //add edittext widget
                                                    row.Post(() =>
                                                    {
                                                        var et = new EditText(BaseContext)
                                                        {
                                                            Hint = placeholderStr
                                                        };

                                                        et.TextChanged += (sender, e) =>
                                                        {
                                                            var text = e.Text.ToString();
                                                            if (!replaceDict.TryAdd(placeholderStr, text))
                                                            {
                                                                replaceDict[placeholderStr] = text;
                                                            }
                                                        };
                                                        row.AddView(et);
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                AddTextView(row, word);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    AddTextView(row, line);
                                }
                                rowContainer.Post(() =>
                                {
                                    rowContainer.AddView(row);
                                });
                            }
                            var submitButton = new Android.Widget.Button(BaseContext)
                            {
                                Text = "Submit",
                            };
                            submitButton.Click += (sender, ea) =>
                            {
                                // Replace all occurrences of keys with values
                                var formText = match.Form;
                                foreach (var item in replaceDict)
                                {
                                    string key = $"[[{item.Key}]]";
                                    formText = formText.Replace(key, item.Value);
                                }
                                windowManager.RemoveView(floatView);
                                rowContainer.RemoveAllViewsInLayout();
                                formExpansion = formText;
                                formKey = text;
                            };
                            rowContainer.Post(() =>
                            {
                                rowContainer.AddView(submitButton);
                            });
                            windowManager.AddView(floatView, layoutParams);
                        }
                        else
                        {
                            if (globals is not null)
                            {
                                foreach (var item in globals)
                                {
                                    replace = await ParseItemAsync(item, replace);
                                }
                            }
                            if (match.Vars is not null && match.Vars.Count > 0)
                            {
                                foreach (var item in match.Vars)
                                {
                                    replace = await ParseItemAsync(item, replace);
                                }
                            }
                            if (replace is not null)
                            {
                                var end = expansionStr[triggerIndex..].Replace(text, replace);
                                expansionStr = expansionStr[..triggerIndex] + end;
                                send = true;
                            }
                        }
                    }
                    if (send)
                    {
                        DoExpansion(e, expansionStr);
                        if (storeOg)
                        {
                            previousOg = og;
                            previousExpansion = expansionStr;
                        }
                        else
                        {
                            previousOg = "";
                            previousExpansion = "";
                        }
                    }
                }
            }

        }
        catch (Exception ex)
        {

        }
    }

    private void DoExpansion(AccessibilityEvent e, string og)
    {
        TextArgs.Remove(AccessibilityNodeInfo.ActionArgumentSetTextCharsequence);
        TextArgs.PutCharSequence(AccessibilityNodeInfo.ActionArgumentSetTextCharsequence, og);
        e.Source.PerformAction(Android.Views.Accessibility.Action.SetText, TextArgs);
        if (e.Source.Refresh())
        {
            CheckAndUpdateCursorArgs(og, sendIfCursorFound: false, e);
            e.Source.PerformAction(Android.Views.Accessibility.Action.SetSelection, CursorArgs);
        }
    }

    private void AddTextView(LinearLayout row, string word)
    {
        row.Post(() =>
        {
            row.AddView(new TextView(BaseContext)
            {
                Text = word,
            });
        });
    }

    private void CheckAndUpdateCursorArgs(string og, bool sendIfCursorFound, AccessibilityEvent e)
    {
        int startIndex = og.IndexOf(CursorStr);
        CursorArgs.Remove(AccessibilityNodeInfo.ActionArgumentSelectionStartInt);
        CursorArgs.Remove(AccessibilityNodeInfo.ActionArgumentSelectionEndInt);
        if (startIndex != -1)
        {
            CursorArgs.PutInt(AccessibilityNodeInfo.ActionArgumentSelectionStartInt, startIndex);
            CursorArgs.PutInt(AccessibilityNodeInfo.ActionArgumentSelectionEndInt, startIndex + CursorStr.Length);
            if (sendIfCursorFound)
            {
                e.Source.PerformAction(Android.Views.Accessibility.Action.SetSelection, CursorArgs);
            }
        }
        else
        {
            CursorArgs.PutInt(AccessibilityNodeInfo.ActionArgumentSelectionStartInt, og.Length);
            CursorArgs.PutInt(AccessibilityNodeInfo.ActionArgumentSelectionEndInt, og.Length);
        }
    }

    private static async Task<string> ParseItemAsync(Var item, string replace)
    {
        try
        {
            if (item.Type is not null)
            {
                switch (item.Type)
                {
                    case "echo":
                        replace = replace.Replace(WrapName(item.Name), item.Params.Echo);
                        break;
                    case "random":
                        var choices = item.Params.Choices;
                        replace = replace.Replace(WrapName(item.Name), choices[RandomNumberGenerator.GetInt32(0, choices.Count)]);
                        break;
                    case "clipboard":
                        //if (Clipboard.Default.HasText)
                        {
                            var clip = await Clipboard.Default.GetTextAsync();
                            replace = replace.Replace(WrapName(item.Name), clip);
                        }
                        break;
                    case "date":
                        var param = item.Params;
                        var date = (DateTime.Now + TimeSpan.FromSeconds(param.Offset)).ToString(param.Format);
                        replace = replace.Replace(WrapName(item.Name), date);
                        break;
                    default:
                        break;
                }
            }
            return replace;
        }
        catch (Exception)
        {
            return null;
        }
    }
    private static string WrapName(string name)
    {
        return "{{" + name + "}}";
    }
    public override void OnInterrupt()
    {
        //throw new NotImplementedException();
    }

    protected override void OnServiceConnected()
    {
        base.OnServiceConnected();
        WeakReferenceMessenger.Default.Send(new AcServiceMessage(("_", null)));
        var linearLayout = new LinearLayout(this);
        linearLayout.Orientation = Orientation.Vertical;
        layoutParams = new WindowManagerLayoutParams();
        layoutParams.Type = WindowManagerTypes.AccessibilityOverlay;
        layoutParams.Format = Format.Translucent;
        layoutParams.Width = ViewGroup.LayoutParams.WrapContent;
        layoutParams.Height = ViewGroup.LayoutParams.WrapContent;
        layoutParams.Gravity = GravityFlags.Top;
        LayoutInflater inflater = LayoutInflater.From(this);
        floatView = inflater.Inflate(Microsoft.Maui.Resource.Layout.floatview, linearLayout);
        var closeBtn = floatView.FindViewById<Android.Widget.ImageButton>(Microsoft.Maui.Resource.Id.close_button);
        if (closeBtn != null)
        {
            closeBtn.Click += (sender, e) =>
            {
                windowManager.RemoveView(floatView);
                rowContainer.RemoveAllViewsInLayout();
                skipNextFormEvent = true;
            };
            floatView.SetOnTouchListener(this);
            rowContainer = floatView.FindViewById<LinearLayout>(Microsoft.Maui.Resource.Id.rowContainer);
            windowManager = GetSystemService(WindowService).JavaCast<IWindowManager>();
        }
    }

    public override bool OnUnbind(Intent intent)
    {
        //Remove the overlay when the service is unbound
        if (floatView != null)
        {
            IWindowManager windowManager = (IWindowManager)GetSystemService(Context.WindowService);
            windowManager.RemoveView(floatView);
        }
        return base.OnUnbind(intent);
    }
    public bool OnTouch(Android.Views.View v, MotionEvent e)
    {
        var action = e.Action;

        switch (action)
        {
            case MotionEventActions.Down:
                xDown = e.RawX;
                yDown = e.RawY;
                return true;
            case MotionEventActions.Move:
                float deltaX = e.RawX - xDown;
                float deltaY = e.RawY - yDown;

                layoutParams.X += (int)deltaX;
                layoutParams.Y += (int)deltaY;

                windowManager.UpdateViewLayout(floatView, layoutParams);

                xDown = e.RawX;
                yDown = e.RawY;
                return true;
            default:
                return false;
        }
    }
}
